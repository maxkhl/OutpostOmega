using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OutpostOmega.Data;
using OutpostOmega.Game;
using System.IO;

namespace OutpostOmega.Server
{
    partial class Main : Form
    {

        /*void GameObjects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            tSSL_activeWorld.Text = "World: " + _activeWorld.Name + " (" + activeWorld.GameObjects.Count + " GOs)";
        }*/
        private List<Game.Lua.ModPack> Mods;

        public Data.Config Configuration;

        public Statistics Statistic;
        
        public World ActiveWorld
        {
            get
            {
                return _ActiveWorld;
            }
            set
            {
                _ActiveWorld = value;
                if (Statistic != null)
                    Statistic.Suspend = true;

                if(Host != null)
                {
                    if (Host.netServer.Status != Lidgren.Network.NetPeerStatus.Starting)
                    {
                        Message("Shutting down world '" + Host.networkHandler.World.ID + "'");
                        Host.Shutdown();
                        Host = null;
                    }

                    Message("Wiping addon-assembly");
                    Game.Lua.ModPack.WipeAddonAssembly();
                }
                if(_mainGame != null)
                {
                    _mainGame.Stop();
                }
                
                Message("Loading Mods:", Color.Silver);
                Mods = new List<Game.Lua.ModPack>();
                var modFiles = new List<FileInfo>();
                var modFolder = new DirectoryInfo(HostSettings.Default.ModFolder);
                if (modFolder.Exists)
                {
                    modFiles = OutpostOmega.Game.Lua.ModPack.SearchFolder(modFolder);
                    foreach (var ModFile in modFiles)
                        Mods.Add(_ActiveWorld.LoadMod(ModFile));
                }
                else
                    throw new Exception("ModFolder not found");
                foreach(var mod in Mods)
                    Message(string.Format("{0} loaded", mod.ID), Color.Silver);



                tSSL_activeWorld.Text = "World: " + ActiveWorld.ID + " (" + ActiveWorld.AllGameObjects.Count + " GOs)";
                Message("Server starting world '" + value.ID + "' on port '" + HostSettings.Default.Port + "'", Color.LimeGreen);

                Message("Calling mod-startup hook");
                bool Error = false;
                Message("### Mod Output starting ###", Color.Silver);
                foreach (var mod in Mods)
                {
                    var messages = mod.Execute(Game.Lua.ModPack.ScriptHook.startup, _ActiveWorld);
                    foreach (var msg in messages)
                    {
                        Message(string.Format("{0}@{1}: {2}", msg.Sender, mod.Name, msg.Text), msg.Error ? Color.Red : Color.Silver);
                        Error = msg.Error ? true : Error;
                    }
                }
                Message("### Mod Output ending ###", Color.Silver);
                if (Error)
                    Message("WARNING! Script Errors detected! You should probably check that.", Color.Yellow);
                Host = new Network.Host(value.ID, value);
                if (!Host.Start())
                    return;


                if (Statistic != null)
                    Statistic.Suspend = false;
                else
                    Statistic = new Statistics(this);

                _mainGame = new MainGame(value, Host);

                _ActiveWorld.NewGameObject += _ActiveWorld_NewGameObject;

                if (ClientPanel != null)
                    ClientPanel.Dispose();
                ClientPanel = new Dialog.uc_Clients(Host)
                {
                    Parent = splitContainer1.Panel2,
                    Dock = DockStyle.Fill
                };
            }
        }

        public Dialog.uc_Clients ClientPanel;

        /// <summary>
        /// True when Form is about to close
        /// </summary>
        public bool Closing = false;

        /// <summary>
        /// Used to refresh the GO-Counter
        /// </summary>
        void _ActiveWorld_NewGameObject(GameObject newGameObject)
        {
            tSSL_activeWorld.Text = "World: " + ActiveWorld.ID + " (" + ActiveWorld.AllGameObjects.Count + " GOs)";
        }

        public MainGame _mainGame = null;

        private World _ActiveWorld = null;

        public Network.Host Host;

        //private System.Windows.Forms.Timer SecondTimer;

        public Main()
        {
            _MainForm = this;
            InitializeComponent();

            //TEST
            tSMI_Test_CreateTestworld_Click(null, null);
            //tSMI_Test_Testclient_Click(null, null);
            //tSMI_Physic_Debug_Click(null, null);

            Configuration = Data.Config.Load(HostSettings.Default.ConfigFile);

            /*SecondTimer = new System.Windows.Forms.Timer();
            SecondTimer.Interval = 1000;
            SecondTimer.Tick += SecondTimer_Tick;
            SecondTimer.Start();*/

            var netstats = new Dialog.uc_NetStats(this)
            {
                Parent = tP_Stats,
                Dock = DockStyle.Fill
            };
            var worldviewer = new Dialog.uc_WorldViewer(this)
            {
                Parent = tP_WorldViewer,
                Dock = DockStyle.Fill
            };
        }

