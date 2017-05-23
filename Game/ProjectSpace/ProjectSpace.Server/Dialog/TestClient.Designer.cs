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
            this.tB_Output = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
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
            this.tB_Output.Size = new System.Drawing.Size(284, 262);
            this.tB_Output.TabIndex = 0;
            // 
            // TestClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.tB_Output);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "TestClient";
            this.Text = "TestClient";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestClient_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tB_Output;
    }
}