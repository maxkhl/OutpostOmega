using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Input;
using OpenTK;
using Jitter.LinearMath;

namespace OutpostOmega.Game.GameObjects.Mobs
{
    /// <summary>
    /// A standard mind - interface between different types of inputs (local, remote or ai) and mob
    /// </summary>
    public abstract class Mind : GameObject
    {
        /// <summary>
        /// The Mob, this mind is assigned to
        /// </summary>
        public Mob Mob
        { 
            get
            {
                return _Mob;
            }
            set
            {
                if (this._Mob != value)
                {
                    _Mob = value;
                    this.Parent = _Mob;
                    _Mob.Mind = this;
                    NotifyPropertyChanged("Mob");
                }
            }
        }
        private Mob _Mob;

        /// <summary>
        /// Gets a value indicating whether this mind has body.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has a body; otherwise, <c>false</c>.
        /// </value>
        public bool HasBody
        {
            get
            {
                return Mob != null;
            }
        }

        /// <summary>
        /// Currently active actions for this mind
        /// </summary>
        [Attributes.Serialize(Attributes.SerializeState.DoNotSerialize)]
        public List<Tools.Action> ActiveActions { get; set; }


        public Mind(World world, string ID = "mind") : base(world, ID)
        {
        }

        public override void Initialise()
        {
            ActiveActions = new List<Tools.Action>();
            this.ActionActivated += Mind_ActionActivated;

            base.Initialise();
        }

        private void Mind_ActionActivated(Tools.Action Action)
        {
            this.Mob?.DoUse(Action);
        }

        /// <summary>
        /// Lets this mind execute the given action with the given state
        /// </summary>
        /// <param name="Action">Action to execute</param>
        /// <param name="ActionState">State, the action should take</param>
        protected void ExecuteAction(Tools.Action Action, Tools.ActionState ActionState)
        {
            lock (ActiveActions)
            {
                switch (ActionState)
                {
                    //Add it if inactive
                    case Tools.ActionState.Activate:
                        if (!ActiveActions.Contains(Action))
                        {
                            World.DebugMessages.Enqueue(Action.ToString());
                            ActiveActions.Add(Action);
                            ActionActivated?.Invoke(Action);
                        }
                        break;

                    //Remove it if active
                    case Tools.ActionState.Release:
                        if (ActiveActions.Contains(Action))
                        {
                            ActiveActions.Remove(Action);
                            ActionDeactivated?.Invoke(Action);
                        }
                        break;

                    //Do both
                    case Tools.ActionState.Toggle:
                        if (ActiveActions.Contains(Action))
                        {
                            ActiveActions.Remove(Action);
                            ActionDeactivated?.Invoke(Action);
                        }
                        else
                        {
                            ActiveActions.Add(Action);
                            ActionActivated?.Invoke(Action);
                        }
                        break;

                    //Throw error because that should not arrive here
                    case Tools.ActionState.Undefined:
                        throw new Exception(String.Format("Undefined action state arrived in '{0}' the action was '{1}'", this.ToString(), Action.ToString()));
                }
            }
        }

        #region ActionEvents
        /// <summary>
        /// Handler for the ActionActivated event
        /// </summary>
        /// <param name="Action">Action that got activated</param>
        public delegate void ActionActivatedHandler(Tools.Action Action);

        /// <summary>
        /// Gets fired when a action is being added to the ActiveActions list
        /// </summary>
        public event ActionActivatedHandler ActionActivated;

        /// <summary>
        /// Handler for the ActionDeactivated event
        /// </summary>
        /// <param name="Action">Action that got deactivated</param>
        public delegate void ActionDeactivatedHandler(Tools.Action Action);

        /// <summary>
        /// Gets fired when a action is being removed from the ActiveActions list
        /// </summary>
        public event ActionDeactivatedHandler ActionDeactivated;
        #endregion

        public override void Update(double ElapsedTime)
        {
            //Locks all updating after that
            //This should include playerinput and ai calculations
            if (!this.Freeze)
                base.Update(ElapsedTime);
        }

        /// <summary>
        /// Freezes this mind. It wont be able to think or control a mob anymore
        /// </summary>
        public bool Freeze { get; set; }
    }
}
