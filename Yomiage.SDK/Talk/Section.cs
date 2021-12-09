// <copyright file="Section.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Yomiage.SDK.Config;
using Yomiage.SDK.VoiceEffects;

namespace Yomiage.SDK.Talk
{
    /// <summary>
    /// アクセント句(ポーズ + 複数モーラ)のこと。
    /// </summary>
    public class Section : VoiceEffectValueBase
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public override string Type => "Section";

        /// <summary>
        /// アクセント句の直前のポーズ情報
        /// </summary>
        [JsonIgnore]
        public Pause Pause { get; set; } = new Pause();

        /// <summary>
        /// ポーズ（Json用）
        /// </summary>
        [JsonPropertyName("p")]
        public string P
        {
            get => Pause.P;
            set => Pause.P = value;
        }

        /// <summary>
        /// アクセント句のモーラ情報
        /// </summary>
        [JsonPropertyName("M")]
        public List<Mora> Moras { get; set; } = new List<Mora>();

        /// <summary>
        /// nullになっている部分を全て埋める
        /// </summary>
        /// <param name="section">セクションを埋めるための値</param>
        /// <param name="mora">モーラを埋めるための値</param>
        /// <param name="shortPause">短ポーズ</param>
        /// <param name="longPause">長ポーズ</param>
        public void Fill(VoiceEffectValue section, VoiceEffectValue mora, int shortPause, int longPause)
        {
            Volume ??= section.Volume;
            section.Volume = Volume;
            Speed ??= section.Speed;
            section.Speed = Speed;
            Pitch ??= section.Pitch;
            section.Pitch = Pitch;
            Emphasis ??= section.Emphasis;
            section.Emphasis = Emphasis;
            foreach (var s in section.AdditionalEffect)
            {
                var val = GetAdditionalValue(s.Key);
                if (val == null)
                {
                    SetAdditionalValue(s.Key, s.Value);
                }
            }

            foreach (var s in AdditionalEffect)
            {
                section.SetAdditionalValue(s.Key, s.Value);
            }

            Moras.ForEach(m => m.Fill(mora));
            switch (Pause.Type)
            {
                case PauseType.Long:
                    Pause.Span_ms = longPause;
                    break;
                case PauseType.Short:
                    Pause.Span_ms = shortPause;
                    break;
                case PauseType.None:
                    Pause.Span_ms = 0;
                    break;
            }
        }

        /// <summary>
        /// Curve を埋める
        /// </summary>
        /// <param name="curve">Curve 値を埋めるための値</param>
        /// <param name="config">エンジンコンフィグ</param>
        public void FillCurve(VoiceEffectValue curve, EngineConfig config)
        {
            for (int i = Moras.Count - 1; i >= 0; i--)
            {
                Moras[i].FillCurve(curve, config);
            }

            config.AdditionalSettings?.ForEach(s =>
            {
                if (s.Type != "Curve")
                {
                    return;
                }

                var val = GetAdditionalValuesOrDefault(s.Key, s.DefaultValue);
                var list = new List<double>(val)
                {
                    curve.GetAdditionalValuesOrDefault(s.Key, s.DefaultValue).First(),
                };
                SetAdditionalValues(s.Key, list.ToArray());
                curve.SetAdditionalValues(s.Key, list.ToArray());
            });
        }

        /// <summary>
        /// 読みを取得する
        /// </summary>
        /// <returns>読み</returns>
        public string GetYomi()
        {
            return string.Join(string.Empty, Moras.Select(m => m.Character));
        }
    }
}
