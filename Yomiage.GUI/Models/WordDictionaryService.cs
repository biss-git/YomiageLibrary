using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core.Types;
using Yomiage.GUI.Classes;
using Yomiage.GUI.EventMessages;
using Yomiage.SDK.Common;
using Yomiage.SDK.Talk;

namespace Yomiage.GUI.Models
{
    public class WordDictionaryService
    {
        private string lastFileNeme = "user.ywdic";
        public Dictionary<string, WordSet> WordDictionarys { get; set; } = new(); // なんでこれ最後に s がついているんでしょうね。

        private readonly SettingService settingService;
        private readonly ConfigService configService;
        private readonly IMessageBroker messageBroker;

        public WordDictionaryService(
            SettingService settingService,
            ConfigService configService,
            IMessageBroker messageBroker
            )
        {
            this.settingService = settingService;
            this.configService = configService;
            this.messageBroker = messageBroker;
            LoadDictionary();
        }

        public bool RegisterDictionary(TalkScript script, string priority)
        {
            var key = script.OriginalText;
            if (string.IsNullOrWhiteSpace(key)) { return false; }
            script.OriginalText = null;
            var jsonText = script.GetPhraseJsonText_toSave();

            Add(key, new WordSet() { JsonText = jsonText, Priority = priority, Yomi = script.GetYomi() });

            script.OriginalText = key;
            SaveDictionary(this.lastFileNeme);
            return true;
        }

        public void UnRegiserDictionary(TalkScript script)
        {
            UnRegiserDictionary(script.OriginalText);
        }
        public void UnRegiserDictionary(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) { return; }
            if (WordDictionarys.ContainsKey(key))
            {
                WordDictionarys.Remove(key);
            }
            SaveDictionary(this.lastFileNeme);
        }

        public void Edit(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) { return; }
            EditWord edit = new() { OriginalText = key, Phrase = new TalkScript(), Priority = "3.標準" };
            if (WordDictionarys.TryGetValue(key, out WordSet word))
            {
                edit.Phrase = TalkScript.GetTalkScript_fromPhraseJsonText(word.JsonText);
                edit.Priority = word.Priority;
            }
            this.messageBroker.Publish(edit);
            this.messageBroker.Publish(new ChangeTuningTab() { TabIndex = 4 });
        }

        public void Send(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) { return; }
            EditWord edit = new() { OriginalText = word, Phrase = null, Priority = "3.標準" };
            this.messageBroker.Publish(edit);
            this.messageBroker.Publish(new ChangeTuningTab() { TabIndex = 4 });
        }

        public bool LoadDictionary(string fileName = null)
        {
            if (fileName == null)
            {
                CheckDictionaryPath();
                fileName = settingService.WordDictionaryPath;
            }
            if (!File.Exists(fileName)) { return false; }
            try
            {
                var text = File.ReadAllText(fileName);
                if (string.IsNullOrWhiteSpace(text))
                {
                    this.WordDictionarys = new Dictionary<string, WordSet>();
                    this.lastFileNeme = fileName;
                    return true;
                }

                var dict = JsonUtil.Deserialize<Dictionary<string, WordSet>>(fileName);
                if (dict == null)
                {
                    return false;
                }
                this.WordDictionarys = dict;
            }
            catch
            {
                return false;
            }
            this.lastFileNeme = fileName;
            return true;
        }

        public void SaveDictionary(string fileName)
        {
            try
            {
                JsonUtil.Serialize(this.WordDictionarys, fileName);
                this.lastFileNeme = fileName;
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// true: 登録されていて内容も同じ
        /// null: 登録はされているが内容は異なる
        /// false: 登録されていない。
        /// </summary>
        public bool? IsRegisterd(string key, TalkScript phrase = null)
        {
            if (string.IsNullOrWhiteSpace(key)) { return false; }
            if (!WordDictionarys.TryGetValue(key, out WordSet word))
            {
                return false;
            }
            if (phrase != null && phrase.GetPhraseJsonText_toSave() == word.JsonText)
            {
                return true;
            }
            return null;
        }

        public void CheckDictionaryPath()
        {
            if (string.IsNullOrWhiteSpace(this.settingService.WordDictionaryPath) || this.settingService.WordDictionaryPath == "未登録")
            {
                this.settingService.WordDictionaryPath = Path.Combine(this.configService.WordDirectory, "user.ywdic");
            }
            string filePath = this.settingService.WordDictionaryPath;
            if (!File.Exists(filePath))
            {
                try
                {
                    File.Create(filePath).Close();
                }
                catch
                {
                    return;
                }
                if (!File.Exists(filePath)) { return; }
            }
            this.settingService.WordDictionaryPath = filePath;
        }

        private void Add(string key, WordSet word)
        {
            if (WordDictionarys.ContainsKey(key))
            {
                WordDictionarys[key] = word;
            }
            else
            {
                WordDictionarys.Add(key, word);
            }
        }

    }
}
