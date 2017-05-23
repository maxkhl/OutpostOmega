using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OutpostOmega.Game.Content
{
    /// <summary>
    /// Every Content-Object in the Game derives from this class (like models, sounds, pictures,...)
    /// </summary>
    public abstract class ContentFile : IDisposable
    {
        public static UTF8Encoding Encoder = new UTF8Encoding();

        /// <summary>
        /// The physical content of this file.
        /// Since the gameworld stuff doesn't handle much data shit, this should be a option to use
        /// the gameworlds content management in frontent applications. Store and retreive your
        /// display-data here and you dont have to worry about loading stuff multiple times because
        /// this system is restricted to one content object per file!
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Filepath for this contentfile
        /// </summary>
        public string Path 
        {
            get
            {
                return _Path;
            }
            set
            {
                if (File.Exists(value))
                    _Path = GetRelativePath(value, Environment.CurrentDirectory);
                else
                    throw new FileNotFoundException("Contentfile '" + value + "' not found");
            }
        }
        private string _Path = "";

        /// <summary>
        /// FileInfo for this contentFile
        /// </summary>
        public FileInfo FileInfo
        {
            get
            {
                return new FileInfo(Path);
            }
        }

        public ContentManager Manager { get; set; }

        /// <summary>
        /// Initializes a new pre-loaded <see cref="ContentFile"/>. You have to add the ContentManager first and then call Load() to finalize this instance. (Used for serialization only)
        /// </summary>
        public ContentFile(string Path)
        {
            Data = null;
            this.Path = Path;
        }

        protected ContentFile(string Path, ContentManager Manager)
        {
            this.Manager = Manager;
            Data = null;
            this.Path = Path;
            Load();
        }

        public bool Loaded { get; protected set; }
        public virtual void Load()
        {
            Loaded = true;
        }



        /// <summary>
        /// Loads a specific file
        /// </summary>
        public static ContentFile LoadFile(FileInfo File, ContentManager Manager)
        {
            if (!File.Exists)
                throw new FileNotFoundException("ContentFile '"+File.FullName+"' not found");

            ContentFile NewCFile = null;
            switch (File.Extension)
            {
                case ".png":
                case ".jpg":
                case ".bmp":
                    NewCFile = new Texture(File.FullName, Manager);
                    break;
                case ".wav":
                    NewCFile = new Sound(File.FullName, Manager);
                    break;
                case ".avi":
                    NewCFile = new Video(File.FullName, Manager);
                    break;
                case ".dae":
                    NewCFile = new Model(File.FullName, Manager);
                    break;
                case ".xml":
                    NewCFile = new UserInterface(File.FullName, Manager);
                    break;
            }
            
            return NewCFile;
        }

        public static string GetRelativePath(string filespec, string folder)
        {
            Uri pathUri = new Uri(filespec, UriKind.RelativeOrAbsolute);

            if (pathUri.IsAbsoluteUri)
            {
                // Folders must end in a slash
                if (!folder.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                {
                    folder += System.IO.Path.DirectorySeparatorChar;
                }
                Uri folderUri = new Uri(folder);
                return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', System.IO.Path.DirectorySeparatorChar));
            }
            else
                return filespec;
        }


        public bool Disposing { get; set; }
        public virtual void Dispose()
        {
            Disposing = true;
        }
    }
}
