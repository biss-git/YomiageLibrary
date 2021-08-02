using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yomiage.SDK.Settings
{
    /// <summary>
    /// 文字列の設定
    /// </summary>
    public class StringSetting : SettingBase
    {
        /// <summary>
        /// 設定値
        /// </summary>
        public string Value { get; set; } = string.Empty;
        /// <summary>
        /// 初期値
        /// </summary>
        public string DefaultValue { get; set; } = string.Empty;
        /// <summary>
        /// Type を file としたときの拡張子のフォーマット
        /// OpenFileDialog の Filter に渡されるので、例えば以下のように書く。
        /// "テキスト(.txt)|*.txt|全てのファイル|*.*"
        /// </summary>
        public string FileDialogFilter { get; set; } = string.Empty;
        /// <summary>
        /// 文字列の最大長さ
        /// </summary>
        public int MaxLength { get; set; } = 512;
        /// <summary>
        /// Type を combobox としたときの選択肢
        /// </summary>
        public string[] ComboItems { get; set; }

        /// <summary>
        /// 初期値を設定値に代入
        /// </summary>
        public override void ResetValue()
        {
            Value = DefaultValue;
        }
        /// <summary>
        /// 不正な値をはじく
        /// </summary>
        public override void Fix()
        {
            Value = Value.Fix();
            DefaultValue = DefaultValue.Fix();
            FileDialogFilter = FileDialogFilter.Fix();
            MaxLength = MaxLength.Fix();
            MaxLength = Math.Max(0, MaxLength);
        }
    }

    internal static class StringExtension
    {
        /// <summary>
        /// 不正な値をはじく
        /// </summary>
        public static string Fix(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
