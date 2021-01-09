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
using System.IO;

namespace Notepad
{
    /// <summary>
    /// Interaction logic for RichTextBoxUC.xaml
    /// </summary>
    public partial class RichTextBoxUC : UserControl
    {
        #region field

        public bool lazyload; //true if test is loaded by lazy load method
        public int nextStreamReaderPosition;
        public StreamReader fileStream;
        private Font defaultFont=new Font(new FontFamily(MainWindow.appSetting.Get("font_family")), float.Parse(MainWindow.appSetting.Get("font_size")), System.Drawing.FontStyle.Regular);
        private Color defaultColor= JsonDeserialize.GetColorFromString(MainWindow.appSetting.Get("foreground_color"));
        private Color defaultBackColor= JsonDeserialize.GetColorFromString(MainWindow.appSetting.Get("background_color"));
        private MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        private Control virtualControl = null;
        private Languages _language=Languages.None;
        public Stack<string> undoStack = new Stack<string>();
        public Stack<string> redoStack = new Stack<string>();
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
                UpdateLineNumber(Text.Split('\n').Length);
                //richTextBox.ScrollToCaret();
            }
        }

        
        #endregion
        public RichTextBoxUC()
        {
            InitializeComponent();
            this.DataContext = mainWindow;

            #region setup
            //defaultFont = de
            //defaultColor = JsonDeserialize.GetColorFromString(MainWindow.appSetting.Get("foreground_color"));
            //defaultBackColor = JsonDeserialize.GetColorFromString(MainWindow.appSetting.Get("background_color"));
            undoStack.Push("");
            #endregion

            #region Setup RichTextBox
            richTextBox.KeyDown += richTextBox_AutoBracketKeyDown;
            richTextBox.KeyDown += richTextBox_AutoIndentLine;
            richTextBox.Resize += RichTextBox_Resize;
            SubscribeTextChangedEvents();
            richTextBox.Font = defaultFont;
            #endregion
        }

        private void RichTextBox_Resize(object sender, EventArgs e)
        {
            UpdateLineNumber();
        }

        private int getWidth()
        {
            int w = 25;
            // get total lines of richTextBox1    
            int line = richTextBox.Lines.Length;

            if (line <= 99)
            {
                w = 20 + (int)richTextBox.Font.Size;
            }
            else if (line <= 999)
            {
                w = 30 + (int)richTextBox.Font.Size;
            }
            else
            {
                w = 50 + (int)richTextBox.Font.Size;
            }

            return w;
        }
        private void CountLineNumber(int linesCount=Int16.MaxValue)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (linesCount == Int16.MaxValue)
            {
                int firstLine = GetFirstVisibleLine();
                int lastLine = GetLastVisibleLine();
                for (int i = firstLine; i <= lastLine; i++)
                {
                    stringBuilder.AppendLine((i + 1).ToString());
                }
            }
            else
            {
                for (int i = 1; i <= linesCount; i++)
                {
                    stringBuilder.AppendLine(i.ToString());
                }
            }
            mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].TabItem.LineNumber = stringBuilder.ToString();
        }
        public void RichTextBox_TextChangedLineNumber(object sender, EventArgs e)
        {
            UpdateLineNumber();
        }

        public void UnsubscribeTextChangedEvents()
        {
            richTextBox.TextChanged -= this.richTextBox_TextChangedSavedIcon;
            richTextBox.TextChanged -= this.richTextBox_UndoStackTextChanged;
            richTextBox.TextChanged -= this.richTextBox_Highlight;
            richTextBox.TextChanged -= this.RichTextBox_TextChangedLineNumber;
        }

        public void SubscribeTextChangedEvents()
        {
            richTextBox.TextChanged += this.richTextBox_TextChangedSavedIcon;
            richTextBox.TextChanged += this.richTextBox_UndoStackTextChanged;
            richTextBox.TextChanged += this.richTextBox_Highlight;
            richTextBox.TextChanged += this.RichTextBox_TextChangedLineNumber;
        }

        public void richTextBox_UndoStackTextChanged(object sender, EventArgs e)
        {
            undoStack.Push(Text);
            GC.Collect();
        }

        public void richTextBox_Highlight(object sender,EventArgs e)
        {
            InvokeHighlight();
        }
        public void richTextBox_TextChangedSavedIcon(object sender, EventArgs e)
        {
            MainWindowExtension.RaiseUnsavedIcon();
        }    
        private void InvokeHighlight()
        {
            if (virtualControl == null)
            {
                virtualControl = mainWindow;
                virtualControl.Focus();
            }
            //Unsubscribe TextChanged Events
            UnsubscribeTextChangedEvents();

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
            SubscribeTextChangedEvents();

            virtualControl = null;
        }

        public void InvokeHighlightAll()
        {
            if (virtualControl == null)
            {
                virtualControl = mainWindow;
                virtualControl.Focus();
            }

            //Unsubscribe TextChanged events;
            UnsubscribeTextChangedEvents();

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
            SubscribeTextChangedEvents();

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
            previousCaret = (previousCaret > currentCaret) ? currentCaret - 1 : previousCaret;

            richTextBox.SelectionStart = start;
            richTextBox.SelectionLength = length;

            richTextBox.SelectionColor = defaultColor;
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

        private void richTextBox_VScroll(object sender, EventArgs e)
        {
            ScrollChangedEventArgs scrollChangedEventArgs = e as ScrollChangedEventArgs;
            //StringBuilder stringBuilder = new StringBuilder(richTextBox.Text);
            //if (lazyload==true)
            //{
            //    while(!fileStream.EndOfStream)
            //    {
            //        stringBuilder.Append(fileStream.ReadLine());
            //    }
            //}
            //richTextBox.Text = stringBuilder.ToString();
            UpdateLineNumber();
        }
        private void UpdateLineNumber(int linesCount=Int16.MaxValue)
        {
            mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].TabItem.LineNumber = "";

            if (Int16.MaxValue == linesCount)
                CountLineNumber();
            else
                CountLineNumber(linesCount);
            mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].TabItem.textBox.Invalidate();
        }

        private void richTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == System.Windows.Forms.Keys.N && Commands.NewFileCanExecute)
            {
                Commands.NewFileExecuted();
            }
            else if (!e.Shift && e.Control && e.KeyCode == System.Windows.Forms.Keys.O && Commands.OpenFileCanExecute)
            {
                Commands.OpenFileExecuted();
            }
            else if (e.Control && e.Shift && e.KeyCode == System.Windows.Forms.Keys.O && Commands.OpenFolderCanExecute)
            {
                Commands.OpenFolderExecuted();
            }
            else if (!e.Shift && e.Control && e.KeyCode == System.Windows.Forms.Keys.S && Commands.SaveCanExecute)
            {
                Commands.SaveExecuted();
            }
            else if (e.Control && e.Shift && e.KeyCode == System.Windows.Forms.Keys.S && Commands.SaveAsCanExecute)
            {
                Commands.SaveAsExecuted();
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.W && Commands.CloseAllFilesCanExecute)
            {
                Commands.CloseFileExecuted();
            }
            else if (!e.Shift && e.Control && e.KeyCode == System.Windows.Forms.Keys.T && Commands.NewTerminalCanExecute)
            {
                Commands.NewTerminalExecuted();
            }
            else if (e.Control && e.Shift && e.KeyCode == System.Windows.Forms.Keys.T && Commands.NewTerminalCurrentDirCanExecute)
            {
                Commands.NewTerminalCurrentDirExecuted();
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.B && Commands.BuildCanExecute)
            {
                Commands.BuildExecuted();
            }
            else if (!e.Shift && e.Control && e.Shift && e.KeyCode == System.Windows.Forms.Keys.B && Commands.BuildAndRunCanExecute)
            {
                Commands.BuildAndRunExecuted();
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.F && Commands.FindCanExecute)
            {
                Commands.FindExecuted();
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.H && Commands.ReplaceCanExecute)
            {
                Commands.ReplaceExecuted();
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.Z && Commands.UndoCanExecute)
            {
                Commands.UndoExecuted();
            }
            else if(e.Control&&e.KeyCode==System.Windows.Forms.Keys.Y&&Commands.RedoCanExecute)
            {
                Commands.RedoExecuted();
            }
        }
        private int loadedLine=200;
        private void richTextBox_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            { 
                UpdateLineNumber();
                mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].TabItem.textBox.Font = new Font(richTextBox.Font.FontFamily, richTextBox.ZoomFactor * richTextBox.Font.Size);
                mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].TabItem.textBox.Width = getWidth();
            }
            if (lazyload == true)
            {
                {
                    if (e.Delta>0)
                    {
                        var lines = System.IO.File.ReadLines(mainWindow.tabItems[mainWindow.tabControl.SelectedIndex].FilePath).Skip(loadedLine).Take(200).ToArray();
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (string line in lines)
                        {
                            richTextBox.Text += line+"\n";
                            //richTextBox.Text.Insert(richTextBox.TextLength - 1, line+"\n");
                            richTextBox.SelectionStart += line.Length;
                        }
                        loadedLine += 200;
                    }
                }
            }
            else
                lazyload = false;
        }

        //declare  isCurslyBracesKeyPressed variable as Boolean and assign false value  
        //to check { key is pressed or not  
        public static Boolean isCurslyBracesKeyPressed = false;

        //richTextBox1 KeyPress events  

        // if key (,{,<,",',[ is pressed then insert opposite key to richTextBox1 at Position SelectionStart+1  
        // add one line after inserting, e.Handled=true;  
        //finally set SelectionStart to specified position  


        private void richTextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            String s = e.KeyChar.ToString();
            int sel = richTextBox.SelectionStart;
            
            switch (s)
            {
                case "(":
                    richTextBox.Text = richTextBox.Text.Insert(sel, "()");
                    e.Handled = true;
                    richTextBox.SelectionStart = sel + 1;
                    break;

                case "{":
                    richTextBox.Text = richTextBox.Text.Insert(sel, "{}");
                    e.Handled = true;
                    richTextBox.SelectionStart = sel + 1;
                    isCurslyBracesKeyPressed = true;
                    break;

                case "[":
                    richTextBox.Text = richTextBox.Text.Insert(sel, "[]");
                    e.Handled = true;
                    richTextBox.SelectionStart = sel + 1;
                    break;

                case "\"":
                    richTextBox.Text = richTextBox.Text.Insert(sel, "\"\"");
                    e.Handled = true;
                    richTextBox.SelectionStart = sel + 1;
                    break;

                case "'":
                    richTextBox.Text = richTextBox.Text.Insert(sel, "''");
                    e.Handled = true;
                    richTextBox.SelectionStart = sel + 1;
                    break;
                    
            }
            InvokeHighlight();
        }


        // richTextBox1 Key Down event  
        /// <summary>
        /// Check if user press enter between Cursly Bracket, a line break will be insert 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void richTextBox_AutoBracketKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            /// when key  {  is pressed and {} is inserted in richTextBox 
            /// and isCurslyBracesKeyPressed is true then insert some blank text to richTextBox1
            /// it will look like this when Enter key is down
            /// 
            int sel = richTextBox.SelectionStart;
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                if (isCurslyBracesKeyPressed == true)
                {
                    richTextBox.Text = richTextBox.Text.Insert(sel, "\n\t\n");
                    e.Handled = true;
                    richTextBox.SelectionStart = sel + "\t\t".Length;
                    //sel = richTextBox.SelectionStart;
                    //richTextBox.SelectionStart += "\n".Length;
                    //richTextBox.SelectionStart = sel;
                    isCurslyBracesKeyPressed = false;
                }
            }
        }
        private void richTextBox_AutoIndentLine(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //if (e.KeyCode == System.Windows.Forms.Keys.Enter || e.KeyCode == System.Windows.Forms.Keys.Return)
            //{
            //    int previousLineNumber = richTextBox.GetLineFromCharIndex(richTextBox.SelectionStart) - 1;
            //    if (previousLineNumber < 0 || previousLineNumber > richTextBox.Lines.Count())
            //        return;

            //    //get previous line
            //    string previousLine = richTextBox.Lines[previousLineNumber];

            //    //get the amount of indent of previous line
            //    //read more about regex here: https://autohotkey.com/docs/misc/RegEx-QuickRef.htm
            //    Match indent = Regex.Match(previousLine, @"^[ \t]*");

            //    richTextBox.SelectedText = indent.Value;
            //    if (Regex.IsMatch(richTextBox.Lines[richTextBox.GetLineFromCharIndex(richTextBox.SelectionStart)], @"}\s*"))
            //    {
            //        richTextBox.SelectedText = "\t\n" + indent.Value;
            //        richTextBox.SelectionStart = richTextBox.GetFirstCharIndexOfCurrentLine() - 1;
            //    }
            //}
        }
    }
}
