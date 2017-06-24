using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.Tools
{
    /// <summary>
    /// Executes a action in the game, this can be used by AI or players to control what's going on in the game
    /// </summary>
    public enum Action : byte
    {
        /// <summary>
        /// Nothing
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Move forward
        /// </summary>
        MoveForward = 1,


        /// <summary>
        /// Move backwarwd
        /// </summary>
        MoveBack = 2,


        /// <summary>
        /// Strafe right
        /// </summary>
        StrafeRight = 3,

        /// <summary>
        /// Strafe left
        /// </summary>
        StrafeLeft = 4,

        /// <summary>
        /// Primary interaction action
        /// </summary>
        InteractPrimary = 5,

        /// <summary>
        /// Secondary interaction action
        /// </summary>
        InteractSecondary = 6,

        /// <summary>
        /// Tertiary interaction action
        /// </summary>
        InteractTertiary = 7,

        /// <summary>
        /// What did you expect here
        /// </summary>
        Jump = 8,

        /// <summary>
        /// Inspect something
        /// </summary>
        Inspect = 9,

        /// <summary>
        /// Run faaast like the wind
        /// </summary>
        Run = 10,

        /// <summary>
        /// Quickslot 1 select action
        /// </summary>
        SelectQuickslot1 = 11,

        /// <summary>
        /// Quickslot 2 select action
        /// </summary>
        SelectQuickslot2 = 12,

        /// <summary>
        /// Quickslot 3 select action
        /// </summary>
        SelectQuickslot3 = 13,

        /// <summary>
        /// Quickslot 4 select action
        /// </summary>
        SelectQuickslot4 = 14,

        /// <summary>
        /// Quickslot 5 select action
        /// </summary>
        SelectQuickslot5 = 15,

        /// <summary>
        /// Quickslot 6 select action
        /// </summary>
        SelectQuickslot6 = 16,

        /// <summary>
        /// Quickslot 7 select action
        /// </summary>
        SelectQuickslot7 = 17,

        /// <summary>
        /// Quickslot 8 select action
        /// </summary>
        SelectQuickslot8 = 18,

        /// <summary>
        /// Quickslot 9 select action
        /// </summary>
        SelectQuickslot9 = 19,

        /// <summary>
        /// Toggles between WASD- and mousemode
        /// </summary>
        ToggleMouseMode = 20,

        /// <summary>
        /// Makes the characters mob fart
        /// </summary>
        Fart = 21,

        /// <summary>
        /// Drops the item in the currently active hand
        /// </summary>
        Drop = 22,
    }

    /// <summary>
    /// State of a action. Used to descibe changes
    /// </summary>
    public enum ActionState : byte
    {
        /// <summary>
        /// Nothing
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Activate a action, keeping it active
        /// </summary>
        Activate = 1,

        /// <summary>
        /// Release a action, changing it to inactive
        /// </summary>
        Release = 2,

        /// <summary>
        /// Quickly activate a action and then release it again
        /// </summary>
        Toggle = 3,
    }
}
