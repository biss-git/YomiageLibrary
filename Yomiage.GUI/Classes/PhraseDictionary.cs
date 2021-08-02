using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Classes
{
    public class PhraseDictionary
    {
        public DateTime DateTime { get; set; }
        public Dictionary<string, EnginePhraseDictionary> EngineList { get; set; } = new Dictionary<string, EnginePhraseDictionary>();

        public EnginePhraseDictionary GetDictionarySafely(string key)
        {
            if (!this.EngineList.TryGetValue(key, out EnginePhraseDictionary dictionary))
            {
                dictionary = new EnginePhraseDictionary();
                this.EngineList.Add(key, dictionary);
            }
            return dictionary;
        }
    }

    public class EnginePhraseDictionary
    {
        public string Default { get; set; }
        public Dictionary<string, string> CharacterList { get; set; } = new Dictionary<string, string>();
        public void RegisterCharaDictionary(string key, string jsonText)
        {
            if (this.CharacterList.ContainsKey(key))
            {
                this.CharacterList[key] = jsonText;
            }
            else
            {
                this.CharacterList.Add(key, jsonText);
            }
        }
    }
}
