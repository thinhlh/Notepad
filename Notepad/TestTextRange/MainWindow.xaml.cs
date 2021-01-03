using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing;
namespace TestTextRange
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static readonly Regex symbols = new Regex("([ \\t{}():;])");
        public string pattern;
        public string word;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Snippet Highlight = new CSharph();
            
            pattern=@"\b("+GetListOfKeyWord(JsonDesirialize.CSharph.keywords)+")\\b";
        }


        private string GetListOfKeyWord(List<Dictionary<string,string>> keywords)
        {
            string regex="";
            foreach (Dictionary<string,string> keyword in keywords)
            {
                var key = keyword.Keys.ToList();
                regex += "|" + key[0];
            }
            return regex.Substring(1);
        }
        public string Word
        {
            get => word;
            set
           {
                word = value;
                OnPropertyChanged();
            }
        }

        public void GetWord(string line)
        {
            string[] tokens = symbols.Split(line);


            foreach (string token in tokens)
            {
                string value="#000000";
                richTextBox.SelectionColor = System.Drawing.Color.Black;
                for (int i = 0; i < JsonDesirialize.CSharph.keywords.Count; i++)
                {

                    if(JsonDesirialize.CSharph.keywords[i].TryGetValue(token,out value))
                    {
                        richTextBox.SelectionColor = (System.Drawing.Color)System.Windows.Media.ColorConverter.ConvertFromString(value);
                        break;
                    }
                }
                richTextBox.SelectedText = token;
                
            }
            richTextBox.SelectedText = "\n";
        }


        private int GetLastCharIndexOfCurrentLine()
        {
            int lastCharIndex = richTextBox.GetFirstCharIndexOfCurrentLine();

            if (lastCharIndex < 0 || lastCharIndex > richTextBox.Text.Length) return richTextBox.Text.Length;

            while (lastCharIndex < richTextBox.Text.Length && richTextBox.Text[lastCharIndex] != '\n')
            {
                lastCharIndex++;
            }
            return lastCharIndex;
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
        private void richTextBox_SelectionChanged(object sender, EventArgs e)
        {
            if (richTextBox.Text == "")
                Word = "";
            else Word=richTextBox.Lines[richTextBox.GetLineFromCharIndex(richTextBox.SelectionStart)];   
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            ClearStyle();
            SetStyle(pattern,TokenType.keywords);
            SetStyle("\".*\"", TokenType.String);
            SetStyle(@"\/\/.*",TokenType.comment);
            SetStyle(@"\/\*.*\*\/", TokenType.comment);
            
        }
        enum TokenType
        {
            keywords,
            comment,
            String,

        }
        private void SetStyle(string Pattern,TokenType tokenType)
        {   
            int currentCaret = richTextBox.SelectionStart;
            int currentLenght = richTextBox.SelectionLength;
            int length = GetLastCharIndexOfCurrentLine() - richTextBox.GetFirstCharIndexOfCurrentLine();

            string token = richTextBox.Text.Substring(richTextBox.GetFirstCharIndexOfCurrentLine(), length);
            int offset = richTextBox.GetFirstCharIndexOfCurrentLine();
            MatchCollection matchCollection = Regex.Matches(token, Pattern);
            foreach (Match m in matchCollection)
            {
                richTextBox.SelectionStart = m.Index + offset;
                richTextBox.SelectionLength = m.Length;
                if (tokenType==TokenType.keywords)
                {
                    foreach (Dictionary<string, string> keyword in JsonDesirialize.CSharph.keywords)
                    {
                        var key = keyword.Keys.ToList();
                        if (key.Contains(m.Value))
                        {
                            richTextBox.SelectionColor = System.Drawing.ColorTranslator.FromHtml(keyword[m.Value]);
                            break;
                        }
                    }
                }
                else if(tokenType==TokenType.comment)
                {
                    richTextBox.SelectionColor = System.Drawing.ColorTranslator.FromHtml(JsonDesirialize.CSharph.comment);
                    richTextBox.SelectionFont = new Font(richTextBox.Font, System.Drawing.FontStyle.Italic);
                }
                else if(tokenType==TokenType.String)
                {
                    richTextBox.SelectionColor = System.Drawing.ColorTranslator.FromHtml(JsonDesirialize.CSharph.String);
                }
            }

            richTextBox.SelectionStart = currentCaret;
            richTextBox.SelectionLength = currentLenght;
            richTextBox.SelectionColor = System.Drawing.Color.Black;
            richTextBox.SelectionFont = new Font(richTextBox.Font, System.Drawing.FontStyle.Regular);
        }
        private void ClearStyle()
        {
            int currentCaret = richTextBox.SelectionStart;
            int currentLenght=richTextBox.SelectionLength;
            richTextBox.SelectionStart = richTextBox.GetFirstCharIndexOfCurrentLine();
            richTextBox.SelectionLength = GetLastCharIndexOfCurrentLine() - richTextBox.SelectionStart;
            richTextBox.SelectionColor = System.Drawing.Color.Black;
            richTextBox.SelectionFont = new Font(richTextBox.Font, System.Drawing.FontStyle.Regular);
            richTextBox.SelectionStart = currentCaret;
            richTextBox.SelectionLength = currentLenght;
        }
    }

}




        #region commented
