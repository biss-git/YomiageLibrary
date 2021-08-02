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
    class LibraryListViewModel : DialogViewModelBase
    {
        public override string Title => "音声ライブラリ一覧";
        public ReactiveCommand AddCommand { get; }
        public ReactiveCommand<Library> SettingCommand { get; }

        public ReadOnlyReactiveCollection<Library> Librarys { get; }
        public ReactivePropertySlim<Library> SelectedLibrary { get; }

        ConfigService ConfigService;
        IDialogService dialogService;

        public LibraryListViewModel(
            ConfigService configService,
            VoiceLibraryService voiceLibraryService,
            IDialogService dialogService)
        {
            this.ConfigService = configService;
            this.dialogService = dialogService;
            AddCommand = new ReactiveCommand().WithSubscribe(AddAction).AddTo(Disposables);
            SettingCommand = new ReactiveCommand<Library>().WithSubscribe(SettingAction).AddTo(Disposables);
            Librarys = voiceLibraryService.AllLibrarys.ToReadOnlyReactiveCollection().AddTo(Disposables);
            SelectedLibrary = new ReactivePropertySlim<Library>().AddTo(Disposables);
        }

        private void AddAction()
        {
            var ofd = new OpenFileDialog() { Filter = "音声ライブラリ(.vlibj)|*.vlibj" };
            if (ofd.ShowDialog() != true) { return; }

            var directorys = Directory.GetDirectories(ConfigService.LibraryDirectory);
            var directory = string.Empty;
            for (int i = 0; i < 1000; i++)
            {
                directory = Path.Combine(ConfigService.LibraryDirectory, "Library_" + i.ToString("000"));
                if (!directorys.Contains(directory)) { break; }
            }

            ZipFile.ExtractToDirectory(ofd.FileName, directory, Encoding.GetEncoding("sjis"));

            var configs = new List<string>();
            Util.Utility.SearchFile(directory, "library.config.json", 4, configs);

            var Text = configs.Count == 0 ? ($"音声ライブラリが見つかりませんでした。\n{directory} を確認してください。") :
                                            ($"音声ライブラリが {configs.Count} 件みつかりました。\nアプリケーションを再起動してください。");

            MessageBox.Show(Text, "音声ライブラリのインストール", MessageBoxButton.OK);

        }

        private void SettingAction(Library library)
        {
            IDialogParameters parameters = new DialogParameters();
            parameters.Add("Library", library);
            this.dialogService.ShowDialog("SettingsLibraryDialog", parameters, result=> { });
        }

    }
}
