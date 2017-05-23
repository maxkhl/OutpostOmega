using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OutpostOmega.Game.turf.types;

namespace OutpostOmega.Game.turf
{
    /// <summary>
    /// Basically every solid structure like walls, floors and roofs. 
    /// Interior or special structures that require advanced functionality (like windows, doors, stairs, ...), dont belong here. Look at gameobject.structure for that.
    /// This struct is used to store turfs in a compact and performant way.
    /// 
    /// Everything you need to do with a turf, should be done over the static methods. Please change values only directly when you are absolutely sure what you are doing!
    /// </summary>
    public struct Block
    {
        /// <summary>
        /// Type of this turf. Check the turfType enumeration for further info
        /// </summary>
        public byte type;

        /// <summary>
        /// Determins if this block is indoor (can contain atmosphere)
        /// </summary>
        public bool IsIndoor;

        /// <summary>
        /// Temperate of the gas inside this turf
        /// </summary>
        public float Temperature;

        /// <summary>
        /// Determin if this block needs to be processed by the gas simulation
        /// </summary>
        public bool NeedsProcessing;

        /// <summary>
        /// X-Position inside chunk
        /// </summary>
        public byte X;

        /// <summary>
        /// Y-Position inside chunk
        /// </summary>
        public byte Y;

        /// <summary>
        /// Z-Position inside chunk
        /// </summary>
        public byte Z;

        public Int16 UVFront;

        public Int16 UVBack;

        public Int16 UVLeft;

        public Int16 UVRight;

        public Int16 UVTop;

        public Int16 UVBottom;

        public byte[] Cables;

        public byte[] Pipes;

        public static bool HasDecoy(Block turf, structures.Direction direction)
        {
            if (GetCables(turf, direction).Count == 0)
                return false;

            return true;
        }

        public static bool SetCable(ref Block turf, structures.Direction direction, datums.turf.Cable.cableType cableType)
        {
            if (GetCables(turf, direction).Contains(cableType))
                return false;

            int count = 0;
            if (turf.Cables == null)
                turf.Cables = new byte[2];
            else
            {
                bool exists = false;
                for (int i = 0; i < turf.Cables.Length; i += 2)
                {
                    if (turf.Cables[i] == (byte)direction)
                    {
                        count = i;
                        exists = true;
                    }
                }

                if (!exists)
                {
                    count = turf.Cables.Length;
                    Array.Resize(ref turf.Cables, turf.Cables.Length + 2);
                }
            }

            turf.Cables[count] = (byte)direction;
            count++;
            turf.Cables[count] = (byte)cableType;

            return true;
        }
        public static List<datums.turf.Cable.cableType> GetCables(Block turf, structures.Direction direction)
        {
            var ret = new List<datums.turf.Cable.cableType>();

            if (turf.Cables == null || turf.Cables.Length == 0) return ret;

            for(int i = 0; i < turf.Cables.Count(); i+=2)
            {
                if ((structures.Direction)turf.Cables[i] == direction)
                    ret.Add((datums.turf.Cable.cableType)turf.Cables[i + 1]);
            }
            return ret;
        }

        public static void RefreshCovered(Block turf)
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
        public static bool CanContainGas(Block turf)
        {
            return turf.IsIndoor && !GetTurfType(turf.type).IsAirtight;
        }

        /// <summary>
        /// Determins if gas can pass through this block
        /// </summary>
        public static bool IsAirtight(Block turf)
        {
            return GetTurfType(turf.type).IsAirtight;
        }

        /// <summary>
        /// The gas-pressure inside this turf. This value is calculated out of the gas composition inside this turf and its unit-values
        /// </summary>
        public static int Pressure(Block turf)
        {
            float pressure = 0;
            if (turf.gasComposition != null)
            {
                for (int i = 0; i < turf.gasComposition.Count; i++)
                {
                    pressure += turf.gasComposition[i].Units;
                }
            }
            return (int)pressure;
        }

        /// <summary>
        /// Determins if this turf should be drawn / is visible (renderwise)
        /// </summary>
        public static bool IsVisible(Block turf)
        {
            return GetTurfType(turf.type).IsVisible;
        }

        /// <summary>
        /// Every available turf type. Do not change the order!
        /// </summary>
        private static turfType[] types = new turfType[] 
        {             
            new types.space(),
            new types.floor(),            
        };

        /// <summary>
        /// Returns the typeobject of the specific turf. Changing stuff there will apply to every turf of the same type
        /// </summary>
        public static turfType GetTurfType(byte type)
        {
            return types[type];
        }

        /// <summary>
        /// Returns the typeobject of the specific turf. Changing stuff there will apply to every turf of the same type
        /// </summary>
        public static turfType GetTurfType(Block turf)
        {
            return types[turf.type];
        }

        /// <summary>
        /// Creates a new turf object. Better use this to make a valid turf
        /// </summary>
        /// <param name="TurfType">Type</param>
        public static Block Create(turfTypeE TurfType, byte X, byte Y, byte Z)
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
        public static void ModGas(ref Block turf, byte gasID, float units)
        {
            if (!CanContainGas(turf))
                return;

            //This is a space block. So we let air, that got sent to us, dissapear
            if (!turf.IsIndoor)
                return;


            bool hit = false;
            if (turf.gasComposition == null)
                turf.gasComposition = new List<atmospherics.GasState>();

            for (int i = 0; i < turf.gasComposition.Count; i++)
            {
                if (turf.gasComposition[i].GasID == gasID)
                {
                    var gasState = turf.gasComposition[i];
                    gasState.Units += units;
                    turf.gasComposition[i] = gasState;

                    // No gas left
                    if (turf.gasComposition[i].Units == 0)
                        turf.gasComposition.RemoveAt(i);


                    hit = true;
                }
            }
            if (!hit)
            {
                turf.gasComposition.Add(new atmospherics.GasState()
                {
                    GasID = gasID,
                    Units = units
                });
            }

            turf.NeedsProcessing = true; // Important!
        }
    }
}
