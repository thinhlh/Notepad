using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Notepad.Snippets;
using Notepad.Classes;
namespace Notepad
{
    /// <summary>
    /// Interaction logic for RichTextBoxUC.xaml
    /// </summary>
    public partial class RichTextBoxUC : UserControl
    {
        #region field

        private Font defaultFont;
        private Color defaultColor;
        private Color defaultBackColor;
        private MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        private Control virtualControl = null;
        private Languages _language=Languages.None;
        public Languages Language { 
            get => _language; 
            set { 
                _language = value;
                InvokeHighlightAll(); 
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
                List<MainTabItem> tabItems = mainWindow.tabItems;
                TabControl tabControl = mainWindow.tabControl;

                if (tabControl.SelectedIndex < 0) //Initialize
                    tabItems[tabItems.Count - 1].Data = value;
                else
                    tabItems[tabControl.SelectedIndex].Data = value;
                //richTextBox.ScrollToCaret();
            }
        }

        
        #endregion
        public RichTextBoxUC()
        {
            InitializeComponent();
            this.DataContext = mainWindow;

            #region setup
            defaultFont = new Font(new FontFamily(MainWindow.appSetting.Get("font_family")), float.Parse(MainWindow.appSetting.Get("font_size")), System.Drawing.FontStyle.Regular);
            defaultColor = JsonDeserialize.GetColorFromString(MainWindow.appSetting.Get("foreground_color"));
            defaultBackColor = JsonDeserialize.GetColorFromString(MainWindow.appSetting.Get("background_color"));
            #endregion



            #region Setup RichTextBox

            richTextBox.TextChanged += richTextBox_TextChangedSavedIcon;
            richTextBox.TextChanged += richTextBox_Highlight;
            richTextBox.TextChanged += RichTextBox_TextChangedLineNumber;
            richTextBox.Font = defaultFont;
            #endregion
        }

        private void CountLineNumber()
        {
            TabControl tabControl = mainWindow.tabControl;

            if(tabControl.SelectedIndex<0) return;

            (mainWindow.tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).textBox.Text = "";


            int firstLine = GetFirstVisibleLine();
            int lastLine = GetLastVisibleLine();
            StringBuilder stringBuilder = new StringBuilder();
            if (firstLine == lastLine) (mainWindow.tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).textBox.Text = (firstLine + 1).ToString() + "\n";
            else
            {
                for (int i = firstLine + 1; i <= lastLine + 1; i++)
                {

                    stringBuilder.AppendLine(i.ToString());
                }
                //(mainWindow.tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).textBox.Text = stringBuilder.ToString();
            }
        }
        public void RichTextBox_TextChangedLineNumber(object sender, EventArgs e)
        {
            CountLineNumber();
        }



        public void richTextBox_TextChangedSavedIcon(object sender, EventArgs e)
        {
            MainWindowExtension.RaiseUnsavedIcon();
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
            //Unsubscribe TextChanged Events
            richTextBox.TextChanged -= this.richTextBox_Highlight;
            richTextBox.TextChanged -= this.richTextBox_TextChangedSavedIcon;
            richTextBox.TextChanged -= this.RichTextBox_TextChangedLineNumber;

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

            //Subscribe TextChanged events;
            richTextBox.TextChanged += this.richTextBox_Highlight;
            richTextBox.TextChanged += this.richTextBox_TextChangedSavedIcon;
            richTextBox.TextChanged += this.RichTextBox_TextChangedLineNumber;

            virtualControl = null;
        }

        private void InvokeHighlightAll()
        {
            if (virtualControl == null)
            {
                virtualControl = mainWindow;
                virtualControl.Focus();
            }

            //Unsubscribe TextChanged events;
            richTextBox.TextChanged -= this.richTextBox_Highlight;
            richTextBox.TextChanged -= this.richTextBox_TextChangedSavedIcon;
            richTextBox.TextChanged -= this.RichTextBox_TextChangedLineNumber;

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

            highlighter.HighlightRange(0, richTextBox.Text.Length);

            richTextBox.Focus();
            richTextBox.SelectionStart = currentCaret;

            //Subscribe TextChanged events;
            richTextBox.TextChanged += this.richTextBox_Highlight;
            richTextBox.TextChanged += this.richTextBox_TextChangedSavedIcon;
            richTextBox.TextChanged += this.RichTextBox_TextChangedLineNumber;

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
                    List<Dictionary<string, string>> keywords;

                    if (Language == Languages.CSharph)
                        keywords = JsonDeserialize.CSharph.keywords;
                    else if (Language == Languages.Java)
                        keywords = JsonDeserialize.Java.keywords;
                    else if (Language == Languages.CPlusPlus)
                        keywords = JsonDeserialize.CPlusPlus.keywords;
                    else if (Language == Languages.C)
                        keywords = JsonDeserialize.C.keywords;
                    else
                        keywords = new List<Dictionary<string, string>>();

                    foreach (Dictionary<string, string> keyword in keywords)
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
                    string comment;

                    if (Language == Languages.CSharph)
                        comment = JsonDeserialize.CSharph.comment;
                    else if (Language == Languages.Java)
                        comment = JsonDeserialize.Java.comment;
                    else if (Language == Languages.CPlusPlus)
                        comment = JsonDeserialize.CPlusPlus.comment;
                    else if (Language == Languages.C)
                        comment = JsonDeserialize.C.comment;
                    else
                        comment = null;

                    richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(comment);
                    richTextBox.SelectionFont = new Font(richTextBox.Font, System.Drawing.FontStyle.Italic);
                }

