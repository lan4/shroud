using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Input;

using Shroud.Utilities;

namespace Shroud.Entities
{
    public class Button : PositionedObject
    {
        #region Fields

        // Basic Entity Properties
        private Sprite mAppearance;
        private Circle mCollision;

        // Keep the ContentManager for easy access:
        string mContentManagerName;

        #endregion

        #region Properties

        public Circle Collision
        {
            get { return mCollision; }
        }

        public bool On
        {
            get { return mAppearance.CurrentChainName == "ON"; }
        }

        public bool Visible
        {
            get { return mAppearance.Visible; }
            set { mAppearance.Visible = value; }
        }

        #endregion

        #region Methods

        // Constructor
        public Button(string contentManagerName, string assetname)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            // If you don't want to add to managers, make an overriding constructor
            Initialize(true, assetname);
        }

        protected virtual void Initialize(bool addToManagers, string assetname)
        {

            if (addToManagers)
            {
                AddToManagers(null, assetname);
            }
        }

        public virtual void AddToManagers(Layer layerToAddTo, string assetname)
        {
            SpriteManager.AddPositionedObject(this);

            InitializeSprites(assetname);

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);
            mCollision.Radius = 2.0f;
        }

        private void InitializeSprites(string assetname)
        {
            AnimationChainList buttonSprites = new AnimationChainList();

            AnimationChain buttonOn = new AnimationChain();

            buttonOn.Add(new AnimationFrame(@"Content/Entities/Button/" + assetname + "_ON", 0.0833f, mContentManagerName));

            buttonOn.Name = "ON";

            AnimationChain buttonOff = new AnimationChain();

            buttonOff.Add(new AnimationFrame(@"Content/Entities/Button/" + assetname + "_OFF", 0.0833f, mContentManagerName));

            buttonOff.Name = "OFF";

            buttonSprites.Add(buttonOn);
            buttonSprites.Add(buttonOff);

            mAppearance = SpriteManager.AddSprite(buttonSprites);
            mAppearance.AttachTo(this, false);
            mAppearance.RelativeRotationZ = GameProperties.WorldRotation;
            mAppearance.CurrentChainName = "OFF";

            GameProperties.RescaleSprite(mAppearance);
        }

        public void Toggle()
        {
            if (mAppearance.CurrentChainName.Equals("ON"))
                mAppearance.CurrentChainName = "OFF";
            else
                mAppearance.CurrentChainName = "ON";
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
            SpriteManager.RemoveSprite(mAppearance);
            ShapeManager.Remove(mCollision);
        }

        #endregion
    }
}
