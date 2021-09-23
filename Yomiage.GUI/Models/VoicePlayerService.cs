using Microsoft.Win32;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core.Types;
using Yomiage.Core.Models;
using Yomiage.GUI.Util;
using Yomiage.SDK.Talk;
using Yomiage.SDK.VoiceEffects;
using Reactive.Bindings.Notifiers;
using Yomiage.GUI.EventMessages;
using System.IO;
using NAudio.MediaFoundation;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using Yomiage.GUI.Dialog;

namespace Yomiage.GUI.Models
{
    public class VoicePlayerService
    {
        private IWavePlayer wavPlayer;
        private MMDevice mmDevice;

        const int bufferLimit = 20000;
        const int bytesLimit = 40000;

        /// <summary>
        /// ファイル命名規則を指定して選択するのときに
        /// フォルダが指定されていない。または存在しない場合に一時的にフォルダを選択する用。
        /// </summary>
        private string tempDirectory;
        private DateTime saveTime;

        public ReactiveProperty<bool> IsPlaying { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> CanPlay { get; }

        private VoicePresetService voicePresetService;
        private VoiceEngineService voiceEngineService;
        private TextService textService;
        private SettingService settingService;
        PhraseDictionaryService phraseDictionaryService;
        WordDictionaryService wordDictionaryService;
        PauseDictionaryService pauseDictionaryService;
        IMessageBroker messageBroker;
        public VoicePlayerService(
            SettingService settingService,
            VoicePresetService voicePresetService,
            VoiceEngineService voiceEngineService,
            TextService textService,
            PhraseDictionaryService phraseDictionaryService,
            WordDictionaryService wordDictionaryService,
            PauseDictionaryService pauseDictionaryService,
            IMessageBroker messageBroker)
        {
            this.settingService = settingService;
            this.messageBroker = messageBroker;
            this.voicePresetService = voicePresetService;
            this.voiceEngineService = voiceEngineService;
            this.textService = textService;
            this.phraseDictionaryService = phraseDictionaryService;
            this.wordDictionaryService = wordDictionaryService;
            this.pauseDictionaryService = pauseDictionaryService;
            this.CanPlay = this.IsPlaying.Select(x => !x).ToReactiveProperty();
        }

        public async Task Play(TalkScript script, VoicePreset preset = null)
        {
            if (this.IsPlaying.Value) { return; }
            this.IsPlaying.Value = true;
            int fs = 44100;

            var buffer = new ConcurrentQueue<double[]>();
            var list = await GetVoiceBuffer(script, preset, fs);
            list?.ForEach(l => buffer.Enqueue(l));
            if (buffer.Count == 0) { return; }

            var bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(fs, 16, 1)); //16bit１チャンネルの音源を想定
            var wavProvider = new VolumeWaveProvider16(bufferedWaveProvider);  //ボリューム調整をするために上のBufferedWaveProviderをデコレータっぽく包む
            wavProvider.Volume = 1f;
            this.mmDevice?.Dispose();
            this.mmDevice = new MMDeviceEnumerator()
                .GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);  //再生デバイスと出力先を設定

            InitWaveProvider(wavProvider);
            this.wavPlayer.Play();
            var tempWave = new List<double>();
            while (this.wavPlayer.PlaybackState != PlaybackState.Stopped &&
                  (bufferedWaveProvider.BufferedBytes > 0 || buffer.Count > 0))
            {
                if (bufferedWaveProvider.BufferedBytes < bytesLimit)
                {
                    buffer.TryDequeue(out double[] temp); // 分析結果を取り出す
                    if (temp != null)
                    {
                        if (tempWave.Count > bufferLimit)
                        {
                            tempWave.RemoveRange(0, tempWave.Count - bufferLimit);
                        }
                        tempWave.AddRange(temp);
                        var Bbuffer = Utility.DoubleToByte(temp, 16);
                        bufferedWaveProvider.AddSamples(Bbuffer, 0, Bbuffer.Length); // バッファーを渡す
                    }
                }

                // キャラクターにメッセージを送る
                var part = GetPart(tempWave, tempWave.Count - bufferedWaveProvider.BufferedBytes / 2);
                this.messageBroker.Publish(new PlayingEvent() { part = part, fs = fs });

                await Task.Delay(100);
            }
            await Task.Delay(100);
            await Stop();
        }

