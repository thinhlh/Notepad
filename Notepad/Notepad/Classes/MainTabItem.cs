using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Notepad.Classes
{
    public class MainTabItem:TabItem
    {
        private bool _isSaved { get; set; }
        private string _data { get; set; }
        private string _filePath { get; set; }

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
