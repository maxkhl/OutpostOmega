using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Tools
{
    /// <summary>
    /// Manages input (translation between keys and ingame actions)
    /// </summary>
    public static class Input
    {
        public enum InputDevice : byte
        {
            Keyboard = 1,
            Mouse = 2,
        }

        public static Dictionary<Tuple<InputDevice, string>, Game.Tools.Action> KeyBindings { get; private set; }

        public static Game.Tools.Action TranslateKey(InputDevice Device, string Key)
        {
            return KeyBindings[new Tuple<InputDevice, string>(Device, Key)];
        }

        private static void AddKey(InputDevice Device, string Key, Game.Tools.Action Action)
        {
            KeyBindings[new Tuple<InputDevice, string>(Device, Key)] = Action;
        }

        public static void LoadDefaultSet()
        {
            AddKey(InputDevice.Keyboard, OpenTK.Input.Key.W.ToString(), Game.Tools.Action.MoveForward);
            AddKey(InputDevice.Keyboard, OpenTK.Input.Key.S.ToString(), Game.Tools.Action.MoveBack);

            AddKey(InputDevice.Keyboard, OpenTK.Input.Key.A.ToString(), Game.Tools.Action.StrafeLeft);
            AddKey(InputDevice.Keyboard, OpenTK.Input.Key.D.ToString(), Game.Tools.Action.StrafeRight);

            AddKey(InputDevice.Keyboard, OpenTK.Input.Key.Space.ToString(), Game.Tools.Action.Jump);
            //AddKey(InputDevice.Keyboard, OpenTK.Input.Key.W.ToString(), Game.Tools.Action.MoveForward);

            AddKey(InputDevice.Mouse, OpenTK.Input.MouseButton.Left.ToString(), Game.Tools.Action.InteractPrimary);
            AddKey(InputDevice.Mouse, OpenTK.Input.MouseButton.Middle.ToString(), Game.Tools.Action.InteractTertiary);
            AddKey(InputDevice.Mouse, OpenTK.Input.MouseButton.Right.ToString(), Game.Tools.Action.InteractSecondary);
        }
    }
}
