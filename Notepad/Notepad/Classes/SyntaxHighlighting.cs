using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Notepad.Classes
{
    public class SyntaxHighlighting : MainWindow
    {
        public static void SyntaHighlighting()
        {

        }
        private static void TextChanged_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
        public static void GetLineAtCurrentCaret(RichTextBox richTextBox)
        {

        }

        public static void RichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            RichTextBox richTextBox = (RichTextBox)sender;
            if (e.Key == Key.Space||e.Key==Key.Tab||e.Key==Key.Enter)
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

        public static void Coloring(TextRange textRange, Brushes color)
        {
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
        }
    }
}