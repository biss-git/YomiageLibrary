using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yomiage.SDK.Settings
{
    /// <summary>
    /// 実数値の設定
    /// </summary>
    public class DoubleSetting : SettingBase
    {
        /// <summary>
        /// 設定値
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        /// 初期値
        /// </summary>
        public double DefaultValue { get; set; }
        /// <summary>
        /// 設定可能な最小値
        /// </summary>
        public double Min { get; set; }
        /// <summary>
        /// 設定可能な最大値
        /// </summary>
        public double Max { get; set; }
        /// <summary>
        /// GUIでの最小ステップ
        /// ただし、入力の最小値ではない。
        /// 例えば SmallStep = 2 としても、偶数のみ、奇数のみの入力になるわけではない。
        /// </summary>
        public double SmallStep { get; set; }
        /// <summary>
        /// 0.00 
        /// </summary>
        public string StringFormat { get; set; }

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
            StringFormat = StringFormat.Fix();
            if(Min > Max)
            {
                var temp = Min;
                Min = Max;
                Max = temp;
            }
            Value = Value.Clamp(Min, Max);
            DefaultValue = DefaultValue.Clamp(Min, Max);
            if(SmallStep <= 0)
            {
                SmallStep = (Max - Min) / 100;
            }
        }
    }
    
    /// <summary>
    /// double の拡張メソッド
    /// </summary>
    internal static class DoubleExtension
    {
        /// <summary>
        /// 非数値の判定
        /// </summary>
        public static bool IsNotNumber(this double value)
        {
            return double.IsNaN(value) || double.IsInfinity(value);
        }
        /// <summary>
        /// 非数値の場合はデフォルト値を返す。
        /// default(double) とか気取った書き方してるけど、要するに非数値の場合は 0 を返す。
        /// </summary>
        public static double Fix(this double value)
        {
            return value.IsNotNumber() ? default(double) : value;
        }
        /// <summary>
        /// [min, max] の区間に納めて返す。
        /// 区間が0以下の場合は min を返す。（Math.Clampだとエラーを返すので使えない）
        /// </summary>
        public static double Clamp(this double value, double min, double max)
        {
            return Math.Max(min, Math.Min(value, max));
        }
    }
}
