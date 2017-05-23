using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using OpenTK;
using System.IO;

namespace OutpostOmega.Drawing.UI
{
    class Load : WindowControl
    {
        Button load;

        /// <summary>
        /// Base directory for savegames
        /// </summary>
        DirectoryInfo BaseDir;

        /// <summary>
        /// Local directory path for navigation
        /// </summary>
        string DirPath
        {
            get
            {
                return adressBox.Text;
            }
            set
            {
                adressBox.Text = value;
            }
        }

        TextBox adressBox;
        ListBox listBox;

        Button okButton;

        Scene Scene;

        public Load(Scene Scene, Base parent)
            : base(parent, "Load Game")
        {
            this.Scene = Scene;
            this.SetSize(400, 300);
            this.Position(Pos.Center);

            var adressBase = new Base(this)
            {
                Dock = Pos.Top,
                Height = 30,
            };

            adressBox = new Gwen.Control.TextBox(adressBase) 
            { 
                IsDisabled = true,
                Dock = Pos.Fill
            };
            var adressBackButton = new Gwen.Control.Button(adressBase)
            {
                Dock = Pos.Right,
                Text = "Back"
            };
            adressBackButton.Clicked += adressBackButton_Clicked;


            listBox = new Gwen.Control.ListBox(this)
            {
                Dock = Pos.Fill
            };
            listBox.RowSelected += listBox_RowSelected;
            listBox.DoubleClicked += listBox_DoubleClicked;

            okButton = new Gwen.Control.Button(this)
            {
                Dock = Pos.Bottom,
                IsHidden = true
            };
            okButton.Clicked += okButton_Clicked;

            BaseDir = new DirectoryInfo("Save");
            if (!BaseDir.Exists) BaseDir.Create();

            loadFiles();
        }

        void listBox_DoubleClicked(Base sender, ClickedEventArgs arguments)
        {
            throw new NotImplementedException();
        }

        void okButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (listBox.SelectedRow == null) return;

            if (listBox.SelectedRow.UserData.GetType() == typeof(DirectoryInfo))
            {
                DirPath += "\\" + ((DirectoryInfo)listBox.SelectedRow.UserData).Name;
                loadFiles();
            }
            else if (listBox.SelectedRow.UserData.GetType() == typeof(FileInfo))
            {
                GameStarter.Load(this.Scene.Game.SceneManager, this.Scene.Canvas, (FileInfo)listBox.SelectedRow.UserData);
                //OutpostOmega.Data.DataHandler.LoadWorldFromFile((FileInfo)listBox.SelectedRow.UserData);
            }
        }

        void adressBackButton_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (DirPath.Contains('\\'))
            {
                DirPath = DirPath.Substring(0, DirPath.LastIndexOf('\\'));
                loadFiles();
            }
        }

        void listBox_RowSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            ListBox lSender = (ListBox)sender;

            if(lSender.SelectedRow == null)
            {
                okButton.IsHidden = true;
                return;
            }

            if (lSender.SelectedRow.UserData.GetType() == typeof(DirectoryInfo))
            {
                okButton.Text = "Open Folder";
                okButton.IsHidden = false;
            }
            else if (lSender.SelectedRow.UserData.GetType() == typeof(FileInfo))
            {
                var fileName = ((FileInfo)lSender.SelectedRow.UserData).Name;
                okButton.Text = string.Format("Load {0}", fileName);
                okButton.IsHidden = false;
            }
        }

        private void loadFiles()
        {
            var targetDir = new DirectoryInfo(BaseDir.FullName + "\\" + DirPath);
            if(!targetDir.Exists) return;

            var saveFiles = targetDir.GetFiles("*.sav");
            var subDirs = targetDir.GetDirectories();

            listBox.Clear();

            // Add sub directories
            foreach (var dir in subDirs)
                listBox.AddRow(dir.Name, dir.Name, dir);

            foreach (var file in saveFiles)
                listBox.AddRow(file.Name, file.Name, file);

            listBox_RowSelected(listBox, null);
        }
    }
}
