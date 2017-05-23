using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;
using OpenTK;

namespace OutpostOmega.Game.GameObjects.Items.Devices
{
    [Attributes.IconAttribute(@"Content\Model\Items\Devices\Write.png")]
    [Attributes.Definition("CableSpawner", "Used to spawn cables")]
    public class CableSpawner : SpawnTool
    {
        public CableSpawner(World world, string ID = "cableSpawner")
            : base(world, ID)
        {
            var model = LoadModel(@"Content\Model\Items\Devices\Gun.dae");
            model.AssignTexture("Gun", this, LoadTexture(@"Content\Model\Items\Devices\Gun.png"));
            model.AssignTexture("Display", this, LoadTexture(@"Content\Model\Items\Devices\Write.png"));

            this.Shape = MeshToShape(model, model.Meshs["Colission"]);

            this.Mass = 20;
            this.Static = false;
            this.PhysicCreateMaterial();
            this.PhysicEnable();
            //this.PhysicSetPosition(new Jitter.LinearMath.JVector(5, 20, 5));
            this.PhysicEnableDebug();
        }

        public byte CableX = 0;
        public byte CableY = 0;

        public override void Update(double ElapsedTime)
        {
            if (Holder != null)
            {
                if (Holder.View.TargetStructure != null)// && OutpostOmega.Game.turf.Block.IsVisible(Holder.View.TargetStructure[Holder.View.TargetHitInside.X, Holder.View.TargetHitInside.Y, Holder.View.TargetHitInside.Z]))
                {

                    var tpos = Holder.View.TargetStructure.GetBlockPosition(Holder.View.TargetHitInside);
                    var cpos = Holder.View.TargetHit - tpos;
                      //cpos += JVector.Multiply(cpos, Holder.View.TargetHitNormal);

                    cpos /= new JVector(0.333333333f);
                    cpos = new JVector((int)cpos.X, (int)cpos.Y, (int)cpos.Z);
                    if (Holder.View.TargetHitNormal.X != 0)
                    {
                        CableX = (byte)cpos.Y;
                        CableY = (byte)cpos.Z;
                    }
                    else if (Holder.View.TargetHitNormal.Y != 0)
                    {
                        CableX = (byte)cpos.X;
                        CableY = (byte)cpos.Z;
                    }
                    else if (Holder.View.TargetHitNormal.Z != 0)
                    {
                        CableX = (byte)cpos.X;
                        CableY = (byte)cpos.Y;
                    }
                    cpos = JVector.Multiply(cpos, new JVector(0.333333333f));

                    var invNorm = Holder.View.TargetHitNormal - JVector.One;
                    invNorm.Negate();

                    var hStart = tpos + (cpos + new JVector(0.333333333f) - JVector.Multiply(new JVector(0.333333333f), Holder.View.TargetHitNormal)) + JVector.Multiply(new JVector(0.01f), Holder.View.TargetHitNormal);
                    if (Holder.View.TargetHitNormal >= JVector.Zero)
                        hStart += Holder.View.TargetHitNormal - JVector.Multiply(new JVector(0.333333333f), Holder.View.TargetHitNormal) * 3;
                    else
                        hStart += JVector.Multiply(new JVector(0.333333333f), Holder.View.TargetHitNormal) * 2;
                    var hEnd = tpos + JVector.Multiply(JVector.One, cpos) + JVector.Multiply(new JVector(0.01f), Holder.View.TargetHitNormal);
                    //if(Holder.View.TargetHitNormal >= JVector.Zero)
                    //    hStart += Holder.View.TargetHitNormal;
                    //else
                    //    hEnd += Holder.View.TargetHitNormal;
                    //hStart = Holder.View.TargetHit;
                    World.CallUI(this, UICommand.Highlight, new Jitter.LinearMath.JBBox(hStart,  hEnd));
                }
                else
                    World.CallUI(this, UICommand.HighlightStop, null);
            }

            base.Update(ElapsedTime);
        }

