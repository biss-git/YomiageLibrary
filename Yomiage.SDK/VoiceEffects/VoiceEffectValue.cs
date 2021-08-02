using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yomiage.SDK.Config;
using Yomiage.SDK.Settings;

namespace Yomiage.SDK.VoiceEffects
{
    /// <summary>
    /// プリセット毎の音声効果
    /// </summary>
    public class VoiceEffectValue : VoiceEffectValueBase
    {
        /// <summary>
        /// 短・長ポーズをボイス毎の設定を使うか
        /// </summary>
        public bool PauseOverride { get; set; }
        /// <summary>
        /// 短ポーズの設定値
        /// </summary>
        public double ShortPause { get; set; }
        /// <summary>
        /// 長ポーズの設定値
        /// </summary>
        public double LongPause { get; set; }

        public VoiceEffectValue() { }
        public VoiceEffectValue(EngineConfig config)
        {
            Volume = config.VolumeSetting.DefaultValue;
            Speed = config.SpeedSetting.DefaultValue;
            Pitch = config.PitchSetting.DefaultValue;
            Emphasis = config.EmphasisSetting.DefaultValue;
            config.AdditionalSettings?.ForEach(s =>
            {
                if(s.Type == "Curve")
                {
                    AdditionalEffects.Add(s.Key, Enumerable.Repeat(s.DefaultValue, 11).ToArray());
                }
                else
                {
                    AdditionalEffect.Add(s.Key, s.DefaultValue);
                }
            });
        }

    }
}
