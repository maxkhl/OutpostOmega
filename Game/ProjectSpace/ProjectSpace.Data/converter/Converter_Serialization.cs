using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace OutpostOmega.Data
{
    /// <summary>
    /// Serialization-part of the Converter class. Contains everything that handles serialization
    /// </summary>
    public abstract partial class cConverter
    {
        /// <summary>
        /// Serializes a object that fits to this converter. Make sure to check the compatibility first (CheckCompatibility())
        /// </summary>
        /// <param name="obj">The object, that should be serialized</param>
        /// <returns>Serialized object as XElement. null = error</returns>
        public virtual XElement Serialize(string SenderID, object obj)
        {
            XElement element = new XElement(SanitizeString(obj.GetType().Name.ToLower()));

            WriteCID(element);

            element.Add(new XElement(XPropType, obj.GetType().FullName));

            return element;
        }

        /// <summary>
        /// Used to serialize a object (mainly gameobjects). Check OutpostOmega.Data.converter for compatibilty
        /// </summary>
        public static XElement SerializeObject(string SenderID, object obj, string Name)
        {
            if (obj == null)
                return new XElement("null");

            LoadConverter();

            if (typeof(OutpostOmega.Game.GameObject).IsAssignableFrom(obj.GetType()))
            { }

            if (Name == "")
                return null;

            bool handlesProps = false;

            cConverter conv = (from lconv in AllConverter
                              where lconv.CheckCompatibility(obj)
                              select lconv).FirstOrDefault();

            XElement Result = null;
            bool firstTime = false;
            if(conv != null)
            {
                conv.CheckObjectPeek(SenderID, obj, out firstTime);
                handlesProps = conv.Options.HandlesProperties;

                // List
                if (obj.GetType().IsGenericType)
                {
                    Result = SerializeList(SenderID, obj, Name);
                }
                else
                    Result = conv.Serialize(SenderID, obj);

                Result.Name = Name;
            }                

            if (Result == null)
                return null;

            if (!handlesProps && firstTime)
            {
                PropertyInfo[] props = obj.GetType().GetProperties();

                XElement Xprops = new XElement(XPropSub);
                foreach (PropertyInfo property in props)
                {
                    
                    if (!property.CanWrite)
                        continue; //No need to serialize a read only property

                    var serializeAttr = (Game.GameObjects.Attributes.Serialize)property.GetCustomAttribute(typeof(Game.GameObjects.Attributes.Serialize));
                    if (serializeAttr != null && serializeAttr.DoSerialize || serializeAttr == null)
                    {
                        if (property.GetIndexParameters().Length == 0)
                        {
                            object value = property.GetValue(obj, null);
                            if (value == null)
                                continue;

                            if (value.GetType().IsGenericType)
                            {
                                Xprops.Add(SerializeList(SenderID, value, property.Name));
                            }
                            else
                            {
                                //TODO Smart Error Handling
                                Xprops.Add(cConverter.SerializeObject(SenderID, value, property.Name));
                            }
                        }
                    }
                }


                if (Xprops.Elements().Count() > 0)
                    Result.Add(Xprops);
            }
            return Result;
        }

        private static XElement SerializeList(string SenderID, object ListObject, string Name)
        {
            if (!ListObject.GetType().IsGenericType)
                return null;

            XElement xlist = new XElement(Name);

            xlist.Add(new XAttribute(XPropAttrList, XPropTrue));
            xlist.Add(new XElement(XPropType, ListObject.GetType().GetGenericTypeDefinition()));

            // Default List
            if (ListObject.GetType().GetGenericTypeDefinition() == typeof(List<>) ||
                ListObject.GetType().GetGenericTypeDefinition() == typeof(ObservableCollection<>) ||
                ListObject.GetType().GetGenericTypeDefinition() == typeof(System.Collections.Generic.Dictionary<,>.ValueCollection) ||
                ListObject.GetType().GetGenericTypeDefinition() == typeof(System.Collections.Generic.Dictionary<,>.KeyCollection))
            {
                foreach (object subvalue in (IEnumerable<object>)ListObject)
                {
                    xlist.Add(cConverter.SerializeObject(SenderID, subvalue, Name));
                }
            }
            // Dictionary
            else if (ListObject.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                var propKeys = ListObject.GetType().GetProperty("Keys");
                var propValues = ListObject.GetType().GetProperty("Values");

                // Keys
                xlist.Add(cConverter.SerializeObject(SenderID, (IEnumerable<object>)propKeys.GetValue(ListObject), "keys"));

                // Values
                xlist.Add(cConverter.SerializeObject(SenderID, (IEnumerable<object>)propValues.GetValue(ListObject), "values"));
            }
            // Lists that can be ignored
            else if (ListObject.GetType().GetGenericTypeDefinition() == typeof(Jitter.DataStructures.ReadOnlyHashset<>))
            {

            }
            else
            {
                throw new Exception("Unknown List. Unable to serialize. (" + ListObject.GetType().GetGenericTypeDefinition().ToString() + ")");
            }
            return xlist;
        }

    }
}
