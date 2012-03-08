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

        // Here you'd define things that your Entity contains, like Sprites
        // or Circles:
        private Sprite mVisibleRepresentation;
        private Circle mCollision;

        // Keep the ContentManager for easy access:
        string mContentManagerName;

        private string mID;

        #endregion

        #region Properties


        // Here you'd define properties for things
        // you want to give other Entities and game code
        // access to, like your Collision property:
        public Circle Collision
        {
            get { return mCollision; }
        }

        public string ID
        {
            get { return mID; }
        }

        #endregion

        #region Methods

        // Constructor
        public Button(string contentManagerName, string id)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            mID = id;

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
            /*
            mVisibleRepresentation = SpriteManager.AddSprite("redball.png", mContentManagerName);
            mVisibleRepresentation.AttachTo(this, false);
            mVisibleRepresentation.RelativeRotationZ = GameProperties.WorldRotation;*/

            InitializeSprites();

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);
            mCollision.Radius = 2.0f;
        }

        private void InitializeSprites()
        {
            AnimationChainList buttonSprites = new AnimationChainList();

            AnimationChain buttonOn = new AnimationChain();

            buttonOn.Add(new AnimationFrame(@"Content/Entities/Button/bow_btn_2", 0.0833f, mContentManagerName));

            buttonOn.Name = "ON";

            AnimationChain buttonOff = new AnimationChain();

            buttonOff.Add(new AnimationFrame(@"Content/Entities/Button/character_btn2", 0.0833f, mContentManagerName));

            buttonOff.Name = "OFF";

            buttonSprites.Add(buttonOn);
            buttonSprites.Add(buttonOff);

            mVisibleRepresentation = SpriteManager.AddSprite(buttonSprites);
            mVisibleRepresentation.AttachTo(this, false);
            mVisibleRepresentation.RelativeRotationZ = GameProperties.WorldRotation;
            mVisibleRepresentation.CurrentChainName = "OFF";

            GameProperties.RescaleSprite(mVisibleRepresentation);
        }

        public void Toggle()
        {
            if (mVisibleRepresentation.CurrentChainName.Equals("ON"))
                mVisibleRepresentation.CurrentChainName = "OFF";
            else
                mVisibleRepresentation.CurrentChainName = "ON";
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
