using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Math.Geometry;

using Shroud.Utilities;

namespace Shroud.Entities
{
    public class Trap : PositionedObject
    {
        #region Fields

        // Basic Entity Properties
        private Sprite mAppearance;
        private Circle mCollision;

        // Keep the ContentManager for easy access:
        string mContentManagerName;

        public enum TrapType
        {
            Bomb,
            Trip,
            Smoke
        }

        private TrapType mType;
        private float mYOffset;

        #endregion

        #region Properties

        public Circle Collision
        {
            get { return mCollision; }
        }

        public TrapType TType
        {
            get { return mType; }
        }

        public bool IsActive
        {
            get { return mAppearance.Visible; }
        }

        public float YOffset
        {
            get { return mYOffset; }
        }

        #endregion

        #region Methods

        // Constructor
        public Trap(string contentManagerName, TrapType t)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            mType = t;

            // If you don't want to add to managers, make an overriding constructor
            Initialize(true);
        }

        protected virtual void Initialize(bool addToManagers)
        {
            switch (mType)
            {
                case TrapType.Bomb:
                    mYOffset = -2.0f;
                    break;
                case TrapType.Trip:
                    mYOffset = -2.0f;
                    break;
                case TrapType.Smoke:
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Error: Trap type invalid");
                    break;
            }

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

            // InitializeAnimations();
            // REPLACE WITH INITIALIZE ANIMATIONS
            mAppearance = SpriteManager.AddSprite(@"Content/Entities/Trap/Bomb/flash_bomb", mContentManagerName);
            mAppearance.AttachTo(this, false);
            mAppearance.Visible = false;
            mAppearance.RelativeRotationZ = GameProperties.WorldRotation;
            GameProperties.RescaleSprite(mAppearance);

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);
        }

        private void InitializeAnimations()
        {
            AnimationChainList animations = new AnimationChainList();
            float frametime = 0.012f;
            int framenum = 0;

            AnimationChain idle = new AnimationChain();
            AnimationChain active = new AnimationChain();

            switch (mType)
            {
                case TrapType.Bomb:
                    int bombTotalFrames = 10;
                    for (framenum = 0; framenum < bombTotalFrames; framenum++)
                    {
                        idle.Add(new AnimationFrame(@"Content/Trap/Bomb/idle" + framenum, frametime, mContentManagerName));
                    }

                    bombTotalFrames = 10;
                    for (framenum = 0; framenum < bombTotalFrames; framenum++)
                    {
                        active.Add(new AnimationFrame(@"Content/Trap/Bomb/active" + framenum, frametime, mContentManagerName));
                    }
                    break;
                case TrapType.Trip:
                    int tripTotalFrames = 10;
                    for (framenum = 0; framenum < tripTotalFrames; framenum++)
                    {
                        idle.Add(new AnimationFrame(@"Content/Trap/Trip/idle" + framenum, frametime, mContentManagerName));
                    }

                    tripTotalFrames = 10;
                    for (framenum = 0; framenum < tripTotalFrames; framenum++)
                    {
                        active.Add(new AnimationFrame(@"Content/Trap/Trip/active" + framenum, frametime, mContentManagerName));
                    }
                    break;
            }

            idle.Name = "Idle";
            active.Name = "Active";
            animations.Add(idle);
            animations.Add(active);

            mAppearance = SpriteManager.AddSprite(animations);
            mAppearance.AttachTo(this, false);
            GameProperties.RescaleSprite(mAppearance);

            mAppearance.CurrentChainName = "Idle";
            mAppearance.Visible = false;
        }

        #region Main Functions

        public void Trigger()
        {
            mAppearance.CurrentChainName = "Active";
        }

        public bool IsTriggerDone()
        {
            return mAppearance.CurrentChainName == "Active" && mAppearance.JustCycled;
        }

        public void Deactivate()
        {
            //mAppearance.CurrentChainName = "Idle";
            mAppearance.Visible = false;
        }

        public void Activate()
        {
            mAppearance.Visible = true;
        }

        #endregion

        public virtual void Activity()
        {
            // Unneeded
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
