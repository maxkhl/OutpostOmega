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
    public class Converter_JQuaternion : cConverter
    {
        public Converter_JQuaternion()
            : base()
        {
            SupportedTypes.Add(typeof(Jitter.LinearMath.JQuaternion));

            cID = (Int16)ConverterID.Converter_JQuaternion;
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

            if (objType == typeof(Jitter.LinearMath.JQuaternion))
            {
                var quaternion = (Jitter.LinearMath.JQuaternion)obj;
                newObject.Add(new XAttribute("X", FloatToString(quaternion.X)));
                newObject.Add(new XAttribute("Y", FloatToString(quaternion.Y)));
                newObject.Add(new XAttribute("Z", FloatToString(quaternion.Z)));
                newObject.Add(new XAttribute("W", FloatToString(quaternion.W)));
            }

            return newObject;
        }

        public override object Deserialize(string SenderID, XElement element)
        {
            Type type = GetType(element.Element(XPropType).Value);

            object returnobj = null;
            if (type == typeof(Jitter.LinearMath.JQuaternion))
            {
                returnobj = new Jitter.LinearMath.JQuaternion()
                {
                    X = StringToFloat(element.Attribute("X").Value),
                    Y = StringToFloat(element.Attribute("Y").Value),
                    Z = StringToFloat(element.Attribute("Z").Value),
                    W = StringToFloat(element.Attribute("W").Value)
                };
            }


            RegisterObject(SenderID, returnobj);
            return returnobj;
        }
    }
}
