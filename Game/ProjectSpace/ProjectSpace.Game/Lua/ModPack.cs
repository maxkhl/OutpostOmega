using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using LuaInterface;

namespace OutpostOmega.Game.Lua
{
    public class ModPack
    {
        public List<ModContentFile> ContentFiles { get; set; }
        public List<ModScriptFile> Scripts { get; set; }

        public DirectoryInfo Folder
        {
            get
            {
                return ConfigFile.Directory;
            }
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }

        public string StartupScript { get; set; }

        public string MainPath;
        public FileInfo ConfigFile;

        public Assembly Assembly { get; set; }

        public World World { get; set; }
        
        public ModPack(string Name, string Author, string Version, World World)
        {
            this.Name = Name;
            this.Author = Author;
            this.Version = Version;
            this.World = World;

            ContentFiles = new List<ModContentFile>();
            Scripts = new List<ModScriptFile>();
        }

        public ModPack(FileInfo DefinitionFile)
        {

            ContentFiles = new List<ModContentFile>();
            Scripts = new List<ModScriptFile>();

            MainPath = DefinitionFile.Directory.FullName + "\\";
            ConfigFile = DefinitionFile;
            ReadDefinition(XDocument.Load(DefinitionFile.FullName));

            Validate();
        }

        public ModPack(FileInfo DefinitionFile, World World)
        {
            this.World = World;

            ContentFiles = new List<ModContentFile>();
            Scripts = new List<ModScriptFile>();

            MainPath = DefinitionFile.Directory.FullName + "\\";
            ConfigFile = DefinitionFile;
            ReadDefinition(XDocument.Load(DefinitionFile.FullName));

            Validate();
        }

        private void ReadDefinition(XDocument Definition)
        {
            var mainNode = Definition.Element("oo_mod");
            if (mainNode.Attribute("name") != null)
                this.Name = mainNode.Attribute("name").Value;

            if (mainNode.Attribute("author") != null)
                this.Author = mainNode.Attribute("author").Value;

            if (mainNode.Attribute("version") != null)
                this.Version = mainNode.Attribute("version").Value;

            this.ID = this.Name + "_" + this.Version;

            var content = mainNode.Element("content");
            if (content != null)
            {
                var contentFiles = content.Elements("file");

                foreach (var cfile in contentFiles)
                {
                    var mcf = new ModContentFile();
                    if (cfile.Attribute("name") != null)
                        mcf.Name = cfile.Attribute("name").Value;

                    ContentImporter importer;
                    if (cfile.Attribute("importer") != null)
                    {
                        if (Enum.TryParse(cfile.Attribute("importer").Value, out importer))
                            mcf.Importer = importer;
                        else
                            throw new Exception("Content importer '" + cfile.Attribute("importer").Value + "' not recognized");
                    }

                    mcf.File = new FileInfo(MainPath + cfile.Value);

                    ContentFiles.Add(mcf);
                }
            }

            var programs = mainNode.Element("scripts");
            if (programs != null)
            {
                var programFiles = programs.Elements("script");

                foreach (var pfile in programFiles)
                {
                    var mpf = new ModScriptFile();
                    
                    ScriptHook hook;
                    if (pfile.Attribute("hook") != null)
                    {
                        if (Enum.TryParse(pfile.Attribute("hook").Value, out hook))
                            mpf.Hook = hook;
                        else
                            throw new Exception("Script hook '" + pfile.Attribute("hook").Value + "' not recognized");
                    }

                    mpf.File = new FileInfo(MainPath + pfile.Value);

                    Scripts.Add(mpf);
                }
            }
        }

        public bool Validate()
        {
            bool ret = true;
            foreach (var cont in ContentFiles)
                if (!cont.File.Exists)
                    ret = false;

            foreach (var prog in Scripts)
                if (!prog.File.Exists)
                    ret = false;

            return ret;
        }

        public Assembly.Message[] Execute(ScriptHook hook, World world)
        {
            var targetScripts = (from script in this.Scripts
                                     where script.Hook == hook
                                        select script).ToArray();
            
            List<Assembly.Message> messages = new List<Assembly.Message>();
            if (targetScripts.Length > 0)
            {
                if (this.Assembly == null)
                    CreateAssembly(world);

                foreach (var script in targetScripts)
                {
                    if (script.File.Exists)
                    {
                        this.Assembly.ExecuteFile(script.File.FullName);
                    }
                    else
                        throw new Exception(string.Format("File '{0}' in Modpack '{1}' is missing", script.File.Name, this.ID));
                }

                while (this.Assembly.Output.Count > 0)
                {
                    var msgText = this.Assembly.Output.Dequeue();
                    messages.Add(msgText);
                }
            }
            return messages.ToArray();
        }

