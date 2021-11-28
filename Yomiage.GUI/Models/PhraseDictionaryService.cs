using Reactive.Bindings.Notifiers;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Yomiage.Core.Models;
using Yomiage.Core.Types;
using Yomiage.GUI.EventMessages;
using Yomiage.SDK.Talk;

namespace Yomiage.GUI.Models
{
    public class PhraseDictionaryService : PhraseDictionaryServiceBase
    {
        private readonly PhraseService phraseService;
        private readonly ScriptService scriptService;
        private readonly SettingService settingService;
        private readonly ConfigService configService;
        private readonly TextService textService;
        private readonly WordDictionaryService wordDictionaryService;
        private readonly PauseDictionaryService pauseDictionaryService;

        private readonly IMessageBroker messageBroker;
        public PhraseDictionaryService(
            SettingService settingService,
            ConfigService configService,
            PhraseService phraseService,
            ScriptService scriptService,
            TextService textService,
            WordDictionaryService wordDictionaryService,
            PauseDictionaryService pauseDictionaryService,
            IMessageBroker messageBroker
            ) : base(null)
        {
            this.phraseService = phraseService;
            this.scriptService = scriptService;
            this.settingService = settingService;
            this.configService = configService;
            this.messageBroker = messageBroker;
            this.textService = textService;
            this.wordDictionaryService = wordDictionaryService;
            this.pauseDictionaryService = pauseDictionaryService;
            {
                CheckDictionaryPath();
                var fileName = settingService.PhraseDictionaryPath;
                LoadDictionary(fileName);
            }
        }

        public override bool RegisterDictionary(string key, string engineId, TalkScript script, string libraryId = null, bool characterDict = false)
        {
            var result = base.RegisterDictionary(key, engineId, script, libraryId, characterDict);
            if (result)
            {
                this.messageBroker.Publish(new PhraseDictionaryChanged(key, DictionaryChangedMode.Register));
            }
            return result;
        }

        public override bool UnRegiserDictionary(string key)
        {
            var result = base.UnRegiserDictionary(key);
            if (result)
            {
                this.messageBroker.Publish(new PhraseDictionaryChanged(key, DictionaryChangedMode.UnRegister));
            }
            return result;
        }

        public void Edit(string key, string engineId, string libraryId)
        {
            var phrase = GetDictionary(key, engineId, libraryId);
            if (phrase == null)
            {
                var scripts = this.textService.Parse(
                    key,
                    false,
                    false,
                    "",
                    SearchDictionary,
                    this.wordDictionaryService.WordDictionarys,
                    this.pauseDictionaryService.PauseDictionary.ToList());
                if (scripts.Length == 0)
                {
                    return;
                }
                phrase = scripts.First();
            }
            phrase.OriginalText = key;
            phraseService.Send(phrase);
            this.messageBroker.Publish(new ChangeTuningTab() { TabIndex = 3 });
        }

        public void CheckDictionaryPath()
        {
            if (string.IsNullOrWhiteSpace(this.settingService.PhraseDictionaryPath) || this.settingService.PhraseDictionaryPath == "未登録")
            {
                this.settingService.PhraseDictionaryPath = Path.Combine(this.configService.PhraseDirectory,
                    Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName) + ".ypdic");
            }
            string filePath = this.settingService.PhraseDictionaryPath;
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
            this.settingService.PhraseDictionaryPath = filePath;
        }

        /// <summary>
        /// テキストに割りついたローカル辞書を探し、無ければメインの辞書を探す
        /// </summary>
        /// <param name="text"></param>
        /// <param name="preset"></param>
        /// <returns></returns>
        public TalkScript SearchDictionaryWithLocalDict(string text, VoicePreset preset)
        {
            var key = text.Replace("\r", "").Replace("\n", "");
            var dict = this.scriptService.ActiveScript.Value.PhraseDictionary?.GetDictionary(key, preset.EngineKey, preset.LibraryKey);
            dict ??= GetDictionary(key, preset?.EngineKey, preset?.LibraryKey);
            return dict;
        }

    }
}
