using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        public static T DeepClone<T>(T obj) where T : class
        {
            if(obj == null) { return default(T); }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            var jsonstr = JsonSerializer.Serialize(obj, options);

            var x = JsonSerializer.Deserialize<T>(jsonstr);
            if(x is IFixAble xx)
            {
                xx.Fix();
            }
            return x;
        }

        /// <summary>
        /// json ファイルに保存する。
        /// </summary>
        public static void Serialize<T>(T obj, string fileName, bool IgnoreReadOnlyProperties = true, JsonIgnoreCondition DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IgnoreReadOnlyProperties = IgnoreReadOnlyProperties,
                DefaultIgnoreCondition = DefaultIgnoreCondition,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            var jsonstr = JsonSerializer.Serialize(obj, options);
            File.WriteAllText(fileName, jsonstr);
        }

        public static string SerializeToString<T>(T obj, bool WriteIndented = false, bool IgnoreReadOnlyProperties = true, JsonIgnoreCondition DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = WriteIndented,
                IgnoreReadOnlyProperties = IgnoreReadOnlyProperties,
                DefaultIgnoreCondition = DefaultIgnoreCondition,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            return JsonSerializer.Serialize(obj, options);
        }

        /// <summary>
        /// json からデシリアライズ
        /// </summary>
        public static T Deserialize<T>(string fileName) where T : class
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName)) { return default(T); }
            var jsonstr = File.ReadAllText(fileName);
            var x = JsonSerializer.Deserialize<T>(jsonstr);
            if (x is IFixAble xx)
            {
                xx.Fix();
            }
            return x;
        }

        public static T DeserializeFromString<T>(string jsonstr) where T : class
        {
            var x = JsonSerializer.Deserialize<T>(jsonstr);
            if (x is IFixAble xx)
            {
                xx.Fix();
            }
            return x;
        }

    }

}
