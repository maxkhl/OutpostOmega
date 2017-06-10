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
        Activate = 0,

        /// <summary>
        /// Release a action, changing it to inactive
        /// </summary>
        Release = 1,

        /// <summary>
        /// Quickly activate a action and then release it again
        /// </summary>
        Toggle = 2,
    }
}
