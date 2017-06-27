using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutpostOmega.Network
{
    /// <summary>
    /// Network commands for server/client communication
    /// </summary>
    public enum Command : byte
    {
        /// <summary>
        /// Login command
        /// </summary>
        Login = 20,

        /// <summary>
        /// Create command - used to create a specific object
        /// </summary>
        Create = 25,

        /// <summary>
        /// Data update command
        /// </summary>
        Data = 26,

        /// <summary>
        /// Delete command - used to delete a specific object
        /// </summary>
        Delete = 27,

        /// <summary>
        /// Message command - used to send messages
        /// </summary>
        Message = 29,
    }


    /// <summary>
    /// Second-byte command
    /// </summary>
    public enum SecondCommand : byte
    {
        /// <summary>
        /// No second command
        /// </summary>
        Null = 0,

        /// <summary>
        /// Confirmation
        /// </summary>
        Confirmed = 1,

        /// <summary>
        /// Declination
        /// </summary>
        Declined = 2,

        /// <summary>
        /// Request
        /// </summary>
        Request = 3,

        /// <summary>
        /// Block
        /// </summary>
        CreateBlock = 7,

        /// <summary>
        /// Block
        /// </summary>
        RemoveBlock = 8,

        /// <summary>
        /// GameObject
        /// </summary>
        GameObject = 3,

        /// <summary>
        /// Input (Keys)
        /// </summary>
        Input = 4,

        /// <summary>
        /// Input (Mouse)
        /// </summary>
        InputMouseDelta = 5
    }

    /// <summary>
    /// Second-byte command
    /// </summary>
    public enum InputType : byte
    {
        /// <summary>
        /// No type.. like.. idk..
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Action trigger
        /// </summary>
        Action = 1,

        /// <summary>
        /// Mouse coordinates
        /// </summary>
        Mouse = 2,

        /// <summary>
        /// Position >> THIS IS TEMPORARY AND NEEDS TO BE REPLACED BY A REAL SYSTEM
        /// </summary>
        Position = 8,

        /// <summary>
        /// Orientation >> THIS IS TEMPORARY AND NEEDS TO BE REPLACED BY A REAL SYSTEM
        /// </summary>
        Orientation = 9,

        /// <summary>
        /// Used to separate input packages
        /// </summary>
        //Separator = 255,
    }
}
