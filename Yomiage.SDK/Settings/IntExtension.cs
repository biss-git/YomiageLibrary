// <copyright file="IntExtension.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System;

namespace Yomiage.SDK.Settings
{
    /// <summary>
    /// int 用拡張メソッド
    /// </summary>
    internal static class IntExtension
    {
        /// <summary>
        /// 非数値の判定
        /// </summary>
        /// <param name="value">int</param>
        /// <returns>_</returns>
        public static bool IsNotNumber(this int value)
        {
            return double.IsNaN(value) || double.IsInfinity(value);
        }

        /// <summary>
        /// 非数値の場合はデフォルト値を返す。
        /// </summary>
        /// <param name="value">int</param>
        /// <returns>_</returns>
        public static int Fix(this int value)
        {
            return value.IsNotNumber() ? default(int) : value;
        }

        /// <summary>
        /// [min, max] の区間に納めて返す。
        /// 区間が0以下の場合は min を返す。（Math.Clampだとエラーを返すので使えない）
        /// </summary>
        /// <param name="value">int</param>
        /// <param name="min">min</param>
        /// <param name="max">max</param>
        /// <returns>_</returns>
        public static int Clamp(this int value, int min, int max)
        {
            return Math.Max(min, Math.Min(value, max));
        }
    }
}
