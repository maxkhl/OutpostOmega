using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using Jitter.LinearMath;
using System.Reflection;
using OutpostOmega.Game.GameObjects.Attributes;

namespace OutpostOmega.Drawing.UI
{
    class SpawnMenu : Menu
    {
        VerticalSplitter vsplitter;
        public SpawnMenu(Scenes.Game GameScene, Base parent)
            : base(GameScene, parent, "Spawner")
        {
            this.SetSize(500, 600);

            var clearButton = new Button(this);
            clearButton.Dock = Pos.Bottom;
            clearButton.Text = "Clear Selection";
            clearButton.Pressed += clearButton_Pressed;

            vsplitter = new VerticalSplitter(this);
            vsplitter.Dock = Pos.Fill;
            vsplitter.SetHValue(0.5f);

            /*TreeControl treeV = new TreeControl(this);
            treeV.Dock = Pos.Fill;
            splitter.SetPanel(0, treeV);*/

            PropertyTree ptree = new PropertyTree(this);
            ptree.Dock = Pos.Fill;
            ptree.SelectionChanged += ptree_SelectionChanged;
            vsplitter.SetPanel(0, ptree);

            var gameObjectType = typeof(OutpostOmega.Game.GameObject);
            var members = gameObjectType.GetMembers();

            List<Assembly> assemblies = new List<Assembly>();
            var assemblyName = Assembly.GetExecutingAssembly().GetReferencedAssemblies().SingleOrDefault(t => t.Name == "OutpostOmega.Game");
            assemblies.Add(Assembly.Load(assemblyName));

            if (OutpostOmega.Game.GameObject.AddonAssembly != null)
                assemblies.Add(OutpostOmega.Game.GameObject.AddonAssembly);

            var MainNode = ptree.AddNode("GameObject");
            BuildGameObjectTree(assemblies, MainNode, typeof(OutpostOmega.Game.GameObject));

            var TurfNode = ptree.AddNode("Turf");
            BuildTurfTree(TurfNode);




            hsplitter = new HorizontalSplitter(this);
            vsplitter.SetPanel(1, hsplitter);

            ScreenPanel = new ImagePanel(this);
            ScreenPanel.SetBounds(0, 0, 200, 200);
            hsplitter.SetPanel(0, ScreenPanel);
            ScreenPanel.BoundsChanged += ScreenPanel_BoundsChanged;

        }

        void clearButton_Pressed(Base sender, EventArgs arguments)
        {
            ((Scenes.Game)Scene).World.Player.SelectedBuildObject = null;
        }

        private void BuildTurfTree(TreeNode ParentNode)
        {
            var tnames = Enum.GetNames(typeof(OutpostOmega.Game.turf.types.turfTypeE));
            foreach (string turfname in tnames)
                ParentNode.AddNode(turfname).UserData = (OutpostOmega.Game.turf.types.turfTypeE)Enum.Parse(typeof(OutpostOmega.Game.turf.types.turfTypeE), turfname);
        }

