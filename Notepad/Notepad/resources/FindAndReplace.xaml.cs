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
using System.Windows.Shapes;
using Notepad.Classes;
namespace Notepad.resources
{
    /// <summary>
    /// Interaction logic for FindOrReplace.xaml
    /// </summary>
    public partial class FindAndReplace : Window
    {
        public FindAndReplace()
        {
            InitializeComponent();
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(FindTextBox.Text)||(Application.Current.MainWindow as MainWindow).tabControl.SelectedIndex<0)
            {
                return;
            }
            else
            {
                MainWindowExtension.Find(FindTextBox.Text);
            }
        }
    }
}
