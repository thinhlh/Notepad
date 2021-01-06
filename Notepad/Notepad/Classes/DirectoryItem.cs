using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Notepad.Classes
{
    public class DirectoryItem:TreeViewItem
    {
        public List<TreeViewItem> items { get; set; }
        public DirectoryItem()
        {
            items = new List<TreeViewItem>();  
        }
    }
}
