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
    class Multiplayer : WindowControl
    {
        Scene Scene;
        Button connect;
        TextBox adress;
        TextBox username;
        //ListBox output;
        public Multiplayer(Scene Scene, Base parent)
            : base(parent, "Multiplayer")
        {
            this.Scene = Scene;
            this.SetSkin(Scene.Skin, true);

            this.SetSize(230, 190);
            this.Position(Pos.Center);
            int posY = 20;

            /*output = new ListBox(this);
            output.SetPosition(5, posY);
            output.Width = 200;
            output.Height = 100;*/
            //posY += 105;


            Label username_l = new Label(this);
            username_l.SetPosition(10, posY);
            username_l.Text = "Username";
            posY += 20;

            username = new TextBox(this);
            username.SetPosition(10, posY);
            username.Width = 200;
            posY += 30;


            Label adress_l = new Label(this);
            adress_l.SetPosition(10, posY);
            adress_l.Text = "Adress";
            posY += 20;

            adress = new TextBox(this);
            adress.SetPosition(10, posY);
            adress.Width = 200;
            adress.Text = "server.maxkhl.com:12041";
            posY += 30;

            connect = new Button(this);
            connect.SetPosition(10, posY);
            connect.SetText("Connect");
            connect.IsDisabled = false;
            connect.Clicked += connect_Clicked;
            posY += 30;

            username.Text = AppSettings.Default.LastUsername;
            //adress.Text = "gameserver.outpost-omega.com:12041";
            adress.Text = AppSettings.Default.LastServer;
        }

        /*public override void Think()
        {
            if(Client != null)
                while(Client.Output.Count > 0)
                {
                    string message = "";
                    if (Client.Output.TryDequeue(out message))
                        output.AddRow(message);
                }
            base.Think();
        }*/

        float LastDequeue = 0;
        public override void Update(float ElapsedTime)
        {
            string message = "";
            LastDequeue += ElapsedTime;

            if (loadingScreen != null && LastDequeue > 1000 && Client.Output.TryDequeue(out message))
            {
                loadingScreen.Message = message;
                LastDequeue = 0;
            }

            if(newWorld != null)
            {
                int handler = Scene.Game.SceneManager.AddScene(new Scenes.NetworkGame(Client, Scene.Game, newWorld));
                Scene.Game.SceneManager.ScheduleSceneChange(handler);
                newWorld = null;
                this.Close();
            }

            base.Update(ElapsedTime);
        }

        OutpostOmega.Network.nClient Client;

        private LoadingScreen loadingScreen;
        void connect_Clicked(Base sender, ClickedEventArgs arguments)
        {
            loadingScreen = LoadingScreen.Start(Scene, Scene.Canvas, "Connecting...");
            Client = new OutpostOmega.Network.nClient(username.Text);
            Client.NewWorldReceived += client_NewWorldReceived;
            Client.Disconnected += Client_Disconnected;

            if (adress.Text.Contains(":"))
                Client.Connect(adress.Text.Split(':')[0], int.Parse(adress.Text.Split(':')[1]));
            else
                Client.Connect(adress.Text, OutpostOmega.Network.Specifications.DefaultPort);

            AppSettings.Default.LastServer = adress.Text;
            AppSettings.Default.LastUsername = username.Text;
            AppSettings.Default.Save();

        }

        void Client_Disconnected(Network.nClient sender, string reason)
        {
            if (loadingScreen != null)
            {
                loadingScreen.Parent.RemoveChild(loadingScreen, true);
                loadingScreen = null;
            }
            new MessageBox(this, string.Format("Disconnected with reason '{0}'", reason));
            Client.Dispose();
        }

        OutpostOmega.Game.World newWorld;
        void client_NewWorldReceived(OutpostOmega.Game.World oldWorld, OutpostOmega.Game.World newWorld)
        {
            Client.Disconnected -= Client_Disconnected;
            this.newWorld = newWorld;

            // Could have gotten disconnect event meanwhile
            if (loadingScreen != null)
            {
                loadingScreen.Parent.RemoveChild(loadingScreen, true);
                loadingScreen = null;
            }
        }
    }
}
