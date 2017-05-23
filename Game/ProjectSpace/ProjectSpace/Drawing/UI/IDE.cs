using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using System.IO;

namespace OutpostOmega.Drawing.UI
{
    class IDE : Menu
    {
        MultilineTextBox input;
        MultilineTextBox output;
        MenuItem recent;
        Queue<MenuItem> RecentFiles = new Queue<MenuItem>();
        public IDE(Scenes.Game Scene, Base parent)
            : base(Scene, parent, "Console")
        {
            this.SetSize(600, 560);
            this.Position(Pos.Center);

            var mstrip = new MenuStrip(this);
            var Script = mstrip.AddItem("Script");
            var exec = Script.Menu.AddItem("Execute");
            exec.Clicked += exec_Clicked;
            recent = Script.Menu.AddItem("Recent");

            var splitter = new HorizontalSplitter(this);
            splitter.Dock = Pos.Fill;
            splitter.SetVValue(.9f);

            output = new MultilineTextBox(this);
            output.KeyboardInputEnabled = false;
            output.MouseInputEnabled = false;
            splitter.SetPanel(0, output);

            var submit = new Button(this);
            submit.Dock = Pos.Bottom;
            submit.Text = "Execute";
            submit.Pressed += submit_Pressed;

            input = new MultilineTextBox(this);
            splitter.SetPanel(1, input);
        }

        void exec_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "lua files (*.lua)|*.lua|All files (*.*)|*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var fileInfo = new FileInfo(ofd.FileName);

                RunFile(fileInfo);


                if ((from recentFile in RecentFiles where ((FileInfo)recentFile.UserData).FullName == fileInfo.FullName select recentFile).FirstOrDefault() == null)
                {
                    var newFileMenu = recent.Menu.AddItem(fileInfo.Name);
                    newFileMenu.UserData = fileInfo;
                    newFileMenu.Pressed += newFileMenu_Pressed;

                    RecentFiles.Enqueue(newFileMenu);

                    while (RecentFiles.Count > 5)
                    {
                        var deleteMenu = RecentFiles.Dequeue();
                        recent.Menu.RemoveChild(deleteMenu, true);
                    }
                }
            }
        }

        void newFileMenu_Pressed(Base sender, EventArgs arguments)
        {
            RunFile((FileInfo)sender.UserData);
        }

        private void RunFile(FileInfo fileInfo)
        {
            if(fileInfo.Exists)
            {
                try
                {
                    var messageList = ((Scenes.Game)Scene).World.Console.ExecuteFile(fileInfo.FullName);
                    foreach (var message in messageList)
                        output.Text += string.Format("{0}-{1}: {2}\n", message.TimeStamp.ToShortTimeString(), message.Sender, message.Text);
                }
                catch (Exception e)
                {
                    output.Text += e.Message + "\n";
                }
            }
        }

        void submit_Pressed(Base sender, EventArgs arguments)
        {
            var messageList = ((Scenes.Game)Scene).World.Console.Execute(input.Text);
            foreach (var message in messageList)
                output.Text += string.Format("{0}-{1}: {2}\n", message.TimeStamp.ToShortTimeString(), message.Sender, message.Text);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
