using Microsoft.International.Converters;
using NMeCab.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Yomiage.Core.Types;
using Yomiage.Core.Utility;
using Yomiage.SDK.Talk;

namespace Yomiage.Core.Models
{
    public class TextService
    {
        private string[] splitChar = new string[]
        {
            "。",
            "！",
            "!",
            "？",
            @"\?",
            "♪",
        };

        private string[] ignoreList = new string[]
        {
            "。",
            "！",
            "!",
            "？",
            @"\?",
            "♪",
            "、"
        };

        private string[] ltChar = new string[]
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

        VoicePresetService voicePresetService;
        public TextService(VoicePresetService voicePresetService)
        {
            this.voicePresetService = voicePresetService;
        }

        public TalkScript[] Parse(
            string text,
            bool SplitByEnter = true,
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
                if (SearchDictionary != null)
                {
                    // 辞書に登録されていればそれを利用する。
                    var registerdSctipt = SearchDictionary(t, voicePresetService.SelectedPreset.Value);
                    if (registerdSctipt != null)
                    {
                        registerdSctipt.OriginalText = t; // 改行の有無が違ったりするので改めてテキストを代入。
                        scripts.Add(registerdSctipt);
                        continue;
                    }
                }

                List<TextPart> textParts = new List<TextPart>();
                textParts.Add(new TextPart() { Text = t });
                MakeScriptByWordDictionary(textParts, WordDictionarys);
                MakeScriptByPauseDictionary(textParts, PauseDictionary);
                MakeScriptByIPA(textParts);

                var script = MargeScripts(textParts);
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
                    var newPart2 = new TextPart() { Text = part.Text.Substring(index + newPart.Text.Length) };
                    textParts.Insert(j + 1, newPart2);
                    textParts.Insert(j + 1, newPart);
                    part.Text = part.Text.Substring(0, index);
                    j += 1;
                }
                else if (index == 0)
                {
                    // 前方一致　
                    part.Text = part.Text.Substring(newPart.Text.Length);
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
            using (var tagger = MeCabIpaDicTagger.Create()) // Taggerインスタンスを生成
            {
                var nodes = tagger.Parse(text); // 形態素解析を実行
                bool pauseFlag = false;
                var caPronounciation = string.Empty;
                foreach (var node in nodes) // 形態素ノード配列を順に処理
                {
                    var section = new Section()
                    {
                        Pause = new Pause(),
                        Moras = new List<Mora>(),
                    };
                    var pronounciation = node.Pronounciation;
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
                        // 次の node が小文字で始まる場合はつなげる。
                        if (!string.IsNullOrWhiteSpace(caPronounciation))
                        {
                            pronounciation = caPronounciation + pronounciation;
                            caPronounciation = null;
                        }
                        var index = Array.IndexOf(nodes, node);
                        if (index + 1 < nodes.Length)
                        {
                            var np = nodes[index + 1].Pronounciation;
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

                    pronounciation = pronounciation.Replace("ヲ", "オ");
                    var moras = YomiDictionary.TextToMoras(pronounciation);
                    if (moras.Count == 0) { continue; }
                    section.Moras = moras
                        .Select(c => new Mora() { Character = c, Accent = true }).ToList();
                    section.Moras[0].Accent = false; // アクセント辞書が無い場合はとりあえずのアクセントで
                    if (pauseFlag)
                    {
                        section.Pause.Type = PauseType.Long;
                        pauseFlag = false;
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
                    s1.Moras.Add(s2.Moras[0]);
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




        class TextPart
        {
            public string Text { get; set; }
            public TalkScript Script { get; set; }
        }
    }

}
