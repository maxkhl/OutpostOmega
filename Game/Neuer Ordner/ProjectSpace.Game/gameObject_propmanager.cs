using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OutpostOmega.Game
{
    /// <summary>
    /// Propertymanagement! Mainly used for network synchonization. Call NotifyPropertyChanged() in your properties set proc to implement synchronization.
    /// </summary>
    public partial class GameObject
    {
        public delegate void GoPropertyChangedEventHandler(GameObject Object, string PropertyName, bool IndirectChange);

        /// <summary>
        /// Fires every time a property of this object is changed
        /// Properties NEED to call NotifyPropertyChanged() after changes to make this work
        /// </summary>
        public event GoPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called by properties to fire the PropertyChanged-Event
        /// </summary>
        /// <param name="IndirectChange">Tells if the raise came from the object itself or is a hierarchical change</param>
        protected void NotifyPropertyChanged([CallerMemberName]String propertyName = "", bool IndirectChange = false)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, propertyName, IndirectChange);
            }
        }
    }
}
