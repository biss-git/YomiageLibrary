using ImTools;
using Microsoft.Win32;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
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
using Yomiage.GUI.Views;
using Yomiage.SDK.Talk;

namespace Yomiage.GUI.ViewModels
{
    public class MainTextViewModel : ViewModelBase
    {
        private readonly ScriptService ScriptService;
        public VoicePlayerService VoicePlayerService { get; }
        private readonly TextService textService;

        public PhraseDictionaryServiceBase PhraseDictionary { get; private set; }

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
        public ReactiveCommand PlayCommand { get; }
        public AsyncReactiveCommand StopCommand { get; }
        public AsyncReactiveCommand SaveCommand { get; }
        public ReactiveCommand NewTabCommand { get; }
        public ReactiveCommand<KeyEventArgs> KeyDownCommand { get; }

        public MainText MainText;

        public ReadOnlyReactivePropertySlim<FlowDocument> Document { get; }
        //public ReactivePropertySlim<FlowDocument> Document_Playing { get; }
        private readonly PhraseService phraseService;
        private readonly PhraseDictionaryService phraseDictionaryService;
        public SettingService SettingService { get; }
        private readonly WordDictionaryService wordDictionaryService;
        private readonly VoicePresetService voicePresetService;
        readonly PauseDictionaryService pauseDictionaryService;

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
            PauseDictionaryService pauseDictionaryService)
            : base(_dialogService)
        {
            this.ScriptService = scriptService;
            this.voicePresetService = voicePresetService;
            this.VoicePlayerService = voicePlayerService;
            this.textService = textService;
            this.phraseService = phraseService;
            this.phraseDictionaryService = phraseDictionaryService;
            this.SettingService = settingService;
            this.wordDictionaryService = wordDictionaryService;
            this.pauseDictionaryService = pauseDictionaryService;

            Title = new ReactivePropertySlim<string>("新規").AddTo(Disposables);
            TitleWithDirty = new ReactivePropertySlim<string>().AddTo(Disposables);
            FilePath = new ReactivePropertySlim<string>(null).AddTo(Disposables);
            FilePath.Subscribe(FileNameChanged).AddTo(Disposables);
            Visibility = new ReactivePropertySlim<Visibility>(System.Windows.Visibility.Visible).AddTo(Disposables);
            Content = new ReactiveProperty<string>("").AddTo(Disposables);
            Document = Content.Select(text => CreateFlowDoc(text)).ToReadOnlyReactivePropertySlim().AddTo(Disposables);
            //Document_Playing = new ReactivePropertySlim<FlowDocument>().AddTo(Disposables);
            IsLineNumberVisible = layoutService.IsLineNumberVisible.ToReadOnlyReactivePropertySlim().AddTo(Disposables);

            Lines = new ReactivePropertySlim<string[]>(Enumerable.Range(1, 10).Select(x => x.ToString()).ToArray()).AddTo(Disposables);

            CloseCommand = new ReactiveCommand().WithSubscribe(CloseAction).AddTo(Disposables);
            ScriptCommand = new ReactiveCommand<string>().WithSubscribe(ScriptAction).AddTo(Disposables);
            PlayCommand = new ReactiveCommand().WithSubscribe(() => PlayAction()).AddTo(Disposables);
            StopCommand = new AsyncReactiveCommand().WithSubscribe(voicePlayerService.Stop).AddTo(Disposables);
            SaveCommand = this.VoicePlayerService.CanSave.ToAsyncReactiveCommand().WithSubscribe(SaveAction).AddTo(Disposables);
            NewTabCommand = new ReactiveCommand().WithSubscribe(NewTabAction).AddTo(Disposables);
            KeyDownCommand = new ReactiveCommand<KeyEventArgs>().WithSubscribe(KeyDownAction).AddTo(Disposables);

            this.VoicePlayerService.IsPlaying.Subscribe(playing =>
            {
                if (playing)
                {
                    if (MainText != null)
                    {
                        MainText.SetPlayingDocument(CreateFlowDoc(Content.Value));
                    }
                    //Application.Current.Dispatcher.Invoke(() =>
                    //{
                    //    Document_Playing.Value = CreateFlowDoc(Content.Value);
                    //});
                }
            });

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
        private static FlowDocument CreateFlowDoc(string innerText)
        {
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(innerText));
            return new FlowDocument(paragraph);
        }

        private static FlowDocument CreateFlowDoc_Select(string text1, string text2_selected, string text3)
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



