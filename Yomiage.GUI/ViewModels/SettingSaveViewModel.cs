using Microsoft.WindowsAPICodePack.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Yomiage.GUI.Models;

namespace Yomiage.GUI.ViewModels
{
    class SettingSaveViewModel : ViewModelBase
    {
        public ReactiveCommand SelectFolderCommand { get; }
        public ReactiveCommand<string> TemplateCommand { get; }
        public ReactiveCommand<string> AddTemplateCommand { get; }

        public ReactivePropertySlim<string> Rule { get; }
        public ReactivePropertySlim<string> RuleFolderPath { get; }

        public SettingService SettingService { get; }
        public SettingSaveViewModel(SettingService settingService) : base()
        {
            this.SettingService = settingService;
            SelectFolderCommand = new ReactiveCommand().WithSubscribe(SelectFolderAction).AddTo(Disposables);
            TemplateCommand = new ReactiveCommand<string>().WithSubscribe(TemplateAction).AddTo(Disposables);
            AddTemplateCommand = new ReactiveCommand<string>().WithSubscribe(AddTemplateAction).AddTo(Disposables);
            this.Rule = new(settingService.Rule);
            this.Rule.Subscribe(v => settingService.Rule = v);
            this.RuleFolderPath = new(settingService.RuleFolderPath);
            this.RuleFolderPath.Subscribe(v => settingService.RuleFolderPath = v);
        }

        private void SelectFolderAction()
        {
            using var cofd = new CommonOpenFileDialog()
            {
                Title = "音声保存先フォルダの選択",
                // フォルダ選択モードにする
                IsFolderPicker = true,
            };
            if (cofd.ShowDialog() != CommonFileDialogResult.Ok) { return; }
            RuleFolderPath.Value = cofd.FileName;
        }

        private void TemplateAction(string param)
        {
            if (!string.IsNullOrWhiteSpace(Rule.Value))
            {
                var result = MessageBox.Show("命名規則を書き換えます。", "確認", MessageBoxButton.OKCancel);
                if(result != MessageBoxResult.OK) { return; }
            }
            switch (param)
            {
                case "連番_入力文":
                    Rule.Value = "{Number}_{Text}";
                    break;
                case "日時_連番_入力文":
                    Rule.Value = "{yyMMdd_HHmmss}_{Number}_{Text}";
                    break;
                case "日時_連番_キャラ名_入力文":
                    Rule.Value = "{yyMMdd_HHmmss}_{Number}_{VoicePreset}_{Text}";
                    break;
                case "キャラ名\\日時_連番_入力文":
                    Rule.Value = "{VoicePreset}\\{yyMMdd_HHmmss}_{Number}_{Text}";
                    break;
                case "日付\\時刻_連番_キャラ名_入力文":
                    Rule.Value = "{yyyyMMdd}\\{HHmmss}_{Number}_{VoicePreset}_{Text}";
                    break;
            }
        }

        private void AddTemplateAction(string param)
        {
            Rule.Value += "{" + param + "}";
        }
    }
}
