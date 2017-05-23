namespace OutpostOmega.Server.Dialog
{
    partial class uc_Clients
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lV_Clients = new System.Windows.Forms.ListView();
            this.cMS_User = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.messageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mindToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mobToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.kickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.banToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.temporaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.permanentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cMS_User.SuspendLayout();
            this.SuspendLayout();
            // 
            // lV_Clients
            // 
            this.lV_Clients.BackColor = System.Drawing.Color.White;
            this.lV_Clients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lV_Clients.ForeColor = System.Drawing.Color.Black;
            this.lV_Clients.Location = new System.Drawing.Point(0, 0);
            this.lV_Clients.Name = "lV_Clients";
            this.lV_Clients.Size = new System.Drawing.Size(150, 150);
            this.lV_Clients.TabIndex = 2;
            this.lV_Clients.UseCompatibleStateImageBehavior = false;
            this.lV_Clients.View = System.Windows.Forms.View.SmallIcon;
            this.lV_Clients.SelectedIndexChanged += new System.EventHandler(this.lV_Clients_SelectedIndexChanged);
            this.lV_Clients.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lV_Clients_MouseDown);
            // 
            // cMS_User
            // 
            this.cMS_User.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.messageToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolStripSeparator1,
            this.kickToolStripMenuItem,
            this.banToolStripMenuItem});
            this.cMS_User.Name = "contextMenuStrip1";
            this.cMS_User.Size = new System.Drawing.Size(117, 98);
            // 
            // messageToolStripMenuItem
            // 
            this.messageToolStripMenuItem.Image = global::OutpostOmega.Server.Properties.Resources.Mail;
            this.messageToolStripMenuItem.Name = "messageToolStripMenuItem";
            this.messageToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.messageToolStripMenuItem.Text = "Message";
            this.messageToolStripMenuItem.Click += new System.EventHandler(this.messageToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mindToolStripMenuItem,
            this.mobToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // mindToolStripMenuItem
            // 
            this.mindToolStripMenuItem.Name = "mindToolStripMenuItem";
            this.mindToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.mindToolStripMenuItem.Text = "Mind";
            this.mindToolStripMenuItem.Click += new System.EventHandler(this.mindToolStripMenuItem_Click);
            // 
            // mobToolStripMenuItem
            // 
            this.mobToolStripMenuItem.Name = "mobToolStripMenuItem";
            this.mobToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.mobToolStripMenuItem.Text = "Mob";
            this.mobToolStripMenuItem.Click += new System.EventHandler(this.mobToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(113, 6);
            // 
            // kickToolStripMenuItem
            // 
            this.kickToolStripMenuItem.Image = global::OutpostOmega.Server.Properties.Resources.Close;
            this.kickToolStripMenuItem.Name = "kickToolStripMenuItem";
            this.kickToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.kickToolStripMenuItem.Text = "Kick";
            this.kickToolStripMenuItem.Click += new System.EventHandler(this.kickToolStripMenuItem_Click);
            // 
            // banToolStripMenuItem
            // 
            this.banToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.temporaryToolStripMenuItem,
            this.permanentToolStripMenuItem});
            this.banToolStripMenuItem.Enabled = false;
            this.banToolStripMenuItem.Name = "banToolStripMenuItem";
            this.banToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.banToolStripMenuItem.Text = "Ban";
            // 
            // temporaryToolStripMenuItem
            // 
            this.temporaryToolStripMenuItem.Name = "temporaryToolStripMenuItem";
            this.temporaryToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.temporaryToolStripMenuItem.Text = "Temporary";
            // 
            // permanentToolStripMenuItem
            // 
            this.permanentToolStripMenuItem.Name = "permanentToolStripMenuItem";
            this.permanentToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.permanentToolStripMenuItem.Text = "Permanent";
            // 
            // uc_Clients
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lV_Clients);
            this.Name = "uc_Clients";
            this.cMS_User.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lV_Clients;
        private System.Windows.Forms.ContextMenuStrip cMS_User;
        private System.Windows.Forms.ToolStripMenuItem messageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mindToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mobToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem kickToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem banToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem temporaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem permanentToolStripMenuItem;
    }
}
