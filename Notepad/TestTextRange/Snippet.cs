using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TestTextRange
{
    public class Snippet
    {
        private readonly Dictionary<string, SolidColorBrush> formatList=new Dictionary<string, SolidColorBrush>();
        public Dictionary<string,SolidColorBrush> FormatList
        {
            get => formatList;
        }
        public Snippet()
        {
            formatList.Add("#include", Brushes.Red);
            formatList.Add("using", Brushes.Red);
            formatList.Add("namespace", Brushes.Red);
            formatList.Add("for", Brushes.Red);
            formatList.Add("return", Brushes.Red);
            formatList.Add("while", Brushes.Red);


            formatList.Add("main", Brushes.Green);

            formatList.Add("private", Brushes.Blue);
            formatList.Add("protected", Brushes.Blue);
            formatList.Add("int", Brushes.Blue);
            formatList.Add("string", Brushes.Blue);
            formatList.Add("virtual", Brushes.Blue);
            formatList.Add("void", Brushes.Blue);
            formatList.Add("override", Brushes.Blue);
        }
        public int length()
        {
            return formatList.Count;
        }
    }
}
