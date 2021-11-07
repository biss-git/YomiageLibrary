using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;

namespace Yomiage.GUI.ViewModels
{
    public class ViewModelBase : BindableBase, IDisposable
    {
        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();
        public void Dispose() => Disposables.Dispose();

        protected IDialogService DialogService;

        public ReactiveCommand<string> OpenDialogCommand { get; }

        public ViewModelBase(IDialogService dialogService)
        {
            this.DialogService = dialogService;
            OpenDialogCommand = new ReactiveCommand<string>().WithSubscribe(OpenDialogAction).AddTo(Disposables);
        }
        public ViewModelBase()
        {
        }

        private void OpenDialogAction(string param)
        {
            try
            {
                this.DialogService?.ShowDialog(param, new DialogParameters(), result => { });
            }
            catch (Exception)
            {

            }
        }

    }
}
