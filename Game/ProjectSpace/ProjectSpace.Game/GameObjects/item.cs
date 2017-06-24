using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects
{
    /// <summary>
    /// Item base
    /// Items are carry- and equiptable
    /// </summary>
    abstract public class Item : GameObject
    {
        public Mob Holder 
        { 
            get
            {
                return _Holder;
            }
            set
            {
                _Holder = value;
                NotifyPropertyChanged();
            }
        }
        private Mob _Holder = null;

        public Item(World world, string ID = "item")
            : base(world, ID)
        {
            // Items should not react to characters
            this.IsPassable = true;
        }

        /*public override void KeyPress(OpenTK.Input.Key Key, bool IsRepeat)
        {
            if (Key == OpenTK.Input.Key.E && !IsRepeat)
                this.AddToInventory(World.Player.Mob);
            
            base.KeyPress(Key, IsRepeat);
        }*/

        public Mob Carrier { get; set; }
        /*public void AddToInventory(Mob Mob)
        {
            Carrier = Mob;
            if(!Mob.Inventory.Contains(this))
                Mob.Inventory.Add(this);

            if(this.RigidBody != null)
                World.PhysicSystem.RemoveBody(this.RigidBody);

            this.Visible = false;
        }
        public void DropFromInventory()
        {
            if (Carrier.Inventory.Contains(this))
                Carrier.Inventory.Remove(this);

            float DropDistance = 2;
            var DropPosition = Carrier.Position + JVector.Transform(JVector.Forward * DropDistance, Carrier.Orientation);

            this.SetPosition(DropPosition);

            if (this.RigidBody != null)
                World.PhysicSystem.AddBody(this.RigidBody);

            this.Visible = true;
        }*/

        public override bool Use(Mob User, Item Item, Game.Tools.Action Action)
        {
            if(Item == null) //Hands
            {
                if (Action == Game.Tools.Action.InteractPrimary) // Equip
                {
                    var qSlot = User.AddToQuickslot(this);
                    if (qSlot >= 0)
                        User.SelectedQuickslot = qSlot;
                }
            }

            return base.Use(User, Item, Action);
        }
    }
}
