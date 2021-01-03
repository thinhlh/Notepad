using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Notepad.Snippets;
namespace Notepad.Classes
{
    /// <summary>
    /// Interaction logic for RichTextBoxUC.xaml
    /// </summary>
    public partial class RichTextBoxUC : UserControl
    {
        #region field

        private Font defaultFont = new Font(System.Drawing.FontFamily.GenericSansSerif, 14, System.Drawing.FontStyle.Regular);
        private Color defaultColor = Color.Black;
        private Color defaultBackColor = Color.White;
        private MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        private Control virtualControl = null;
        private Languages _language=Languages.None;
        public Languages Language { 
            get => _language; 
            set { 
                _language = value;
                InvokeHighlight(); 
            }
        }
        public int previousCaret = 0;
        public int currentCaret = 0;
        public ISnippet highlighter=new PlainText();

        public string Text
        {
            get => richTextBox.Text;
            set
            {
                richTextBox.Text = value;
                //richTextBox.ScrollToCaret();
            }
        }

        
        #endregion
        public RichTextBoxUC()
        {
            InitializeComponent();
            this.DataContext = DataContext;


            #region Setup RichTextBox

            richTextBox.TextChanged += richTextBox_TextChangedSavedIcon;
            richTextBox.TextChanged += richTextBox_Highlight;
            richTextBox.TextChanged += RichTextBox_TextChangedLineNumber;
            richTextBox.Font = defaultFont;
            #endregion
        }

        private void RichTextBox_TextChangedLineNumber(object sender, EventArgs e)
        {
            TabControl tabControl = mainWindow.tabControl;
            (mainWindow.tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).textBox.Text = "";


            int firstLine = GetFirstVisibleLine();
            int lastLine = GetLastVisibleLine();
            if (firstLine == lastLine) (mainWindow.tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).textBox.Text = (firstLine + 1).ToString() + "\n";
            else
            {
                for (int i = firstLine+1; i <= lastLine+1; i++)
                {
                    (mainWindow.tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).textBox.Text += i + "\n";
                }
            }
        }

        private void richTextBox_TextChangedSavedIcon(object sender, EventArgs e)
        {

            List<MainTabItem> tabItems = mainWindow.tabItems;
            TabControl tabControl = mainWindow.tabControl;

            if (tabControl.SelectedIndex < 0) //Initialize
                tabItems[tabItems.Count - 1].Data = Text;
            else
                tabItems[tabControl.SelectedIndex].Data = Text;

            RaiseUnsavedIcon();
        }
        public void richTextBox_Highlight(object sender,EventArgs e)
        {
            InvokeHighlight();         
        }

        private void InvokeHighlight()
        {
            if (virtualControl == null)
            {
                virtualControl = mainWindow;
                virtualControl.Focus();
            }

            //check for highlight and return highlighter here

            int currentCaret = richTextBox.SelectionStart;

            if (Language == Languages.CSharph)
                highlighter = new CSharph();
            else if (Language == Languages.Java)
                highlighter = new Java();
            else if (Language == Languages.CPlusPlus)
                highlighter = new CPlusPlus();
            else if (Language == Languages.C)
                highlighter = new C();
            else
                highlighter = new PlainText();

            highlighter.Highlight();

            richTextBox.Focus();
            richTextBox.SelectionStart = currentCaret;

            virtualControl = null;
        }

        

        

        #region Extension Methods

        

        public void SetStyle(string Pattern, TokenType tokenType) //the range should be the first character of the lineBefore textchanged and the last character of the line after changed
        {
            string token = richTextBox.Text.Substring(richTextBox.SelectionStart, richTextBox.SelectionLength);
            MatchCollection matchCollection = Regex.Matches(token, Pattern);

            if (matchCollection.Count < 1) return;

            int currentPosition = richTextBox.SelectionStart;
            int currenLength = richTextBox.SelectionLength;
            int offset = richTextBox.SelectionStart;

            

            foreach (Match m in matchCollection)
            {
                richTextBox.SelectionStart = m.Index + offset;
                richTextBox.SelectionLength = m.Length;
                richTextBox.SelectionColor = Color.Transparent;

                if (tokenType == TokenType.keywords)
                {
                    foreach (Dictionary<string, string> keyword in JsonDeserialize.CSharph.keywords)
                    {
                        var key = keyword.Keys.ToList();
                        if (key.Contains(m.Value))
                        {
                            richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(keyword[m.Value]);
                            break;
                        }
                    }
                }

                else if (tokenType == TokenType.comment)
                {
                    richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(JsonDeserialize.CSharph.comment);
                    richTextBox.SelectionFont = new Font(richTextBox.Font, System.Drawing.FontStyle.Italic);
                }

                else if (tokenType == TokenType.String)
                {
                    richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(JsonDeserialize.CSharph.String);
                }

                else if(tokenType==TokenType.preprocessor)
                {
                    richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(JsonDeserialize.CSharph.preprocessor);
                }
            }

            richTextBox.SelectionStart = currentPosition;
            richTextBox.SelectionLength = currenLength;
        }


