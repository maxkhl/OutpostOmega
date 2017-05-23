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
        /// Chunk
        /// </summary>
        Chunk = 3,

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
        InputMouseDelta = 5,

        /// <summary>
        /// Position (Hotfix prediction problem)
        /// </summary>
        Position = 6,

        /// <summary>
        /// Orientation (Hotfix prediction problem)
        /// </summary>
        Orientation = 7
    }
}
