using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Notepad.Classes
{
    public class Model
    {
        private List<bool> _isSaved = new List<bool>();

        private List<TabItem> _tabItems = new List<TabItem>(); // each Tab Item is contained here

        private List<string> _filePaths = new List<string>(); // FilePath for each Tab

        private List<string> _fileData = new List<string>();//File Data for each File

        public Action<List<object>,int,object> Set = (list,index,value) => list[index]=value;// Delegate Set element use Action because it dont return 

        //return methods
        public bool isSaved(int index) => _isSaved[index];
        public TabItem tabItem(int index) => _tabItems[index];
        public string filePath(int index) => _fileData[index];
        public string fileData(int index) => _fileData[index];
    }
}
