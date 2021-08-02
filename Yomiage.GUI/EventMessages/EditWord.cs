using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.SDK.Talk;

namespace Yomiage.GUI.EventMessages
{
    class EditWord
    {
        public string OriginalText { get; set; }
        public TalkScript Phrase { get; set; }
        public string Priority { get; set; }
    }
}
