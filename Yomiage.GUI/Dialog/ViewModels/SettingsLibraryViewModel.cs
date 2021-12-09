using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Yomiage.Core.Types;
using Yomiage.SDK.Common;
using Yomiage.SDK.Settings;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class SettingsLibraryViewModel : DialogViewModelBase
    {
        public override string Title => "ライブラリ設定";

        public Library Library { get; set; }

        public ReactiveCollection<ISetting> Settings { get; } = new ReactiveCollection<ISetting>();

        public ReactivePropertySlim<string> Name { get; } = new();
        public ReactivePropertySlim<string> Description { get; } = new();
        public ReactivePropertySlim<string[]> LibraryFormat { get; } = new();
        public ReactivePropertySlim<string> OS { get; } = new();
        public ReactivePropertySlim<string> Language { get; } = new();
        public ReactivePropertySlim<string> ActivationKey { get; } = new();
        public ReactivePropertySlim<bool> Activated { get; } = new();
        public ReadOnlyReactivePropertySlim<bool> NotActivated { get; }
        public ReactivePropertySlim<string> LastState { get; } = new();
        public ReactivePropertySlim<int> MajorVersion { get; } = new();
        public ReactivePropertySlim<int> MinorVersion { get; } = new();

        public ReactivePropertySlim<LibrarySettings> SettingsEdit { get; } = new();

        public ReactiveCommand OpenLicenseCommand { get; }
        public AsyncReactiveCommand<string> ActivationCommand { get; }
        public ReactiveCommand DefaultCommand { get; }
        public ReactiveCommand ApplyCommand { get; }
        public ReactiveCommand OpenFolderCommand { get; }

        private ReactiveTimer timer = new(new TimeSpan(0, 0, 1));

        private IDialogService dialogService;

        public SettingsLibraryViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
            OpenLicenseCommand = new ReactiveCommand().WithSubscribe(OpenLicenseAction).AddTo(Disposables);
            ActivationCommand = new AsyncReactiveCommand<string>().WithSubscribe(ActivationAction).AddTo(Disposables);
            DefaultCommand = new ReactiveCommand().WithSubscribe(DefaultAction).AddTo(Disposables);
            ApplyCommand = new ReactiveCommand().WithSubscribe(ApplyAction).AddTo(Disposables);
            OpenFolderCommand = new ReactiveCommand().WithSubscribe(OpenFolderAction).AddTo(Disposables);
            NotActivated = Activated.Select(x => !x).ToReadOnlyReactivePropertySlim();
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Library"))
            {
                Library = parameters.GetValue<Library>("Library");
            }
            if (Library == null)
            {
                CancelAction();
                return;
            }
            Initialize();
        }

        private void OpenFolderAction()
        {
            Process.Start("EXPLORER.EXE", this.Library.ConfigDirectory);
        }

        private void Initialize()
        {
            var config = Library.LibraryConfig;
            Name.Value = config.Name;
            Description.Value = config.Description;
            LibraryFormat.Value = config.LibraryFormat;
            OS.Value = config.OS;
            Language.Value = config.Language;
            MajorVersion.Value = Library.VoiceLibrary.MajorVersion;
            MinorVersion.Value = Library.VoiceLibrary.MinorVersion;
            Activated.Value = Library.VoiceLibrary.IsActivated;
            timer.Subscribe(_ => LastState.Value = Library.VoiceLibrary.StateText);
            timer.Start();
            SetSettingList();
        }

        private void SetSettingList()
        {
            var list = new List<ISetting>();
            var settings = JsonUtil.DeepClone(Library.VoiceLibrary.Settings);
            SettingsEdit.Value = settings;
            settings?.Bools?
                .Where(s => !s.Hide)?
                .ToList()?
                .ForEach(s => list.Add(s));
            settings?.Ints?
                .Where(s => !s.Hide)?
                .ToList()?
                .ForEach(s => list.Add(s));
            settings?.Doubles?
                .Where(s => !s.Hide)?
                .ToList()?
                .ForEach(s => list.Add(s));
            settings?.Strings?
                .Where(s => !s.Hide)?
                .ToList()?
                .ForEach(s => list.Add(s));
            Settings.Clear();
            list.OrderBy(s => s.Order)
                .ToList()
                .ForEach(s => Settings.Add(s));
        }

        private void OpenLicenseAction()
        {
            if (OpenText(Path.Combine(Library.ConfigDirectory, "license.md"))) { return; }
            if (OpenText(Path.Combine(Library.ConfigDirectory, "license.txt"))) { return; }
            if (!string.IsNullOrWhiteSpace(Library.PairEngineConfigDirectory))
            {
                if (OpenText(Path.Combine(Library.PairEngineConfigDirectory, "license.md"))) { return; }
                if (OpenText(Path.Combine(Library.PairEngineConfigDirectory, "license.txt"))) { return; }
            }
        }
        private bool OpenText(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }
            IDialogParameters param = new DialogParameters();
            param.Add("FileName", path);
            this.dialogService.ShowDialog("TextDialog", param, _ => { });
            return true;
        }
        private async Task ActivationAction(string param)
        {
            switch (param)
            {
                case "Activate":
                    await Library.VoiceLibrary.Activate(ActivationKey.Value);
                    break;
                case "DeActivate":
                    await Library.VoiceLibrary.DeActivate();
                    break;
            }
            Activated.Value = Library.VoiceLibrary.IsActivated;
        }
        private void DefaultAction()
        {
            if (MessageBox.Show("全ての設定値をデフォルト値に戻します。\nよろしいですか？ ", "リセット確認", MessageBoxButton.OKCancel) != MessageBoxResult.OK) { return; }
            foreach (var s in Settings)
            {
                if (s is BoolSetting sb) { sb.Value = sb.DefaultValue; }
                if (s is IntSetting si) { si.Value = si.DefaultValue; }
                if (s is DoubleSetting sd) { sd.Value = sd.DefaultValue; }
                if (s is StringSetting ss) { ss.Value = ss.DefaultValue; }
            }
            SetSettingList();
            ActivationKey.Value = string.Empty;
        }
        private void ApplyAction()
        {
            if (SettingsEdit.Value != null)
            {
                JsonUtil.Serialize(SettingsEdit.Value, Library.SettingPath);
            }
            Library.VoiceLibrary.Settings = JsonUtil.DeepClone(SettingsEdit.Value);
        }
        protected override void OkAction()
        {
            ApplyAction();
            base.OkAction();
        }
    }
}