//int commentIndex = isComment();
//if (commentIndex != -1)
//{
//    /*
//     * Because of when having a space, richtextbox auto add end,start element so that 
//     * counting the number of space then double it and plus to commentIndex to have the right start index of comment
//     */

//    //Add This line number to LineThatHasComment
//    Highlighting(new TextRange(currentLine.Start.GetPositionAtOffset(commentIndex), currentLine.End));
//}
//else
//{
//    //check if the line has comment before (if has => check the whole line and highlight it)
//    Highlighting(getCurrentWordRange());
//} 
//public struct Tag
//{
//    public TextPointer start;
//    public TextPointer end;
//    public string word;
//    TextRange getRange()
//    {
//        return new TextRange(start, end);
//    }
//}
//public List<Tag> tags = new List<Tag>();
//public static readonly List<string> spaces = new List<string> { " ", "\t" };
////public static readonly List<char> symbols = new List<char> { ';', '(', ')', '=', '<', '>', ',', '.', '[', ']', ':' };
//public static readonly List<string> symbols = new List<string> { ";", "(", ")", "=", "<", ">", ",", ".", "[", "]", ":" };
//private Snippet highlighter;

//private string word="";
//public string Word
//{
//    get => word;
//    set 
//    { 
//        word = value;
//        OnPropertyChanged(); 
//    }
//}
////private TextRange getCurrentLine() //from content start
////{
////    TextPointer start = richTextBox.CaretPosition.GetLineStartPosition(0);
////    TextPointer end = richTextBox.CaretPosition.GetLineStartPosition(1);// Get the LineStart of the following line but return null if it is document end

////    if (end != null)
////        end = end.GetInsertionPosition(LogicalDirection.Backward);//Rollback to the previous insertion
////    else
////        end = richTextBox.CaretPosition.DocumentEnd.GetInsertionPosition(LogicalDirection.Backward);

////    Word = new TextRange(start, end).Text;
////    return new TextRange(start, end);
////}

//private void NavigateThroughLine(TextRange line)
//{
//    TextPointer navigator = line.Start;
//    while(navigator.CompareTo(line.End)<0)
//    {

//    }
//}


////private int isComment() //return index or -1 if the current line not have comment
////{
////    int commentIndex = getCurrentLine().Text.IndexOf("//");
////    return commentIndex;
////}

//////private int numberOfSpacesFromStart(string sentence,int index)
//////{
//////    string stringBehindComment = sentence.Substring(0, index);
//////    string[] words=stringBehindComment.Split(' ');
//////    return words.Length - 1;
//////}

////private TextRange getCurrentWordRange()
////{
////    TextPointer current = richTextBox.CaretPosition;
////    TextPointer start = current, end = current;
////    TextRange backward;
////    TextRange forward;


////    if (start.GetPositionAtOffset(-1) == null)
////        backward = new TextRange(start, start);
////    else
////        backward = new TextRange(start.GetPositionAtOffset(-1), start);

////    if (end.GetPositionAtOffset(1) == null)
////        forward = new TextRange(end, end);
////    else
////        forward = new TextRange(end, end.GetPositionAtOffset(1));



////    while (!spaces.Contains(backward.Text) && start.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text && !symbols.Contains(backward.Text))
////    {
////        start = start.GetPositionAtOffset(-1);
////        backward = new TextRange(start, start.GetPositionAtOffset(-1));
////    }
////    while (!spaces.Contains(forward.Text) && end.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text && !symbols.Contains(forward.Text))
////    {
////        end = end.GetPositionAtOffset(1);
////        forward = new TextRange(end, end.GetPositionAtOffset(1));
////    }

////    if (new TextRange(start, end).IsEmpty)
////    {
////        if (symbols.Contains(backward.Text))
////        {
////            start = start.GetPositionAtOffset(-1);
////        }
////        else if (symbols.Contains(forward.Text))
////        {
////            end = end.GetPositionAtOffset(-1);
////        }
////    }

////    //Word = new TextRange(start, end).Text;
////    return new TextRange(start, end);
////}


//private void richTextBox_SelectionChanged(object sender,RoutedEventArgs e)
//{
//    //getCurrentWordRange();
//    //getCurrentLine();
//}

////private struct Tag
////{
////    public TextPointer start;
////    public TextPointer end;
////    public string word;
////}

