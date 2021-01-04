using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.Snippets
{
    public interface ISnippet
    {
        void Highlight();

        void HighlightRange(int start,int length);
        
        string GetListOfKeyWord(List<Dictionary<string, string>> keywords);

        bool IsNumber(string token);
        


    }

    public enum Languages
    {
        CSharph,
        Java,
        CPlusPlus,
        C,
        None
    }

    public enum TokenType
    {
        keywords,
        comment,
        String,
        preprocessor
    }
}
