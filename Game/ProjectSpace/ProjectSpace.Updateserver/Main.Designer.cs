namespace OutpostOmega.Updateserver
{
    partial class Main
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
            this.tb_output = new System.Windows.Forms.TextBox();
            this.b_refresh = new System.Windows.Forms.Button();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.b_files = new System.Windows.Forms.Button();
            this.b_xml = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tb_output
            // 
            this.tb_output.Dock = System.Windows.Forms.DockStyle.Top;
            this.tb_output.Location = new System.Drawing.Point(0, 0);
            this.tb_output.Multiline = true;
            this.tb_output.Name = "tb_output";
            this.tb_output.ReadOnly = true;
            this.tb_output.Size = new System.Drawing.Size(633, 130);
            this.tb_output.TabIndex = 0;
            // 
            // b_refresh
            // 
            this.b_refresh.Location = new System.Drawing.Point(12, 136);
            this.b_refresh.Name = "b_refresh";
            this.b_refresh.Size = new System.Drawing.Size(75, 23);
            this.b_refresh.TabIndex = 1;
            this.b_refresh.Text = "Refresh";
            this.b_refresh.UseVisualStyleBackColor = true;
            this.b_refresh.Click += new System.EventHandler(this.b_refresh_Click);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.propertyGrid.Location = new System.Drawing.Point(0, 165);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(633, 186);
            this.propertyGrid.TabIndex = 4;
            // 
            // b_files
            // 
            this.b_files.Location = new System.Drawing.Point(93, 136);
            this.b_files.Name = "b_files";
            this.b_files.Size = new System.Drawing.Size(75, 23);
            this.b_files.TabIndex = 5;
            this.b_files.Text = "See Files";
            this.b_files.UseVisualStyleBackColor = true;
            this.b_files.Click += new System.EventHandler(this.b_files_Click);
            // 
            // b_xml
            // 
            this.b_xml.Location = new System.Drawing.Point(174, 136);
            this.b_xml.Name = "b_xml";
            this.b_xml.Size = new System.Drawing.Size(75, 23);
            this.b_xml.TabIndex = 6;
            this.b_xml.Text = "See XML";
            this.b_xml.UseVisualStyleBackColor = true;
            this.b_xml.Click += new System.EventHandler(this.b_xml_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 351);
            this.Controls.Add(this.b_xml);
            this.Controls.Add(this.b_files);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.b_refresh);
            this.Controls.Add(this.tb_output);
            this.Name = "Main";
            this.Text = "Updateserver";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_output;
        private System.Windows.Forms.Button b_refresh;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Button b_files;
        private System.Windows.Forms.Button b_xml;
    }
}

