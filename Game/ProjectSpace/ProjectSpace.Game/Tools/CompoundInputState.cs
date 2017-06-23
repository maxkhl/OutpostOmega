using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.Tools
{
    /// <summary>
    /// This is a combined input state class that unites mouse and keyboard states
    /// It helps looking at mouse- and keyboard-keys as one entity
    /// </summary>
    public class CompoundInputState
    {
        /// <summary>
        /// Keyboard state featured in this compound
        /// </summary>
        public KeybeardState KeyboardState { get; set; }

        /// <summary>
        /// Mouse state featured in this compound
        /// </summary>
        public MouseState MouseState { get; set; }

        /// <summary>
        /// Default constructor, takes no input, creates a empty state - no buttons set
        /// </summary>
        public CompoundInputState()
        {
            KeyboardState = new KeybeardState();
            MouseState = new MouseState();
        }

        /// <summary>
        /// Use this constructor to set the states during construction
        /// </summary>
        /// <param name="KeyboardState">Keyboard state that should be used</param>
        /// <param name="MouseState">Mouse state that should be used</param>
        public CompoundInputState(KeybeardState KeyboardState, MouseState MouseState)
        {
            this.KeyboardState = KeyboardState;
            this.MouseState = MouseState;
        }

        /// <summary>
        /// Checks if a key fits the given string and returns it
        /// </summary>
        /// <param name="Key">Given key that should be in the opentks keyboard or mousebutton enums. Use tostring</param>
        /// <returns>true, if the button is pressed in this state</returns>
        public bool IsKeyDown(Tools.Keys Key)
        {
            switch (Tools.KeysHelper.GetInputDevice(Key))
            {
                case InputDevice.Keyboard:
                    return this.KeyboardState.IsKeyDown((OpenTK.Input.Key)KeysHelper.ConvertOut(Key).Item2);
                case InputDevice.Mouse:
                    return this.MouseState.IsButtonDown((OpenTK.Input.MouseButton)KeysHelper.ConvertOut(Key).Item2);
                default:
                    throw new Exception(String.Format("The key '{0}' with the input device '{1}' needs to be added here you cunt. Stop messing everything up"));
            }
        }

        /// <summary>
        /// Contains available Keys - Used as a buffer for the GetAvailableKeys() method
        /// </summary>
        private static Dictionary<Tools.Keys, InputDevice> availableKeys { get; set; }

        /// <summary>
        /// Gathers a dictionary (string, InputDevice) of all available keys in opentk
        /// The dictionary is buffered so feel free to use this as often as you want
        /// </summary>
        /// <returns>String-array of all keys</returns>
        public static Dictionary<Tools.Keys, InputDevice> GetAvailableKeys()
        {
            // Leave if buffered already
            if (availableKeys != null) return availableKeys;

            // Run this to generate the first time
            availableKeys = new Dictionary<Tools.Keys, InputDevice>();

            foreach (var keyboardKey in Enum.GetValues(typeof(OpenTK.Input.Key)))
                availableKeys.Add(KeysHelper.ConvertIn((OpenTK.Input.Key)keyboardKey) , InputDevice.Keyboard);

            foreach (var mouseKey in Enum.GetValues(typeof(OpenTK.Input.MouseButton)))
                availableKeys.Add(KeysHelper.ConvertIn((OpenTK.Input.MouseButton)mouseKey), InputDevice.Mouse);

            return availableKeys;
        }
    }
}
