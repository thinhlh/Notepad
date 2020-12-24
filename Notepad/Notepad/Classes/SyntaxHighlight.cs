using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Notepad.Classes
{
    public class SyntaxHighlight
    {
        public enum Language
        {
            CSharph,
            CPlusPlus,
            Java,
            HTML
        }
        private TextRange currentWord(RichTextBox richTextBox)
        {
            return new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
        }
        public static void Text_Changed(object sender, TextChangedEventArgs e)
        {
            RichTextBox richtextBox = sender as RichTextBox;

            //TextRange matched = getWordFromPosition(richtextBox.Document.ContentStart, "using");
            //if (matched != null) matched.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);

        }
    }
}
