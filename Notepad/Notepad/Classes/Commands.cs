using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
namespace Notepad.Classes
{
    public static class Commands
    {
        /// <summary>
        /// Fields to access easier
        /// </summary>
        private static MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        private static TabControl tabControl = (Application.Current.MainWindow as MainWindow).tabControl;
        private static List<MainTabItem> tabItems = (Application.Current.MainWindow as MainWindow).tabItems;


        /// <summary>
        /// New File Executed and Can Execute
        /// </summary>
        public static void NewFileExecuted()
        {
            MainWindowExtension.InitializeTabItem();
        }
        public static bool NewFileCanExecute
        {
            get => true;
        }

        /// <summary>
        /// Open File Executed and Can Execute
        /// </summary>
        public static void OpenFileExecuted()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Java (*.java)|*.java|C (*.c)|*.c|C++ (*.cpp)|*.cpp|C# (*.cs)|*.cs|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                int indexForTab;
                /* 
                 * Determining which tab the file will be open
                 * If open in new tab => tabItems.Count-1
                 * if open in recent tab => tabControl.selected
                */
                if (tabItems[tabControl.SelectedIndex].FilePath != "" || !string.IsNullOrWhiteSpace(tabItems[tabControl.SelectedIndex].Data))
                {
                    MainWindowExtension.InitializeTabItem();
                    indexForTab = tabItems.Count - 1;
                }
                else 
                    indexForTab = tabControl.SelectedIndex;

                tabItems[indexForTab].Data = System.IO.File.ReadAllText(openFileDialog.FileName); //Read File here

