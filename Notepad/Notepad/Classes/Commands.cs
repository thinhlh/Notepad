using System.Windows;
using System.Windows.Input;

namespace Notepad.Classes
{
    public class Commands
    {
        public static ICommand Exit = new ViewModels.RelayCommand<object>((p) => { return true; }, (p) => { Application.Current.Shutdown(); });
        
    }
}
