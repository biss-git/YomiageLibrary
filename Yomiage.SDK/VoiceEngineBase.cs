using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yomiage.SDK.Config;
using Yomiage.SDK.Settings;
using Yomiage.SDK.Talk;
using Yomiage.SDK.VoiceEffects;

namespace Yomiage.SDK
{
    /// <summary>
    /// 音声合成エンジンのベースクラス。
    /// これを使わずに IVoiceEngine だけ継承して１から作っても問題ない。
    /// </summary>
    public abstract class VoiceEngineBase : IVoiceEngine
    {
        /// <summary>
        /// Playing という名前だが合成処理中を表すフラグ
        /// </summary>
        protected virtual bool isPlaying { get; set; } = false;
        /// <summary>
        /// 停止指令がきたときに使うフラグ
        /// </summary>
        protected virtual bool stopFlag { get; set; } = false;
        /// <summary>
        /// engin.config.json ファイルのパス
        /// </summary>
        protected string ConfigDirectory { get; private set; }
        /// <summary>
        /// 自身(dll) のパス
        /// </summary>
        protected string DllDirectory { get; private set; }

        private Version version
        {
            get
            {
                //自分自身のAssemblyを取得
                Assembly asm = Assembly.GetExecutingAssembly();
                return asm.GetName().Version;
            }
        }
        /// <summary>
        /// メジャーバージョン
        /// 過去のバージョンと互換性がなくなるような変更時にインクリメントしてください。
        /// </summary>
        public virtual int MajorVersion => version.Major;
        /// <summary>
        /// マイナーバージョン
        /// 過去のバージョンと互換性が保たれるような修正に対してインクリメントしてください。
        /// </summary>
        public virtual int MinorVersion => version.Minor;

        /// <summary>
        /// 音声ライブラリのコンフィグ
        /// GUIからは変えられないシステム的な設定値
        /// </summary>
        public EngineConfig Config { get; private set; }
        /// <summary>
        /// エンジンのセッティング
        /// GUIから変更可能な設定値
        /// </summary>
        public EngineSettings Settings { get; set; }
        /// <summary>
        /// ライセンスが通っているかどうか
        /// ライセンスとかつけないよって場合は
        /// 常に true を返せばいい
        /// </summary>
        public virtual bool IsActivated { get; protected set; } = true;
        /// <summary>
        /// 使用可能かどうか
        /// </summary>
        public virtual bool IsEnable => !isPlaying && IsActivated;
        /// <summary>
        /// 処理に成功したとか失敗したとか
        /// ユーザーに伝えたい情報があればここで返す。
        /// GUIのどこかに表示される。
        /// </summary>
        public virtual string StateText { get; protected set; } = string.Empty;

        /// <summary>
        /// ライセンスのアクティベーション
        /// ライセンスキーがGUIから送られる
        /// </summary>
        /// <param name="key">ライセンスキー</param>
        /// <returns>true: 成功、 false: 失敗 (失敗した場合はStateTextに理由を入れておいて欲しい)</returns>
        public virtual async Task<bool> Activate(string key)
        {
            StateText = "アクティベートされました。";
            IsActivated = true;
            await Task.Delay(100);
            return true;
        }
        /// <summary>
        /// ライセンスのディスアクティベーション
        /// </summary>
        /// <returns>true: 成功、 false: 失敗 (失敗した場合はStateTextに理由を入れておいて欲しい)</returns>
        public virtual async Task<bool> DeActivate()
        {
            StateText = "ディアクティベートされました。";
            IsActivated = false;
            await Task.Delay(100);
            return true;
        }
        /// <summary>
        /// 破棄
        /// </summary>
        public virtual void Dispose()
        {
        }
        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="configDirectory"> ライブラリconfigのディレクトリ </param>
        /// <param name="dllDirectory"> ライブラリdllのディレクトリ </param>
        /// <param name="config"> エンジンのコンフィグ、GUIからは変えられないシステム的な設定値 </param>
        public virtual void Initialize(string configDirectory, string dllDirectory, EngineConfig config)
        {
            StateText = "初期化されました。";
            this.Config = config;
            this.ConfigDirectory = configDirectory;
            this.DllDirectory = dllDirectory;
        }
        /// <summary>
        /// 音声の合成処理
        /// 再生ボタンや保存ボタンを押されたときに呼ばれる。
        /// </summary>
        /// <param name="mainVoice"> 読み上げてほしい子のボイス設定 </param>
        /// <param name="subVoice"> サブの音声情報。EnginConfig で VoiceFusionEnable を true にしたときにのみ呼ばれる。基本的には使わなくていい。 </param>
        /// <param name="talkScript"> 読み上げる内容 </param>
        /// <param name="masterEffect"> マスターコントロールの音声効果 </param>
        /// <param name="setSamplingRate_Hz"> サンプリング周波数を教えてほしい </param>
        /// <returns> 音声波形 </returns>
        public virtual async Task<double[]> Play(VoiceConfig mainVoice, VoiceConfig subVoice, TalkScript talkScript, MasterEffectValue masterEffect, Action<int> setSamplingRate_Hz)
        {
            StateText = "再生されました。";
            if (isPlaying) { return null; }
            isPlaying = true;
            stopFlag = false;

            setSamplingRate_Hz(44100);
            if (!mainVoice.Library.TryGetValue("", "", out double[] wave))
            {
                wave = new double[0];
            }

            await Task.Delay(100);

            isPlaying = false;
            stopFlag = false;
            return wave;
        }
        /// <summary>
        /// 音声の保存処理
        /// 基本的には実装しなくてよい。
        /// EngineSaveEnable が true の場合、保存ボタンを押したときに呼ばれる。
        /// UI 側で保存処理は行われないので、この関数内で保存処理を行う。
        /// </summary>
        /// <param name="mainVoice"> 読み上げてほしい子のボイス設定 </param>
        /// <param name="subVoice"> サブの音声情報。EnginConfig で VoiceFusionEnable を true にしたときにのみ呼ばれる。基本的には使わなくていい。 </param>
        /// <param name="talkScripts"> 読み上げる内容 </param>
        /// <param name="masterEffect"> マスターコントロールの音声効果 </param>
        /// <param name="filePath"> 保存先の指定アドレス (拡張子は .wav, .mp3 の可能性がある) </param>
        /// <param name="startPause"> 保存時の前ポーズ [ms] </param>
        /// <param name="endPause"> 保存時の後ポーズ [ms] </param>
        /// <param name="saveWithText"> テキストを一緒に保存するかどうか </param>
        /// <param name="encoding"> テキストのエンコード情報 </param>
        /// <returns> 音声波形 </returns>
        public virtual async Task Save(VoiceConfig mainVoice, VoiceConfig subVoice, TalkScript[] talkScripts, MasterEffectValue masterEffect, string filePath, int startPause, int endPause, bool saveWithText, Encoding encoding)
        {
            StateText = "保存機能は実装されていません。";
            await Task.Delay(100);
            throw new NotImplementedException();
        }
        /// <summary>
        /// 音声生成の中断処理
        /// </summary>
        /// <returns> true: 成功、false: 失敗 </returns>
        public virtual async Task<bool> Stop()
        {
            if (!isPlaying) { return true; }
            stopFlag = true;
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(100);
                if (!stopFlag) { break; }
            }
            return !stopFlag;
        }

    }
}
