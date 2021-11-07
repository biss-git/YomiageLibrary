using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Yomiage.SDK.Config;
using Yomiage.SDK.VoiceEffects;

namespace Yomiage.SDK.Talk
{
    /// <summary>
    /// 語尾情報
    /// </summary>
    public class EndSection : VoiceEffectValueBase
    {
        /// <summary>
        /// 
        /// </summary>
        public override bool IsEndSection => true;

        /// <summary>
        /// フレーズ編集の最後のポーズ
        /// </summary>
        [JsonIgnore]
        public Pause Pause { get; set; } = new Pause();
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("p")]
        public string P
        {
            get => Pause.P;
            set => Pause.P = value;
        }
        /// <summary>
        /// "！" , "？", "。", "♪", "" など
        /// </summary>
        [JsonPropertyName("C")]
        public string EndSymbol { get; set; }

        /// <summary>
        /// nullになっている部分を全て埋める
        /// </summary>
        public void Fill(VoiceEffectValue section, VoiceEffectValue mora, EngineConfig config, int shortPause, int longPause)
        {
            if (EndSymbol == null)
            {
                EndSymbol = "";
            }
            if (Volume == null)
            {
                Volume = (config.VolumeSetting.Type == "Mora") ? mora.Volume : section.Volume;
            }
            if (Speed == null)
            {
                Speed = (config.SpeedSetting.Type == "Mora") ? mora.Speed : section.Speed;
            }
            if (Pitch == null)
            {
                Pitch = (config.PitchSetting.Type == "Mora") ? mora.Pitch : section.Pitch;
            }
            if (Emphasis == null)
            {
                Emphasis = (config.EmphasisSetting.Type == "Mora") ? mora.Emphasis : section.Emphasis;
            }
            config.AdditionalSettings?.ForEach(s =>
            {
                var val = GetAdditionalValue(s.Key);
                if (val == null)
                {
                    SetAdditionalValue(s.Key, (s.Type == "Mora") ?
                        mora.GetAdditionalValue(s.Key) :
                        section.GetAdditionalValue(s.Key));
                }
            });
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
        /// 
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="config"></param>
        public void FillCurve(VoiceEffectValue curve, EngineConfig config)
        {
            config.AdditionalSettings?.ForEach(s =>
            {
                if (s.Type != "Curve") { return; }
                var val = GetAdditionalValuesOrDefault(s.Key, s.DefaultValue);
                var list = new List<double>(val)
                {
                    val.Last()
                };
                SetAdditionalValues(s.Key, list.ToArray());
                curve.SetAdditionalValues(s.Key, list.ToArray());
            });
        }
    }
}