        public void SetStyle(int start,int length,string Pattern, TokenType tokenType) //the range should be the first character of the lineBefore textchanged and the last character of the line after changed
        {
            string token = richTextBox.Text.Substring(start,length);
            MatchCollection matchCollection = Regex.Matches(token, Pattern);

            if (matchCollection.Count < 1) return;

            int currentPosition = richTextBox.SelectionStart;
            int currenLength = richTextBox.SelectionLength;
            int offset = richTextBox.SelectionStart;



            foreach (Match m in matchCollection)
            {
                richTextBox.SelectionStart = m.Index + offset;
                richTextBox.SelectionLength = m.Length;
                richTextBox.SelectionColor = Color.Transparent;

                if (tokenType == TokenType.keywords)
                {
                    foreach (Dictionary<string, string> keyword in JsonDeserialize.CSharph.keywords)
                    {
                        var key = keyword.Keys.ToList();
                        if (key.Contains(m.Value))
                        {
                            richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(keyword[m.Value]);
                            break;
                        }
                    }
                }

                else if (tokenType == TokenType.comment)
                {
                    richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(JsonDeserialize.CSharph.comment);
                    richTextBox.SelectionFont = new Font(richTextBox.Font, System.Drawing.FontStyle.Italic);
                }

                else if (tokenType == TokenType.String)
                {
                    richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(JsonDeserialize.CSharph.String);
                }

                else if (tokenType == TokenType.preprocessor)
                {
                    richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(JsonDeserialize.CSharph.preprocessor);
                }
            }

            richTextBox.SelectionStart = currentPosition;
            richTextBox.SelectionLength = currenLength;
        }


        public void ClearStyle()
        {

            previousCaret = (previousCaret > currentCaret) ? currentCaret - 1 : previousCaret;


            richTextBox.SelectionStart = GetFirstCharIndexFromLine(previousCaret);
            richTextBox.SelectionLength = GetLastCharIndexFromLine(currentCaret) - richTextBox.SelectionStart;

            richTextBox.SelectionColor = defaultColor; // can change to the system.default color
            richTextBox.SelectionFont = defaultFont;
            richTextBox.SelectionBackColor = defaultBackColor;
            
        }

        public void ClearStyle(int start,int length)
        {

            richTextBox.SelectionStart = start;
            richTextBox.SelectionLength = length;

            richTextBox.SelectionColor = defaultColor;
            richTextBox.SelectionBackColor = defaultBackColor;
            richTextBox.SelectionFont = defaultFont;
            

        }
        private int GetFirstCharIndexFromLine(int charIndexOfLine)
        {
            int firstCharIndex = charIndexOfLine;

            if (firstCharIndex < 0 || firstCharIndex > richTextBox.Text.Length) return 0;

            while (firstCharIndex > 0 && Text[firstCharIndex - 1] != '\n')
            {
                firstCharIndex--;
            }

            return firstCharIndex;
        }
        

        private int GetLastCharIndexFromLine(int charIndexOfLine)
        {

            if (charIndexOfLine < 0 || charIndexOfLine > richTextBox.Text.Length) return richTextBox.Text.Length;

            while (charIndexOfLine < richTextBox.Text.Length && Text[charIndexOfLine] != '\n')
            {
                charIndexOfLine++;
            }

            return charIndexOfLine;
        }
        private void RaiseUnsavedIcon()
        {
            List<MainTabItem> tabItems = mainWindow.tabItems;
            TabControl tabControl = mainWindow.tabControl;

            if (tabItems[tabControl.SelectedIndex].IsSaved == false)
            {
                return;
            }
            else
            {
                if (Text == "" || Text == "\r\n") //Initialize Circumstance
                    return;
                else
                {
                    //Raise* at the end and keep isSaved = false when there is a change with out save before
                    tabItems[tabControl.SelectedIndex].Header += "*";
                    tabItems[tabControl.SelectedIndex].IsSaved = false;
                }
            }
        }

        public int GetFirstVisibleLine()
        {
            int index = richTextBox.GetCharIndexFromPosition(new System.Drawing.Point(0, 0));
            int line = richTextBox.GetLineFromCharIndex(index);
            return line;
        }

        public int GetLastVisibleLine()
        {
            int index = richTextBox.GetCharIndexFromPosition(new System.Drawing.Point(0, richTextBox.Height));
            int line = richTextBox.GetLineFromCharIndex(index);
            return line;
        }
        #endregion

        private void richTextBox_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            if(!e.IsInputKey&&e.Control&&e.KeyCode==System.Windows.Forms.Keys.N)
            {
               
            }
        }

        private void richTextBox_VScroll(object sender, EventArgs e)
        {
            ScrollChangedEventArgs scrollChangedEventArgs = e as ScrollChangedEventArgs;
            
        }
    }
}
