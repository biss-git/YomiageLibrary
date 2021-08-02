using Microsoft.International.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Yomiage.Core.Utility
{
    public static class YomiDictionary
    {
        public static Dictionary<string, string> Numbers = new Dictionary<string, string>()
        {
            {"0", "ゼロ" },
            {"1", "イチ" },
            {"2", "ニ" },
            {"3", "サン" },
            {"4", "ヨン" },
            {"5", "ゴ" },
            {"6", "ロク" },
            {"7", "ナナ" },
            {"8", "ハチ" },
            {"9", "キュウ" },
        };

        public static Dictionary<string, string> Letters = new Dictionary<string, string>()
        {
            {"a", "エー" },
            {"b", "ビー" },
            {"c", "シー" },
            {"d", "ディー" },
            {"e", "イー" },
            {"f", "エフ" },
            {"g", "ジー" },
            {"h", "エッチ" },
            {"i", "アイ" },
            {"j", "ジェー" },
            {"k", "ケー" },
            {"l", "エル" },
            {"m", "エム" },
            {"n", "エヌ" },
            {"o", "オー" },
            {"p", "ピー" },
            {"q", "キュー" },
            {"r", "アール" },
            {"s", "エス" },
            {"t", "ティー" },
            {"u", "ユー" },
            {"v", "ブイ" },
            {"w", "ダブリュー" },
            {"x", "エックス" },
            {"y", "ワイ" },
            {"z", "ゼット" },
        };

        /// <summary>
        /// 英数字(半角小文字)を読みに変換
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetPronunce(string text)
        {
            foreach(var pair in Numbers)
            {
                if (text.Contains(pair.Key))
                {
                    text = text.Replace(pair.Key, pair.Value);
                }
            }
            foreach (var pair in Letters)
            {
                if (text.Contains(pair.Key))
                {
                    text = text.Replace(pair.Key, pair.Value);
                }
            }
            return text;
        }


        public static char[] LittleChar = new char[]
        {
            'ァ', 'ィ', 'ゥ', 'ェ', 'ォ', 'ャ', 'ュ', 'ョ'
        };


        /// <summary>
        /// １文字の文字一覧
        /// ヰ と ヱ は無い
        /// </summary>
        public static readonly string[] Mora1List = new string[]
        {
            "ア","イ","ウ","エ","オ",
            "カ","キ","ク","ケ","コ",
            "サ","シ","ス","セ","ソ",
            "タ","チ","ツ","テ","ト",
            "ナ","ニ","ヌ","ネ","ノ",
            "ハ","ヒ","フ","ヘ","ホ",
            "マ","ミ","ム","メ","モ",
            "ヤ",     "ユ",     "ヨ",
            "ラ","リ","ル","レ","ロ",
            "ワ",               "ヲ","ン",
                      "ヴ",
            "ガ","ギ","グ","ゲ","ゴ",
            "ザ","ジ","ズ","ゼ","ゾ",
            "ダ","ヂ","ヅ","デ","ド",
            "バ","ビ","ブ","ベ","ボ",
            "パ","ピ","プ","ペ","ポ",
            "ァ","ィ","ゥ","ェ","ォ",
            "ャ",     "ュ",     "ョ",
            "ー", "ッ"
        };

        /// <summary>
        /// ２文字で１モーラとなるもののリスト
        /// スズモフ先生の「ボイロチューニング！！Vol.1～基本編～」P.23
        /// ボイスロイドで発音可能な拗音の一覧に準拠。
        /// ただし、イェは含む、ヂェヂャヂュヂョは含まない。
        /// </summary>
        public static string[] Mora2List = new string[]
        {
                                    "イェ",
                    "ウィ",         "ウェ", "ウォ",
                                    "キェ",         "キャ", "キュ", "キョ",
            "クァ", "クィ",         "クェ", "クォ",
                                    "シェ",         "シャ", "シュ", "ショ",
            "スァ", "スィ",         "スェ", "スォ",
                                    "チェ",         "チャ", "チュ", "チョ",
            "ツァ", "ツィ",         "ツェ", "ツォ",
                    "ティ",                         "テャ", "テュ", "テョ",
                            "トゥ",
                                    "ニェ",         "ニャ", "ニュ", "ニョ",
            "ヌァ", "ヌィ",         "ヌェ", "ヌォ",
                                    "ヒェ",         "ヒャ", "ヒュ", "ヒョ",
            "ファ", "フィ",         "フェ", "フォ", "フャ", "フュ", "フョ",
                                    "ミェ",         "ミャ", "ミュ", "ミョ",
            "ムァ", "ムィ",         "ムェ", "ムォ",
                                    "リェ",         "リャ", "リュ", "リョ",
            "ルァ", "ルィ",         "ルェ", "ルォ",
            "ヴァ", "ヴィ",         "ヴェ", "ヴォ", "ヴャ", "ヴュ", "ヴョ",
                                    "ギェ",         "ギャ", "ギュ", "ギョ",
            "グァ", "グィ",         "グェ", "グォ",
                                    "ジェ",         "ジャ", "ジュ", "ジョ",
            "ズァ", "ズィ",         "ズェ", "ズォ",
                    "ディ",                         "デャ", "デュ", "デョ",
                            "ドゥ",
                                    "ビェ",         "ビャ", "ビュ", "ビョ",
            "ブァ", "ブィ",         "ブェ", "ブォ",
                                    "ピェ",         "ピャ", "ピュ", "ピョ",
            "プァ", "プィ",         "プェ", "プォ",
        };

        public static List<string> TextToMoras(string text)
        {
            var moras = new List<string>();
            while(text.Length > 0)
            {
                if (text.Length > 1 &&
                    LittleChar.Contains(text[1]) &&
                    Mora2List.Contains(text.Substring(0,2)))
                {
                    moras.Add(text.Substring(0, 2));
                    text = text.Substring(2);
                    continue;
                }

                var c = text.Substring(0, 1);
                if (Mora1List.Contains(c))
                {
                    moras.Add(c);
                }
                text = text.Substring(1);
            }
            return moras;
        }



        /// <summary>
        /// 英数字は読み上げるための処理
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        public static string SurfaceToYomi(string surface)
        {
            string temp = ZenkakuToHankaku(surface); // 全角英数字を半角に変換
            temp = temp.ToLower(); // 小文字に変換
            temp = KanaConverter.RomajiToHiragana(temp);  // ローマ字として読めるものは変換
            temp = YomiDictionary.GetPronunce(temp); // 英数字を既定の文字に変換
            temp = KanaConverter.HiraganaToKatakana(temp); // ひらがなをカタカナに変換
            return temp;
        }

        /// <summary>
        /// 全角英数字を半角英数字にする。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ZenkakuToHankaku(string text)
        {
            var temp = Regex.Replace(text, "[０-９]", p => ((char)(p.Value[0] - '０' + '0')).ToString());
            temp = Regex.Replace(temp, "[Ａ-Ｚ]", p => ((char)(p.Value[0] - 'Ａ' + 'A')).ToString());
            return temp;
        }
    }
}
