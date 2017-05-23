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
    partial class uc_Clients : UserControl
    {
        private Network.Host Host;
        public uc_Clients(Network.Host Host)
        {
            this.Host = Host;
            InitializeComponent();

            this.Host.ConnectedClients.CollectionChanged += ConnectedClients_CollectionChanged;


            var clients = Host.ConnectedClients.ToArray();
            foreach (var client in clients)
            {
                var newItem = new ListViewItem(client.ID);
                newItem.Tag = client;
                lV_Clients.Items.Add(newItem);
            }
        }

        private delegate void CollectionChangedCallback(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e);
        void ConnectedClients_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (lV_Clients.InvokeRequired)
                lV_Clients.Invoke(new CollectionChangedCallback(ConnectedClients_CollectionChanged), new object[2] { sender, e });
            else
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                    foreach (Network.Client client in e.NewItems)
                    {
                        var lvItem = new ListViewItem();
                        lvItem.Name = client.ID;
                        lvItem.Text = client.ID + " (" + client.Connection.RemoteEndPoint.Address.ToString() + ")";
                        lvItem.Tag = client;
                        var groupname = client.Mind.Group.ToString();
                        var group = (from ListViewGroup grp in lV_Clients.Groups
                                     where grp.Header == groupname
                                     select grp).SingleOrDefault();
                        if(group != null)
                            lvItem.Group = group;
                        else
                        {
                            group = new ListViewGroup(groupname);
                            lvItem.Group = group;
                            lV_Clients.Groups.Add(group);
                        }
                        lV_Clients.Items.Add(lvItem);
                    }                        

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                    foreach (Network.Client client in e.OldItems)
                    {
                        for (int i = 0; i < lV_Clients.Items.Count; i++)
                        {
                            if (lV_Clients.Items[i].Name == client.ID)
                                lV_Clients.Items.RemoveAt(i);
                        }
                    }
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            this.Host.ConnectedClients.CollectionChanged -= ConnectedClients_CollectionChanged;
            base.OnHandleDestroyed(e);
        }

        private void lV_Clients_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void mindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lV_Clients.SelectedItems)
            {
                var client = (Network.Client)item.Tag;
                var objEditor = new EditObject(client.Mind);
                objEditor.Show();
            }
        }

        private void mobToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lV_Clients.SelectedItems)
            {
                var client = (Network.Client)item.Tag;
                if(client.Mind.Mob == null)
                {
                    MessageBox.Show(string.Format("No mob assigned to {0}", client.ID));
                    continue;
                }

                var objEditor = new EditObject(client.Mind.Mob);
                objEditor.Show();
            }
        }

        private void kickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ibox = new InputBox("Kick reason");
            if (ibox.ShowDialog() == DialogResult.OK)
                foreach (ListViewItem item in lV_Clients.SelectedItems)
                {
                    var client = (Network.Client)item.Tag;
                    client.Disconnect(ibox.InputText);
                }
        }

        private void messageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ibox = new InputBox("Enter message");
            if (ibox.ShowDialog() == DialogResult.OK)
                foreach (ListViewItem item in lV_Clients.SelectedItems)
                {
                    var client = (Network.Client)item.Tag;
                    client.SendMessage(ibox.InputText);                    
                }
        }

        private void lV_Clients_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewItem match = null;

                foreach (ListViewItem item in lV_Clients.Items)
                {
                    if (item.Bounds.Contains(new Point(e.X, e.Y)))
                    {
                        match = item;
                        item.Selected = true;
                        break;
                    }
                    else
                        item.Selected = false;
                }
                if (match != null)
                {
                    cMS_User.Show(MousePosition);
                }
            }
        }
    }
}
