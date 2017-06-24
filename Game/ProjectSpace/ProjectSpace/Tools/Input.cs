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
        /// <summary>
        /// Contains the current games key bindings
        /// </summary>
        public static Dictionary<Game.Tools.Keys, Game.Tools.Action> KeyBindings { get; private set; }

        /// <summary>
        /// Translates a given key to the current set action and action state that is defined in the bindings
        /// </summary>
        /// <param name="Key">Given key that should be in the opentks keyboard or mousebutton enums. Use tostring</param>
        /// <param name="compoundInputState">The compoundinputstate that should be asked</param>
        /// <returns>A tuple of action and actionstate, if nothing bound returns unknown action</returns>
        public static Tuple<Game.Tools.Action, Game.Tools.ActionState> TranslateKey(Game.Tools.Keys Key, Game.Tools.CompoundInputState compoundInputState)
        {
            var inputDevice = Game.Tools.KeysHelper.GetInputDevice(Key);
            

            if (KeyBindings.ContainsKey(Key))
                return new Tuple<Game.Tools.Action, Game.Tools.ActionState>(
                    KeyBindings[Key],
                    compoundInputState.IsKeyDown(Key) ? Game.Tools.ActionState.Activate : Game.Tools.ActionState.Release);
            else
                return new Tuple<Game.Tools.Action, Game.Tools.ActionState>(
                    Game.Tools.Action.Undefined,
                    Game.Tools.ActionState.Undefined);
        }

        /// <summary>
        /// Adds a key to the keybindings
        /// </summary>
        /// <param name="Key">Given key that should be in the opentks keyboard or mousebutton enums. Use tostring</param>
        /// <param name="Action">Action defined in the Game.Tools.Action enumeration</param>
        private static void AddKey(Game.Tools.Keys Key, Game.Tools.Action Action)
        {
            KeyBindings[Key] = Action;
        }

        /// <summary>
        /// Loads a default keybinding
        /// </summary>
        public static void LoadDefaultSet()
        {
            KeyBindings = new Dictionary<Game.Tools.Keys, Game.Tools.Action>();

            AddKey(Game.Tools.Keys.W, Game.Tools.Action.MoveForward);
            AddKey(Game.Tools.Keys.S, Game.Tools.Action.MoveBack);

            AddKey(Game.Tools.Keys.A, Game.Tools.Action.StrafeLeft);
            AddKey(Game.Tools.Keys.D, Game.Tools.Action.StrafeRight);

            AddKey(Game.Tools.Keys.Space, Game.Tools.Action.Jump);

            AddKey(Game.Tools.Keys.Tab, Game.Tools.Action.ToggleMouseMode);

            AddKey(Game.Tools.Keys.LeftMouseButton, Game.Tools.Action.InteractPrimary);
            AddKey(Game.Tools.Keys.MiddleMouseButton, Game.Tools.Action.InteractTertiary);
            AddKey(Game.Tools.Keys.RightMouseButton, Game.Tools.Action.InteractSecondary);

            AddKey(Game.Tools.Keys.X, Game.Tools.Action.Fart);

            AddKey(Game.Tools.Keys.Q, Game.Tools.Action.Drop);

            AddKey(Game.Tools.Keys.Number1, Game.Tools.Action.SelectQuickslot1);
            AddKey(Game.Tools.Keys.Number2, Game.Tools.Action.SelectQuickslot2);
            AddKey(Game.Tools.Keys.Number3, Game.Tools.Action.SelectQuickslot3);
            AddKey(Game.Tools.Keys.Number4, Game.Tools.Action.SelectQuickslot4);
            AddKey(Game.Tools.Keys.Number5, Game.Tools.Action.SelectQuickslot5);
            AddKey(Game.Tools.Keys.Number6, Game.Tools.Action.SelectQuickslot6);
            AddKey(Game.Tools.Keys.Number7, Game.Tools.Action.SelectQuickslot7);
            AddKey(Game.Tools.Keys.Number8, Game.Tools.Action.SelectQuickslot8);
            AddKey(Game.Tools.Keys.Number9, Game.Tools.Action.SelectQuickslot9);
        }
    }
}
