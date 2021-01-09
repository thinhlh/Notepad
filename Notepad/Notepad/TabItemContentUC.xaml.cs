using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Notepad.Classes;
using System.Windows.Forms;
using System.Drawing;
using Notepad.Snippets;

namespace Notepad
{
    /// <summary>
    /// Interaction logic for TabItemContentUC.xaml
    /// </summary>
    public partial class TabItemContentUC : System.Windows.Controls.UserControl
    {
        private MainWindow mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
        public string LineNumber
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }

        public string Data
        {
            get => richTextBoxUserControl.Text;
            set => richTextBoxUserControl.Text = value;
        }
        public Languages Language
        {
            get => richTextBoxUserControl.Language;
            set => richTextBoxUserControl.Language = value;
        }

        
        public TabItemContentUC()
        {
            InitializeComponent();
            DataContext = this;

            LineNumber = 1+"\n";
            
            textBox.Font = new System.Drawing.Font(new FontFamily(MainWindow.appSetting.Get("font_family")), float.Parse(MainWindow.appSetting.Get("font_size")));
            textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            richTextBoxUserControl.BorderThickness = new Thickness(1, 0, 0, 0);
        }

    }
}
