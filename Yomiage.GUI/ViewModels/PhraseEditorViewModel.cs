using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Yomiage.Core.Models;
using Yomiage.GUI.EventMessages;
using Yomiage.GUI.Models;
using Yomiage.GUI.Util;
using Yomiage.SDK.Common;
using Yomiage.SDK.Config;
using Yomiage.SDK.Talk;
using Yomiage.SDK.VoiceEffects;

namespace Yomiage.GUI.ViewModels
{
    public class PhraseEditorViewModel : ViewModelBase
    {
        public ReactivePropertySlim<string> TitleWithDirty { get; }
        public ReactivePropertySlim<Visibility> Visibility { get; }
        public ReactiveProperty<string> Content { get; }

        public ReactiveProperty<bool> IsDirty { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> CanRegister { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> CanRegisterChara { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> CanUnRegister { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> CanClear { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> AccentHide { get; } = new ReactiveProperty<bool>(false);

        public ReactiveCommand CloseCommand { get; }
        public ReactiveCommand<string> SelectCommand { get; }
        public ReactiveCommand<string> KeyDownCommand { get; }
        public ReactiveCommand UndoCommand { get; }
        public ReactiveCommand RedoCommand { get; }
        public AsyncReactiveCommand PlayCommand { get; }
        public AsyncReactiveCommand StopCommand { get; }
        public ReactiveCommand CopyEditorCommand { get; }
        public ReactiveCommand RegisterCommand { get; }
        public ReactiveCommand RegisterCharaCommand { get; }
        public ReactiveCommand UnRegisterCommand { get; }
        public ReactiveCommand ClearCommand { get; }
        public ReactiveCommand CopyCommand { get; }
        public ReactiveCommand PasteCommand { get; }

        public ReactivePropertySlim<bool> AccentSelected { get; } = new(true);
        public ReactivePropertySlim<bool> VolumeSelected { get; } = new(false);
        public ReactivePropertySlim<bool> SpeedSelected { get; } = new(false);
        public ReactivePropertySlim<bool> PitchSelected { get; } = new(false);
        public ReactivePropertySlim<bool> EmphasisSelected { get; } = new(false);
        public ReactivePropertySlim<TalkScript> Phrase { get; }
        public ReactivePropertySlim<string> OriginalText { get; } = new ReactivePropertySlim<string>(string.Empty);
        public ReadOnlyReactivePropertySlim<EngineConfig> EngineConfig { get; }
        public ReadOnlyReactivePropertySlim<EffectSetting> VolumeSetting { get; }
        public ReadOnlyReactivePropertySlim<EffectSetting> SpeedSetting { get; }
        public ReadOnlyReactivePropertySlim<EffectSetting> PitchSetting { get; }
        public ReadOnlyReactivePropertySlim<EffectSetting> EmphasisSetting { get; }
        public ReactiveCollection<PhraseSettingConfig> AdditionalSettings { get; } = new();
        public ReactivePropertySlim<int> PlayPosition { get; } = new();
        public ReadOnlyReactivePropertySlim<bool> IsExtend { get; }
        public string[] EndChars { get; } = new string[5] { "---", "通常。", "呼びかけ♪", "疑問？", "断定！" };
        private readonly Dictionary<string, string> EndCharDict = new() { { "", "---" }, { "。", "通常。" }, { "♪", "呼びかけ♪" }, { "？", "疑問？" }, { "！", "断定！" }, };
        public ReactiveProperty<string> SelectedEndChar { get; } = new ReactiveProperty<string>("");
        public ReactiveCommand<string> UpdateCommand { get; }

        private readonly ReactivePropertySlim<bool> CanUndo = new(false);
        private readonly ReactivePropertySlim<bool> CanRedo = new(false);
        private readonly UndoRedoManager<string> undoRedoManager = new();
        private readonly IMessageBroker messageBroker;
        private readonly PhraseService phraseService;
        private readonly PhraseDictionaryService phraseDictionaryService;
        private readonly VoicePresetService voicePresetService;
        private readonly VoicePlayerService voicePlayerService;
        private readonly TextService textService;
        private readonly WordDictionaryService wordDictionaryService;
        private readonly PauseDictionaryService pauseDictionaryService;

        public PhraseEditorViewModel(
            PhraseService phraseService,
            PhraseDictionaryService PhraseDictionaryService,
            VoicePresetService voicePresetService,
            VoicePlayerService voicePlayerService,
            SettingService settingService,
            TextService textService,
            WordDictionaryService wordDictionaryService,
            PauseDictionaryService pauseDictionaryService,
            IMessageBroker messageBroker,
            IDialogService _dialogService) : base(_dialogService)
        {
            this.phraseDictionaryService = PhraseDictionaryService;
            this.phraseService = phraseService;
            this.messageBroker = messageBroker;
            this.voicePresetService = voicePresetService;
            this.voicePlayerService = voicePlayerService;
            this.textService = textService;
            this.wordDictionaryService = wordDictionaryService;
            this.pauseDictionaryService = pauseDictionaryService;

            TitleWithDirty = new ReactivePropertySlim<string>("最初").AddTo(Disposables);
            Visibility = new ReactivePropertySlim<Visibility>(System.Windows.Visibility.Visible).AddTo(Disposables);
            Content = new ReactiveProperty<string>("").AddTo(Disposables);

            CloseCommand = new ReactiveCommand().WithSubscribe(CloseAction).AddTo(Disposables);
            SelectCommand = new ReactiveCommand<string>().WithSubscribe(SelectAction).AddTo(Disposables);
            UpdateCommand = new ReactiveCommand<string>().WithSubscribe(UpdateAction).AddTo(Disposables);
            CopyCommand = new ReactiveCommand().WithSubscribe(CopyAction).AddTo(Disposables);
            PasteCommand = new ReactiveCommand().WithSubscribe(PasteAction).AddTo(Disposables);

            KeyDownCommand = new ReactiveCommand<string>().WithSubscribe(key =>
            {
                switch (key)
                {
                    case "Undo": UndoAction(); break;
                    case "Redo": RedoAction(); break;
                }
            }).AddTo(Disposables);

            Phrase = new ReactivePropertySlim<TalkScript>(null, mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);
            Phrase.Subscribe(PhraseChanged).AddTo(Disposables);
            this.EngineConfig = voicePresetService.SelectedPreset.Select(p => p?.Engine?.EngineConfig).ToReadOnlyReactivePropertySlim().AddTo(Disposables);
            this.VolumeSetting = voicePresetService.SelectedPreset.Select(p => p?.Engine?.EngineConfig?.VolumeSetting).ToReadOnlyReactivePropertySlim().AddTo(Disposables);
            this.SpeedSetting = voicePresetService.SelectedPreset.Select(p => p?.Engine?.EngineConfig?.SpeedSetting).ToReadOnlyReactivePropertySlim().AddTo(Disposables);
            this.PitchSetting = voicePresetService.SelectedPreset.Select(p => p?.Engine?.EngineConfig?.PitchSetting).ToReadOnlyReactivePropertySlim().AddTo(Disposables);
            this.EmphasisSetting = voicePresetService.SelectedPreset.Select(p => p?.Engine?.EngineConfig?.EmphasisSetting).ToReadOnlyReactivePropertySlim().AddTo(Disposables);
            voicePresetService.SelectedPreset.Subscribe(p =>
            {
                if (p == null) { return; }
                AdditionalSettings.Clear();
                p.Engine.EngineConfig.AdditionalSettings?.ForEach(s =>
                {
                    var setting = new PhraseSettingConfig(s, this.SelectCommand);
                    AdditionalSettings.Add(setting);
                });
                AccentHide.Value = p.Engine.EngineConfig.AccentHide;
                UpdateState();
            });

            IsExtend = settingService.ExpandEffectRange.ToReadOnlyReactivePropertySlim().AddTo(Disposables);
            this.SelectedEndChar.Subscribe(selected =>
            {
                var endChar = EndCharDict.FirstOrDefault(x => x.Value == selected).Key;
                if (endChar != null)
                {
                    this.Phrase.Value.EndSection.EndSymbol = endChar;
                    this.UpdateCommand.Execute("ToGraph");
                }
                AddState();
            }).AddTo(Disposables);

            RedoCommand = this.CanRedo.ToReactiveCommand().WithSubscribe(RedoAction).AddTo(Disposables);
            UndoCommand = this.CanUndo.ToReactiveCommand().WithSubscribe(UndoAction).AddTo(Disposables);
            PlayCommand = this.voicePlayerService.CanPlay.ToAsyncReactiveCommand().WithSubscribe(PlayAction).AddTo(Disposables);
            StopCommand = new AsyncReactiveCommand().WithSubscribe(voicePlayerService.Stop).AddTo(Disposables);
            CopyEditorCommand = new ReactiveCommand().WithSubscribe(CopyEditorAction).AddTo(Disposables);

            this.CanRegister.Subscribe(_ => SetTitleWithDirty()).AddTo(Disposables);
            this.CanRegisterChara.Subscribe(_ => SetTitleWithDirty()).AddTo(Disposables);

            RegisterCommand = this.CanRegister.ToReactiveCommand().WithSubscribe(RegisterAction).AddTo(Disposables);
            RegisterCharaCommand = this.CanRegisterChara.ToReactiveCommand().WithSubscribe(RegisterCharaAction).AddTo(Disposables);
            UnRegisterCommand = this.CanUnRegister.ToReactiveCommand().WithSubscribe(UnRegisterAction).AddTo(Disposables);
            ClearCommand = this.CanClear.ToReactiveCommand().WithSubscribe(ClearAction).AddTo(Disposables);

            this.OriginalText.Subscribe(_ =>
            {
                UpdateState();
                SetTitleWithDirty();
            }).AddTo(Disposables);

            this.messageBroker.Subscribe<PhraseDictionaryChanged>(value => UpdateState());
        }

        private async Task PlayAction()
        {
            var script = JsonUtil.DeepClone(this.Phrase.Value);
            script.OriginalText = OriginalText.Value;
            script.Sections.RemoveRange(0, PlayPosition.Value);
            await this.voicePlayerService.Play(script);
            AddState();
        }

        private void UpdateAction(string param)
        {
            switch (param)
            {
                case "EndChanged":
                    // 文末記号が変化したことを通知
                    EndSymbleUpdate(this.Phrase.Value);
                    break;
                case "VoicelessChanged":
                    // 無声化に関する変更
                    AddState_Continuous();
                    break;
                case "AccentChanged":
                    // ポーズや読み
                    AddState();
                    break;
                case "ValueChanged_MouseDown":
                    // マウスクリックでの値の変更
                    AddState();
                    break;
                case "ValueChanged_MouseWheel":
                    // マウスホイールでの値の変更　これは全て保存すると大変なことになるので、連続する場合は最後のものだけを保存。
                    AddState_Continuous();
                    break;
                case "MouseUp":
                    AddState();
                    break;
            }
        }

        private void UndoAction()
        {
            if (!this.undoRedoManager.CanUndo) { return; }
            var state = undoRedoManager.Undo();
            LoadPhraseDictionary(state);
            UpdateCanUndoRedo();
        }

        private void RedoAction()
        {
            if (!this.undoRedoManager.CanRedo) { return; }
            var state = undoRedoManager.Redo();
            LoadPhraseDictionary(state);
            UpdateCanUndoRedo();
        }

        public bool LoadPhraseDictionary(string phraseDictionary)
        {
            var phrase = JsonUtil.DeserializeFromString<TalkScript>(phraseDictionary);
            if (phrase == null) { return false; }
            this.Phrase.Value = phrase;
            return true;
        }

        private void AddState()
        {
            if (this.Phrase.Value == null) { return; }
            this.Phrase.Value.OriginalText = null;
            var state = JsonUtil.SerializeToString(this.Phrase.Value);
            this.undoRedoManager.AddState(state);
            UpdateCanUndoRedo();
            SetTitleWithDirty();
        }
        private void AddState_Continuous()
        {
            if (this.Phrase.Value == null) { return; }
            this.Phrase.Value.OriginalText = null;
            var state = JsonUtil.SerializeToString(this.Phrase.Value);
            this.undoRedoManager.AddState_Continuous(state);
            UpdateCanUndoRedo();
        }
        private void UpdateCanUndoRedo()
        {
            this.CanUndo.Value = this.undoRedoManager.CanUndo;
            this.CanRedo.Value = this.undoRedoManager.CanRedo;
            UpdateState();
        }

        private void RegisterAction()
        {
            this.Phrase.Value.RemoveUnnecessaryParameters(this.voicePresetService.SelectedPreset.Value.Engine.EngineConfig);
            this.phraseDictionaryService.RegisterDictionary(
                this.OriginalText.Value,
                this.voicePresetService.SelectedPreset.Value.Engine.EngineConfig.Key,
                this.Phrase.Value,
                this.voicePresetService.SelectedPreset.Value.Library.LibraryConfig.Key,
                false
                );
            AddState();
            UpdateState();
        }
        private void RegisterCharaAction()
        {
            this.Phrase.Value.RemoveUnnecessaryParameters(this.voicePresetService.SelectedPreset.Value.Engine.EngineConfig);
            this.phraseDictionaryService.RegisterDictionary(
                this.OriginalText.Value,
                this.voicePresetService.SelectedPreset.Value.Engine.EngineConfig.Key,
                this.Phrase.Value,
                this.voicePresetService.SelectedPreset.Value.Library.LibraryConfig.Key,
                true
                );
            AddState();
            UpdateState();
        }
        private void UnRegisterAction()
        {
            var result = MessageBox.Show("編集中のフレーズを辞書から削除してよろしいですか？", "確認", MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK) { return; }
            this.phraseDictionaryService.UnRegiserDictionary(this.OriginalText.Value);
            UpdateState();
        }
        private void ClearAction()
        {
            var phrase = this.phraseDictionaryService.GetDictionary(
                OriginalText.Value,
                this.voicePresetService.SelectedPreset.Value.Engine.EngineConfig.Key,
                this.voicePresetService.SelectedPreset.Value.Library.LibraryConfig.Key
                );
            if (phrase == null)
            {
                var scripts = this.textService.Parse(OriginalText.Value, false, false, "", this.phraseDictionaryService.SearchDictionary, this.wordDictionaryService.WordDictionarys, this.pauseDictionaryService.PauseDictionary.ToList());
                if (scripts.Length > 0)
                {
                    phrase = scripts.First();
                }
            }
            if (phrase != null)
            {
                Phrase.Value = phrase;
                UpdateState();
            }
        }

        private void UpdateState()
        {
            if (this.voicePresetService.SelectedPreset.Value == null) { return; }
            (bool? engineRegisterd, bool? charaRegisterd) = this.phraseDictionaryService.IsRegisterd(
                OriginalText.Value,
                Phrase.Value,
                this.voicePresetService.SelectedPreset.Value.Engine.EngineConfig.Key,
                this.voicePresetService.SelectedPreset.Value.Library.LibraryConfig.Key
                );
            this.CanRegister.Value = engineRegisterd != true;
            this.CanRegisterChara.Value = charaRegisterd != true;
            this.CanUnRegister.Value = engineRegisterd != false || charaRegisterd != false || IsDirty.Value;
            this.CanClear.Value = true; // engineRegisterd != false || charaRegisterd != false || IsDirty.Value;
        }

        private void CopyEditorAction()
        {
            phraseService.CopyWithFocus(this);
        }

        private void PhraseChanged(TalkScript phrase)
        {
            PlayPosition.Value = 0;
            if (!string.IsNullOrWhiteSpace(phrase?.OriginalText))
            {
                this.OriginalText.Value = phrase?.OriginalText.Replace("\r\n", "").Replace("\n", "");
            }
            EndSymbleUpdate(phrase);
        }
        private void EndSymbleUpdate(TalkScript phrase)
        {
            if (!string.IsNullOrEmpty(phrase?.EndSection?.EndSymbol) &&
                EndCharDict.TryGetValue(phrase.EndSection.EndSymbol, out string endChar))
            {
                this.SelectedEndChar.Value = endChar;
            }
            else
            {
                this.SelectedEndChar.Value = "";
            }
            this.UpdateCommand.Execute("ToGraph");
            AddState();
        }

        public void SetTitleWithDirty()
        {
            this.IsDirty.Value = this.CanRegister.Value && this.CanRegisterChara.Value && CanUndo.Value;
            var title = OriginalText.Value.Substring(0, Math.Min(6, OriginalText.Value.Length)).Replace("\n", "").Replace("\r", "");
            this.TitleWithDirty.Value = title + (this.IsDirty.Value ? "*" : "");
        }

        public void CloseAction()
        {
            if (!this.CanRegister.Value || !this.CanRegisterChara.Value || !this.CanUndo.Value)
            {
                this.phraseService.Remove(this);
            }
            else
            {
                // 変更がある場合は保存するかきく
                var result = MessageBox.Show("フレーズが保存されていません。\nフレーズ登録しますか？", "フレーズ編集を閉じる", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        this.RegisterAction();
                        this.phraseService.Remove(this);
                        break;
                    case MessageBoxResult.No:
                        this.phraseService.Remove(this);
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
        }

        public void ClearUndoRedo()
        {
            this.undoRedoManager.Clear();
        }

        private void SelectAction(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                AccentSelected.Value = key == "Accent";
                VolumeSelected.Value = key == "Volume";
                SpeedSelected.Value = key == "Speed";
                PitchSelected.Value = key == "Pitch";
                EmphasisSelected.Value = key == "Emphasis";
                foreach (var s in AdditionalSettings)
                {
                    s.IsSelected.Value = s.Key == key;
                }
            }
            this.UpdateCommand.Execute("ToGraph");
        }

        private void CopyAction()
        {
            if (Phrase.Value == null) { return; }
            Phrase.Value.OriginalText = OriginalText.Value;
            Clipboard.SetText(JsonUtil.SerializeToString(Phrase.Value));
        }

        private void PasteAction()
        {
            var text = Clipboard.GetText();
            var phrase = JsonUtil.DeserializeFromString<TalkScript>(text);
            if (phrase != null &&
                !string.IsNullOrWhiteSpace(phrase.OriginalText))
            {
                OriginalText.Value = phrase.OriginalText;
                Phrase.Value = phrase;
            }
        }
    }

    public class PhraseSettingConfig
    {
        public ReactiveCommand<string> SelectCommand { get; }
        public ReactivePropertySlim<bool> IsSelected { get; } = new();
        public ReactivePropertySlim<bool> IsVisible { get; } = new();
        public bool Hide { get; }
        public string Key { get; }
        public string Description { get; }
        public string Name { get; }
        public EffectSetting Setting;
        public Brush Color { get; } = Brushes.DarkGray;
        public string IconKind { get; }

        public PhraseSettingConfig(EffectSetting setting, ReactiveCommand<string> SelectCommand)
        {
            this.SelectCommand = SelectCommand;
            Hide = setting.Hide;
            Key = setting.Key;
            Description = setting.Description;
            Name = setting.Name;
            IconKind = setting.IconKind;
            this.Setting = setting;
            try
            {
                if (!string.IsNullOrWhiteSpace(setting.Color))
                {
                    this.Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(setting.Color));
                }
            }
            catch
            {
            }
            this.IsVisible.Subscribe(_ => this.SelectCommand.Execute(""));
        }
    }
}
