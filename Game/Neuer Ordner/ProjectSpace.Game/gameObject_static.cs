using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Globalization;

namespace OutpostOmega.Game
{
    /// <summary>
    /// Static gameObject methods and information
    /// </summary>
    public partial class GameObject
    {
        /// <summary>
        /// Generates a completely new gameobject and tries to feed the constructor. This can cause a crash pretty easy so be careful with it!
        /// </summary>
        /// <param name="Type">The type of the gameObject</param>
        /// <param name="Type">The world it should be spawned in</param>
        /// <returns></returns>
        public static GameObject GenerateNew(Type gOType, World world)
        {
            if (gOType == null)
                throw new NullReferenceException();

            if (gOType.IsAbstract)
                throw new Exception("Abstract classes can not be generated. Wtf dude you should know that!");

            if (!typeof(GameObject).IsAssignableFrom(gOType))
                throw new Exception("Must be a gameObject type!");

            // Now we can start
            GameObject result = null;

            // Space to do individual generation


            // We got a structure!
            if (result == null && typeof(GameObjects.Structures.Structure).IsAssignableFrom(gOType))
            {
                // Process individual structures if you want to

                
                // Make a default generation for structures
                if(result == null)
                {
                    //try
                    //{
                        result = (GameObject)Activator.CreateInstance(gOType,
                            BindingFlags.CreateInstance |
                            BindingFlags.Public |
                            BindingFlags.Instance |
                            BindingFlags.OptionalParamBinding,
                            null,
                            new object[] { 0, 0, 0, world.Structures[0], world, Type.Missing },
                            CultureInfo.CurrentCulture);
                    //}
                    //catch
                    //{ }
                }
            }

            
            // If no generation happened, we try it with a universal method
            if(result == null)
            {
                // This stuff is VERY hacky and unstable but I see no other way here
                // Lets feed the default constructor. Everything else will crash
                //try
                //{
                    result = (GameObject)Activator.CreateInstance(gOType,
                        BindingFlags.CreateInstance |
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.OptionalParamBinding,
                        null,
                        new object[] { world, Type.Missing },
                        CultureInfo.CurrentCulture);
                //}
                //catch
                //{
                //    result = null;
                //}
            }

            // Result still null and did not crash till now? Let it crash
            //if(result == null)
            //    throw new Exception("GameObject generation failed!");

            return result;
        }
    }
}
