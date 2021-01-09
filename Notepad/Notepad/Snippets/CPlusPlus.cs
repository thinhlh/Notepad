using Notepad.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Notepad.Snippets
{
    public class CPlusPlus : ISnippet
    {
        private string pattern;

        public CPlusPlus() //Initialize and Deserialize Snippet if haven't
        {
            if (JsonDeserialize.CPlusPlus == null)
            {
                using (
                    StreamReader streamReader = File.OpenText(
                        (JsonDeserialize.TryGetSolutionDirectoryInfo().FullName
                        + @"\Notepad\resources\CPlusPlusSnippet.json").ToString()))
                {
                    var jsonString = streamReader.ReadToEnd();
                    JsonDeserialize.CPlusPlus = Newtonsoft.Json.JsonConvert.DeserializeObject<CPlusPlusSnippet>(jsonString);
                }
            }

            pattern = @"\b(" + GetPatternFromListOfKeyword(JsonDeserialize.CPlusPlus.keywords) + ")\\b";

        }

        public string GetPatternFromListOfKeyword(List<Dictionary<string, string>> keywords)
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
            RichTextBoxUC richtextBox = mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].RichTextBox;

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
            RichTextBoxUC richtextBox = mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].RichTextBox;


            richtextBox.currentCaret = richtextBox.richTextBox.SelectionStart;
            int currentLength = richtextBox.richTextBox.SelectionLength;

            richtextBox.ClearStyle(start, length);
            richtextBox.SetStyle(start, length, pattern, TokenType.keywords);
            richtextBox.SetStyle(start, length, @"\s*#\s*(define|error|import|undef|elif|if|include|using|else|ifdef|line|endif|ifndef|pragma)\s*\S*", TokenType.preprocessor);
            richtextBox.SetStyle(start, length, "\".*\"", TokenType.String);
            richtextBox.SetStyle(start, length, @"\/\/.*", TokenType.comment);
            richtextBox.SetStyle(start, length, @"\/\*.*\*\/", TokenType.comment);

            richtextBox.previousCaret = richtextBox.richTextBox.Text.Length;
            richtextBox.richTextBox.SelectionStart = richtextBox.currentCaret;
            richtextBox.richTextBox.SelectionLength = currentLength;
        }

        public bool IsNumber(string token)
        {
            return false;
            throw new NotImplementedException();
        }
    }
}