        /*void SecondTimer_Tick(object sender, EventArgs e)
        {
            SetPPS(
                string.Format("Sending - Packets/s: {0} Messages/s: {2} KBytes/s: {1}",
                Host.netServer.Statistics.SentPackets,
                Host.netServer.Statistics.SentBytes / 1000,
                Host.netServer.Statistics.SentMessages));
            Host.netServer.Statistics.Reset();
        }*/

        private void tSMI_Physic_Debug_Click(object sender, EventArgs e)
        {
            Thread debugThread = new Thread(new ParameterizedThreadStart(Drawer.Main.Start))
            {
                Name = "DebugDrawer"
            };
            debugThread.Start(ActiveWorld);
        }

        private void tSMI_Main_World_Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "Save Files (*.sav)|*.sav|All files (*.*)|*.*",
                Multiselect = false
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK && ofd.CheckFileExists)
            {
                FileInfo loadFile = new FileInfo(ofd.FileName);
                ActiveWorld = DataHandler.LoadWorldFromFile(loadFile, false);
                Message("World '" + ActiveWorld.ID + "' (" + ActiveWorld.AllGameObjects.Count.ToString() + " GOs) loaded. ");
            }
        }

        private void tSMI_Main_World_Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "Save Files (*.sav)|*.sav"
            };
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileInfo saveFile = new FileInfo(sfd.FileName);

                ActiveWorld.SaveToFile(saveFile, false);
                Message("World '" + ActiveWorld.ID + "' (" + ActiveWorld.AllGameObjects.Count.ToString() + " GOs) saved to '" + saveFile.FullName + "'. ");
            }
        }

        private void tSMI_Test_CreateTestworld_Click(object sender, EventArgs e)
        {
            var World = Game.World.CreateTest();
            World.MakePlayer();

            /*var structure = new Game.world.Structure(World);
            structure.Add(Game.world.Chunk.GenerateTestChunk());
            structure.Add(Game.world.Chunk.GenerateTestChunk());
            structure.Add(Game.world.Chunk.GenerateTestChunk());
            World.Structures.Add(structure);*/

            //Game.turf.Structure.AddFlat(World, 80, 1);

            /*for (int i = 0; i < 3; i++)
            {
                new Game.gObject.structure.window(World);
            }*/

            /*var mob = new Game.GameObjects.Mobs.CarbonBased.Human(World);
            mob.PhysicSetPosition(new Jitter.LinearMath.JVector(0, 20, 0));
            huchmehn = mob;
            mob = new Game.GameObjects.Mobs.CarbonBased.Human(World);
            mob.PhysicSetPosition(new Jitter.LinearMath.JVector(0, 30, 3));
            mob = new Game.GameObjects.Mobs.CarbonBased.Human(World);
            mob.PhysicSetPosition(new Jitter.LinearMath.JVector(0, 20, 0));
            mob = new Game.GameObjects.Mobs.CarbonBased.Human(World);
            mob.PhysicSetPosition(new Jitter.LinearMath.JVector(0, 20, 7));*/

            /*for (int i = 0; i < 1; i++)
            {
                new Data.gObject.turf.floor();
            }*/
            Message("Testworld '" + World.ID + "' (" + World.AllGameObjects.Count.ToString() + " GOs) created. ");
            ActiveWorld = World;
        }

        /// <summary>
        /// Used for static methods
        /// </summary>
        private static Main _MainForm;

        private delegate void MessageCallback(string Text);
        /// <summary>
        /// Display a Message on the MainForm. (Threadsafe)
        /// </summary>
        /// <param name="Text">Message</param>
        public static void Message(string Text)
        {
            if (_MainForm == null || _MainForm.Closing || _MainForm.Disposing)
                return;

            if (_MainForm.rTB_Output.InvokeRequired)
            {
                try
                {
                    _MainForm.rTB_Output.Invoke(new MessageCallback(Message), Text);
                }
                catch (Exception e)
                {
                    if (!Program.Crashed) throw e;
                }
            }
            else
            {
                string Message = DateTime.UtcNow.ToString() + " - " + Text + Environment.NewLine;
                _MainForm.rTB_Output.AppendText(Message);
            }
        }

        private delegate void MessageColorCallback(string Text, Color color);

        /// <summary>
        /// Display a Message on the MainForm. (Threadsafe)
        /// </summary>
        /// <param name="Text">Message</param>
        public static void Message(string Text, Color color)
        {
            if (_MainForm == null || _MainForm.Closing)
                return;

            if (_MainForm.rTB_Output.InvokeRequired)
            {
                try
                {
                    _MainForm.rTB_Output.Invoke(new MessageColorCallback(Message), Text, color);
                }
                catch (Exception e)
                {
                    if (!Program.Crashed) throw e;
                }
            }
            else
            {
                string Message = DateTime.UtcNow.ToString() + " - " + Text + Environment.NewLine;
                _MainForm.rTB_Output.AppendText(Message, color);
            }
        }

        private delegate void CLSCallback();
        /// <summary>
        /// Clears the console. (Threadsafe)
        /// </summary>
        public static void CLS()
        {
            if (_MainForm == null || _MainForm.Closing)
                return;

            if (_MainForm.rTB_Output.InvokeRequired)
            {
                try
                {
                    _MainForm.rTB_Output.Invoke(new CLSCallback(CLS));
                }
                catch (Exception e)
                {
                    if (!Program.Crashed) throw e;
                }
            }
            else
            {
                _MainForm.rTB_Output.Clear();
            }
        }

        private delegate void FPSCallback(float FPS);
        public static void SetFPS(float FPS)
        {
            if (_MainForm == null || _MainForm.Closing || _MainForm.IsDisposed)
                return;
            

            if (_MainForm.menuStrip1.InvokeRequired)
            {
                try
                {
                    _MainForm.menuStrip1.Invoke(new FPSCallback(SetFPS), FPS);
                }
                catch(Exception e)
                {
                    if (!Program.Crashed) throw e;
                }
            }
            else
            {
                _MainForm.tSSL_FPS.Text = "Tickrate: " + FPS.ToString("##.#");
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Closing = true;
            Host.Shutdown();
            _mainGame.Stop();
            _ActiveWorld.Dispose();
        }

        private void tSMI_Test_World_TestSerial_Click(object sender, EventArgs e)
        {
            if (Host == null || ActiveWorld == null)
                MessageBox.Show("Load a World first!");

            byte[] byteData = new byte[0];
            Message("Serialization of the loaded World '" + ActiveWorld.ID + "' started");
            try
            {
                byteData = Host.networkHandler.GetObjectData(DataHandler.ConverterFileID, Host.networkHandler.World);
            }
            catch(Exception exc)
            {
                Message("Exception during serialization: " + exc.Message + " in " + exc.Source);
            }
            Message("Serialization finished. Result is " + byteData.Length.ToString() + " bytes big");

            World World_Copy = null;

            Message("Starting deserialization");
            try
            {
                World_Copy = (World)OutpostOmega.Network.NetworkHandler.ReadSerializedData(byteData);
            }
            catch (Exception exc)
            {
                Message("Exception during deserialization: " + exc.Message + " in " + exc.Source);
            }

            if(World_Copy != null)
                Message("Deserialization successfull. Copied World seems consistent!");
            else
                Message("Deserialization not successfull. There seems to be a problem!");

        }

        private void tSMI_Main_World_New_Click(object sender, EventArgs e)
        {
            var newWorldDialog = new Dialog.NewWorld();
            if(newWorldDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var dialogResult = System.Windows.Forms.DialogResult.No;
                if (ActiveWorld != null)
                {
                    dialogResult = MessageBox.Show("Want to save the current world before loading the new one?", "Save World", MessageBoxButtons.YesNoCancel);
                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        tSMI_Main_World_Save_Click(null, null);
                    }
                }

                if (dialogResult == System.Windows.Forms.DialogResult.No)
                    ActiveWorld = newWorldDialog.World;
            }
        }

        private void tSMI_Test_Testclient_Click(object sender, EventArgs e)
        {
            if(ActiveWorld == null)
            {
                MessageBox.Show("Load a world first you stupid donkeyass");
                return;
            }
            var testClient = new Dialog.TestClient(ActiveWorld);
            testClient.Show();
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            var messages = ActiveWorld.Console.Execute(tB_Input.Text);
            foreach(var message in messages)
            {
                rTB_Output.AppendText(string.Format("{0} - {1}: {2}\n", message.TimeStamp, message.Sender, message.Text), Color.Yellow);
            }
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _mainGame.Pause = !_mainGame.Pause;
            if (_mainGame.Pause)
                Message("World paused");
            else
                Message("World running");
            ((ToolStripMenuItem)sender).Checked = _mainGame.Pause;                
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //tSMI_Main_World_Save_Click_1(null, null);
            ActiveWorld = ActiveWorld;
        }

        private void browserToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            new Dialog.Browser(_ActiveWorld).Show();
        }

        private void lockServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Host.Locked)
            {
                var ibox = new Dialog.InputBox("Enter lock reason", Host.Lockreason);
                if (ibox.ShowDialog() == DialogResult.OK)
                {
                    Host.Lock(ibox.InputText);
                }
            }
            else
                Host.Unlock();

            lockServerToolStripMenuItem.Checked = Host.Locked;
        }

        private void accountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Dialog.Accounts(Configuration.Accounts).Show();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var propwindow = new Dialog.PropGrid(HostSettings.Default, "Server Settings");
            propwindow.ShowDialog();
            if(propwindow.ValueChanged)
            {
                HostSettings.Default.Save();
                if(MessageBox.Show("Server settings changed. Some settings require the server to restart to take effect. Restart now?", "Settings changed", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    restartToolStripMenuItem_Click(null, null);
                }
            }
        }

        private void tSMI_Main_World_Save_Click_1(object sender, EventArgs e)
        {
            Save();
        }

        public FileInfo LastLoadLocation = null;
        public void Save()
        {
            if (ActiveWorld == null)
            {
                MessageBox.Show("No world loaded.", "Error");
                return;
            }
            if (LastLoadLocation == null || !LastLoadLocation.Exists)
                SaveAs();
            else
            {
                ActiveWorld.SaveToFile(LastLoadLocation, false);
                Message("World '" + ActiveWorld.ID + "' (" + ActiveWorld.AllGameObjects.Count.ToString() + " GOs) saved to '" + LastLoadLocation.FullName + "'. ");
            }

        }
        public void SaveAs()
        {
            if (ActiveWorld == null)
            {
                MessageBox.Show("No world loaded.", "Error");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Save world '" + ActiveWorld.ID + "' to",
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "Save Files (*.sav)|*.sav",
                DefaultExt = ".sav"
            };
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var targetFile = new FileInfo(sfd.FileName);
                ActiveWorld.SaveToFile(targetFile, false);
                Message("World '" + ActiveWorld.ID + "' (" + ActiveWorld.AllGameObjects.Count.ToString() + " GOs) saved to '" + targetFile.FullName + "'. ");
            }
        }

        private void tSMI_Main_World_Load_Click_1(object sender, EventArgs e)
        {
            if(ActiveWorld != null)
            {
                var result = MessageBox.Show("Save loaded world first?", "Load new world", MessageBoxButtons.YesNoCancel);
                if(result == System.Windows.Forms.DialogResult.Yes)
                    Save();
                else if(result == System.Windows.Forms.DialogResult.Cancel)
                    return;
            }

            OpenFileDialog ofd = new OpenFileDialog()
            {
                Title = "Load world from",
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "Save Files (*.sav)|*.sav|All files (*.*)|*.*",
                Multiselect = false
            };
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var targetFile = new FileInfo(ofd.FileName);

                try
                {
                    var newWorld = OutpostOmega.Data.DataHandler.LoadWorldFromFile(targetFile, false);
                    Message("World '" + newWorld.ID + "' (" + newWorld.AllGameObjects.Count.ToString() + " GOs) loaded  from '" + targetFile.FullName + "'.");
                    ActiveWorld = newWorld;
                    LastLoadLocation = targetFile;
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Could not load world. Error '" + exc.Message + "'");
                }
            }
        }

        /// <summary>
        /// Used to stop all asynchronous shit going on
        /// </summary>
        public void Stop()
        {
            Closing = true;

            Statistic.Stop();
            _mainGame.Stop(); // Stop main processing thread
        }

        private void modfolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Path.GetFullPath(HostSettings.Default.ModFolder));
        }

        private void rTB_Output_TextChanged(object sender, EventArgs e)
        {
            if (!rTB_Output.ContainsFocus)
            {
                rTB_Output.ScrollToCaret();
            }
        }

        /// <summary>
        /// Adds a random block to the gameworld
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmi_addRandomBlock_Click(object sender, EventArgs e)
        {
            bool result = false;
            while(!result)
                result = _ActiveWorld.Structures[0].Add(Game.Turf.Types.TurfTypeE.floor, Jitter.LinearMath.JVector.GetRandom(50), true);
        }
    }
}
