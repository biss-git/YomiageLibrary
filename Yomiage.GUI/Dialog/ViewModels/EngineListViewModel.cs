using Microsoft.Win32;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core.Types;
using Yomiage.Core.Models;
using Yomiage.GUI.Models;
using Prism.Services.Dialogs;
using System.Windows;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class EngineListViewModel : DialogViewModelBase
    {
        public override string Title => "エンジン一覧";
        public ReactiveCommand AddCommand { get; }
        public ReactiveCommand<Engine> SettingCommand { get; }

        public ReadOnlyReactiveCollection<Engine> Engines { get; }
        public ReactivePropertySlim<Engine> SelectedEngine { get; }

        ConfigService ConfigService;
        IDialogService dialogService;

        public EngineListViewModel(
            ConfigService configService,
            VoiceEngineService voiceEngineService,
            IDialogService dialogService)
        {
            this.ConfigService = configService;
            this.dialogService = dialogService;
            AddCommand = new ReactiveCommand().WithSubscribe(AddAction).AddTo(Disposables);
            SettingCommand = new ReactiveCommand<Engine>().WithSubscribe(SettingAction).AddTo(Disposables);
            Engines = voiceEngineService.AllEngines.ToReadOnlyReactiveCollection().AddTo(Disposables);
            SelectedEngine = new ReactivePropertySlim<Engine>().AddTo(Disposables);
        }

        private void AddAction()
        {
            var ofd = new OpenFileDialog() { Filter = "音声合成エンジン(.vengj)|*.vengj" };
            if(ofd.ShowDialog() != true) { return; }

            var directorys = Directory.GetDirectories(ConfigService.EngineDirectory);
            var directory = string.Empty;
            for(int i=0; i < 1000; i++)
            {
                directory = Path.Combine(ConfigService.EngineDirectory, "Engine_" + i.ToString("000"));
                if (!directorys.Contains(directory)){ break; }
            }

            ZipFile.ExtractToDirectory(ofd.FileName, directory, Encoding.GetEncoding("sjis"));

            var configs = new List<string>();
            Util.Utility.SearchFile(directory, "engine.config.json", 4, configs);

            var Text = configs.Count == 0 ? ($"音声合成エンジンが見つかりませんでした。\n{directory} を確認してください。") :
                                            ($"音声合成エンジンが {configs.Count} 件みつかりました。\nアプリケーションを再起動してください。");

            MessageBox.Show(Text, "音声合成エンジンのインストール", MessageBoxButton.OK);
        }

        private void SettingAction(Engine engine)
        {
            IDialogParameters parameters = new DialogParameters();
            parameters.Add("Engine", engine);
            this.dialogService.ShowDialog("SettingsEngineDialog", parameters, result => { });
        }
    }
}
