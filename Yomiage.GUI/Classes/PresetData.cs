using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core.Types;
using Yomiage.SDK.Common;
using Yomiage.SDK.VoiceEffects;

namespace Yomiage.GUI.Classes
{
    public class PresetData : IFixAble
    {
        public PresetType Type { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string EngineKey { get; set; }
        public string LibraryKey { get; set; }
        public VoiceEffectValue VoiceEffect { get; set; }
        public string SubPresetKey { get; set; }
        public bool IsSelected { get; set; }

        public PresetData()
        {

        }
        public PresetData(VoicePreset preset, bool isSelected)
        {
            this.Type = preset.Type;
            this.Key = preset.Key;
            this.Name = preset.Name;
            this.EngineKey = preset.Engine.EngineConfig.Key;
            this.LibraryKey = preset.Library.LibraryConfig.Key;
            this.SubPresetKey = preset.SubPreset?.Key;
            this.VoiceEffect = preset.VoiceEffect;
            this.IsSelected = isSelected;
        }


        public void Fix()
        {
        }
    }
}
