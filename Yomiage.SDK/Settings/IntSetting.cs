// <copyright file="IntSetting.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System;

namespace Yomiage.SDK.Settings
{
    /// <summary>
    /// 整数値の設定
    /// </summary>
    public class IntSetting : SettingBase, ISetting<int>
    {
        /// <inheritdoc/>
        public int Value { get; set; }

        /// <inheritdoc/>
        public int DefaultValue { get; set; }

        /// <summary>
        /// 設定可能な最小値
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// 設定可能な最大値
        /// </summary>
        public int Max { get; set; }

        /// <summary>
        /// GUIでの最小ステップ
        /// ただし、入力の最小値ではない。
        /// 例えば SmallStep = 2 としても、偶数のみ、奇数のみの入力になるわけではない。
        /// </summary>
        public int SmallStep { get; set; }

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
            Min = Min.Fix();
            Max = Max.Fix();
            SmallStep = SmallStep.Fix();
            if (Min > Max)
            {
                var temp = Min;
                Min = Max;
                Max = temp;
            }

            Value = Value.Clamp(Min, Max);
            DefaultValue = DefaultValue.Clamp(Min, Max);
            SmallStep = SmallStep.Clamp(1, Max - Min);
        }
    }
}
