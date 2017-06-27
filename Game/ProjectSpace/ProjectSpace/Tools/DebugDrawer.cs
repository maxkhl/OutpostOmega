using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter;
using Jitter.LinearMath;
using OpenTK;
using OpenTK.Graphics;

namespace OutpostOmega.Tools
{
    public class DebugDrawer : Jitter.IDebugDrawer
    {
        public void DrawLine(JVector start, JVector end, byte R, byte G, byte B)
        {
            var color = new Color4(R, G, B, 255);
            Draw.Line(
                Convert.Vector.Jitter_To_OpenGL(start),
                Convert.Vector.Jitter_To_OpenGL(end),
                color);
        }
        public void DrawTriangle(JVector pos1, JVector pos2, JVector pos3)
        {

            Draw.Line(
                Convert.Vector.Jitter_To_OpenGL(pos1),
                Convert.Vector.Jitter_To_OpenGL(pos3),
                Color4.Red);
            Draw.Line(
                Convert.Vector.Jitter_To_OpenGL(pos1),
                Convert.Vector.Jitter_To_OpenGL(pos2),
                Color4.Red);
            Draw.Line(
                Convert.Vector.Jitter_To_OpenGL(pos2),
                Convert.Vector.Jitter_To_OpenGL(pos3),
                Color4.Red);
            //dso.dsDrawLine(pos2, pos1);
            //dso.dsDrawLine(pos2, pos3);
            //dso.dsDrawTriangle(JVector.Zero, JMatrix.Identity, pos1, pos2, pos3, true);
        }
        public void DrawPoint(JVector pos)
        {
        }
    }
}
