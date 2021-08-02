using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yomiage.GUI.ViewModels;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class PauseCharacterViewModel : DialogViewModelBase
    {
        public override string Title => "記号ポーズ";
        public ReactiveProperty<string> PauseCharacter { get; } = new ReactiveProperty<string>().SetValidateNotifyError(x =>
        {
            if (string.IsNullOrEmpty(x)) { return "入力してください。"; }
            return Regex.IsMatch(x, "^[!-~ ]*$") ? null : "半角英数字、半角記号を入力してください。";
        });
        public ReactiveProperty<int> PauseSpan_ms { get; } = new ReactiveProperty<int>(80);

        public PauseCharacterViewModel()
        {
            this.PauseCharacter.ObserveHasErrors.Subscribe(value =>
            {
                CanOk.Value = !value;
            }).AddTo(Disposables);
        }

        protected override void OkAction()
        {
            var param = new DialogParameters();
            param.Add("key", PauseCharacter.Value);
            param.Add("span_ms", PauseSpan_ms.Value);
            var result = new DialogResult(ButtonResult.OK, param);
            RaiseRequestClose(result);
        }
        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (!parameters.TryGetValue("key", out string key)) { return; }
            PauseCharacter.Value = key;
            if (!parameters.TryGetValue("span_ms", out int span_ms)) { return; }
            PauseSpan_ms.Value = span_ms;
        }

    }
}
