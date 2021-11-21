// <copyright file="IFileConverter.cs" company="bisu">
// © 2021 bisu
// </copyright>

using System;
using System.Threading.Tasks;
using Yomiage.SDK.Talk;

namespace Yomiage.SDK.FileConverter
{
    /// <summary>
    /// ファイルのインポータ用インターフェイス
    /// </summary>
    public interface IFileConverter : IDisposable
    {
        /// <summary>
        /// インポートできるファイル用のフィルターリスト
        /// "テキスト|*.txt" みたいな感じ
        /// </summary>
        string[] ImportFilterList { get; }

        /// <summary>
        /// ファイルを開く処理
        /// </summary>
        /// <param name="filepath">ファイル名</param>
        /// <param name="filter">選択されたフィルター</param>
        /// <returns>(テキスト、ローカル辞書)</returns>
        (string, TalkScript[]) Open(string filepath, string filter);
    }
}
