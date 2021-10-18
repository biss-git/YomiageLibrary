using System;
using System.Collections.Generic;
using System.Text;

namespace Yomiage.API
{
    public static class CommandService
    {
        public static Func<string, bool> PlayVoice;

        public static Action StopAction;
    }
}
