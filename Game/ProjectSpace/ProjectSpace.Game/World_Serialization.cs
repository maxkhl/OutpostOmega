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

        /// <summary>
        /// Returns a string that contains all data about this world in a readable format
        /// </summary>
        public string GetDebugString()
        {
            string outstr = "";
            outstr += this.ToString() + Environment.NewLine;

            outstr += string.Format("{3} \nLoaded Content Objects: {0}\nLoaded Modpacks: {1}\nClientmode: {2}\nWorld ID: {3}\nGameObjects: {4}\nDatums: {5}\nDebug Mode: {6}\nPlayer: {7}\nStructures: {8}\n",
                this.ContentManager.LoadedContent.Count, //0
                this.Mods.Count, //1
                this.ClientMode, //2
                this.ID, //3
                this.AllGameObjects.Count, //4
                this.Datums.Count, //5
                this.Debug, //6
                this.Player.ID, //7
                this.Structures.Count); //8

            outstr += "-------------------------------------------" + Environment.NewLine;
            outstr += "--------------LOADED CONTENT---------------" + Environment.NewLine;
            for (int i = 0; i < this.ContentManager.LoadedContent.Count; i++ )
            {
                var content = this.ContentManager.LoadedContent[i];
                outstr += "-------------------------------------------" + Environment.NewLine;
                outstr += string.Format("{0} \nCreation: {1}\nRead only: {2}\nLength: {3}\nLoaded: {4}\n", 
                    content.Path, 
                    content.FileInfo.CreationTime.ToShortDateString(), 
                    content.FileInfo.IsReadOnly.ToString(), 
                    content.FileInfo.Length.ToString(),
                    content.Loaded.ToString());
                
            }

            outstr += "-------------------------------------------" + Environment.NewLine;
            outstr += "-------------LOADED MODPACKS---------------" + Environment.NewLine;
            for (int i = 0; i < this.Mods.Count; i++)
            {
                var mod = this.Mods[i];
                outstr += "-------------------------------------------" + Environment.NewLine;
                outstr += string.Format("{0} \nVersion/Author: {6}/{2}\nStartup: {1}\nID: {5}\nFolder: {4}\nConfig: {3}\n",
                    mod.Name,
                    mod.StartupScript,
                    mod.Author,
                    mod.ConfigFile.FullName,
                    mod.Folder,
                    mod.ID,
                    mod.Version);
            }


            outstr += "-------------------------------------------" + Environment.NewLine;
            outstr += "---------------GAMEOBJECTS-----------------" + Environment.NewLine;
            for (int i = 0; i < AllGameObjects.Count; i++)
            {
                var GameObject = AllGameObjects[i];

                outstr += "-------------------------------------------" + Environment.NewLine;
                outstr += GameObject.ToString() + Environment.NewLine;

                var properties = GameObject.GetType().GetProperties();
                foreach (var property in properties)
                {
                    if (!property.CanWrite) continue; // unwriteable properties cant be de-/serialized
                    object value = property.GetValue(GameObject);
                    outstr += property.Name + ":" + (value == null ? "null" : value.ToString()) + Environment.NewLine;
                }
            }
            return outstr;
        }        
    }
}
