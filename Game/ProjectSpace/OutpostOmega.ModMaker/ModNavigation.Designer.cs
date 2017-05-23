namespace OutpostOmega.ModMaker
{
    partial class ModNavigation
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModNavigation));
            this.tV_Navigation = new System.Windows.Forms.TreeView();
            this.iL_TreeIcons = new System.Windows.Forms.ImageList(this.components);
            this.cMS_Navigation = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TMSI_Navigation_New = new System.Windows.Forms.ToolStripMenuItem();
            this.TMSI_Navigation_New_Script = new System.Windows.Forms.ToolStripMenuItem();
            this.cMS_Navigation.SuspendLayout();
            this.SuspendLayout();
            // 
            // tV_Navigation
            // 
            this.tV_Navigation.BackColor = System.Drawing.SystemColors.Window;
            this.tV_Navigation.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tV_Navigation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tV_Navigation.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tV_Navigation.ImageIndex = 0;
            this.tV_Navigation.ImageList = this.iL_TreeIcons;
            this.tV_Navigation.LineColor = System.Drawing.Color.Navy;
            this.tV_Navigation.Location = new System.Drawing.Point(0, 0);
            this.tV_Navigation.Name = "tV_Navigation";
            this.tV_Navigation.SelectedImageIndex = 0;
            this.tV_Navigation.Size = new System.Drawing.Size(240, 216);
            this.tV_Navigation.TabIndex = 0;
            // 
            // iL_TreeIcons
            // 
            this.iL_TreeIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("iL_TreeIcons.ImageStream")));
            this.iL_TreeIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.iL_TreeIcons.Images.SetKeyName(0, "Empty.png");
            this.iL_TreeIcons.Images.SetKeyName(1, "Photo.png");
            this.iL_TreeIcons.Images.SetKeyName(2, "Media.png");
            this.iL_TreeIcons.Images.SetKeyName(3, "Video.png");
            this.iL_TreeIcons.Images.SetKeyName(4, "Plane.png");
            this.iL_TreeIcons.Images.SetKeyName(5, "Flag.png");
            this.iL_TreeIcons.Images.SetKeyName(6, "Sound.png");
            this.iL_TreeIcons.Images.SetKeyName(7, "Document Text.png");
            this.iL_TreeIcons.Images.SetKeyName(8, "Folder.png");
            this.iL_TreeIcons.Images.SetKeyName(9, "Document Graph.png");
            this.iL_TreeIcons.Images.SetKeyName(10, "Close.png");
            // 
            // cMS_Navigation
            // 
            this.cMS_Navigation.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TMSI_Navigation_New});
            this.cMS_Navigation.Name = "cMS_Navigation";
            this.cMS_Navigation.Size = new System.Drawing.Size(96, 26);
            // 
            // TMSI_Navigation_New
            // 
            this.TMSI_Navigation_New.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TMSI_Navigation_New_Script});
            this.TMSI_Navigation_New.Name = "TMSI_Navigation_New";
            this.TMSI_Navigation_New.Size = new System.Drawing.Size(95, 22);
            this.TMSI_Navigation_New.Text = "New";
            // 
            // TMSI_Navigation_New_Script
            // 
            this.TMSI_Navigation_New_Script.Name = "TMSI_Navigation_New_Script";
            this.TMSI_Navigation_New_Script.Size = new System.Drawing.Size(152, 22);
            this.TMSI_Navigation_New_Script.Text = "Script";
            this.TMSI_Navigation_New_Script.Click += new System.EventHandler(this.TMSI_Navigation_New_Script_Click);
            // 
            // ModNavigation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tV_Navigation);
            this.Name = "ModNavigation";
            this.Size = new System.Drawing.Size(240, 216);
            this.cMS_Navigation.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip cMS_Navigation;
        private System.Windows.Forms.ToolStripMenuItem TMSI_Navigation_New;
        private System.Windows.Forms.ToolStripMenuItem TMSI_Navigation_New_Script;
        public System.Windows.Forms.TreeView tV_Navigation;
        public System.Windows.Forms.ImageList iL_TreeIcons;
    }
}
