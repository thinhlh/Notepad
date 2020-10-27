using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notepad
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Notepad());
        }
        /*https://www.thecodingguys.net/blog/creating-a-texteditor-in-csharp-and-visual-basic*/
        /*https://www.youtube.com/watch?v=G6XYDc8tPw0*/
        /*https://tutorialslink.com/Articles/How-to-create-Notepad-in-Windows-Forms-using-Csharp/1505*/
}
}
