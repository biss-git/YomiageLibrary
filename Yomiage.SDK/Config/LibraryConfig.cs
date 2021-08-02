using System;
using System.Collections.Generic;
using System.Text;
using Yomiage.SDK.Settings;
using Yomiage.SDK.VoiceEffects;

namespace Yomiage.SDK.Config
{
    /// <summary>
    /// 音声ライブラリの設定内容
    /// library.config.json として音声ライブラリと一緒に置いておく
    /// </summary>
    public class LibraryConfig : ConfigBase
    {
        /// <summary>
        /// 不正値をはじく
        /// </summary>
        public override void Fix()
        {
            base.Fix();
        }
    }
}
