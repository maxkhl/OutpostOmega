using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.LinearMath;
using Jitter.Collision;
using Jitter;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;

namespace OutpostOmega.Server.Drawer
{
    class Main : DrawStuffOtk, IDisposable
    {
        private World pSystem;
        private OutpostOmega.Game.World world;
        private bool initFrame = true;
        private DebugDrawer debugDrawer;

        private const string title = "Ingame Debug Drawer";

        public static void Start(object world)
        {
            if (!(world is OutpostOmega.Game.World))
                return;

            using (Main p = new Main((OutpostOmega.Game.World)world))
            {
                p.Run();
            }
        }

        public Main(OutpostOmega.Game.World world)
            : base(800, 600)
        {
            this.world = world;
            this.pSystem = world.PhysicSystem;
            this.debugDrawer = new DebugDrawer(this);

            dsSetSphereQuality(2);

            this.VSync = OpenTK.VSyncMode.Off;
            this.Title = title;

            Keyboard.KeyDown += new EventHandler<OpenTK.Input.KeyboardKeyEventArgs>(Keyboard_KeyDown);
        }

        void Keyboard_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {

        }

        protected override void OnBeginRender(double elapsedTime)
        {
            if (initFrame)
            {
                dsSetViewpoint(new float[] { 18, 10, 8 }, new float[] { 190, -10, 0 });
                initFrame = false;
            }

            RenderAll();

            base.OnBeginRender(elapsedTime);
        }

        private void RenderAll()
        {
            //dsSetTexture(DS_TEXTURE_NUMBER.DS_WOOD);
            
            for(int i = 0; i < pSystem.RigidBodies.Count; i++)
            //foreach (RigidBody body in pSystem.RigidBodies)
            {
                var body = pSystem.RigidBodies.ToArray()[i];

                if (body.Tag is bool) continue;

                if(body.EnableDebugDraw == false)
                    body.EnableDebugDraw = true;

                this.dsSetColor(255, 0, 0);

                body.DebugDraw(this.debugDrawer);
                /*if (body.Shape is BoxShape)
                {
                    BoxShape shape = body.Shape as BoxShape;

                    if (body.IsActive) dsSetColor(1, 1, 1);
                    else dsSetColor(0.5f, 0.5f, 1);

                    dsDrawBox(body.Position, body.Orientation, shape.Size);
                }
                else if (body.Shape is SphereShape)
                {
                    SphereShape shape = body.Shape as SphereShape;

                    if (body.IsActive) dsSetColor(1,1,0);
                    else dsSetColor(0.5f, 0.5f, 1);

                    dsDrawSphere(body.Position, body.Orientation, shape.Radius-0.1f);
                }*/
            }

            DebugDrawer debugDrawer = new DebugDrawer(this);
            for (int i = 0; i < world.AllGameObjects.Count; i++)
            {
                dsSetColor(0, 0, 1);

                dsDrawBox(world.AllGameObjects[i].Position, world.AllGameObjects[i].Orientation, new JVector(.5f));
                debugDrawer.DrawLine(world.AllGameObjects[i].Position, world.AllGameObjects[i].Position + world.AllGameObjects[i].Forward, 0, 0, 0);
            }
        }

        float accTime = 0.0f;

        protected override void OnUpdateFrame(OpenTK.FrameEventArgs e)
        {
            accTime += 1.0f / (float)RenderFrequency;

            if (accTime > 1.0f)
            {
                this.Title = title + " " + RenderFrequency.ToString("##.#") + " fps";
                accTime = 0.0f;
            }

            float step = 1.0f / (float)RenderFrequency;
            if (step > 1.0f / 100.0f) step = 1.0f / 100.0f;
            //pSystem.Step(step, true);

            base.OnUpdateFrame(e);
        }
        public class DebugDrawer : IDebugDrawer
        {
            private DrawStuffOtk dso;
            public DebugDrawer(DrawStuffOtk dso)
            {
                this.dso = dso;
            }

            public void DrawLine(JVector start, JVector end, byte R, byte G, byte B)
            {
                dso.dsDrawLine(start, end);
            }
            public void DrawTriangle(JVector pos1, JVector pos2, JVector pos3)
            {
                dso.dsDrawLine(pos3, pos1);
                //dso.dsDrawLine(pos2, pos1);
                //dso.dsDrawLine(pos2, pos3);
                //dso.dsDrawTriangle(JVector.Zero, JMatrix.Identity, pos1, pos2, pos3, true);
            }
            public void DrawPoint(JVector pos)
            {
            }
        }
    }

}
