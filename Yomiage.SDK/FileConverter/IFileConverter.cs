using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yomiage.SDK.Talk;

namespace Yomiage.SDK.FileConverter
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFileConverter : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        string[] ImportFilterList { get; }

        /// <summary>
        /// 
        /// </summary>
        void Initialize();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<(string, TalkScript[])> Open(string filepath, string filter);
    }
}
