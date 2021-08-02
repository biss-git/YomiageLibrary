using Microsoft.Win32;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.GUI.Models;

namespace Yomiage.GUI.ViewModels
{
    class SettingVoiceViewModel : ViewModelBase
    {
        public ReactiveProperty<string> PresetFilePath { get; }

        public ReactiveCommand SelectCommand { get; }
        public ReactiveCommand NewCommand { get; }

        public SettingService SettingService { get; }
        private ConfigService configService;

        public SettingVoiceViewModel(
            SettingService settingService,
            ConfigService configService
            ): base()
        {
            this.SettingService = settingService;
            this.configService = configService;

            SelectCommand = new ReactiveCommand().WithSubscribe(SelectAction).AddTo(Disposables);
            NewCommand = new ReactiveCommand().WithSubscribe(NewAction).AddTo(Disposables);

            PresetFilePath = new ReactiveProperty<string>(settingService.PresetFilePath)
                .SetValidateNotifyError(s =>
                string.IsNullOrWhiteSpace(s) ? "辞書のファイルパスを入力してください。" :
                !File.Exists(s) ? "ファイルが存在しません。" : null
                ).AddTo(Disposables);
            PresetFilePath.Subscribe(s => settingService.PresetFilePath = s);
        }

        private void SelectAction()
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "プリセットファイル (.yvpc)|*.yvpc",
                InitialDirectory = this.configService.PresetDirectory,
            };
            if(ofd.ShowDialog() == true)
            {
                this.PresetFilePath.Value = ofd.FileName;
            }
        }

        private void NewAction()
        {
            var sfd = new SaveFileDialog()
            {
                Filter = "プリセットファイル (.yvpc)|*.yvpc",
                InitialDirectory = this.configService.PresetDirectory,
            };
            if (sfd.ShowDialog() == true)
            {
                File.Create(sfd.FileName).Close();
                this.PresetFilePath.Value = sfd.FileName;
            }
        }
    }
}
