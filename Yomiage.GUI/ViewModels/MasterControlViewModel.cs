using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core.Types;
using Yomiage.GUI.EventMessages;
using Yomiage.GUI.Models;

namespace Yomiage.GUI.ViewModels
{
    class MasterControlViewModel : ViewModelBase
    {
        public SettingService SettingService { get; }

        public ReactivePropertySlim<double> SpeedMinimum { get; } = new(0.5);
        public ReactivePropertySlim<double> SpeedMaximum { get; } = new(4);
        public ReactivePropertySlim<double> PitchMinimum { get; } = new(0.5);
        public ReactivePropertySlim<double> PitchMaximum { get; } = new(2);
        public ReactivePropertySlim<double> EmphasisMaximum { get; } = new(2);

        public ReactiveCollection<PauseSet> PauseList { get; }
        public ReactivePropertySlim<PauseSet> Selected { get; } = new();

        public ReactiveCommand<string> PauseCharacterCommand { get; }
        public ReactiveCommand<string> ClearCommand { get; }

        PauseDictionaryService pauseDictionaryService;

        public MasterControlViewModel(
            PauseDictionaryService pauseDictionaryService,
            IDialogService _dialogService,
            SettingService settingService) : base(_dialogService)
        {
            this.SettingService = settingService;
            this.pauseDictionaryService = pauseDictionaryService;

            PauseCharacterCommand = new ReactiveCommand<string>().WithSubscribe(PauseCharacterAction).AddTo(Disposables);
            ClearCommand = new ReactiveCommand<string>().WithSubscribe(ClearAction).AddTo(Disposables);

            settingService.ExpandEffectRange.Subscribe(SetRange).AddTo(Disposables);

            this.PauseList = pauseDictionaryService.PauseDictionary;
        }

        private void SetRange(bool isExpand)
        {
            SpeedMinimum.Value = isExpand ? 0.1 : 0.5;
            SpeedMaximum.Value = isExpand ? 5 : 4;
            PitchMinimum.Value = isExpand ? 0.1 : 0.5;
            PitchMaximum.Value = isExpand ? 5 : 2;
            EmphasisMaximum.Value = isExpand ? 5 : 2;
        }

        private void PauseCharacterAction(string param)
        {
            switch (param)
            {
                case "Create":
                    pauseDictionaryService.Create();
                    break;
                case "Edit":
                    if (Selected.Value == null) { return; }
                    pauseDictionaryService.Edit(Selected.Value.key);
                    break;
                case "Remove":
                    if (Selected.Value == null) { return; }
                    pauseDictionaryService.Remove(Selected.Value.key);
                    break;
            }
        }

        private void ClearAction(string param)
        {
            switch (param)
            {
                case "Effect":
                    this.SettingService.MasterVolume.Value = 1;
                    this.SettingService.MasterSpeed.Value = 1;
                    this.SettingService.MasterPitch.Value = 1;
                    this.SettingService.MasterEmphasis.Value = 1;
                    this.SettingService.SaveMaster();
                    break;
                case "Pause":
                    this.SettingService.MasterShortPause.Value = 150;
                    this.SettingService.MasterLongPause.Value = 370;
                    this.SettingService.MasterEndPause.Value = 800;
                    this.SettingService.SaveMaster();
                    break;
                default:
                    break;
            }
        }
    }
}
