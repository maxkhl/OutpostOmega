using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutpostOmega.Game.GameObjects.Items.Devices
{
    /// <summary>
    /// Contains code for spawn tools. Like targeting and spawndirections and shit
    /// </summary>
    public abstract class SpawnTool : Device
    {


        /// <summary>
        /// Defines the object that should be spawned in buildmode.
        /// </summary>
        [Attributes.Serialize(Attributes.SerializeState.DoNotSerialize)]
        public object SelectedBuildObject { get; set; }

        /// <summary>
        /// Direction, a object will be facing when spawned (0 = X+, 1 = Z+, 2 = X-, 3 = Z-)
        /// </summary>
        public byte SpawnDirection { get; set; }

        public SpawnTool(World world, string ID = "spawntool")
            : base(world, ID)
        {
            this.SpawnDirection = 0;
        }

        /// <summary>
        /// Update-method of this gameObject
        /// </summary>
        public override void Update(double ElapsedTime)
        {
            // Figure out the direction the holder is looking and make it the spawndirection
            if (Holder != null && Holder.View != null)
            {
                if (Holder.View.Forward.X > 0)
                {
                    if (Holder.View.Forward.X > Holder.View.Forward.Z)
                        this.SpawnDirection = 0;
                    else if (Holder.View.Forward.X < Holder.View.Forward.Z)
                        this.SpawnDirection = 2;
                }
                else
                    if (Holder.View.Forward.X > Holder.View.Forward.Z)
                        this.SpawnDirection = 1;
                    else if (Holder.View.Forward.X < Holder.View.Forward.Z)
                        this.SpawnDirection = 3;
            }

            base.Update(ElapsedTime);
        }

        public override void UseDevice(GameObject Target, Mob User, UseAction Action)
        {
            // Allows user to rotate object
            if (Action == UseAction.Tertiary)
                if (this.SpawnDirection == 3)
                    this.SpawnDirection = 0;
                else
                    this.SpawnDirection++;
        }
    }
}
