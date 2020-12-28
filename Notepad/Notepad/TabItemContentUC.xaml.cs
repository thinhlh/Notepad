using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Notepad.Classes;
namespace Notepad
{
    /// <summary>
    /// Interaction logic for TabItemContentUC.xaml
    /// </summary>
    public partial class TabItemContentUC : UserControl
    {
        private MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        private int lineNumber;


        public static readonly DependencyProperty DataProperty = 
            DependencyProperty.Register(
                "RichTextBoxData", 
                typeof(string), 
                typeof(TabItemContentUC),
                new UIPropertyMetadata(""));
       
        public string Data
        {
            get { return new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text; }
            set { 
                richTextBox.Document.Blocks.Clear();
                richTextBox.Document.Blocks.Add(new Paragraph(new Run(value)));
                richTextBox.ScrollToEnd();
                richTextBox.CaretPosition = richTextBox.Document.ContentEnd;
            }
        }
        public TabItemContentUC()
        {
            InitializeComponent();
            DataContext = this;

            richTextBox.Document.PageWidth = 5000; //disable word wrap if line length <10000
            Data = mainWindow.tabItems[mainWindow.tabItems.Count - 1].Data;
        }

        [Obsolete]
        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<MainTabItem> tabItems = mainWindow.tabItems;
            TabControl tabControl = mainWindow.tabControl;

            //FormattedText ft = new FormattedText(
            //    Data,
            //    System.Globalization.CultureInfo.CurrentCulture,
            //    FlowDirection.LeftToRight,
            //    new Typeface(richTextBox.FontFamily, richTextBox.FontStyle, richTextBox.FontWeight, richTextBox.FontStretch),
            //    richTextBox.FontSize,
            //    Brushes.Black);
            //richTextBox.Document.PageWidth = ft.Width + richTextBox.FontSize;

            if (tabControl.SelectedIndex < 0) //Initialize
                tabItems[tabItems.Count - 1].Data = Data;
            else
                tabItems[tabControl.SelectedIndex].Data = Data;

            AddSavedIcon();
        }

        private void AddSavedIcon()
        {
            List<MainTabItem> tabItems = mainWindow.tabItems;
            TabControl tabControl = mainWindow.tabControl;

            if (tabItems[tabControl.SelectedIndex].IsSaved==false)
            {
                return;
            }
            else
            {
                if (Data == "" || Data == "\r\n") //Initialize Circumstance
                    return;
                else
                {
                    //Raise* at the end and keep isSaved = false when there is a change with out save before
                    tabItems[tabControl.SelectedIndex].Header += "*";
                    tabItems[tabControl.SelectedIndex].IsSaved = false;
                } 
            }
               
        }


        public void LineNumberOnChanged(object sender, TextChangedEventArgs e)
        {
            LineNumberChanged();
        }

        private void LineNumberChanged()
        {
            lineNumber = CountLineNumber();
            string strLine = "";
            for (int i = 1; i <= lineNumber; i++)
            {
                strLine += i.ToString() + "\n";
            }
            textBox.Text = strLine;
        }

        private int CountLineNumber()
        {
            string strtext = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
            var textArr = strtext.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            lineNumber = textArr.Length - 1;
            return lineNumber;
        }

        public void Zoom(object sender,MouseWheelEventArgs e)
        {
            List<MainTabItem> tabItems = mainWindow.tabItems;
            TabControl tabControl = mainWindow.tabControl;
            if (Keyboard.Modifiers != ModifierKeys.Control)
            {
                return;
            }
            else if (e.Delta > 0)
            {
                if (textBox.FontSize+5>50 || tabItems.Count == 0)
                    return;
                else
                {
                    textBox.FontSize += 5;
                    richTextBox.FontSize += 5;
                }
            }
            else
            {
                if (textBox.FontSize-5<5||tabItems.Count==0)
                    return;
                else
                {
                    textBox.FontSize -= 5;
                    richTextBox.FontSize -= 5;
                }
            }
            LineNumberChanged();
        }
    }
}
