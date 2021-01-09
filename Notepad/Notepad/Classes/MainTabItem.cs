using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Notepad.Snippets;
namespace Notepad.Classes
{
    public class MainTabItem : TabItem  // each Tab Item is contained here
    {
        private bool _isSaved { get; set; } //Check whether a file is Saved or not
        //private string _data { get; set; } //Data for each File
        private string _filePath { get; set; }  // FilePath for each Tab
        private bool _isPinned { get; set; } //pinned or havent pinned
        private string tempHeader { get; set; } //temporary header

        public string Data
        {
            get => (Content as TabItemContentUC).Data;
            set => (Content as TabItemContentUC).Data = value;
        }
        public string FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }
        public bool IsSaved
        {
            get => _isSaved;
            set => _isSaved = value;
        }
        
        public bool IsPinned
        {
            get => _isPinned;
            set => _isPinned = value;
        }
        public Languages Language
        {
            get => (Content as TabItemContentUC).Language;
            set => (Content as TabItemContentUC).Language = value;
        }

        public TabItemContentUC TabItem
        {
            set => Content = value;
            get => Content as TabItemContentUC;
        }
        public RichTextBoxUC RichTextBox
        {
            set => (Content as TabItemContentUC).richTextBoxUserControl = value;
            get => (Content as TabItemContentUC).richTextBoxUserControl;
        }

        public MainTabItem()
        {
            _isSaved = true;
            _filePath = "";
            this.Content = new TabItemContentUC() { Name = "UC" };

            ContextMenu contextMenu = new ContextMenu();
            
            contextMenu.Items.Add(new MenuItem() { Name = "PinTabMenuItem", IsCheckable = true, Header = "Pin Tab" });
            (contextMenu.Items[0] as MenuItem).Checked += PinTab_Checked;
            (contextMenu.Items[0] as MenuItem).Unchecked += PinTab_Checked;

            this.ContextMenu = contextMenu;
            
            //this.ContextMenu = Application.Current.Resources["ContextMenuTemplate"] as ContextMenu;
            this.MouseRightButtonDown += MainTabItem_MouseRightButtonDown;
        }

        public void ScrollToEnd()
        {
            RichTextBox.richTextBox.SelectionStart = RichTextBox.richTextBox.Text.Length;
            RichTextBox.richTextBox.ScrollToCaret();
        }
        private void PinTab_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuItem).IsChecked)
            {
                tempHeader = Header as string;
                Header = (IsSaved)?"Tab":"Tab*";
                IsPinned = true;

                MainWindowExtension.MovePinnedTab(this);
            }
            else
            {
                if(IsSaved)
                {
                    if (tempHeader.Contains("*")) //unsaved file but then saved
                        Header = tempHeader.Substring(0, tempHeader.Length-1);
                    else //saved file and now saved 
                        Header = tempHeader;
                }
                else
                {
                    if (tempHeader.Contains("*")) //unsaved file and reamin unsaved
                        Header = tempHeader;
                    else
                        Header = tempHeader + "*"; // saved file but unsaved now
                }
                IsPinned = false;
            }
        }
        public void SetDefaultHeader()
        {
            Header = tempHeader;
            IsPinned = false;
        }
        private void MainTabItem_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Focus();
            this.ContextMenu.IsOpen = true;
        }
    }
}
