using Microsoft.Win32;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Yomiage.Core.Models;
using Yomiage.GUI.Models;
using Yomiage.GUI.Util;

namespace Yomiage.GUI.ViewModels
{
    public class MainTextViewModel : ViewModelBase
    {
        private ScriptService ScriptService;
        private VoicePlayerService voicePlayerService;
        private TextService textService;

        public ReactivePropertySlim<string> Title { get; }
        public ReactivePropertySlim<string> TitleWithDirty { get; }
        public ReactivePropertySlim<string> FilePath { get; }
        public ReactivePropertySlim<Visibility> Visibility { get; }
        public ReactiveProperty<string> Content { get; }
        public ReadOnlyReactivePropertySlim<bool> IsLineNumberVisible { get; }

        public ReactivePropertySlim<string[]> Lines { get; }

        // 変更されたかどうか
        public ReactiveProperty<bool> IsDirty { get; }

        public ReactiveCommand CloseCommand { get; }
        public ReactiveCommand<string> ScriptCommand { get; }
        public AsyncReactiveCommand PlayCommand { get; }
        public AsyncReactiveCommand StopCommand { get; }
        public AsyncReactiveCommand SaveCommand { get; }

        public Func<string> GetSelectedText;
        public Func<string> GetCursorText;

        public ReactiveProperty<FlowDocument> Document { get; }
        private PhraseService phraseService;
        private PhraseDictionaryService phraseDictionaryService;
        public SettingService SettingService { get; }
        private WordDictionaryService wordDictionaryService;
        private VoicePresetService voicePresetService;
        PauseDictionaryService pauseDictionaryService;

        public MainTextViewModel(
            IDialogService _dialogService,
            SettingService settingService,
            VoicePresetService voicePresetService,
            VoicePlayerService voicePlayerService,
            ScriptService scriptService,
            PhraseService phraseService,
            PhraseDictionaryService phraseDictionaryService,
            LayoutService layoutService,
            WordDictionaryService wordDictionaryService,
            TextService textService,
            PauseDictionaryService pauseDictionaryService,
            IMessageBroker messageBroker) : base(_dialogService)
        {
            this.ScriptService = scriptService;
            this.voicePresetService = voicePresetService;
            this.voicePlayerService = voicePlayerService;
            this.textService = textService;
            this.phraseService = phraseService;
            this.phraseDictionaryService = phraseDictionaryService;
            this.SettingService = settingService;
            this.wordDictionaryService = wordDictionaryService;
            this.pauseDictionaryService = pauseDictionaryService;

            Title = new ReactivePropertySlim<string>("新規").AddTo(Disposables);
            TitleWithDirty = new ReactivePropertySlim<string>().AddTo(Disposables);
            FilePath = new ReactivePropertySlim<string>(null).AddTo(Disposables);
            Visibility = new ReactivePropertySlim<Visibility>(System.Windows.Visibility.Visible).AddTo(Disposables);
            Content = new ReactiveProperty<string>("").AddTo(Disposables);
            Document = Content.Select(text => CreateFlowDoc(text)).ToReactiveProperty().AddTo(Disposables);
            IsLineNumberVisible = layoutService.IsLineNumberVisible.ToReadOnlyReactivePropertySlim().AddTo(Disposables);

            Lines = new ReactivePropertySlim<string[]>(Enumerable.Range(1, 10).Select(x => x.ToString()).ToArray()).AddTo(Disposables);

            CloseCommand = new ReactiveCommand().WithSubscribe(CloseAction).AddTo(Disposables);
            ScriptCommand = new ReactiveCommand<string>().WithSubscribe(ScriptAction).AddTo(Disposables);
            PlayCommand = this.voicePlayerService.CanPlay.ToAsyncReactiveCommand().WithSubscribe(PlayAction).AddTo(Disposables);
            StopCommand = new AsyncReactiveCommand().WithSubscribe(voicePlayerService.Stop).AddTo(Disposables);
            SaveCommand = this.voicePlayerService.CanPlay.ToAsyncReactiveCommand().WithSubscribe(SaveAction).AddTo(Disposables);

            this.IsDirty = Observable.Merge(
                this.Content.ChangedAsObservable())
                .ToReactiveProperty(false);

            this.Title.Subscribe(_ => SetTitleWithDirty()).AddTo(Disposables);
            this.IsDirty.Subscribe(_ => SetTitleWithDirty()).AddTo(Disposables);
        }

