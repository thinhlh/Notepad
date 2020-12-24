using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Notepad.Classes
{
    public class MainTabItem : TabItem  // each Tab Item is contained here
    {
        private bool _isSaved { get; set; } //Check whether a file is Saved or not
        private string _data { get; set; } //Data for each File
        private string _filePath { get; set; }  // FilePath for each Tab

        public bool IsSaved
        {
            get => _isSaved;
            set => _isSaved = value;
        }

        public string Data
        {
            get => _data;
            set => _data = value;
        }

        public string FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }
        public MainTabItem()
        {
            _isSaved = true;
            _data = "";
            _filePath = "";
        }
    }
}
