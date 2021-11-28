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
using System.Diagnostics;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class LibraryListViewModel : DialogViewModelBase
    {
        public override string Title => "音声ライブラリ一覧";
        public ReactiveCommand AddCommand { get; }
        public ReactiveCommand RemoveCommand { get; }
        public ReactiveCommand OpenFolderCommand { get; }
        public ReactiveCommand<Library> SettingCommand { get; }

        public ReadOnlyReactiveCollection<Library> Librarys { get; }
        public ReactivePropertySlim<Library> SelectedLibrary { get; }

        readonly ConfigService ConfigService;
        readonly IDialogService dialogService;

        public LibraryListViewModel(
            ConfigService configService,
            VoiceLibraryService voiceLibraryService,
            IDialogService dialogService)
        {
            this.ConfigService = configService;
            this.dialogService = dialogService;
            AddCommand = new ReactiveCommand().WithSubscribe(AddAction).AddTo(Disposables);
            RemoveCommand = new ReactiveCommand().WithSubscribe(RemoveAction).AddTo(Disposables);
            OpenFolderCommand = new ReactiveCommand().WithSubscribe(OpenFolderAction).AddTo(Disposables);
            SettingCommand = new ReactiveCommand<Library>().WithSubscribe(SettingAction).AddTo(Disposables);
            Librarys = voiceLibraryService.AllLibrarys.ToReadOnlyReactiveCollection().AddTo(Disposables);
            SelectedLibrary = new ReactivePropertySlim<Library>().AddTo(Disposables);
        }

        private void AddAction()
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "音声ライブラリ(.vlib)|*.vlib",
                Multiselect = true,
            };

            if (ofd.ShowDialog() != true) { return; }

            // var directorys = Directory.GetDirectories(ConfigService.LibraryDirectory).ToList();
            var configs = new List<string>();

            foreach (var vlib in ofd.FileNames)
            {
                this.ConfigService.UnZipVLib(vlib, configs);
                //var directory = string.Empty;
                //for (int i = 0; i < 1000; i++)
                //{
                //    directory = Path.Combine(ConfigService.LibraryDirectory, "Library_" + i.ToString("000"));
                //    if (!directorys.Contains(directory)) { break; }
                //}
                //try
                //{
                //    ZipFile.ExtractToDirectory(vlib, directory, Encoding.GetEncoding("sjis"));
                //    directorys.Add(directory);
                //    Util.Utility.SearchFile(directory, "library.config.json", 6, configs);
                //}
                //catch (Exception)
                //{
                //}
            }

            ConfigService.LoadLibrary(ConfigService.LibraryDirectory);
            ConfigService.InitPreset();

            var Text = configs.Count == 0 ? ($"音声ライブラリが見つかりませんでした。") :
                                            ($"音声ライブラリが {configs.Count} 件みつかりました。");

            MessageBox.Show(Text, "音声ライブラリのインストール", MessageBoxButton.OK);

        }

        private void RemoveAction()
        {
            ProcessStartInfo pi = new()
            {
                FileName = "https://sites.google.com/view/unicoe/%E3%83%81%E3%83%A5%E3%83%BC%E3%83%88%E3%83%AA%E3%82%A2%E3%83%AB/%E3%82%A8%E3%83%B3%E3%82%B8%E3%83%B3%E3%83%A9%E3%82%A4%E3%83%96%E3%83%A9%E3%83%AA%E3%82%92%E5%89%8A%E9%99%A4%E3%81%99%E3%82%8B",
                UseShellExecute = true,
            };
            Process.Start(pi);
        }

        private void OpenFolderAction()
        {
            Process.Start("EXPLORER.EXE", this.ConfigService.LibraryDirectory);
        }

        private void SettingAction(Library library)
        {
            IDialogParameters parameters = new DialogParameters
            {
                { "Library", library }
            };
            this.dialogService.ShowDialog("SettingsLibraryDialog", parameters, result => { });
        }

    }
}
