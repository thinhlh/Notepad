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
        private int TabCount = 0;
        public MainWindow()
        {
            
            InitializeComponent();

            InitTabItem();
        }
        private void StylingTab(TabItem tabItem,TextBox txBox)
        {
            
            tabItem.Header = "Document " + ++TabCount;
            tabItem.Name = "Document" + TabCount;
            tabItem.Content = txBox;

            txBox.BorderThickness = new Thickness(0);
            txBox.Margin = new Thickness(0, -2, 0, 0);
            txBox.FontSize = 20;

            tabControl.Items.Add(tabItem);
            tabItem.Focus();

        }
        public void InitTabItem()
        {
            StylingTab(new TabItem(), new TextBox());   
        }


        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            StylingTab(new TabItem(), new TextBox());
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {

            TextBox txBox = new TextBox();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Java (*.java)|*.java|C (*.c)|*.c|C++ (*.cpp)|*.cpp|All files (*.*)|*.*";
            
            if (openFileDialog.ShowDialog() == true)
            {
               
                txBox.Text = System.IO.File.ReadAllText(openFileDialog.FileName);
            }
            StylingTab(new TabItem(), txBox);
            txBox.CaretIndex = txBox.Text.Length;
        }
        
        private void Save_Click(object sender,RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter= "Text files(*.txt)| *.txt | Java(*.java) | *.java | C(*.c) | *.c | C++(*.cpp) | *.cpp | All files(*.*) | *.* ";
            var tabItem = (TabItem)tabControl.SelectedItem;
            var txtBox = (TextBox)tabItem.Content;
            if (saveFileDialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(saveFileDialog.FileName, txtBox.Text);  
            }
            
        }
    }
    
}
