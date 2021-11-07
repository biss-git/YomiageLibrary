using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Data
{
    public class AppConfig
    {
        public string EngineDirectory { get; set; } = string.Empty;
        public string LibraryDirectory { get; set; } = string.Empty;
        public bool AllowMultiProcess { get; set; } = true;
        public bool DocumentDirectoryMode { get; set; } = false;
        public int PortNumber { get; set; } = 42503;
    }
}
