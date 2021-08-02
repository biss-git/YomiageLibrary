using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core.Types;
using Yomiage.SDK.Common;

namespace Yomiage.Core.Models
{
    public class VoicePresetService : IDisposable
    {
        public ReactivePropertySlim<VoicePreset> SelectedPreset { get; } = new ReactivePropertySlim<VoicePreset>();
        public string SelectedEngineKey => SelectedPreset.Value?.Engine?.EngineConfig?.Key;
        public string SelectedLibraryKey => SelectedPreset.Value?.Library?.LibraryConfig?.Key;

        private readonly ObservableCollection<VoicePreset> presets = new ObservableCollection<VoicePreset>();
        public ReadOnlyObservableCollection<VoicePreset> AllPresets { get; }
        public IFilteredReadOnlyObservableCollection<VoicePreset> StandardPreset { get; }
        public IFilteredReadOnlyObservableCollection<VoicePreset> UserPreset { get; }
        public IFilteredReadOnlyObservableCollection<VoicePreset> ExternalPreset { get; }


        public VoicePresetService(IMessageBroker messageBroker)
        {
            AllPresets = new ReadOnlyObservableCollection<VoicePreset>(presets);
            StandardPreset = AllPresets.ToFilteredReadOnlyObservableCollection(p => p.Type == PresetType.Standard).AddTo(disposables);
            UserPreset = AllPresets.ToFilteredReadOnlyObservableCollection(p => p.Type == PresetType.User).AddTo(disposables);
            ExternalPreset = AllPresets.ToFilteredReadOnlyObservableCollection(p => p.Type == PresetType.External).AddTo(disposables);
        }

        public bool Add(VoicePreset preset)
        {
            if (preset.Type == PresetType.Standard &&
                presets.Any(p => p.Type == preset.Type &&
                                 p.Engine.EngineConfig.Key == preset.Engine.EngineConfig.Key &&
                                 p.Library.LibraryConfig.Key == preset.Library.LibraryConfig.Key))
            {
                return false;
            }
            presets.Add(preset);
            return true;
        }

        public void Copy(VoicePreset preset, string name = null, bool select = true)
        {
            if(preset == null) { return; }
            if (string.IsNullOrWhiteSpace(name))
            {
                name = preset.Name + " - コピー";
            }
            var newPreset = new VoicePreset(preset.Engine, preset.Library) {
                Name = name,
                Type = PresetType.User,
                VoiceEffect = JsonUtil.DeepClone(preset.VoiceEffect),
                SubPreset = preset.SubPreset,
            };
            Add(newPreset);
            if (select)
            {
                SelectedPreset.Value = newPreset;
            }
        }

        public void Remove(VoicePreset preset)
        {
            if( presets.Contains(preset))
            {
                presets.Remove(preset);
            }
        }

        public void RemoveUserPresets()
        {
            for(int i=presets.Count-1; i>=0; i--)
            {
                if (presets[i].Type == PresetType.User) {
                    Remove(presets[i]);
                }
            }
        }

        private readonly CompositeDisposable disposables = new CompositeDisposable();
        public void Dispose() => disposables.Dispose();
    }
}
