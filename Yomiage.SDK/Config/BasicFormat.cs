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
        public string Base { get; set; } = "picture/Base.png";

        /// <summary>
        /// 発音しているときの口を空けている立ち絵画像（口パク用）
        /// </summary>
        public string MouthOpen { get; set; } = "picture/MouthOpen.png";

        /// <summary>
        /// 目を閉じている立ち絵画像（目パチ用）
        /// </summary>
        public string EyeClose { get; set; } = "picture/EyeClose.png";

        /// <summary>
        /// 寝ているときの立ち絵１
        /// １と２は１秒くらいずつで交互に表示される。寝息用。
        /// </summary>
        public string Sleep1 { get; set; } = "picture/Sleep1.png";

        /// <summary>
        /// 寝ているときの立ち絵２
        /// </summary>
        public string Sleep2 { get; set; } = "picture/Sleep2.png";

        /// <summary>
        /// 背景画像（ダークモード時）
        /// </summary>
        public string DarkBackground { get; set; } = "picture/DarkBackground.png";

        /// <summary>
        /// 背景画像（ライトモード時）
        /// </summary>
        public string LightBackground { get; set; } = "picture/LightBackground.png";

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
