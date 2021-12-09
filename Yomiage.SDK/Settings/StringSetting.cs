// <copyright file="StringSetting.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System;

namespace Yomiage.SDK.Settings
{
    /// <summary>
    /// 文字列の設定
    /// </summary>
    public class StringSetting : SettingBase, ISetting<string>
    {
        /// <inheritdoc/>
        public string Value { get; set; } = string.Empty;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override void ResetValue()
        {
            Value = DefaultValue;
        }

        /// <inheritdoc/>
        public override void Fix()
        {
            Value = Value.Fix();
            DefaultValue = DefaultValue.Fix();
            FileDialogFilter = FileDialogFilter.Fix();
            MaxLength = MaxLength.Fix();
            MaxLength = Math.Max(0, MaxLength);
        }
    }
}