        private void CreateAssembly(World world)
        {
            this.Assembly = new Assembly(world);
            this.Assembly.RegisterFunction("GetFirstContent", this, this.GetType().GetMethod("GetFirstContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.Assembly.RegisterFunction("GetContent", this, this.GetType().GetMethod("GetContent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            
        }

        protected object GetFirstContent(string ID)
        {
            foreach (var modContentFile in this.ContentFiles)
            {
                if (modContentFile.Name.StartsWith(ID))
                    return GetOrLoadContent(modContentFile);
            }
            return null;
        }

        protected object GetContent(string ID)
        {
            var table = (LuaTable)this.Assembly.DoString("return {}")[0];
            foreach (var modContentFile in this.ContentFiles)
            {
                if (modContentFile.Name.Contains(ID))
                    table[modContentFile.Name] = GetOrLoadContent(modContentFile);
            }
            return table;
        }

        private Content.ContentFile GetOrLoadContent(ModContentFile modContentFile)
        {
            var cFile = World.ContentManager.Exists(modContentFile.ContentFilePath);
            if (cFile != null)
                return cFile;
            else
            {
                return World.ContentManager.Load(modContentFile.ContentFilePath);
            }
        }

        public void ClearAssembly()
        {
            this.Assembly = null;
        }

        public string Save(DirectoryInfo TargetFolder = null)
        {
            if (TargetFolder == null)
                TargetFolder = this.Folder;

            if (!TargetFolder.Exists)
                TargetFolder.Create();

            if (TargetFolder.GetFiles().Length == 0)
                return "Targetfolder is not empty";

            var top = new XElement("oo_mod");
            top.Add(new XAttribute("name", this.Name));
            top.Add(new XAttribute("version", this.Version));
            top.Add(new XAttribute("author", this.Author));

            var contentNode = new XElement("content");
            foreach (var contentFile in this.ContentFiles)
            {
                var file = new XElement("file", contentFile.ContentFilePath);
                file.Add(new XAttribute("name", contentFile.Name));
                file.Add(new XAttribute("importer", contentFile.Importer.ToString()));
                contentNode.Add(file);
            }
            top.Add(contentNode);
            
            var scriptNode = new XElement("scripts");
            foreach(var scriptFile in this.Scripts)
            {
                var file = new XElement("script", scriptFile.File.Name);
                file.Add(new XAttribute("hook", scriptFile.Hook.ToString()));
                scriptNode.Add(file);
            }


            top.Add(scriptNode);

            var newConf = new FileInfo(TargetFolder.FullName + "\\" + this.Name + ".xml");
            if (this.ConfigFile != null)
                newConf = this.ConfigFile;

            top.Save(newConf.FullName);

            this.ConfigFile = newConf;

            return "";
        }

        public static List<FileInfo> SearchFolder(DirectoryInfo TargetFolder)
        {
            var modfiles = new List<FileInfo>();

            if (!TargetFolder.Exists)
                return modfiles;

            var conffiles = TargetFolder.GetFiles("define.xml", SearchOption.AllDirectories);

            foreach (var conffile in conffiles)
                modfiles.Add(conffile);

            return modfiles;
        }

        public static void WipeAddonAssembly()
        {
            GameObject.CreateNewAssembly();
            //gameObject.AddonAssembly.Save("TestAssembly.dll", System.Reflection.PortableExecutableKinds.Preferred32Bit, System.Reflection.ImageFileMachine.AMD64);
        }

        public class ModContentFile
        {
            public string Name { get; set; }
            public FileInfo File { get; set; }
            public ContentImporter Importer { get; set; }
            public Content.ContentManager Manager { get; set; }
            public string ContentFilePath
            {
                get
                {                    
                    return this.File.FullName;
                }
            }

            public ModContentFile()
            {
            }

            public ModContentFile(string Name, FileInfo File, ContentImporter Importer, Content.ContentManager Manager)
            {
                this.Name = Name;
                this.File = File;
                this.Importer = Importer;
                this.Manager = Manager;
            }
        }

        public struct ModScriptFile
        {
            public FileInfo File;
            public ScriptHook Hook;
        }

        public enum ContentImporter
        {
            Texture2D = 1,
            AnimatedTexture2D = 2,
            Video = 3,
            Model = 4,
            UserInterface = 5,
            Sound = 6,
        }

        public enum ScriptHook
        {
            startup,
            update
        }
    }
}
