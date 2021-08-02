using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yomiage.SDK.Settings
{
    /// <summary>
    /// 整数値の設定
    /// </summary>
    public class IntSetting : SettingBase
    {
        /// <summary>
        /// 設定値
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// 初期値
        /// </summary>
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

    internal static class IntExtension
    {
        /// <summary>
        /// 非数値の判定
        /// </summary>
        public static bool IsNotNumber(this int value)
        {
            return double.IsNaN(value) || double.IsInfinity(value);
        }
        /// <summary>
        /// 非数値の場合はデフォルト値を返す。
        /// </summary>
        public static int Fix(this int value)
        {
            return value.IsNotNumber() ? default(int) : value;
        }
        /// <summary>
        /// [min, max] の区間に納めて返す。
        /// 区間が0以下の場合は min を返す。（Math.Clampだとエラーを返すので使えない）
        /// </summary>
        public static int Clamp(this int value, int min, int max)
        {
            return Math.Max(min, Math.Min(value, max));
        }
    }
}
