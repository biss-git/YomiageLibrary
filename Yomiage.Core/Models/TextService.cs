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

        VoicePresetService voicePresetService;
        public TextService(VoicePresetService voicePresetService)
        {
            this.voicePresetService = voicePresetService;
        }

        public TalkScript[] Parse(string text, bool SplitByEnter, Func<string, VoicePreset, TalkScript> SearchDictionary = null)
        {

            var scripts = new List<TalkScript>();

            string[] texts = new string[] { text };
            foreach(var c in splitChar)
            {
                // text = text.Replace(c, c + "\n");
                texts = this.Split(texts, c);
            }
            if (SplitByEnter)
            {
                texts = this.Split(texts, "\n");
            }

            using (var tagger = MeCabIpaDicTagger.Create()) // Taggerインスタンスを生成
            {
                foreach (var t in texts)
                {
                    if(SearchDictionary != null)
                    {
                        // 辞書に登録されていればそれを利用する。
                        var registerdSctipt = SearchDictionary(t, voicePresetService.SelectedPreset.Value);
                        if(registerdSctipt != null)
                        {
                            registerdSctipt.OriginalText = t; // 改行の有無が違ったりするので改めてテキストを代入。
                            scripts.Add(registerdSctipt);
                            continue;
                        }
                    }

                    var script = new TalkScript()
                    {
                        OriginalText = t,
                    };
                    scripts.Add(script);
                    if (string.IsNullOrWhiteSpace(t)) { continue; }
                    var nodes = tagger.Parse(t); // 形態素解析を実行
                    bool pauseFlag = false;
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
                        pronounciation = pronounciation.Replace("ヲ", "オ");
                        var moras = YomiDictionary.TextToMoras(pronounciation);
                        if(moras.Count == 0) { continue; }
                        section.Moras = moras
                            .Select(c => new Mora() { Character = c, Accent = true }).ToList();
                        section.Moras[0].Accent = false; // アクセント辞書が無い場合はとりあえずのアクセントで
                        //bool isFirst = true;
                        //foreach (var c in moras)
                        //{
                        //    section.Moras.Add(new Mora() {
                        //        Character = c.ToString(),
                        //        Accent = !isFirst });
                        //    isFirst = false;
                        //}
                        if (pauseFlag)
                        {
                            section.Pause.Type = PauseType.Long;
                            pauseFlag = false;
                        }
                        script.Sections.Add(section);
                    }
                    if (t.Contains("。"))
                    {
                        script.EndSection = new EndSection() { EndSymbol = "。" };
                    }
                    if (t.Contains("？") || t.Contains("?"))
                    {
                        script.EndSection = new EndSection() { EndSymbol = "？" };
                    }
                    if (t.Contains("！") || t.Contains("!"))
                    {
                        script.EndSection = new EndSection() { EndSymbol = "！" };
                    }
                    if (t.Contains("♪"))
                    {
                        script.EndSection = new EndSection() { EndSymbol = "♪" };
                    }
                }
            }
            return scripts.ToArray();
        }

        public string[] Split(string[] texts, string splitChar)
        {
            List<string> result = new List<string>();
            foreach(var text in texts)
            {
                var split = Regex.Split(text, splitChar);
                for (int i = 0; i < split.Length - 1; i++)
                {
                    split[i] += splitChar.Replace("\\","");
                }
                result.AddRange(split);
            }
            return result.ToArray();
        }



    }
}
