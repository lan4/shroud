using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;

using Microsoft.Xna.Framework;

namespace Shroud.Entities
{
    public class InteractObject : PositionedObject
    {
        #region Fields

        // Basic Entity Properties
        // private Sprite mVisibleRepresentation;
        private Circle mCollision;

        // Keep the ContentManager for easy access:
        string mContentManagerName;

        private Vector3 mInteractOffset;

        private bool mIsActive;

        #endregion

        #region Properties

        public Circle Collision
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

        #endregion

        #region Methods

        // Constructor
        public InteractObject(string contentManagerName, Vector3 offset)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            mInteractOffset = offset;

            // If you don't want to add to managers, make an overriding constructor
            Initialize(true);
        }

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
            // Add the Entity to the SpriteManager
            // so it gets managed properly (velocity, acceleration, attachments, etc.)
            SpriteManager.AddPositionedObject(this);

            // Here you may want to add your objects to the engine.  Use layerToAddTo
            // when adding if your Entity supports layers.  Make sure to attach things
            // to this if appropriate.
            //mVisibleRepresentation = SpriteManager.AddSprite("redball.bmp", contentManagerName);
            //mVisibleRepresentation.AttachTo(this, false);

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);
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
            //SpriteManager.RemoveSprite(mVisibleRepresentation);
            ShapeManager.Remove(mCollision);
        }

        #endregion
    }
}
