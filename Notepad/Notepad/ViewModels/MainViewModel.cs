using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Notepad.Classes;
namespace Notepad.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ICommand Exit { get; set; }= Commands.Exit;
        public KeyBinding ExitKey { get; set; } = KeyBindings.keyBindings["exit"] as KeyBinding;
        
        public MainViewModel()
        {
            
        }
        public void Exit_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        public void Exit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
    }
}
