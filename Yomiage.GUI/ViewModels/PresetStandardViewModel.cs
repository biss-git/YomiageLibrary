using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Yomiage.Core.Types;
using Yomiage.Core.Models;
using Yomiage.GUI.Models;

namespace Yomiage.GUI.ViewModels
{
    class PresetStandardViewModel : ViewModelBase
    {
        public ReadOnlyReactiveCollection<VoicePreset> Presets { get; }
        public IFilteredReadOnlyObservableCollection<VoicePreset> FilterdPresets { get; }
        public ReactivePropertySlim<VoicePreset> SelectedPreset { get; }
        public ReactiveCommand CopyCommand { get; }
        public VoicePresetService VoicePresetService { get; }
        public ReactivePropertySlim<string> SearchText { get; } = new();

        private ReactivePropertySlim<bool> isSelected;
        LayoutService layoutService;
        public SettingService SettingService { get; }

        public PresetStandardViewModel(
            SettingService settingService,
            VoicePresetService voicePresetService,
            LayoutService layoutService,
            IDialogService dialogService) : base(dialogService)
        {
            settingService.IconSizeNum.Subscribe(_ =>
            {
            });
            this.SettingService = settingService;
            this.VoicePresetService = voicePresetService;
            this.layoutService = layoutService;

            isSelected = new ReactivePropertySlim<bool>(false).AddTo(Disposables);
            Presets = voicePresetService.StandardPreset.ToReadOnlyReactiveCollection();
            FilterdPresets = Presets.ToFilteredReadOnlyObservableCollection(x => x.IsVisible).AddTo(Disposables);
            CopyCommand = isSelected.ToReactiveCommand().WithSubscribe(CopyAction).AddTo(Disposables);
            SelectedPreset = new ReactivePropertySlim<VoicePreset>().AddTo(Disposables);
            SelectedPreset.Subscribe(preset =>
            {
                if (preset != null)
                {
                    this.VoicePresetService.SelectedPreset.Value = preset;
                }
            }).AddTo(Disposables);
            voicePresetService.SelectedPreset.Subscribe(preset =>
            {
                isSelected.Value = (preset != null && preset.Type == PresetType.Standard);
                SelectedPreset.Value = isSelected.Value ? preset : null;
            }).AddTo(Disposables);

            SearchText.Subscribe(text =>
            {
                foreach (var p in Presets)
                {
                    p.IsVisible = string.IsNullOrWhiteSpace(text) ||
                                  p.Name.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                                  p.Engine.Name.Contains(text, StringComparison.OrdinalIgnoreCase);
                }
            });
        }

        private void CopyAction()
        {
            if(this.VoicePresetService.SelectedPreset.Value == null) { return; }
            var result = MessageBox.Show(this.SelectedPreset.Value.Name + " のコピーを作成してよろしいですか？", "確認", MessageBoxButton.YesNo);
            if(result == MessageBoxResult.Yes)
            {
                this.VoicePresetService.Copy(this.SelectedPreset.Value);
                layoutService.ShowTabCommand.Execute(TabType.UserTab);
            }
        }
    }
}