        public async Task Play(TalkScript[] scripts, Action<int> SubmitPlayIndex, VoicePreset preset = null)
        {
            if (this.IsPlaying.Value) { return; }
            this.IsPlaying.Value = true;
            int fs = 44100;
            if (scripts == null || scripts.Length == 0) { return; }

            var buffer = new ConcurrentQueue<double[]>();

            var bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(fs, 16, 1)); //16bit１チャンネルの音源を想定
            var wavProvider = new VolumeWaveProvider16(bufferedWaveProvider);  //ボリューム調整をするために上のBufferedWaveProviderをデコレータっぽく包む
            wavProvider.Volume = 1f;
            this.mmDevice?.Dispose();
            this.mmDevice = new MMDeviceEnumerator()
                .GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);  //再生デバイスと出力先を設定

            InitWaveProvider(wavProvider);

            this.wavPlayer.Play();
            var tempWave = new List<double>();
            int scriptIndex = 0;
            List<double[]> nextBuffer = null;
            VoicePreset presetNow = null;
            VoicePreset presetNext = null;
            while (this.wavPlayer.PlaybackState != PlaybackState.Stopped &&
                  (bufferedWaveProvider.BufferedBytes > 0 || buffer.Count > 0 || scriptIndex < scripts.Length))
            {
                if (nextBuffer == null && scriptIndex < scripts.Length)
                {
                    // 次の音声を合成する。
                    nextBuffer = await GetVoiceBuffer(scripts[scriptIndex], preset, fs);
                    presetNext = GetPreset(scripts[scriptIndex], preset);
                    scriptIndex += 1;
                    if (nextBuffer?.Count == 0)
                    {
                        nextBuffer = null;
                    }
                }
                if (nextBuffer != null && buffer.Count <= 1)
                {
                    // 再生中の音声が少なくなったら次の音声を流し込む
                    nextBuffer.ForEach(l => buffer.Enqueue(l));
                    nextBuffer = null;
                    if(SubmitPlayIndex != null)
                    {
                        // 台本のほうに現在の再生位置を送る
                        SubmitPlayIndex(scriptIndex - 1);
                    }
                    presetNow = presetNext;
                    await Task.Delay(500);
                }
                if (bufferedWaveProvider.BufferedBytes < bytesLimit)
                {
                    buffer.TryDequeue(out double[] temp); // 分析結果を取り出す
                    if (temp != null)
                    {
                        if (tempWave.Count > bufferLimit)
                        {
                            tempWave.RemoveRange(0, tempWave.Count - bufferLimit);
                        }
                        tempWave.AddRange(temp);
                        var Bbuffer = Utility.DoubleToByte(temp, 16);
                        bufferedWaveProvider.AddSamples(Bbuffer, 0, Bbuffer.Length); // バッファーを渡す
                    }
                }

                // キャラクターにメッセージを送る
                var part = GetPart(tempWave, tempWave.Count - bufferedWaveProvider.BufferedBytes / 2);
                this.messageBroker.Publish(new PlayingEvent() { part = part, fs = fs, preset = presetNow });

                await Task.Delay(100);
            }
            this.messageBroker.Publish(new PlayingEvent() { preset = null });
            await Task.Delay(100);
            await Stop();
        }

        private async Task<List<double[]>> GetVoiceBuffer(TalkScript script, VoicePreset preset = null, int target_fs = 44100)
        {
            if (script.MoraCount == 0)
            {
                return new List<double[]>();
            }
            var wave = await GetVoice(script, preset, target_fs);
            if (wave == null) { return null; }

            var list = new List<double[]>();
            for (int i = 0; i < wave.Length; i += bufferLimit)
            {
                var end = Math.Min(i + bufferLimit, wave.Length);
                var temp = new double[end - i];
                Array.Copy(wave, i, temp, 0, temp.Length);
                list.Add(temp);
            }
            return list;
        }

        private async Task<double[]> GetVoice(TalkScript script, VoicePreset preset = null, int target_fs = 44100)
        {
            target_fs = Math.Clamp(target_fs, 8000, 80000);
            if (script == null) { return null; }
            preset = GetPreset(script, preset);
            if (preset == null) { return null; }
            int fs = 44100;
            var wave = await preset.Play(script, settingService.GetMasterEffectValue(),
                x => fs = x);

            // サンプリングレート変更　音質が劣化するかもしれないという不安から MediaFoundationEncoder を使っているが必要ないかも
            if (fs == target_fs) { return wave; }

            var reSampledWave = new List<double>();
            SaveWav(wave, "original.wav", fs);
            {
                MediaFoundationReader reader = new MediaFoundationReader("original.wav");
                WaveFormat format = new WaveFormat(target_fs, 16, 1);
                MediaType mediaType = new MediaType(format);

                using (MediaFoundationEncoder encoder = new MediaFoundationEncoder(mediaType))
                {
                    encoder.Encode("resampled.wav", reader);
                }
            }
            {
                using var reader = new WaveFileReader("resampled.wav");
                while (reader.Position < reader.Length)
                {
                    var samples = reader.ReadNextSampleFrame();
                    reSampledWave.Add(samples.First());
                }
            }
            return reSampledWave.ToArray();
        }

        private VoicePreset GetPreset(TalkScript script, VoicePreset preset)
        {
            if (preset == null)
            {
                if (!string.IsNullOrWhiteSpace(script?.PresetName))
                {
                    // プリセットタグから
                    preset = this.voicePresetService.AllPresets.FirstOrDefault(p => p.Name == script?.PresetName);
                }
                if (preset == null)
                {
                    // デフォルトプリセット
                    preset = this.voicePresetService.SelectedPreset.Value;
                }
            }
            return preset;
        }

        private void InitWaveProvider(IWaveProvider wavProvider)
        {
            this.wavPlayer?.Dispose();
            this.wavPlayer = null;
            if (settingService.AudioDefault)
            {
                // 出力デバイスの検索
                for (int i = 0; i < WaveOut.DeviceCount; i++)
                {
                    var capabilities = WaveOut.GetCapabilities(i);
                    if (capabilities.ProductName == settingService.AudioName)
                    {
                        this.wavPlayer = new WaveOut()
                        {
                            DeviceNumber = i,
                        };
                        break;
                    }
                }
            }
            if (wavPlayer == null)
            {
                this.wavPlayer = new WasapiOut(this.mmDevice, AudioClientShareMode.Shared, false, 50);
            }
            this.wavPlayer.Init(wavProvider); //出力に入力を接続
        }

        private double[] GetPart(List<double> wave, int position)
        {
            var part = new double[2048];
            var start = Math.Max(position, 0);
            var end = Math.Min(start + 2048, wave.Count);
            for (int i = start; i < end; i++) { part[i - start] = wave[i]; }
            return part;
        }

        public async Task Save(string text, VoicePreset preset = null)
        {
            using var waitDialog = new WaitDialog();
            waitDialog.Show();

            await Task.Delay(30);

            string[] texts;
            if (settingService.OutputMultiByChar)
            {
                texts = text.Split(settingService.OutputSplitChar);
            }
            else
            {
                texts = new string[] { text };
            }
            var scriptsList = new List<TalkScript[]>();
            foreach (var t in texts)
            {
                var scripts = this.textService.Parse(t, settingService.SplitByEnter, settingService.PromptStringEnable, settingService.PromptString, phraseDictionaryService.SearchDictionary, this.wordDictionaryService.WordDictionarys, this.pauseDictionaryService.PauseDictionary.ToList());
                scriptsList.Add(scripts);
            }
            if (scriptsList.Count <= 0) { return; }

            if (preset == null && this.voicePresetService.SelectedPreset.Value == null) { return; }

            string fileName = null;
            if (settingService.SaveByDialog)
            {
                // ファイル保存ダイアログで選択する
                var sfd = new SaveFileDialog() { Filter = "音声ファイル(.wav) |*.wav| 音声ファイル(.mp3) |*.mp3| 音声ファイル(.wma) |*.wma" };
                if (settingService.OutputModeMp3) { sfd.Filter = "音声ファイル(.mp3) |*.mp3| 音声ファイル(.wav) |*.wav| 音声ファイル(.wma) |*.wma"; }
                if (settingService.OutputModeWma) { sfd.Filter = "音声ファイル(.wma) |*.wma| 音声ファイル(.wav) |*.wav| 音声ファイル(.mp3) |*.mp3"; }
                if (sfd.ShowDialog() != true) { return; }
                fileName = sfd.FileName;
            }

            tempDirectory = settingService.RuleFolderPath;
            if (settingService.SaveByRule && !Directory.Exists(tempDirectory))
            {
                using var cofd = new CommonOpenFileDialog()
                {
                    Title = "音声保存先フォルダの選択",
                    // フォルダ選択モードにする
                    IsFolderPicker = true,
                };
                if (cofd.ShowDialog() != CommonFileDialogResult.Ok) { return; }
                tempDirectory = cofd.FileName;
            }

            saveTime = DateTime.Now;

            if (settingService.OutputMultiFile)
            {
                // １文毎に区切って複数のファイルに書き出す
                int count = 0;
                foreach (var scripts in scriptsList)
                {
                    foreach (var script in scripts)
                    {
                        var p = GetPreset(script, preset);
                        if (await Save(new TalkScript[] { script },
                            FileNameWithNumber(fileName, count, script.OriginalText, p), p))
                        {
                            count += 1;
                        }
                    }
                }
            }
            else if (scriptsList.Count == 1)
            {
                // 一つのファイルに書き出す
                var p = GetPreset(scriptsList.First().First(), preset);
                var allText = string.Join("", scriptsList.First().Select(s =>s.OriginalText ).ToArray());
                await Save(scriptsList.First(), FileNameWithNumber(fileName, -1, allText, p), preset);
            }
            else
            {
                // 指定された文字列で区切って複数のファイルに書き出す
                int count = 0;
                foreach (var scripts in scriptsList)
                {
                    var p = GetPreset(scripts.First(), preset);
                    var allText = string.Join("", scripts.Select(s => s.OriginalText).ToArray());
                    if (await Save(scripts, FileNameWithNumber(fileName, count, allText, p), preset))
                    {
                        count += 1;
                    }
                }
            }

            await Task.Delay(1000);
        }

        private async Task<bool> Save(TalkScript[] scripts, string filePath, VoicePreset preset = null)
        {
            if (string.IsNullOrWhiteSpace(filePath) ||
                scripts == null ||
                scripts.Length <= 0 ||
                scripts.All(s => s.MoraCount <= 0)) { return false; }
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (preset == null && this.voicePresetService.SelectedPreset.Value == null)
            {
                return false;
            }

            var firstPreset = GetPreset(scripts.First(), preset);
            if (scripts.All(s => firstPreset == GetPreset(s, preset)) && // 全て同じプリセットのときのみ、本当は同じエンジンにしたほうがよさそう。
                firstPreset.Engine.EngineConfig.EngineSaveEnable)
            {
                Encoding enc = Encoding.UTF8;
                if (settingService.Encoding.Contains("UTF-16 LE")) { enc = Encoding.Unicode; }
                if (settingService.Encoding.Contains("UTF-16 BE")) { enc = Encoding.BigEndianUnicode; }
                if (settingService.Encoding == "Shift-JIS") { enc = Encoding.GetEncoding("Shift-JIS"); }

                await firstPreset.Save(scripts, settingService.GetMasterEffectValue(), filePath, settingService.StartPause, settingService.EndPause, settingService.SaveWithText, enc);
                return true;
            }

            int fs = 44100;
            var waves = new List<double[]>();
            foreach (var script in scripts)
            {
                if (script.MoraCount <= 0) { continue; }
                var p = GetPreset(script, preset);
                var wave = await p.Play(script, settingService.GetMasterEffectValue(),
                    x => fs = x);
                waves.Add(wave);
            }

            var wave_pause = new List<double>();
            int startPause = fs * settingService.StartPause / 1000;
            if (startPause > 0) { wave_pause.AddRange(new double[startPause]); }
            waves.ForEach(w => wave_pause.AddRange(w));
            int endPause = fs * settingService.EndPause / 1000;
            if (endPause > 0) { wave_pause.AddRange(new double[endPause]); }
            SaveWav(wave_pause.ToArray(), filePath, fs);

            var textPath = Path.ChangeExtension(filePath, ".txt");
            if (settingService.SaveWithText)
            {
                var text = "";
                foreach (var s in scripts)
                {
                    text += settingService.PromptStringOutput ? s.GetOriginalTextWithPresetName(settingService.PromptString) : s.OriginalText;
                }
                SaveText(textPath, text, settingService.Encoding);
            }
            return true;
        }

        private void SaveWav(double[] Dbuffer, string filePath, int fs)
        {
            var tempPath = "temp.wav";
            using (var writer = new WaveFileWriter(tempPath, new WaveFormat(fs, 16, 1)))
            {
                writer.WriteSamples(Dbuffer.Select(v => (float)v).ToArray(), 0, Dbuffer.Length);
            }
            Convert(tempPath, filePath);
        }
        private void SaveText(string filePath, string text, string encoding)
        {
            Encoding enc = Encoding.UTF8;
            if (encoding.Contains("UTF-16 LE")) { enc = Encoding.Unicode; }
            if (encoding.Contains("UTF-16 BE")) { enc = Encoding.BigEndianUnicode; }
            if (encoding == "Shift-JIS") { enc = Encoding.GetEncoding("Shift-JIS"); }
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.WriteAllText(filePath, text, enc);
            }
            catch
            {

            }
        }

        private void Convert(string inputPath, string outputPath)
        {
            var ext = Path.GetExtension(outputPath).ToLower();
            if (ext.Contains("mp3"))
            {
                outputPath = Path.ChangeExtension(outputPath, ".mp3");
                ConvertMp3(inputPath, outputPath, settingService.OutputFormatMp3);
            }
            else if (ext.Contains("wma"))
            {
                outputPath = Path.ChangeExtension(outputPath, ".wma");
                ConvertWma(inputPath, outputPath, settingService.OutputFormatWma);
            }
            else
            {
                outputPath = Path.ChangeExtension(outputPath, ".wav");
                ConvertWav(inputPath, outputPath, settingService.OutputFormatWav);
            }
        }
        private void ConvertWav(string inputPath, string outputPath, string format)
        {
            WaveFormat waveFormat;
            waveFormat = new WaveFormat(GetFs(format), GetBit(format), 1);

            using var reader = new MediaFoundationReader(inputPath);
            MediaType mediaType = new MediaType(waveFormat);
            using var encoder = new MediaFoundationEncoder(mediaType);
            encoder.Encode(outputPath, reader);
        }
        private int GetFs(string format)
        {
            if (string.IsNullOrWhiteSpace(format)) { return 44100; }
            if (format.Contains("48000Hz")) { return 48000; }
            if (format.Contains("32000Hz")) { return 32000; }
            if (format.Contains("22050Hz")) { return 22050; }
            if (format.Contains("16000Hz")) { return 16000; }
            if (format.Contains("11050Hz")) { return 11050; }
            if (format.Contains("8000Hz")) { return 8000; }
            return 44100;
        }
        private int GetBit(string format)
        {
            if (!string.IsNullOrWhiteSpace(format) && format.Contains("8bit"))
            {
                return 8;
            }
            else
            {
                return 16;
            }
        }
        private void ConvertMp3(string inputPath, string outputPath, string format)
        {
            int bps = 128000;
            if (format.Contains("96 kbps")) { bps = 96000; }
            if (format.Contains("48 kbps")) { bps = 48000; }
            using var reader = new MediaFoundationReader(inputPath);
            MediaFoundationEncoder.EncodeToMp3(reader, outputPath, bps);
        }
        private void ConvertWma(string inputPath, string outputPath, string format)
        {
            int bps = 48000;
            if (format.Contains("32 kbps")) { bps = 32000; }
            if (format.Contains("20 kbps")) { bps = 20000; }
            using var reader = new MediaFoundationReader(inputPath);
            MediaFoundationEncoder.EncodeToWma(reader, outputPath, bps);
        }

        public async Task Stop()
        {
            this.wavPlayer?.Stop();
            foreach (var engine in this.voiceEngineService.AllEngines)
            {
                engine.VoiceEngine?.Stop();
            }
            await Task.Delay(100);
            this.mmDevice?.Dispose();
            this.wavPlayer?.Dispose();
            this.IsPlaying.Value = false;
            this.messageBroker.Publish(new StopEvent());
        }

        private string FileNameWithNumber(string fileName, int num, string text, VoicePreset preset)
        {
            var result = "";
            if (fileName == null)
            {
                // ファイル命名規則を指定して選択する
                if (string.IsNullOrWhiteSpace(tempDirectory) || !Directory.Exists(tempDirectory)) { return null; }

                string name = settingService.Rule;

                var Matches = new Regex(@"\{([^\{\}]*)\}").Matches(name).Select(m => m.Value).Distinct().OrderBy(m => m.Length).ToList();
                foreach (var match in Matches)
                {
                    var symbol = match.Substring(1, match.Length - 2);
                    switch (match)
                    {
                        case "{VoicePreset}":
                            name = name.Replace(match, preset.Name);
                            break;
                        case "{Number}":
                            num = Math.Max(num, 0) + settingService.RuleStartNum;
                            name = name.Replace(match, Math.Max(num, 0).ToString(String.Join("", Enumerable.Repeat("0", settingService.RuleNumDigits))));
                            break;
                        case "{Text}":
                            text = RemoveInvalidChar(text);
                            if (text.Length > settingService.RuleTextLength)
                            {
                                text = text.Substring(0, settingService.RuleTextLength);
                            }
                            name = name.Replace(match, text);
                            break;
                        default:
                            name = name.Replace(match, saveTime.ToString(symbol));
                            break;
                    }
                }


                if (!Matches.Contains("{Number}") && num >= 0 ||
                    name == string.Empty)
                {
                    name += "-" + Math.Max(num, 0).ToString();
                }

                var ext = ".wav";
                if (settingService.OutputModeMp3) { ext = ".mp3"; }
                if (settingService.OutputModeWma) { ext = ".wma"; }
                result = Path.Combine(tempDirectory, name + ext);
                return result;
            }
            // ファイル保存ダイアログで選択する
            if (num < 0)
            {
                return fileName;
            }
            var extension = Path.GetExtension(fileName);
            result = Path.Combine(Path.GetDirectoryName(fileName),
                Path.GetFileNameWithoutExtension(fileName) + "-" + num.ToString() + extension);
            return result;
        }

        /// <summary>
        /// ファイル名から禁止文字を取り除く。
        /// </summary>
        /// <param name="filePath"></param>
        private static string RemoveInvalidChar(string filePath)
        {
            var invalids = Path.GetInvalidFileNameChars();
            foreach(var i in invalids)
            {
                filePath = filePath.Replace(i.ToString(), "");
            }
            return filePath;
        }
    }
}
