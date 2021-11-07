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
    /// モーラ情報。
    /// モーラ も VoiceEffectValueBase を継承して、音量や話速をモーラ単位で設定できるようにしているのは誤りではない。
    /// 今後、全分割せずに、各モーラの音声効果を設定できるようなオプションをつけるときのため。つけるかわからないけど。
    /// </summary>
    public class Mora : VoiceEffectValueBase
    {
        /// <summary>
        /// 
        /// </summary>
        public override bool IsMora => true;

        /// <summary>
        /// モーラの記号
        /// 日本語の場合は　ア　とか全角文字で来るはず。
        /// </summary>
        [JsonIgnore]
        public string Character { get; set; }
        /// <summary>
        /// アクセント
        /// true なら上、false なら下 (上とか下とかはGUI上の丸の位置のこと)
        /// </summary>
        [JsonIgnore]
        public bool Accent { get; set; }
        /// <summary>
        /// 無声化のこと。
        /// true : 無声音 の指定 ▽
        /// false: 有声音 の指定 ▼
        /// null : 指定なし
        /// </summary>
        [JsonIgnore]
        public bool? Voiceless { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string C
        {
            get
            {
                return Character +
                    (Accent ? "A" : "_") +
                    (Voiceless == true ? "D" :
                     Voiceless == false ? "V" : "");
            }
            set
            {
                switch (value[^1..])
                {
                    case "D":
                        Voiceless = true;
                        Accent = "A" == value.Substring(value.Length - 2, 1);
                        Character = value[0..^2];
                        break;
                    case "V":
                        Voiceless = false;
                        Accent = "A" == value.Substring(value.Length - 2, 1);
                        Character = value[0..^2];
                        break;
                    default:
                        Accent = "A" == value.Substring(value.Length - 1, 1);
                        Character = value[0..^1];
                        break;
                }
            }
        }

        /// <summary>
        /// nullになっている部分を全て埋める
        /// </summary>
        public void Fill(VoiceEffectValue mora)
        {
            Volume ??= mora.Volume;
            mora.Volume = Volume;
            Speed ??= mora.Speed;
            mora.Speed = Speed;
            Pitch ??= mora.Pitch;
            mora.Pitch = Pitch;
            Emphasis ??= mora.Emphasis;
            mora.Emphasis = Emphasis;
            foreach (var s in mora.AdditionalEffect)
            {
                var val = GetAdditionalValue(s.Key);
                if (val == null)
                {
                    SetAdditionalValue(s.Key, s.Value);
                }
            }
            foreach (var s in AdditionalEffect)
            {
                mora.SetAdditionalValue(s.Key, s.Value);
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
                    curve.GetAdditionalValuesOrDefault(s.Key, s.DefaultValue).First()
                };
                SetAdditionalValues(s.Key, list.ToArray());
                curve.SetAdditionalValues(s.Key, list.ToArray());
            });
        }
    }
}
