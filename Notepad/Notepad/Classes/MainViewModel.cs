using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Notepad;
namespace Notepad.Classes
{
    class MainViewModel
    {
        private MainWindow mainWindow;
        private List<bool> isSaved = new List<bool>();

        private List<TabItem> tabItems = new List<TabItem>(); // each Tab Item is contained here

        private List<string> filePaths = new List<string>(); // FilePath for each Tab

        private List<string> fileData = new List<string>();//File Data for each File

        private TabControl tabControl;
        private StatusBar StatusBar;
        private TextBox StatusText;
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitTab(new TabItem());
        }
        public MainViewModel()
        {
            //mainWindow = Application.Current.MainWindow as MainWindow;
            //tabControl = mainWindow.tabControl;
            //StatusBar = mainWindow.StatusBar;
        }
        public MainViewModel(bool load)
        {
            mainWindow = Application.Current.MainWindow as MainWindow;
            tabControl = mainWindow.tabControl;
            StatusBar = mainWindow.StatusBar;
        }
        #region Variables



        #endregion

        #region Commands

        public void NewFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InitTab(new TabItem());
        }

        public void NewFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void OpenFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Java (*.java)|*.java|C (*.c)|*.c|C++ (*.cpp)|*.cpp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                int indexForTab;// Defining which tab the file will be open
                                // If open in new tab => tabItems.Count-1
                                // if open in recent tab => tabControl.selecte
                if (fileData[tabControl.SelectedIndex] != "")
                {
                    InitTab(new TabItem());
                    indexForTab = tabItems.Count - 1;
                }
                else indexForTab = tabControl.SelectedIndex;
                fileData[indexForTab] = System.IO.File.ReadAllText(openFileDialog.FileName);

                // Add content to richTextBox
                RichTextBox richTextBox = (RichTextBox)tabItems[indexForTab].Content;
                SetText(richTextBox, fileData[indexForTab]);
                /*Pointer to the end of paragraph*/
                richTextBox.ScrollToEnd();


                tabItems[indexForTab].Header = Path.GetFileName(openFileDialog.FileName);
                tabItems[indexForTab].Content = richTextBox;
                filePaths[indexForTab] = openFileDialog.FileName;
                isSaved[indexForTab] = true;

                //Update Status Bar
                UpdateStatusBar(indexForTab);
            }
        }

        public void OpenFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void Save_Executed(object sender, ExecutedRoutedEventArgs e) // For normal Save 
        {
            IndexSave_Executed(tabControl.SelectedIndex);
        }

        public void IndexSave_Executed(int index)//for saving tab index_th using for Save All method Only 
        {
            if (!isSaved[index]) // not yet saved
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                bool fileExsisted = System.IO.File.Exists(filePaths[index]);
                if (fileExsisted)
                {
                    System.IO.File.WriteAllText(filePaths[index], fileData[index]);
                    RemoveSavedIcon(index);
                }
                else IndexSaveAs_Executed(index);
            }
        }

        public void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e) //Normal Save As
        {
            e.CanExecute = true;
        }

        public void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IndexSaveAs_Executed(tabControl.SelectedIndex);
        }

        public void IndexSaveAs_Executed(int index) // Save As for Save All Method Only 
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text (*.txt)| *.txt | Java (*.java) | *.java | C (*.c) | *.c | C++ (*.cpp) | *.cpp | All files (*.*) | *.* ";
            string tabHeader = (string)tabItems[tabControl.SelectedIndex].Header;
            saveFileDialog.FileName = tabHeader.Substring(0, tabHeader.Length - 1); // remove the * flag
            if (saveFileDialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName, fileData[index]);
                RemoveSavedIcon(index);
                tabItems[index].Header = Path.GetFileName(saveFileDialog.FileName);
                filePaths[index] = saveFileDialog.FileName;

                //Update Status Bar
                UpdateStatusBar(index);
            }
        }

        public void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void CloseFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IndexCloseFile_Executed(tabControl.SelectedIndex);
        }

        public void CloseFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !(tabControl.SelectedIndex < 0);
        }

        public void NewTerminal_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ProcessStartInfo process = new ProcessStartInfo("cmd.exe");

            process.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            Process.Start(process);

        }

        public void NewTerminal_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void NewTerminalCurrentDir_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ProcessStartInfo process = new ProcessStartInfo("cmd.exe");

            // Get Path of the current Tab then set process.Working Directory to parent to open cmd at working directory    
            var path = getParentFullPath(tabControl.SelectedIndex);
            process.WorkingDirectory = Directory.GetParent(path).FullName;
            Process.Start(process);
        }

        public void NewTerminalCurrentDir_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (filePaths[tabControl.SelectedIndex] != "")
                e.CanExecute = true;
            else e.CanExecute = false;
        }

        public void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CloseWindow_Click(sender, (RoutedEventArgs)e);
        }

        public void Exit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void Build_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string childFileNameWithExt = Path.GetFileName(filePaths[tabControl.SelectedIndex]);
            string childFileNameWithoutExt = Path.GetFileNameWithoutExtension(filePaths[tabControl.SelectedIndex]);
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd");
            Process process = new Process();


            if (isSaved[tabControl.SelectedIndex] == false)
            {
                MessageBoxResult result = MessageBox.Show("You need to save before compile, save changes?", "Request", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    IndexSave_Executed(tabControl.SelectedIndex);
                }
                else return;
            }

            // We can not use normal argument to write in cmd we have to use redirect Standard Input the write in cmd 

            startInfo.UseShellExecute = false; // For redirect Input
            startInfo.WorkingDirectory = getParentFullPath(tabControl.SelectedIndex);
            startInfo.RedirectStandardInput = true;// Allow to write later
            process.StartInfo = startInfo;
            process.Start();

            process.StandardInput.WriteLine("g++ " + childFileNameWithExt + " -o " + childFileNameWithoutExt);
            process.StandardInput.Flush();
            process.WaitForExit();

        }

        public void Build_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void BuildAndRun_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Build_Executed(sender, e);
            string childFileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePaths[tabControl.SelectedIndex]);
            string parentPath = getParentFullPath(tabControl.SelectedIndex);
            if (
                (isSaved[tabControl.SelectedIndex] == false)
                ||
                (System.IO.File.Exists(Path.Combine(parentPath, childFileNameWithoutExtension + ".exe")) == false)
               )
                return; // must be saved and compiled sucessfully

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.WorkingDirectory = getParentFullPath(tabControl.SelectedIndex);

            process.Start();
            process.StandardInput.WriteLine(Path.GetFileNameWithoutExtension(filePaths[tabControl.SelectedIndex]));

            process.WaitForExit();
            System.IO.File.Delete(Path.Combine(parentPath, childFileNameWithoutExtension + ".exe")); //Delete .exe File

        }

        public void BuildAndRun_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region Additional Functions

        /* use tabControl.SelectedIndex as an argument for normal save and save as 
         * use index in loop for save all method */

        private void RemoveSavedIcon(int index)
        {
            isSaved[index] = true;

            string header = tabItems[index].Header.ToString();
            tabItems[index].Header = header.Remove(header.Length - 1, 1);
        }

        private void AddSavedIcon()
        {
            tabItems[tabControl.SelectedIndex].Header += "*";
            isSaved[tabControl.SelectedIndex] = false;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            String header = tabItems[tabControl.SelectedIndex].Header.ToString();

            if (isSaved[tabControl.SelectedIndex]) // normal situation
            {
                //Raise* at the end and keep isSaved = false when there is a change with out save before
                AddSavedIcon();
            }

            //When init situation // is Save equal to false and header does not contains * => so add * when text changed
            else if (header.Contains("*") == false && isSaved[tabControl.SelectedIndex] == false)
                AddSavedIcon();

            RichTextBox richTextBox = (RichTextBox)sender;
            fileData[tabControl.SelectedIndex] = GetText(richTextBox);

        }

        private List<int> closedTabIndexes = new List<int>();// this List holds indexs of tabs that was removed from tabControl

        private int FindIndexForTab() // this function define which tabIndex is approriate for InitTab 
        {
            if (closedTabIndexes.Count == 0)
                return tabControl.Items.Count;
            else
            {
                int index = closedTabIndexes[0];
                closedTabIndexes.RemoveAt(0);
                return index;
            }

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                UpdateStatusBar(tabControl.SelectedIndex);
            }
        }

        public void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            for (int i = 0; i < tabControl.Items.Count; i++)
            {
                RichTextBox richTextBox = (RichTextBox)tabItems[i].Content;
                richTextBox.FontSize = e.NewValue;
                //tabItems[i].FontSize = e.NewValue;
            }
        }

        private void InitTab(TabItem tabItem)
        {
            //Add filePaths and fileData

            filePaths.Add("");
            fileData.Add("");

            //Setup for RichTextBox of tabItem
            RichTextBox richTextBox = new RichTextBox();
            RichTextBoxSetUp(richTextBox);

            //Setup for tabItem
            int tabIndex = FindIndexForTab();
            tabItem.Header = "Document " + (tabIndex + 1); // Header Display Always larger than 1 of the number of element in tabItems
            tabItem.Name = "Document" + (tabIndex);
            tabItem.Content = richTextBox;

            //Add to tabItems
            tabItems.Add(tabItem);

            //Add tabItem to tabControl
            tabControl.Items.Add(tabItem);
            tabItem.Focus();

            // Init isSave 
            isSaved.Add(true);

            tabControl.SelectionChanged += TabControl_SelectionChanged;
        }

        #region RichTextBox Setup

        private void RichTextBoxSetUp(RichTextBox richTextBox)
        {
            SetText(richTextBox, fileData[tabItems.Count]);
            richTextBox.AcceptsReturn = true;
            richTextBox.AcceptsTab = true;
            richTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            richTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            richTextBox.BorderThickness = new Thickness(0);
            richTextBox.Margin = new Thickness(0, -2, 0, 0);
            richTextBox.FontSize = 18;
            richTextBox.TextChanged += TextBox_TextChanged;

            //richTextBox.PreviewKeyDown += SyntaxHighlighting.RichTextBox_PreviewKeyDown;
        }

        private void SetText(RichTextBox richTextBox, string text)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
        }

        private string GetText(RichTextBox richTextBox)
        {
            return new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
        }

        private void UpdateStatusBar(int index)
        {
            if (index == -1)
                StatusText.DataContext = "None";// No update when close all tab
            else if (filePaths[index] == "")
                StatusText.DataContext = "Plain Text";
            else
                StatusText.DataContext = filePaths[tabControl.SelectedIndex];
        }

        private string getParentFullPath(int tabIndex)
        {
            return Path.Combine(filePaths[tabIndex], "..");
        }

        #endregion

        private void ConsoleControl(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
            processStartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            processStartInfo.Arguments = "/START g++";

            //Console.StartProcess(processStartInfo);

        }

        #endregion

        #region Click

        public void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            for (int i = tabControl.Items.Count - 1; i >= 0; i--)
                IndexSave_Executed(i);
        }

        private void IndexCloseFile_Executed(int index)
        {
            if (isSaved[index] == false)
            {
                if (fileData[index] != "")
                {
                    //Message then request save
                    string tabHeader = (string)tabItems[index].Header;
                    string message = tabHeader.Substring(0, tabHeader.Length - 1) + " have been modified, save changes?";

                    MessageBoxResult result = MessageBox.Show(message, "Request", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                        IndexSaveAs_Executed(index);
                    else if (result == MessageBoxResult.Cancel)
                        return;
                }
            }
            tabControl.Items.RemoveAt(index);

            int deletedIndexTab = Int16.Parse(tabItems[index].Name.Substring(8)); // Return the index of deleted tabItem by get subTring from name then convert to int

            tabItems.RemoveAt(index);
            fileData.RemoveAt(index);
            filePaths.RemoveAt(index);
            isSaved.RemoveAt(index);

            //Add Index of tab then sort it for reopen new tab situation 
            closedTabIndexes.Add(deletedIndexTab);
            closedTabIndexes.Sort();
        }

        public void CloseAllFiles_Click(object sender, RoutedEventArgs e)
        {
            for (int i = tabControl.Items.Count - 1; i >= 0; i--)
                IndexCloseFile_Executed(i);
        }

        public void NewWindow_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("Notepad.exe");
        }

        public void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            CloseAllFiles_Click(sender, e);
            System.Windows.Application.Current.Shutdown();
        }

        public void Test(object sender, RoutedEventArgs e)
        {

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c g++";
            process.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            process.Start();
            process.WaitForExit();

        }

        #endregion
    }
}
