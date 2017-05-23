using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using System.IO;
using System.Threading;

namespace OutpostOmega.Drawing.UI
{
    class ToolBar : DockBase
    {
        Scenes.Game GameScene;
        Label buttonLabel;
        Base[] Containers = new Base[10];
        OutpostOmega.Game.GameObjects.Mob PlayerMob;

        int size = 64;
        int subcontsize = 64 - 16;

        public ToolBar(Scenes.Game GameScene, Base parent)
            : base(parent)
        {
            this.GameScene = GameScene;
            PlayerMob = GameScene.World.Player.Mob;
            PlayerMob.QuickslotChanged += PlayerMob_QuickslotChanged;
            PlayerMob.QuickslotSelectedChanged += PlayerMob_QuickslotSelectedChanged;
            this.SetSize(200, 200);
            this.Dock = Pos.Bottom;
            //this.IsClosable = false;
            
            int overallWidth = (size + 12) * 9;
            int startPos = this.Parent.Width / 2 - overallWidth / 2;

            int posx = startPos;


            buttonLabel = new Label(this);
            for(int i = 0; i < 9; i++)
            {
                var cont = new ImagePanel(this);
                cont.Name = (i+1).ToString();
                cont.ImageName = @"Content\Image\ToolBG.png";
                cont.Width = size;
                cont.Height = size;
                cont.SetPosition(posx, this.Height - cont.Height);
                cont.Clicked += cont_Clicked;
                //cont.HoverEnter += HoverEnter;
                //cont.HoverLeave += HoverLeave;
                posx += size + 12;

                var subcont = new ImagePanel(cont);
                cont.UserData = subcont;
                subcont.ImageName = @"Content\Image\ToolEmpty.png";
                subcont.Name = "Empty";
                subcont.Width = subcontsize;
                subcont.Height = subcontsize;
                subcont.X = (size - subcontsize) / 2;
                subcont.Y = (size - subcontsize) / 2;
                subcont.MouseInputEnabled = false;
                subcont.KeyboardInputEnabled = false;

                Label name = new Label(cont);
                name.Font.Size = 8;
                name.Text = (i+1).ToString();
                name.SetPosition(subcont.X, subcont.Y);
                name.MouseInputEnabled = false;
                name.KeyboardInputEnabled = false;

                Containers[i] = cont;

                //if (i == PlayerMob.SelectedQuickslot && PlayerMob.Quickslot[PlayerMob.SelectedQuickslot] != null)
                //    Highlight(cont);
            }
            
            for (int i = 0; i < 9; i++)
            {
                if(PlayerMob.Quickslot[i] != null)
                    PlayerMob_QuickslotChanged(PlayerMob.Quickslot[i], i, true);


                UnHighlight(Containers[i]);

            }

            Highlight(Containers[0]);
            //buttonLabel.Width = 200;

            //exit_Pressed(null, null);
        }

        protected override void OnBoundsChanged(System.Drawing.Rectangle oldBounds)
        {
            int overallWidth = (size + 12) * 9;
            int startPos = this.Parent.Width / 2 - overallWidth / 2;
            int posx = startPos;

            for (int i = 0; i < 9; i++)
            {
                if (Containers[i] == null) continue;
                var cont = (ImagePanel)Containers[i];
                cont.SetPosition(posx, this.Height - size);
                posx += size + 12;
            }

            base.OnBoundsChanged(oldBounds);
        }

        void PlayerMob_QuickslotSelectedChanged(int oldIndex, int Index)
        {
            UnHighlight(Containers[oldIndex]);
            Highlight(Containers[Index]);
        }

        void PlayerMob_QuickslotChanged(OutpostOmega.Game.GameObjects.Item item, int index, bool Added)
        {
            if (Added)
            {
                if(item.GetType().GetCustomAttributes(typeof(OutpostOmega.Game.GameObjects.Attributes.IconAttribute), true).Length > 0)
                    ((ImagePanel)Containers[index].UserData).ImageName =
                        ((OutpostOmega.Game.GameObjects.Attributes.IconAttribute)item.GetType().GetCustomAttributes(typeof(OutpostOmega.Game.GameObjects.Attributes.IconAttribute), true)[0]).Path;
                else
                    ((ImagePanel)Containers[index].UserData).ImageName = @"Content\Image\ToolUnknown.png";
                ((ImagePanel)Containers[index].UserData).Name = item.ID;
                
                Highlight(Containers[index]);
            }
            else
            {
                ((ImagePanel)Containers[index].UserData).ImageName = @"Content\Image\ToolEmpty.png";
                ((ImagePanel)Containers[index].UserData).Name = "Empty";
                UnHighlight(Containers[index]);
            }
        }

