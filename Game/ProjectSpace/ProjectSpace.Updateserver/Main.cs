using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace OutpostOmega.Updateserver
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            propertyGrid.SelectedObject = Settings.Default;
        }

        private void b_opendir_Click(object sender, EventArgs e)
        {

        }

        private void b_refresh_Click(object sender, EventArgs e)
        {
            XElement XMLConfig = new XElement("launcher");
            XMLConfig.Add(new XElement("rev", Settings.Default.version));
            XMLConfig.Add(new XElement("news", Settings.Default.newsurl));
            

            XElement filemanifest = new XElement("filemanifest");
            filemanifest.Add(new XAttribute("ressources", Settings.Default.webadress));

            XMLConfig.Add(filemanifest);

            DirectoryInfo mainDir = new DirectoryInfo(Settings.Default.UploadDirPath);
            if (!mainDir.Exists)
                mainDir.Create();
            
            BuildFolderStructureAsXml(mainDir, filemanifest);

            FileInfo xmlconffile = new FileInfo(Settings.Default.XMLPath);
            var filestream = xmlconffile.OpenWrite();
            filestream.SetLength(0);
            XMLConfig.Save(filestream);
            filestream.Close();

            MessageBox.Show("Done");

        }
        private XElement BuildFolderStructureAsXml(DirectoryInfo dir, XElement xdir)
        {
            var newxDir = new XElement("dir");
            newxDir.Add(new XAttribute("name", dir.Name));

            if (dir.FullName == Settings.Default.UploadDirPath)
                newxDir = xdir;

            xdir.Add(newxDir);

            foreach (FileInfo file in dir.GetFiles("*", SearchOption.TopDirectoryOnly))
            {
                var newxFile = new XElement("file");
                newxFile.Add(new XAttribute("name", file.Name));
                string FilePath = "";
                XElement folder = newxDir;
                while (folder != null)
                {
                    if (folder.Attribute("name") != null)
                        FilePath = FilePath.Insert(0, folder.Attribute("name").Value + "/");

                    if (folder.Name != "filemanifest")
                        folder = folder.Parent;
                    else
                        folder = null;
                }
                newxFile.Add(new XAttribute("path", Settings.Default.webadress + "/" + FilePath + file.Name));


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


            return xdir;
        }

        private void b_files_Click(object sender, EventArgs e)
        {
            DirectoryInfo mainDir = new DirectoryInfo(Settings.Default.UploadDirPath);
            if (!mainDir.Exists)
                mainDir.Create();

            Process.Start(Settings.Default.UploadDirPath);
        }

        private void b_xml_Click(object sender, EventArgs e)
        {
            Process.Start(Settings.Default.XMLPath);
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();
        }
    }

    static class bleh
    {
    

            public static UTF8Encoding Encoder = new UTF8Encoding();
            /// <summary>
            /// Saves a XElement to a stream
            /// </summary>
            public static void Save(this XElement baseElement, Stream stream)
            {
                StringWriter stringWriter = new StringWriter();
                baseElement.Save(stringWriter);
                StringToStream(stream, stringWriter.ToString());
                stream.Close();
            }
            public static void StringToStream(Stream stream, string Text)
            {
                stream.Write(Encoder.GetBytes(Text), 0, Encoder.GetByteCount(Text));
            }
    }
}