        /// <summary>
        /// 文字列を RichTextBox の Document に変換する
        /// </summary>
        /// <param name="innerText"></param>
        /// <returns></returns>
        private FlowDocument CreateFlowDoc(string innerText)
        {
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(innerText));
            return new FlowDocument(paragraph);
        }

        private FlowDocument CreateFlowDoc_Select(string text1, string text2_selected, string text3)
        {
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(text1));
            paragraph.Inlines.Add(new Run(text2_selected) { Background = new SolidColorBrush(Colors.BlueViolet) });
            paragraph.Inlines.Add(new Run(text3));
            return new FlowDocument(paragraph);
        }

        public string GetContent()
        {
            string text = "";
            var document = this.Document.Value;
            foreach (var block in document.Blocks)
            {
                if (block is Paragraph p)
                {
                    foreach (var l in p.Inlines)
                    {
                        if (l is Run r)
                        {
                            text += r.Text;
                        }
                    }
                }
                if (block != document.Blocks.Last())
                {
                    text += Environment.NewLine;
                }
            }
            return text;
        }

        private void SetTitleWithDirty()
        {
            this.TitleWithDirty.Value = this.Title.Value + ((this.IsDirty?.Value == true) ? "*" : "");
        }

        public void CloseAction()
        {
            if (!this.IsDirty.Value)
            {
                ScriptService.Remove(this);
            }
            else
            {
                // 変更がある場合は保存するかきく
                var result = MessageBox.Show("テキストが保存されていません。\n保存しますか？", "テキストを閉じる", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        if (this.Save())
                        {
                            ScriptService.Remove(this);
                        }
                        break;
                    case MessageBoxResult.No:
                        ScriptService.Remove(this);
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
        }

        private void ScriptAction(string param)
        {
            switch (param)
            {
                case "Save":
                    this.Save();
                    break;
                case "SaveAs":
                    this.SaveAs();
                    break;
            }
        }



        private async Task PlayAction()
        {
            Content.Value = GetContent();
            var text = "";
            if (Keyboard.Modifiers == ModifierKeys.Shift &&
                GetCursorText != null)
            {
                text = GetCursorText();
            }
            else if (GetSelectedText != null)
            {
                text = GetSelectedText();
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                text = Content.Value;
            }
            var scripts = this.textService.Parse(text, SettingService.SplitByEnter, SettingService.PromptStringEnable, SettingService.PromptString, phraseDictionaryService.SearchDictionary, this.wordDictionaryService.WordDictionarys, this.pauseDictionaryService.PauseDictionary.ToList());

            Action<int> SubmitPlayIndex = async (int index) =>
            {
                string text1 = "";
                string text3 = "";
                bool beforeScript = true;
                if (index < 0 || index >= scripts.Length) { return; }
                var script = scripts[index];
                foreach (var s in scripts)
                {
                    if (s == script)
                    {
                        beforeScript = false;
                        continue;
                    }
                    if (beforeScript)
                    {
                        text1 += s.GetOriginalTextWithPresetName(SettingService.PromptString);
                    }
                    else
                    {
                        text3 += s.GetOriginalTextWithPresetName(SettingService.PromptString);
                    }
                }

                if (index > 0)
                {
                    await Task.Delay(1000);
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!this.voicePlayerService.IsPlaying.Value) { return; }
                    Document.Value = CreateFlowDoc_Select(text1, script.GetOriginalTextWithPresetName(SettingService.PromptString), text3);
                    this.phraseService.Send(script);
                });
            };

            await this.voicePlayerService.Play(scripts, SubmitPlayIndex);
            Document.Value = CreateFlowDoc(Content.Value);
            this.ScriptService.SaveScripts();
        }
        private async Task SaveAction()
        {
            Content.Value = GetContent();
            var text = "";
            if (Keyboard.Modifiers == ModifierKeys.Shift &&
                GetCursorText != null)
            {
                text = GetCursorText();
            }
            else if (GetSelectedText != null)
            {
                text = GetSelectedText();
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                text = Content.Value;
            }
            await this.voicePlayerService.Save(text);
            //var script = this.textService.Parse(Content.Value, SettingService.SplitByEnter);
            //await this.voicePlayerService.Save(script);
        }


        public bool Save()
        {
            Content.Value = GetContent();
            if (!File.Exists(FilePath.Value))
            {
                return SaveAs();
            }
            try
            {
                File.WriteAllText(FilePath.Value, Content.Value);
            }
            catch
            {
                // 保存に失敗しました。
                return false;
            }
            ResetDirty();
            return true;
        }

        public bool SaveAs()
        {
            Content.Value = GetContent();
            var sfd = new SaveFileDialog() { Filter = "テキスト文書|*.txt" };
            if (sfd.ShowDialog() != true) { return false; }
            FilePath.Value = sfd.FileName;
            Title.Value = Path.GetFileNameWithoutExtension(sfd.FileName);
            try
            {
                File.WriteAllText(FilePath.Value, Content.Value);
            }
            catch
            {
                // 保存に失敗しました。
                return false;
            }
            ResetDirty();
            return true;
        }

        public void ResetDirty()
        {
            this.IsDirty.Value = false;
        }

        public void AddPresetAction(int line1, int line2)
        {
            var text = GetContent();
            var lines = text.Replace("\r", "").Split("\n");
            for (int i = line1; i <= line2; i++)
            {
                if (i - 1 >= lines.Length || i <= 0) { continue; }
                lines[i - 1] = AddCharaLine(lines[i - 1]);
            }
            text = "";
            for (int i = 0; i < lines.Length; i++)
            {
                text += lines[i];
                if (i != lines.Length - 1)
                {
                    text += Environment.NewLine;
                }
            }
            Content.Value = text;
        }
        public void RemovePresetAction(int line1, int line2)
        {
            var text = GetContent();
            var lines = text.Replace("\r", "").Split("\n");
            for (int i = line1; i <= line2; i++)
            {
                if (i - 1 >= lines.Length || i <= 0) { return; }
                lines[i - 1] = RemoveCharaLine(lines[i - 1]);
            }
            text = "";
            for (int i = 0; i < lines.Length; i++)
            {
                text += lines[i];
                if (i != lines.Length - 1)
                {
                    text += Environment.NewLine;
                }
            }
            Content.Value = text;
        }

        private string AddCharaLine(string line)
        {
            var promptString = SettingService.PromptString;
            if (string.IsNullOrWhiteSpace(promptString)) { return line; }
            line = RemoveCharaLine(line);
            if (string.IsNullOrWhiteSpace(line)) { return line; }
            var charaName = this.voicePresetService.SelectedPreset.Value?.Name;
            if (string.IsNullOrWhiteSpace(charaName))
            {
                return line;
            }

            return charaName + promptString + line;
        }
        private string RemoveCharaLine(string line)
        {
            var promptString = SettingService.PromptString;
            if (string.IsNullOrWhiteSpace(promptString)) { return line; }
            if (line.Contains(promptString))
            {
                var index = line.IndexOf(promptString);
                if (line.Length > index + 1)
                {
                    line = line.Substring(index + 1);
                }
            }
            return line;
        }

        public void WordAction(string word)
        {
            this.wordDictionaryService.Send(word);
        }
    }


}
