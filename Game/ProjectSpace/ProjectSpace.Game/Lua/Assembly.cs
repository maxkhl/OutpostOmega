using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuaInterface;

namespace OutpostOmega.Game.Lua
{
    public class Assembly : Parser
    {
        public Queue<Message> Output;
        public struct Message
        {
            public string Text;
            public DateTime TimeStamp;
            public string Sender;
            public bool Error;

            public static string Format(Message message)
            {
                return string.Format("{0} {1}: {2}", message.TimeStamp.ToShortTimeString(), message.Sender, message.Text);
            }
        }

        public World world;
        public Assembly(World world) : base()
        {
            this.world = world;
            Output = new Queue<Message>();

            GeneralFunctions();
            AddonFunctions();
            ContentFunctions();
            WorldFunctions();
            MathFunctions();
            PhysicFunctions();

            /*var methods = typeof(Assembly).GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            List<LuaDocumentationAttr> Attributes = new List<LuaDocumentationAttr>();
            foreach(var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(LuaDocumentationAttr), false);
                if(attributes.Length == 1)
                {
                    ((LuaDocumentationAttr)attributes[0]).MethodInfo = method;
                    Attributes.Add((LuaDocumentationAttr)attributes[0]);
                }
            }
            Attributes = Attributes.OrderBy(o => o.Category).ToList();
            string OldCategory = "";
            string Output = "";
            foreach(var attribute in Attributes)
            {
                if(OldCategory != attribute.Category)
                {
                    
                    Output += "####" + new String('#', attribute.Category.Length) + "####" + Environment.NewLine;
                    Output += "### " + " ###";
                    Output += "####" + new String('#', attribute.Category.Length) + "####" + Environment.NewLine; 
                }
                OldCategory = attribute.Category;

                Output += attribute.MethodInfo.ToString() + Environment.NewLine;
                Output += "Description: " + attribute.Description + Environment.NewLine;
                Output += "Parameters: " + attribute.Parameters + Environment.NewLine;
                Output += "Returns: " + attribute.Return + Environment.NewLine;
                Output += Environment.NewLine;
            }*/


        }

        #region ErrorHandling
        public override void Execute(string Command)
        {
            try
            {
                base.Execute(Command);
            }
            catch (Exception e)
            {
                print(e.Message, "InnerException", true);
            }

            //Check for Script errors
            while (LuaInterface.Lua.ExceptionQueue.Count > 0)
                ScriptException(LuaInterface.Lua.ExceptionQueue.Dequeue());

        }
        public override void ExecuteFile(string FileName)
        {
            try
            {
                base.ExecuteFile(FileName);
            }
            catch (Exception e)
            {
                print(e.Message, "InnerException", true);
            }


            //Check for Script errors
            while (LuaInterface.Lua.ExceptionQueue.Count > 0)
                ScriptException(LuaInterface.Lua.ExceptionQueue.Dequeue());
        }
        private void ScriptException(LuaInterface.LuaException e)
        {
            print(string.Format("{0} in {1} {2}", e.Message, e.Source, e.InnerException != null ? e.InnerException.Message : ""), "ScriptError", true);
        }
        #endregion
        

