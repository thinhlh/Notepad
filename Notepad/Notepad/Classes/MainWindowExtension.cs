using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Notepad;
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

            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.TextChanged += (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox_TextChangedSavedIcon;
            //Resubscribe when this tab is Saved
        }

        private static void RemoveSavedIcon(int index)
        {
            tabItems[index].IsSaved = true;

            string header = tabItems[index].Header.ToString();
            tabItems[index].Header = header.Remove(header.Length - 1, 1);
            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.TextChanged -= (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox_TextChangedSavedIcon;
        }

        public static string GetParentFullPath(int index)
        {
            return Path.Combine(tabItems[index].FilePath, "..");
        }

        public static DirectoryInfo TryGetSolutionDirectoryInfo(string currentPath = null)
        {
            var directory = new DirectoryInfo(
                currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null &&!directory.GetFiles("*.sln").Any())
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

        public static void Find(string token)
        {
            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.Find(token);
            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.Focus();
        }

        public static void LoadFile(int tabIndex,string path)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(path);
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
            System.IO.BufferedStream bs = new System.IO.BufferedStream(fs);
            int bufferSize = 40000000;      //Set the buffer size to 10MB.
            Byte[] buffer = new byte[bufferSize];
            int readLength;
            do
            {
                readLength = bs.Read(buffer, 0, bufferSize);
                (tabItems[tabIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.Text += System.Text.Encoding.ASCII.GetString(buffer);
            } while (readLength == bufferSize);
        }

        private static void AppendText(System.Windows.Forms.RichTextBox richTextBox,string line)
        {
            if (richTextBox.InvokeRequired)
            {
                richTextBox.Invoke((ThreadStart)(() => AppendText(richTextBox,line)));
            }
            else
            {
                richTextBox.AppendText(line + Environment.NewLine);
            }
        }
    }
}
