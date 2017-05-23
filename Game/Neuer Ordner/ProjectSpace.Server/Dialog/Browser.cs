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
using System.Reflection;

namespace OutpostOmega.Server.Dialog
{
    public partial class Browser : Form
    {
        private World world;
        Timer tick;
        public Browser(World world)
        {
            this.world = world;
            InitializeComponent();
            this.Text = string.Format("Worldbrowser for '{0}'", world.ID);
            this.world.GameObjectRemoved += world_GameObjectRemoved;
            this.world.NewGameObject += world_NewGameObject;

            RefreshTree();

        }

        void world_NewGameObject(GameObject newGameObject)
        {
            RefreshInstance();
        }

        void world_GameObjectRemoved(GameObject removedGameObject)
        {
            RefreshInstance();
        }

        private delegate void RefreshInstanceInvoke();
        public void RefreshInstance()
        {
            if(tV_Objects.InvokeRequired)
            {
                tV_Objects.Invoke(new RefreshInstanceInvoke(RefreshInstance));
            }
            else
                tV_Objects_NodeMouseClick(null, new TreeNodeMouseClickEventArgs(tV_Objects.SelectedNode, System.Windows.Forms.MouseButtons.None, 0, 0, 0));
        }

        private void RefreshTree()
        {
            tV_Objects.BeginUpdate();

            //tV_Objects.Nodes.Clear();

            List<Assembly> GameAssemblies = new List<Assembly>();
            var assemblyName = Assembly.GetExecutingAssembly().GetReferencedAssemblies().SingleOrDefault(t => t.Name == "OutpostOmega.Game");
            GameAssemblies.Add(Assembly.Load(assemblyName));

            if (OutpostOmega.Game.GameObject.AddonAssembly != null)
                GameAssemblies.Add(OutpostOmega.Game.GameObject.AddonAssembly);

            tV_Objects.Nodes.Clear();
            var nodes = UpdateTree(GameAssemblies, typeof(GameObject));
            tV_Objects.Nodes.Add(new TreeNode("GameObject", nodes));
            tV_Objects.ExpandAll();

            tV_Objects.EndUpdate();
        }

        private TreeNode[] UpdateTree(List<Assembly> GameAssemblies, Type ParentType)
        {
            List<Type> types = new List<Type>();
            foreach (Assembly GameAssembly in GameAssemblies)
                types.AddRange(GameAssembly.GetTypes().Where(t => t.BaseType == ParentType));

            TreeNode[] nodes = new TreeNode[types.Count];
            for(int i = 0; i < types.Count; i++)
            {
                var subNodes = UpdateTree(GameAssemblies, types[i]);

                var newNode = new TreeNode(types[i].Name, subNodes.ToArray());
                newNode.Tag = types[i];
                nodes[i] = newNode;
            }
            return nodes;
        }

        private void Browser_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.world.GameObjectRemoved -= world_GameObjectRemoved;
            this.world.NewGameObject -= world_NewGameObject;
        }

        Type SelectedType = null;
        private void tV_Objects_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var node = e.Node;
            var type = (Type)node.Tag;
            SelectedType = type;

            var gameObjects = (from go in world.AllGameObjects
                               where go.GetType() == type
                               select go).ToArray();
            
            lB_Instances.BeginUpdate();
            lB_Instances.Items.Clear();
            foreach (var gameObject in gameObjects)
                lB_Instances.Items.Add(gameObject);

            lB_Instances.EndUpdate();
        }

        private void refreshToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            RefreshTree();
        }

        private void lB_Instances_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedType != null)
            {
                var newVecDialog = new EditVector3(Jitter.LinearMath.JVector.Zero, "Position");
                if (newVecDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var newGameObject = GameObject.GenerateNew(SelectedType, world);
                    newGameObject.SetPosition(newVecDialog.NewVector);
                    new EditObject(newGameObject).ShowDialog();
                    newGameObject.Register();
                    tV_Objects_NodeMouseClick(null, new TreeNodeMouseClickEventArgs(tV_Objects.SelectedNode, System.Windows.Forms.MouseButtons.None, 0, 0, 0));
                }
                else
                    MessageBox.Show("Creation cancelled");
            }
        }


        private void lB_Instances_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //select the item under the mouse pointer
                lB_Instances.SelectedIndex = lB_Instances.IndexFromPoint(e.Location);

                cMS_Instance.Show(MousePosition);
            }
        }

        private void cMS_Instance_Opening(object sender, CancelEventArgs e)
        {
            editToolStripMenuItem.Enabled = lB_Instances.SelectedIndex > -1;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new EditObject(lB_Instances.SelectedItem).ShowDialog();
        }
    }
}
