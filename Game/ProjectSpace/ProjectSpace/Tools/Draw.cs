using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OutpostOmega.Tools
{
    static class Draw
    {
        public static void Line(Vector3 pos1, Vector3 pos2, Color4 color)
        {
            GL.Color3(color.R, color.G, color.B);
            GL.LineWidth(2);

            GL.Begin(PrimitiveType.Lines);

            GL.Vertex3(pos1);
            GL.Vertex3(pos2);

            GL.End();

            Tools.OpenGL.CheckError();
        }

        public static void Cursor(Game.World World)
        {
            var lookat = World.Player.Mob.View.TargetHit;
            if(World.Player.Mob.View.TargetGameObject != null)
            {
                var targetRigidBody = World.Player.Mob.View.TargetGameObject.RigidBody;

                //This happens appearently
                if (targetRigidBody == null)
                    return;
                
                var max = new Vector3(targetRigidBody.BoundingBox.Max.X, targetRigidBody.BoundingBox.Max.Y, targetRigidBody.BoundingBox.Max.Z);
                var min = new Vector3(targetRigidBody.BoundingBox.Min.X, targetRigidBody.BoundingBox.Min.Y, targetRigidBody.BoundingBox.Min.Z);
                var diff = max - min;


                Color4 color = Color4.Lime;

                Line(min, min + new Vector3(diff.X, 0, 0), color);
                Line(min, min + new Vector3(0, diff.Y, 0), color);
                Line(min, min + new Vector3(0, 0, diff.Z), color);


                Line(max, max - new Vector3(diff.X, 0, 0), color);
                Line(max, max - new Vector3(0, diff.Y, 0), color);
                Line(max, max - new Vector3(0, 0, diff.Z), color);
            }
            //Disabled for now. Objectselection is now a property of the tool and not the player - wich is more correct
            //but this also makes drawing a preview a lot harder. Need to figure out a way to do this inside the tool (SpawnTools class)
            /*else if (World.Player.Mob.View.TargetStructure != null && World.Player.SelectedBuildObject != null)
            {
                var chunk = World.Player.Mob.View.TargetStructure.GetChunkAtPos(lookat);
                if (chunk == null)
                    return;

                var buildRotation = World.Player.SpawnDirection;

                var buildObject = World.Player.SelectedBuildObject;

                int locX = (int)(lookat.X - chunk.Position.X),
                    locY = (int)(lookat.Y - chunk.Position.Y),
                    locZ = (int)(lookat.Z - chunk.Position.Z);
                var block = chunk[locX, locY, locZ];
                var blockWorldPos = World.Player.Mob.View.TargetStructure.Origin + chunk.Position + new Jitter.LinearMath.JVector(locX, locY, locZ);

                var glblockPos = new Vector3(blockWorldPos.X, blockWorldPos.Y, blockWorldPos.Z);

                Color4 color = Color4.Lime;

                bool[, ,] cubes = null;

                if (buildObject == null)
                    return;
                if (buildObject.GetType() == typeof(OutpostOmega.Game.turf.types.turfTypeE))
                    cubes = new bool[1, 1, 1] { { { true } } };
                else if (typeof(OutpostOmega.Game.GameObjects.Structures.Structure).IsAssignableFrom(buildObject.GetType()))
                    cubes = ((OutpostOmega.Game.GameObjects.Structures.Structure)buildObject).SpaceRequirement;

                if (cubes == null)
                    return;


                for (int X = 0; X < cubes.GetLength(0); X++)
                    for (int Y = 0; Y < cubes.GetLength(1); Y++)
                        for (int Z = 0; Z < cubes.GetLength(2); Z++)
                        {
                            var addPos = new Vector3(-X, Y, Z);


                            var targetPos = glblockPos + Vector3.Transform(addPos, Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90 * buildRotation)));

                            Line(targetPos, targetPos + Vector3.UnitX, color);
                            Line(targetPos, targetPos + Vector3.UnitY, color);
                            Line(targetPos, targetPos + Vector3.UnitZ, color);


                            Line(targetPos + Vector3.One, targetPos + Vector3.One - Vector3.UnitX, color);
                            Line(targetPos + Vector3.One, targetPos + Vector3.One - Vector3.UnitY, color);
                            Line(targetPos + Vector3.One, targetPos + Vector3.One - Vector3.UnitZ, color);
                        }

            }*/
            Tools.OpenGL.CheckError();
        }

        public static void Atmos(Game.World World)
        {
            // Iterate through each structure in our world
            for (int s = 0; s < World.Structures.Count; s++)
            {
                var structure = World.Structures[s];

                // Iterate through each chunk
                for (int c = 0; c < structure.chunks.Count; c++)
                {
                    var chunk = structure.chunks[c];

                    // Iterate through each block
                    for (int x = 0; x < Game.Turf.Chunk.SizeXYZ; x++)
                        for (int y = 0; y < Game.Turf.Chunk.SizeXYZ; y++)
                            for (int z = 0; z < Game.Turf.Chunk.SizeXYZ; z++)
                            {
                                var block = chunk.blocks[x, y, z];

                                var pressure = block.Pressure;
                                if (pressure > 0)
                                {
                                    var worldPos = new Vector3(chunk.Position.X + x, chunk.Position.Y + y, chunk.Position.Z + z);
                                    var colorStrength = pressure / 2000 * 255;
                                    Line(worldPos, worldPos + Vector3.One, new Color4(colorStrength, colorStrength, colorStrength, 255));
                                }
                            }
                }
            }
        }
    }
}
