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
    public class Converter_JShape : cConverter
    {
        public Converter_JShape()
            : base()
        {
            SupportedTypes.Add(typeof(Jitter.Collision.Shapes.BoxShape));
            SupportedTypes.Add(typeof(Jitter.Collision.Shapes.CapsuleShape));
            SupportedTypes.Add(typeof(Jitter.Collision.Shapes.CylinderShape));
            SupportedTypes.Add(typeof(Jitter.Collision.Shapes.SphereShape));
            SupportedTypes.Add(typeof(Jitter.Collision.Shapes.ConeShape));
            SupportedTypes.Add(typeof(Jitter.Collision.Shapes.TriangleMeshShape));

            cID = (Int16)ConverterID.Converter_JShape;
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

            if (objType == typeof(Jitter.Collision.Shapes.BoxShape))
            {
                var box = (Jitter.Collision.Shapes.BoxShape)obj;
                newObject.Add(new XAttribute("X", FloatToString(box.Size.X)));
                newObject.Add(new XAttribute("Y", FloatToString(box.Size.Y)));
                newObject.Add(new XAttribute("Z", FloatToString(box.Size.Z)));
            }

            if (objType == typeof(Jitter.Collision.Shapes.CapsuleShape))
            {
                var capsule = (Jitter.Collision.Shapes.CapsuleShape)obj;
                newObject.Add(new XAttribute("length", FloatToString(capsule.Length)));
                newObject.Add(new XAttribute("radius", FloatToString(capsule.Radius)));
            }

            if (objType == typeof(Jitter.Collision.Shapes.CylinderShape))
            {
                var cylinder = (Jitter.Collision.Shapes.CylinderShape)obj;
                newObject.Add(new XAttribute("height", FloatToString(cylinder.Height)));
                newObject.Add(new XAttribute("radius", FloatToString(cylinder.Radius)));
            }

            if (objType == typeof(Jitter.Collision.Shapes.SphereShape))
            {
                var cylinder = (Jitter.Collision.Shapes.SphereShape)obj;
                newObject.Add(new XAttribute("radius", FloatToString(cylinder.Radius)));
            }

            if (objType == typeof(Jitter.Collision.Shapes.ConeShape))
            {
                var cone = (Jitter.Collision.Shapes.ConeShape)obj;
                newObject.Add(new XAttribute("height", FloatToString(cone.Height)));
                newObject.Add(new XAttribute("radius", FloatToString(cone.Radius)));
            }

            if (objType == typeof(Jitter.Collision.Shapes.TriangleMeshShape))
            {
                var tmesh = (Jitter.Collision.Shapes.TriangleMeshShape)obj;
                
                var Positions = new string[tmesh.octree.positions.Count()*3];
                for(int i = 0; i < tmesh.octree.positions.Count(); i++)
                {
                    Positions[i*3] = tmesh.octree.positions[i].X.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    Positions[i * 3 + 1] = tmesh.octree.positions[i].Y.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    Positions[i * 3 + 2] = tmesh.octree.positions[i].Z.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }

                newObject.Add(new XAttribute("positions", tmesh.octree.positions.Length));
                newObject.Add(new XElement("positions", string.Join(" ", Positions)));


                var Triangles = new string[tmesh.octree.tris.Length * 3];

                for(int i = 0; i < tmesh.octree.tris.Length; i++)
                {
                    Triangles[i*3]  = tmesh.octree.tris[i].I0.ToString();
                    Triangles[i * 3 + 1] = tmesh.octree.tris[i].I1.ToString();
                    Triangles[i * 3 + 2] = tmesh.octree.tris[i].I2.ToString();
                }

                newObject.Add(new XAttribute("triangles", tmesh.octree.tris.Length));
                newObject.Add(new XElement("triangles", string.Join(" ", Triangles)));
            }

            return newObject;
        }

        public override object Deserialize(string SenderID, XElement element)
        {
            Type type = GetType(element.Element(XPropType).Value);

            object returnobj = null;

            if (type == typeof(Jitter.Collision.Shapes.BoxShape))
            {
                returnobj = new Jitter.Collision.Shapes.BoxShape(
                    new Jitter.LinearMath.JVector(
                        StringToFloat(element.Attribute("X").Value),
                        StringToFloat(element.Attribute("Y").Value),
                        StringToFloat(element.Attribute("Z").Value)));
            }

            if (type == typeof(Jitter.Collision.Shapes.CapsuleShape))
            {
                returnobj = new Jitter.Collision.Shapes.CapsuleShape(
                    StringToFloat(element.Attribute("length").Value),
                    StringToFloat(element.Attribute("radius").Value));
            }

            if (type == typeof(Jitter.Collision.Shapes.CylinderShape))
            {
                returnobj = new Jitter.Collision.Shapes.CylinderShape(
                    StringToFloat(element.Attribute("height").Value),
                    StringToFloat(element.Attribute("radius").Value));
            }

            if (type == typeof(Jitter.Collision.Shapes.SphereShape))
            {
                returnobj = new Jitter.Collision.Shapes.SphereShape(
                    StringToFloat(element.Attribute("radius").Value));
            }

            if (type == typeof(Jitter.Collision.Shapes.ConeShape))
            {
                returnobj = new Jitter.Collision.Shapes.ConeShape(
                    StringToFloat(element.Attribute("height").Value),
                    StringToFloat(element.Attribute("radius").Value));
            }

            if (type == typeof(Jitter.Collision.Shapes.TriangleMeshShape))
            {

                string[] strpositions = element.Element("positions").Value.Split(' ');
                float[] positions = new float[strpositions.GetLongLength(0)];

                for (long i = 0; i < strpositions.GetLongLength(0); i++)
                {
                    positions[i] = float.Parse(strpositions[i], new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." });
                }

                string[] strtriangles = element.Element("triangles").Value.Split(' ');

                int mod = 0;
                if (strtriangles[strtriangles.Length - 1] == "")
                    mod = -1;

                int[] triangles = new int[strtriangles.GetLongLength(0) + mod];
                for (long i = 0; i < strtriangles.GetLongLength(0) + mod; i++)
                {
                    triangles[i] = Convert.ToInt32(strtriangles[i]);
                }


                List<Jitter.LinearMath.JVector> ocPositions = new List<Jitter.LinearMath.JVector>();
                for (int i = 0; i < positions.Length; i+=3)
                {
                    ocPositions.Add(new Jitter.LinearMath.JVector(
                        positions[i], positions[i + 1], positions[i + 2]));
                }

                List<Jitter.Collision.TriangleVertexIndices> ocTriangles = new List<Jitter.Collision.TriangleVertexIndices>();
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    ocTriangles.Add(new Jitter.Collision.TriangleVertexIndices(
                        triangles[i], triangles[i + 1], triangles[i + 2]));
                }


                returnobj = new Jitter.Collision.Shapes.TriangleMeshShape(
                    new Jitter.Collision.Octree(ocPositions, ocTriangles));
            }

            


            RegisterObject(SenderID, returnobj);
            return returnobj;
        }
    }
}
