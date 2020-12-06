using System.Windows.Controls;
using System.Windows.Input;

namespace Notepad.Classes
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand CloseFile = new RoutedUICommand(
            "Close File",
            "Close File",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.W, ModifierKeys.Control) //Multi ModifierKeys
            }
        );

        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            "Exit",
            "Exit",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F4, ModifierKeys.Alt) //Multi ModifierKeys
            }
        );

        public static readonly RoutedUICommand NewTerminal = new RoutedUICommand(
            "Terminal",
            "Terminal",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.T, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand NewTerminalCurrentDir = new RoutedUICommand(
            "Terminal in Current Directory",
            "Terminal in Current Directory",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.T, ModifierKeys.Control|ModifierKeys.Shift) //Multi ModifierKeys
            }
        );

        public static readonly RoutedUICommand Build = new RoutedUICommand(
            "Build",
            "Build",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F5)
            }
        );

        public static readonly RoutedUICommand BuildAndRun = new RoutedUICommand(
            "Build and Run",
            "Build and Run",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F5, ModifierKeys.Control)
            }
        );
    }
}
