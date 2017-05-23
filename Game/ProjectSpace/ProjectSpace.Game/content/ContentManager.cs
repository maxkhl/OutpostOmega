using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OutpostOmega.Game.Content
{
    public class ContentManager
    {
        /// <summary>
        /// All the loaded Content in this game
        /// </summary>
        public List<ContentFile> LoadedContent { get; set; }

        public ContentManager()
        {
            LoadedContent = new List<ContentFile>();
        }


        public void Add(ContentFile File)
        {
            if (Exists(File.Path) == null)
            {
                LoadedContent.Add(File);
                if (ContentChanged != null)
                    ContentChanged(File, ContentManager.ContentChange.Loaded, null);
            }
        }
        public void Remove(ContentFile File)
        {
            if (LoadedContent.Contains(File))
            {
                LoadedContent.Remove(File);
                ContentChanged(File, ContentChange.Disposed, null);
            }
        }

        /// <summary>
        /// Checks if a specific file is already loaded (and returns the contentfile)
        /// </summary>
        public ContentFile Exists(string Path)
        {
            
            return (from cFile in LoadedContent
                    where System.IO.Path.GetFullPath(cFile.Path) == System.IO.Path.GetFullPath(Path)
                    select cFile).FirstOrDefault();
        }

        public void Update()
        {
            foreach (var content in LoadedContent)
                if (!content.Loaded)
                {
                    content.Manager = this;
                    content.Load();
                }
        }

        /// <summary>
        /// Loads a file from the path. (returns already loaded content if match found)
        /// </summary>
        public ContentFile Load(string Path)
        {
            var cfile = Exists(Path);
            if (cfile != null)
                return cfile;
            else
            {
                var newContent = Content.ContentFile.LoadFile(new FileInfo(Path), this);
                this.Add(newContent);
                return newContent;
            }
        }

        /// <summary>
        /// Loads a file from the path. (returns already loaded content if match found)
        /// </summary>
        public ContentFile Load(FileInfo File)
        {
            var cfile = Exists(File.FullName);
            if (cfile != null)
                return cfile;
            else
                return Content.ContentFile.LoadFile(File, this);
        }

        /// <summary>
        /// Unloads a content file
        /// </summary>
        public void Unload(ContentFile cFile)
        {
            LoadedContent.Remove(cFile);
            ContentChanged(cFile, ContentChange.Disposed, null);
        }


        public delegate void ContentChangedHandler(ContentFile contentFile, ContentChange change, object Data);

        /// <summary>
        /// Occurs when content has changed
        /// </summary>
        public event ContentChangedHandler ContentChanged;

        /// <summary>
        /// Type of content changes
        /// </summary>
        public enum ContentChange
        {
            Disposed,
            Loaded,
            Modified,
        }

        public T[] Get<T>()
        {
            return (from cfile in LoadedContent
                    where cfile.GetType() == typeof(T)
                    select (T)Convert.ChangeType(cfile, typeof(T))).ToArray();
        }
    }
}
