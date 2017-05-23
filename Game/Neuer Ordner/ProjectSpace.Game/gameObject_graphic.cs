﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.LinearMath;
using System.IO;

namespace OutpostOmega.Game
{
    /// <summary>
    /// Graphics-part of the gameobject class. Mainly used for Position and 3D-stuff
    /// </summary>
    public partial class GameObject
    {
        #region Public Properties
        /// <summary>
        /// Position of the models origin. Set accessor is protected. Please use the method SetPosition()
        /// </summary>
        [GameObjects.Attributes.SynchronizationAttr(GameObjects.Attributes.SynchronizePriority.UnreliableSequenced, GameObjects.Attributes.SynchronizeState.Prediction)]
        public JVector Position
        {
            get
            {
                if (this.Parent != null)
                    return this.Parent.Position + localPosition;
                else
                    if (IsPhysical)
                        return RigidBody.Position;
                    else
                        return _Position;
            }
            set
            {
                if (Position != value)
                {
                    LastMove = _Position - value;
                    if (IsPhysical)
                        SetPosition(value);

                    NotifyPropertyChanged();
                }
                _Position = value;
            }
        }
        private JVector _Position;

        /// <summary>
        /// Last movement delta (oldPos - newPos) - NOT SYNCHRONIZED
        /// </summary>
        [GameObjects.Attributes.SynchronizationAttr(GameObjects.Attributes.SynchronizePriority.NoSynchronization)]
        public JVector LastMove { get; set; }

        /// <summary>
        /// Orientation of this object
        /// </summary>
        [GameObjects.Attributes.SynchronizationAttr(GameObjects.Attributes.SynchronizePriority.UnreliableSequenced, GameObjects.Attributes.SynchronizeState.Prediction)]
        public virtual JMatrix Orientation
        {
            get
            {
                if (Parent != null)
                    return Parent.Orientation * _localOrientation;
                else
                {
                    if (RigidBody == null)
                        return _Orientation;
                    else
                        return RigidBody.Orientation;
                }
            }
            set
            {
                if (!JMatrix.Compare(value, Orientation))
                {
                    if (Parent != null)
                        _localOrientation = JMatrix.Inverse(Parent.Orientation) * value;
                    else
                        if (RigidBody == null && !JMatrix.Compare(_Orientation, value) ||
                            RigidBody != null && !JMatrix.Compare(RigidBody.Orientation, value))
                        {
                            if (RigidBody == null)
                                _Orientation = value;
                            else
                                RigidBody.Orientation = value;

                        }
                    NotifyPropertyChanged("Orientation");
                }
            }
        }
        public JMatrix _Orientation = JMatrix.Identity;

        /// <summary>
        /// Scale of this object. Only visual. Physics is not affected. (Change the rigidbody to archieve that)
        /// </summary>
        public float Scale
        {
            get
            {
                return _Scale;
            }
            set
            {
                _Scale = value;
                NotifyPropertyChanged("Scale");
            }
        }
        public float _Scale = 1;

        public JMatrix localOrientation
        {
            get
            {
                return _localOrientation;
            }
            set
            {
                _localOrientation = value;
                NotifyPropertyChanged();
            }
        }
        private JMatrix _localOrientation = JMatrix.Identity;

        /// <summary>
        /// Returns the forward-vector of this object
        /// </summary>
        public JVector Forward
        {
            get
            {
                return JMatrix.ViewMatrixForward(this.Orientation);
            }
        }

        /// <summary>
        /// All meshs that are assigned to this gameobject
        /// </summary>
        [GameObjects.Attributes.Serialize(GameObjects.Attributes.SerializeState.DoNotSerialize)]
        public List<Content.Mesh> Meshs
        {
            get
            {
                List<Content.Mesh> meshList = new List<Content.Mesh>();
                foreach(var contentFile in 
                    (from contentFile in World.ContentManager.LoadedContent
                     where
                        contentFile.GetType() == typeof(Content.Model)
                         select (Content.Model)contentFile))
                {
                    meshList.AddRange(contentFile.GetMesh(this));
                }
                return meshList;
            }
        }

        /// <summary>
        /// Makes this and all child objects invisible
        /// </summary>
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                if (this._Visible != value)
                {
                    this._Visible = value;

                    NotifyPropertyChanged();
                }

                // Set all children (and they set their children)
                foreach (var child in this.Children)
                    child.Visible = value;
            }
        }
        private bool _Visible = true;

        /// <summary>
        /// Binds a mesh to this gameobject
        /// </summary>
        public void AddMesh(Content.Mesh mesh)
        {
            mesh.Add(this);
        }

        /// <summary>
        /// Unbinds a mesh from this gameobject
        /// </summary>
        public void RemoveMesh(Content.Mesh mesh)
        {
            mesh.Remove(this);
        }

        /// <summary>
        /// Loads a model, binds all meshs to this gameObject and introduces it to the Worlds ContentManager
        /// </summary>
        public Content.Model LoadModel(string modelPath)
        {
            var model = (Content.Model)World.ContentManager.Load(modelPath);
            World.ContentManager.Add(model);
            model.AssignGameObject(this);
            return model;
        }

        #endregion
    }
}
