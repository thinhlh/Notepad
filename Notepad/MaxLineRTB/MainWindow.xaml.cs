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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MaxLineRTB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static int lineNumber;
        public MainWindow()
        {
            InitializeComponent();
        }
        public void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            lineNumber = CountLineNumber(richTextBox);

            string strLine = "";
            for (int i = 1; i <= lineNumber; i++)
            {
                strLine += i.ToString() + "\n";
            }
            lineNumberTextBox.Text = strLine;
        }

        private static int CountLineNumber(RichTextBox richTextBox)
        {
            string strtext = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
            var textArr = strtext.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            lineNumber = textArr.Length - 1;
            return lineNumber;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
             
            
           
        }
    }
}
