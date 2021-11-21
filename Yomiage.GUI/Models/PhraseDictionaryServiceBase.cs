using System;
using System.Collections.Generic;
using System.IO;
using Yomiage.Core.Types;
using Yomiage.GUI.Classes;
using Yomiage.SDK.Common;
using Yomiage.SDK.Talk;

namespace Yomiage.GUI.Models
{
    public class PhraseDictionaryServiceBase
    {

        protected string lastFileNeme = "user.ypdic";
        public Dictionary<string, PhraseDictionary> PhraseDictionarys { get; set; } = new Dictionary<string, PhraseDictionary>();

        public PhraseDictionaryServiceBase(string dictionaryPath)
        {
            LoadDictionary(dictionaryPath);
        }

        public virtual bool RegisterDictionary(string key, string engineId, TalkScript script, string libraryId = null, bool characterDict = false)
        {
            if (string.IsNullOrWhiteSpace(key)) { return false; }
            script.OriginalText = null;
            var jsonText = script.GetPhraseJsonText_toSave();

            var dictionary = GetDictionarySafely(key);
            dictionary.DateTime = DateTime.Now;

            var engineDictionary = dictionary.GetDictionarySafely(engineId);
            if (characterDict)
            {
                engineDictionary.RegisterCharaDictionary(libraryId, jsonText);
            }
            else
            {
                engineDictionary.Default = jsonText;
                if (!string.IsNullOrWhiteSpace(libraryId) &&
                    engineDictionary.CharacterList.ContainsKey(libraryId))
                {
                    engineDictionary.RegisterCharaDictionary(libraryId, jsonText);
                }
            }

            // this.messageBroker.Publish(new PhraseDictionaryChanged(key, DictionaryChangedMode.Register));
            SaveDictionary(this.lastFileNeme);
            return true;
        }

        public void UnRegiserDictionary(TalkScript script)
        {
            UnRegiserDictionary(script.OriginalText);
        }
        public virtual bool UnRegiserDictionary(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) { return false; }
            if (PhraseDictionarys.ContainsKey(key))
            {
                PhraseDictionarys.Remove(key);
            }
            // this.messageBroker.Publish(new PhraseDictionaryChanged(key, DictionaryChangedMode.UnRegister));
            SaveDictionary(this.lastFileNeme);
            return true;
        }

        /// <summary>
        /// 登録されている辞書を返す。
        /// キャラ限定を優先して探す。
        /// 無ければエンジンに登録されている辞書を返す。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="engineId"></param>
        /// <param name="libraryId"></param>
        /// <returns></returns>
        public TalkScript GetDictionary(string key, string engineId, string libraryId = null)
        {
            if (!string.IsNullOrWhiteSpace(key) &&
                PhraseDictionarys.TryGetValue(key, out var value) &&
                value.EngineList.TryGetValue(engineId, out var engineDict))
            {
                if (!string.IsNullOrWhiteSpace(libraryId) &&
                    engineDict.CharacterList.TryGetValue(libraryId, out var charaDict))
                {
                    return TalkScript.GetTalkScript_fromPhraseJsonText(charaDict);
                }
                return TalkScript.GetTalkScript_fromPhraseJsonText(engineDict.Default);
            }
            return null;
        }
        /// <summary>
        /// true: 登録されており内容も同じ、null: 登録はされているが内容は異なる、false: 登録されていない
        /// 1つめEngine ２つめキャラ
        /// </summary>
        public (bool?, bool?) IsRegisterd(string key, TalkScript phrase, string engineId, string libraryId = null)
        {
            bool? engineRegisterd = false;
            bool? charaRegisterd = false;
            if (!string.IsNullOrWhiteSpace(key) &&
                PhraseDictionarys.TryGetValue(key, out var value) &&
                value.EngineList.TryGetValue(engineId, out var engineDict))
            {
                var jsonText = phrase.GetPhraseJsonText_toSave();
                if (!string.IsNullOrWhiteSpace(engineDict.Default))
                {
                    engineRegisterd = (jsonText == engineDict.Default) ? true : null;
                }
                if (!string.IsNullOrWhiteSpace(libraryId) &&
                    engineDict.CharacterList.TryGetValue(libraryId, out var charaDict))
                {
                    charaRegisterd = (jsonText == charaDict) ? true : null;
                }
            }
            return (engineRegisterd, charaRegisterd);
        }

        public bool LoadDictionary(string fileName = null)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName)) { return false; }
            try
            {

                var text = File.ReadAllText(fileName);
                if (string.IsNullOrWhiteSpace(text))
                {
                    this.PhraseDictionarys = new Dictionary<string, PhraseDictionary>();
                    this.lastFileNeme = fileName;
                    return true;
                }

                var dict = JsonUtil.Deserialize<Dictionary<string, PhraseDictionary>>(fileName);
                if (dict == null)
                {
                    return false;
                }
                this.PhraseDictionarys = dict;
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
                JsonUtil.Serialize(this.PhraseDictionarys, fileName);
                this.lastFileNeme = fileName;
            }
            catch (Exception)
            {

            }
        }

        private PhraseDictionary GetDictionarySafely(string key)
        {
            if (!PhraseDictionarys.TryGetValue(key, out PhraseDictionary dictionary))
            {
                dictionary = new PhraseDictionary();
                PhraseDictionarys.Add(key, dictionary);
            }
            return dictionary;
        }

        public TalkScript SearchDictionary(string text, VoicePreset preset)
        {
            var key = text.Replace("\r", "").Replace("\n", "");
            return GetDictionary(key, preset.EngineKey, preset.LibraryKey);
        }
    }
}
