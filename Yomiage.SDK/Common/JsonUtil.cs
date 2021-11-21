// <copyright file="JsonUtil.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Yomiage.SDK.Common
{
    /// <summary>
    /// json にしてファイルに保存したり、読み込んだり、
    /// シリアライズを利用してディープクローンしたりするユーティリティクラス。
    /// </summary>
    public static class JsonUtil
    {
        /// <summary>
        /// ディープクローンを返す。
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="obj">対象</param>
        /// <returns>ディープクローン</returns>
        public static T DeepClone<T>(T obj)
            where T : class
        {
            if (obj == null)
            {
                return default;
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };
            var jsonstr = JsonSerializer.Serialize(obj, options);

            var x = JsonSerializer.Deserialize<T>(jsonstr);
            if (x is IFixAble xx)
            {
                xx.Fix();
            }

            return x;
        }

        /// <summary>
        /// json ファイルに保存する。
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="obj">シリアライズの対象</param>
        /// <param name="fileName">ファイル名</param>
        /// <param name="ignoreReadOnlyProperties">リードオンリーを無視するか</param>
        /// <param name="defaultIgnoreCondition">デフォルト値を無視するか</param>
        public static void Serialize<T>(T obj, string fileName, bool ignoreReadOnlyProperties = true, JsonIgnoreCondition defaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IgnoreReadOnlyProperties = ignoreReadOnlyProperties,
                DefaultIgnoreCondition = defaultIgnoreCondition,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };
            var jsonstr = JsonSerializer.Serialize(obj, options);
            File.WriteAllText(fileName, jsonstr);
        }

        /// <summary>
        /// 文字列にシリアライズ
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="obj">シリアライズの対象</param>
        /// <param name="writeIndented">インデント</param>
        /// <param name="ignoreReadOnlyProperties">リードオンリーを無視する</param>
        /// <param name="defaultIgnoreCondition">デフォルト値なら無視する</param>
        /// <returns>jsonテキスト</returns>
        public static string SerializeToString<T>(T obj, bool writeIndented = false, bool ignoreReadOnlyProperties = true, JsonIgnoreCondition defaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = writeIndented,
                IgnoreReadOnlyProperties = ignoreReadOnlyProperties,
                DefaultIgnoreCondition = defaultIgnoreCondition,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };
            return JsonSerializer.Serialize(obj, options);
        }

        /// <summary>
        /// json からデシリアライズ
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="fileName">ファイル名</param>
        /// <returns>デシリアライズされたインスタンス</returns>
        public static T Deserialize<T>(string fileName)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
            {
                return default;
            }

            var jsonstr = File.ReadAllText(fileName);
            try
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                };
                var x = JsonSerializer.Deserialize<T>(jsonstr, options);
                if (x is IFixAble xx)
                {
                    xx.Fix();
                }

                return x;
            }
            catch (Exception)
            {
            }

            return null;
        }

        /// <summary>
        /// 文字列からデシリアライズ
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="jsonstr">ファイル名</param>
        /// <returns>デシリアライズされたインスタンス</returns>
        public static T DeserializeFromString<T>(string jsonstr)
            where T : class
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                };
                var x = JsonSerializer.Deserialize<T>(jsonstr, options);
                if (x is IFixAble xx)
                {
                    xx.Fix();
                }

                return x;
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}
