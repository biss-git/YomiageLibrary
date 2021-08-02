using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Data
{
    public enum SearchGroup
    {
        All,
        A,
        K,
        S,
        T,
        N,
        H,
        M,
        Y,
        R,
        W
    }

    static class StringUtil
    {
        public static bool StartsWith(this string text, string[] values)
        {
            return values.Any(t => text.StartsWith(t));
        }
    }

    public static class SearchGroupUtil
    {
        public static string[] GroupA_katakana = { "ア", "イ", "ウ", "エ", "オ"};
        public static string[] GroupA_hiragana = { "あ", "い", "う", "え", "お"};
        public static string[] GroupK_katakana = { "カ", "キ", "ク", "ケ", "コ" };
        public static string[] GroupK_hiragana = { "か", "き", "く", "け", "こ" };
        public static string[] GroupS_katakana = { "サ", "シ", "ス", "セ", "ソ" };
        public static string[] GroupS_hiragana = { "さ", "し", "す", "せ", "そ" };
        public static string[] GroupT_katakana = { "タ", "チ", "ツ", "テ", "ト" };
        public static string[] GroupT_hiragana = { "た", "ち", "つ", "て", "と" };
        public static string[] GroupN_katakana = { "ナ", "ニ", "ヌ", "ネ", "ノ" };
        public static string[] GroupN_hiragana = { "な", "に", "ぬ", "ね", "の" };
        public static string[] GroupH_katakana = { "ハ", "ヒ", "フ", "ヘ", "ホ" };
        public static string[] GroupH_hiragana = { "は", "ひ", "ふ", "へ", "ほ" };
        public static string[] GroupM_katakana = { "マ", "ミ", "ム", "メ", "モ" };
        public static string[] GroupM_hiragana = { "ま", "み", "む", "め", "も" };
        public static string[] GroupY_katakana = { "ヤ", "ユ", "ヨ" };
        public static string[] GroupY_hiragana = { "や", "ゆ", "よ" };
        public static string[] GroupR_katakana = { "ラ", "リ", "ル", "レ", "ロ" };
        public static string[] GroupR_hiragana = { "ら", "り", "る", "れ", "ろ" };
        public static string[] GroupW_katakana = { "ワ", "ヲ", "ン" };
        public static string[] GroupW_hiragana = { "わ", "を", "ん" };

        public static bool GroupMatch_Both(string text, SearchGroup group)
        {
            switch (group)
            {
                case SearchGroup.All:
                    return true;
                case SearchGroup.A:
                    if (text.StartsWith(GroupA_katakana) || text.StartsWith(GroupA_hiragana)) { return true; }
                    break;
                case SearchGroup.K:
                    if (text.StartsWith(GroupK_katakana) || text.StartsWith(GroupK_hiragana)) { return true; }
                    break;
                case SearchGroup.S:
                    if (text.StartsWith(GroupS_katakana) || text.StartsWith(GroupS_hiragana)) { return true; }
                    break;
                case SearchGroup.T:
                    if (text.StartsWith(GroupT_katakana) || text.StartsWith(GroupT_hiragana)) { return true; }
                    break;
                case SearchGroup.N:
                    if (text.StartsWith(GroupN_katakana) || text.StartsWith(GroupN_hiragana)) { return true; }
                    break;
                case SearchGroup.H:
                    if (text.StartsWith(GroupH_katakana) || text.StartsWith(GroupH_hiragana)) { return true; }
                    break;
                case SearchGroup.M:
                    if (text.StartsWith(GroupM_katakana) || text.StartsWith(GroupM_hiragana)) { return true; }
                    break;
                case SearchGroup.Y:
                    if (text.StartsWith(GroupY_katakana) || text.StartsWith(GroupY_hiragana)) { return true; }
                    break;
                case SearchGroup.R:
                    if (text.StartsWith(GroupR_katakana) || text.StartsWith(GroupR_hiragana)) { return true; }
                    break;
                case SearchGroup.W:
                    if (text.StartsWith(GroupW_katakana) || text.StartsWith(GroupW_hiragana)) { return true; }
                    break;
            }

            return false;
        }

        public static bool GroupMatch_katakana(string text, SearchGroup group)
        {
            switch (group)
            {
                case SearchGroup.All:
                    return true;
                case SearchGroup.A:
                    if (text.StartsWith(GroupA_katakana)) { return true; }
                    break;
                case SearchGroup.K:
                    if (text.StartsWith(GroupK_katakana)) { return true; }
                    break;
                case SearchGroup.S:
                    if (text.StartsWith(GroupS_katakana)) { return true; }
                    break;
                case SearchGroup.T:
                    if (text.StartsWith(GroupT_katakana)) { return true; }
                    break;
                case SearchGroup.N:
                    if (text.StartsWith(GroupN_katakana)) { return true; }
                    break;
                case SearchGroup.H:
                    if (text.StartsWith(GroupH_katakana)) { return true; }
                    break;
                case SearchGroup.M:
                    if (text.StartsWith(GroupM_katakana)) { return true; }
                    break;
                case SearchGroup.Y:
                    if (text.StartsWith(GroupY_katakana)) { return true; }
                    break;
                case SearchGroup.R:
                    if (text.StartsWith(GroupR_katakana)) { return true; }
                    break;
                case SearchGroup.W:
                    if (text.StartsWith(GroupW_katakana)) { return true; }
                    break;
            }

            return false;
        }

    }

}
