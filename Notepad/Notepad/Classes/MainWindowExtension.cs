using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Notepad;
using Notepad.Snippets;

namespace Notepad.Classes
{
    /// <summary>
    /// Use for Extension Methods of MainWindow
    /// </summary>
    public static class MainWindowExtension
    {
        /// <summary>
        /// Params to access fields easiers
        /// </summary>
        private static MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        private static TabControl tabControl = (Application.Current.MainWindow as MainWindow).tabControl;
        private static List<MainTabItem> tabItems = (Application.Current.MainWindow as MainWindow).tabItems;
        private static List<int> closedTabIndexes = (Application.Current.MainWindow as MainWindow).closedTabIndexes;
        private static System.Collections.Specialized.NameValueCollection appSetting = ConfigurationManager.AppSettings;

        public static void InitializeTabItem()
        {
            MainTabItem tabItem = new MainTabItem();
            //
            //Add tabItem to tabControl
            //
            tabControl.Items.Add(tabItem);
            tabItems.Add(tabItem);
            tabItem.Focus(); // selected index will focus on the new tab



            //Setup for tabItem
            int tabIndex = FindIndexForTab();
            tabItem.Header = "Document " + (tabIndex + 1); // Header Display Always larger than 1 of the number of element in tabItems
            tabItem.Name = "TabItem" + (tabIndex);

            //Set Content for TabItem
        }

        public static int FindIndexForTab() // this function define which tabIndex is approriate for InitTab 
        {
            if (closedTabIndexes.Count == 0)
                return tabControl.Items.Count - 1;
            else
            {
                int index = closedTabIndexes[0];
                closedTabIndexes.RemoveAt(0);
                return index;
            }
        }

        public static void UpdateStatusBar(int index)
        {
            if (index == -1)
                mainWindow.StatusText.DataContext = "None";// No update when close all tab
            else if (tabItems[index].FilePath == "")
                mainWindow.StatusText.DataContext = "Plain Text";
            else
                mainWindow.StatusText.DataContext = tabItems[tabControl.SelectedIndex].FilePath;
        }
        public static void RaiseUnsavedIcon()
        {
            List<MainTabItem> tabItems = mainWindow.tabItems;
            TabControl tabControl = mainWindow.tabControl;

            if (tabControl.SelectedIndex < 0) return;
            if (tabItems[tabControl.SelectedIndex].IsSaved == false)
            {
                return;
            }
            else
            {
                //Raise* at the end and keep isSaved = false when there is a change with out save before
                tabItems[tabControl.SelectedIndex].Header += "*";
                tabItems[tabControl.SelectedIndex].IsSaved = false;  
            }
            //Resubscribe when this tab is Saved
        }

        private static void RemoveSavedIcon(int index)
        {
            tabItems[index].IsSaved = true;

            string header = tabItems[index].Header.ToString();
            tabItems[index].Header = header.Remove(header.Length - 1, 1);
        }
        public static string GetParentFullPath(int index)
        {
            return Path.Combine(tabItems[index].FilePath, "..");
        }

        public static DirectoryInfo TryGetSolutionDirectoryInfo(string currentPath = null)
        {
            var directory = new DirectoryInfo(
                currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            return directory;
        }

        public static List<TemporaryDetail> DeserializeTemporaryDetail()
        {
            string JsonPath = TryGetSolutionDirectoryInfo().FullName + @"\Notepad\temp\TabDetails.json";

            if (!File.Exists(JsonPath)) return null;
            string output = File.ReadAllText(JsonPath);
            var details = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TemporaryDetail>>(output);
            return details;
        }

        public static void SaveExecuted(int index)
        {
            if (!tabItems[index].IsSaved || string.IsNullOrWhiteSpace(tabItems[index].Data)) // not yet saved or new tab but not have data
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                bool fileExsisted = System.IO.File.Exists(tabItems[index].FilePath);
                if (fileExsisted)
                {
                    System.IO.File.WriteAllText(tabItems[index].FilePath, tabItems[index].Data);
                    RemoveSavedIcon(index);
                }
                else SaveAsExecuted(index);
            }
            //subscribe to listen when text changed again
        }
        public static void SaveAsExecuted(int index)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text (*.txt)|*.txt| Java (*.java) |*.java| C (*.c) |*.c| C++ (*.cpp) |*.cpp| C# (*.cs) |*.cs| All files (*.*) |*.* ";
            string tabHeader = (string)tabItems[tabControl.SelectedIndex].Header;
            saveFileDialog.FileName = tabHeader.Substring(0, tabHeader.Length - 1); // remove the * flag
            if (saveFileDialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName, tabItems[index].Data);
                RemoveSavedIcon(index);
                tabItems[index].Header = Path.GetFileName(saveFileDialog.FileName);
                tabItems[index].FilePath = saveFileDialog.FileName;

