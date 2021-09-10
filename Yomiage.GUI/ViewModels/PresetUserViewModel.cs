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
using Yomiage.GUI.Dialog.Views;
using Yomiage.GUI.Models;

namespace Yomiage.GUI.ViewModels
{
    class PresetUserViewModel : ViewModelBase
    {
        public ReadOnlyReactiveCollection<VoicePreset> Presets { get; }
        public IFilteredReadOnlyObservableCollection<VoicePreset> FilterdPresets { get; }
        public ReactivePropertySlim<VoicePreset> SelectedPreset { get; } = new();

        public ReactiveCommand CreateCommand { get; }
        public ReactiveCommand CopyCommand { get; } = new();
        public ReactiveCommand DeleteCommand { get; } = new();
        private ReactivePropertySlim<bool> isSelected = new(false);
        public ReactivePropertySlim<string> SearchText { get; } = new();

        public VoicePresetService VoicePresetService { get; }
        public SettingService SettingService { get; }

        public PresetUserViewModel(
            SettingService settingService,
            VoicePresetService voicePresetService,
            IDialogService _dialogService) : base(_dialogService)
        {
            this.SettingService = settingService;
            this.VoicePresetService = voicePresetService;

            Presets = voicePresetService.UserPreset.ToReadOnlyReactiveCollection();
            FilterdPresets = Presets.ToFilteredReadOnlyObservableCollection(x => x.IsVisible).AddTo(Disposables);


            CreateCommand = new ReactiveCommand().WithSubscribe(CreateAction).AddTo(Disposables);
            CopyCommand = isSelected.ToReactiveCommand().WithSubscribe(CopyAction).AddTo(Disposables);
            DeleteCommand = isSelected.ToReactiveCommand().WithSubscribe(DeleteAction).AddTo(Disposables);

            SelectedPreset = new ReactivePropertySlim<VoicePreset>().AddTo(Disposables);
            SelectedPreset.Subscribe(preset =>
            {
                if(preset != null)
                {
                    this.VoicePresetService.SelectedPreset.Value = preset;
                }
            }).AddTo(Disposables);
            voicePresetService.SelectedPreset.Subscribe(preset =>
            {
                isSelected.Value = (preset != null && preset.Type == PresetType.User);
                SelectedPreset.Value = isSelected.Value ? preset : null;
            }).AddTo(Disposables);

            SearchText.Subscribe(text =>
            {
                foreach(var p in Presets)
                {
                    p.IsVisible = string.IsNullOrWhiteSpace(text) ||
                                  p.Name.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                                  p.Engine.Name.Contains(text, StringComparison.OrdinalIgnoreCase);
                }
            });
        }

        private void CreateAction()
        {
            this.DialogService?.ShowDialog("PresetNewDialog", new DialogParameters(), result => {});
        }

        private void CopyAction()
        {
            if (SelectedPreset.Value == null) { return; }
            var result = MessageBox.Show(SelectedPreset.Value.Name + " のコピーを作成してよろしいですか？", "確認", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.VoicePresetService.Copy(this.SelectedPreset.Value);
            }
        }

        private void DeleteAction()
        {
            if (this.SelectedPreset.Value == null) { return; }
            var result = MessageBox.Show(this.SelectedPreset.Value.Name + " を削除してよろしいですか？", "確認", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.VoicePresetService.Remove(this.SelectedPreset.Value);
            }
        }
    }
}
