using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notepad.Snippets;
namespace Notepad.Classes
{
    public class FileItem :TreeViewItem
    {

        public Languages language {
            get => MainWindowExtension.GetLanguageFromExtension(Path.GetExtension(path));
        }
        public string IconPath
        {
            get
            {
                switch(Path.GetExtension(path))
                {
                    case ".cs":
                        return @"\resources\CSharphIcon.png";
                    case ".cpp":
                        return @"\resources\CPlusPlusIcon.png";
                    case ".c":
                        return @"\resources\CIcon.png";
                    case ".java":
                        return @"\resources\JavaIcon.png";
                    case ".txt":
                    case ".md":
                        return @"\resources\TextIcon.png";
                    default:
                        return "";
                }
            }
        }
    }
}