                //Unsubscribe TextChange event
                (tabItems[indexForTab].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.TextChanged -= (tabItems[indexForTab].Content as TabItemContentUC).richTextBoxUserControl.richTextBox_Highlight;

                // Add content to richTextBox
                (tabItems[indexForTab].Content as TabItemContentUC).Data = tabItems[indexForTab].Data;// Set Data For RTB

                //Highlight
                (tabItems[indexForTab].Content as TabItemContentUC).richTextBoxUserControl.highlighter.HighlightRange(0, (tabItems[indexForTab].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.Text.Length);

                //Resubscribe
                (tabItems[indexForTab].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.TextChanged += (tabItems[indexForTab].Content as TabItemContentUC).richTextBoxUserControl.richTextBox_Highlight;



                //Scroll to the end of the text
                (tabItems[indexForTab].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.SelectionStart = (tabItems[indexForTab].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.Text.Length;
                (tabItems[indexForTab].Content as TabItemContentUC).richTextBoxUserControl.richTextBox.ScrollToCaret();

                tabItems[indexForTab].Header = Path.GetFileName(openFileDialog.FileName);
                tabItems[indexForTab].FilePath = openFileDialog.FileName;
                tabItems[indexForTab].IsSaved = true;

                //Update Status Bar
                MainWindowExtension.UpdateStatusBar(indexForTab);
            }
        }
        public static bool OpenFileCanExecute 
        { 
            get => true; 
        }

        /// <summary>
        /// Open Folder Executed and Can Execute
        /// </summary>
        public static void OpenFolderExecuted()
        {

        }
        public static bool OpenFolderCanExecute
        {
            get => false;
        }

        /// <summary>
        /// Save Executed and Can Execute
        /// </summary>
        public static void SaveExecuted()
        {
            MainWindowExtension.SaveExecuted(tabControl.SelectedIndex);
        }
        public static bool SaveCanExecute
        {
            get=> true;
        }

        /// <summary>
        /// Save As Executed and Can Execute
        /// </summary>
        public static void SaveAsExecuted()
        {
            MainWindowExtension.SaveAsExecuted(tabControl.SelectedIndex);
        }
        public static bool SaveAsCanExecute
        {
            get => true;
        }

        /// <summary>
        /// Save All Executed and Can Execute
        /// </summary>
        public static void SaveAllExecuted()
        {
            for (int i = tabControl.Items.Count - 1; i >= 0; i--)
                MainWindowExtension.SaveExecuted(i);
        }
        public static bool SaveAllCanExecute
        {
            get => (tabControl.Items.Count != 0);
        }

        /// <summary>
        /// Close File Executed and Can Execute
        /// </summary>
        public static void CloseFileExecuted()
        {
            MainWindowExtension.CloseFileExecuted(tabControl.SelectedIndex);
        }
        public static bool CloseFileCanExecute
        {
            get => !(tabControl.SelectedIndex < 0);
        }

        /// <summary>
        /// Close All File Executed and Can Execute
        /// </summary>
        public static void CloseAllFilesExecuted()
        {
            for (int i = tabControl.Items.Count - 1; i >= 0; i--)
                MainWindowExtension.CloseFileExecuted(i);
        }
        public static bool CloseAllFilesCanExecute
        {
            get => (tabControl.Items.Count >= 1);
        }

        /// <summary>
        /// New Window Executed and Can Execute
        /// </summary>
        public static void NewWindowExecuted()
        {
            Process.Start("Notepad.exe");
        }
        public static bool NewWindowCanExecute
        {
            get => true;
        }

        /// <summary>
        /// Exit Executed and Can Execute
        /// </summary>
        public static void ExitExecuted()
        {
            //Write JSON to save file path and index here
            if (tabControl.Items.Count >0)
            {
                List<TemporaryDetail> details = new List<TemporaryDetail>();
                for(int i=0;i<tabControl.Items.Count;i++)
                {
                    
                    details.Add(new TemporaryDetail());
                    details[i].path = (tabControl.Items[i] as MainTabItem).FilePath;
                    details[i].header = (tabControl.Items[i] as MainTabItem).Header as string;
                    details[i].text = (tabControl.Items[i] as MainTabItem).Data;

                }
                string JsonPath = MainWindowExtension.TryGetSolutionDirectoryInfo().FullName+ @"\Notepad\temp\TabDetails.json";
                string output = JsonConvert.SerializeObject(details);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(JsonPath,false))
                {
                    file.WriteLine(output);
                    //file.WriteLineAsync(output);
                }

            }
            else
            {
                string JsonPath = MainWindowExtension.TryGetSolutionDirectoryInfo().FullName + @"\Notepad\temp\TabDetails.json";
                File.WriteAllText(JsonPath, string.Empty);
            }
            System.Windows.Application.Current.Shutdown();


        }
        public static bool ExitCanExecute
        {
            get => true;
        }

        /// <summary>
        /// New Terminal Executed and Can Execute
        /// </summary>
        public static void NewTerminalExecuted()
        {
            ProcessStartInfo process = new ProcessStartInfo("cmd.exe");

            process.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            Process.Start(process);
        }
        public static bool NewTerminalCanExecute
        {
            get => true;
        }

        /// <summary>
        /// New Terminal At Current Directory Executed and Can Execute
        /// </summary>
        public static void NewTerminalCurrentDirExecuted()
        {
            ProcessStartInfo process = new ProcessStartInfo("cmd.exe");

            // Get Path of the current Tab then set process.Working Directory to parent to open cmd at working directory    
            var path = MainWindowExtension.GetParentFullPath(tabControl.SelectedIndex);
            process.WorkingDirectory = Directory.GetParent(path).FullName;
            Process.Start(process);
        }
        public static bool NewTerminalCurrentDirCanExecute
        {
            get => tabItems[tabControl.SelectedIndex].IsSaved;
        }

        /// <summary>
        /// Build Executed and Can Execute
        /// </summary>
        public static void BuildExecuted()
        {
            string childFileNameWithExt = Path.GetFileName(tabItems[tabControl.SelectedIndex].FilePath);
            string childFileNameWithoutExt = Path.GetFileNameWithoutExtension(tabItems[tabControl.SelectedIndex].FilePath);
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd");
            Process process = new Process();


            if (tabItems[tabControl.SelectedIndex].IsSaved == false)
            {
                MessageBoxResult result = MessageBox.Show("You need to save before compile, save changes?", "Request", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    MainWindowExtension.SaveExecuted(tabControl.SelectedIndex);
                else
                    return;
            }

            // We can not use normal argument to write in cmd we have to use redirect Standard Input the write in cmd 

            startInfo.UseShellExecute = false; // For redirect Input
            startInfo.WorkingDirectory = MainWindowExtension.GetParentFullPath(tabControl.SelectedIndex);
            startInfo.RedirectStandardInput = true;// Allow to write later
            process.StartInfo = startInfo;
            process.Start();

            process.StandardInput.WriteLine("g++ " + childFileNameWithExt + " -o " + childFileNameWithoutExt);
            process.StandardInput.Flush();
            process.WaitForExit();
        }
        public static bool BuildCanExecute
        {
            get => true;
        }

        /// <summary>
        /// Build And Run Executed and Can Execute
        /// </summary>
        public static void BuildAndRunExecuted()
        {
            Commands.BuildExecuted();
            string childFileNameWithoutExtension = Path.GetFileNameWithoutExtension(tabItems[tabControl.SelectedIndex].FilePath);
            string parentPath = MainWindowExtension.GetParentFullPath(tabControl.SelectedIndex);
            if (
                (tabItems[tabControl.SelectedIndex].IsSaved == false)
                ||
                (System.IO.File.Exists(Path.Combine(parentPath, childFileNameWithoutExtension + ".exe")) == false)
               )
                return; // must be saved and compiled sucessfully

            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.WorkingDirectory = MainWindowExtension.GetParentFullPath(tabControl.SelectedIndex);

            process.Start();
            process.StandardInput.WriteLine(Path.GetFileNameWithoutExtension(tabItems[tabControl.SelectedIndex].FilePath));

            process.WaitForExit();
            System.IO.File.Delete(Path.Combine(parentPath, childFileNameWithoutExtension + ".exe")); //Delete .exe File

        }
        public static bool BuildAndRunCanExecute
        {
            get => true;
        }
        
    }
}
