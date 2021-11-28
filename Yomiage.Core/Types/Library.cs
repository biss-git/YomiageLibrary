using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.SDK;
using Yomiage.SDK.Config;
using Yomiage.SDK.Settings;
using Yomiage.SDK.VoiceEffects;

namespace Yomiage.Core.Types
{
    public class Library
    {
        public string Name { get { return LibraryConfig.Name; } }
        public string ConfigDirectory { get; }
        public string DllDirectory { get; }
        public string PairEngineConfigDirectory { get; }
        public string IconPath => Path.Combine(ConfigDirectory, "icon.png");
        public string SettingPath => Path.Combine(ConfigDirectory, "library.settings.json");
        public IVoiceLibrary VoiceLibrary { get; }
        public LibraryConfig LibraryConfig { get; }
        public LibrarySettings LibrarySettings { get; set; }
        public CharacterConfig CharacterConfig { get; }


        public Library(string configDirectory, string dllDirectory, IVoiceLibrary VoiceLibrary, LibraryConfig LibraryConfig, LibrarySettings LibrarySettings, CharacterConfig characterConfig, string pairEngineConfigDirectory = null)
        {
            VoiceLibrary.Initialize(configDirectory, dllDirectory, LibraryConfig);
            VoiceLibrary.Settings = LibrarySettings;
            this.ConfigDirectory = configDirectory;
            this.DllDirectory = dllDirectory;
            this.VoiceLibrary = VoiceLibrary;
            this.LibraryConfig = LibraryConfig;
            this.LibrarySettings = LibrarySettings;
            this.CharacterConfig = characterConfig;
            this.PairEngineConfigDirectory = pairEngineConfigDirectory;
        }

    }
}
