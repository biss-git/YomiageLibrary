using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.GUI.Models;
using Yomiage.GUI.ViewModels;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class SaveVoiceViewModel : DialogViewModelBase
    {
        public SaveVoiceViewModel(SettingService settingService)
        {
            SettingService = settingService;
        }

        public SettingService SettingService { get; }

        public override string Title => "音声保存";

        protected override void OkAction()
        {
            SettingService.SaveEnvironment();
            base.RaiseRequestClose(null);
        }

        /// <summary>
        /// 閉じるときにの処理
        /// プレビューのためにアプリ全体に適用してしまっている設定をもとに戻す。
        /// </summary>
        public override void OnDialogClosed()
        {
            base.OnDialogClosed();
            SettingService.Reload();
        }
    }
}
