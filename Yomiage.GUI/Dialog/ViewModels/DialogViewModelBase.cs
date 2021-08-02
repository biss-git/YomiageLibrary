using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Dialog.ViewModels
{
    public abstract class DialogViewModelBase : BindableBase, IDisposable, IDialogAware
    {

        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();
        public void Dispose() => Disposables.Dispose();

        public virtual string Title => "タイトル";

        protected ReactivePropertySlim<bool> CanOk { get; } = new ReactivePropertySlim<bool>(true);
        protected ReactivePropertySlim<bool> CanCancel { get; } = new ReactivePropertySlim<bool>(true);
        public ReactiveCommand OkCommand { get; protected set; }
        public ReactiveCommand CancelCommand { get; protected set; }

        public DialogViewModelBase()
        {
            OkCommand = CanOk.ToReactiveCommand().WithSubscribe(OkAction).AddTo(Disposables);
            CancelCommand = CanCancel.ToReactiveCommand().WithSubscribe(CancelAction).AddTo(Disposables);
        }

        protected virtual void OkAction()
        {
            IDialogResult result = new DialogResult(ButtonResult.OK);
            this.RaiseRequestClose(result);
        }
        protected virtual void CancelAction()
        {
            IDialogResult result = new DialogResult(ButtonResult.Cancel);
            this.RaiseRequestClose(result);
        }

        public virtual event Action<IDialogResult> RequestClose;

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
        }

        protected void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }
    }
}
