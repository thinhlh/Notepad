using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notepad.Classes;
using Notepad;
using System.Windows;
namespace Notepad.Snippets
{
    public class CSharph : ISnippet
    {
        private string pattern;

        public CSharph() //Initialize and Deserialize Snippet if haven't
        {
            if (JsonDeserialize.CSharph == null)
            {
                using (
                    StreamReader streamReader = File.OpenText(
                        (JsonDeserialize.TryGetSolutionDirectoryInfo().FullName
                        + @"\Notepad\resources\CSharphSnippet.json").ToString()))
                {
                    var jsonString = streamReader.ReadToEnd();
                    JsonDeserialize.CSharph = Newtonsoft.Json.JsonConvert.DeserializeObject<CSharphSnippet>(jsonString);
                }
            }

            pattern = @"\b(" + GetListOfKeyWord(JsonDeserialize.CSharph.keywords) + ")\\b";

        }

        public string GetListOfKeyWord(List<Dictionary<string, string>> keywords)
        {
            string regex = "";
            foreach (Dictionary<string, string> keyword in keywords)
            {
                var key = keyword.Keys.ToList();
                regex += "|" + key[0];
            }
            return regex.Substring(1);
        }

        public void Highlight()
        {
            
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            RichTextBoxUC richtextBox=(mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl;

            richtextBox.currentCaret = richtextBox.richTextBox.SelectionStart;
            int length = richtextBox.richTextBox.SelectionLength;

            

            /*
             * For every Language, the below code block are different 
             */
            richtextBox.ClearStyle();
            richtextBox.SetStyle(pattern, TokenType.keywords);
            richtextBox.SetStyle(@"\s*#\s*(define|error|import|undef|elif|if|include|using|else|ifdef|line|endif|ifndef|pragma)\s*\S*", TokenType.preprocessor);
            richtextBox.SetStyle("\".*\"", TokenType.String);
            richtextBox.SetStyle(@"\/\/.*", TokenType.comment);
            richtextBox.SetStyle(@"\/\*.*\*\/", TokenType.comment);

            //Return normal
            richtextBox.previousCaret = richtextBox.richTextBox.SelectionStart;
            richtextBox.richTextBox.SelectionStart = richtextBox.currentCaret;
            richtextBox.richTextBox.SelectionLength = length;
        }

        public void HighlightRange(int start, int length)
        {

            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            RichTextBoxUC richtextBox = (mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl;


            richtextBox.currentCaret = richtextBox.richTextBox.SelectionStart;
            int currentLength = richtextBox.richTextBox.SelectionLength;

            richtextBox.ClearStyle(start,length);
            richtextBox.SetStyle(start, length, pattern, TokenType.keywords);
            richtextBox.SetStyle(start, length, @"\s*#\s*(define|error|import|undef|elif|if|include|using|else|ifdef|line|endif|ifndef|pragma)\s*\S*", TokenType.preprocessor);
            richtextBox.SetStyle(start, length, "\".*\"", TokenType.String);
            richtextBox.SetStyle(start, length, @"\/\/.*", TokenType.comment);
            richtextBox.SetStyle(start, length, @"\/\*.*\*\/", TokenType.comment);

            richtextBox.previousCaret = richtextBox.richTextBox.Text.Length;
            richtextBox.richTextBox.SelectionStart = richtextBox.currentCaret;
            richtextBox.richTextBox.SelectionLength = currentLength;

        }


        #region Implements Interface

        public bool IsNumber(string token)
        {
            foreach (char ch in token)
            {
                if (ch != '0' || ch != '1' || ch != '2' || ch != '3' || ch != '4' || ch != '5' || ch != '6' || ch != '7' || ch != '8' || ch != '9')
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
