using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class TextViewModel : DialogViewModelBase
    {
        public override string Title => "ライセンス";

        public ReactivePropertySlim<string> TextPath { get; } = new();
        public ReactivePropertySlim<string> Text { get; } = new();

        public ReactiveCommand OpenLicenseCommand { get; }

        public TextViewModel()
        {
            OpenLicenseCommand = new ReactiveCommand().WithSubscribe(OpenLicenseAction).AddTo(Disposables);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("FileName"))
            {
                var path = parameters.GetValue<string>("FileName");
                if (File.Exists(path))
                {
                    TextPath.Value = path;
                    Text.Value = File.ReadAllText(path, Encoding.UTF8);
                }
            }
        }

        private void OpenLicenseAction()
        {
            Process.Start("notepad.exe", TextPath.Value);
        }
    }
}
