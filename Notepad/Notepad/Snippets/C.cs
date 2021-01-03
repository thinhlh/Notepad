using Notepad.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.Snippets
{
    public class C : ISnippet
    {
        private string pattern;

        public C() //Initialize and Deserialize Snippet if haven't
        {
            if (JsonDeserialize.CSharph == null)
            {
                using (
                    StreamReader streamReader = File.OpenText(
                        (JsonDeserialize.TryGetSolutionDirectoryInfo().FullName
                        + @"\Notepad\CSharphSnippet.json").ToString()))
                {
                    var jsonString = streamReader.ReadToEnd();
                    JsonDeserialize.CSharph = Newtonsoft.Json.JsonConvert.DeserializeObject<CSharphSnippet>(jsonString);
                }
            }

            pattern = @"\b(" + GetListOfKeyWord(JsonDeserialize.CSharph.keywords) + ")\\b";

        }


        public string GetListOfKeyWord(List<Dictionary<string, string>> keywords)
        {
            return "";
            throw new NotImplementedException();
        }

        public void Highlight()
        {
            return;
            throw new NotImplementedException();
        }

        public void HighlightRange(int start, int end)
        {
            return;
            throw new NotImplementedException();
        }

        public bool IsNumber(string token)
        {
            return false;
            throw new NotImplementedException();
        }
    }
}
