using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;
using OutpostOmega.Game.Tools;

namespace OutpostOmega
{
    /// <summary>
    /// This part of the class handles the players input
    /// </summary>
    partial class Scene
    {
        /// <summary>
        /// The compoundInputState of the last frame (more like last call of UpdateInput())
        /// </summary>
        private CompoundInputState oldCompoundInputState { get; set; }

        /// <summary>
        /// Call this to check for updated inputs and fire the InputChanged event
        /// </summary>
        private void UpdateInput()
        {
            // First run creates a empty oldCompoundInputState
            if (oldCompoundInputState == null)
                oldCompoundInputState = new CompoundInputState();

            // first create a compound state to have a complete sample of the keys for this frame
            var compState = new Game.Tools.CompoundInputState(
                new KeybeardState(Keyboard.GetState()),
                new Game.Tools.MouseState(Mouse.GetState()));

            // Now check all key states and fire the event once something changes
            if(KeyStateChanged != null)
                foreach(Keys key in Enum.GetValues(typeof(Keys)))
                {
                    if(compState.IsKeyDown(key) != oldCompoundInputState.IsKeyDown(key))
                    {
                        var actionTuple = Tools.Input.TranslateKey(key, compState);

                        // Undefined actions are trigger by unbound keys and should not be fired
                        if (actionTuple.Item1 != OutpostOmega.Game.Tools.Action.Undefined)
                        {
                            // Fire event with given parameters
                            KeyStateChanged(
                                actionTuple.Item1,
                                actionTuple.Item2);
                        }
                    }
                }

            // Check for mouse movement and fire the event
            if (MouseMoved != null)
            {
                int MouseDeltaX = compState.MouseState.X - oldCompoundInputState.MouseState.X,
                    MouseDeltaY = compState.MouseState.Y - oldCompoundInputState.MouseState.Y;
                if (MouseDeltaX != 0 || MouseDeltaY != 0)
                    MouseMoved(MouseDeltaX, MouseDeltaY);
            }

            // Store compound to compare in the next call
            oldCompoundInputState = compState;
        }

        /// <summary>
        /// InputEventHandler for input events
        /// </summary>
        /// <param name="action">Triggered action</param>
        /// <param name="actionState">The actions current state</param>
        public delegate void KeyStateChangedHandler(OutpostOmega.Game.Tools.Action action, OutpostOmega.Game.Tools.ActionState actionState);

        /// <summary>
        /// Is being fired whenever a actions state changes
        /// </summary>
        public event KeyStateChangedHandler KeyStateChanged;


        /// <summary>
        /// MouseMovedHandler for mouse movement event
        /// </summary>
        /// <param name="X">Delta X movement</param>
        /// <param name="Y">Delta Y movement</param>
        public delegate void MouseMovedHandler(int X, int Y);

        /// <summary>
        /// Is being fired whenever the mouse moves
        /// </summary>
        public event MouseMovedHandler MouseMoved;
    }
}
