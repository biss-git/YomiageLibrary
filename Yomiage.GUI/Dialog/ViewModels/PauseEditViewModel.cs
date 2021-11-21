using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yomiage.SDK.Config;
using Yomiage.SDK.Talk;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class PauseEditViewModel : DialogViewModelBase
    {
        public override string Title => "ポーズ長編集";
        public ReactivePropertySlim<int> Min { get; } = new ReactivePropertySlim<int>(-300);
        public ReactivePropertySlim<int> Max { get; } = new ReactivePropertySlim<int>(30000);
        public ReactivePropertySlim<int> Span_ms { get; } = new ReactivePropertySlim<int>(0);
        public ReactiveCommand<KeyEventArgs> KeyDownCommand { get; }
        private Pause pause;

        public PauseEditViewModel()
        {
            KeyDownCommand = new ReactiveCommand<KeyEventArgs>().WithSubscribe(source =>
            {
                switch (source.Key)
                {
                    case Key.Enter: OkAction(); break;
                    case Key.Escape: CancelAction(); break;
                }
            }).AddTo(Disposables);
        }

        protected override void OkAction()
        {
            if (pause != null)
            {
                pause.Type = PauseType.Manual;
                pause.Span_ms = Span_ms.Value;
            }
            this.RaiseRequestClose(null);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.TryGetValue("pause", out Pause pause))
            {
                this.pause = pause;
                this.Span_ms.Value = pause.Span_ms;
            }
            if (parameters.TryGetValue("config", out EngineConfig config) && config != null)
            {
                try
                {
                    Min.Value = Math.Min(config.ShortPauseSetting.Min, config.LongPauseSetting.Min);
                    Max.Value = Math.Max(config.ShortPauseSetting.Max, config.LongPauseSetting.Max);
                    Span_ms.Value = Math.Max(Min.Value, Math.Min(Span_ms.Value, Max.Value));
                }
                catch (Exception)
                {

                }
            }
        }

    }
}
