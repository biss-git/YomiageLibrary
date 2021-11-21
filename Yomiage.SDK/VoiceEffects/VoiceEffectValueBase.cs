// <copyright file="VoiceEffectValueBase.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Yomiage.SDK.Common;
using Yomiage.SDK.Config;

namespace Yomiage.SDK.VoiceEffects
{
    /// <summary>
    /// 基本的な音声情報
    /// 音量、話速、高さ、抑揚　の４つ。
    /// </summary>
    public abstract class VoiceEffectValueBase : IFixAble
    {
        /// <summary>
        /// モーラかどうか
        /// </summary>
        [JsonIgnore]
        public virtual bool IsMora => false;

        /// <summary>
        /// エンドセクションかどうか
        /// </summary>
        [JsonIgnore]
        public virtual bool IsEndSection => false;

        /// <summary>
        /// 音量の設定値
        /// </summary>
        [JsonPropertyName("V")]
        public double? Volume { get; set; }

        /// <summary>
        /// 話速の設定値
        /// </summary>
        [JsonPropertyName("S")]
        public double? Speed { get; set; }

        /// <summary>
        /// 高さの設定値
        /// </summary>
        [JsonPropertyName("P")]
        public double? Pitch { get; set; }

        /// <summary>
        /// 抑揚の設定値
        /// </summary>
        [JsonPropertyName("E")]
        public double? Emphasis { get; set; }

        /// <summary>
        /// 追加の設定
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, double?> AdditionalEffect { get; set; } = new Dictionary<string, double?>();

        /// <summary>
        /// 追加パラメータ（Json用）
        /// </summary>
        [JsonPropertyName("A")]
        public Dictionary<string, double?> AdditionalEffectIO
        {
            get
            {
                if (AdditionalEffect == null)
                {
                    return null;
                }

                if (AdditionalEffect.Any(pair => pair.Value != null))
                {
                    return AdditionalEffect;
                }

                return null;
            }

            set
            {
                if (value != null)
                {
                    AdditionalEffect = value;
                }
            }
        }

        /// <summary>
        /// Curve 用の追加パラメータ
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, double[]> AdditionalEffects { get; set; } = new Dictionary<string, double[]>();

        /// <summary>
        /// Curve 用の追加パラメータ（Json用）
        /// </summary>
        [JsonPropertyName("As")]
        public Dictionary<string, string> AdditionalEffectsString
        {
            get
            {
                if (AdditionalEffects == null || AdditionalEffects.Count == 0)
                {
                    return null;
                }

                var dict = new Dictionary<string, string>();
                foreach (var i in AdditionalEffects)
                {
                    var base64 = ToBase64(i.Value);
                    if (!string.IsNullOrWhiteSpace(base64))
                    {
                        dict.Add(i.Key, base64);
                    }
                }

                if (dict.Count == 0)
                {
                    return null;
                }

                return dict;
            }

            set
            {
                if (value == null || value.Count == 0)
                {
                    return;
                }

                AdditionalEffects = new Dictionary<string, double[]>();
                foreach (var i in value)
                {
                    var values = FromBase64(i.Value);
                    if (values != null)
                    {
                        AdditionalEffects.Add(i.Key, values);
                    }
                }
            }
        }

        /// <summary>
        /// 追加パラメータを取得する
        /// 取得できなかった場合はデフォルト値を返す
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>追加パラメータ</returns>
        public double GetAdditionalValueOrDefault(string key, double defaultValue = 0)
        {
            var val = GetAdditionalValue(key);
            return val == null ? defaultValue : (double)val;
        }

        /// <summary>
        /// 追加パラメータを取得する
        /// null が返ることもある
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>パラメータ値</returns>
        public double? GetAdditionalValue(string key)
        {
            if (AdditionalEffect != null &&
                !string.IsNullOrWhiteSpace(key) &&
                AdditionalEffect.ContainsKey(key))
            {
                return AdditionalEffect[key];
            }

            return null;
        }

        /// <summary>
        /// 追加パラメータを設定する
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        public void SetAdditionalValue(string key, double? value)
        {
            if (AdditionalEffect != null &&
                !string.IsNullOrWhiteSpace(key))
            {
                if (!AdditionalEffect.ContainsKey(key))
                {
                    AdditionalEffect.Add(key, null);
                }

                AdditionalEffect[key] = value;
            }
        }

