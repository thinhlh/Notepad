using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Notepad.Classes
{
    public class TestTextRange // this class is used to find the range of the current word at current caret 
    {
        private static TextPointer current;
        public static string word="ok";
        public static void Selection_Changed(object sender, RoutedEventArgs e)
        {
            RichTextBox richTextBox = sender as RichTextBox;
            current = richTextBox.CaretPosition;
            getCurrentWordRange(richTextBox);
        }
        private static TextRange getCurrentWordRange(RichTextBox richTextBox)
        {
            current = richTextBox.CaretPosition; 
            
            
            TextPointer start = current, end;


            while(
                start.GetNextContextPosition(LogicalDirection.Backward) != null
                &&
                start.GetPointerContext(LogicalDirection.Backward)==TextPointerContext.Text
                && 
                !(start.GetTextInRun(LogicalDirection.Backward).Contains(' ')|| start.GetTextInRun(LogicalDirection.Backward).Contains('\t')))
            
            {
                start = start.GetNextContextPosition(LogicalDirection.Backward);
            }


            end = start;


            while (
                end.GetNextContextPosition(LogicalDirection.Forward) != null
                &&
                end.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text 
                &&
                !(end.GetTextInRun(LogicalDirection.Backward).Contains(' ') || end.GetTextInRun(LogicalDirection.Backward).Contains('\t')))
                
            {
                word = end.GetTextInRun(LogicalDirection.Forward);
                end = end.GetNextContextPosition(LogicalDirection.Forward);
            }

            return new TextRange(start,end);
        }
    }
}
