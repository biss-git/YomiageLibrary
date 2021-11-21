// <copyright file="BasicFormat.cs" company="bisu">
// © 2021 bisu
// </copyright>

using Yomiage.SDK.Common;
using Yomiage.SDK.Settings;

namespace Yomiage.SDK.Config
{
    /// <summary>
    /// 立ち絵用のフォーマット
    /// </summary>
    public class BasicFormat : IFixAble
    {
        /// <summary>
        /// 何もしていないときの立ち絵画像
        /// </summary>
        public string Base { get; set; }

        /// <summary>
        /// 発音しているときの口を空けている立ち絵画像（口パク用）
        /// </summary>
        public string MouthOpen { get; set; }

        /// <summary>
        /// 目を閉じている立ち絵画像（目パチ用）
        /// </summary>
        public string EyeClose { get; set; }

        /// <summary>
        /// 寝ているときの立ち絵１
        /// １と２は１秒くらいずつで交互に表示される。寝息用。
        /// </summary>
        public string Sleep1 { get; set; }

        /// <summary>
        /// 寝ているときの立ち絵２
        /// </summary>
        public string Sleep2 { get; set; }

        /// <inheritdoc/>
        public void Fix()
        {
            Base = Base.Fix();
            MouthOpen = MouthOpen.Fix();
            EyeClose = EyeClose.Fix();
            Sleep1 = Sleep1.Fix();
            Sleep2 = Sleep2.Fix();
        }
    }
}