        /// <summary>
        /// 追加パラメータを取得し、無ければデフォルト値で埋める
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>追加パラメータ</returns>
        public double[] GetAdditionalValuesOrAdd(string key, double defaultValue = 0)
        {
            if (AdditionalEffects != null &&
                !string.IsNullOrWhiteSpace(key))
            {
                if (!AdditionalEffects.ContainsKey(key))
                {
                    AdditionalEffects.Add(key, Enumerable.Repeat(defaultValue, 10).ToArray());
                }

                var val = AdditionalEffects[key];
                if (val != null)
                {
                    return val;
                }

                AdditionalEffects[key] = Enumerable.Repeat(defaultValue, 10).ToArray();
            }

            return Enumerable.Repeat(defaultValue, 10).ToArray();
        }

        /// <summary>
        /// 追加パラメータを取得し、無ければデフォルト値を返す
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="defaultValue">デフォルト値</param>
        /// <returns>追加パラメータ</returns>
        public double[] GetAdditionalValuesOrDefault(string key, double defaultValue = 0)
        {
            if (AdditionalEffects != null &&
                !string.IsNullOrWhiteSpace(key))
            {
                if (!AdditionalEffects.ContainsKey(key))
                {
                    return Enumerable.Repeat(defaultValue, 10).ToArray();
                }

                var val = AdditionalEffects[key];
                if (val != null)
                {
                    return val;
                }
            }

            return Enumerable.Repeat(defaultValue, 10).ToArray();
        }

        /// <summary>
        /// 追加パラメータを取得する
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>パラメータ</returns>
        public double[] GetAdditionalValues(string key)
        {
            if (AdditionalEffects != null &&
                !string.IsNullOrWhiteSpace(key) &&
                AdditionalEffects.ContainsKey(key))
            {
                return AdditionalEffects[key];
            }

            return null;
        }

        /// <summary>
        /// 追加パラメータを設定する
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        public void SetAdditionalValues(string key, double? value)
        {
            if (value == null)
            {
                SetAdditionalValues(key, (double[])null);
                return;
            }

            var values = GetAdditionalValues(key);
            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = (double)value;
                }
            }
        }

        /// <summary>
        /// 追加パラメータを設定する
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="values">値</param>
        public void SetAdditionalValues(string key, double[] values)
        {
            if (AdditionalEffects != null &&
                !string.IsNullOrWhiteSpace(key))
            {
                if (!AdditionalEffects.ContainsKey(key))
                {
                    AdditionalEffects.Add(key, values);
                }
                else
                {
                    AdditionalEffects[key] = values;
                }
            }
        }

        /// <inheritdoc/>
        public void Fix()
        {
        }

        /// <summary>
        /// 不要なパラメータを削除する。
        /// </summary>
        /// <param name="engineConfig">エンジンコンフィグ</param>
        public void RemoveUnnecessaryParameters(EngineConfig engineConfig)
        {
            if (engineConfig.VolumeSetting.Hide || !engineConfig.VolumeSetting.CheckIsMora(IsMora, IsEndSection))
            {
                this.Volume = null;
            }

            if (engineConfig.SpeedSetting.Hide || !engineConfig.SpeedSetting.CheckIsMora(IsMora, IsEndSection))
            {
                this.Speed = null;
            }

            if (engineConfig.PitchSetting.Hide || !engineConfig.PitchSetting.CheckIsMora(IsMora, IsEndSection))
            {
                this.Pitch = null;
            }

            if (engineConfig.EmphasisSetting.Hide || !engineConfig.EmphasisSetting.CheckIsMora(IsMora, IsEndSection))
            {
                this.Emphasis = null;
            }

            {
                var keys = AdditionalEffect.Keys.ToList();
                foreach (var key in keys)
                {
                    if (!engineConfig.AdditionalSettings.Any(s =>
                        s.Key == key && s.Type != "Curve" && !s.Hide && s.CheckIsMora(IsMora, IsEndSection)))
                    {
                        AdditionalEffect.Remove(key);
                    }
                }
            }

            {
                var keys = AdditionalEffects.Keys.ToList();
                foreach (var key in keys)
                {
                    if (!engineConfig.AdditionalSettings.Any(s =>
                        s.Key == key && s.Type == "Curve" && !s.Hide && s.CheckIsMora(IsMora, IsEndSection)))
                    {
                        AdditionalEffects.Remove(key);
                    }
                }
            }
        }

        private static string ToBase64(double[] values)
        {
            if (values == null || values.Length != 10)
            {
                return null;
            }

            byte[] bytes = new byte[40];
            Buffer.BlockCopy(values.Select(v => (float)v).ToArray(), 0, bytes, 0, bytes.Length);
            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        private static double[] FromBase64(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
            {
                return null;
            }

            byte[] bytes = Convert.FromBase64String(base64);
            if (bytes.Length != 40)
            {
                return null;
            }

            var values = new float[10];
            Buffer.BlockCopy(bytes, 0, values, 0, bytes.Length);
            return values.Select(v => (double)v).ToArray();
        }
    }
}
