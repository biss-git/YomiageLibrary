using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Data
{
    enum SleepMode
    {
        SM10SEC = 0,
        SM30SEC = 1,
        SM1MIN = 2,
        SM5MIN = 3,
        SM10MIN,
        SM30MIN,
        SM1HOUR,
        NONE,
    }

    static class SleepModeUtility
    {
        public static TimeSpan ToTimeSpan(this SleepMode sleepMode)
        {
            switch (sleepMode)
            {
                case SleepMode.SM10SEC: return new TimeSpan(0, 0, 10);
                case SleepMode.SM30SEC: return new TimeSpan(0, 0, 30);
                case SleepMode.SM1MIN: return new TimeSpan(0, 1, 00);
                case SleepMode.SM5MIN: return new TimeSpan(0, 5, 00);
                case SleepMode.SM10MIN: return new TimeSpan(0, 10, 00);
                case SleepMode.SM30MIN: return new TimeSpan(0, 30, 00);
                case SleepMode.SM1HOUR: return new TimeSpan(1, 0, 00);
            }
            return new TimeSpan(0,5,0);
        }
    }
}