        void cont_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var iSender = (ImagePanel)sender;
            PlayerMob.SelectedQuickslot = int.Parse(iSender.Name) - 1;
            ItemFunctions(PlayerMob.Quickslot[PlayerMob.SelectedQuickslot]);
        }


        int hovamount = 10;
        new void UnHighlight(Base sender)
        {
            var imgPanel = (ImagePanel)sender;
            if (imgPanel.Width == size) return; // not highlighted

            imgPanel.Width = imgPanel.Width - hovamount;
            imgPanel.X = imgPanel.X + hovamount;

            imgPanel.Height = imgPanel.Height - hovamount;
            imgPanel.Y = imgPanel.Y + hovamount / 2;

            buttonLabel.Hide();
            //imgPanel.Height = imgPanel.Height - 5;
            //imgPanel.PaddingOutlineColor = System.Drawing.Color.Transparent;
            //imgPanel.UpdateColors();
        }

        new void Highlight(Base sender)
        {
            var imgPanel = (ImagePanel)sender;
            if (imgPanel.Width > size) return; // already highlighted

            imgPanel.Width = imgPanel.Width + hovamount;
            imgPanel.X = imgPanel.X - hovamount;

            imgPanel.Height = imgPanel.Height + hovamount;
            imgPanel.Y = imgPanel.Y - hovamount / 2;

            buttonLabel.Show();
            buttonLabel.Text = ((ImagePanel)imgPanel.UserData).Name.ToString();
            buttonLabel.SetPosition(imgPanel.X + (imgPanel.Width / 2) - (buttonLabel.Width / 2), imgPanel.Y - buttonLabel.Height - 10);
            //imgPanel.PaddingOutlineColor = System.Drawing.Color.Red;
            //imgPanel.UpdateColors();
        }

        void save_Clicked(Base sender, ClickedEventArgs arguments)
        {
            GameStarter.Save(GameScene.Game.SceneManager, this.Parent, GameScene.World, new FileInfo("Saves/Autosave.sav"));

            //EnqueueSaving();
            //new MessageBox(this.Parent, "Game saved!", "Save").Show();
        }

        bool WorldSaved = false;
        public void EnqueueSaving()
        {
            WorldSaved = false;
            if (loadScreen == null)
                loadScreen = new LoadingScreen(this.GameScene, this.Parent);
            loadScreen.Message = "Saving current world";
            loadScreen.Show();

            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveWorld));
        }
        LoadingScreen loadScreen;
        public void SaveWorld(object state)
        {
            if (GameScene.World != null)
            {
                GameScene.Stop = true;
                FileInfo save = new FileInfo("Autosave.sav");
                OutpostOmega.Data.DataHandler.SaveToFile(GameScene.World, save, false);
                //loadScreen.Hide();
                WorldSaved = true;
                GameScene.Stop = false;
            }
        }

        void reset_Clicked(Base sender, ClickedEventArgs arguments)
        {
            GameScene.World.Player.Mob.SetPosition(new Jitter.LinearMath.JVector(0, 20, 0));
            GameScene.World.Player.Mob.charController.Position = GameScene.World.Player.Mob.Position;
            //game.dWorld.World.Player.Mob._RigidBody.LinearVelocity = Jitter.LinearMath.JVector.Zero;
        }

        bool Exit = false;
        void exit_Pressed(Base sender, EventArgs arguments)
        {
            if (AppSettings.Default.Autosave)
                GameStarter.Save(GameScene.Game.SceneManager, this.Parent, GameScene.World, new FileInfo("Saves/Autosave.sav"));

            Exit = true;
            //GameScene.Dispose();
            //this.Parent.Hide();
        }

        OutpostOmega.Game.GameObjects.Item[] OldQuickslots;
        public override void Think()
        {
            if (Exit)
            {
                GameScene.Game.SceneManager.MakeSceneActive(GameScene.Game.SceneManager.AddScene(new Scenes.Menu(GameScene.Game)));
            }
            else if (WorldSaved)
                WorldSaved = false;
            base.Think();
        }

        private void ItemFunctions(OutpostOmega.Game.GameObjects.Item item)
        {
            if (item == null) return;

            if(item.GetType() == typeof(OutpostOmega.Game.GameObjects.Items.Devices.Spawner))
            {
                var sMenu = new SpawnMenu(GameScene, this.Parent);
                sMenu.Show();
            }
        }
    }
}