                else if (tokenType == TokenType.String)
                {
                    string String;

                    if (Language == Languages.CSharph)
                        String = JsonDeserialize.CSharph.String;
                    else if (Language == Languages.Java)
                        String = JsonDeserialize.Java.String;
                    else if (Language == Languages.CPlusPlus)
                        String = JsonDeserialize.CPlusPlus.String;
                    else if (Language == Languages.C)
                        String = JsonDeserialize.C.String;
                    else
                        String = null;

                    richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(String);
                }

                else if(tokenType==TokenType.preprocessor)
                {
                    string preprocessor;

                    if (Language == Languages.CSharph)
                        preprocessor = JsonDeserialize.CSharph.preprocessor;
                    else if (Language == Languages.Java)
                        preprocessor = JsonDeserialize.Java.preprocessor;
                    else if (Language == Languages.CPlusPlus)
                        preprocessor = JsonDeserialize.CPlusPlus.preprocessor;
                    else
                        preprocessor = null;

                    richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(preprocessor);
                    richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(preprocessor);
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
                    List<Dictionary<string, string>> keywords;

                    if (Language == Languages.CSharph)
                        keywords = JsonDeserialize.CSharph.keywords;
                    else if (Language == Languages.Java)
                        keywords = JsonDeserialize.Java.keywords;
                    else if (Language == Languages.CPlusPlus)
                        keywords = JsonDeserialize.CPlusPlus.keywords;
                    else if (Language == Languages.C)
                        keywords = JsonDeserialize.C.keywords;
                    else
                        keywords = new List<Dictionary<string, string>>();

                    foreach (Dictionary<string, string> keyword in keywords)
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
                    string comment;

                    if (Language == Languages.CSharph)
                        comment = JsonDeserialize.CSharph.comment;
                    else if (Language == Languages.Java)
                        comment = JsonDeserialize.Java.comment;
                    else if (Language == Languages.CPlusPlus)
                        comment = JsonDeserialize.CPlusPlus.comment;
                    else if (Language == Languages.C)
                        comment = JsonDeserialize.C.comment;
                    else
                        comment = null;

                    richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(comment);
                    richTextBox.SelectionFont = new Font(richTextBox.Font, System.Drawing.FontStyle.Italic);
                }

                else if (tokenType == TokenType.String)
                {
                    string String;

                    if (Language == Languages.CSharph)
                        String = JsonDeserialize.CSharph.String;
                    else if (Language == Languages.Java)
                        String = JsonDeserialize.Java.String;
                    else if (Language == Languages.CPlusPlus)
                        String = JsonDeserialize.CPlusPlus.String;
                    else if (Language == Languages.C)
                        String = JsonDeserialize.C.String;
                    else
                        String = null;

                    richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(String);
                }

                else if (tokenType == TokenType.preprocessor)
                {
                    string preprocessor;

                    if (Language == Languages.CSharph)
                        preprocessor = JsonDeserialize.CSharph.preprocessor;
                    else if (Language == Languages.Java)
                        preprocessor = JsonDeserialize.Java.preprocessor;
                    else if (Language == Languages.CPlusPlus)
                        preprocessor = JsonDeserialize.CPlusPlus.preprocessor;
                    else
                        preprocessor = null;

                    richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(preprocessor);
                    richTextBox.SelectionColor = JsonDeserialize.GetColorFromString(preprocessor);
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
            if(e.Control&&e.KeyCode==System.Windows.Forms.Keys.N&&Commands.NewFileCanExecute)
            {
                Commands.NewFileExecuted();
            }
            else if(!e.Shift&&e.Control&&e.KeyCode==System.Windows.Forms.Keys.O&&Commands.OpenFileCanExecute)
            {
                Commands.OpenFileExecuted();
            }
            else if(e.Control&&e.Shift&&e.KeyCode==System.Windows.Forms.Keys.O&&Commands.OpenFolderCanExecute)
            {
                Commands.OpenFolderExecuted();
            }
            else if(!e.Shift&&e.Control&&e.KeyCode==System.Windows.Forms.Keys.S&&Commands.SaveCanExecute)
            {
                Commands.SaveExecuted();
            }
            else if(e.Control&&e.Shift&&e.KeyCode==System.Windows.Forms.Keys.S&&Commands.SaveAsCanExecute)
            {
                Commands.SaveAsExecuted();
            }
            else if(e.Control&&e.KeyCode==System.Windows.Forms.Keys.W&&Commands.CloseAllFilesCanExecute)
            {
                Commands.CloseFileExecuted();
            }
            else if(!e.Shift&&e.Control&&e.KeyCode==System.Windows.Forms.Keys.T&&Commands.NewTerminalCanExecute)
            {
                Commands.NewTerminalExecuted();
            }
            else if(e.Control&&e.Shift&&e.KeyCode==System.Windows.Forms.Keys.T&&Commands.NewTerminalCurrentDirCanExecute)
            {
                Commands.NewTerminalCurrentDirExecuted();
            }
            else if(e.Control&&e.KeyCode==System.Windows.Forms.Keys.B&&Commands.BuildCanExecute)
            {
                Commands.BuildExecuted();
            }
            else if(!e.Shift&&e.Control&&e.Shift&&e.KeyCode==System.Windows.Forms.Keys.B&&Commands.BuildAndRunCanExecute)
            {
                Commands.BuildAndRunExecuted();
            }
        }

        private void richTextBox_VScroll(object sender, EventArgs e)
        {
            ScrollChangedEventArgs scrollChangedEventArgs = e as ScrollChangedEventArgs;
            CountLineNumber();
        }
    }
}
