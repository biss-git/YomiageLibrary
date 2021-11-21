// <copyright file="IFixAble.cs" company="bisu">
// © 2021 bisu
// </copyright>

namespace Yomiage.SDK.Common
{
    /// <summary>
    /// これを継承しているやつは
    /// JsonUtil でオブジェクトクローンしたり、
    /// json にしてファイルに保存したりするときに、
    /// 不正値をはじく処理を実行する。
    ///
    /// Common という名前空間はなんかダサい。Utilとかのほうが良かったかも。
    /// </summary>
    public interface IFixAble
    {
        /// <summary>
        /// 不正値をはじく
        /// </summary>
        void Fix();
    }
}
