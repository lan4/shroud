using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;

using Microsoft.Xna.Framework;

namespace Shroud.Entities
{
    public class WorldObject : PositionedObject
    {
        #region Fields

        // Basic Entity Properties
        private Sprite mAppearance;
        private AxisAlignedRectangle mCollision;

        // Keep the ContentManager for easy access:
        string mContentManagerName;

        private Vector3 mInteractOffset;

        private bool mIsActive;

        public enum ObjectType
        {
            Normal,
            Interactive,
            Destructible,
            Ground
        };

        private ObjectType mType;

        #endregion

        #region Properties

        public AxisAlignedRectangle Collision
        {
            get { return mCollision; }
        }

        public Vector3 InteractPoint
        {
            get { return this.Position + mInteractOffset; }
        }

        public bool IsActive
        {
            get { return mIsActive; }
            set { mIsActive = value; }
        }

        public ObjectType OType
        {
            get { return mType; }
        }

        #endregion

        #region Methods

        #region Constructors

        public WorldObject(string contentManagerName, ObjectType o, Vector3 offset)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            mInteractOffset = offset;
            mType = o;

            Initialize(true);
        }

        public WorldObject(string contentManagerName, ObjectType o)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            mInteractOffset = new Vector3(0, 0, 0);
            mType = o;

            Initialize(true);
        }

        public WorldObject(string contentManagerName)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            mInteractOffset = new Vector3(0, 0, 0);
            mType = ObjectType.Normal;

            Initialize(true);
        }

        #endregion

        protected virtual void Initialize(bool addToManagers)
        {
            mIsActive = false;

            if (addToManagers)
            {
                AddToManagers(null);
            }
        }

        public virtual void AddToManagers(Layer layerToAddTo)
        {
            SpriteManager.AddPositionedObject(this);

            // Here you may want to add your objects to the engine.  Use layerToAddTo
            // when adding if your Entity supports layers.  Make sure to attach things
            // to this if appropriate.
            //mAppearance = SpriteManager.AddSprite("redball.bmp", contentManagerName);
            //mAppearance.AttachTo(this, false);

            mCollision = ShapeManager.AddAxisAlignedRectangle();
            mCollision.AttachTo(this, false);
        }

        private void InitializeAnimations()
        {

        }

        public virtual void Activity()
        {
            // Probably Unused
        }

        public virtual void Destroy()
        {
            // Remove self from the SpriteManager:
            SpriteManager.RemovePositionedObject(this);

            // Remove any other objects you've created:
            SpriteManager.RemoveSprite(mAppearance);
            ShapeManager.Remove(mCollision);
        }

        #endregion
    }
}

