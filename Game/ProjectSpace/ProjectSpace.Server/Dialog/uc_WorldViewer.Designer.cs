namespace OutpostOmega.Server.Dialog
{
    partial class uc_WorldViewer
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tP_StructureDelta = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lB_StructureDelta = new System.Windows.Forms.ListBox();
            this.pG_StructureDelta = new System.Windows.Forms.PropertyGrid();
            this.tP_GameObjectDelta = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lB_GameObjectDelta = new System.Windows.Forms.ListBox();
            this.pG_GameObjectDelta = new System.Windows.Forms.PropertyGrid();
            this.tabControl1.SuspendLayout();
            this.tP_StructureDelta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tP_GameObjectDelta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(827, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tP_StructureDelta);
            this.tabControl1.Controls.Add(this.tP_GameObjectDelta);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(827, 481);
            this.tabControl1.TabIndex = 1;
            // 
            // tP_StructureDelta
            // 
            this.tP_StructureDelta.Controls.Add(this.splitContainer1);
            this.tP_StructureDelta.Location = new System.Drawing.Point(4, 22);
            this.tP_StructureDelta.Name = "tP_StructureDelta";
            this.tP_StructureDelta.Padding = new System.Windows.Forms.Padding(3);
            this.tP_StructureDelta.Size = new System.Drawing.Size(819, 455);
            this.tP_StructureDelta.TabIndex = 0;
            this.tP_StructureDelta.Text = "Structure Delta";
            this.tP_StructureDelta.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lB_StructureDelta);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pG_StructureDelta);
            this.splitContainer1.Size = new System.Drawing.Size(813, 449);
            this.splitContainer1.SplitterDistance = 271;
            this.splitContainer1.TabIndex = 0;
            // 
            // lB_StructureDelta
            // 
            this.lB_StructureDelta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lB_StructureDelta.FormattingEnabled = true;
            this.lB_StructureDelta.Location = new System.Drawing.Point(0, 0);
            this.lB_StructureDelta.Name = "lB_StructureDelta";
            this.lB_StructureDelta.Size = new System.Drawing.Size(271, 449);
            this.lB_StructureDelta.TabIndex = 0;
            // 
            // pG_StructureDelta
            // 
            this.pG_StructureDelta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pG_StructureDelta.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pG_StructureDelta.Location = new System.Drawing.Point(0, 0);
            this.pG_StructureDelta.Name = "pG_StructureDelta";
            this.pG_StructureDelta.Size = new System.Drawing.Size(538, 449);
            this.pG_StructureDelta.TabIndex = 0;
            // 
            // tP_GameObjectDelta
            // 
            this.tP_GameObjectDelta.Controls.Add(this.splitContainer2);
            this.tP_GameObjectDelta.Location = new System.Drawing.Point(4, 22);
            this.tP_GameObjectDelta.Name = "tP_GameObjectDelta";
            this.tP_GameObjectDelta.Padding = new System.Windows.Forms.Padding(3);
            this.tP_GameObjectDelta.Size = new System.Drawing.Size(819, 455);
            this.tP_GameObjectDelta.TabIndex = 1;
            this.tP_GameObjectDelta.Text = "GameObject Delta";
            this.tP_GameObjectDelta.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.lB_GameObjectDelta);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pG_GameObjectDelta);
            this.splitContainer2.Size = new System.Drawing.Size(813, 449);
            this.splitContainer2.SplitterDistance = 271;
            this.splitContainer2.TabIndex = 0;
            // 
            // lB_GameObjectDelta
            // 
            this.lB_GameObjectDelta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lB_GameObjectDelta.FormattingEnabled = true;
            this.lB_GameObjectDelta.Location = new System.Drawing.Point(0, 0);
            this.lB_GameObjectDelta.Name = "lB_GameObjectDelta";
            this.lB_GameObjectDelta.Size = new System.Drawing.Size(271, 449);
            this.lB_GameObjectDelta.TabIndex = 0;
            this.lB_GameObjectDelta.SelectedIndexChanged += new System.EventHandler(this.lB_GameObjectDelta_SelectedIndexChanged);
            // 
            // pG_GameObjectDelta
            // 
            this.pG_GameObjectDelta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pG_GameObjectDelta.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pG_GameObjectDelta.Location = new System.Drawing.Point(0, 0);
            this.pG_GameObjectDelta.Name = "pG_GameObjectDelta";
            this.pG_GameObjectDelta.Size = new System.Drawing.Size(538, 449);
            this.pG_GameObjectDelta.TabIndex = 0;
            // 
            // uc_WorldViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "uc_WorldViewer";
            this.Size = new System.Drawing.Size(827, 505);
            this.tabControl1.ResumeLayout(false);
            this.tP_StructureDelta.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tP_GameObjectDelta.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tP_StructureDelta;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox lB_StructureDelta;
        private System.Windows.Forms.PropertyGrid pG_StructureDelta;
        private System.Windows.Forms.TabPage tP_GameObjectDelta;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListBox lB_GameObjectDelta;
        private System.Windows.Forms.PropertyGrid pG_GameObjectDelta;
    }
}
