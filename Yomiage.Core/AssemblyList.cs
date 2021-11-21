using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Yomiage.Core
{
    public static class AssemblyList
    {
        public static Assembly GetJPNKanaConv()
        {
            return Assembly.GetAssembly(typeof(Microsoft.International.Converters.KanaConverter));
        }
        public static Assembly GetLibNMeCab()
        {
            return Assembly.GetAssembly(typeof(NMeCab.MeCabTagger));
        }

    }
}
