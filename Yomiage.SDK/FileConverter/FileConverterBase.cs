// <copyright file="FileConverterBase.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System;
using System.Threading.Tasks;
using Yomiage.SDK.Talk;

namespace Yomiage.SDK.FileConverter
{
    /// <summary>
    /// ファイル読み込み用のベースクラス
    /// </summary>
    public class FileConverterBase : IFileConverter
    {
        /// <inheritdoc/>
        public virtual string[] ImportFilterList { get; }

        /// <inheritdoc/>
        public virtual void Dispose()
        {
        }

        /// <inheritdoc/>
        public virtual (string, TalkScript[]) Open(string filepath, string filter)
        {
            return (null, null);
        }
    }
}
