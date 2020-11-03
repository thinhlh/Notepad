using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
                if (fileData[tabControl.SelectedIndex].Length > 0)
                {
                    InitTab(new TabItem());
                    indexForTab = tabItems.Count - 1;
                }
                else indexForTab = tabControl.SelectedIndex;
                fileData[indexForTab] = System.IO.File.ReadAllText(openFileDialog.FileName);

                /*Pointer to the end of paragraph*/
                TextBox textBox = (TextBox)tabItems[indexForTab].Content;
                textBox.SelectionStart = textBox.Text.Length;
                textBox.SelectionLength = 0;
                textBox.Text = fileData[indexForTab];

                tabItems[indexForTab].Header = Path.GetFileName(openFileDialog.FileName);
                tabItems[indexForTab].Content = textBox;
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
            saveFileDialog.Filter = "Text files (*.txt)| *.txt | Java (*.java) | *.java | C (*.c) | *.c | C++ (*.cpp) | *.cpp | All files (*.*) | *.* ";
            saveFileDialog.FileName = tabItems[index].Header.ToString();
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
            CloseFile_Executed();
        }

        private void CloseFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
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

            TextBox textBox = (TextBox)sender;
            fileData[tabControl.SelectedIndex] = textBox.Text;
        }

        private List<int> closedTabIndexes = new List<int>();// this List holds indexs of tabs that was removed from tabControl

        private int FindIndexForTab()
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

        private void InitTab(TabItem tabItem)
        {
            //Add filePaths and fileData

            filePaths.Add("");
            fileData.Add("");

            //Setup for TextBox of  tabItem
            TextBox textBox = new TextBox();
            tabItem.Content = textBox;

            textBox.Text = fileData[tabItems.Count];
            textBox.AcceptsReturn = true;
            textBox.AcceptsTab = true;
            textBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.BorderThickness = new Thickness(0);
            textBox.Margin = new Thickness(0, -2, 0, 0);
            textBox.FontSize = 20;
            textBox.TextChanged += TextBox_TextChanged;

            //Setup for tabItem
            int tabIndex = FindIndexForTab();
            tabItem.Header = "Document " + (tabIndex + 1); // Header Display Always larger than 1 of the number of element in tabItems
            tabItem.Name = "Document" + (tabIndex);
            tabItem.Content = textBox;

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

        private void CloseFile_Executed()
        {
            if (isSaved[tabControl.SelectedIndex] == false)
            {
                //Message then request save
                string message = "This document have been modified, save changes?";
                MessageBoxResult result = MessageBox.Show(message, "Yes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    SaveAs_Executed(tabControl.SelectedIndex);
                else if (result == MessageBoxResult.Cancel)
                    return;
            }
            tabControl.Items.RemoveAt(tabControl.SelectedIndex);

            // File have data and saved

            if (tabControl.SelectedIndex >= 0) // In case the recent deleted tab was the last tab => selected index=-1
            {

                int deletedIndexTab = Int16.Parse(tabItems[tabControl.SelectedIndex].Name.Substring(8)); // Return the index of deleted tabItem by get subTring from name then convert to int
                tabItems.RemoveAt(tabControl.SelectedIndex);
                fileData.RemoveAt(tabControl.SelectedIndex);
                filePaths.RemoveAt(tabControl.SelectedIndex);
                isSaved.RemoveAt(tabControl.SelectedIndex);

                closedTabIndexes.Add(deletedIndexTab);
            }
        }

        private void CloseAll_Click(object sender, RoutedEventArgs e)
        {
            for (int i = tabControl.Items.Count - 1; i >= 0; i--)
                CloseFile_Executed();
        }

        private void NewWindow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            //CloseAll_Click(sender,e);
            //System.Windows.Application.Current.Shutdown();
        }

        #endregion
    }
}

