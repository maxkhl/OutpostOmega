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
	/// Deserialization-part of the gameobject
	/// </summary>
	public partial class GameObject
    {
        /// <summary>
        /// Only for deserialization
        /// </summary>
        public GameObject()
        { }

        /// <summary>
        /// Will be called from the Converter when this object got deserialized.
        /// Basically a constructor for the converter without any parameters.
        /// </summary>
        public virtual void OnDeserialization()
        {
            if (World == null)
                throw new Exception("GameObject not deserialized properly");

            if (Shape != null && RigidBody == null)
                PhysicEnable();

            Register();

            Initialise();
        }
	}
}

