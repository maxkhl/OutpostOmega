namespace OutpostOmega.Server.Dialog
{
    partial class EditVector3
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.nUD_X = new System.Windows.Forms.NumericUpDown();
            this.nUD_Y = new System.Windows.Forms.NumericUpDown();
            this.nUD_Z = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_X)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Z)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Z:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 90);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // nUD_X
            // 
            this.nUD_X.DecimalPlaces = 3;
            this.nUD_X.Location = new System.Drawing.Point(31, 12);
            this.nUD_X.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.nUD_X.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
            this.nUD_X.Name = "nUD_X";
            this.nUD_X.Size = new System.Drawing.Size(69, 20);
            this.nUD_X.TabIndex = 7;
            // 
            // nUD_Y
            // 
            this.nUD_Y.DecimalPlaces = 3;
            this.nUD_Y.Location = new System.Drawing.Point(31, 38);
            this.nUD_Y.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.nUD_Y.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
            this.nUD_Y.Name = "nUD_Y";
            this.nUD_Y.Size = new System.Drawing.Size(69, 20);
            this.nUD_Y.TabIndex = 8;
            // 
            // nUD_Z
            // 
            this.nUD_Z.DecimalPlaces = 3;
            this.nUD_Z.Location = new System.Drawing.Point(31, 64);
            this.nUD_Z.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.nUD_Z.Minimum = new decimal(new int[] {
            999999999,
            0,
            0,
            -2147483648});
            this.nUD_Z.Name = "nUD_Z";
            this.nUD_Z.Size = new System.Drawing.Size(69, 20);
            this.nUD_Z.TabIndex = 9;
            // 
            // EditVector3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(115, 125);
            this.Controls.Add(this.nUD_Z);
            this.Controls.Add(this.nUD_Y);
            this.Controls.Add(this.nUD_X);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EditVector3";
            this.Text = "Vector 3";
            ((System.ComponentModel.ISupportInitialize)(this.nUD_X)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_Z)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown nUD_X;
        private System.Windows.Forms.NumericUpDown nUD_Y;
        private System.Windows.Forms.NumericUpDown nUD_Z;
    }
}