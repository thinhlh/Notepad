using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TestTextRange
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private string word="";
        public string Word
        {
            get => word;
            set 
            { 
                word = value;
                OnPropertyChanged(); 
            }
        }
        

        private void richTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            /* Example: This is not a sentence |||and Current a pointer at 'o' in "not" 
             * start will move backward until TextRange between start and current have space 
             * and end pointer will move forward until TextRange between end and current have space too*/
            List<string> spaces = new List<string> { " ", "\t" ,";","(", ")", "=" };
            List<string> symbols=new List<string>{};

            TextPointer current = richTextBox.CaretPosition;
            TextPointer start = current, end = current;

            //Traverse to get the start of word
            while (start.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text)
            {
                
                TextRange textRange = new TextRange(start.GetPositionAtOffset(-1), start); // get the textRange of the previous start
                if (spaces.Any(token => textRange.Text.Contains(token))) // check if textRange.Text has token that inside spaces
                    break;
                //else if(symbols.Any(token=>textRange.Text.Contains(token)))
                //{
                //    start = start.GetPositionAtOffset(-1);
                //}
                else
                {
                    start = start.GetPositionAtOffset(-1);
                }
                
            }


            //Traverse to get the end of word
            while(end.GetPointerContext(LogicalDirection.Forward)==TextPointerContext.Text)
            {
                TextRange textRange = new TextRange(end,end.GetPositionAtOffset(1)); // get the textRange of the next start
                if (spaces.Any(token => textRange.Text.Contains(token)))
                    break;
                //else if (symbols.Any(token => textRange.Text.Contains(token)))
                //{
                   
                //}
                else
                {
                    end = end.GetPositionAtOffset(1);
                }
            }
            Word = new TextRange(start, end).Text;

            Highlighting(new TextRange(start, end));
        }

        private void Highlighting(TextRange textRange)
        {
            string word = textRange.Text;
            Snippet CSharphSnippet = new Snippet();
            int result;
            if (Int32.TryParse(word, out result) == true)
                textRange.ApplyPropertyValue(ForegroundProperty, Brushes.Chocolate);
            else if (CSharphSnippet.FormatList.ContainsKey(word))
                textRange.ApplyPropertyValue(ForegroundProperty, CSharphSnippet.FormatList[word]);
            else
                textRange.ApplyPropertyValue(ForegroundProperty, Brushes.Black);
        }

        #region Inplement Interface
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string newName=null)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(newName));
            }
        }
        #endregion
        //textRange.Text.Contains(";") || textRange.Text.Contains("(") || textRange.Text.Contains(")") || textRange.Text.Contains("=")
    }
}
