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
    /// Converts different types of matrices (Unity and Jitter)
    /// </summary>
    public class Converter_JMatrix : cConverter
    {
        public Converter_JMatrix()
            : base()
        {
            SupportedTypes.Add(typeof(Jitter.LinearMath.JMatrix));

            cID = (Int16)ConverterID.Converter_JMatrix;
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
            
            if (objType == typeof(Jitter.LinearMath.JMatrix))
            {
                var matrix = (Jitter.LinearMath.JMatrix)obj;
                //matrix.

                /*double thetaY = 0,
                      thetaX = 0,
                      thetaZ = 0;

                if (matrix.M13 < 1)
                {
                    if(matrix.M13 > -1)
                    {
                        thetaY = Math.Asin(matrix.M13);
                        thetaX = Math.Atan2(-matrix.M23, matrix.M33);
                        thetaZ = Math.Atan2(-matrix.M12, matrix.M11);
                    }
                    else
                    {
                        thetaY = -Math.PI/2;
                        thetaX = -Math.Atan2(-matrix.M11, matrix.M12);
                        thetaZ = 0;
                    }
                }
                else
                {
                    thetaY = Math.PI / 2;
                    thetaX = Math.Atan2(-matrix.M11, matrix.M12);
                    thetaZ = 0;
                }*/




                newObject.Add(new XAttribute("dat", 
                    FloatToString(matrix.M11) + " " +
                    FloatToString(matrix.M12) + " " +
                    FloatToString(matrix.M13) + " " +
                    FloatToString(matrix.M21) + " " +
                    FloatToString(matrix.M22) + " " +
                    FloatToString(matrix.M23) + " " +
                    FloatToString(matrix.M31) + " " +
                    FloatToString(matrix.M32) + " " +
                    FloatToString(matrix.M33)));
            }

            return newObject;
        }

        public override object Deserialize(string SenderID, XElement element)
        {
            Type type = GetType(element.Element(XPropType).Value);

            object returnobj = null;
            if (type == typeof(Jitter.LinearMath.JMatrix))
            {
                var data = element.Attribute("dat").Value.Split(' ');
                returnobj = new Jitter.LinearMath.JMatrix()
                {

                    M11 = StringToFloat(data[0]),
                    M12 = StringToFloat(data[1]),
                    M13 = StringToFloat(data[2]),

                    M21 = StringToFloat(data[3]),
                    M22 = StringToFloat(data[4]),
                    M23 = StringToFloat(data[5]),

                    M31 = StringToFloat(data[6]),
                    M32 = StringToFloat(data[7]),
                    M33 = StringToFloat(data[8]),
                };
            }


            RegisterObject(SenderID, returnobj);
            return returnobj;
        }
    }
}
