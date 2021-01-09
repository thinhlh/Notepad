using Notepad.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Notepad
{
    /// <summary>
    /// Interaction logic for Goto.xaml
    /// </summary>
    public partial class GotoWindow : Window
    {
        public GotoWindow()
        {
            InitializeComponent();
        }

        private void LineTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.D0|| e.Key == Key.D1|| e.Key == Key.D2|| e.Key == Key.D3|| e.Key == Key.D4|| e.Key == Key.D5|| e.Key == Key.D6|| e.Key == Key.D7|| e.Key == Key.D8|| e.Key == Key.D9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (Application.Current.MainWindow as MainWindow);
            int line = Int16.Parse(LineTextBox.Text);
            if (mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].RichTextBox.richTextBox.Lines.Length < line || line <= 0)
                MessageBox.Show("Index is out of range");
            else
                mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].RichTextBox.richTextBox.SelectionStart=mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].RichTextBox.richTextBox.GetFirstCharIndexFromLine(line-1);
            mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].RichTextBox.richTextBox.Focus();
            this.Close();
        }
    }
}
