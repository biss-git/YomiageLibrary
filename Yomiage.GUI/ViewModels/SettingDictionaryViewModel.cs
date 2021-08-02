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
    class SettingDictionaryViewModel : ViewModelBase
    {
        public ReactiveProperty<string> PhraseDictionaryPath { get; }
        public ReactiveProperty<string> PauseDictionaryPath { get; }
        public ReactiveProperty<string> WordDictionaryPath { get; }

        public ReactiveCommand<string> SelectCommand { get; }
        public ReactiveCommand<string> NewCommand { get; }

        public SettingService SettingService { get; }
        private ConfigService configService;

        public SettingDictionaryViewModel(
            SettingService settingService,
            ConfigService configService
            ) : base()
        {
            this.SettingService = settingService;
            this.configService = configService;

            SelectCommand = new ReactiveCommand<string>().WithSubscribe(SelectAction).AddTo(Disposables);
            NewCommand = new ReactiveCommand<string>().WithSubscribe(NewAction).AddTo(Disposables);

            PhraseDictionaryPath = new ReactiveProperty<string>(settingService.PhraseDictionaryPath)
                .SetValidateNotifyError(FileValidation).AddTo(Disposables);
            PhraseDictionaryPath.Subscribe(s => settingService.PhraseDictionaryPath = s);

            PauseDictionaryPath = new ReactiveProperty<string>(settingService.PauseDictionaryPath)
                .SetValidateNotifyError(FileValidation).AddTo(Disposables);
            PauseDictionaryPath.Subscribe(s => settingService.PauseDictionaryPath = s);

            WordDictionaryPath = new ReactiveProperty<string>(settingService.WordDictionaryPath)
                .SetValidateNotifyError(FileValidation).AddTo(Disposables);
            WordDictionaryPath.Subscribe(s => settingService.WordDictionaryPath = s);
        }

        private string FileValidation(string s)
        {
            return string.IsNullOrWhiteSpace(s) ? "辞書のファイルパスを入力してください。" :
                !File.Exists(s) ? "ファイルが存在しません。" : null;
        }

        private void SelectAction(string param)
        {
            switch (param)
            {
                case "Phrase": SelectAction_Phrase(); break;
                case "Pause": SelectAction_Pause(); break;
                case "Word": SelectAction_Word(); break;
            }
        }
        private void SelectAction_Phrase()
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "フレーズ辞書 (.ypdic)|*.ypdic",
                InitialDirectory = this.configService.PhraseDirectory,
            };
            if (ofd.ShowDialog() == true)
            {
                this.PhraseDictionaryPath.Value = ofd.FileName;
            }
        }
        private void SelectAction_Pause()
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "ポーズ辞書 (.ysdic)|*.ysdic",
                InitialDirectory = this.configService.PauseDirectory,
            };
            if (ofd.ShowDialog() == true)
            {
                this.PauseDictionaryPath.Value = ofd.FileName;
            }
        }
        private void SelectAction_Word()
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "単語辞書 (.ywdic)|*.ywdic",
                InitialDirectory = this.configService.WordDirectory,
            };
            if (ofd.ShowDialog() == true)
            {
                this.WordDictionaryPath.Value = ofd.FileName;
            }
        }

        private void NewAction(string param)
        {
            switch (param)
            {
                case "Phrase": NewAction_Phrase(); break;
                case "Pause": NewAction_Pause(); break;
                case "Word": NewAction_Word(); break;
            }
        }
        private void NewAction_Phrase()
        {
            var sfd = new SaveFileDialog()
            {
                Filter = "フレーズ辞書 (.ypdic)|*.ypdic",
                InitialDirectory = this.configService.PhraseDirectory,
            };
            if (sfd.ShowDialog() == true)
            {
                File.Create(sfd.FileName).Close();
                this.PhraseDictionaryPath.Value = sfd.FileName;
            }
        }
        private void NewAction_Pause()
        {
            var sfd = new SaveFileDialog()
            {
                Filter = "ポーズ辞書 (.ysdic)|*.ysdic",
                InitialDirectory = this.configService.PauseDirectory,
            };
            if (sfd.ShowDialog() == true)
            {
                File.Create(sfd.FileName).Close();
                this.PauseDictionaryPath.Value = sfd.FileName;
            }
        }
        private void NewAction_Word()
        {
            var sfd = new SaveFileDialog()
            {
                Filter = "単語辞書 (.ywdic)|*.ywdic",
                InitialDirectory = this.configService.WordDirectory,
            };
            if (sfd.ShowDialog() == true)
            {
                File.Create(sfd.FileName).Close();
                this.WordDictionaryPath.Value = sfd.FileName;
            }
        }
    }
}
