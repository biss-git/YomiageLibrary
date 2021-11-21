// <copyright file="StringExtension.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System;

namespace Yomiage.SDK.Settings
{
    /// <summary>
    /// string 拡張メソッド
    /// </summary>
    internal static class StringExtension
    {
        /// <summary>
        /// 不正な値をはじく
        /// </summary>
        /// <param name="value">値</param>
        /// <returns>_</returns>
        public static string Fix(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
