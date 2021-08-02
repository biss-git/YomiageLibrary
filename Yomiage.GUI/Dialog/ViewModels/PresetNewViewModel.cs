using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core.Types;
using Yomiage.Core.Models;
using Yomiage.GUI.Models;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class PresetNewViewModel : DialogViewModelBase
    {
        public override string Title => "新規ボイスプリセット";

        public ReadOnlyReactiveCollection<VoicePreset> Presets { get; }
        public ReactivePropertySlim<VoicePreset> SelectedPreset { get; }
        public ReactivePropertySlim<string> PresetName { get; }
        private ReactivePropertySlim<bool> isSelected;

        VoicePresetService voicePresetService;

        public PresetNewViewModel(VoicePresetService voicePresetService, LayoutService layoutService)
        {
            this.voicePresetService = voicePresetService;

            Presets = voicePresetService.StandardPreset.ToReadOnlyReactiveCollection().AddTo(Disposables);
            SelectedPreset = new ReactivePropertySlim<VoicePreset>().AddTo(Disposables);
            PresetName = new ReactivePropertySlim<string>("新規プリセット").AddTo(Disposables);
            isSelected = new ReactivePropertySlim<bool>(false).AddTo(Disposables);
            OkCommand = isSelected.ToReactiveCommand().WithSubscribe(() =>
            {
                this.voicePresetService.Copy(SelectedPreset.Value, PresetName.Value);
                layoutService.ShowTabCommand.Execute(TabType.UserTab);
                base.RaiseRequestClose(null);
            }).AddTo(Disposables);
            CancelCommand = new ReactiveCommand().WithSubscribe(() => base.RaiseRequestClose(null)).AddTo(Disposables);
            SelectedPreset.Subscribe(preset =>
            {
                if(preset != null)
                {
                    PresetName.Value = preset.Name + " - 新規";
                }
                isSelected.Value = preset != null;
            }).AddTo(Disposables);
        }

    }
}
