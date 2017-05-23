using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OutpostOmega.Game;
using OutpostOmega.Game.Lua;
using OutpostOmega.Data;
using System.IO;
using System.Security.Cryptography;

namespace OutpostOmega.ModMaker
{
    public partial class Main : Form
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

                Navigation.LoadedMod = value;

                AppSettings.Default.LastMod = _LoadedMod.ConfigFile.FullName;
                AppSettings.Default.Save();
            }
        }
        private ModPack _LoadedMod;

        public Main()
        {
            InitializeComponent();

            Navigation.tV_Navigation.NodeMouseDoubleClick += tV_Navigation_NodeMouseDoubleClick;
            tC_Editor.ImageList = Navigation.tV_Navigation.ImageList;

            if(AppSettings.Default.LastMod != "" &&
                File.Exists(AppSettings.Default.LastMod))
            {
                LoadedMod = new ModPack(new FileInfo(AppSettings.Default.LastMod));    
            }
        }
        
        private void tsmi_file_mod_load_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Mod Description (*.xml)|*.xml|All files (*.*)|*.*";
            ofd.Multiselect = false;
            if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var fileInfo = new FileInfo(ofd.FileName);
                LoadedMod = new ModPack(fileInfo);
            }            
        }

        private void tC_Editor_MouseMove(object sender, MouseEventArgs e)
        {
            bool Anyhit = false;
            for (int i = 0; i < tC_Editor.TabCount; i++)
            {
                var tabRectangle = tC_Editor.GetTabRect(i);
                if (new Rectangle(new Point(tabRectangle.X + 3, tabRectangle.Y + 3), new Size(18, tabRectangle.Height - 6)).Contains(e.Location))
                {
                    if (tC_Editor.TabPages[i].ImageIndex != 10) tC_Editor.TabPages[i].ImageIndex = 10;
                    Anyhit = true;
                }
                else
                {
                    var node = (TreeNode)tC_Editor.TabPages[i].Tag;
                    if (tC_Editor.TabPages[i].ImageIndex != node.ImageIndex) tC_Editor.TabPages[i].ImageIndex = node.ImageIndex;
                }
            }
            if (Anyhit)
                Cursor.Current = Cursors.Hand;
            else
                Cursor.Current = Cursors.Default;
        }

        private void tC_Editor_MouseDown(object sender, MouseEventArgs e)
        {
            for(int i = 0; i < this.tC_Editor.TabPages.Count; i++)
            {
                Rectangle r = tC_Editor.GetTabRect(i);
                Rectangle closeButton = new Rectangle(r.Left + 3, r.Top + 3, 18, r.Height - 6);
                if(closeButton.Contains(e.Location))
                {
                    CloseTab(i);
                    break;
                }
            }
        }

        private void CloseTab(int TabIndex)
        {
            var TabPage = tC_Editor.TabPages[TabIndex];
            
            var editor = (from Control control in TabPage.Controls
                            where control.GetType() == typeof(CodeEditor)
                            select (CodeEditor)control).SingleOrDefault();
            if (editor != null)
            {
                if (!editor.Save())
                    return;
            }

            this.tC_Editor.TabPages.RemoveAt(TabIndex);
            TabPage.Dispose();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newModDialog = new Dialog.NewMod();
            if(newModDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadedMod = newModDialog.NewModPack;
            }
        }

        
        private void TMSI_Navigation_New_Script_Click(object sender, EventArgs e)
        {
            DirectoryInfo TargetFolder = Navigation.TargetFolder;

            if (TargetFolder != null && TargetFolder.Exists)
            {
                Functions.NewScript(LoadedMod, TargetFolder);
                Navigation.RefreshTree();
            }
        }

        private void tV_Navigation_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                bool contains = (from TabPage tab in tC_Editor.TabPages
                                 where tab.Text == e.Node.Text
                                 select true).SingleOrDefault();
                if (!contains)
                {
                    var tabPage = new TabPage(e.Node.Text);
                    tabPage.ImageIndex = e.Node.ImageIndex;
                    tabPage.Tag = e.Node;


                    string fPath = "";
                    if (e.Node.Tag.GetType() == typeof(ModPack.ModScriptFile)) // CODE
                    {
                        var scriptFile = (ModPack.ModScriptFile)e.Node.Tag;
                        fPath = scriptFile.File.FullName;

                        var scEditor = new CodeEditor(scriptFile);
                        scEditor.Parent = tabPage;
                        scEditor.Dock = DockStyle.Fill;
                        scEditor.BorderStyle = BorderStyle.None;
                    }
                    else if (e.Node.Tag.GetType() == typeof(ModPack.ModContentFile)) // CONTENT
                    {
                        var contentFile = ((ModPack.ModContentFile)e.Node.Tag);
                        fPath = contentFile.File.FullName;

                        switch (contentFile.Importer)
                        {
                            case ModPack.ContentImporter.Texture2D:
                                var picBox = new PictureBox();
                                picBox.Parent = tabPage;
                                picBox.Dock = DockStyle.Fill;
                                picBox.Image = Image.FromFile(fPath);
                                break;
                        }



                    }
                    tC_Editor.TabPages.Add(tabPage);
                }
            }
        }

        private void functionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new Dialog.CodeHelp()).Show();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var error = LoadedMod.Save();
        }
    }
}
