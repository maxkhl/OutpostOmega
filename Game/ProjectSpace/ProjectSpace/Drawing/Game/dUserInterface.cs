using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gwen;
using Gwen.Control;
using OutpostOmega.Game.Content;
using UIdef = OutpostOmega.Game.datums.UserInterface;
using System.Drawing;
using System.Reflection;
using System.Globalization;

namespace OutpostOmega.Drawing.Game
{
    /// <summary>
    /// Used to visualize the Content.UserInterface class inside the worlds content using GWEN
    /// </summary>
    class dUserInterface : Screen, IDisposable
    {
        /// <summary>
        /// Gwen Renderer
        /// </summary>
        public Gwen.Renderer.OpenTK renderer { get; protected set; }

        /// <summary>
        /// Default Skin
        /// </summary>
        public Gwen.Skin.Base Skin { get; protected set; }

        /// <summary>
        /// Main GWEN Container for this scene
        /// </summary>
        public Gwen.Control.Canvas Canvas { get; protected set; }

        /// <summary>
        /// Ref to the content userinterface that this mesh is visualizing
        /// </summary>
        public UserInterface UserInterface { get; protected set; }

        private uiElement _uiElement;

        /// <summary>
        /// Opengl texture handle of the target-texture
        /// </summary>
        public int Handle
        {
            get
            {
                if (RenderTarget != null)
                    return (int)RenderTarget.OutTexture;
                else
                    throw new Exception("Interface not loaded");
            }
        }

        /// <summary>
        /// Input handler
        /// </summary>
        private Gwen.Input.OpenTK _input;

        /// <summary>
        /// Determins if this UI is fully loaded
        /// </summary>
        public bool Loaded { get; protected set; }

        public dUserInterface(Scenes.Game Parent, UserInterface UserInterface)
            : base(Parent, UserInterface.ResolutionX, UserInterface.ResolutionY)
        {
            this.UserInterface = UserInterface;
            this.UserInterface.Data = this;

            this.ForceResolution = true;

            RenderTarget.CullFace = false;

            GameScene.Screens.Add(this);
        }

        /// <summary>
        /// Causes this interface to rescale to the new resolution of the underlying content interface
        /// </summary>
        public void Rescale()
        {
            if (Disposing || !Loaded) return;

            RenderTarget.Width = UserInterface.ResolutionX;
            RenderTarget.Height = UserInterface.ResolutionY;
            Canvas.SetSize(RenderTarget.Width, RenderTarget.Height);
            renderer.FlushTextCache();
        }

        /// <summary>
        /// Loads and prepares this interface (make sure you call it from the main thread)
        /// </summary>
        public void Load()
        {
            if (!Loaded)
            {
                Loaded = true;
                //RenderTarget = new RenderTarget(UserInterface.ResolutionX, UserInterface.ResolutionY);

                this.renderer = new Gwen.Renderer.OpenTK();

                var lSkin = Skin;
                Scene.CreateDefaultSkin(ref lSkin, renderer);
                Skin = lSkin;
                Skin.DefaultFont.Size = 18;

                this.Canvas = new Canvas(Skin);
                Canvas.BackgroundColor = Color.Yellow;

                _input = new Gwen.Input.OpenTK(this.GameScene.Game);
                _input.Initialize(Canvas);

                _uiElement = new uiElement(UserInterface.UIBase, Canvas);

                Rescale();
            }
        }

        /// <summary>
        /// Update call
        /// </summary>
        public override void Update(double ElapsedTime)
        {
            if (Disposing) return;

            if (!Loaded) Load();
        }

        protected override void DrawScene()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Draw call
        /// </summary>
        public override void DrawScreen()
        {
            if (Disposing || !Loaded) return;

            RenderTarget.Start();

            /*if (Shader != null)
            {
                Shader.Bind();
                for (int i = 0; i < Shader.PassCount; i++)
                {
                    SetUniform(i);
                    GameScene.Canvas.RenderCanvas();
                }
                Shader.UnBind();
            }
            else*/
            Canvas.RenderCanvas();

            RenderTarget.End();
        }

        /// <summary>
        /// Draw-call
        /// </summary>
        public override void Draw()
        {
            DrawScreen();
        }

        public override int Render()
        {
            return 0;
        }

        public bool Disposing { get; protected set; }
        public void Dispose()
        {
            Disposing = true;

            // Unregister from current scene
            GameScene.Screens.Remove(this);

            //Dispose this GWEN instance
            if (renderer != null)
                renderer.Dispose();
            if (Skin != null)
                Skin.Dispose();
            if (Canvas != null)
                Canvas.Dispose();
            if (RenderTarget != null)
                RenderTarget.Dispose();
        }


        /// <summary>
        /// Used to synchronize a single UI element with the world data
        /// </summary>
        private class uiElement
        {
            public UIdef.Base Base { get; protected set; }

            private List<uiElement> Children;

            public Base dBase { get; protected set; }

            public Base Parent { get; protected set; }

            public uiElement(UIdef.Base Base, Base Parent)
            {
                this.Base = Base;
                this.Base.AttributeChanged += Base_AttributeChanged;

                this.Parent = Parent;
                var BaseType = OutpostOmega.Data.cConverter.GetType("Gwen.Control." + Base.Type.ToString());

                if (BaseType == null)
                    throw new Exception("Could not process userinterface '" + Base.Type.ToString() + "'");

                dBase = (Base)Activator.CreateInstance(BaseType,
                    BindingFlags.CreateInstance |
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.OptionalParamBinding,
                    null,
                    new object[] { Parent },
                    CultureInfo.CurrentCulture);

                // Synchronize attributes
                foreach (var prop in dBase.GetType().GetProperties())
                {
                    UIdef.AttributeType Attribute;
                    if (Enum.TryParse<UIdef.AttributeType>(prop.Name, out Attribute))
                        if (Base.Attributes.Exists(m => m.Type == Attribute))
                        {
                            object value = Base.Attributes.Find(m => m.Type == Attribute).Value;
                            ChangeValue(prop, Attribute, value, dBase);
                        }
                }

                Children = new List<uiElement>();
                foreach (var child in Base.Children)
                    Children.Add(new uiElement(child, this.dBase));
            }

            private void ChangeValue(PropertyInfo propInfo, UIdef.AttributeType Attribute, object Value, Base Instance)
            {
                switch (Attribute)
                {
                    case UIdef.AttributeType.Dock:
                        Value = Enum.Parse(typeof(Pos), (string)Value, true);
                        break;
                    case UIdef.AttributeType.X:
                    case UIdef.AttributeType.Y:
                    case UIdef.AttributeType.Width:
                    case UIdef.AttributeType.Height:
                    case UIdef.AttributeType.Handle:
                        Value = int.Parse((string)Value);
                        break;
                    case UIdef.AttributeType.Disabled:
                    case UIdef.AttributeType.Hidden:
                    case UIdef.AttributeType.Tabable:
                        Value = bool.Parse((string)Value);
                        break;
                }
                propInfo.SetValue(Instance, Value);
            }

            void Base_AttributeChanged(UIdef.AttributeType Type, object Value)
            {
                foreach (var prop in dBase.GetType().GetProperties())
                {
                    UIdef.AttributeType Attribute;
                    if (prop.Name.ToLower() == Type.ToString().ToLower() && Enum.TryParse<UIdef.AttributeType>(prop.Name, out Attribute))
                        if (Base.Attributes.Exists(m => m.Type == Attribute))
                        {
                            ChangeValue(prop, Type, Value, dBase);
                        }
                }
            }
        }
    }
}