                //Update Status Bar
                UpdateStatusBar(index);
            }
            
            mainWindow.treeView.Items.Refresh();
            mainWindow.treeView.UpdateLayout();
        }
        public static void CloseFileExecuted(int index)
        {
            if (tabItems[index].IsSaved == false)
            {
                //Message then request save
                string tabHeader = (string)tabItems[index].Header;
                string message = tabHeader.Substring(0, tabHeader.Length - 1) + " have been modified, save changes?";

                MessageBoxResult result = MessageBox.Show(message, "Request", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    MainWindowExtension.SaveAsExecuted(index);
                else if (result == MessageBoxResult.Cancel)
                    return;
            }
            tabControl.Items.RemoveAt(index);
            int deletedIndexTab = Int16.Parse(tabItems[index].Name.Substring(7)); // Return the index of deleted tabItem by get subTring from name then convert to int

            tabItems.RemoveAt(index);

            //Add Index of tab then sort it for reopen new tab situation 
            closedTabIndexes.Add(deletedIndexTab);
            closedTabIndexes.Sort();
        }


        public static void OpenFileInNewTab(Languages language,string path)
        {
            MainWindowExtension.InitializeTabItem();

            int indexForTab;
            /* 
             * Determining which tab the file will be open
             * If open in new tab => tabItems.Count-1
             * if open in recent tab => tabControl.selected
            */
            if (tabItems[tabControl.SelectedIndex].FilePath != "" || !string.IsNullOrWhiteSpace(tabItems[tabControl.SelectedIndex].Data))
            {
                MainWindowExtension.InitializeTabItem();
                indexForTab = tabItems.Count - 1;
            }
            else
                indexForTab = tabControl.SelectedIndex;

            tabItems[indexForTab].Data = System.IO.File.ReadAllText(path); //Read File here

            //Unsubscribe TextChange event
            tabItems[indexForTab].RichTextBox.richTextBox.TextChanged -= tabItems[indexForTab].RichTextBox.richTextBox_Highlight;

            // Add content to richTextBox
            tabItems[indexForTab].Data = tabItems[indexForTab].Data;// Set Data For RTB

            //set language => auto Highlight all
            tabItems[indexForTab].RichTextBox.Language =language;

            //Resubscribe
            tabItems[indexForTab].RichTextBox.richTextBox.TextChanged += tabItems[indexForTab].RichTextBox.richTextBox_Highlight;

            //Scroll to the end of the text
            tabItems[indexForTab].RichTextBox.richTextBox.SelectionStart = tabItems[indexForTab].RichTextBox.richTextBox.Text.Length;

            tabItems[indexForTab].Header = Path.GetFileName(path);
            tabItems[indexForTab].FilePath = path;
            tabItems[indexForTab].IsSaved = true;

            //Update Status Bar
            MainWindowExtension.UpdateStatusBar(indexForTab);
        }
        /// <summary>
        /// Get Languages From The extension
        /// </summary>
        /// <param name="pathHasExtension"> Path to the file that contains Extension</param>
        public static Languages GetLanguageFromExtension(string pathHasExtension)
        {
            switch (Path.GetExtension(pathHasExtension))
            {
                case ".cs":
                    return Languages.CSharph;
                case ".cpp":
                    return Languages.CPlusPlus;
                case ".c":
                    return Languages.C;
                case ".java":
                    return Languages.Java;
                default:
                    return Languages.None;
            }
        }
        public static void ReadFile(string path)
        {

        }

        public static void MovePinnedTab(MainTabItem tabItem)
        {
            tabControl.Items.Remove(tabItem);
            tabItems.Remove(tabItem);
            tabControl.Items.Insert(0, tabItem);
            tabItems.Insert(0, tabItem);
            tabItem.IsSelected = true;
        }

    }
}
