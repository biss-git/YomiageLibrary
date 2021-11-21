// <copyright file="DoubleExtension.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System;

namespace Yomiage.SDK.Settings
{
    /// <summary>
    /// double の拡張メソッド
    /// </summary>
    internal static class DoubleExtension
    {
        /// <summary>
        /// 非数値の判定
        /// </summary>
        /// <param name="value">double</param>
        /// <returns>非数値かどうか</returns>
        public static bool IsNotNumber(this double value)
        {
            return double.IsNaN(value) || double.IsInfinity(value);
        }

        /// <summary>
        /// 非数値の場合はデフォルト値を返す。
        /// default(double) とか気取った書き方してるけど、要するに非数値の場合は 0 を返す。
        /// </summary>
        /// <param name="value">double</param>
        /// <returns>_</returns>
        public static double Fix(this double value)
        {
            return value.IsNotNumber() ? default(double) : value;
        }

        /// <summary>
        /// [min, max] の区間に納めて返す。
        /// 区間が0以下の場合は min を返す。（Math.Clampだとエラーを返すので使えない）
        /// </summary>
        /// <param name="value">double</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <returns>_</returns>
        public static double Clamp(this double value, double min, double max)
        {
            return Math.Max(min, Math.Min(value, max));
        }
    }
}
