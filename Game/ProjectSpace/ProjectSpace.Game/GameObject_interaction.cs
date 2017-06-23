using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using Jitter.LinearMath;

namespace OutpostOmega.Game
{
	/// <summary>
	/// Interaction-part of a gameobject
	/// </summary>
	public partial class GameObject
    {
        /// <summary>
        /// Determins whether this object is interactable.
        /// </summary>
        public bool CanInteract { get; set; }

        /// <summary>
        /// Gets called by a mob when this object is interacted with
        /// </summary>
        public virtual bool Use(GameObjects.Mob User, GameObjects.Item Item, Tools.Action Action)
        {
            if (!CanInteract)
                return false;
            else
                return true;
        }

        //public enum UseAction
        //{
        //    /// <summary>
        //    /// Primary interaction (left mouse usualy)
        //    /// </summary>
        //    Primary,
        //    /// <summary>
        //    /// Secondary interaction (right mouse)
        //    /// </summary>
        //    Secondary,
        //    /// <summary>
        //    /// Tertiary interaction (middle mouse)
        //    /// </summary>
        //    Tertiary,
        //    /// <summary>
        //    /// Inspect interaction (e)
        //    /// </summary>
        //    Inspect,
        //}
	}
}

