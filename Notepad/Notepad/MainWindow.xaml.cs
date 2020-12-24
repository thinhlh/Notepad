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
        }

        #region Variables

        private List<MainTabItem> tabItems = new List<MainTabItem>();        

        #endregion

        #region Commands

        private void NewFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InitTab();
        }

        private void NewFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Java (*.java)|*.java|C (*.c)|*.c|C++ (*.cpp)|*.cpp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                int indexForTab;
                /* 
                 * Determining which tab the file will be open
                 * If open in new tab => tabItems.Count-1
                 * if open in recent tab => tabControl.selected
                */
                if (tabItems[tabControl.SelectedIndex].FilePath != "" || tabItems[tabControl.SelectedIndex].Data != "")
                {
                    InitTab();
                    indexForTab = tabItems.Count - 1;
                }
                else indexForTab = tabControl.SelectedIndex;
                tabItems[indexForTab].Data = System.IO.File.ReadAllText(openFileDialog.FileName);

                // Add content to richTextBox
                RichTextBox richTextBox = (RichTextBox)tabItems[indexForTab].Content;
                SetText(richTextBox, tabItems[indexForTab].Data);
                /*Pointer to the end of paragraph*/
                richTextBox.ScrollToEnd();


                tabItems[indexForTab].Header = Path.GetFileName(openFileDialog.FileName);
                tabItems[indexForTab].Content = richTextBox;
                tabItems[indexForTab].FilePath = openFileDialog.FileName;
                tabItems[indexForTab].IsSaved = true;

                //Update Status Bar
                UpdateStatusBar(indexForTab);
            }
        }

        private void OpenFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e) // For normal Save 
        {
            Save_Executed(tabControl.SelectedIndex);
        }

        private void Save_Executed(int index)//for saving tab index_th using for Save All method Only 
        {
            if (!tabItems[index].IsSaved||tabItems[index].Data=="") // not yet saved or new tab but not have data
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                bool fileExsisted = System.IO.File.Exists(tabItems[index].FilePath);
                if (fileExsisted)
                {
                    System.IO.File.WriteAllText(tabItems[index].FilePath, tabItems[index].Data);
                    RemoveSavedIcon(index);
                }
                else SaveAs_Executed(index);
            }
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e) //Normal Save As
        {
            e.CanExecute = true;
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveAs_Executed(tabControl.SelectedIndex);
        }

        private void SaveAs_Executed(int index) // Save As for Save All Method Only 
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text (*.txt)| *.txt | Java (*.java) | *.java | C (*.c) | *.c | C++ (*.cpp) | *.cpp | All files (*.*) | *.* ";
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

        private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CloseFile_Executed(tabControl.SelectedIndex);
        }

        private void CloseFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !(tabControl.SelectedIndex < 0);
        }

        private void NewTerminal_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ProcessStartInfo process = new ProcessStartInfo("cmd.exe");

            process.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            Process.Start(process);

        }

        private void NewTerminal_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewTerminalCurrentDir_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ProcessStartInfo process = new ProcessStartInfo("cmd.exe");

            // Get Path of the current Tab then set process.Working Directory to parent to open cmd at working directory    
            var path = getParentFullPath(tabControl.SelectedIndex);
            process.WorkingDirectory = Directory.GetParent(path).FullName;
            Process.Start(process);
        }

        private void NewTerminalCurrentDir_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (tabItems[tabControl.SelectedIndex].FilePath != "")
                e.CanExecute = true;
            else e.CanExecute = false;
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CloseWindow_Click(sender, (RoutedEventArgs)e);
        }

        private void Exit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Build_Executed(object sender,ExecutedRoutedEventArgs e)
        {
            string childFileNameWithExt = Path.GetFileName(tabItems[tabControl.SelectedIndex].FilePath);
            string childFileNameWithoutExt = Path.GetFileNameWithoutExtension(tabItems[tabControl.SelectedIndex].FilePath);
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd");
            Process process = new Process();


            if (tabItems[tabControl.SelectedIndex].IsSaved == false)
            {
                MessageBoxResult result = MessageBox.Show("You need to save before compile, save changes?", "Request", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Save_Executed(tabControl.SelectedIndex);
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

        private void Build_CanExecute(object sender,CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void BuildAndRun_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Build_Executed(sender, e);
            string childFileNameWithoutExtension = Path.GetFileNameWithoutExtension(tabItems[tabControl.SelectedIndex].FilePath);
            string parentPath = getParentFullPath(tabControl.SelectedIndex);
            if (
                (tabItems[tabControl.SelectedIndex].IsSaved==false)
                ||
                (System.IO.File.Exists(Path.Combine(parentPath,childFileNameWithoutExtension+".exe"))==false)
               )
                return; // must be saved and compiled sucessfully

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.WorkingDirectory = getParentFullPath(tabControl.SelectedIndex);

            process.Start();
            process.StandardInput.WriteLine(Path.GetFileNameWithoutExtension(tabItems[tabControl.SelectedIndex].FilePath));

            process.WaitForExit();
            System.IO.File.Delete(Path.Combine(parentPath, childFileNameWithoutExtension + ".exe")); //Delete .exe File

        }

        private void BuildAndRun_CanExecute(object sender,CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region Additional Functions
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitTab();
        }

        /* use tabControl.SelectedIndex as an argument for normal save and save as 
         * use index in loop for save all method */

        private void RemoveSavedIcon(int index)
        {
            tabItems[index].IsSaved = true;

            string header = tabItems[index].Header.ToString();
            tabItems[index].Header = header.Remove(header.Length - 1, 1);
        }

        private void AddSavedIcon()
        {
            tabItems[tabControl.SelectedIndex].Header += "*";
            tabItems[tabControl.SelectedIndex].IsSaved = false;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            String header = tabItems[tabControl.SelectedIndex].Header.ToString();

            if (tabItems[tabControl.SelectedIndex].IsSaved) // normal situation
            {
                //Raise* at the end and keep isSaved = false when there is a change with out save before
                AddSavedIcon();
            }

            //When init situation // is Save equal to true and header does not contains * => so add * when text changed
            else if (header.Contains("*") == false && tabItems[tabControl.SelectedIndex].IsSaved == false)
                AddSavedIcon();

            tabItems[tabControl.SelectedIndex].Data =GetText(sender as RichTextBox);
            
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
            if(e.Source is TabControl)
            {
                UpdateStatusBar(tabControl.SelectedIndex);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            for (int i = 0; i < tabControl.Items.Count; i++)
            {
                RichTextBox richTextBox = (RichTextBox)tabItems[i].Content;
                richTextBox.FontSize = e.NewValue;
                //tabItems[i].FontSize = e.NewValue;
            }
        }

        private void InitTab()
        {
            MainTabItem tabItem = new MainTabItem();

            tabItems.Add(tabItem);

            //Setup for RichTextBox of tabItem
            RichTextBox richTextBox = new RichTextBox();
            RichTextBoxSetUp(richTextBox);

            //Setup for tabItem
            int tabIndex = FindIndexForTab();
            tabItem.Header = "Document " + (tabIndex + 1); // Header Display Always larger than 1 of the number of element in tabItems
            tabItem.Name = "Document" + (tabIndex);
            tabItem.Content = richTextBox;


            //Add tabItem to tabControl
            tabControl.Items.Add(tabItem);
            tabItem.Focus();
            UpdateStatusBar(tabControl.SelectedIndex);
        }

        #region RichTextBox Setup

        private void RichTextBoxSetUp(RichTextBox richTextBox)
        {
            SetText(richTextBox, tabItems[tabItems.Count-1].Data);
            richTextBox.TextChanged += TextBox_TextChanged;
            //richTextBox.TextChanged += SyntaxHighlight.Text_Changed;
            richTextBox.SelectionChanged += TestTextRange.Selection_Changed;
            
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
                StatusText.DataContext="None";// No update when close all tab
            else if (tabItems[index].FilePath == "")
                StatusText.DataContext = "Plain Text";
            else
                StatusText.DataContext = tabItems[tabControl.SelectedIndex].FilePath;
        }

        private string getParentFullPath(int tabIndex)
        {
            return Path.Combine(tabItems[tabIndex].FilePath, "..");
        }

        #endregion

        private void ConsoleControl(object sender,RoutedEventArgs e)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
            processStartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            processStartInfo.Arguments = "/START g++";

            //Console.StartProcess(processStartInfo);
            
        }
        #endregion

        #region Click

        private void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            for (int i = tabControl.Items.Count - 1; i >= 0; i--)
                Save_Executed(i);
        }

        private void CloseFile_Executed(int index)
        {
            if (tabItems[index].IsSaved == false)
            {
                //Message then request save
                string tabHeader = (string)tabItems[index].Header;
                string message = tabHeader.Substring(0,tabHeader.Length-1) + " have been modified, save changes?";

                MessageBoxResult result = MessageBox.Show(message, "Request", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    SaveAs_Executed(index);
                else if (result == MessageBoxResult.Cancel)
                    return;
            }
            tabControl.Items.RemoveAt(index);
            
            int deletedIndexTab = Int16.Parse(tabItems[index].Name.Substring(8)); // Return the index of deleted tabItem by get subTring from name then convert to int

            tabItems.RemoveAt(index);

            //Add Index of tab then sort it for reopen new tab situation 
            closedTabIndexes.Add(deletedIndexTab);
            closedTabIndexes.Sort();
        }

        private void CloseAllFiles_Click(object sender, RoutedEventArgs e)
        {
            for (int i = tabControl.Items.Count - 1; i >= 0; i--)
                CloseFile_Executed(i);
        }

        private void NewWindow_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("Notepad.exe");
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            CloseAllFiles_Click(sender, e);
            System.Windows.Application.Current.Shutdown();
        }
        #endregion   
    }
}



