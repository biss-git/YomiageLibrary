// <copyright file="LibraryConfig.cs" company="bisu">
// © 2021 bisu
// </copyright>

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
