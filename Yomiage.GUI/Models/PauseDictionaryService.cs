using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core.Types;
using Yomiage.GUI.EventMessages;
using Yomiage.SDK.Common;

namespace Yomiage.GUI.Models
{
    public class PauseDictionaryService
    {
        private string dictionaryPath = "user.ysdic";

        public ReactiveCollection<PauseSet> PauseDictionary { get; set; } = new(); // Dictionary なのに Dictionary じゃないけど、まあいいか。

        private readonly SettingService settingService;
        private readonly ConfigService configService;
        private readonly IDialogService dialogService;

        public PauseDictionaryService(
            SettingService settingService,
            ConfigService configService,
            IDialogService dialogService
            )
        {
            this.dialogService = dialogService;
            this.settingService = settingService;
            this.configService = configService;
            LoadDictionary();
        }

        public void Create()
        {
            this.dialogService.ShowDialog("PauseCharacterDialog", result =>
            {
                if (result.Result != ButtonResult.OK) { return; }
                if (!result.Parameters.TryGetValue("key", out string key) || key == null) { return; }
                if (!result.Parameters.TryGetValue("span_ms", out int span_ms)) { return; }
                Add(key, span_ms);
            });
        }
        public void Edit(string key)
        {
            IDialogParameters param = new DialogParameters
            {
                { "key", key }
            };
            var pair = PauseDictionary.FirstOrDefault(p => p.key == key);
            if (pair != null)
            {
                param.Add("span_ms", pair.span_ms);
            }
            this.dialogService.ShowDialog("PauseCharacterDialog", param, result =>
            {
                if (result.Result != ButtonResult.OK) { return; }
                if (!result.Parameters.TryGetValue("key", out string newKey) || newKey == null) { return; }
                if (!result.Parameters.TryGetValue("span_ms", out int span_ms)) { return; }
                if (key != newKey) { Remove(key); }
                Add(newKey, span_ms);
            });
        }
        private void Add(string key, int span_ms)
        {
            var pair = PauseDictionary.FirstOrDefault(p => p.key == key);
            if (pair == null)
            {
                pair = new PauseSet(key, span_ms);
                PauseDictionary.Add(pair);
            }
            pair.span_ms = span_ms;
            SaveDictionary(dictionaryPath);
        }
        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key)) { return; }
            var pair = PauseDictionary.FirstOrDefault(p => p.key == key);
            if (pair == null) { return; }
            PauseDictionary.Remove(pair);
            SaveDictionary(dictionaryPath);
        }


        public bool LoadDictionary(string fileName = null)
        {
            if (fileName == null)
            {
                CheckDictionaryPath();
                fileName = settingService.PauseDictionaryPath;
            }
            if (!File.Exists(fileName)) { return false; }
            try
            {
                var text = File.ReadAllText(fileName);
                if (string.IsNullOrWhiteSpace(text))
                {
                    this.PauseDictionary.Clear();
                    this.dictionaryPath = fileName;
                    return true;
                }

                var dict = JsonUtil.Deserialize<List<PauseSet>>(fileName);
                if (dict == null)
                {
                    return false;
                }
                this.PauseDictionary.Clear();
                dict.ForEach(d => this.PauseDictionary.Add(d));
            }
            catch
            {
                return false;
            }
            this.dictionaryPath = fileName;
            return true;
        }

        public void SaveDictionary(string fileName)
        {
            try
            {
                JsonUtil.Serialize(this.PauseDictionary, fileName);
                this.dictionaryPath = fileName;
            }
            catch (Exception)
            {

            }
        }

        public void CheckDictionaryPath()
        {
            if (string.IsNullOrWhiteSpace(this.settingService.PauseDictionaryPath) || this.settingService.PauseDictionaryPath == "未登録")
            {
                this.settingService.PauseDictionaryPath = Path.Combine(this.configService.PauseDirectory, "user.ysdic");
            }
            string filePath = this.settingService.PauseDictionaryPath;
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
            this.settingService.PauseDictionaryPath = filePath;
        }

    }

}
