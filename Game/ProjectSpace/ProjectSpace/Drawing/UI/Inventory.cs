using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using OpenTK;

namespace OutpostOmega.Drawing.UI
{
    /*class Inventory : Menu
    {
        bool IsGameScene = false;
        ListBox items;
        Label name;
        Label description;
        Button equipt;
        ImagePanel itemViewer;
        Screens.GameObjectViewer gameObjectViewer;

        Scenes.Game gameScene;
        public Inventory(Scene Scene, Base parent)
            : base(Scene, parent, "Inventory")
        {
            this.SetSize(600, 560);

            //Center meh
            this.SetPosition(Scene.Game.Width / 2 - this.Width / 2, Scene.Game.Height / 2 - this.Height / 2);


            IsGameScene = typeof(Scenes.Game) == Scene.GetType() && ((Scenes.Game)Scene).World != null;
                
            if(!IsGameScene)
            {
                new MessageBox(this.Parent, "What the fuack?", "Error");
                return;
            }

            gameScene = (Scenes.Game)Scene;


            int posY = 0;

            items = new ListBox(this);
            items.Dock = Pos.Top;
            items.Height = 200;
            posY += 210;
            items.RowSelected += items_RowSelected;
            RefreshList();
            
            VerticalSplitter splitter = new VerticalSplitter(this);
            splitter.Dock = Pos.Fill;
            splitter.SetHValue(0.1f);

            //ImagePanel screentest = new ImagePanel(this);
            //splitter.SetPanel(0, screentest);
            //screentest.ImageHandle = (int)gameScene.Screens[0].RenderTarget.ColorTexture;

            HorizontalSplitter hsplitter = new HorizontalSplitter(this);
            hsplitter.Dock = Pos.Fill;
            hsplitter.SetVValue(0.2f);

            splitter.SetPanel(1, hsplitter);


            hsplitter.SetPanel(0, items);

            GroupBox group = new GroupBox(this);
            hsplitter.SetPanel(1, group);




            splitter.CenterPanels();

            

            posY = 0;
            Button refresh = new Button(group);
            refresh.Y = posY;
            posY += 25;
            refresh.Text = "Refresh";
            refresh.Clicked += refresh_Clicked;

            itemViewer = new ImagePanel(group);
            itemViewer.SetBounds(0, posY, 200, 150);
            posY += 160;
            gameObjectViewer = new Screens.GameObjectViewer(gameScene);
            gameObjectViewer.Width = 200;
            gameObjectViewer.Height = 150;
            gameScene.BackgroundScreens.Add(gameObjectViewer);


            name = new Label(group);
            name.Y = posY;
            posY += 25;

            description = new Label(group);
            description.Y = posY;
            posY += 25;

            equipt = new Button(group);
            equipt.Y = posY;
            posY += 25;
            equipt.Text = "Equipt";
            equipt.Clicked += equipt_Clicked;
            equipt.IsDisabled = true;
        }

        public override void Dispose()
        {
            gameScene.BackgroundScreens.Remove(gameObjectViewer);
            base.Dispose();
        }

        void equipt_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if(items.SelectedRow == null)
                return;

            if(gameScene.World.Player.Mob.GetType().IsAssignableFrom(typeof(OutpostOmega.Game.GameObjects.Mobs.CarbonBased.Human)))
            {
                var human = (OutpostOmega.Game.GameObjects.Mobs.CarbonBased.Human)gameScene.World.Player.Mob;
                human.RightHand = (OutpostOmega.Game.GameObjects.Item)items.SelectedRow.UserData;
            }
            
        }

        void refresh_Clicked(Base sender, ClickedEventArgs arguments)
        {
            RefreshList();
        }

        void RefreshList()
        {
            if (gameScene.World.Player.Mob.Inventory == null)
                return;

            items.Clear();
            for (int i = 0; i < gameScene.World.Player.Mob.Inventory.Count; i++)
            {
                var item = gameScene.World.Player.Mob.Inventory[i];
                items.AddRow(item.ID, item.ID, item);
            }
        }

        void items_RowSelected(Base sender, ItemSelectedEventArgs arguments)
        {
            var item = (OutpostOmega.Game.GameObjects.Item)items.SelectedRow.UserData;
            var definitions = item.GetType().GetCustomAttributes(typeof(OutpostOmega.Game.GameObjects.Attributes.Definition), false);
            if (definitions != null && definitions.Length > 0)
            {
                var definition = (OutpostOmega.Game.GameObjects.Attributes.Definition)definitions[0];

                name.Text = definition.Name;
                description.Text = definition.Description;
            }

            gameObjectViewer.GameObject = item;
            itemViewer.ImageHandle = (int)gameObjectViewer.RenderTarget.ColorTexture;

            equipt.IsDisabled = false;
        }
    }*/
}
