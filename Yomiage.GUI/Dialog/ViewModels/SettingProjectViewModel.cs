using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.GUI.Models;

namespace Yomiage.GUI.Dialog.ViewModels
{
    class SettingProjectViewModel : DialogViewModelBase
    {
        public override string Title => "プロジェクト設定";
        public SettingService SettingService { get; }
        private ConfigService ConfigService;
        private PhraseDictionaryService phraseDictionaryService;
        private PauseDictionaryService pauseDictionaryService;
        private WordDictionaryService wordDictionaryService;

        public SettingProjectViewModel(
            PhraseDictionaryService phraseDictionaryService,
            PauseDictionaryService pauseDictionaryService,
            WordDictionaryService wordDictionaryService,
            ConfigService configService,
            SettingService settingService
            )
        {
            this.phraseDictionaryService = phraseDictionaryService;
            this.pauseDictionaryService = pauseDictionaryService;
            this.wordDictionaryService = wordDictionaryService;
            this.ConfigService = configService;
            this.SettingService = settingService;
        }

        protected override void OkAction()
        {
            SettingService.Save();
            this.ConfigService.LoadPresets();
            this.phraseDictionaryService.LoadDictionary();
            this.pauseDictionaryService.LoadDictionary();
            this.wordDictionaryService.LoadDictionary();
            base.RaiseRequestClose(null);
        }


        public override void OnDialogClosed()
        {
            SettingService.Reload();
            base.OnDialogClosed();
        }
    }
}
