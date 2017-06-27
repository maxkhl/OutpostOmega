using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OutpostOmega.Game.Turf.Types;

namespace OutpostOmega.Game.Turf
{
    /// <summary>
    /// Basically every solid structure like walls, floors and roofs. 
    /// Interior or special structures that require advanced functionality (like windows, doors, stairs, ...), dont belong here. Look at gameobject.structure for that.
    /// This struct is used to store turfs in a compact and performant way.
    /// 
    /// Everything you need to do with a turf, should be done over the methods. Please change values only directly when you are absolutely sure what you are doing!
    /// </summary>
    public struct Block
    {
        /// <summary>
        /// Type of this turf. Check the turfType enumeration for further info
        /// </summary>
        public byte type { get; private set; }

        /// <summary>
        /// Determins if this block is indoor (can contain atmosphere)
        /// </summary>
        public bool IsIndoor { get; set; }

        /// <summary>
        /// Temperate of the gas inside this turf
        /// </summary>
        public float Temperature { get; private set; }

        /// <summary>
        /// Determin if this block needs to be processed by the gas simulation
        /// </summary>
        public bool NeedsProcessing { get; set; }

        /// <summary>
        /// X-Position inside chunk
        /// </summary>
        public byte X { get; private set; }

        /// <summary>
        /// Y-Position inside chunk
        /// </summary>
        public byte Y { get; private set; }

        /// <summary>
        /// Z-Position inside chunk
        /// </summary>
        public byte Z { get; private set; }

        public Int16 UVFront;

        public Int16 UVBack;

        public Int16 UVLeft;

        public Int16 UVRight;

        public Int16 UVTop;

        public Int16 UVBottom;

        public byte[] Cables;

        public byte[] Pipes;

        public bool HasDecoy(Structures.Direction direction)
        {
            if (GetCables( direction).Count == 0)
                return false;

            return true;
        }

        public bool SetCable(Structures.Direction direction, datums.turf.Cable.cableType cableType)
        {
            if (GetCables(direction).Contains(cableType))
                return false;

            int count = 0;
            if (this.Cables == null)
                this.Cables = new byte[2];
            else
            {
                bool exists = false;
                for (int i = 0; i < this.Cables.Length; i += 2)
                {
                    if (this.Cables[i] == (byte)direction)
                    {
                        count = i;
                        exists = true;
                    }
                }

                if (!exists)
                {
                    count = this.Cables.Length;
                    Array.Resize(ref this.Cables, this.Cables.Length + 2);
                }
            }

            this.Cables[count] = (byte)direction;
            count++;
            this.Cables[count] = (byte)cableType;

            return true;
        }
        public List<datums.turf.Cable.cableType> GetCables(Structures.Direction direction)
        {
            var ret = new List<datums.turf.Cable.cableType>();

            if (this.Cables == null || this.Cables.Length == 0) return ret;

            for(int i = 0; i < this.Cables.Count(); i+=2)
            {
                if ((Structures.Direction)this.Cables[i] == direction)
                    ret.Add((datums.turf.Cable.cableType)this.Cables[i + 1]);
            }
            return ret;
        }

        public void RefreshCovered()
        {

        }


        /// <summary>
        /// The gas composition inside this turf
        /// </summary>
        public List<atmospherics.GasState> gasComposition;

        /// <summary>
        /// Gas-changes that are waiting for processing
        /// </summary>
        public Queue<atmospherics.GasState> gasQueue { get; set; }


        /// <summary>
        /// Determins if this turf is able to contain gas (Airtight will override this value!)
        /// </summary>
        public bool CanContainGas
        {
            get
            {
                return this.IsIndoor && !this.TurfType.IsAirtight;
            }
        }

        /// <summary>
        /// Determins if gas can pass through this block
        /// </summary>
        public bool IsAirtight
        {
            get
            {
                return this.TurfType.IsAirtight;
            }
        }

        /// <summary>
        /// The gas-pressure inside this turf. This value is calculated out of the gas composition inside this turf and its unit-values
        /// </summary>
        public int Pressure
        {
            get
            {
                float pressure = 0;
                if (this.gasComposition != null)
                {
                    for (int i = 0; i < this.gasComposition.Count; i++)
                    {
                        pressure += this.gasComposition[i].Units;
                    }
                }
                return (int)pressure;
            }
        }

        /// <summary>
        /// Determins if this turf should be drawn / is visible (renderwise)
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return this.TurfType.IsVisible;
            }
        }

        /// <summary>
        /// Every available turf type. Do not change the order!
        /// </summary>
        private static TurfType[] types = new TurfType[] 
        {             
            new Types.SpaceTurf(),
            new Types.FloorTurf(),            
        };

        /// <summary>
        /// Returns the typeobject of the specific turf. Changing stuff there will apply to every turf of the same type
        /// </summary>
        public TurfType TurfType
        {
            get
            {
                return types[this.type];
            }
        }

        /// <summary>
        /// Creates a new block. Better use this to make a valid block
        /// </summary>
        /// <param name="TurfType">Type</param>
        public static Block Create(TurfTypeE TurfType, byte X, byte Y, byte Z)
        {
            return new Block()
            {
                type = (byte)TurfType,
                X = X,
                Y = Y,
                Z = Z,
                UVFront = -1,
                UVBack = -1,
                UVLeft = -1,
                UVRight = -1,
                UVTop = -1,
                UVBottom = -1,
            };
        }
        

        /// <summary>
        /// Modifies a specific type and amount of gas to this turf
        /// </summary>
        /// <param name="gasID">Gas ID</param>
        /// <param name="units">Units of the gas to add</param>
        public void ModGas(byte gasID, float units)
        {
            if (!this.CanContainGas)
                return;

            //This is a space block. So we let air, that got sent to us, dissapear
            if (!this.IsIndoor)
                return;


            bool hit = false;
            if (this.gasComposition == null)
                this.gasComposition = new List<atmospherics.GasState>();

            for (int i = 0; i < this.gasComposition.Count; i++)
            {
                if (this.gasComposition[i].GasID == gasID)
                {
                    var gasState = this.gasComposition[i];
                    gasState.Units += units;
                    this.gasComposition[i] = gasState;

                    // No gas left
                    if (this.gasComposition[i].Units == 0)
                        this.gasComposition.RemoveAt(i);


                    hit = true;
                }
            }
            if (!hit)
            {
                this.gasComposition.Add(new atmospherics.GasState()
                {
                    GasID = gasID,
                    Units = units
                });
            }

            this.NeedsProcessing = true; // Important!
        }
    }
}
