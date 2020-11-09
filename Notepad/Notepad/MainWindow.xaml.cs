using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

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

            InitTab(new TabItem());
        }

        #region Variables

        private List<bool> isSaved = new List<bool>();

        private List<TabItem> tabItems = new List<TabItem>(); // each Tab Item is contained here

        private List<string> filePaths = new List<string>(); // FilePath for each Tab

        private List<string> fileData = new List<string>();//File Data for each File

        #endregion

        #region Command

        private void NewFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InitTab(new TabItem());
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
                int indexForTab;// Defining which tab the file will be open
                // If open in new tab => tabItems.Count-1
                // if open in recent tab => tabControl.selecte
                if (fileData[tabControl.SelectedIndex]!="")
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
            if (!isSaved[index]) // not yet saved
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                bool fileExsisted = System.IO.File.Exists(filePaths[index]);
                if (fileExsisted)
                {
                    System.IO.File.WriteAllText(filePaths[index], fileData[index]);
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
            saveFileDialog.FileName = tabItems[tabControl.SelectedIndex].Header.ToString();
            if (saveFileDialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName, fileData[index]);
                RemoveSavedIcon(index);
                tabItems[index].Header = Path.GetFileName(saveFileDialog.FileName);
                filePaths[index] = saveFileDialog.FileName;
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
            e.CanExecute = true;
        }

        private void NewTerminal_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ProcessStartInfo process = new ProcessStartInfo();
            process.FileName = @"C:\WINDOWS\system32\cmd.exe";
            process.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            Process.Start(process);
        }

        private void NewTerminal_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewTerminalCurrentDir_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ProcessStartInfo process = new ProcessStartInfo();
            process.FileName = "cmd.exe";

            // Get Path of the current Tab then set process.Working Directory to parent to open cmd at working directory    
            var path = Path.GetFullPath(filePaths[tabControl.SelectedIndex]);
            process.WorkingDirectory = Directory.GetParent(path).FullName;

            Process.Start(process);
        }

        private void NewTerminalCurrentDir_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (filePaths[tabControl.SelectedIndex] != "")
                e.CanExecute = true;
            else e.CanExecute = false;
        }

        private void Exit_Executed(object sender,ExecutedRoutedEventArgs e)
        {
            CloseWindow_Click(sender, (RoutedEventArgs)e);
        }

        private void Exit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        #endregion

        #region Additional Function

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
            richTextBox.FontSize = 16;
            richTextBox.TextChanged += TextBox_TextChanged;
            richTextBox.PreviewKeyDown += SyntaxHighlighting.RichTextBox_PreviewKeyDown;
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

        #endregion

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
            isSaved.Add(new bool());
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
            if (isSaved[index] == false)
            {
                RichTextBox richTextBox = (RichTextBox)tabItems[index].Content;
                if (fileData[index]!="")
                {
                    //Message then request save
                    string message = "This document have been modified, save changes?";
                    MessageBoxResult result = MessageBox.Show(message, "Request", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                        SaveAs_Executed(index);
                }
            }
            tabControl.Items.RemoveAt(index);

            // File have data and saved

            if (tabControl.SelectedIndex >= 0) // In case the recent deleted tab was the last tab => selected index=-1
            {

                int deletedIndexTab = Int16.Parse(tabItems[index].Name.Substring(8)); // Return the index of deleted tabItem by get subTring from name then convert to int
                tabItems.RemoveAt(index);
                fileData.RemoveAt(index);
                filePaths.RemoveAt(index);
                isSaved.RemoveAt(index);

                //Add Index of tab then sort it for reopen new tab situation 
                closedTabIndexes.Add(deletedIndexTab);
                closedTabIndexes.Sort();
            }
        }


        private void CloseAllFiles_Click(object sender, RoutedEventArgs e)
        {
            for (int i = tabControl.Items.Count - 1; i >= 0; i--)
                CloseFile_Executed(i);
        }
        
        private void NewWindow_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            CloseAllFiles_Click(sender, e);
            System.Windows.Application.Current.Shutdown();
        }
        private void btn_click(object sender,RoutedEventArgs e)
        {
            SyntaxHighlighting.GetLineAtCurrentCaret((RichTextBox)tabItems[tabControl.SelectedIndex].Content);
        }
        #endregion
    }

    #region CustomCommands
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
        
    }

    #endregion

    public class SyntaxHighlighting : MainWindow
    {
        public static void SyntaHighlighting()
        {
            
        }
        private static void TextChanged_PreviewKeyDown(object sender,KeyEventArgs e)
        {
            
        }
        public static void GetLineAtCurrentCaret(RichTextBox richTextBox)
        {
            
        }

        public static void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            RichTextBox richTextBox = (RichTextBox)sender;
            if (e.Key == Key.Space)
            {
                TextPointer start = richTextBox.CaretPosition;
                string text1 = start.GetTextInRun(LogicalDirection.Backward);
                TextPointer end = start.GetNextContextPosition(LogicalDirection.Backward);
                string text2 = end.GetTextInRun(LogicalDirection.Backward);

                richTextBox.Selection.Select(start, end);
                richTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                richTextBox.Selection.Select(start, start);
                richTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
            }
        }

        public static void IRegex(string line)
        {
            Regex symbols = new Regex(@"([\t\n(){}:;])<>");
            String[] words = symbols.Split(line); // list of words 
           
        }

        public static void Coloring(TextRange textRange,SolidColorBrush color)
        {
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, color);
        }
    }
}

