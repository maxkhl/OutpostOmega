using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Structures
{
    /// <summary>
    /// Structure gameobjects. Use this class to make windows, stairs, signs, ...
    /// Every structure has to be aligned to the main grid (offset possible)
    /// Use machinery for structures with advanced functionality
    /// </summary>
    public abstract class Structure : GameObject
    {
        #region Properties
        /// <summary>
        /// Durability of this structure. (hitpoint modifier)
        /// </summary>
        public float Durability
        {
            get { return _Durability; }
            set { _Durability = value; NotifyPropertyChanged("Durability"); }
        }
        private float _Durability = 1;

        /// <summary>
        /// Hitpoints of this structure. (health of the structure 0 = destroyed, 100 = perfect)
        /// </summary>
        public float Hitpoints
        {
            get { return _Hitpoints; }
            set { UpdateHP(_Hitpoints, value);  _Hitpoints = value; NotifyPropertyChanged("Hitpoints"); }
        }
        private float _Hitpoints = 100;

        /// <summary>
        /// Anchored-state. False makes this object moveable and it can be affected by 2-dimensional physic (can be dragged around by players or physic)
        /// </summary>
        public bool Anchored
        {
            get { return _Anchored; }
            set { _Anchored = value; NotifyPropertyChanged("Anchored"); }
        }
        private bool _Anchored = true;

        /// <summary>
        /// Size, this object is using (in blockspace)
        /// </summary>
        public bool[,,] SpaceRequirement
        {
            get { return _SpaceRequirement; }
            set { _SpaceRequirement = value; NotifyPropertyChanged(); }
        }
        private bool[,,] _SpaceRequirement;

        /// <summary>
        /// X-Position in connected structures space (not chunk space)
        /// </summary>
        public int BlockX 
        { 
            get
            {
                return _BlockX; 
            }
            set
            {
                _BlockX = value;
                NotifyPropertyChanged();
            }
        }
        private int _BlockX { get; set; }

        /// <summary>
        /// Y-Position in connected structures space (not chunk space)
        /// </summary>
        public int BlockY
        {
            get
            {
                return _BlockY;
            } 
            set
            {
                _BlockY = value;
                NotifyPropertyChanged();
            }        
        }
        private int _BlockY { get; set; }

        /// <summary>
        /// Z-Position in connected structures space (not chunk space)
        /// </summary>
        public int BlockZ
        {
            get
            {
                return _BlockZ;
            }
            protected set
            {
                _BlockZ = value;
                NotifyPropertyChanged();
            }
        }
        private int _BlockZ { get; set; }

        /// <summary>
        /// Overall draw-offset (including structure offset)
        /// </summary>
        public override JVector Offset
        {
            get
            {
                return base.Offset + BlockOffset;
            }
        }

        public JVector BlockOffset
        {
            get
            {
                return _BlockOffset;
            }
            set
            {
                _BlockOffset = value;

                // Trigger pos recalculation per axis
                SetPosition(BlockX, BlockY, BlockZ);

                NotifyPropertyChanged();
            }
        }
        private JVector _BlockOffset;

        /// <summary>
        /// Structure, this object is connected to
        /// </summary>
        public Turf.Structure ParentStructure
        {
            get
            {
                return _ParentStructure;
            }
            set
            {
                _ParentStructure = value;
                NotifyPropertyChanged();
            }
        }
        private Turf.Structure _ParentStructure;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="SpaceRequirement">Determins the space requirement of this object (in block space)</param>
        /// <param name="X">X position in block space</param>
        /// <param name="Y">Y position in block space</param>
        /// <param name="Z">Z position in block space</param>
        /// <param name="Structure">Structure, this gameobject belongs to</param>
        /// <param name="Offset">Offset to the block space (in units)</param>
        /// <param name="World">Connected World</param>
        /// <param name="ID">ID of the object</param>
        public Structure(int X, int Y, int Z, Turf.Structure Structure, World World, string ID = "structure")
            : base(World, ID)
        {
            this.SpaceRequirement = new bool[1,1,1] {{{true}}};
            this.ParentStructure = Structure;
            this.BlockOffset = JVector.Zero;
            this.SetPosition(X, Y, Z);
        }

        /// <summary>
        /// Used to react on hitpoint-changes
        /// </summary>
        protected void UpdateHP(float OldHP, float NewHP)
        {

        }

        public void SetPosition(int BlockX, int BlockY, int BlockZ)
        {
            var rotatedOffset = JVector.Transform(BlockOffset, Orientation);

            var target = new JVector(BlockX + rotatedOffset.X, BlockY + rotatedOffset.Y, BlockZ + rotatedOffset.Z);
            if (ParentStructure != null)
            {
                target.X += ParentStructure.Origin.X;
                target.Y += ParentStructure.Origin.Y;
                target.Z += ParentStructure.Origin.Z;
            }
            SetPosition(target);

            this.BlockX = BlockX;
            this.BlockY = BlockY;
            this.BlockZ = BlockZ;
        }
    }
}
