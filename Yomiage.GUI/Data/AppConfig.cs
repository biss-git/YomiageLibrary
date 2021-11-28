using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Yomiage.GUI.Data
{
    public class AppConfig
    {
        public string EngineDirectory { get; set; } = string.Empty;
        public string LibraryDirectory { get; set; } = string.Empty;
        public bool AllowMultiProcess { get; set; } = true;
        public bool DocumentDirectoryMode { get; set; } = false;
        public int PortNumber { get; set; } = 42503;

        public static void SetCurrent(AppConfig config)
        {
            Application.Current.Properties["AppConfig"] = config;
        }
        public static AppConfig GetCurrent()
        {
            if (Application.Current.Properties.Contains("AppConfig") &&
                Application.Current.Properties["AppConfig"] is AppConfig config)
            {
                return config;
            }
            return new AppConfig();
        }
    }
}
