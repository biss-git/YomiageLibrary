using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yomiage.SDK.Talk;

namespace Yomiage.SDK.Common
{
    public static class CharacterUtil
    {
        /// <summary>
        /// 母音, 拗音
        /// </summary>
        public static Dictionary<string, string> BoinYouon = new Dictionary<string, string>
        {
            { "ア" , "ァ" },
            { "イ" , "ィ" },
            { "ウ" , "ゥ" },
            { "エ" , "ェ" },
            { "オ" , "ォ" },
        };

        /// <summary>
        /// 母音, 拗音
        /// </summary>
        public static Dictionary<string, string> YouonBoin = new Dictionary<string, string>
        {
            { "ァ", "ア" },
            { "ィ", "イ" },
            { "ゥ", "ウ" },
            { "ェ", "エ" },
            { "ォ", "オ" },
        };

        public static string[] BoinA = new string[]
        {
            "ア",
            "カ",
            "サ",
            "タ",
            "ナ",
            "ハ",
            "マ",
            "ヤ",
            "ラ",
            "ワ",
            "ガ",
            "ザ",
            "ダ",
            "バ",
            "パ",
            "ァ",
            "ャ",
        };

        public static string[] BoinI = new string[]
        {
            "イ",
            "キ",
            "シ",
            "チ",
            "ニ",
            "ヒ",
            "ミ",
            "リ",
            "ギ",
            "ジ",
            "ヂ",
            "ビ",
            "ピ",
            "ィ",
        };

        public static string[] BoinU = new string[]
        {
            "ウ",
            "ク",
            "ス",
            "ツ",
            "ヌ",
            "フ",
            "ム",
            "ユ",
            "ル",
            "ヴ",
            "グ",
            "ズ",
            "ヅ",
            "ブ",
            "プ",
            "ゥ",
            "ュ",
        };

        public static string[] BoinE = new string[]
        {
            "エ",
            "ケ",
            "セ",
            "テ",
            "ネ",
            "ヘ",
            "メ",
            "レ",
            "ゲ",
            "ゼ",
            "デ",
            "ベ",
            "ペ",
            "ェ",
        };

        public static string[] BoinO = new string[]
        {
            "オ",
            "コ",
            "ソ",
            "ト",
            "ノ",
            "ホ",
            "モ",
            "ヨ",
            "ロ",
            "ヲ",
            "ゴ",
            "ゾ",
            "ド",
            "ボ",
            "ポ",
            "ォ",
            "ョ",
        };

        public static string FindBoin(Section section, Mora mora)
        {
            if (section == null || mora == null) { return null; }
            var index = section.Moras.IndexOf(mora);
            for (int i = index; i >= 0; i--)
            {
                var b = FindBoin(section.Moras[i]);
                if (b != null)
                {
                    return b;
                }
            }
            return null;
        }

        public static string FindBoin(Mora mora)
        {
            if (string.IsNullOrWhiteSpace(mora?.Character) || mora.Character == "ッ" || mora.Character == "ー" || mora.Character == "ン") { return null; }
            foreach (var pair in YouonBoin)
            {
                if (mora.Character.Contains(pair.Key))
                {
                    return pair.Value;
                }
            }
            if (BoinA.Contains(mora.Character[^1..]))
            {
                return "ア";
            }
            if (BoinI.Contains(mora.Character[^1..]))
            {
                return "イ";
            }
            if (BoinU.Contains(mora.Character[^1..]))
            {
                return "ウ";
            }
            if (BoinE.Contains(mora.Character[^1..]))
            {
                return "エ";
            }
            if (BoinO.Contains(mora.Character[^1..]))
            {
                return "オ";
            }
            return null;
        }

    }
}
