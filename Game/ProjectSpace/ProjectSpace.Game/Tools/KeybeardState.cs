using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace OutpostOmega.Game.Tools
{
    public class KeybeardState : Dictionary<Key, bool>
    {
        public double ElapsedTime = -1;

        public KeybeardState()
        {
            var keynames = Enum.GetNames(typeof(Key));

            foreach (var keyname in keynames)
            {
                var key = (Key)Enum.Parse(typeof(Key), keyname);
                if(!this.ContainsKey(key))
                    this.Add(key, false);
            }
        }

        public KeybeardState(OpenTK.Input.KeyboardState openTKState)
        {
            var keynames = Enum.GetNames(typeof(Key));

            foreach(var keyname in keynames)
            {
                var key = (Key)Enum.Parse(typeof(Key), keyname);
                if (!this.ContainsKey(key))
                    this.Add(key, openTKState.IsKeyDown(key));
            }
        }
    }
}
