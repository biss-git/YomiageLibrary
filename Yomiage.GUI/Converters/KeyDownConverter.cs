using Reactive.Bindings.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Yomiage.GUI.Converters
{
    public class KeyDownConverter : DelegateConverter<KeyEventArgs, string>
    {
        protected override string OnConvert(KeyEventArgs source)
        {
            string command = "";
            switch (source.Key)
            {
                case Key.Enter: command = "Enter"; break;
                case Key.Escape: command = "ESC"; break;
            }
            if(Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (source.Key)
                {
                    case Key.Z: command = "Undo"; break;
                    case Key.Y: command = "Redo"; break;
                }
            }
            return command;
        }
    }
}
