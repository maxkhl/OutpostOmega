using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutpostOmega.Server.Dialog
{
    partial class uc_WorldViewer : UserControl
    {
        Main MainForm;
        public uc_WorldViewer(Main MainForm)
        {
            this.MainForm = MainForm;
            InitializeComponent();

            this.MainForm.ActiveWorld.NewGameObject += ActiveWorld_NewGameObject;
            this.MainForm.ActiveWorld.GameObjectRemoved += ActiveWorld_GameObjectRemoved;

            //this.MainForm.ActiveWorld.Structures[0].chunks[0].w
        }

        public delegate void NewGameObjectHandler(Game.GameObject newGameObject);
        private void ActiveWorld_NewGameObject(Game.GameObject newGameObject)
        {
            if (lB_GameObjectDelta.InvokeRequired)
            {
                lB_GameObjectDelta.Invoke(new NewGameObjectHandler(ActiveWorld_NewGameObject), new object[] { newGameObject });
            }
            else
            {
                lB_GameObjectDelta.Items.Add(newGameObject);
            }
        }



        public delegate void GameObjectRemovedHandler(Game.GameObject removedGameObject);
        private void ActiveWorld_GameObjectRemoved(Game.GameObject removedGameObject)
        {
            if (lB_GameObjectDelta.InvokeRequired)
            {
                lB_GameObjectDelta.Invoke(new GameObjectRemovedHandler(ActiveWorld_GameObjectRemoved), new object[] { removedGameObject });
            }
            else
            {
                lB_GameObjectDelta.Items.Remove(removedGameObject);
            }
        }

        private void lB_GameObjectDelta_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(pG_GameObjectDelta.SelectedObject != null)
            {
                var oldGO = (Game.GameObject)pG_GameObjectDelta.SelectedObject;
                oldGO.PropertyChanged -= GO_PropertyChanged;
            }

            var newGO = (Game.GameObject)lB_GameObjectDelta.SelectedItem;
            pG_GameObjectDelta.SelectedObject = newGO;
            newGO.PropertyChanged += GO_PropertyChanged;
        }

        public delegate void GO_PropertyChangedHandler(Game.GameObject Object, string PropertyName, bool IndirectChange);
        private void GO_PropertyChanged(Game.GameObject Object, string PropertyName, bool IndirectChange)
        {
            if (pG_GameObjectDelta.InvokeRequired)
            {
                pG_GameObjectDelta.Invoke(new GO_PropertyChangedHandler(GO_PropertyChanged), new object[] { Object, PropertyName, IndirectChange });
            }
            else
            {
                pG_GameObjectDelta.Refresh();
            }
        }
    }
}
