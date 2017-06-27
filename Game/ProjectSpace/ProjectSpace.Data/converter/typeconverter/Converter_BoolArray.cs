using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Linq;
using OutpostOmega.Game.Turf;

namespace OutpostOmega.Data.converter
{
    /// <summary>
    /// Used to convert the World class
    /// </summary>
    public class Converter_BoolArray : cConverter
    {
        public Converter_BoolArray()
            : base()
        {
            SupportedTypes.Add(typeof(bool[,,]));
            cID = (Int16)ConverterID.Converter_BoolArray;
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
            newObject.Add(new XAttribute(XPropAttrInst, ID));

            var bArray = (bool[,,])obj;

            //We store the boolData as byte to be able to apply compression
            byte[] boolData = new byte[bArray.GetLength(0) * bArray.GetLength(1) * bArray.GetLength(2)];

            for (int x = 0; x < bArray.GetLength(0); x++)
                for (int y = 0; y < bArray.GetLength(1); y++)
                    for (int z = 0; z < bArray.GetLength(2); z++)
                        boolData[x + bArray.GetLength(0) * (y + bArray.GetLength(1) * z)] = SerializeValue(bArray[x, y, z]);

            newObject.Add(new XElement(XPropData, Convert.ToBase64String(Compress(boolData))));

            newObject.Add(new XAttribute("dX", bArray.GetLength(0)));
            newObject.Add(new XAttribute("dY", bArray.GetLength(1)));
            newObject.Add(new XAttribute("dZ", bArray.GetLength(2)));

            return newObject;
        }
        
        private byte SerializeValue(bool value)
        {
            if (value)
                return 1;
            else
                return 0;
        }

        private bool DeserializeValue(byte value)
        {
            if (value == 0)
                return false;
            else //Everything except 0 is true
                return true;
        }

        // Default should work aswell
        public override object Deserialize(string SenderID, XElement element)
        {
            var boolRawData = Decompress(Convert.FromBase64String(element.Element(XPropData).Value));

            var boolData = new bool[int.Parse(element.Attribute("dX").Value), int.Parse(element.Attribute("dY").Value), int.Parse(element.Attribute("dZ").Value)];
            for (byte x = 0; x < boolData.GetLength(0); x++)
                for (byte y = 0; y < boolData.GetLength(1); y++)
                    for (byte z = 0; z < boolData.GetLength(2); z++)
                        boolData[x, y, z] = DeserializeValue(boolRawData[x + boolData.GetLength(0) * (y + boolData.GetLength(1) * z)]);


            RegisterObject(SenderID, boolData);

            return boolData;
        }
    }
}
