using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Web;
namespace TestTextRange
{
    public class Snippet
    {
        private readonly Dictionary<string, SolidColorBrush> formatList=new Dictionary<string, SolidColorBrush>();
        public Dictionary<string,SolidColorBrush> FormatList
        {
            get => formatList;
        }
        public enum Languages
        {
            CPlusPlus,
            CSharph,
            Java,
            C,
            JavaScripst,    
        }

        public Snippet(Languages language)
        {
            string json = @"{ 'name':'John', 'age':30, 'car':null }";
            var utf8Reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json.Substring(1,json.Length-2)));
            
            JSONDesirialize result = JsonSerializer.Deserialize<JSONDesirialize>(ref utf8Reader);
            MessageBox.Show(result.ToString());
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


        public static readonly List<string> keywords = new List<string> { "#include", "using", "namespace", "for", "while", "return" };
        public static readonly List<char> symbols = new List<char> { '.', ')', '(', '[', ']', '>', '<', ':', ';', '\n', '\t', '\r' };
        public int length()
        {
            return formatList.Count;
        }
    }
}
