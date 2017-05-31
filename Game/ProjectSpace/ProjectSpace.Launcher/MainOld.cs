using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace OutpostOmega.Launcher
{
    public partial class MainOld : Form
    {
        XElement remoteXmlFile;
        
        public MainOld()
        {
            InitializeComponent();
            p_ready.Hide();
            p_update.Hide();
            p_update_button.Hide();

            l_locver.Text = Settings.Default.version;

            // Try to get news site
            try
            {
                webBrowser.Url = new Uri(Settings.Default.newsurl);
            }
            catch
            {
                l_error.Text += "Could not show news page. ";
                webBrowser.Hide();
            }

            // Get XML File
            try
            {
                remoteXmlFile = XElement.Load(Settings.Default.launcherdaturl);
            }
            catch
            {
                l_error.Text += "Unable to contact update server. ";
                b_launch.Text += " (maybe out of date)";
                p_ready.Show();
                p_update.Hide();
                return;
            }
            
            // compare server version
            CheckVersion();
        }

        public void CheckVersion()
        {
            var version = remoteXmlFile.Element("rev").Value;
            
            l_newver.Text = version;

            if (Settings.Default.version != version)
            {
                l_vcomment.Text = "Your version is outdated!";
                p_ready.Hide();
                p_update.Hide();
                p_update_button.Show();
            }
            else
            {
                p_ready.Show();
                p_update.Hide();
                p_update_button.Hide();            
                l_vcomment.Text = "";
            }
        }

        Thread updateThread;
        DirectoryInfo mainDir;
        private void b_update_Click(object sender, EventArgs e)
        {
            mainDir = new DirectoryInfo("game");

            if (!mainDir.Exists)
                mainDir.Create();

            var folderStructure = BuildFolderStructureAsXml(mainDir, null);
            if (CompareFiles(folderStructure, remoteXmlFile.Element("filemanifest"), true))
            {
                p_update.Show();
                p_update_button.Hide();
                p_ready.Hide();


                updateThread = new Thread(StartUpdate);
                updateThread.Start();

                var threadbar = new Thread(UpdateTimer);
                threadbar.Start();
            }
        }

        private delegate void UpdateTimerCallback();
        private void UpdateTimer()
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new UpdateTimerCallback(UpdateTimer));
            }
            else
            {
                if (overallFiles > 0)
                {
                    while (updateThread.IsAlive)
                    {
                        if (processedFiles > 0)
                        {
                            try
                            {
                                progressBar.Value = (int)Math.Round(((decimal)processedFiles / (decimal)overallFiles) * progressBar.Maximum);
                                progressBar.Refresh();
                                l_update_perc.Text = processedFiles + " / " + overallFiles + " files";
                                l_update_perc.Refresh();
                            }
                            catch
                            { }
                        }
                    }
                }
                if (processedFiles == overallFiles)
                {
                    p_update.Hide();
                    p_update_button.Hide();
                    p_ready.Show();

                    Settings.Default.version = remoteXmlFile.Element("rev").Value;
                    Settings.Default.Save();

                    l_locver.Text = Settings.Default.version;
                    l_vcomment.Text = "";
                }
            }
        }

        private void StartUpdate()
        {

            var folderStructure = BuildFolderStructureAsXml(mainDir, null);

            //CountFiles(folderStructure, ref locFiles);
            //CountFiles(remoteXmlFile.Element("filemanifest"), ref remFiles);


            if (!CompareFiles(folderStructure, remoteXmlFile.Element("filemanifest")))
                processedFiles = overallFiles * 2;
        }

        int processedFiles = 0;
        int overallFiles = 0;
        private bool CompareFiles(XElement localStructure, XElement remoteStructure, bool test = false)
        {
            var remoteELements = remoteStructure.Elements();
            var localElements = localStructure.Elements();

            bool hit = false;
            foreach (XElement elem in localElements)
            {
                switch(elem.Name.ToString())
                {
                    case "dir":
                        hit = false;
                        foreach(XElement relem in remoteELements)
                        {
                            if(relem.Name == elem.Name &&
                                relem.Attribute("name").Value == elem.Attribute("name").Value)
                            {
                                //CompareFiles(elem, relem, test);
                                hit = true;
                            }
                        }
                        if(!hit)
                        {
                            if (!test)
                                Directory.Delete(elem.Attribute("path").Value, true);
                        }
                        break;
                    case "file":
                        hit = false;
                        foreach (XElement relem in remoteELements)
                        {
                            if (relem.Name == elem.Name &&
                                relem.Attribute("name").Value == elem.Attribute("name").Value)
                            {
                                if (relem.Attribute("hash").Value != elem.Attribute("hash").Value)
                                    if (!test)
                                    {
                                        File.Delete(elem.Attribute("path").Value);
                                    }

                            }
                        }
                        break;
                }
            }


            foreach (XElement elem in remoteELements)
            {
                switch (elem.Name.ToString())
                {
                    case "dir":
                        hit = false;
                        foreach (XElement lelem in localElements)
                        {
                            if (lelem.Name == elem.Name &&
                                lelem.Attribute("name").Value == elem.Attribute("name").Value)
                            {
                                CompareFiles(lelem, elem, test);
                                hit = true;
                            }
                        }
                        if(!hit)
                        {
                            var newdirinfo = Directory.CreateDirectory(localStructure.Attribute("path").Value + @"\" + elem.Attribute("name").Value);
                            var newXelem = new XElement(newdirinfo.Name);
                            newXelem.Add(new XAttribute("name", newdirinfo.Name));
                            newXelem.Add(new XAttribute("path", newdirinfo.FullName));
                            localStructure.Add(newXelem);

                            CompareFiles(newXelem, elem, test);
                        }
                        break;
                    case "file":
                        hit = false;
                        foreach (XElement lelem in localElements)
                        {
                            if (lelem.Name == elem.Name &&
                                lelem.Attribute("name").Value == elem.Attribute("name").Value &&
                                lelem.Attribute("hash").Value == elem.Attribute("hash").Value)
                            {
                                hit = true;
                            }
                        }
                        if (!hit)
                        {
                            if (!test)
                                using (WebClient Client = new WebClient())
                                {
                                    try
                                    {
                                        Client.DownloadFile(elem.Attribute("path").Value, localStructure.Attribute("path").Value + @"\" + elem.Attribute("name").Value);
                                    }
                                    catch(System.Net.WebException e)
                                    {
                                        MessageBox.Show("Could not download a file. Please retry", "Download Error", MessageBoxButtons.OK);
                                        return false;
                                    }
                                    processedFiles++;
                                }
                            else
                                overallFiles++;
                        }
                        break;
                }
            }
            return true;
        }

        private void CountFiles(XElement filestructure, ref int Count)
        {
            Count += filestructure.Elements("file").Count();
            foreach (XElement elemdir in filestructure.Elements("dir"))
                CountFiles(elemdir, ref Count);
        }

        private XElement BuildFolderStructureAsXml(DirectoryInfo dir, XElement xdir)
        {
            var newxDir = new XElement("dir");
            newxDir.Add(new XAttribute("name", dir.Name));
            newxDir.Add(new XAttribute("path", dir.FullName));

            foreach (FileInfo file in dir.GetFiles("*", SearchOption.TopDirectoryOnly))
            {
                var newxFile = new XElement("file");
                newxFile.Add(new XAttribute("name", file.Name));
                newxFile.Add(new XAttribute("path", file.FullName));

                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(file.FullName))
                    {
                        newxFile.Add(new XAttribute("hash", System.BitConverter.ToString(md5.ComputeHash(stream))));
                    }
                }
                newxDir.Add(newxFile);
            }


            foreach (DirectoryInfo subdir in dir.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                BuildFolderStructureAsXml(subdir, newxDir);
            }

            if (xdir != null)
            {
                xdir.Add(newxDir);

                return xdir;
            }
            else
                return newxDir;
        }
        private void l_text_vmine_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var setwin = new SettingsWindow();
            setwin.ShowDialog();
        }

        private void b_launch_Click(object sender, EventArgs e)
        {

            Process pr = new Process();
            pr.StartInfo.FileName = (new FileInfo(Settings.Default.launchfile)).FullName;
            pr.StartInfo.WorkingDirectory = (new FileInfo(Settings.Default.launchfile)).Directory.FullName;
            pr.Start();

            this.Close();
            /*while (pr.HasExited == false)
                if ((DateTime.Now.Second % 5) == 0)
                {
                    // Show a tick every five seconds.         
                    Console.Write(".");
                    System.Threading.Thread.Sleep(1000);
                }*/
        }

        /*protected override void OnPaintBackground(PaintEventArgs e)
        {
            //empty implementation<
        }*/
    }
}
