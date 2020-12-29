using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Windows;
using System.Web;
using System.IO;

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
            
            switch (language)
            {
                case Languages.CPlusPlus:
                    MessageBox.Show("OK");
                    break;
                case Languages.C:
                    break;
                case Languages.CSharph:
                    break;
                case Languages.Java:
                    break;
                case Languages.JavaScripst:
                    break;
                default:
                    break;
            }
            Color color = (Color)ColorConverter.ConvertFromString("#d8a0df");
           
            formatList.Add("#include", Brushes.Red);
            formatList.Add("using", Brushes.Red);
            formatList.Add("namespace", Brushes.Red);
            formatList.Add("for", Brushes.Red);
            formatList.Add("return", Brushes.Red);
            formatList.Add("while", Brushes.Red);
            formatList.Add("break", new SolidColorBrush(color));
            
                     
            
            formatList.Add("main", Brushes.Green);

            formatList.Add("private", Brushes.Blue);
            formatList.Add("public", Brushes.Blue);
            formatList.Add("protected", Brushes.Blue);
            formatList.Add("int", Brushes.Blue);
            formatList.Add("string", Brushes.Blue);
            formatList.Add("virtual", Brushes.Blue);
            formatList.Add("void", Brushes.Blue);
            formatList.Add("override", Brushes.Blue);
        }
        public Snippet()
        {
            //using (StreamReader streamReader = File.OpenText(@"C:\Users\thinh\Desktop\json.json"))
            //{
            //    var obj = streamReader.ReadToEnd();
            //    Test instance = JsonConvert.DeserializeObject<Test>(obj);
            //    MessageBox.Show(instance.name + "\n" + instance.age);
            //}

        }




        
    }
}
