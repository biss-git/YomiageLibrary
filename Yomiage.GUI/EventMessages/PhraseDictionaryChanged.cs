using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.EventMessages
{
    public class PhraseDictionaryChanged
    {
        public string Key { get; set; }
        public DictionaryChangedMode Mode { get; set; }
        public PhraseDictionaryChanged(string key, DictionaryChangedMode mode)
        {
            this.Key = key;
            this.Mode = mode;
        }
    }
    public enum DictionaryChangedMode
    {
        Register,
        UnRegister,
    }
}
