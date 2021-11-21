// <copyright file="PauseType.cs" company="bisu">
// © 2021 bisu
// </copyright>

namespace Yomiage.SDK.Talk
{
    /// <summary>
    /// ポーズの種類
    /// </summary>
    public enum PauseType
    {
        /// <summary>
        /// ポーズ無し
        /// </summary>
        None,

        /// <summary>
        /// 指定された時間
        /// </summary>
        Manual,

        /// <summary>
        /// 短ポーズ
        /// </summary>
        Short,

        /// <summary>
        /// 長ポーズ
        /// </summary>
        Long,
    }
}
