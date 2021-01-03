using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.Snippets
{
    public class CSharphSnippet
    {
        public List<Dictionary<string, string>> keywords { get; set; } //Keywords and theirs colors
        public string number { get; set; } //number color
        public string comment { get; set; }//comment color

        public string preprocessor { get; set; } //  preprocessor color
        public string String { get; set; }// String color
    }
}