        #region General
        private void GeneralFunctions()
        {
            this.RegisterFunction("print", this, this.GetType().GetMethod("print", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("hook", this, this.GetType().GetMethod("hook", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("getType", this, this.GetType().GetMethod("getType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        }

        [LuaDocumentationAttr(
            "General Commands",
            "Prints a message to the console",
            "",
            "1 Text: The text you want to display\n" +
            "2 Sender: Display name of the sender (optional)\n" +
            "3 Error: Whether this is a error message or not (optional)")]
        protected void print(object Text, string Sender = "System", bool Error = false)
        {
            if (Text != null && Text.ToString() != "")
            {
                Output.Enqueue(new Message()
                {
                    Text = Text.ToString(),
                    Sender = Sender,
                    TimeStamp = DateTime.Now,
                    Error = Error
                });
            }
        }

        [LuaDocumentationAttr(
            "General Commands",
            "Used to call a lua function from the engine (just for testing)",
            "",
            "1 Function: A function that should be called")]
        protected void hook(object Function)
        {
            var func = (LuaInterface.LuaFunction)Function;
            func.Call();
        }

        [LuaDocumentationAttr(
            "General Commands",
            "Tries to find a certain type from the given string",
            "Found Type-Object (raises an exception if nothing found - internal error)",
            "1 TypeName: Name of the type that should be searched")]
        protected Type getType(string TypeName)
        {
            var type = Type.GetType((string)TypeName);
            if (type == null)
                throw new Exception(string.Format("Could not find type '{0}'", TypeName));
            else
                return type;
        }
        #endregion


        #region Addon
        public void AddonFunctions()
        {
            this.RegisterFunction("NewBuilder", this, this.GetType().GetMethod("NewBuilder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            
            this.DoString("g_type_structure = \"OutpostOmega.Game.GameObjects.Structure\"");
            this.DoString("g_type_machine = \"OutpostOmega.Game.GameObjects.Structures.Machine\"");
            this.DoString("g_type_item = \"OutpostOmega.Game.GameObjects.Item\"");
            this.DoString("g_type_mob = \"OutpostOmega.Game.GameObjects.Mob\"");

            this.DoString("g_type_attr_definition = \"OutpostOmega.Game.GameObjects.Attributes.Definition\"");
            this.DoString("g_type_attr_construction = \"OutpostOmega.Game.GameObjects.Attributes.Construction\"");
        }

        [LuaDocumentationAttr(
            "Addon Commands",
            "Creates a new GameObject-Builder used to create custom GameObjects",
            "New Builder object",
            "1 ClassName: A unique name for the new GameObject\n" +
            "2 Parent: The parent class, the new class should inherit from (see g_type_* variables)")]
        protected Builder NewBuilder(string ClassName, string Parent)
        {
            return new Builder(ClassName, Parent);
        }
        #endregion


        #region Content
        public void ContentFunctions()
        {
            this.RegisterFunction("LoadUI", this, this.GetType().GetMethod("LoadUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));

        }

        [LuaDocumentationAttr(
            "Content Commands",
            "Loads a UI-Window from the given location",
            "New UI object",
            "1 ContentFile: The path to the UI File (use GetFirstContent())")]
        protected datums.UserInterface.Base LoadUI(string ContentFile)
        {
            var fileInfo = new System.IO.FileInfo(ContentFile);
            if (fileInfo.Exists)
                return Content.UserInterface.LoadUI(fileInfo);
            else
                throw new System.IO.FileNotFoundException("UI-File '" + ContentFile + "' not found");
        }
        #endregion


        #region World
        private void WorldFunctions()
        {
            this.RegisterFunction("GetFirstGO", this, this.GetType().GetMethod("GetFirstGO", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("GetGO", this, this.GetType().GetMethod("GetGO", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("MoveGO", this, this.GetType().GetMethod("MoveGO", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        }

        [LuaDocumentationAttr(
            "World Commands",
            "Gets the first GameObject matching the given name", 
            "First found GameObject",
            "1. arg: Searched name (has to be contained in the searched objects name)")]
        protected object GetFirstGO(string arg)
        {
            foreach (GameObject gameObject in world.AllGameObjects)
            {
                if (gameObject.ID.Contains(arg))
                    return gameObject;
            }
            return null;
        }

        [LuaDocumentationAttr(
            "World Commands",
            "Gets all GameObjects matching the given name",
            "Array if all found GameObject",
            "1. arg: Searched name (has to be contained in the searched objects name)")]
        protected object GetGO(string arg)
        {
            var table = (LuaTable)this.DoString("return {}")[0];
            foreach (GameObject gameObject in world.AllGameObjects)
            {
                if (gameObject.ID.Contains(arg))
                    table[gameObject.ID] = gameObject;
            }
            return table;
        }

        [LuaDocumentationAttr(
            "World Commands",
            "Moves a specific GameObject to the given Position",
            "",
            "1. GameObject: The GameObject that should be moved\n2. Position: The new position")]
        protected void MoveGO(GameObject GameObject, Jitter.LinearMath.JVector Position)
        {
            ((GameObject)GameObject).SetPosition((Jitter.LinearMath.JVector)Position);
        }
        #endregion


        #region Math
        private void MathFunctions()
        {
            this.RegisterFunction("Vector2", this, this.GetType().GetMethod("Vector2", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("Vector3", this, this.GetType().GetMethod("Vector3", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("VectorTransform", this, this.GetType().GetMethod("VectorTransform", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("MatrixIdentity", this, this.GetType().GetMethod("MatrixIdentity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("MatrixRotationX", this, this.GetType().GetMethod("MatrixRotationX", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("MatrixRotationY", this, this.GetType().GetMethod("MatrixRotationY", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("MatrixRotationZ", this, this.GetType().GetMethod("MatrixRotationZ", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("MatrixTranslation", this, this.GetType().GetMethod("MatrixTranslation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("MatrixMultiply", this, this.GetType().GetMethod("MatrixMultiply", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        }

        [LuaDocumentationAttr(
            "Math Commands",
            "Creates a 2-dimensional vector from the given values",
            "Created Vector2",
            "1 X: float value of x-dimension\n" +
            "2 Y: float value of y-dimension")]
        protected object Vector2(string X, string Y)
        {
            return new Jitter.LinearMath.JVector2(float.Parse(X), float.Parse(Y));
        }

        [LuaDocumentationAttr(
            "Math Commands",
            "Creates a 3-dimensional vector from the given values",
            "Created Vector2",
            "1 X: float value of x-dimension\n" +
            "2 Y: float value of y-dimension\n" +
            "3 Z: float value of z-dimension")]
        protected object Vector3(string X, string Y, string Z)
        {
            return new Jitter.LinearMath.JVector(float.Parse(X), float.Parse(Y), float.Parse(Z));
        }

        [LuaDocumentationAttr(
            "Math Commands",
            "Transforms a 3-dimensional vector using a matrix",
            "Transformed Vector3",
            "1 vector3: The 3-dimensional vector that should be transformed\n" +
            "2 matrix: Matrix that should be used to transform the vector")]
        protected object VectorTransform(object vector3, object matrix)
        {
            return Jitter.LinearMath.JVector.Transform((Jitter.LinearMath.JVector)vector3, (Jitter.LinearMath.JMatrix)matrix);
        }

        [LuaDocumentationAttr(
            "Math Commands",
            "Creates a empty (indentity) matrix",
            "empty (indentity) matrix",
            "")]
        protected object MatrixIdentity()
        {
            return Jitter.LinearMath.JMatrix.Identity;
        }


        [LuaDocumentationAttr(
            "Math Commands",
            "Creates a rotation matrix on the x-axis",
            "Rotation matrix",
            "1 deg: X-rotation in degrees (0-360)")]
        protected object MatrixRotationX(string deg)
        {
            return Jitter.LinearMath.JMatrix.CreateRotationX((float)Tools.MathHelper.DegreeToRadian(float.Parse(deg)));
        }

        [LuaDocumentationAttr(
            "Math Commands",
            "Creates a rotation matrix on the y-axis",
            "Rotation matrix",
            "1 deg: Y-rotation in degrees (0-360)")]
        protected object MatrixRotationY(string deg)
        {
            return Jitter.LinearMath.JMatrix.CreateRotationY((float)Tools.MathHelper.DegreeToRadian(float.Parse(deg)));
        }

        [LuaDocumentationAttr(
            "Math Commands",
            "Creates a rotation matrix on the z-axis",
            "Rotation matrix",
            "1 deg: Z-rotation in degrees (0-360)")]
        protected object MatrixRotationZ(string deg)
        {
            return Jitter.LinearMath.JMatrix.CreateRotationZ((float)Tools.MathHelper.DegreeToRadian(float.Parse(deg)));
        }

        [LuaDocumentationAttr(
            "Math Commands",
            "Creates a translation (position/movement) matrix",
            "Translation matrix",
            "1 X: X-value of the translation\n" +
            "2 Y: Y-value of the translation\n" +
            "3 Z: Z-value of the translation")]
        protected object MatrixTranslation(string X, string Y, string Z)
        {
            return Jitter.LinearMath.JMatrix.CreateTranslation(new Jitter.LinearMath.JVector(float.Parse(X), float.Parse(Y), float.Parse(Z)));
        }

        [LuaDocumentationAttr(
            "Math Commands",
            "Multiplys 2 matrices",
            "Result of multiplication (matrix)",
            "1 Matrix1: First matrix\n" +
            "2 Matrix2: Second Matrix")]
        protected object MatrixMultiply(object Matrix1, object Matrix2)
        {
            return (Jitter.LinearMath.JMatrix)Matrix1 * (Jitter.LinearMath.JMatrix)Matrix1;
        }        
        #endregion


        #region Physic
        private void PhysicFunctions()
        {
            this.RegisterFunction("PhysicBoxShape", this, this.GetType().GetMethod("PhysicBoxShape", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("PhysicCapsuleShape", this, this.GetType().GetMethod("PhysicCapsuleShape", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("PhysicConeShape", this, this.GetType().GetMethod("PhysicConeShape", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("PhysicCylinderShape", this, this.GetType().GetMethod("PhysicCylinderShape", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
            this.RegisterFunction("PhysicSphereShape", this, this.GetType().GetMethod("PhysicSphereShape", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
        }

        [LuaDocumentationAttr(
            "Physic Commands",
            "Creates a box shape",
            "New box shape object",
            "1 X: X-dimension of the box\n" +
            "1 Y: Y-dimension of the box\n" +
            "1 Z: Z-dimension of the box")]
        protected object PhysicBoxShape(float x, float y, float z)
        {
            return new Jitter.Collision.Shapes.BoxShape(x, y, z);
        }

        [LuaDocumentationAttr(
            "Physic Commands",
            "Creates a capsule shape",
            "New capsule shape object",
            "1 length: Length of the capsule\n" +
            "2 radius: Radius of the capsule")]
        protected object PhysicCapsuleShape(float length, float radius)
        {
            return new Jitter.Collision.Shapes.CapsuleShape(length, radius);
        }

        [LuaDocumentationAttr(
            "Physic Commands",
            "Creates a cone shape",
            "New cone shape object",
            "1 height: Height of the cone\n" +
            "2 radius: Radius of the cone base")]
        protected object PhysicConeShape(float height, float radius)
        {
            return new Jitter.Collision.Shapes.ConeShape(height, radius);
        }

        [LuaDocumentationAttr(
            "Physic Commands",
            "Creates a cylinder shape",
            "New cylinder shape object",
            "1 height: Height of the cylinder\n" +
            "2 radius: Radius of the cylinder")]
        protected object PhysicCylinderShape(float height, float radius)
        {
            return new Jitter.Collision.Shapes.CylinderShape(height, radius);
        }

        [LuaDocumentationAttr(
            "Physic Commands",
            "Creates a sphere shape",
            "New sphere shape object",
            "1 radius: Radius of the shpere")]
        protected object PhysicSphereShape(float radius)
        {
            return new Jitter.Collision.Shapes.SphereShape(radius);
        }

        #endregion
    }
}
/*[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
public static void Example()
{
    AppDomain currentDomain = AppDomain.CurrentDomain;
    currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);

    try
    {
        throw new Exception("1");
    }
    catch (Exception e)
    {
        Console.WriteLine("Catch clause caught : " + e.Message);
    }

    throw new Exception("2");

    // Output: 
    //   Catch clause caught : 1 
    //   MyHandler caught : 2
}

static void MyHandler(object sender, UnhandledExceptionEventArgs args)
{
    Exception e = (Exception)args.ExceptionObject;
    Console.WriteLine("MyHandler caught : " + e.Message);
}

public static void Main()
{
    Example();
}*/