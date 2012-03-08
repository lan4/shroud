using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Input;

using Microsoft.Xna.Framework;

namespace Shroud.Entities
{
    public class HUD : PositionedObject
    {
        #region Fields

        // Keep the ContentManager for easy access:
        string mContentManagerName;

        private Button mBowButton;
        private Button[] mButtons;

        #endregion

        #region Properties


        public Button BowButton
        {
            get { return mBowButton; }
        }

        #endregion

        #region Methods

        // Constructor
        public HUD(string contentManagerName)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            // If you don't want to add to managers, make an overriding constructor
            Initialize(true);
        }

        protected virtual void Initialize(bool addToManagers)
        {
            // Here you can preload any content you will be using
            // like .scnx files or texture files.

            mButtons = new Button[1];

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

            mBowButton = new Button(mContentManagerName, "BOW_BUTTON");
            mBowButton.AttachTo(this, false);
            //mBowButton.RelativePosition.X = 14.0f;
            //mBowButton.RelativePosition.Y = -8.0f;
            mBowButton.RelativePosition.X = -6.0f;
            mBowButton.RelativePosition.Y = -10.0f;
            //mBowButton.RelativePosition.Z = -30.0f;

            mButtons[0] = mBowButton;

            //this.AttachTo(SpriteManager.Camera, false);
            //this.RelativePosition.Z = -30.0f;
        }

        public Button CheckButtonPressed(Vector3 pt)
        {
            foreach (Button b in mButtons)
            {
                //System.Diagnostics.Debug.WriteLine(Vector2.Distance(new Vector2(pt.X, pt.Y), new Vector2(b.X, b.Y)));

                if (b.Collision.IsPointInside(pt.X, pt.Y))
                {
                    return b;
                }
            }

            return null;
        }

        public virtual void Activity()
        {
            // This code should do things like set Animations, respond to input, and so on.
        }

        public virtual void Destroy()
        {
            // Remove self from the SpriteManager:
            SpriteManager.RemovePositionedObject(this);

            mBowButton.Destroy();
        }

        #endregion
    }
}
