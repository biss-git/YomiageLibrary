// <copyright file="VoiceConfig.cs" company="bisu">
// © 2021 bisu
// </copyright>

using Yomiage.SDK.VoiceEffects;

namespace Yomiage.SDK
{
    /// <summary>
    /// ボイス設定
    /// </summary>
    public class VoiceConfig
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="library"> 音声ライブラリ </param>
        /// <param name="voiceEffect"> 音声効果 </param>
        public VoiceConfig(IVoiceLibrary library, VoiceEffectValue voiceEffect)
        {
            this.Library = library;
            this.VoiceEffect = voiceEffect;
        }

        /// <summary>
        /// 音声ライブラリー
        /// </summary>
        public IVoiceLibrary Library { get; }

        /// <summary>
        /// 音声効果
        /// </summary>
        public VoiceEffectValue VoiceEffect { get; }
    }
}
