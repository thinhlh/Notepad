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
using System.ComponentModel;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Configuration;
namespace Notepad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<Classes.TreeViewItem> _treeViewItems { get; set; }
        public static System.Collections.Specialized.NameValueCollection appSetting;

        public List<Classes.TreeViewItem> TreeViewItemsList
        {
            get => _treeViewItems;
            set
            {
                _treeViewItems = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            appSetting = ConfigurationManager.AppSettings;
            this.DataContext = this;
        }


        /// <summary>
        /// Loaded Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    
                    tabItems[i].FilePath = detail.path; // after set file path => Update status bar
                    MainWindowExtension.UpdateStatusBar(i);

                    (tabItems[i].Content as TabItemContentUC).richTextBoxUserControl.Language = JsonDeserialize.GetLanguageFromString(detail.language); //set language and highlight it

                    // Add content to richTextBox
                    (tabItems[i].Content as TabItemContentUC).Data = detail.text;// Set Data For RTB

                    //Scroll to the end of the text
                    //(tabItems[i].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.SelectionStart = (tabItems[i].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.Text.Length;
                    //(tabItems[i].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.ScrollToCaret();

                    tabItems[i].Header = detail.header;
                    tabItems[i].IsSaved = !detail.header.Contains("*");

                    //Change menu item to fit with content's language
                    switch ((tabItems[i].Content as TabItemContentUC).richTextBoxUserControl.Language)
                    {
                        case Snippets.Languages.None:
                            this.PlainText.IsChecked = true;
                            break;
                        case Snippets.Languages.CSharph:
                            this.CSharph.IsChecked = true;
                            break;
                        case Snippets.Languages.Java:
                            this.Java.IsChecked = true;
                            break;
                        case Snippets.Languages.CPlusPlus:
                            this.CPlusPlus.IsChecked = true;
                            break;
                        case Snippets.Languages.C:
                            this.C.IsChecked = true;
                            break;
                    }

                    i++;
                }
            }

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

        private ICommand _openLargeFileCommand;
        public ICommand OpenLargeFileCommand
        {
            get => _openLargeFileCommand ?? (_openLargeFileCommand = new Command(() => Commands.OpenLargeFileExecuted(), () => Commands.OpenLargeFileCanExecute));
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
        /// <summary>
        /// Find Command
        /// </summary>
        private ICommand _findCommand;
        public ICommand FindCommand
        {
            get => _findCommand ?? (_findCommand = new Command(() => Commands.FindExecuted(), () => Commands.FindCanExecute));
        }

        /// <summary>
        /// Replace Command
        /// </summary>
        private ICommand _replaceCommand;
        public ICommand ReplaceCommand
        {
            get => _replaceCommand ?? (_replaceCommand = new Command(() => Commands.ReplaceExecuted(), () => Commands.ReplaceCanExecute));
        }

        /// <summary>
        /// Paste Command
        /// </summary>
        private ICommand _pasteCommand;
        public ICommand PasteCommand
        {
            get => _pasteCommand ?? (_pasteCommand = new Command(() => Commands.PasteExecuted(),()=> Commands.PasteCanExecute));
        }

        /// <summary>
        /// Copy Command
        /// </summary>
        private ICommand _copyCommand;
        public ICommand CopyCommand
        {
            get => _copyCommand ?? (_copyCommand = new Command(() => Commands.CopyExecuted(), () => Commands.CopyCanExecute));
        }

        /// <summary>
        /// Cut Command
        /// </summary>
        private ICommand _cutCommand;
        public ICommand CutCommand
        {
            get => _cutCommand ?? (_cutCommand = new Command(() => Commands.CutExecuted(), () => Commands.CutCanExecute));
        }
        
        /// <summary>
        /// Undo Command
        /// </summary>
        public ICommand _undoCommand;
        public ICommand UndoCommand
        {
            get => _undoCommand ?? (_undoCommand = new Command(() => Commands.UndoExecuted(), () => Commands.UndoCanExecute));
        }

        /// <summary>
        /// Redo Command
        /// </summary>
        public ICommand _redoCommand;
        public ICommand RedoCommand
        {
            get => _redoCommand ?? (_redoCommand = new Command(() => Commands.RedoExecuted(), () => Commands.RedoCanExecute));
        }

        /// <summary>
        /// Open Containing Folder Command
        /// </summary>
        public ICommand _openContainingFolderCommand;
        public ICommand OpenContainingFolderCommand
        {
            get => _openContainingFolderCommand ?? (_openContainingFolderCommand = new Command(() => Commands.OpenContainingFolderExecuted(), () => Commands.OpenContainingFolderCanExecute));
        }

        #endregion

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                MainWindowExtension.UpdateStatusBar(tabControl.SelectedIndex);
            }

            //Change menu item to fit with content's language
            if (tabControl.SelectedIndex < 0)
            {
                this.PlainText.IsChecked = true;
                return;
            }
            else
            {
                switch ((tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.Language)
                {
                    case Snippets.Languages.None:
                        this.PlainText.IsChecked = true;
                        break;
                    case Snippets.Languages.CSharph:
                        this.CSharph.IsChecked = true;
                        break;
                    case Snippets.Languages.Java:
                        this.Java.IsChecked = true;
                        break;
                    case Snippets.Languages.CPlusPlus:
                        this.CPlusPlus.IsChecked = true;
                        break;
                    case Snippets.Languages.C:
                        this.C.IsChecked = true;
                        break;
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Commands.ExitExecuted(); //Save all current file to JSON
            base.OnClosing(e);
        }

        #region Language Check

        private void PlainText_Checked(object sender, RoutedEventArgs e)
        {
            this.CSharph.IsChecked = false;
            this.Java.IsChecked = false;
            this.CPlusPlus.IsChecked = false;
            this.C.IsChecked = false;

            if (tabControl.SelectedIndex >=0)
            {
                (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.Language = Snippets.Languages.None;
            }

        }
        private void CSharph_Checked(object sender,RoutedEventArgs e)
        {
            this.PlainText.IsChecked = false;
            this.Java.IsChecked = false;
            this.CPlusPlus.IsChecked = false;
            this.C.IsChecked = false;

            if (tabControl.SelectedIndex >= 0)
            {
                (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.Language = Snippets.Languages.CSharph;
            }

        }

        private void Java_Checked(object sender, RoutedEventArgs e)
        {
            this.PlainText.IsChecked = false;
            this.CSharph.IsChecked = false;
            this.CPlusPlus.IsChecked = false;
            this.C.IsChecked = false;

            if (tabControl.SelectedIndex >= 0)
            {
                (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.Language = Snippets.Languages.Java;
            }
        }

        private void CPlusPlus_Checked(object sender, RoutedEventArgs e)
        {
            this.PlainText.IsChecked = false;
            this.CSharph.IsChecked = false;
            this.Java.IsChecked = false;
            this.C.IsChecked = false;

            if (tabControl.SelectedIndex >= 0)
            {
                (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.Language = Snippets.Languages.CPlusPlus;
            }
        }

        private void C_Checked(object sender, RoutedEventArgs e)
        {
            this.PlainText.IsChecked = false;
            this.CSharph.IsChecked = false;
            this.Java.IsChecked = false;
            this.CPlusPlus.IsChecked = false;
            
            if (tabControl.SelectedIndex >= 0)
            {
                (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.Language = Snippets.Languages.C;
            }
        }

        
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue.GetType() == typeof(FileItem))
            {
                var currentFile = e.NewValue as FileItem;

                if (tabControl.SelectedIndex >= 0 && tabItems[tabControl.SelectedIndex].IsSaved) //close current tab and reopen in item with the choosen path
                {
                    MainWindowExtension.CloseFileExecuted(tabControl.SelectedIndex);
                }
                MainWindowExtension.OpenFileInNewTab(currentFile.language,currentFile.path);
            }
        }
    }

}