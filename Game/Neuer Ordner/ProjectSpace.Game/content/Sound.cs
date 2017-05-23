using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.Content
{
    public class Sound : ContentFile
    {
        public Sound(string Path)
            : base(Path)
        {
        }
        public Sound(string Path, ContentManager Manager)
            : base(Path, Manager)
        {
        }
    }
}
