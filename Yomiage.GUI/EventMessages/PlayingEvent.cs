using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yomiage.Core.Types;

namespace Yomiage.GUI.EventMessages
{
    class PlayingEvent
    {
        public double[] part { get; set; }
        public int fs { get; set; }
        public VoicePreset preset { get; set; }
    }
}
