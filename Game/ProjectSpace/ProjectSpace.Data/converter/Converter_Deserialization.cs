using System;
using System.Collections;
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
    /// Deserialization-part of the Converter class. Contains everything that handles deserialization
    /// </summary>
    public abstract partial class cConverter
    {
        /// <summary>
        /// Deserializes a object that fits to this converter. Make sure to check the compatibility first (CheckCompatibility())
        /// </summary>
        /// <param name="stream">Stream that contains the object. Make sure the pointer is at the correct position</param>
        /// <returns>Deserialized object</returns>
        public virtual object Deserialize(string senderID, XElement element)
        {
            if (Options.HandlesProperties)
                throw new Exception("Converter uses default Deserialization but HandlesProperties is set to true. Default Deserializer is unable to handle the properties. Please override this method or switch the option to false.");

            XElement XTypeElem = element.Element(XPropType);

            Type objType = null;
            if(XTypeElem != null)
            {
                objType = GetType(XTypeElem.Value);
            }

            if (objType != null)
            {
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                object retObj = Activator.CreateInstance(objType, flags, null, new object[] { null }, null);
                RegisterObject(senderID, retObj);

                return retObj;
            }
            else
                return null;
        }


        /// <summary>
        /// Used to deserialize a object from a stream. This method will automaticaly determine the correct converter
        /// </summary>
        /// <param name="stream">Stream that contains the object. Make sure the pointer is at the correct position</param>
        /// <returns>Deserialized object</returns>
        public static object DeserializeObject(string SenderID, XElement element, PropertyInfo parentProperty = null, object Parent = null)
        {
            if (element.Name == "null")
                return null;

            LoadConverter();

            bool isList = element.Attribute(XPropAttrList) != null &&
                element.Attribute(XPropAttrList).Value == XPropTrue;

            Type listType = null;

            List<dynamic> objects = new List<dynamic>();
            List<XElement> Elements = new List<XElement>();
            if (!isList)
                Elements.Add(element);
            else
            {
                listType = Type.GetType(element.Element(XPropType).Value);

                foreach (XElement elem in element.Elements())
                    if(elem.Name != XPropType)
                        Elements.Add(elem);
            }

            foreach (XElement elem in Elements)
            {
                if (elem.Name == "null")
                {
                    objects.Add(null);
                    continue;
                }

                // Get converter
                cConverter targetConverter = (from conv in AllConverter
                                             where conv.CheckCompatibility(elem)
                                             select conv).FirstOrDefault();

                if (targetConverter == null)
                    continue;

                
                string InstanceID = "";
                // Get Instance (when already deserialized)
                if (elem.Attribute(XPropAttrInst) != null)
                {
                    InstanceID = elem.Attribute(XPropAttrInst).Value;
                    if (targetConverter.HasInstance(SenderID, InstanceID))
                    {
                        objects.Add(targetConverter.GetInstance(SenderID, InstanceID));
                        continue;
                    }
                }


                // Deserialize
                object obj = targetConverter.TryDeserialize(SenderID, elem, parentProperty, Parent);

                if (obj == null)
                    continue;
                else if(InstanceID != "")
                {
                    // Check for unresolved references
                    targetConverter.TryResolveReferences(InstanceID, obj);
                }


                // Handle properties (if converter doesn't handle it)
                if (!targetConverter.Options.HandlesProperties)
                {
                    PropertyInfo[] props = obj.GetType().GetProperties();
                    foreach (PropertyInfo property in props)
                    {
                        if (elem.Element(XPropSub) == null)
                        {
                            //Finaly stopped this mess. Unresolvable references are now stored in a separate section and 
                            //are being tried to resolve at the end of the deserialization process.
                            //In case of modded objects, the late decompilation has to be executed after the creation of the addon assembly
                            targetConverter.EnqueueReference(SenderID, elem.Attribute(XPropAttrInst).Value, property, obj, elem);
                            continue; // Skip this for now
                            //throw new Exception("Unable to resolve a reference.");

                        }

                        XElement propElement = elem.Element(XPropSub).Element(property.Name);
                        if (propElement == null)
                            continue;

                        object propValue = null;

                        // Dictionary
                        if (property.PropertyType.IsGenericType &&
                            property.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            var xKeyElement = propElement.Element("keys");
                            var xValueElement = propElement.Element("values");

                            var keyPart = (List<object>)DeserializeObject(SenderID, xKeyElement);
                            var valuePart = (List<object>)DeserializeObject(SenderID, xValueElement);


                            IDictionary newDict = (IDictionary)Activator.CreateInstance(property.PropertyType, new object[] { });
                            for (int i = 0; i < keyPart.Count(); i++)
                                newDict.Add(keyPart[i], valuePart[i]);

                            propValue = newDict;
                        }
                        else // Default Deserialization
                            propValue = DeserializeObject(SenderID, propElement, property, obj); //DeserializeObject(SenderID, propElement);

                        if (propValue == null)
                            continue;

                        //Convert it to the right list
                        if (property.PropertyType.IsGenericType)
                        {
                            if (property.PropertyType.GetGenericTypeDefinition() == typeof(List<>) ||
                                property.PropertyType.GetGenericTypeDefinition() == typeof(ObservableCollection<>))
                            {
                                List<object> objList = (List<object>)propValue;
                                var objArray = Array.CreateInstance(property.PropertyType.GetProperty("Item").PropertyType, objList.Count);
                                for (int i = 0; i < objList.Count; i++)
                                    objArray.SetValue(objList[i], i);
                                propValue = Activator.CreateInstance(property.PropertyType, new object[] { objArray });
                            }
                            //newList[0] = propValue;
                        }
                        //property.SetValue(,,
                        if (property.CanWrite)
                        {
                            if (property.PropertyType.IsAssignableFrom(propValue.GetType()))
                                property.SetValue(obj, propValue);
                            else
                                throw new Exception(String.Format("Could not set property {0} of {1} using {2} ({3})", property.Name, obj.ToString(), propValue.ToString(), propValue.GetType().Name));

                            //propValue = DeserializeObject(SenderID, propElement, property, obj); //DeserializeObject(SenderID, propElement);
                        }
                    }
                }

                var dMethod = obj.GetType().GetMethod("OnDeserialization");
                if(dMethod != null && dMethod.GetParameters().Length == 0)
                {
                    dMethod.Invoke(obj, null);
                }

                objects.Add(obj);
            }

            if (objects.Count == 0)
                return null;
            else if (isList)
                return objects;
            else
                return objects[0];


        }
        private static IEnumerable<DictionaryEntry> CastDict(IDictionary dictionary)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                yield return entry;
            }
        }

        private object TryDeserialize(string SenderID, XElement element, PropertyInfo property, object parent)
        {
            if(element.Element(XPropType) != null) // IF a type is given
            {
                Type type = GetType(element.Element(XPropType).Value);
                if(type == null && element.Attribute(XPropAttrList) == null) // AND the type cant be resolved
                {
                    // Note it for later derserialization when all mods are loaded
                    DataHandler.UnloadedObjects.Add(new UnidentifiedReference()
                        {
                            Data = element,
                            Sender = SenderID,
                            Identity = element.Attribute(XPropAttrInst) != null ? element.Attribute(XPropAttrInst).Value : "",
                            TargetInstance = parent,
                            TargetProperty = property,
                        });
                    return null;
                }
            }

            object instance = null;
            if(element.Attribute(XPropAttrInst) != null)
            {
                instance = GetInstance(SenderID, element.Attribute(XPropAttrInst).Value);
                if (instance != null)
                    return instance;
            }
                

            return Deserialize(SenderID, element);
        }
    }
}
