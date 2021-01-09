using Notepad.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Notepad
{
    /// <summary>
    /// Interaction logic for FindWindow.xaml
    /// </summary>
    public partial class FindWindow : Window
    {
        #region fields

        private int originalCaret;
        private List<int> tokensFound = new List<int>();
        private int searchIndex = -1;
        private System.Drawing.Color findColor = JsonDeserialize.GetColorFromString(ConfigurationManager.AppSettings["find_background_color"]);
        private System.Drawing.Color defaultColor = JsonDeserialize.GetColorFromString(ConfigurationManager.AppSettings["background_color"]);
        private string previousText = "";
        private MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        private TabControl tabControl = (Application.Current.MainWindow as MainWindow).tabControl;
        private List<MainTabItem> tabItems = (Application.Current.MainWindow as MainWindow).tabItems;
        private System.Windows.Forms.RichTextBox richTextBox; //curent RTB

        private ICommand _replaceCommand;
        public ICommand ReplaceCommand
        {
            get => _replaceCommand ?? (_replaceCommand = new Command(() => ReplaceExecuted(), () => ReplaceCanExecute));
        }
        private ICommand _replaceAllCommand;
        public ICommand ReplaceAllCommand
        {
            get => _replaceAllCommand ?? (_replaceAllCommand = new Command(() => ReplaceAllExecuted(), () => ReplaceAllCanExecute));
        }

        #endregion

        public FindWindow()
        {
            InitializeComponent();
            DataContext = this;

            richTextBox = (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox;
            originalCaret = richTextBox.SelectionStart;

            textBox.Focus();
        }

        private void matchCase_Checked(object sender, RoutedEventArgs e)
        {
            textBox_TextChanged(sender, e as TextChangedEventArgs);
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ///ClearTokenFromPreviousSearch();
            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.UnsubscribeTextChangedEvents();

            richTextBox.SelectAll();
            richTextBox.SelectionBackColor = defaultColor;
            richTextBox.DeselectAll();

            foreach (int position in FindAll((matchCase.IsChecked == false) ? System.Windows.Forms.RichTextBoxFinds.None : System.Windows.Forms.RichTextBoxFinds.MatchCase))
            {
                richTextBox.Select(position, textBox.Text.Length);
                richTextBox.SelectionBackColor = findColor;
            }
            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.SubscribeTextChangedEvents();
        }

        private List<int> FindAll(System.Windows.Forms.RichTextBoxFinds option = System.Windows.Forms.RichTextBoxFinds.None)
        {

            //list to hold all the positions
            List<int> tokenPosition = new List<int>();

            int position = -1;
            int searchStart = 0;
            int searchEnd = richTextBox.TextLength;

            // if index= -1, the end of search
            while (searchStart < richTextBox.Text.Length)
            {
                // A valid ending index
                // Find the position of search string in RichTextBox
                position = richTextBox.Find(textBox.Text, searchStart, searchEnd,option);

                // Determine whether the text was found in richTextBox1.
                if (position != -1)
                {
                    tokenPosition.Add(position);
                    searchStart = position + textBox.Text.Length;
                }
                else
                {
                    break;
                }
            }
            return tokenPosition;
        }

        private void FindNext_ButtonClick(object sender, RoutedEventArgs e)
        {
            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.UnsubscribeTextChangedEvents();

            if (previousText != richTextBox.Text)
            {
                previousText = richTextBox.Text;

                //get this again because we might have changed the text in text area and it made some of the found text position changed
                tokensFound.Clear();
                tokensFound = (matchCase.IsChecked == true) ? FindAll(System.Windows.Forms.RichTextBoxFinds.MatchCase) : FindAll();
            }

            if (tokensFound.Count != 0)
            {
                if (textBox.Text.Length == 0)
                    return;
                searchIndex++;
                if (searchIndex == tokensFound.Count)
                {
                    searchIndex = 0; //return to the start
                }
                richTextBox.Select(tokensFound[searchIndex], textBox.Text.Length);
            }
            richTextBox.Focus();

            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.SubscribeTextChangedEvents();
        }

        private void FindPrevious_ButtonClick(object sender, RoutedEventArgs e)
        {
            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.UnsubscribeTextChangedEvents();

            if (previousText != richTextBox.Text)
            {
                previousText = richTextBox.Text;

                //get this again because we might have changed the text in text area and it made some of the found text position changed
                tokensFound.Clear();
                tokensFound = (matchCase.IsChecked == true) ? FindAll(System.Windows.Forms.RichTextBoxFinds.MatchCase) : FindAll();
            }

            if (tokensFound.Count != 0)
            {
                if (textBox.Text.Length == 0)
                    return;
                searchIndex--;
                if (searchIndex <= -1)
                {
                    searchIndex = tokensFound.Count - 1; //return to the end
                }

                richTextBox.Select(tokensFound[searchIndex], textBox.Text.Length);
            }

            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.SubscribeTextChangedEvents();
        }


        //protected override void OnDeactivated(EventArgs e)
        //{
        //    this.Opacity = 0.5;
        //    richTextBox.Focus();
        //    base.OnDeactivated(e);
        //}
        //protected override void OnActivated(EventArgs e)
        //{
        //    this.Opacity = 1;
        //    base.OnActivated(e);
        //}
        protected override void OnClosing(CancelEventArgs e)
        {
            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.UnsubscribeTextChangedEvents();

            richTextBox.SelectAll();
            richTextBox.SelectionBackColor = defaultColor;
            richTextBox.DeselectAll();
            richTextBox.SelectionStart = originalCaret;

            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.SubscribeTextChangedEvents();

            base.OnClosing(e);
        }



        /// <summary>
        /// Commands Executed and Can Execute
        /// </summary>
        private void ReplaceExecuted()
        {

            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.TextChanged -= (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.RichTextBox_TextChangedLineNumber;

            richTextBox.Focus();
            if (string.IsNullOrEmpty(replaceTextBox.Text) || string.IsNullOrEmpty(textBox.Text)||richTextBox.SelectionLength==0)
            {
                return;
            }

            //get this again because we might have changed the text in text area and it made some of the found text position changed
            tokensFound.Clear();
            tokensFound = (matchCase.IsChecked == true) ? FindAll(System.Windows.Forms.RichTextBoxFinds.MatchCase) : FindAll();
            searchIndex = (searchIndex<0||searchIndex>tokensFound.Count-1)?0:searchIndex;
            if (tokensFound.Count > 0)
            {
                richTextBox.Select(tokensFound[searchIndex], textBox.Text.Length);
                richTextBox.SelectedText = replaceTextBox.Text;

                if (searchIndex == tokensFound.Count-1) searchIndex = 0;
                else searchIndex++;
                richTextBox.Select(tokensFound[searchIndex], replaceTextBox.Text.Length);
            }

            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.TextChanged += (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.RichTextBox_TextChangedLineNumber;

            //textBox_TextChanged(sender, e as TextChangedEventArgs);
        }

        private bool ReplaceCanExecute
        {
            get =>
                 (
                 string.IsNullOrEmpty(replaceTextBox.Text)
                 ||
                 string.IsNullOrEmpty(textBox.Text)
                 ||
                 richTextBox.SelectionLength == 0
                 ||
                 FindAll((matchCase.IsChecked == true) ? System.Windows.Forms.RichTextBoxFinds.MatchCase : System.Windows.Forms.RichTextBoxFinds.None).Count > 0
                 );
        }

        private void ReplaceAllExecuted()
        {
            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.TextChanged -= (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.RichTextBox_TextChangedLineNumber;


            if (textBox.Text==replaceTextBox.Text||string.IsNullOrEmpty(textBox.Text))
            {
                return;
            }
            tokensFound=(matchCase.IsChecked == true) ? FindAll(System.Windows.Forms.RichTextBoxFinds.MatchCase) : FindAll();
            richTextBox.Text=richTextBox.Text.Replace(textBox.Text, replaceTextBox.Text);
            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.InvokeHighlightAll(); //rehighlight if changed
            textBox_TextChanged(null,null);

            (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.TextChanged += (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.RichTextBox_TextChangedLineNumber;
        }

        private bool ReplaceAllCanExecute
        {
            get =>
                 (
                 string.IsNullOrEmpty(replaceTextBox.Text)
                 ||
                 string.IsNullOrEmpty(textBox.Text)
                 ||
                 richTextBox.SelectionLength == 0
                 ||
                 FindAll((matchCase.IsChecked == true) ? System.Windows.Forms.RichTextBoxFinds.MatchCase : System.Windows.Forms.RichTextBoxFinds.None).Count > 0
                 );
        }
    }
}
