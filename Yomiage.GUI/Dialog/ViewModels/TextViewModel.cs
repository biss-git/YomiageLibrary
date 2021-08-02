using Prism.Services.Dialogs;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
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
        public ReactivePropertySlim<string> MD { get; } = new();
        public ReactivePropertySlim<string> Text { get; } = new();

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("FileName"))
            {
                var path = parameters.GetValue<string>("FileName");
                if (File.Exists(path))
                {
                    TextPath.Value = path;
                    if (Path.GetExtension(path).Contains("md"))
                    {
                        MD.Value = File.ReadAllText(path, Encoding.UTF8);
                    }
                    else
                    {
                        Text.Value = File.ReadAllText(path, Encoding.UTF8);
                    }
                }
            }
        }
    }
}
