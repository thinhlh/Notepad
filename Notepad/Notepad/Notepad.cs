using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notepad
{
    
    public partial class Notepad : Form
    {
        private int TabCount = 0;

        public Notepad()
        {
            InitializeComponent();
        }

        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBox textBox = new RichTextBox();
            textBox.Name = "textBox";
            textBox.Dock = DockStyle.Fill;
            textBox.ContextMenu = ContextMenu;

            TabPage NewPage = new TabPage();
            TabCount += 1;

            string DocumentText = "Document " + TabCount; 
            NewPage.Name = DocumentText; 
            NewPage.Text = DocumentText; 
            NewPage.Controls.Add(textBox);
            tabControl1.TabPages.Add(NewPage);


            tabControl1.SelectedTab = NewPage;
        }
    }
}