////private List<Tag> tags = new List<Tag>();
////internal void CheckWordsInLine(TextRange range)
////{
////    string text = range.Text;
////    int startIndex = 0, endIndex = 0;
////    for(int i=0;i<text.Length;i++)
////    {
////        if(Char.IsWhiteSpace(text[i])||Snippet.symbols.Any((s)=>text.Contains(s)))
////        {
////            if(i>0&&!(char.IsWhiteSpace(text[i-1])|| Snippet.symbols.Any((s) => text.Contains(s))))
////            {
////                endIndex = i - 1;
////                string currentWord = text.Substring(startIndex, endIndex - startIndex + 1);
////                if(Snippet.keywords.Any((s)=>text.Contains(s)))
////                {
////                    Tag tag = new Tag();
////                    tag.start = range.Start.GetPositionAtOffset(startIndex, LogicalDirection.Forward);
////                    tag.end = range.Start.GetPositionAtOffset(endIndex + 1, LogicalDirection.Backward);
////                    tag.word = currentWord;
////                    tags.Add(tag);
////                }
////            }
////            startIndex = i + 1;
////        }
////    }

////    string lastWord = text.Substring(startIndex, text.Length - startIndex);
////    if(Snippet.keywords.Any((s)=>lastWord.Contains(s)))
////    {
////        Tag tag = new Tag();
////        tag.start = range.Start.GetPositionAtOffset(startIndex, LogicalDirection.Forward);
////        tag.end = range.Start.GetPositionAtOffset(text.Length, LogicalDirection.Backward);
////        tag.word = lastWord;
////        tags.Add(tag);
////    }
////}

////private void richTextBox_SelectionChanged(object sender, RoutedEventArgs e)
////{
////    if (richTextBox.Document == null) return;
////    else
////    {
////        richTextBox.SelectionChanged -= richTextBox_SelectionChanged;
////        tags.Clear();

////        TextRange range = getCurrentLine();
////        range.ClearAllProperties();


////        CheckWordsInLine(range);

////        for (int i = 0; i < tags.Count; i++)
////        {
////            try
////            {
////                TextRange textRange = new TextRange(tags[i].start, tags[i].end);
////                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
////            }
////            catch 
////            {
////                MessageBox.Show("Error");
////            }
////        }
////        richTextBox.SelectionChanged += richTextBox_SelectionChanged;
////        Word = range.Text;
////    }
////}

//private void Highlighting(TextRange textRange)
//{

//    highlighter = new CSharph();
//    highlighter.Highlight(textRange);

//}


//public  void richTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
//{
//    //TextRange currentLine = getCurrentLine();
//    //TextPointer startWord = currentLine.Start;
//    //TextPointer navigator = currentLine.Start;

//    //string text = currentLine.Text;
//    //int startIndex = 0, endIndex = 0;

//    //bool isLetterOrDigit = false;

//    //for(int i=0;i<text.Length;i++)
//    //{

//    //    if(char.IsLetterOrDigit(text[i]))
//    //    {
//    //        isLetterOrDigit = true;
//    //    }
//    //    else if (char.IsWhiteSpace(text[i])||symbols.Contains(text[i]))
//    //    {
//    //        if(!isLetterOrDigit)
//    //        {
//    //            startWord = startWord.GetPositionAtOffset(1);
//    //            isLetterOrDigit = false;
//    //        }
//    //        else
//    //        {
//    //            Tag tag = new Tag();
//    //            tag.start = startWord;
//    //            tag.end = navigator;
//    //            tag.word = new TextRange(startWord, navigator).Text;
//    //            tags.Add(tag);
//    //            startWord = navigator;
//    //            isLetterOrDigit = false;
//    //            MessageBox.Show(tag.word);
//    //        }

//    //    }

//    //    navigator = navigator.GetPositionAtOffset(1);
//    //}

//    //navigator = navigator.GetPositionAtOffset(1);

//    //if (new TextRange(startWord, navigator).Text.IndexOfAny(symbols.ToArray()) < 0 && new TextRange(startWord, navigator).Text.IndexOf(' ') < 0)
//    //{
//    //    Tag tag = new Tag();
//    //    tag.start = startWord;
//    //    tag.end = navigator;
//    //    tag.word = new TextRange(startWord, navigator).Text;
//    //    MessageBox.Show(tag.word);
//    //    tags.Add(tag);
//    //}
//    //for(int i=0;i<tags.Count;i++)
//    //{
//    //    MessageBox.Show(tags[i].word);
//    //    Highlighting(new TextRange(tags[i].start, tags[i].end));
//    //}

//    //int currentLength = getCurrentLine().Text.Length;
//    //if(Math.Abs(currentLength-_previousLength)>1)
//    //{
//    //    MessageBox.Show("Paste!");
//    //}
//    //_previousLength = currentLength;
//    //Highlighting(getCurrentWordRange());
//}

//private int _previousLength = 0;
#endregion