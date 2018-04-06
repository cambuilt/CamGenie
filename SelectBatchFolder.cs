using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CamGenie
{
    public partial class SelectBatchFolder : Form
    {
        public string selectedPath = "";
        private bool loading = true;

        public SelectBatchFolder()
        {
            InitializeComponent();

            string folderName;
            string[] projectFolders = Directory.GetDirectories(@"I:");

            foreach (string projectFolder in projectFolders)
            {
                folderName = projectFolder.Substring(projectFolder.LastIndexOf(":") + 1);
                treeViewFolders.Nodes.Add(projectFolder, folderName, 0);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            selectedPath = treeViewFolders.SelectedNode.Name;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            string menuText = selectedPath.Substring(2);

            if (menuText.Split('\\').Length == 3)
                menuText = menuText.Split('\\')[0] + " " + menuText.Split('\\')[1] + "-" + menuText.Split('\\')[2];
            else if (menuText.Split('\\').Length == 2)
                menuText = menuText.Split('\\')[0] + " " + menuText.Split('\\')[1];

            if (Properties.Settings.Default.RecentBatch1 != "" && Properties.Settings.Default.RecentBatch2 == "")
            {
                Properties.Settings.Default.RecentBatch2 = Properties.Settings.Default.RecentBatch1;
            }
            else if (Properties.Settings.Default.RecentBatch3 == "")
            {
                Properties.Settings.Default.RecentBatch3 = Properties.Settings.Default.RecentBatch2;
                Properties.Settings.Default.RecentBatch2 = Properties.Settings.Default.RecentBatch1;
            }

            Properties.Settings.Default.RecentBatch1 = menuText;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void treeViewFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (loading == true)
            {
                loading = false;
                return;
            }

            if (e.Node.Nodes.Count == 0)
            {
                string folderName, subfolderName;
                string[] subfolders = Directory.GetDirectories(e.Node.Name);

                for (int counter = 0; counter < subfolders.Length; counter++)
                {
                    subfolderName = subfolders[counter].Substring(subfolders[counter].LastIndexOf("\\") + 1);

                    if (subfolderName.Length == 2)
                        subfolders[counter] = subfolders[counter].Substring(0, subfolders[counter].LastIndexOf("\\") + 1) + "0" + subfolderName;
                }

                Array.Sort(subfolders);

                foreach (string subfolder in subfolders)
                {
                    folderName = subfolder.Substring(subfolder.LastIndexOf("\\") + 1);

                    if (folderName.StartsWith("0"))
                        folderName = folderName.Substring(1, 2);

                    if (subfolder.Split('\\').Length == 3)
                    {
                        subfolderName = subfolder.Substring(0, subfolder.LastIndexOf("\\") + 1) + subfolder.Split('\\')[2].Substring(1, 2);

                        if (Directory.GetFiles(subfolderName, "*.tif").Length > 0)
                        {
                            e.Node.Nodes.Add(subfolderName, folderName, 0);
                        }
                    }
                    else
                    { 
                        e.Node.Nodes.Add(subfolder.Replace("\\0", "\\"), folderName, 0); 
                    }
                    
                }
                e.Node.Expand();
            }
        }
    }
}
