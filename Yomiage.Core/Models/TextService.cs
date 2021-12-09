using NMeCab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Yomiage.Core.Types;
using Yomiage.Core.Utility;
using Yomiage.SDK.Talk;

namespace Yomiage.Core.Models
{
    public class TextService : IDisposable
    {
        private readonly string[] splitChar = new string[]
        {
            "。",
            "！",
            "!",
            "？",
            @"\?",
            "♪",
        };

        private readonly string[] ignoreList = new string[]
        {
            "。",
            "！",
            "!",
            "？",
            @"\?",
            "♪",
            "、"
        };

        private readonly string[] ltChar = new string[]
        {
            "ァ",
            "ィ",
            "ゥ",
            "ェ",
            "ォ",
            "ャ",
            "ュ",
            "ョ",
        };
        readonly MeCabTagger tagger;
        readonly VoicePresetService voicePresetService;
        public TextService(VoicePresetService voicePresetService)
        {
            this.voicePresetService = voicePresetService;
            try
            {
                var dir = System.AppDomain.CurrentDomain.BaseDirectory;
                var dicdir = System.IO.Path.Combine(dir, "AccentDic");
                tagger = NMeCab.MeCabTagger.Create(dicdir);
            }
            catch (Exception)
            {
                try
                {
                    tagger = MeCabTagger.Create();
                }
                catch (Exception)
                {

                }
            }
        }

        public TalkScript[] Parse(
            string text,
            bool SplitByEnter = true,
            bool PromptStringEnable = true,
            string PromptString = "＞",
            Func<string, VoicePreset, TalkScript> SearchDictionary = null,
            Dictionary<string, WordSet> WordDictionarys = null,
            List<PauseSet> PauseDictionary = null)
        {

            var scripts = new List<TalkScript>();

            if (string.IsNullOrWhiteSpace(text)) { return scripts.ToArray(); }

            string[] texts = new string[] { text };
            foreach (var c in splitChar)
            {
                texts = this.Split(texts, c);
            }
            if (SplitByEnter)
            {
                texts = this.Split(texts, "\n");
            }


            foreach (var t in texts)
            {
                if (string.IsNullOrWhiteSpace(t))
                {
                    // 改行などの表示を保つために、読み上げられないものも返す。
                    scripts.Add(new TalkScript() { OriginalText = t });
                    continue;
                }

                // プリセットタグを処理する
                var character = string.Empty;
                var tt = t;
                var preset = voicePresetService.SelectedPreset.Value;
                if (PromptStringEnable && !string.IsNullOrWhiteSpace(PromptString) && t.Contains(PromptString))
                {
                    var index = t.IndexOf(PromptString);
                    var c = t.Substring(0, index);
                    if (c.Contains("\n"))
                    {
                        var i = c.LastIndexOf("\n");
                        c = c[(i + 1)..];
                    }
                    if (this.voicePresetService.AllPresets.Any(p => p.Name == c))
                    {
                        character = c;
                        tt = t[(index + PromptString.Length)..];
                        preset = voicePresetService.AllPresets.First(p => p.Name == c);
                    }
                }

                if (SearchDictionary != null)
                {
                    // 辞書に登録されていればそれを利用する。
                    var registerdSctipt = SearchDictionary(tt, preset);
                    if (registerdSctipt != null)
                    {
                        registerdSctipt.OriginalText = tt; // 改行の有無が違ったりするので改めてテキストを代入。
                        registerdSctipt.PresetName = character;
                        scripts.Add(registerdSctipt);
                        continue;
                    }
                }

                List<TextPart> textParts = new List<TextPart>
                {
                    new TextPart() { Text = tt }
                };
                MakeScriptByWordDictionary(textParts, WordDictionarys);
                MakeScriptByPauseDictionary(textParts, PauseDictionary);
                MakeScriptByIPA(textParts);

                var script = MargeScripts(textParts);
                script.PresetName = character;
                scripts.Add(script);
            }

            return scripts.ToArray();
        }

        private TalkScript MargeScripts(List<TextPart> textParts)
        {
            if (textParts.Count == 0) { return null; }
            var originalText = "";
            foreach (var part in textParts)
            {
                originalText += part.Text;
            }
            var script = new TalkScript()
            {
                OriginalText = originalText,
            };
            var sections = new List<Section>(textParts[0].Script.Sections);
            var pauseTime_ms = (textParts[0].Script.EndSection.Pause.Type == PauseType.Manual) ? textParts[0].Script.EndSection.Pause.Span_ms : 0;
            for (int i = 1; i < textParts.Count; i++)
            {
                var part = textParts[i].Script;
                if (pauseTime_ms > 0 && part.Sections.Count > 0)
                {
                    part.Sections[0].Pause.Type = PauseType.Manual;
                    part.Sections[0].Pause.Span_ms += pauseTime_ms;
                    pauseTime_ms = 0;
                }
                if (part.EndSection.Pause.Type == PauseType.Manual)
                {
                    pauseTime_ms += part.EndSection.Pause.Span_ms;
                }
                sections.AddRange(part.Sections);
            }

            script.Sections = sections;
            script.EndSection = textParts.Last().Script.EndSection;
            if (pauseTime_ms > 0)
            {
                script.EndSection.Pause.Type = PauseType.Manual;
                script.EndSection.Pause.Span_ms += pauseTime_ms;
            }

            return script;
        }

