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
    /// Converts different types of vectors (Unity and Jitter)
    /// </summary>
    public class Converter_JVector : cConverter
    {
        public Converter_JVector()
            : base()
        {
            SupportedTypes.Add(typeof(Jitter.LinearMath.JVector));
            SupportedTypes.Add(typeof(Jitter.LinearMath.JVector2));

            cID = (Int16)ConverterID.Converter_JVector;
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

            if (objType == typeof(Jitter.LinearMath.JVector))
            {
                Jitter.LinearMath.JVector vector = (Jitter.LinearMath.JVector)obj;
                newObject.Add(new XAttribute("X", FloatToString(vector.X)));
                newObject.Add(new XAttribute("Y", FloatToString(vector.Y)));
                newObject.Add(new XAttribute("Z", FloatToString(vector.Z)));

                if (vector.X == 17 && vector.Y == 17) 
                { }
            }
            else if (objType == typeof(Jitter.LinearMath.JVector2))
            {
                Jitter.LinearMath.JVector2 vector = (Jitter.LinearMath.JVector2)obj;
                newObject.Add(new XAttribute("X", FloatToString(vector.X)));
                newObject.Add(new XAttribute("Y", FloatToString(vector.Y)));
            }

            return newObject;
        }

        public override object Deserialize(string SenderID, XElement element)
        {
            Type type = GetType(element.Element(XPropType).Value);

            object returnobj = null;
            if (type == typeof(Jitter.LinearMath.JVector))
            {
                returnobj = new Jitter.LinearMath.JVector()
                {
                    X = StringToFloat(element.Attribute("X").Value),
                    Y = StringToFloat(element.Attribute("Y").Value),
                    Z = StringToFloat(element.Attribute("Z").Value)
                };
            }
            else if (type == typeof(Jitter.LinearMath.JVector2))
            {
                returnobj = new Jitter.LinearMath.JVector2()
                {
                    X = StringToFloat(element.Attribute("X").Value),
                    Y = StringToFloat(element.Attribute("Y").Value)
                };
            }


            RegisterObject(SenderID, returnobj);
            return returnobj;
        }
    }
}
