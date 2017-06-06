namespace OutpostOmega.Server.Dialog
{
    partial class TestClient
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.b_connect = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsl_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.tB_Output = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pB_output = new System.Windows.Forms.PictureBox();
            this.pB_input = new System.Windows.Forms.PictureBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pB_output)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pB_input)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.b_connect});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(844, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // b_connect
            // 
            this.b_connect.Name = "b_connect";
            this.b_connect.Size = new System.Drawing.Size(59, 20);
            this.b_connect.Text = "Connect";
            this.b_connect.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsl_status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 390);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(844, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsl_status
            // 
            this.tsl_status.Name = "tsl_status";
            this.tsl_status.Size = new System.Drawing.Size(38, 17);
            this.tsl_status.Text = "Status";
            // 
            // tB_Output
            // 
            this.tB_Output.BackColor = System.Drawing.Color.Black;
            this.tB_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tB_Output.ForeColor = System.Drawing.Color.White;
            this.tB_Output.Location = new System.Drawing.Point(0, 0);
            this.tB_Output.Multiline = true;
            this.tB_Output.Name = "tB_Output";
            this.tB_Output.ReadOnly = true;
            this.tB_Output.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tB_Output.Size = new System.Drawing.Size(281, 366);
            this.tB_Output.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pB_output);
            this.panel1.Controls.Add(this.pB_input);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(314, 251);
            this.panel1.TabIndex = 0;
            // 
            // pB_output
            // 
            this.pB_output.Location = new System.Drawing.Point(159, 3);
            this.pB_output.Name = "pB_output";
            this.pB_output.Size = new System.Drawing.Size(150, 150);
            this.pB_output.TabIndex = 1;
            this.pB_output.TabStop = false;
            // 
            // pB_input
            // 
            this.pB_input.Location = new System.Drawing.Point(3, 3);
            this.pB_input.Name = "pB_input";
            this.pB_input.Size = new System.Drawing.Size(150, 150);
            this.pB_input.TabIndex = 0;
            this.pB_input.TabStop = false;
            this.pB_input.Click += new System.EventHandler(this.pB_input_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 24);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tB_Output);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panel1);
            this.splitContainer2.Size = new System.Drawing.Size(844, 366);
            this.splitContainer2.SplitterDistance = 281;
            this.splitContainer2.TabIndex = 4;
            // 
            // TestClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 412);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TestClient";
            this.Text = "TestClient";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestClient_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pB_output)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pB_input)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tB_Output;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem b_connect;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsl_status;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pB_output;
        private System.Windows.Forms.PictureBox pB_input;
        private System.Windows.Forms.SplitContainer splitContainer2;
    }
}