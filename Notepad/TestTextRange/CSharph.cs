using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using System.Text.Json;
namespace TestTextRange
{
    public class CSharph : Snippet
    {

        /*
         * A list of colors that a color have a list of keywords
         * that can only be specified by that color
        */

        public override void Highlight(TextRange textRange)
        {
            //MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            //mainWindow.richTextBox.TextChanged -= mainWindow.richTextBox_TextChanged;
            //string word = textRange.Text;
            //if (word.Contains("//"))
            //    textRange.ApplyPropertyValue(Control.ForegroundProperty, JsonDesirialize.CSharph.comment);
            //else if (isNumber(word))
            //    textRange.ApplyPropertyValue(Control.ForegroundProperty, JsonDesirialize.CSharph.number);
            //else
            //    textRange.ApplyPropertyValue(Control.ForegroundProperty, JsonDesirialize.GetKeywordColor(word));
            //mainWindow.richTextBox.TextChanged += mainWindow.richTextBox_TextChanged;
        }

        public CSharph() //Initialize and Deserialize Snippet if haven't
        {
            if (JsonDesirialize.CSharph == null)
            {
                using (
                    StreamReader streamReader = File.OpenText(
                        (JsonDesirialize.TryGetSolutionDirectoryInfo().FullName 
                        + @"\Notepad\CSharphSnippet.json").ToString()))
                {
                    var jsonString = streamReader.ReadToEnd();
                    JsonDesirialize.CSharph = Newtonsoft.Json.JsonConvert.DeserializeObject<CSharphSnippet>(jsonString);
                }
            }
        }
    }
}
