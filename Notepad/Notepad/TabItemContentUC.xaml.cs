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
        private int _lineNumber;

        public int LineNumber {
            get => _lineNumber; 
            set {
                _lineNumber = value;
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
            LineNumber = 1;
            textBox.Font = new System.Drawing.Font("Consolas", 14+1.109f);
        }

        public void LoadLineNumber()
        {

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
