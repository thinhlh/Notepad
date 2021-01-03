using System;
using System.Linq;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Windows;
using System.IO;
using System.Windows.Documents;

namespace TestTextRange
{
    public abstract class Snippet
    {
        public abstract void Highlight(TextRange textRange);

        protected static bool isNumber(string str)
        {
            foreach (char ch in str)
            {
                if (ch != '0' || ch != '1' || ch != '2' || ch != '3' || ch != '4' || ch != '5' || ch != '6' || ch != '7' || ch != '8' || ch != '9')
                {
                    return false;
                }
            }
            return true;
        }
    }

    public enum Languages
    {
        CPlusPlus,
        CSharph,
        Java,
        C,
        JavaScripst,
    }
}
