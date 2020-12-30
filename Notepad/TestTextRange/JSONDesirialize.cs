using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace TestTextRange
{
    public class JsonDesirialize
    {
        public static CSharphSnippet CSharph { get; set; }

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

        public static SolidColorBrush GetKeywordColor(string word)
        {
            string value=null;
            CSharph.keywords.Find(x => x.TryGetValue(word, out value));
            return GetColorFromString(value);
        }
        private static SolidColorBrush GetColorFromString(string color)
        {
            if (color is null) return Brushes.Black;
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }
    }
}
