using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OutpostOmega.Game;
using OutpostOmega.Game.Lua;
using OutpostOmega.Data;
using System.IO;

namespace OutpostOmega.ModMaker
{
    public partial class ModNavigation : UserControl
    {
        public ModPack LoadedMod
        {
            get
            {
                return _LoadedMod;
            }
            set
            {
                _LoadedMod = value;


                if (value != null)
                {
                    var topNode = new TreeNode(_LoadedMod.Name);
                    topNode.Expand();
                    topNode.SelectedImageIndex = 9;
                    topNode.ImageIndex = 9;
                    topNode.Tag = _LoadedMod.ConfigFile.Directory;

                    BuildTree(topNode);

                    tV_Navigation.BeginUpdate();
                    tV_Navigation.Nodes.Clear();
                    tV_Navigation.Nodes.Add(topNode);
                    tV_Navigation.EndUpdate();
                }
            }
        }
        private ModPack _LoadedMod;

        public DirectoryInfo TargetFolder
        {
            get
            {
                /*DirectoryInfo TargetFolder;
                if (tV_Navigation.SelectedNode.Tag.GetType() == typeof(DirectoryInfo))
                {
                    TargetFolder = (DirectoryInfo)tV_Navigation.SelectedNode.Tag;
                }
                else
                {
                    TargetFolder = (DirectoryInfo)tV_Navigation.SelectedNode.Parent.Tag;
                }*/
                return LoadedMod.Folder;

            }
        }

        public ModNavigation()
        {
            InitializeComponent();
        }

        public void RefreshTree()
        {
            tV_Navigation.BeginUpdate();
            tV_Navigation.Nodes.Clear();

            var topNode = new TreeNode(_LoadedMod.Name);
            topNode.Expand();
            topNode.SelectedImageIndex = 9;
            topNode.ImageIndex = 9;
            topNode.Tag = _LoadedMod.ConfigFile.Directory;

            BuildTree(topNode);

            tV_Navigation.Nodes.Add(topNode);

            tV_Navigation.EndUpdate();
        }

        private void tV_Navigation_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitNode = tV_Navigation.GetNodeAt(e.X, e.Y);
                tV_Navigation.SelectedNode = hitNode;

                if (hitNode != null)
                    cMS_Navigation.Show(MousePosition);
            }
        }
        
        private void BuildTree(TreeNode Node)
        {
            List<object> ModContent = new List<object>();

            foreach (var script in LoadedMod.Scripts)
                ModContent.Add(script);
            foreach (var content in LoadedMod.ContentFiles)
                ModContent.Add(content);

            foreach (var content in ModContent)
            {
                string fPath = "";
                if (content.GetType() == typeof(ModPack.ModScriptFile))
                    fPath = ((ModPack.ModScriptFile)content).File.FullName;
                if (content.GetType() == typeof(ModPack.ModContentFile))
                    fPath = ((ModPack.ModContentFile)content).File.FullName;


                var relativePath = DataHandler.GetRelativePath(fPath, LoadedMod.Folder.FullName);
                var PathSteps = relativePath.Split('\\');

                var preNode = Node;
                for (int i = 0; i < PathSteps.Length; i++)
                {
                    var existingNode = (from TreeNode node in preNode.Nodes
                                        where node.Text == PathSteps[i]
                                        select node).SingleOrDefault();
                    if (existingNode != null)
                    {
                        preNode = existingNode;
                    }
                    else
                    {
                        var newNode = new TreeNode(PathSteps[i]);
                        if (i == PathSteps.Length - 1)
                        {
                            newNode.Tag = content;
                            if (content.GetType() == typeof(ModPack.ModScriptFile))
                            {
                                newNode.ImageIndex = 7;
                            }
                            else
                                newNode.ImageIndex = (int)((ModPack.ModContentFile)content).Importer;
                        }
                        else
                        {
                            newNode.ImageIndex = 8;
                            string foldRelativePath = "";
                            for (int c = 0; c <= i; c++)
                            {
                                foldRelativePath += PathSteps[c] + "\\";
                            }
                            newNode.Tag = new DirectoryInfo(Path.Combine(LoadedMod.Folder.FullName, foldRelativePath));
                        }
                        newNode.SelectedImageIndex = newNode.ImageIndex;
                        preNode.Nodes.Add(newNode);
                        preNode = newNode;
                    }
                }
            }
        }

        private void TMSI_Navigation_New_Script_Click(object sender, EventArgs e)
        {
            if (TargetFolder != null && TargetFolder.Exists)
            {
                Functions.NewScript(LoadedMod, TargetFolder);
                RefreshTree();
            }
        }
    }
}
