using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Notepad.Classes;
using System.Text.RegularExpressions;
using Notepad.Snippets;
namespace Notepad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var details = MainWindowExtension.DeserializeTemporaryDetail();
            if (details == null)
            {
                MainWindowExtension.InitializeTabItem();
            }
            else
            {
                int i = 0;
                foreach(TemporaryDetail detail in details)
                {
                    MainWindowExtension.InitializeTabItem();
                    tabItems[i].FilePath = detail.path;
                    tabItems[i].Header = detail.header;
                    tabItems[i].Data = detail.text;
                    tabItems[i].IsSaved = detail.header.Contains("*");


                    (tabItems[i].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.TextChanged -= (tabItems[i].Content as TabItemContentUC).richTextBoxUserControl.richTextBox_Highlight;

                    // Add content to richTextBox
                    (tabItems[i].Content as TabItemContentUC).Data = tabItems[i].Data;// Set Data For RTB

                    //Highlight
                    (tabItems[i].Content as TabItemContentUC).richTextBoxUserControl.highlighter.HighlightRange(0, (tabItems[i].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.Text.Length);

                    //Resubscribe
                    (tabItems[i].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.TextChanged += (tabItems[i].Content as TabItemContentUC).richTextBoxUserControl.richTextBox_Highlight;



                    //Scroll to the end of the text
                    (tabItems[i].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.SelectionStart = (tabItems[i].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.Text.Length;
                    (tabItems[i].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.ScrollToCaret();

                    i++;
                }
            }

            this.PlainText.IsChecked = true;
        }

        #region Variables
        /// <summary>
        /// Contains List of TabItems Inside TabControl
        /// </summary>
        public List<MainTabItem> tabItems = new List<MainTabItem>();
        public List<int> closedTabIndexes = new List<int>();// this List holds indexs of tabs that was removed from tabControl
        #endregion

        #region Commands

        /// <summary>
        /// New File Command
        /// </summary>
        private ICommand _newFileCommand;
        public ICommand NewFileCommand
        {
            get=> _newFileCommand ?? (_newFileCommand = new Command(() => Commands.NewFileExecuted(), () => Commands.NewFileCanExecute));
        }

        /// <summary>
        /// Open File Command 
        /// </summary>
        private ICommand _openFileCommand;
        public ICommand OpenFileCommand 
        {
            get => _openFileCommand ?? (_openFileCommand = new Command(() => Commands.OpenFileExecuted(), () => Commands.OpenFileCanExecute));
        }

        /// <summary>
        /// Open Folder Command
        /// </summary>
        private ICommand _openFolderCommand;
        public ICommand OpenFolderCommand
        {
            get => _openFolderCommand ?? (_openFolderCommand = new Command(() => Commands.OpenFolderExecuted(), () => Commands.OpenFolderCanExecute));
        }

        /// <summary>
        /// Save Command
        /// </summary>
        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get => _saveCommand ?? (_saveCommand = new Command(() => Commands.SaveExecuted(), () => Commands.SaveCanExecute));
        }

        /// <summary>
        /// Save As Command
        /// </summary>
        private ICommand _saveAsCommand;
        public ICommand SaveAsCommand
        {
            get => _saveAsCommand ?? (_saveAsCommand = new Command(() => Commands.SaveAsExecuted(), () => Commands.SaveAsCanExecute));
        }

        /// <summary>
        /// Save All Command
        /// </summary>
        private ICommand _saveAllCommand;
        public ICommand SaveAllCommand
        {
            get => _saveAllCommand ?? (_saveAllCommand = new Command(() => Commands.SaveAllExecuted(), () => Commands.SaveAllCanExecute));
        }

        /// <summary>
        /// Close File Command
        /// </summary>
        private ICommand _closeFileCommand;
        public ICommand CloseFileCommand
        {
            get => _closeFileCommand ?? (_closeFileCommand = new Command(() => Commands.CloseFileExecuted(), () => Commands.CloseFileCanExecute));
        }

        /// <summary>
        /// Close All Files Command
        /// </summary>
        private ICommand _closeAllFilesCommand;
        public ICommand CloseAllFilesCommand
        {
            get => _closeAllFilesCommand ?? (_closeAllFilesCommand = new Command(() => Commands.CloseAllFilesExecuted(), () => Commands.CloseAllFilesCanExecute));
        }

        /// <summary>
        /// New Window Command
        /// </summary>
        private ICommand _newWindowCommand;
        public ICommand NewWindowCommand
        {
            get => _newWindowCommand ?? (_newWindowCommand = new Command(() => Commands.NewWindowExecuted(), () => Commands.NewWindowCanExecute));
        }

        /// <summary>
        /// Exit Command
        /// </summary>
        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get => _exitCommand ?? (_exitCommand = new Command(() => Commands.ExitExecuted(), () => Commands.ExitCanExecute));
        }

        /// <summary>
        /// New Terminal Command
        /// </summary>
        private ICommand _newTerminalCommand;
        public ICommand NewTerminalCommand
        {
            get => _newTerminalCommand ?? (_newTerminalCommand = new Command(() => Commands.NewTerminalExecuted(), () => Commands.NewTerminalCanExecute));
        }

        /// <summary>
        /// New Terminal at Current Directory Command
        /// </summary>
        private ICommand _newTerminalCurrentDirCommand;
        public ICommand NewTerminalCurrentDirCommand
        {
            get => _newTerminalCurrentDirCommand ?? (_newTerminalCurrentDirCommand = new Command(() => Commands.NewTerminalCurrentDirExecuted(), () => Commands.NewTerminalCurrentDirCanExecute));
        }

        /// <summary>
        /// Build Command
        /// </summary>
        private ICommand _buildCommand;
        public ICommand BuildCommand
        {
            get => _buildCommand ?? (_buildCommand = new Command(() => Commands.BuildExecuted(), () => Commands.BuildCanExecute));
        }

        /// <summary>
        /// Build And Run Command
        /// </summary>
        private ICommand _buildAndRunCommand;
        public ICommand BuildAndRunCommand
        {
            get => _buildAndRunCommand ?? (_buildAndRunCommand = new Command(() => Commands.BuildAndRunExecuted(), () => Commands.BuildAndRunCanExecute));
        }

        #endregion

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                MainWindowExtension.UpdateStatusBar(tabControl.SelectedIndex);
            }


            //Change menu item to fit with content's language
            //switch((tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.Language)
            //{
            //    case Snippets.Languages.None:
            //        this.PlainText.IsChecked = true;
            //        break;
            //    case Snippets.Languages.CSharph:
            //        this.CSharph.IsChecked = true;
            //        break;
            //    case Snippets.Languages.Java:
            //        this.Java.IsChecked = true;
            //        break;
            //    case Snippets.Languages.CPlusPlus:
            //        this.CPlusPlus.IsChecked = true;
            //        break;
            //    case Snippets.Languages.C:
            //        this.C.IsChecked = true;
            //        break;
            //}
        }

        #region Language Check

        private void PlainText_Checked(object sender, RoutedEventArgs e)
        {
            this.CSharph.IsChecked = false;
            this.Java.IsChecked = false;
            this.CPlusPlus.IsChecked = false;
            this.C.IsChecked = false;

            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.Language = Snippets.Languages.None;

        }
        private void CSharph_Checked(object sender,RoutedEventArgs e)
        {
            this.PlainText.IsChecked = false;
            this.Java.IsChecked = false;
            this.CPlusPlus.IsChecked = false;
            this.C.IsChecked = false;
            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.Language = Snippets.Languages.CSharph;
        }

        private void Java_Checked(object sender, RoutedEventArgs e)
        {
            this.PlainText.IsChecked = false;
            this.CSharph.IsChecked = false;
            this.CPlusPlus.IsChecked = false;
            this.C.IsChecked = false;

            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.Language = Snippets.Languages.Java;
        }

        private void CPlusPlus_Checked(object sender, RoutedEventArgs e)
        {
            this.PlainText.IsChecked = false;
            this.CSharph.IsChecked = false;
            this.Java.IsChecked = false;
            this.C.IsChecked = false;

            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.Language = Snippets.Languages.CPlusPlus;
        }

        private void C_Checked(object sender, RoutedEventArgs e)
        {
            this.PlainText.IsChecked = false;
            this.CSharph.IsChecked = false;
            this.Java.IsChecked = false;
            this.CPlusPlus.IsChecked = false;

            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.Language = Snippets.Languages.C;
        }

        #endregion
    }

}