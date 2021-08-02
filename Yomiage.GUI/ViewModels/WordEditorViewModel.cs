using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Yomiage.Core.Models;
using Yomiage.GUI.EventMessages;
using Yomiage.GUI.Models;
using Yomiage.SDK.Talk;

namespace Yomiage.GUI.ViewModels
{
    class WordEditorViewModel : ViewModelBase
    {
        public ReactivePropertySlim<string> OriginalText { get; } = new("あいう");
        public ReactivePropertySlim<TalkScript> Phrase { get; } = new();
        public ReactivePropertySlim<string> Priority { get; } = new("3.標準");
        public ReactiveProperty<bool> CanRegister { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> CanUnRegister { get; } = new ReactiveProperty<bool>(false);

        public ReactiveCommand<string> UpdateCommand { get; }
        public AsyncReactiveCommand PlayCommand { get; }
        public AsyncReactiveCommand StopCommand { get; }
        public ReactiveCommand RegisterCommand { get; }
        public ReactiveCommand UnRegisterCommand { get; }
        public ReactiveCommand ClearCommand { get; }

        VoicePlayerService voicePlayerService;
        WordDictionaryService wordDictionaryService;
        VoicePresetService voicePresetService;

        public WordEditorViewModel(
            VoicePlayerService voicePlayerService,
            WordDictionaryService wordDictionaryService,
            VoicePresetService voicePresetService,
            IMessageBroker messageBroker,
            IDialogService _dialogService
            ) : base(_dialogService)
        {
            this.voicePlayerService = voicePlayerService;
            this.wordDictionaryService = wordDictionaryService;
            this.voicePresetService = voicePresetService;

            UpdateCommand = new ReactiveCommand<string>().WithSubscribe(UpdateAction).AddTo(Disposables);
            PlayCommand = this.voicePlayerService.CanPlay.ToAsyncReactiveCommand().WithSubscribe(PlayAction).AddTo(Disposables);
            StopCommand = new AsyncReactiveCommand().WithSubscribe(voicePlayerService.Stop).AddTo(Disposables);
            RegisterCommand = this.CanRegister.ToReactiveCommand().WithSubscribe(RegisterAction).AddTo(Disposables);
            UnRegisterCommand = this.CanUnRegister.ToReactiveCommand().WithSubscribe(UnRegisterAction).AddTo(Disposables);
            ClearCommand = new ReactiveCommand().WithSubscribe(ClearAction).AddTo(Disposables);
            OriginalText.Subscribe(_ => UpdateState()).AddTo(Disposables);
            Priority.Subscribe(_ => UpdateState()).AddTo(Disposables);

            TalkScript phrase = new TalkScript();
            phrase.Sections = new List<Section>();
            {
                Section section = new Section();
                section.Pause = new Pause() { Span_ms = 200, Type = PauseType.Short };
                section.Moras.Add(new Mora()
                {
                    Accent = false,
                    Character = "ア",
                    Voiceless = null,
                });
                section.Moras.Add(new Mora()
                {
                    Accent = true,
                    Character = "イ",
                    Voiceless = true,
                });
                section.Moras.Add(new Mora()
                {
                    Accent = false,
                    Character = "ウ",
                    Voiceless = false,
                });
                phrase.Sections.Add(section);
            }
            {
                Section section = new Section();
                section.Pause = new Pause() { Span_ms = 250, Type = PauseType.Long };
                section.Moras.Add(new Mora()
                {
                    Accent = false,
                    Character = "ア",
                    Voiceless = null,
                });
                section.Moras.Add(new Mora()
                {
                    Accent = true,
                    Character = "イ",
                    Voiceless = true,
                });
                phrase.Sections.Add(section);
            }
            {
                Section section = new Section();
                section.Moras.Add(new Mora()
                {
                    Character = "ア",
                });
                section.Moras.Add(new Mora()
                {
                    Character = "イ",
                });
                phrase.Sections.Add(section);
            }
            phrase.EndSection = new EndSection()
            {
                EndSymbol = "。",
                Pause = new Pause() { Type = PauseType.None },
            };

            this.Phrase.Value = phrase;
            messageBroker.Subscribe<EditWord>(word =>
            {
                OriginalText.Value = word.OriginalText;
                if(word.Phrase != null)
                {
                    Phrase.Value = word.Phrase;
                }
                Priority.Value = word.Priority;
            });
        }

        private async Task PlayAction()
        {
            this.Phrase.Value.OriginalText = OriginalText.Value;
            await this.voicePlayerService.Play(this.Phrase.Value);
        }

        private void RegisterAction()
        {
            this.Phrase.Value.OriginalText = this.OriginalText.Value;
            this.wordDictionaryService.RegisterDictionary(
                this.Phrase.Value,
                this.Priority.Value
                );
            UpdateState();
        }

        private void UnRegisterAction()
        {
            var result = MessageBox.Show("編集中の単語を辞書から削除してよろしいですか？", "確認", MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK) { return; }
            this.wordDictionaryService.UnRegiserDictionary(OriginalText.Value);
            UpdateState();
        }

        private void UpdateState()
        {
            if (string.IsNullOrWhiteSpace(OriginalText.Value))
            {
                CanRegister.Value = false;
                CanUnRegister.Value = false;
                return;
            }
            var registerd = this.wordDictionaryService.IsRegisterd(OriginalText.Value, Phrase.Value);
            CanRegister.Value = registerd != true;
            CanUnRegister.Value = registerd != false;
        }

        private void ClearAction()
        {
            this.OriginalText.Value = null;
            this.Phrase.Value = new TalkScript();
            this.Priority.Value = "3.標準";
        }

        private void UpdateAction(string param)
        {
            UpdateState();
        }
    }
}
