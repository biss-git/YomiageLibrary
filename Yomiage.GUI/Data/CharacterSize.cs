using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yomiage.GUI.Data
{
    enum CharacterSize
    {
        S50 = 0,
        S60 = 1,
        S75 = 2,
        S100 = 3,
        S120,
        S150,
        S200,
    }
    static class CharacterSizeUtil
    {
        public static double ToDouble(this CharacterSize characterSize)
        {
            switch (characterSize)
            {
                case CharacterSize.S50: return 0.5;
                case CharacterSize.S60: return 0.6;
                case CharacterSize.S75: return 0.75;
                case CharacterSize.S100: return 1;
                case CharacterSize.S120: return 1.2;
                case CharacterSize.S150: return 1.5;
                case CharacterSize.S200: return 2;
            }
            return 1;
        }
    }
}
