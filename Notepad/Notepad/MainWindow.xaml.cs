using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

        #region Variable

        private List<bool> isSaved = new List<bool>();

        private List<TabItem> tabItems = new List<TabItem>(); // each Tab Item are contained here

        private List<TextBox> textBoxes = new List<TextBox>(); // textBox for each Tab

        private List<string> filePaths = new List<string>(); // FilePath for each Tab

        private int TabCount = 0;

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
                if(textBoxes[tabControl.SelectedIndex].Text.Length>0)InitTab(new TabItem());
                textBoxes[tabControl.SelectedIndex].Text = System.IO.File.ReadAllText(openFileDialog.FileName);
                tabItems[tabControl.SelectedIndex].Header = openFileDialog.FileName.ToString();

                /*Pointer to the end of paragraph*/
                textBoxes[tabControl.SelectedIndex].SelectionStart = textBoxes[tabControl.SelectedIndex].Text.Length;
                textBoxes[tabControl.SelectedIndex].SelectionLength = 0;
                
                tabItems[tabControl.SelectedIndex].Header = Path.GetFileName(openFileDialog.FileName);
                filePaths[tabControl.SelectedIndex] = openFileDialog.FileName;
                isSaved[tabControl.SelectedIndex] = true;
            }

        }

        private void OpenFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e) // For normal Save 
        {
            if (!isSaved[tabControl.SelectedIndex]) // not yet saved
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                bool fileExsisted = System.IO.File.Exists(filePaths[tabControl.SelectedIndex]);
                if (fileExsisted)
                {
                    System.IO.File.WriteAllText(filePaths[tabControl.SelectedIndex], textBoxes[tabControl.SelectedIndex].Text);
                    RemoveSavedIcon(tabControl.SelectedIndex);
                }
                else SaveAs_Executed(sender, e);
            }
        }
        private void Save_Executed(int index)//for saving tab index_th using for Save All method Only 
        {
            if (!isSaved[tabControl.SelectedIndex]) // not yet saved
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                bool fileExsisted = System.IO.File.Exists(filePaths[index]);
                if (fileExsisted)
                {
                    System.IO.File.WriteAllText(filePaths[index], textBoxes[index].Text);
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
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text files (*.txt)| *.txt | Java (*.java) | *.java | C (*.c) | *.c | C++ (*.cpp) | *.cpp | All files (*.*) | *.* ";
            //saveFileDialog.FileName = "*.txt";
            saveFileDialog.FileName = textBoxes[tabControl.SelectedIndex].Text.Substring(0);
            if (saveFileDialog.ShowDialog()==true)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName, textBoxes[tabControl.SelectedIndex].Text);
                RemoveSavedIcon(tabControl.SelectedIndex);
                tabItems[tabControl.SelectedIndex].Header = Path.GetFileName(saveFileDialog.FileName);
                filePaths[tabControl.SelectedIndex] = saveFileDialog.FileName;
            }

        }
        private void SaveAs_Executed(int index) // Save As for Save All Method Only 
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text files (*.txt)| *.txt | Java (*.java) | *.java | C (*.c) | *.c | C++ (*.cpp) | *.cpp | All files (*.*) | *.* ";
            saveFileDialog.FileName = tabItems[index].Header.ToString();
            if (saveFileDialog.ShowDialog()==true)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName, textBoxes[index].Text);
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
            String message = tabItems[tabControl.SelectedIndex].Header.ToString() + " have been modified, save changes ?";
            if (!isSaved[tabControl.SelectedIndex])
            {
                MessageBoxResult result = MessageBox.Show(message, "Yes", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    Save_Executed(sender, e);
                else if (result == MessageBoxResult.No)
                {
                    tabControl.Items.Remove(tabControl.SelectedItem);
                    // header cua tab sau va text box cua tab truoc hien thi cung mot luc!   
                }

            }
        }
        private void CloseFile_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        #endregion


        #region Additional Function
        private void RemoveSavedIcon(int index)
            /* use tabControl.SelectedIndex as an argument for normal save and save as 
             * use index in loop for save all method */
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
                //Raise * at the end and keep isSaved=false when there is a change with out save before
                AddSavedIcon();
            }

            //When init situation // is Save equal to false and header does not contains * => so add * when text changed
            if (header.Contains("*")==false&&isSaved[tabControl.SelectedIndex]==false)
                AddSavedIcon();
        }
        private void InitTab(TabItem tabItem)
        {

            //Setup for TextBox
            textBoxes.Add(new TextBox());
            textBoxes[TabCount].AcceptsReturn = true;
            textBoxes[TabCount].AcceptsTab = true;
            textBoxes[TabCount].HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBoxes[TabCount].VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBoxes[TabCount].BorderThickness = new Thickness(0);
            textBoxes[TabCount].Margin = new Thickness(0, -2, 0, 0);
            textBoxes[TabCount].FontSize = 20;
            textBoxes[TabCount].TextChanged += TextBox_TextChanged;

            //Setup for tabItem
            tabItem.Header = "Document " + (TabCount + 1); // Header Display Always larger than 1 of the tab count in tabItems
            tabItem.Name = "Document" + TabCount;
            tabItem.Content = textBoxes[TabCount];

            //Add filePaths

            filePaths.Add("");
            
            //Add to tabItems
            tabItems.Add(tabItem);

            //Add tabItem to tabControl
            tabControl.Items.Add(tabItem);
            tabItem.Focus();

            // Init is Save 
            isSaved.Add(new bool());
            TabCount++;
        }

        private void SaveAll_Click(object sender,RoutedEventArgs e)
        {
            for(int i=0;i<tabItems.Count;i++)
                if(isSaved[i]==false)
                    Save_Executed(i);
        }

    }
    #endregion
}