        public async Task PlayAction(string playParam = null)
        {
            Content.Value = GetContent();

            var text = "";
            var beforeText = "";
            var afterText = "";
            switch (playParam)
            {
                case "CtrlLeft":
                    if (MainText != null)
                    {
                        await VoicePlayerService.Stop();
                        (beforeText, text, afterText) = MainText.CtrlLeft();
                    }
                    break;
                case "CtrlRight":
                    if (MainText != null)
                    {
                        await VoicePlayerService.Stop();
                        (beforeText, text, afterText) = MainText.CtrlRight();
                    }
                    break;
                default:
                    if (this.VoicePlayerService.IsPlaying.Value)
                    {
                        await this.VoicePlayerService.Pause();
                        return;
                    }
                    // 通常の再生
                    if (Keyboard.Modifiers == ModifierKeys.Shift &&
                        MainText != null)
                    {
                        // シフトが押されているとき
                        (beforeText, text, afterText) = MainText.GetCursorText();
                    }
                    else if (MainText != null)
                    {
                        // 範囲選択されているとき
                        (beforeText, text, afterText) = MainText.GetSelectedText();
                    }
                    break;
            }
            if (string.IsNullOrWhiteSpace(text) && string.IsNullOrEmpty(beforeText) && string.IsNullOrEmpty(afterText))
            {
                beforeText = "";
                afterText = "";
                text = Content.Value;
            }
            var scripts = this.textService.Parse(
                text,
                SettingService.SplitByEnter,
                SettingService.PromptStringEnable,
                SettingService.PromptString,
                phraseDictionaryService.SearchDictionaryWithLocalDict,
                this.wordDictionaryService.WordDictionarys,
                this.pauseDictionaryService.PauseDictionary.ToList());

            int calledIndex = -1;

            Task.Run(async () =>
            {
                await Task.Delay(200);
                var script = scripts.FirstOrDefault(x => x.MoraCount > 0);
                if (script != null)
                {
                    var index = scripts.IndexOf(script);
                    SubmitPlayIndex(index);
                }
            });

            async void SubmitPlayIndex(int index)
            {
                if (index <= calledIndex)
                {
                    return;
                }

                calledIndex = index;

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

                text1 = beforeText + text1;
                text3 = text3 + afterText;

                if (index > 0)
                {
                    await Task.Delay(800);
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!this.VoicePlayerService.IsPlaying.Value) { return; }
                    MainText?.SetPlayingDocument(CreateFlowDoc_Select(text1, script.GetOriginalTextWithPresetName(SettingService.PromptString), text3));
                    //Document_Playing.Value = CreateFlowDoc_Select(text1, script.GetOriginalTextWithPresetName(SettingService.PromptString), text3);
                    this.phraseService.Send(script);
                });
            }

            await this.VoicePlayerService.Play(scripts, SubmitPlayIndex);
            this.ScriptService.SaveScripts();

            Application.Current.Dispatcher.Invoke(() =>
            {
                var a = this.MainText.rich.Focus();
            });
        }

        private async Task SaveAction()
        {
            Content.Value = GetContent();
            var text = "";
            if (Keyboard.Modifiers == ModifierKeys.Shift &&
                MainText != null)
            {
                (_, text, _) = MainText.GetCursorText();
            }
            else if (MainText != null)
            {
                (_, text, _) = MainText.GetSelectedText();
            }
            if (string.IsNullOrWhiteSpace(text))
            {
                text = Content.Value;
            }
            await this.VoicePlayerService.Save(text);
        }


        public bool Save()
        {
            Content.Value = GetContent();

            if (!string.IsNullOrWhiteSpace(FilePath.Value))
            {
                try
                {
                    File.Create(FilePath.Value).Close();
                }
                catch (Exception)
                {

                }
            }

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
                    line = line[(index + 1)..];
                }
            }
            return line;
        }

        public void WordAction(string word)
        {
            this.wordDictionaryService.Send(word);
        }

        private void NewTabAction()
        {
            this.ScriptService.AddNew();
        }

        private void FileNameChanged(string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                if (Path.GetExtension(fileName) == ".txt")
                {
                    var dicPath = fileName + ".ypdic";
                    if (!File.Exists(dicPath))
                    {
                        File.Create(dicPath).Close();
                    }
                    PhraseDictionary = new PhraseDictionaryServiceBase(dicPath);
                }
                else
                {
                    // txt 以外が来た場合は txt に直す
                    fileName = Path.ChangeExtension(fileName, ".txt");
                    FilePath.Value = fileName;
                }
            }
            else
            {
                PhraseDictionary = null;
            }
        }

        private void KeyDownAction(KeyEventArgs source)
        {
            if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt))
            {
                // Ctrl + Alt + Shift が押されているとき
                switch (source.Key)
                {
                    case Key.S:
                        this.SaveAction();
                        break;
                }
            }
        }

        public void AddDict(TalkScript[] dict)
        {
            if (dict == null || PhraseDictionary == null)
            {
                return;
            }

            foreach (var d in dict)
            {
                PhraseDictionary.RegisterDictionary(d.OriginalText, d.EngineName, d);
            }
        }
    }
}
