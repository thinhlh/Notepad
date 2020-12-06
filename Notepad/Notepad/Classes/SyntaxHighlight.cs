using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
namespace Notepad.Classes
{
    public class SyntaxHighlight : MainWindow
    {
        public static void SyntaHighlighting()
        {

        }
        private static void TextChanged_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }
        public static void GetLineAtCurrentCaret(System.Windows.Controls.RichTextBox richTextBox)
        {

        }

        public static void RichTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            System.Windows.Controls.RichTextBox richTextBox = (System.Windows.Controls.RichTextBox)sender;
            if (e.Key == Key.Space)
            {
                TextPointer start = richTextBox.CaretPosition;
                string text1 = start.GetTextInRun(LogicalDirection.Backward);
                TextPointer end = start.GetNextContextPosition(LogicalDirection.Backward);
                string text2 = end.GetTextInRun(LogicalDirection.Backward);

                richTextBox.Selection.Select(start, end);
                richTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, System.Drawing.Brushes.Red);
                richTextBox.Selection.Select(start, start);
                richTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, System.Drawing.Brushes.Black);
            }
        }

        public static void Coloring(TextRange textRange, SolidColorBrush color)
        {
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, color);
        }
    }
}
