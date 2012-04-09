using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;


// Be sure to replace:
// 1.  The namespace
// 2.  The class name
// 3.  The constructor (should be the same as the class name)


namespace Shroud.Entities
{
    public abstract class Entity : PositionedObject
    {
        #region Fields

        // Basic Entity Properties
        protected Sprite mAppearance;
        protected Circle mCollision;

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

            // If you don't want to add to managers, make an overriding constructor
            Initialize(true);
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
