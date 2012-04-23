using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Math.Geometry;

using Shroud.Utilities;
using Scene = Shroud.Utilities.Scene;

namespace Shroud.Entities
{
    public abstract class Entity : PositionedObject
    {
        #region Fields

        // Basic Entity Properties
        protected Sprite mAppearance;
        protected Circle mCollision;

        public Scene MyScene;

        // Keep the ContentManager for easy access:
        string mContentManagerName;

        #endregion

        #region Properties

        public Circle Collision
        {
            get { return mCollision; }
        }

        protected string ContentManagerName
        {
            get { return mContentManagerName; }
        }

        #endregion

        #region Methods

        // Constructor
        protected Entity(string contentManagerName)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            SpriteManager.AddPositionedObject(this);
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
