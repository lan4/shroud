using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;

using Microsoft.Xna.Framework;

using Point = FlatRedBall.Math.Geometry.Point;

using Shroud.Utilities;

namespace Shroud.Entities
{
    public class Obstacle : PositionedObject
    {
        #region Fields

        // Here you'd define things that your Entity contains, like Sprites
        // or Circles:
        private Sprite mVisibleRepresentation;
        private Polygon mCollision;

        // Keep the ContentManager for easy access:
        string mContentManagerName;

        private bool mActive;

        public enum OType
        {
            None,
            Hide,
            Solid
        };

        private OType mType;

        private Vector3 mInteractPoint;

        #endregion

        #region Properties

        public Polygon Collision
        {
            get { return mCollision; }
        }

        public bool IsActive
        {
            get { return mActive; }
            set { mActive = value; }
        }

        public OType myType
        {
            get { return mType; }
        }

        public Vector3 InteractPoint
        {
            get { return mInteractPoint + this.Position; }
        }

        #endregion

        #region Methods

        // Constructor
        public Obstacle(string contentManagerName, OType t, Point[] pa)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            mType = t;

            // If you don't want to add to managers, make an overriding constructor
            Initialize(true, pa);
        }

        protected virtual void Initialize(bool addToManagers, Point[] pa)
        {
            // Here you can preload any content you will be using
            // like .scnx files or texture files.

            mInteractPoint = new Vector3();

            if (addToManagers)
            {
                AddToManagers(null, pa);
            }
        }

        public virtual void AddToManagers(Layer layerToAddTo, Point[] pa)
        {
            SpriteManager.AddPositionedObject(this);

            int val = (int)TimeManager.CurrentTime % 2;
            val++;

            if (mType.Equals(OType.Hide))
                mVisibleRepresentation = SpriteManager.AddSprite(@"Content/Entities/Background/bush" + val, mContentManagerName);
            else
            {
                mVisibleRepresentation = SpriteManager.AddSprite("redball.png", mContentManagerName);
                mVisibleRepresentation.Visible = false;
            }

            mVisibleRepresentation.AttachTo(this, false);
            mVisibleRepresentation.RelativeRotationZ = GameProperties.WorldRotation;
            
            GameProperties.RescaleSprite(mVisibleRepresentation);

            mCollision = ShapeManager.AddPolygon();

            Point[] pointArray =
            {
                new Point(-2.0f,  2.0f),
                new Point( 2.0f,  2.0f),
                new Point( 2.0f, -2.0f),
                new Point(-2.0f, -2.0f),
                new Point(-2.0f,  2.0f)
            };

            if (pa != null)
            {
                mCollision.Points = pa;
            }
            else
                mCollision.Points = pointArray;

            //System.Diagnostics.Debug.WriteLine(mCollision.Points);

            mCollision.AttachTo(this, false);
        }

        public virtual void Activity()
        {
            // This code should do things like set Animations, respond to input, and so on.
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
