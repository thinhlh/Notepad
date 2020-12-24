using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using Notepad;
namespace Notepad.Classes
{
    public class LineNumber
    {
        private static MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        private static int lineNumber = 1;

        public static void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RichTextBox richTextBox = sender as RichTextBox;
            lineNumber=CountLineNumber(richTextBox);
            TextBox lineNumberTextBox=(mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].Content as Grid).Children[0] as TextBox;

            string strLine="";
            for(int i=1;i<=lineNumber;i++)
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
    }
}
