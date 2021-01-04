using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notepad.Snippets;
namespace Notepad.Classes
{
    public class JsonDeserialize
    {
        public static CSharphSnippet CSharph { get; set; }
        public static JavaSnippet Java { get; set; }
        public static CPlusPlusSnippet CPlusPlus { get; set; }
        public static CSnippet C { get; set; }

        public static DirectoryInfo TryGetSolutionDirectoryInfo(string currentPath = null)
        {
            var directory = new DirectoryInfo(
                currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }
            return directory;
        }

        public static System.Drawing.Color GetColorFromString(string color)
        {
            return System.Drawing.ColorTranslator.FromHtml(color);
        }

        public static Languages GetLanguageFromString(string language)
        {
            if (language == "CSharph")
                return Languages.CSharph;
            else if (language == "C")
                return Languages.C;
            else if (language == "Java")
                return Languages.Java;
            else if (language == "CPlusPlus")
                return Languages.CPlusPlus;
            else return Languages.None;
        }
    }
}
