using System.Collections.Generic;

namespace Notepad.Classes
{
    public class CSnippet
    {
        public List<Dictionary<string, string>> keywords { get; set; } //Keywords and theirs colors
        public string number { get; set; } //number color
        public string comment { get; set; }//comment color
        public string String { get; set; }// String color
    }
}