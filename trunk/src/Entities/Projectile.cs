using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;

using Microsoft.Xna.Framework;

using Shroud.Utilities;

namespace Shroud
{
    public class Projectile : PositionedObject
    {
        #region Fields

        private Sprite mVisibleRepresentation;
        private Circle mCollision;

        // Keep the ContentManager for easy access:
        string mContentManagerName;

        private bool mIsReady;

        #endregion

        #region Properties

        public Circle Collision
        {
            get { return mCollision; }
        }

        public bool IsReady
        {
            get { return mIsReady; }
        }

        #endregion

        #region Methods

        // Constructor
        public Projectile(string contentManagerName)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            mIsReady = true;

            // If you don't want to add to managers, make an overriding constructor
            Initialize(true);
        }

        protected virtual void Initialize(bool addToManagers)
        {
            // Here you can preload any content you will be using
            // like .scnx files or texture files.

            if (addToManagers)
            {
                AddToManagers(null);
            }
        }

        public virtual void AddToManagers(Layer layerToAddTo)
        {
            SpriteManager.AddPositionedObject(this);

            mVisibleRepresentation = SpriteManager.AddSprite(@"Content/Entities/Player/arrow", mContentManagerName);
            mVisibleRepresentation.AttachTo(this, false);
            mVisibleRepresentation.RelativeX = -0.8f;
            GameProperties.RescaleSprite(mVisibleRepresentation);

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);

            mCollision.Radius = 0.3f;

            this.Y = 100.0f;
        }

        public void Activate()
        {
            this.Acceleration.X = -10.0f;
            mIsReady = false;
        }

        public void Deactivate()
        {
            this.Acceleration.X = 0.0f;
            this.Velocity = Vector3.Zero;
            mIsReady = true;
        }

        public virtual void Activity()
        {
            // This code should do things like set Animations, respond to input, and so on.
            float faceAtX = this.X + this.Velocity.X;
            float faceAtY = this.Y + this.Velocity.Y;

            float movementRotation = (float)Math.Atan2(faceAtY - this.Y, faceAtX - this.X);

            this.RotationZ = movementRotation;
        }

        public virtual void Destroy()
        {
            // Remove self from the SpriteManager:
            SpriteManager.RemovePositionedObject(this);

            // Remove any other objects you've created:
            SpriteManager.RemoveSprite(mVisibleRepresentation);
            ShapeManager.Remove(mCollision);
        }

        #endregion
    }
}
