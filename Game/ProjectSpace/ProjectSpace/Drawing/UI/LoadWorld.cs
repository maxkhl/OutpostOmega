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
    class LoadWorld : Menu
    {
        Button load;
        FileInfo selectedFile;
        public LoadWorld(Scene Scene, Base parent)
            : base(Scene, parent, "Load World")
        {
            this.SetSize(240, 190);

            int posY = 20;

            ListBox files = new ListBox(this);
            files.SetPosition(5, posY);
            files.Width = 200;
            files.Height = 100;
            posY += 105;


            var saveFolder = new DirectoryInfo("Save");
            if (!saveFolder.Exists)
                saveFolder.Create();

            var fileList = saveFolder.GetFiles("*.sav");

            foreach (FileInfo file in fileList)
                files.AddRow(file.Name, "", file);

            files.RowSelected += files_RowSelected;


            load = new Button(this);
            load.SetPosition(5, posY);
            load.SetText("Load File");
            load.IsDisabled = true;
            load.Clicked += load_Clicked;
            posY += 20;
        }

        void load_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var world = OutpostOmega.Data.DataHandler.LoadWorldFromFile(selectedFile);

            if(world != null)
            {
                //Scene.LoadWorld(world);
                Tools.Other.NotImplemented(this);
                this.Close();
            }
            else
            {
                var msgBox = new MessageBox(this, "File seems to be broken", "File load error");
                msgBox.Show();
            }
        }

        void files_RowSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            load.IsDisabled = false;
            selectedFile = (FileInfo)arguments.SelectedItem.UserData;
        }
    }
}
