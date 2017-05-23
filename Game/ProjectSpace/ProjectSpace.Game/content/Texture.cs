using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.Content
{
    public class Texture : ContentFile
    {
        public Texture(string Path)
            : base(Path)
        { }

        public Texture(string Path, ContentManager Manager)
            : base(Path, Manager)
        {
        }
    }
}
