namespace OutpostOmega.Server
{
    partial class Main
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

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tSSL_activeWorld = new System.Windows.Forms.ToolStripStatusLabel();
            this.tSSL_FPS = new System.Windows.Forms.ToolStripStatusLabel();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.serverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tSMI_Main_World = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_Main_World_New = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tSMI_Main_World_Load = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_Main_World_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.modfolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tSMI_Main_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.browserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_Admin = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lockServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_Test_CreateTestworld = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_Physic = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_Physic_Debug = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_Test_World = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_Test_World_CheckInt = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_Test_World_TestSerial = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_Test_Testclient = new System.Windows.Forms.ToolStripMenuItem();
            this.accountsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tB_Input = new System.Windows.Forms.TextBox();
            this.rTB_Output = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tP_Console = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btn_send = new System.Windows.Forms.Button();
            this.tP_Stats = new System.Windows.Forms.TabPage();
            this.tP_WorldViewer = new System.Windows.Forms.TabPage();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tP_Console.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSSL_activeWorld,
            this.tSSL_FPS});
            this.statusStrip1.Location = new System.Drawing.Point(0, 459);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(926, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tSSL_activeWorld
            // 
            this.tSSL_activeWorld.Name = "tSSL_activeWorld";
            this.tSSL_activeWorld.Size = new System.Drawing.Size(72, 17);
            this.tSSL_activeWorld.Text = "World: none";
            // 
            // tSSL_FPS
            // 
            this.tSSL_FPS.Name = "tSSL_FPS";
            this.tSSL_FPS.Size = new System.Drawing.Size(61, 17);
            this.tSSL_FPS.Text = "Tickrate: 0";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "PS Dedicated Server";
            this.notifyIcon1.Visible = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverToolStripMenuItem,
            this.browserToolStripMenuItem,
            this.tSMI_Admin,
            this.testToolStripMenuItem,
            this.accountsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(926, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // serverToolStripMenuItem
            // 
            this.serverToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restartToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.toolStripSeparator3,
            this.tSMI_Main_World,
            this.modfolderToolStripMenuItem,
            this.toolStripSeparator1,
            this.tSMI_Main_Exit});
            this.serverToolStripMenuItem.Image = global::OutpostOmega.Server.Properties.Resources.Computer;
            this.serverToolStripMenuItem.Name = "serverToolStripMenuItem";
            this.serverToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.serverToolStripMenuItem.Text = "Server";
            // 
            // restartToolStripMenuItem
            // 
            this.restartToolStripMenuItem.Image = global::OutpostOmega.Server.Properties.Resources.Back;
            this.restartToolStripMenuItem.Name = "restartToolStripMenuItem";
            this.restartToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.restartToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.restartToolStripMenuItem.Text = "Restart";
            this.restartToolStripMenuItem.Click += new System.EventHandler(this.restartToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Image = global::OutpostOmega.Server.Properties.Resources.Gear;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(150, 6);
            // 
            // tSMI_Main_World
            // 
            this.tSMI_Main_World.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSMI_Main_World_New,
            this.toolStripSeparator2,
            this.tSMI_Main_World_Load,
            this.tSMI_Main_World_Save});
            this.tSMI_Main_World.Name = "tSMI_Main_World";
            this.tSMI_Main_World.Size = new System.Drawing.Size(153, 22);
            this.tSMI_Main_World.Text = "World";
            // 
            // tSMI_Main_World_New
            // 
            this.tSMI_Main_World_New.Image = global::OutpostOmega.Server.Properties.Resources.Document;
            this.tSMI_Main_World_New.Name = "tSMI_Main_World_New";
            this.tSMI_Main_World_New.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.tSMI_Main_World_New.Size = new System.Drawing.Size(143, 22);
            this.tSMI_Main_World_New.Text = "New";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(140, 6);
            // 
            // tSMI_Main_World_Load
            // 
            this.tSMI_Main_World_Load.Image = global::OutpostOmega.Server.Properties.Resources.Folder;
            this.tSMI_Main_World_Load.Name = "tSMI_Main_World_Load";
            this.tSMI_Main_World_Load.Size = new System.Drawing.Size(143, 22);
            this.tSMI_Main_World_Load.Text = "Load";
            this.tSMI_Main_World_Load.Click += new System.EventHandler(this.tSMI_Main_World_Load_Click_1);
            // 
            // tSMI_Main_World_Save
            // 
            this.tSMI_Main_World_Save.Image = global::OutpostOmega.Server.Properties.Resources.Save;
            this.tSMI_Main_World_Save.Name = "tSMI_Main_World_Save";
            this.tSMI_Main_World_Save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.tSMI_Main_World_Save.Size = new System.Drawing.Size(143, 22);
            this.tSMI_Main_World_Save.Text = "Save";
            this.tSMI_Main_World_Save.Click += new System.EventHandler(this.tSMI_Main_World_Save_Click_1);
            // 
            // modfolderToolStripMenuItem
            // 
            this.modfolderToolStripMenuItem.Image = global::OutpostOmega.Server.Properties.Resources.Folder;
            this.modfolderToolStripMenuItem.Name = "modfolderToolStripMenuItem";
            this.modfolderToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.modfolderToolStripMenuItem.Text = "Modfolder";
            this.modfolderToolStripMenuItem.Click += new System.EventHandler(this.modfolderToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(150, 6);
            // 
            // tSMI_Main_Exit
            // 
            this.tSMI_Main_Exit.Image = global::OutpostOmega.Server.Properties.Resources.Close;
            this.tSMI_Main_Exit.Name = "tSMI_Main_Exit";
            this.tSMI_Main_Exit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.tSMI_Main_Exit.Size = new System.Drawing.Size(153, 22);
            this.tSMI_Main_Exit.Text = "Exit";
            // 
            // browserToolStripMenuItem
            // 
            this.browserToolStripMenuItem.Image = global::OutpostOmega.Server.Properties.Resources.Globe;
            this.browserToolStripMenuItem.Name = "browserToolStripMenuItem";
            this.browserToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            this.browserToolStripMenuItem.Text = "Browser";
            this.browserToolStripMenuItem.Click += new System.EventHandler(this.browserToolStripMenuItem_Click_1);
            // 
            // tSMI_Admin
            // 
            this.tSMI_Admin.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pauseToolStripMenuItem,
            this.lockServerToolStripMenuItem});
            this.tSMI_Admin.Image = global::OutpostOmega.Server.Properties.Resources.Star;
            this.tSMI_Admin.Name = "tSMI_Admin";
            this.tSMI_Admin.Size = new System.Drawing.Size(71, 20);
            this.tSMI_Admin.Text = "Admin";
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.pauseToolStripMenuItem.Text = "Pause World";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
            // 
            // lockServerToolStripMenuItem
            // 
            this.lockServerToolStripMenuItem.Name = "lockServerToolStripMenuItem";
            this.lockServerToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.lockServerToolStripMenuItem.Text = "Lock Server";
            this.lockServerToolStripMenuItem.Click += new System.EventHandler(this.lockServerToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSMI_Test_CreateTestworld,
            this.tSMI_Physic,
            this.tSMI_Test_World,
            this.tSMI_Test_Testclient});
            this.testToolStripMenuItem.Image = global::OutpostOmega.Server.Properties.Resources.Bug;
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // tSMI_Test_CreateTestworld
            // 
            this.tSMI_Test_CreateTestworld.Name = "tSMI_Test_CreateTestworld";
            this.tSMI_Test_CreateTestworld.Size = new System.Drawing.Size(162, 22);
            this.tSMI_Test_CreateTestworld.Text = "Create Testworld";
            this.tSMI_Test_CreateTestworld.Click += new System.EventHandler(this.tSMI_Test_CreateTestworld_Click);
            // 
            // tSMI_Physic
            // 
            this.tSMI_Physic.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSMI_Physic_Debug});
            this.tSMI_Physic.Name = "tSMI_Physic";
            this.tSMI_Physic.Size = new System.Drawing.Size(162, 22);
            this.tSMI_Physic.Text = "Physic";
            // 
            // tSMI_Physic_Debug
            // 
            this.tSMI_Physic_Debug.Name = "tSMI_Physic_Debug";
            this.tSMI_Physic_Debug.Size = new System.Drawing.Size(109, 22);
            this.tSMI_Physic_Debug.Text = "Debug";
            this.tSMI_Physic_Debug.Click += new System.EventHandler(this.tSMI_Physic_Debug_Click);
            // 
            // tSMI_Test_World
            // 
            this.tSMI_Test_World.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSMI_Test_World_CheckInt,
            this.tSMI_Test_World_TestSerial});
            this.tSMI_Test_World.Name = "tSMI_Test_World";
            this.tSMI_Test_World.Size = new System.Drawing.Size(162, 22);
            this.tSMI_Test_World.Text = "World";
            // 
            // tSMI_Test_World_CheckInt
            // 
            this.tSMI_Test_World_CheckInt.Name = "tSMI_Test_World_CheckInt";
            this.tSMI_Test_World_CheckInt.Size = new System.Drawing.Size(161, 22);
            this.tSMI_Test_World_CheckInt.Text = "Check integrity";
            // 
            // tSMI_Test_World_TestSerial
            // 
            this.tSMI_Test_World_TestSerial.Name = "tSMI_Test_World_TestSerial";
            this.tSMI_Test_World_TestSerial.Size = new System.Drawing.Size(161, 22);
            this.tSMI_Test_World_TestSerial.Text = "Test Serialization";
            this.tSMI_Test_World_TestSerial.Click += new System.EventHandler(this.tSMI_Test_World_TestSerial_Click);
            // 
            // tSMI_Test_Testclient
            // 
            this.tSMI_Test_Testclient.Name = "tSMI_Test_Testclient";
            this.tSMI_Test_Testclient.Size = new System.Drawing.Size(162, 22);
            this.tSMI_Test_Testclient.Text = "Testclient";
            this.tSMI_Test_Testclient.Click += new System.EventHandler(this.tSMI_Test_Testclient_Click);
            // 
            // accountsToolStripMenuItem
            // 
            this.accountsToolStripMenuItem.Image = global::OutpostOmega.Server.Properties.Resources.Users;
            this.accountsToolStripMenuItem.Name = "accountsToolStripMenuItem";
            this.accountsToolStripMenuItem.Size = new System.Drawing.Size(85, 20);
            this.accountsToolStripMenuItem.Text = "Accounts";
            this.accountsToolStripMenuItem.Click += new System.EventHandler(this.accountsToolStripMenuItem_Click);
            // 
            // tB_Input
            // 
            this.tB_Input.BackColor = System.Drawing.Color.DimGray;
            this.tB_Input.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tB_Input.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tB_Input.ForeColor = System.Drawing.Color.White;
            this.tB_Input.Location = new System.Drawing.Point(0, 0);
            this.tB_Input.Multiline = true;
            this.tB_Input.Name = "tB_Input";
            this.tB_Input.Size = new System.Drawing.Size(628, 73);
            this.tB_Input.TabIndex = 0;
            // 
            // rTB_Output
            // 
            this.rTB_Output.BackColor = System.Drawing.Color.Black;
            this.rTB_Output.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rTB_Output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rTB_Output.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.rTB_Output.ForeColor = System.Drawing.Color.White;
            this.rTB_Output.Location = new System.Drawing.Point(0, 0);
            this.rTB_Output.Name = "rTB_Output";
            this.rTB_Output.ReadOnly = true;
            this.rTB_Output.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rTB_Output.Size = new System.Drawing.Size(668, 326);
            this.rTB_Output.TabIndex = 3;
            this.rTB_Output.Text = "";
            this.rTB_Output.TextChanged += new System.EventHandler(this.rTB_Output_TextChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(926, 435);
            this.splitContainer1.SplitterDistance = 682;
            this.splitContainer1.TabIndex = 4;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tP_Console);
            this.tabControl1.Controls.Add(this.tP_Stats);
            this.tabControl1.Controls.Add(this.tP_WorldViewer);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(682, 435);
            this.tabControl1.TabIndex = 4;
            // 
            // tP_Console
            // 
            this.tP_Console.Controls.Add(this.splitContainer2);
            this.tP_Console.Location = new System.Drawing.Point(4, 22);
            this.tP_Console.Name = "tP_Console";
            this.tP_Console.Padding = new System.Windows.Forms.Padding(3);
            this.tP_Console.Size = new System.Drawing.Size(674, 409);
            this.tP_Console.TabIndex = 0;
            this.tP_Console.Text = "Console";
            this.tP_Console.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.rTB_Output);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tB_Input);
            this.splitContainer2.Panel2.Controls.Add(this.btn_send);
            this.splitContainer2.Size = new System.Drawing.Size(668, 403);
            this.splitContainer2.SplitterDistance = 326;
            this.splitContainer2.TabIndex = 4;
            // 
            // btn_send
            // 
            this.btn_send.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_send.Location = new System.Drawing.Point(628, 0);
            this.btn_send.Name = "btn_send";
            this.btn_send.Size = new System.Drawing.Size(40, 73);
            this.btn_send.TabIndex = 1;
            this.btn_send.Text = "Exec";
            this.btn_send.UseVisualStyleBackColor = true;
            this.btn_send.Click += new System.EventHandler(this.btn_send_Click);
            // 
            // tP_Stats
            // 
            this.tP_Stats.Location = new System.Drawing.Point(4, 22);
            this.tP_Stats.Name = "tP_Stats";
            this.tP_Stats.Padding = new System.Windows.Forms.Padding(3);
            this.tP_Stats.Size = new System.Drawing.Size(674, 409);
            this.tP_Stats.TabIndex = 1;
            this.tP_Stats.Text = "Resources";
            this.tP_Stats.UseVisualStyleBackColor = true;
            // 
            // tP_WorldViewer
            // 
            this.tP_WorldViewer.Location = new System.Drawing.Point(4, 22);
            this.tP_WorldViewer.Name = "tP_WorldViewer";
            this.tP_WorldViewer.Padding = new System.Windows.Forms.Padding(3);
            this.tP_WorldViewer.Size = new System.Drawing.Size(674, 409);
            this.tP_WorldViewer.TabIndex = 2;
            this.tP_WorldViewer.Text = "World Viewer";
            this.tP_WorldViewer.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 481);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Text = "Project Space - Dedicated Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tP_Console.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TextBox tB_Input;
        private System.Windows.Forms.RichTextBox rTB_Output;
        private System.Windows.Forms.ToolStripStatusLabel tSSL_activeWorld;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tSMI_Test_CreateTestworld;
        private System.Windows.Forms.ToolStripMenuItem tSMI_Physic;
        private System.Windows.Forms.ToolStripMenuItem tSMI_Physic_Debug;
        private System.Windows.Forms.ToolStripMenuItem tSMI_Test_World;
        private System.Windows.Forms.ToolStripMenuItem tSMI_Test_World_CheckInt;
        private System.Windows.Forms.ToolStripMenuItem tSMI_Test_World_TestSerial;
        private System.Windows.Forms.ToolStripMenuItem tSMI_Test_Testclient;
        private System.Windows.Forms.ToolStripStatusLabel tSSL_FPS;
        private System.Windows.Forms.ToolStripMenuItem tSMI_Admin;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btn_send;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restartToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem tSMI_Main_World;
        private System.Windows.Forms.ToolStripMenuItem tSMI_Main_World_New;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tSMI_Main_World_Load;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tSMI_Main_Exit;
        private System.Windows.Forms.ToolStripMenuItem browserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lockServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem accountsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tSMI_Main_World_Save;
        private System.Windows.Forms.ToolStripMenuItem modfolderToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tP_Console;
        private System.Windows.Forms.TabPage tP_Stats;
        private System.Windows.Forms.TabPage tP_WorldViewer;
    }
}