        HorizontalSplitter hsplitter;
        ImagePanel ScreenPanel;
        Screens.GameObjectViewer Screen;
        GroupBox DataBox;
        void ptree_SelectionChanged(Base sender, EventArgs arguments)
        {
            var selectedNode = (TreeNode)sender;
            if (Screen == null)
            {
                var gameScene = (Scenes.Game)this.Scene;
                Screen = new Screens.GameObjectViewer(gameScene);
                Screen.Width = 200;
                Screen.Height = 150;
                gameScene.BackgroundScreens.Add(Screen);
                ScreenPanel.ImageHandle = (int)Screen.RenderTarget.OutTexture;

                DataBox = new GroupBox(this);
                hsplitter.SetPanel(1, DataBox);
            }

            if(selectedNode.UserData != null)
            {
                Type gObjType = selectedNode.UserData.GetType();

                if (typeof(Type).IsAssignableFrom(gObjType) && typeof(OutpostOmega.Game.GameObject).IsAssignableFrom((Type)selectedNode.UserData))
                {
                    var TypeObject = ((Type)selectedNode.UserData);
                    var attributes = TypeObject.GetCustomAttributes(false);

                    Definition DefinitionAttr = null;
                    Construction ConstructionAttr = null;

                    //Cast dat shit
                    if (attributes != null && attributes.Length > 0)
                    {
                        foreach (object obj in attributes)
                        {
                            if (obj.GetType() == typeof(Definition))
                                DefinitionAttr = (Definition)obj;

                            if (obj.GetType() == typeof(Construction))
                                ConstructionAttr = (Construction)obj;
                        }
                    }

                    //if (DefinitionAttr != null && ConstructionAttr != null)
                    //{
                    var newGameObject = OutpostOmega.Game.GameObject.GenerateNew(TypeObject, ((Scenes.Game)Scene).World);
                    if (newGameObject == null)
                    {
                        new MessageBox(this.Parent, "Can't spawn this. Sorry.", "Error").Show();
                        return;
                    }
                    // We do not register it so it'll remain hidden
                    Screen.GameObject = newGameObject;
                    newGameObject.Visible = false;
                    // Disable the physics to make sure no invisible structure is spawned
                    newGameObject.PhysicDisable();

                    DataBox.DeleteAllChildren();

                    DataBox.Text = gObjType.Name;
                    DataBox.Redraw();

                    var name = new Label(DataBox);
                    name.SetPosition(5, 25);
                    if (DefinitionAttr != null)
                    {
                        name.Text = DefinitionAttr.Name;
                        var description = new Label(DataBox);
                        description.SetPosition(5, 45);
                        description.Text = DefinitionAttr.Description;
                    }
                    else
                        name.Text = newGameObject.ID;

                    var Namespace = new Label(DataBox);
                    Namespace.SetPosition(5, 65);
                    if(TypeObject.Namespace != null)
                        Namespace.Text = "Official Object";
                    else
                        Namespace.Text = "Addon Object";

                    ((Scenes.Game)Scene).World.Player.SelectedBuildObject = newGameObject;
                    //}
                }
                else if (gObjType == typeof(OutpostOmega.Game.turf.types.turfTypeE))
                {
                    ((Scenes.Game)Scene).World.Player.SelectedBuildObject = (OutpostOmega.Game.turf.types.turfTypeE)selectedNode.UserData;
                }
            }
        }

        void ScreenPanel_BoundsChanged(Base sender, EventArgs arguments)
        {
            if (Screen != null)
            {
                //Screen.Width = ScreenPanel.InnerBounds.Width;
                Screen.RenderTarget.Width = ScreenPanel.InnerBounds.Width;

                //Screen.Height = ScreenPanel.InnerBounds.Height;
                Screen.RenderTarget.Height = ScreenPanel.InnerBounds.Height;
            }
        }

        private void BuildGameObjectTree(List<Assembly> GameAssemblies, TreeNode ParentNode, Type ParentType)
        {
            List<Type> types = new List<Type>();
            foreach(Assembly GameAssembly in GameAssemblies)
                types.AddRange(GameAssembly.GetTypes().Where(t => t.BaseType == ParentType));

            foreach (Type subType in types)
            {
                var attributes = subType.GetCustomAttributes(false);

                Definition DefinitionAttr = null;                
                Construction ConstructionAttr = null;

                //Cast dat shit
                if (attributes != null && attributes.Length > 0)
                {
                    foreach(object obj in attributes)
                    {
                        if(obj.GetType() == typeof(Definition))
                            DefinitionAttr = (Definition)obj;

                        if(obj.GetType() == typeof(Construction))
                            ConstructionAttr = (Construction)obj;
                    }
                }


                if(subType.IsAbstract) //Category
                {
                    var newNode = ParentNode.AddNode(subType.Name);
                    BuildGameObjectTree(GameAssemblies, newNode, subType);
                }
                else 
                {
                    var newNode = ParentNode.AddNode(subType.Name);
                    newNode.UserData = subType;
                    BuildGameObjectTree(GameAssemblies, newNode, subType);
                }
                //else //Nothing
                //    BuildGameObjectTree(GameAssemblies, ParentNode, subType);
            }
        }
    }
}