        public override void UseDevice(GameObject Target, Mob User, UseAction Action)
        {
            if (User.Mind != null && Action == UseAction.Primary)
            {
                if (User.View.TargetStructure != null)
                {
                    var hitPoint = User.View.TargetHitInside;
                    var hitBlock = User.View.TargetStructure[hitPoint.X, hitPoint.Y, hitPoint.Z];

                    if(turf.Block.IsVisible(hitBlock))
                    {
                        var Chunk = User.View.TargetStructure.GetChunkAtPos(hitPoint);
                        var BlockPos = Chunk.Position + new JVector(hitBlock.X, hitBlock.Y, hitBlock.Z) + new JVector(0.5f);

                        structures.Direction dir = structures.Direction.Front;
                        if (User.View.TargetHitNormal == JVector.Forward)
                            dir = structures.Direction.Front;

                        if (User.View.TargetHitNormal == JVector.Backward)
                            dir = structures.Direction.Back;

                        if (User.View.TargetHitNormal == JVector.Left)
                            dir = structures.Direction.Left;

                        if (User.View.TargetHitNormal == JVector.Right)
                            dir = structures.Direction.Right;

                        if (User.View.TargetHitNormal == JVector.Up)
                            dir = structures.Direction.Top;

                        if (User.View.TargetHitNormal == JVector.Down)
                            dir = structures.Direction.Bottom;

                        var currcables = turf.Block.GetCables(hitBlock, dir);

                        datums.turf.Cable.cableType oldCable = datums.turf.Cable.cableType.CurveWN;
                        if (currcables.Count > 0)
                            oldCable = currcables[0];
                        //hitBlock.Cables = null;


                        datums.turf.Cable.cableType newCable = datums.turf.Cable.cableType.LineNS;

                        if((CableX == 0 || CableX == 2) && CableY == 1)
                            newCable = datums.turf.Cable.cableType.LineNS;

                        if ((CableY == 0 || CableY == 2) && CableX == 1)
                            newCable = datums.turf.Cable.cableType.LineWE;

                        if (CableX == 0 && CableY == 0)
                            newCable = datums.turf.Cable.cableType.CurveWN;

                        if (CableX == 2 && CableY == 0)
                            newCable = datums.turf.Cable.cableType.CurveSW;

                        if (CableX == 2 && CableY == 2)
                            newCable = datums.turf.Cable.cableType.CurveES;

                        if (CableX == 0 && CableY == 2)
                            newCable = datums.turf.Cable.cableType.CurveNE;

                        /*switch(oldCable)
                        {
                            case datums.turf.Cable.cableType.LineNS:
                                newCable = datums.turf.Cable.cableType.LineWE;
                                break;
                            case datums.turf.Cable.cableType.LineWE:
                                newCable = datums.turf.Cable.cableType.CurveES;
                                break;
                            case datums.turf.Cable.cableType.CurveES:
                                newCable = datums.turf.Cable.cableType.CurveNE;
                                break;
                            case datums.turf.Cable.cableType.CurveNE:
                                newCable = datums.turf.Cable.cableType.CurveSW;
                                break;
                            case datums.turf.Cable.cableType.CurveSW:
                                newCable = datums.turf.Cable.cableType.CurveWN;
                                break;
                            case datums.turf.Cable.cableType.CurveWN:
                                newCable = datums.turf.Cable.cableType.LineNS;
                                break;
                        }*/

                        turf.Block.SetCable(ref hitBlock, dir, newCable);

                        User.View.TargetStructure[hitPoint.X, hitPoint.Y, hitPoint.Z] = hitBlock;
                        User.View.TargetStructure.GetChunkAtPos(hitPoint).NeedsRender = true;
                    }


                    //User.View.TargetStructure.Add((turf.types.turfTypeE)User.Mind.SelectedBuildObject, User.View.TargetHit);
                }
            }
        }
        private short AddBlockID(short ID)
        {
            if (ID < 1)
                ID = 2;
            else if (ID < 4)
                ID++;
            else
                ID = 1;

            return ID;
        }
    }
}
