using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Yomiage.SDK;
using Yomiage.SDK.Common;
using Yomiage.SDK.Config;
using Yomiage.SDK.Settings;
using Yomiage.SDK.Talk;
using Yomiage.SDK.VoiceEffects;

namespace Yomiage.Core.Types
{
    public class VoicePreset : BindableBase
    {
        public string Key;
        public PresetType Type;
        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }
        public Engine Engine { get; set; }
        public Library Library { get; set; }
        public VoiceEffectValue VoiceEffect = new VoiceEffectValue();
        public VoiceEffectValue VoiceEffectSaved = null;
        public VoicePreset SubPreset { get; set; }
        private VoicePreset SubPresetSaved { get; set; }
        public string SubPresetKey;

        public string EngineKey { get => Engine.EngineConfig.Key; }
        public string LibraryKey { get => Library.LibraryConfig.Key; }

        private bool isDirty;
        [JsonIgnore]
        public bool IsDirty
        {
            get => isDirty;
            set => SetProperty(ref isDirty, value);
        }

        public VoicePreset(Engine Engine, Library Library)
        {
            this.Engine = Engine;
            this.Library = Library;
            Guid g = System.Guid.NewGuid();
            string id = g.ToString("N").Substring(0, 8);
            Key = Engine.EngineConfig.Key + "_/_" + Library.LibraryConfig.Key + "_" + id;
            Library.LibraryConfig.AdditionalSettings?.ForEach(s =>
            {
                this.VoiceEffect.AdditionalEffect.Add(s.Key, s.Value);
            });
        }

        public async Task<double[]> Play(TalkScript talkScript, MasterEffectValue masterEffectValue, Action<int> SetSamplingRate_Hz)
        {
            var script = JsonUtil.DeepClone(talkScript);
            (int shortPause, int longPause) = GetPauseSpan(masterEffectValue);
            script.Fill(Engine.EngineConfig, shortPause, longPause);
            try
            {
                return await Engine.VoiceEngine.Play(
                    new VoiceConfig(Library.VoiceLibrary, JsonUtil.DeepClone(VoiceEffect)),
                    new VoiceConfig(SubPreset?.Library.VoiceLibrary, JsonUtil.DeepClone(SubPreset?.VoiceEffect)),
                    script,
                    JsonUtil.DeepClone(masterEffectValue),
                    SetSamplingRate_Hz);
            }
            catch(Exception e)
            {
                // ログ出す？ エラー投げる？　何もなかったことにする！
                return null;
            }
        }
        public async Task Save(TalkScript[] talkScripts, MasterEffectValue masterEffectValue, string filePath,
            int startPause, int endPause, bool saveWithText, Encoding encoding)
        {
            var scripts = new List<TalkScript>();
            (int shortPause, int longPause) = GetPauseSpan(masterEffectValue);
            foreach(var s in talkScripts)
            {
                var script = JsonUtil.DeepClone(s);
                script.Fill(Engine.EngineConfig, shortPause, longPause);
                scripts.Add(script);
            }
            try
            {
                await Engine.VoiceEngine.Save(
                    new VoiceConfig(Library.VoiceLibrary, JsonUtil.DeepClone(VoiceEffect)),
                    new VoiceConfig(SubPreset?.Library.VoiceLibrary, JsonUtil.DeepClone(SubPreset?.VoiceEffect)),
                    scripts.ToArray(),
                    JsonUtil.DeepClone(masterEffectValue),
                    filePath,
                    startPause,
                    endPause,
                    saveWithText,
                    encoding);
            }
            catch (Exception e)
            {
                // ログ出す？ エラー投げる？　何もなかったことにする！
            }
        }

        private (int, int) GetPauseSpan(MasterEffectValue masterEffectValue)
        {
            int shortPause = (int)masterEffectValue.ShortPause;
            int longPause = (int)masterEffectValue.LongPause;
            if (VoiceEffect.PauseOverride)
            {
                shortPause = (int)VoiceEffect.ShortPause;
                longPause = (int)VoiceEffect.LongPause;
            }
            return (shortPause, longPause);
        }

        public void MakeSavedEffect()
        {
            if(this.VoiceEffect.AdditionalEffect == null)
            {
                this.VoiceEffect.AdditionalEffect = new Dictionary<string, double?>();
            }
            Library.LibraryConfig.AdditionalSettings?.ForEach(s =>
            {
                if (!this.VoiceEffect.AdditionalEffect.ContainsKey(s.Key) == true)
                {
                    this.VoiceEffect.AdditionalEffect.Add(s.Key, s.Value);
                }
            });
            VoiceEffectSaved = JsonUtil.DeepClone(VoiceEffect);
            SubPresetSaved = SubPreset;
        }
        public void ResetEffect()
        {
            if(VoiceEffectSaved == null) { return; }
            VoiceEffect = JsonUtil.DeepClone(VoiceEffectSaved);
            SubPreset = SubPresetSaved;
        }
    }

    public enum PresetType
    {
        Standard,
        User,
        External,
    }
}
