using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Notepad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variable

        private List<bool> isSaved=new List<bool>();

        private List<TabItem> tabItems = new List<TabItem>();

        private List<TextBox> textBoxes = new List<TextBox>();

        private int TabCount = 0;

        #endregion

        #region Command

        private void NewFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InitTab(new TabItem());
        }
        private void NewFile_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InitTab(new TabItem());
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Java (*.java)|*.java|C (*.c)|*.c|C++ (*.cpp)|*.cpp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                textBoxes[TabCount-1].Text = System.IO.File.ReadAllText(openFileDialog.FileName); // richText=All text
                tabItems[TabCount - 1].Header = openFileDialog.FileName.ToString();
            }

            /* Pointer to the end of the paragraph*/
            textBoxes[TabCount-1].ScrollToEnd();
        }

        private void OpenFile_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            var tabItem = (TabItem)tabControl.SelectedItem;
            var txtBox = (TextBox)tabItem.Content;

            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text files (*.txt)| *.txt | Java (*.java) | *.java | C (*.c) | *.c | C++ (*.cpp) | *.cpp | All files (*.*) | *.* ";
            saveFileDialog.FileName = "*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName, txtBox.Text);
            }
        }

        private void SaveAs_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void Exit_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        #endregion

        public MainWindow()
        {
            
            InitializeComponent();

            InitTab(new TabItem());
        }

        void InitTab(TabItem tabItem)
        {

            textBoxes.Add(new TextBox());
            textBoxes[TabCount].AcceptsReturn = true;
            textBoxes[TabCount].AcceptsTab = true;
            textBoxes[TabCount].HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBoxes[TabCount].VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBoxes[TabCount].BorderThickness = new Thickness(0);
            textBoxes[TabCount].Margin = new Thickness(0, -2, 0, 0);
            textBoxes[TabCount].FontSize = 20;

            tabItem.Header = "*Document " + (TabCount + 1); // Header Display Always larger than 1 of the tab count in tabItems
            tabItem.Name = "Document" + TabCount;
            tabItem.Content = textBoxes[TabCount];

            tabItems.Add(tabItem);

            tabControl.Items.Add(tabItem);
            tabItem.Focus();
            TabCount++;
        }
    }
    
}
