using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Notepad.Classes;
namespace Notepad
{
    /// <summary>
    /// Interaction logic for TabItemContentUC.xaml
    /// </summary>
    public partial class TabItemContentUC : UserControl,INotifyPropertyChanged
    {
        private MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        private string _lineNumber;

        public string LineNumber {
            get => _lineNumber; 
            set {
                _lineNumber = value;
                textBox.Text = value;
                NotifyPropertyChanged(); 
            } 
        }

        public string Data
        {
            get => richTextBoxUserControl.Text;
            set 
            {
                richTextBoxUserControl.Text = value;
            }
        }

        
        public TabItemContentUC()
        {
            InitializeComponent();
            DataContext = this;
            LineNumber = 1.ToString()+"\n";
            textBox.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 14+1.019f, System.Drawing.FontStyle.Regular);
        }


        #region Implement Interface
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