        /// <summary>
        /// 参考データ
        /// </summary>
        static string[] Prioritys { get; } = new string[5]
        {
            "1.最高",
            "2.高い",
            "3.標準",
            "4.低い",
            "5.最低",
        };
        private void MakeScriptByWordDictionary(List<TextPart> textParts, Dictionary<string, WordSet> WordDictionarys)
        {
            if (WordDictionarys == null) { return; }
            for (int i = 1; i <= 5; i++)
            {
                var dict = WordDictionarys.Where(x => x.Value.Priority.StartsWith(i.ToString())).ToList();
                dict.Sort((a, b) => b.Key.Length - a.Key.Length);
                foreach (var w in dict)
                {
                    for (int j = textParts.Count - 1; j >= 0; j--)
                    {
                        var part = textParts[j];
                        if (part.Script == null &&
                            part.Text.Contains(w.Key))
                        {
                            var newPart = new TextPart() { Text = w.Key, Script = TalkScript.GetTalkScript_fromPhraseJsonText(w.Value.JsonText) };
                            ReplacePart(textParts, ref j, newPart);
                        }
                    }
                }
            }
        }

        private void MakeScriptByPauseDictionary(List<TextPart> textParts, List<PauseSet> PauseDictionary)
        {
            if (PauseDictionary == null) { return; }
            PauseDictionary.Sort((a, b) => b.key.Length - a.key.Length);
            foreach (var p in PauseDictionary)
            {
                for (int j = textParts.Count - 1; j >= 0; j--)
                {
                    var part = textParts[j];
                    if (part.Script == null &&
                        part.Text.Contains(p.key))
                    {
                        var newPart = new TextPart() { Text = p.key, Script = new TalkScript() { EndSection = { Pause = { Span_ms = p.span_ms, Type = PauseType.Manual } } } };
                        ReplacePart(textParts, ref j, newPart);
                    }
                }
            }
        }

        /// <summary>
        /// 単語辞書かポーズ辞書で一致するものが見つかった場合に置き換える
        /// </summary>
        /// <param name="textParts">分割されたテキストリスト</param>
        /// <param name="j">textPartsのどこに適用するかのインデックス</param>
        /// <param name="newPart">置き換えたいTextPart</param>
        private void ReplacePart(List<TextPart> textParts, ref int j, TextPart newPart)
        {
            var part = textParts[j];
            if (part.Text == newPart.Text)
            {
                // 完全一致　単語辞書をそのまま適用。
                part.Script = newPart.Script;
            }
            else if (part.Text.EndsWith(newPart.Text))
            {
                // 後方一致　後方に一つ追加
                textParts.Insert(j + 1, newPart);
                part.Text = part.Text.Substring(0, part.Text.Length - newPart.Text.Length);
                j += 1;
            }
            else
            {
                var index = part.Text.LastIndexOf(newPart.Text);
                if (index > 0)
                {
                    // 中央一致　
                    var newPart2 = new TextPart() { Text = part.Text[(index + newPart.Text.Length)..] };
                    textParts.Insert(j + 1, newPart2);
                    textParts.Insert(j + 1, newPart);
                    part.Text = part.Text.Substring(0, index);
                    j += 1;
                }
                else if (index == 0)
                {
                    // 前方一致　
                    part.Text = part.Text[newPart.Text.Length..];
                    textParts.Insert(j, newPart);
                }
            }
        }


        private void MakeScriptByIPA(List<TextPart> textParts)
        {
            foreach (var part in textParts)
            {
                if (part.Script == null)
                {
                    part.Script = MakeScriptByIPA(part.Text);
                }
            }
        }

