using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Notepad.Classes
{
    public class KeyBindings
    {
        public static ListDictionary keyBindings = new ListDictionary();
        public KeyBindings()
        {
            KeyBinding exit = new KeyBinding(Commands.Exit, new KeyGesture(Key.A, ModifierKeys.Alt));
            keyBindings.Add("exit",keyBindings);
        }
    }
}
