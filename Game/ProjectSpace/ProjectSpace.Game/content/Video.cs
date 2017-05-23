using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.Content
{
    public class Video : ContentFile
    {
        public Video(string Path)
            : base(Path)
        {
        }
        public Video(string Path, ContentManager Manager)
            : base(Path, Manager)
        {
        }
    }
}
