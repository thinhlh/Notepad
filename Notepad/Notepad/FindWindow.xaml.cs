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
        private List<int> tokensFound = new List<int>();
        private int searchIndex = -1;
        System.Drawing.Color findColor = JsonDeserialize.GetColorFromString(ConfigurationManager.AppSettings["find_background_color"]);
        System.Drawing.Color defaultColor = JsonDeserialize.GetColorFromString(ConfigurationManager.AppSettings["background_color"]);
        string previousText = "";
        MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        TabControl tabControl = (Application.Current.MainWindow as MainWindow).tabControl;
        List<MainTabItem> tabItems = (Application.Current.MainWindow as MainWindow).tabItems;
        System.Windows.Forms.RichTextBox richTextBox; //curent RTB
        public FindWindow()
        {
            InitializeComponent();
            richTextBox = (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox;
        }
        private void Find_ButtonClick(object sender, RoutedEventArgs e)
        {
            richTextBox.SelectAll();
            richTextBox.BackColor = defaultColor;
            foreach (int position in FindAll())
            {
                richTextBox.Select(position, textBox.Text.Length);
                richTextBox.SelectionBackColor = findColor;
            }

            
        }

        private void ClearTokenFromPreviousSearch()
        {
            richTextBox.Select(0, richTextBox.TextLength);
            richTextBox.SelectionBackColor = defaultColor;
            richTextBox.SelectionLength = 0;
            tokensFound.Clear();
        }

        private List<int> FindAll()
        {
           //list to hold all the positions
            List<int> tokenPosition = new List<int>();

            int position = -1;
            int searchStart = 0;
            int searchEnd = richTextBox.TextLength;

            // A valid starting index should be specified.
            // if index= -1, the end of search
            while (searchStart < richTextBox.Text.Length)
            {
                // A valid ending index
                // Find the position of search string in RichTextBox
                position = richTextBox.Find(textBox.Text, searchStart, searchEnd, System.Windows.Forms.RichTextBoxFinds.None);

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

        protected override void OnClosing(CancelEventArgs e)
        {

            richTextBox.SelectAll();
            richTextBox.SelectionBackColor = defaultColor;

            richTextBox.TextChanged += (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox_Highlight;
            richTextBox.TextChanged += (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox_TextChangedSavedIcon;
            richTextBox.TextChanged += (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.RichTextBox_TextChangedLineNumber;

            base.OnClosing(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            richTextBox.TextChanged -= (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox_Highlight;
            richTextBox.TextChanged -= (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.richTextBox_TextChangedSavedIcon;
            richTextBox.TextChanged -= (tabItems[tabControl.SelectedIndex].Content as TabItemContentUC).richTextBoxUserControl.RichTextBox_TextChangedLineNumber;

        }
    }
}
