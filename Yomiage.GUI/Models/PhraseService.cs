using Prism.Ioc;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Yomiage.GUI.ViewModels;
using Yomiage.SDK.Common;
using Yomiage.SDK.Talk;

namespace Yomiage.GUI.Models
{
    public class PhraseService : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        public void Dispose() => _disposables.Dispose();

        private readonly ObservableCollection<PhraseEditorViewModel> editors = new();
        public ReadOnlyObservableCollection<PhraseEditorViewModel> Editors { get; }
        public ReactivePropertySlim<PhraseEditorViewModel> ActiveEditor { get; } = new ReactivePropertySlim<PhraseEditorViewModel>();

        private readonly IContainerExtension container;

        public PhraseService(IContainerExtension container)
        {
            this.container = container;
            Editors = new ReadOnlyObservableCollection<PhraseEditorViewModel>(editors);
        }

        private void Add(PhraseEditorViewModel editor) => editors.Add(editor);
        public void AddNew(TalkScript script = null)
        {
            var editor = container.Resolve<PhraseEditorViewModel>();
            if (script != null)
            {
                editor.ClearUndoRedo();
                editor.Phrase.Value = script;
            }
            Add(editor);
            ActiveEditor.Value = editor;
        }
        public void AddWithFocus(PhraseEditorViewModel editor)
        {
            Add(editor);
            ActiveEditor.Value = editor;
        }
        public void Send(TalkScript _script)
        {
            var script = JsonUtil.DeepClone(_script);
            foreach (var editor in Editors)
            {
                if (!editor.IsDirty.Value && editor.Phrase.Value?.OriginalText == script.OriginalText)
                {
                    // 内容が同じものがあれば送る
                    editor.ClearUndoRedo();
                    editor.Phrase.Value = script;
                    this.ActiveEditor.Value = editor;
                    return;
                }
            }
            if (!ActiveEditor.Value.IsDirty.Value)
            {
                // アクティブなものに上書きしても良ければ送る
                ActiveEditor.Value.ClearUndoRedo();
                ActiveEditor.Value.Phrase.Value = script;
                return;
            }
            AddNew(script);
        }
        public void CopyWithFocus(PhraseEditorViewModel editor)
        {
            var newEditor = container.Resolve<PhraseEditorViewModel>();
            newEditor.Phrase.Value = JsonUtil.DeepClone(editor.Phrase.Value);
            newEditor.OriginalText.Value = editor.OriginalText.Value;
            Add(newEditor);
            ActiveEditor.Value = newEditor;
        }

        public void Remove(PhraseEditorViewModel editor)
        {
            if (editors.Contains(editor))
            {
                editors.Remove(editor);
            }
            if (editors.Count == 0)
            {
                AddNew();
            }
        }


        public void SaveEditors()
        {
            var dict = editors.Select(e =>
                new Dictionary<string, string>()
                {
                    {"IsActive" , (e == ActiveEditor.Value).ToString()},
                    {"OriginalText" , e.OriginalText.Value},
                    {"Phrase" , JsonUtil.SerializeToString(e.Phrase.Value)},
                });
            JsonUtil.Serialize(dict, "editors.json");
        }
        public void LoadEditors()
        {
            if (!File.Exists("editors.json")) { AddNew(); return; }
            try
            {
                var dict = JsonUtil.Deserialize<Dictionary<string, string>[]>("editors.json");
                foreach (var s in dict)
                {
                    try
                    {
                        var editor = container.Resolve<PhraseEditorViewModel>();
                        editor.ClearUndoRedo();
                        //editor.Title.Value = s["Title"];
                        editor.OriginalText.Value = s["OriginalText"];
                        editor.LoadPhraseDictionary(s["Phrase"]);
                        if (s["IsActive"] == "True")
                        {
                            this.AddWithFocus(editor);
                        }
                        else
                        {
                            this.Add(editor);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch
            {

            }
            if (this.editors.Count == 0) { AddNew(); }
        }

    }
}
