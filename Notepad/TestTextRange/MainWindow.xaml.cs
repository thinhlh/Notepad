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
        private TextRange getCurrentLine()
        {
            TextPointer start = richTextBox.CaretPosition.GetLineStartPosition(0);
            TextPointer end = richTextBox.CaretPosition.GetLineStartPosition(1);
            if (end == null)//the last lije
            {
                end = start;
                while(end.GetPointerContext(LogicalDirection.Forward)!=TextPointerContext.ElementEnd&&!new TextRange(end,end.GetPositionAtOffset(1)).Text.Contains("\r\n"))
                {
                    end = end.GetPositionAtOffset(1);
                }
            }
            return new TextRange(start, end);
            
        }

        private TextRange getCurrentWordRange()
        {
            List<string> spaces = new List<string> { " ", "\t" };
            List<string> symbols = new List<string> { ";", "(", ")", "=" };
            TextPointer current = richTextBox.CaretPosition;
            TextPointer start = current, end = current;
            TextRange backward;
            TextRange forward;


            if (start.GetPositionAtOffset(-1) == null)
                backward = new TextRange(start, start);
            else
                backward = new TextRange(start.GetPositionAtOffset(-1), start);

            if (end.GetPositionAtOffset(1) == null)
                forward = new TextRange(end, end);
            else
                forward = new TextRange(end, end.GetPositionAtOffset(1)); 



            while(!spaces.Contains(backward.Text)&&start.GetPointerContext(LogicalDirection.Backward)==TextPointerContext.Text&&!symbols.Contains(backward.Text))
            {
                start = start.GetPositionAtOffset(-1);
                backward = new TextRange(start, start.GetPositionAtOffset(-1));
            }    
            while(!spaces.Contains(forward.Text) && end.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text && !symbols.Contains(forward.Text))
            {
                end = end.GetPositionAtOffset(1);
                forward = new TextRange(end, end.GetPositionAtOffset(1));
            }
            
            if(new TextRange(start,end).Text=="")
            {
                if(symbols.Contains(backward.Text))
                {
                    start = start.GetPositionAtOffset(-1);
                }
                else if(symbols.Contains(forward.Text))
                {
                    end = end.GetPositionAtOffset(-1);
                }
            }

            Word = new TextRange(start, end).Text;
            return new TextRange(start, end);
        }

        private void richTextBox_SelectionChanged(object sender,RoutedEventArgs e)
        {
            getCurrentWordRange();
        }

        //private struct Tag
        //{
        //    public TextPointer start;
        //    public TextPointer end;
        //    public string word;
        //}

        //private List<Tag> tags = new List<Tag>();
        //internal void CheckWordsInLine(TextRange range)
        //{
        //    string text = range.Text;
        //    int startIndex = 0, endIndex = 0;
        //    for(int i=0;i<text.Length;i++)
        //    {
        //        if(Char.IsWhiteSpace(text[i])||Snippet.symbols.Any((s)=>text.Contains(s)))
        //        {
        //            if(i>0&&!(char.IsWhiteSpace(text[i-1])|| Snippet.symbols.Any((s) => text.Contains(s))))
        //            {
        //                endIndex = i - 1;
        //                string currentWord = text.Substring(startIndex, endIndex - startIndex + 1);
        //                if(Snippet.keywords.Any((s)=>text.Contains(s)))
        //                {
        //                    Tag tag = new Tag();
        //                    tag.start = range.Start.GetPositionAtOffset(startIndex, LogicalDirection.Forward);
        //                    tag.end = range.Start.GetPositionAtOffset(endIndex + 1, LogicalDirection.Backward);
        //                    tag.word = currentWord;
        //                    tags.Add(tag);
        //                }
        //            }
        //            startIndex = i + 1;
        //        }
        //    }

        //    string lastWord = text.Substring(startIndex, text.Length - startIndex);
        //    if(Snippet.keywords.Any((s)=>lastWord.Contains(s)))
        //    {
        //        Tag tag = new Tag();
        //        tag.start = range.Start.GetPositionAtOffset(startIndex, LogicalDirection.Forward);
        //        tag.end = range.Start.GetPositionAtOffset(text.Length, LogicalDirection.Backward);
        //        tag.word = lastWord;
        //        tags.Add(tag);
        //    }
        //}

        //private void richTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        //{
        //    if (richTextBox.Document == null) return;
        //    else
        //    {
        //        richTextBox.SelectionChanged -= richTextBox_SelectionChanged;
        //        tags.Clear();

        //        TextRange range = getCurrentLine();
        //        range.ClearAllProperties();


        //        CheckWordsInLine(range);

        //        for (int i = 0; i < tags.Count; i++)
        //        {
        //            try
        //            {
        //                TextRange textRange = new TextRange(tags[i].start, tags[i].end);
        //                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
        //            }
        //            catch 
        //            {
        //                MessageBox.Show("Error");
        //            }
        //        }
        //        richTextBox.SelectionChanged += richTextBox_SelectionChanged;
        //        Word = range.Text;
        //    }
        //}




        //private void richTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        //{
        //    /* Example: This is not a sentence |||and Current a pointer at 'o' in "not" 
        //     * start will move backward until TextRange between start and current have space 
        //     * and end pointer will move forward until TextRange between end and current have space too*/
        //    List<string> spaces = new List<string> { " ", "\t" };
        //    List<string> symbols=new List<string>{ ";", "(", ")", "=" };

        //    TextPointer current = richTextBox.CaretPosition;
        //    TextPointer start = current, end = current;

        //    // Traverse to the end of the word

        //    while(end.GetPointerContext(LogicalDirection.Forward)==TextPointerContext.Text)
        //    {
        //        TextRange currentCharRange = new TextRange(end, end.GetPositionAtOffset(1));
        //        if(spaces.Contains(currentCharRange.Text)||symbols.Contains(currentCharRange.Text))
        //        {
        //            if(!symbols.Contains(new TextRange(start,end).Text))
        //            {
        //                end = end.GetPositionAtOffset(1);
        //            }
        //            break;
        //        }
        //        else
        //        {
        //            end = end.GetPositionAtOffset(1);
        //            currentCharRange = new TextRange(end, end.GetPositionAtOffset(1));
        //        }
        //    }
        //    while (start.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text)
        //    {
        //        TextRange currentCharRange = new TextRange(start, start.GetPositionAtOffset(-1));
        //        if (spaces.Contains(currentCharRange.Text) || symbols.Contains(currentCharRange.Text))
        //        {
        //            if(!symbols.Contains(new TextRange(start,end).Text))
        //            {
        //                start = start.GetPositionAtOffset(-1);
        //            }
        //            break;
        //        }

        //        else
        //        {

        //            start = start.GetPositionAtOffset(-1);
        //            currentCharRange = new TextRange(start.GetPositionAtOffset(-1),start);
        //        }
        //    }

        //    Word = new TextRange(start, end).Text;

        //    Highlighting(new TextRange(start, end));
        //}

        private void Highlighting(TextRange textRange)
        {
            string word = textRange.Text;
            Snippet snippet = new Snippet(Snippet.Languages.CSharph);
            int result;
            if (Int32.TryParse(word, out result) == true)
                textRange.ApplyPropertyValue(ForegroundProperty, Brushes.Chocolate);
            else if (snippet.FormatList.ContainsKey(word))
                textRange.ApplyPropertyValue(ForegroundProperty, snippet.FormatList[word]);
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

        private void richTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            getCurrentWordRange();
            Highlighting(getCurrentWordRange());
        }
        #endregion
        //textRange.Text.Contains(";") || textRange.Text.Contains("(") || textRange.Text.Contains(")") || textRange.Text.Contains("=")
    }
}
