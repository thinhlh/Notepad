using Notepad.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Notepad.Snippets
{
    public class PlainText : ISnippet
    {
        public string GetListOfKeyWord(List<Dictionary<string, string>> keywords)
        {
            return null;
            throw new NotImplementedException();
        }

        public void Highlight()
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            RichTextBoxUC richtextBox = (mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl;

            richtextBox.currentCaret = richtextBox.richTextBox.SelectionStart;
            int length = richtextBox.richTextBox.SelectionLength;

            richtextBox.ClearStyle();

            richtextBox.previousCaret = richtextBox.richTextBox.SelectionStart;
            richtextBox.richTextBox.SelectionStart = richtextBox.currentCaret;
            richtextBox.richTextBox.SelectionLength = length;

        }

        public void HighlightRange(int start, int end)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            RichTextBoxUC richtextBox = (mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl;

            richtextBox.currentCaret = richtextBox.richTextBox.SelectionStart;
            int length = richtextBox.richTextBox.SelectionLength;

            richtextBox.ClearStyle(0, richtextBox.richTextBox.Text.Length);

            richtextBox.previousCaret = richtextBox.richTextBox.SelectionStart;
            richtextBox.richTextBox.SelectionStart = richtextBox.currentCaret;
            richtextBox.richTextBox.SelectionLength = length;
        }

        public bool IsNumber(string token)
        {
            return false;
            throw new NotImplementedException();
        }
    }
}
