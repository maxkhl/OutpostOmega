using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OutpostOmega.Game
{
    ///Filestructure:
    ///-DEFINITION/INFO-
    ///-MAP- 
    ///  -MAPDATA-
    ///-MAP-
    ///-INSTANCES-
    ///  -NAME-
    ///  -PROPERTIES-
    ///  
    ///-INSTANCES-
    ///
    ///Problems:
    ///How to save player? Best solution would be to save mobs like any other instance and allow re-entry from the lobby like in Civ 5. When Server started, there need to be a startpoint for the remaining playable mobs

    /// <summary>
    /// Handles serialization of every important instance of the game
    /// </summary>
    public partial class World
    {
        /// <summary>
        /// Fires every time a property of this object is changed
        /// Properties NEED to call NotifyPropertyChanged() after changes to make this work
        /// </summary>
        public event GameObject.GoPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Property changed - event. Gets called whenever a important property is changed. Mainly used for networking. Use the World.PropertyChanged-Event to access this
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">PropertyChangedEventArgs</param>
        public void World_PropertyChanged(GameObject sender, string PropertyName, bool MinorChange)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, PropertyName, MinorChange);
            }
        }
        
    }
}
