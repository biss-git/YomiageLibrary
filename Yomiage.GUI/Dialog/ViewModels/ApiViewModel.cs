using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.API;
using Yomiage.GUI.Models;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class ApiViewModel : DialogViewModelBase
    {
        public ReactivePropertySlim<string> UrlText { get; } = new ReactivePropertySlim<string>();

        public ReactiveCommand OpenCommand { get; }

        public ApiViewModel()
        {
            UrlText.Value = $"http://localhost:{ServerInfo.ApiPort}/api/command";
            OpenCommand = new ReactiveCommand().WithSubscribe(OpenAction).AddTo(Disposables);
        }

        private void OpenAction()
        {
            ProcessStartInfo pi = new ProcessStartInfo()
            {
                FileName = "https://biss-git.github.io/YomiageLibrary/",
                UseShellExecute = true,
            };
            Process.Start(pi);
        }
    }
}
