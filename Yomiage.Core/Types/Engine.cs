using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.SDK;
using Yomiage.SDK.Config;
using Yomiage.SDK.Settings;

namespace Yomiage.Core.Types
{
    public class Engine
    {
        public string Name { get { return EngineConfig.Name; } }
        public string ConfigDirectory { get; }
        public string DllDirectory { get; }
        public string IconPath => Path.Combine(ConfigDirectory, "icon.png");
        public string SettingPath => Path.Combine(ConfigDirectory, "engine.settings.json");
        public IVoiceEngine VoiceEngine { get; }
        public EngineConfig EngineConfig { get; }
        public EngineSettings EngineSettings { get; set; }

        public Engine(string configDirectory, string dllDirectory, IVoiceEngine VoiceEngine, EngineConfig EngineConfig, EngineSettings EngineSettings)
        {
            VoiceEngine.Initialize(configDirectory, dllDirectory, EngineConfig);
            this.ConfigDirectory = configDirectory;
            this.DllDirectory = dllDirectory;
            this.VoiceEngine = VoiceEngine;
            this.EngineConfig = EngineConfig;
            this.EngineSettings = EngineSettings;
            VoiceEngine.Settings = EngineSettings;
        }
    }
}
