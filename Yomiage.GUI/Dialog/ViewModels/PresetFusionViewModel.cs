using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core.Models;
using Yomiage.Core.Types;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class PresetFusionViewModel : DialogViewModelBase
    {
        public override string Title => "サブプリセット選択";

        public ReactivePropertySlim<VoicePreset[]> Presets { get; } = new();
        public ReactivePropertySlim<VoicePreset> Preset { get; } = new();
        public ReactivePropertySlim<VoicePreset> SubPreset { get; } = new();

        public ReactiveCommand ReleaseCommand { get; }

        private ReactivePropertySlim<bool> CanRelease = new();

        public PresetFusionViewModel(
            VoicePresetService voicePresetService)
        {
            var list = new List<VoicePreset>();
            foreach( var p in voicePresetService.AllPresets)
            {
                list.Add(p);
            }
            Presets.Value = list.ToArray();
            ReleaseCommand = CanRelease.ToReactiveCommand().WithSubscribe(ReleaseAction).AddTo(Disposables);
            SubPreset.Subscribe(p => CanRelease.Value = p != null);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Preset"))
            {
                Preset.Value = parameters.GetValue<VoicePreset>("Preset");
                SubPreset.Value = Preset.Value?.SubPreset;
            }
            if (Preset == null)
            {
                CancelAction();
                return;
            }
        }

        protected override void OkAction()
        {
            if(Preset.Value != null)
            {
                Preset.Value.SubPreset = SubPreset.Value;
            }
            base.OkAction();
        }

        private void ReleaseAction()
        {
            SubPreset.Value = null;
        }
    }
}
