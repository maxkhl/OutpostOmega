namespace OutpostOmega.Launcher
{
    partial class MainOld
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainOld));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.l_update_status = new System.Windows.Forms.Label();
            this.l_update_perc = new System.Windows.Forms.Label();
            this.p_update = new System.Windows.Forms.Panel();
            this.p_ready = new System.Windows.Forms.Panel();
            this.b_launch = new System.Windows.Forms.Button();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.l_text_vmine = new System.Windows.Forms.Label();
            this.l_text_vtheirs = new System.Windows.Forms.Label();
            this.l_error = new System.Windows.Forms.Label();
            this.l_locver = new System.Windows.Forms.Label();
            this.l_newver = new System.Windows.Forms.Label();
            this.p_update_button = new System.Windows.Forms.Panel();
            this.b_update = new System.Windows.Forms.Button();
            this.l_vcomment = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.p_update.SuspendLayout();
            this.p_ready.SuspendLayout();
            this.p_update_button.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.Image = global::OutpostOmega.Launcher.Properties.Resources.LogoSmall;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(583, 125);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.BackColor = System.Drawing.Color.Black;
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar.ForeColor = System.Drawing.Color.GreenYellow;
            this.progressBar.Location = new System.Drawing.Point(0, 31);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(583, 23);
            this.progressBar.TabIndex = 2;
            this.progressBar.Tag = "";
            // 
            // l_update_status
            // 
            this.l_update_status.AutoSize = true;
            this.l_update_status.ForeColor = System.Drawing.Color.GreenYellow;
            this.l_update_status.Location = new System.Drawing.Point(3, 3);
            this.l_update_status.Name = "l_update_status";
            this.l_update_status.Size = new System.Drawing.Size(86, 13);
            this.l_update_status.TabIndex = 3;
            this.l_update_status.Text = "Update Progress";
            // 
            // l_update_perc
            // 
            this.l_update_perc.AutoSize = true;
            this.l_update_perc.ForeColor = System.Drawing.Color.GreenYellow;
            this.l_update_perc.Location = new System.Drawing.Point(3, 36);
            this.l_update_perc.Name = "l_update_perc";
            this.l_update_perc.Size = new System.Drawing.Size(0, 13);
            this.l_update_perc.TabIndex = 4;
            // 
            // p_update
            // 
            this.p_update.Controls.Add(this.l_update_perc);
            this.p_update.Controls.Add(this.l_update_status);
            this.p_update.Controls.Add(this.progressBar);
            this.p_update.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.p_update.Location = new System.Drawing.Point(0, 338);
            this.p_update.Name = "p_update";
            this.p_update.Size = new System.Drawing.Size(583, 54);
            this.p_update.TabIndex = 5;
            // 
            // p_ready
            // 
            this.p_ready.Controls.Add(this.b_launch);
            this.p_ready.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.p_ready.Location = new System.Drawing.Point(0, 284);
            this.p_ready.Name = "p_ready";
            this.p_ready.Size = new System.Drawing.Size(583, 54);
            this.p_ready.TabIndex = 6;
            // 
            // b_launch
            // 
            this.b_launch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.b_launch.FlatAppearance.BorderColor = System.Drawing.Color.GreenYellow;
            this.b_launch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.b_launch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.b_launch.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.b_launch.ForeColor = System.Drawing.Color.GreenYellow;
            this.b_launch.Location = new System.Drawing.Point(0, 0);
            this.b_launch.Name = "b_launch";
            this.b_launch.Size = new System.Drawing.Size(583, 54);
            this.b_launch.TabIndex = 0;
            this.b_launch.Text = "Start";
            this.b_launch.UseVisualStyleBackColor = true;
            this.b_launch.Click += new System.EventHandler(this.b_launch_Click);
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 125);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(583, 105);
            this.webBrowser.TabIndex = 7;
            // 
            // l_text_vmine
            // 
            this.l_text_vmine.AutoSize = true;
            this.l_text_vmine.ForeColor = System.Drawing.Color.GreenYellow;
            this.l_text_vmine.Location = new System.Drawing.Point(411, 26);
            this.l_text_vmine.Name = "l_text_vmine";
            this.l_text_vmine.Size = new System.Drawing.Size(69, 13);
            this.l_text_vmine.TabIndex = 5;
            this.l_text_vmine.Text = "Your version:";
            this.l_text_vmine.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.l_text_vmine_MouseDoubleClick);
            // 
            // l_text_vtheirs
            // 
            this.l_text_vtheirs.AutoSize = true;
            this.l_text_vtheirs.ForeColor = System.Drawing.Color.GreenYellow;
            this.l_text_vtheirs.Location = new System.Drawing.Point(411, 41);
            this.l_text_vtheirs.Name = "l_text_vtheirs";
            this.l_text_vtheirs.Size = new System.Drawing.Size(83, 13);
            this.l_text_vtheirs.TabIndex = 8;
            this.l_text_vtheirs.Text = "Newest version:";
            // 
            // l_error
            // 
            this.l_error.AutoSize = true;
            this.l_error.ForeColor = System.Drawing.Color.Red;
            this.l_error.Location = new System.Drawing.Point(20, 109);
            this.l_error.Name = "l_error";
            this.l_error.Size = new System.Drawing.Size(0, 13);
            this.l_error.TabIndex = 9;
            // 
            // l_locver
            // 
            this.l_locver.AutoSize = true;
            this.l_locver.ForeColor = System.Drawing.Color.GreenYellow;
            this.l_locver.Location = new System.Drawing.Point(502, 26);
            this.l_locver.Name = "l_locver";
            this.l_locver.Size = new System.Drawing.Size(0, 13);
            this.l_locver.TabIndex = 10;
            // 
            // l_newver
            // 
            this.l_newver.AutoSize = true;
            this.l_newver.ForeColor = System.Drawing.Color.GreenYellow;
            this.l_newver.Location = new System.Drawing.Point(502, 41);
            this.l_newver.Name = "l_newver";
            this.l_newver.Size = new System.Drawing.Size(0, 13);
            this.l_newver.TabIndex = 11;
            // 
            // p_update_button
            // 
            this.p_update_button.Controls.Add(this.b_update);
            this.p_update_button.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.p_update_button.Location = new System.Drawing.Point(0, 230);
            this.p_update_button.Name = "p_update_button";
            this.p_update_button.Size = new System.Drawing.Size(583, 54);
            this.p_update_button.TabIndex = 7;
            // 
            // b_update
            // 
            this.b_update.Dock = System.Windows.Forms.DockStyle.Fill;
            this.b_update.FlatAppearance.BorderColor = System.Drawing.Color.GreenYellow;
            this.b_update.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.b_update.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.b_update.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.b_update.ForeColor = System.Drawing.Color.GreenYellow;
            this.b_update.Location = new System.Drawing.Point(0, 0);
            this.b_update.Name = "b_update";
            this.b_update.Size = new System.Drawing.Size(583, 54);
            this.b_update.TabIndex = 0;
            this.b_update.Text = "Update";
            this.b_update.UseVisualStyleBackColor = true;
            this.b_update.Click += new System.EventHandler(this.b_update_Click);
            // 
            // l_vcomment
            // 
            this.l_vcomment.AutoSize = true;
            this.l_vcomment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l_vcomment.ForeColor = System.Drawing.Color.GreenYellow;
            this.l_vcomment.Location = new System.Drawing.Point(411, 78);
            this.l_vcomment.Name = "l_vcomment";
            this.l_vcomment.Size = new System.Drawing.Size(0, 13);
            this.l_vcomment.TabIndex = 12;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(583, 392);
            this.Controls.Add(this.l_vcomment);
            this.Controls.Add(this.l_newver);
            this.Controls.Add(this.l_locver);
            this.Controls.Add(this.l_error);
            this.Controls.Add(this.l_text_vtheirs);
            this.Controls.Add(this.l_text_vmine);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.p_update_button);
            this.Controls.Add(this.p_ready);
            this.Controls.Add(this.p_update);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "Outpost Omega Launcher";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.p_update.ResumeLayout(false);
            this.p_update.PerformLayout();
            this.p_ready.ResumeLayout(false);
            this.p_update_button.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label l_update_status;
        private System.Windows.Forms.Label l_update_perc;
        private System.Windows.Forms.Panel p_update;
        private System.Windows.Forms.Panel p_ready;
        private System.Windows.Forms.Button b_launch;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Label l_text_vmine;
        private System.Windows.Forms.Label l_text_vtheirs;
        private System.Windows.Forms.Label l_error;
        private System.Windows.Forms.Label l_locver;
        private System.Windows.Forms.Label l_newver;
        private System.Windows.Forms.Panel p_update_button;
        private System.Windows.Forms.Button b_update;
        private System.Windows.Forms.Label l_vcomment;

    }
}