        private TalkScript MakeScriptByIPA(string text)
        {
            var script = new TalkScript()
            {
                OriginalText = text,
            };
            {
                var nodes = tagger.Parse(text); //.ToArray(); // 形態素解析を実行
                bool pauseFlag = false;
                var caPronounciation = string.Empty;
                bool conbineFlag;
                foreach (var node in nodes) // 形態素ノード配列を順に処理
                {
                    if (node.Feature == null) { continue; }
                    var features = node.Feature.Split(',');
                    var section = new Section()
                    {
                        Pause = new Pause(),
                        Moras = new List<Mora>(),
                    };
                    var pronounciation = features.Length > 8 ? features[8] : "";
                    if (node.Surface == "、") { pauseFlag = true; } // "、"を長ポーズとして扱うためのフラグを立てる
                    if (ignoreList.Contains(pronounciation)) { continue; }
                    if (string.IsNullOrWhiteSpace(pronounciation))
                    {
                        pronounciation = YomiDictionary.SurfaceToYomi(node.Surface);
                        if (string.IsNullOrWhiteSpace(pronounciation))
                        {
                            continue;
                        }
                    }

                    {
                        conbineFlag = false;
                        // 次の node が小文字で始まる場合はつなげる。
                        if (!string.IsNullOrWhiteSpace(caPronounciation))
                        {
                            pronounciation = caPronounciation + pronounciation;
                            caPronounciation = null;
                            conbineFlag = true;
                        }
                        var index = Array.IndexOf(nodes, node);
                        if (index + 1 < nodes.Length)
                        {
                            var nextFeatures = nodes[index + 1].Feature?.Split(',');
                            if (nextFeatures != null)
                            {
                                var np = nextFeatures.Length > 8 ? nextFeatures[8] : "";
                                var ns = YomiDictionary.SurfaceToYomi(nodes[index + 1].Surface);
                                if (np.Length > 0 &&
                                    ltChar.Contains(np.Substring(0, 1)) ||
                                    np.Length == 0 &&
                                    ltChar.Contains(ns.Substring(0, 1)))
                                {
                                    caPronounciation = pronounciation;
                                    continue;
                                }
                            }
                        }
                    }

                    pronounciation = pronounciation.Replace("ヲ", "オ");
                    var moras = YomiDictionary.TextToMoras(pronounciation);
                    if (moras.Count == 0) { continue; }
                    section.Moras = moras
                        .Select(c => new Mora() { Character = c, Accent = true }).ToList();
                    // アクセント辞書が無い場合はとりあえずのアクセントで
                    section.Moras[0].Accent = false;
                    if (section.Moras.Count > 1 &&
                        !conbineFlag && // 結合した時はアクセントはつけない
                        features.Length > 9 && // 辞書にアクセントがあるか確認
                        features[9].Length > 1 &&
                        int.TryParse(features[9].Substring(0, 1), out var position) && // アクセント位置を取得
                        position > 0)
                    {
                        for (int i = 0; i < section.Moras.Count; i++)
                        {
                            section.Moras[i].Accent = (i == position - 1) || (0 < i && i < position);
                        }
                    }

                    if (pauseFlag)
                    {
                        section.Pause.Type = PauseType.Long;
                        pauseFlag = false;
                    }

                    if (section.Pause.Type == PauseType.None && // ポーズが無いこと
                        script.Sections.Count > 0 && // ２つ目以降のアクセント句であること
                        node.Prev != null)
                    {
                        // アクセント結合処理
                        var preSection = script.Sections.Last();
                        var type1 = node.Prev.Feature?.Split(',').First();
                        var type2 = features[0];
                        if (type1 == "動詞" && type2 == "助動詞" ||
                            type1 == "名詞" && type2 == "名詞")
                        {
                            preSection.Moras.ForEach(m => m.Accent = (m != preSection.Moras.First()));
                            section.Moras.ForEach(m => m.Accent = (m == section.Moras.First()));
                            preSection.Moras.AddRange(section.Moras);
                            continue;
                        }
                    }

                    script.Sections.Add(section);
                }
                if (text.Contains("。"))
                {
                    script.EndSection = new EndSection() { EndSymbol = "。" };
                }
                if (text.Contains("？") || text.Contains("?"))
                {
                    script.EndSection = new EndSection() { EndSymbol = "？" };
                }
                if (text.Contains("！") || text.Contains("!"))
                {
                    script.EndSection = new EndSection() { EndSymbol = "！" };
                }
                if (text.Contains("♪"))
                {
                    script.EndSection = new EndSection() { EndSymbol = "♪" };
                }
            }
            for (int i = 0; i < script.Sections.Count - 1; i++)
            {
                // １文字のものは連結する。
                var s1 = script.Sections[i];
                var s2 = script.Sections[i + 1];
                if (s2.Pause.Type == PauseType.None && s2.Moras.Count == 1)
                {
                    s2.Moras.First().Accent = s1.Moras.Last().Accent;
                    s1.Moras.Add(s2.Moras[0]);
                    script.Sections.Remove(s2);
                    i -= 1;
                }
            }
            for (int i = 0; i < script.Sections.Count - 1; i++)
            {
                // Accent が true で終わって true ではじまるときはつなげる
                var s1 = script.Sections[i];
                var s2 = script.Sections[i + 1];
                if (s2.Pause.Type == PauseType.None && s1.Moras.Last().Accent && s2.Moras.First().Accent)
                {
                    s1.Moras.AddRange(s2.Moras);
                    script.Sections.Remove(s2);
                    i -= 1;
                }
            }
            return script;
        }

        public string[] Split(string[] texts, string splitChar)
        {
            List<string> result = new List<string>();
            foreach (var text in texts)
            {
                var split = Regex.Split(text, splitChar);
                for (int i = 0; i < split.Length - 1; i++)
                {
                    split[i] += splitChar.Replace("\\", "");
                }
                result.AddRange(split);
            }
            return result.ToArray();
        }

        public void Dispose()
        {
            tagger.Dispose();
        }

        class TextPart
        {
            public string Text { get; set; }
            public TalkScript Script { get; set; }
        }
    }

}
