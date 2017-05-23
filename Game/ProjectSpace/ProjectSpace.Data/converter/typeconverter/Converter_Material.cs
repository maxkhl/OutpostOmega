using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace OutpostOmega.Data.converter
{
    /// <summary>
    /// Converts different types of quaternions (Unity and Jitter)
    /// </summary>
    public class Converter_JMaterial : cConverter
    {
        public Converter_JMaterial()
            : base()
        {
            SupportedTypes.Add(typeof(Jitter.Dynamics.Material));

            cID = (Int16)ConverterID.Converter_JMaterial;
            Options = new Converter_Options()
            {
                HandlesProperties = true
            };
        }


        /// <summary>
        /// Serializes the object
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>XML Structure</returns>
        public override XElement Serialize(string SenderID, object obj)
        {
            XElement newObject = base.Serialize(SenderID, obj);


            bool First = false;
            string ID = CheckObject(SenderID, obj, out First);
            
            Type objType = obj.GetType();

            if (objType == typeof(Jitter.Dynamics.Material))
            {
                var material = (Jitter.Dynamics.Material)obj;
                newObject.Add(new XAttribute("kFrict", FloatToString(material.KineticFriction)));
                newObject.Add(new XAttribute("rest", FloatToString(material.Restitution)));
                newObject.Add(new XAttribute("sFrict", FloatToString(material.StaticFriction)));
            }

            return newObject;
        }

        public override object Deserialize(string SenderID, XElement element)
        {
            Type type = GetType(element.Element(XPropType).Value);

            object returnobj = null;
            if (type == typeof(Jitter.Dynamics.Material))
            {
                returnobj = new Jitter.Dynamics.Material()
                {
                    KineticFriction = StringToFloat(element.Attribute("kFrict").Value),
                    Restitution = StringToFloat(element.Attribute("rest").Value),
                    StaticFriction = StringToFloat(element.Attribute("sFrict").Value)
                };
            }


            RegisterObject(SenderID, returnobj);
            return returnobj;
        }
    }
}
