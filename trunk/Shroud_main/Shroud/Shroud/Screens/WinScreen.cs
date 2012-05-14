using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using FlatRedBall;
using FlatRedBall.Math.Geometry;

using Shroud.Utilities;

namespace Shroud.Screens
{
    public class WinScreen : Screen
    {
        private Sprite mWin;
        private Sprite mBadgeH;
        private Sprite mBadgeO;
        private Sprite mBadgeN;
        private Sprite mNoBadge;
        private BackButton mBack;

        private class BackButton : PositionedObject
        {
            private AxisAlignedRectangle mCollision;
            private Sprite mButton;

            public AxisAlignedRectangle Collision
            {
                get { return mCollision; }
            }

            public BackButton(string contentManagerName)
                : base()
            {
                SpriteManager.AddPositionedObject(this);

                mButton = SpriteManager.AddSprite(@"Content/Menus/goBack", contentManagerName);
                mButton.AttachTo(this, false);
                GameProperties.RescaleSprite(mButton);
                mButton.RelativeRotationZ = GameProperties.WorldRotation;

                mCollision = ShapeManager.AddAxisAlignedRectangle();
                mCollision.ScaleX = mButton.ScaleY;
                mCollision.ScaleY = mButton.ScaleX;
                mCollision.AttachTo(this, false);
            }

            public void Destroy()
            {
                SpriteManager.RemovePositionedObject(this);
                SpriteManager.RemoveSprite(mButton);
                ShapeManager.Remove(mCollision);
            }
        }

        #region Methods

        #region Constructor and Initialize

        public WinScreen()
            : base("WinScreen")
        {
            // Don't put initialization code here, do it in
            // the Initialize method below
            //   |   |   |   |   |   |   |   |   |   |   |
            //   |   |   |   |   |   |   |   |   |   |   |
            //   V   V   V   V   V   V   V   V   V   V   V

        }

        public override void Initialize(bool addToManagers)
        {
            // Set the screen up here instead of in the Constructor to avoid
            // exceptions occurring during the constructor.
            string sname;
            if (GameProperties.PlayerAlive)
            {
                sname = "mission_complete";

                bool one = true;

                if (GameProperties.HiddenBadge)
                {
                    mBadgeH = SpriteManager.AddSprite(@"Content/Menus/Win/achievements0", ContentManagerName);
                    GameProperties.RescaleSprite(mBadgeH);
                    mBadgeH.RotationZ = GameProperties.WorldRotation;
                    mBadgeH.X = SpriteManager.Camera.X - 2.0f;
                    mBadgeH.Y = SpriteManager.Camera.Y + 10.0f;
                    mBadgeH.Z = 0.1f;

                    one = false;
                }

                if (GameProperties.OneKillBadge)
                {
                    mBadgeO = SpriteManager.AddSprite(@"Content/Menus/Win/achievements1", ContentManagerName);
                    GameProperties.RescaleSprite(mBadgeO);
                    mBadgeO.RotationZ = GameProperties.WorldRotation;
                    mBadgeO.X = SpriteManager.Camera.X - 2.0f;
                    mBadgeO.Y = SpriteManager.Camera.Y;
                    mBadgeO.Z = 0.1f;

                    one = false;
                }

                if (GameProperties.NoDieBadge)
                {
                    mBadgeN = SpriteManager.AddSprite(@"Content/Menus/Win/achievements2", ContentManagerName);
                    GameProperties.RescaleSprite(mBadgeN);
                    mBadgeN.RotationZ = GameProperties.WorldRotation;
                    mBadgeN.X = SpriteManager.Camera.X - 2.0f;
                    mBadgeN.Y = SpriteManager.Camera.Y - 10.0f;
                    mBadgeN.Z = 0.1f;
                    
                    one = false;
                }

                if (one)
                {
                    mNoBadge = SpriteManager.AddSprite(@"Content/Menus/Win/achievements3", ContentManagerName);
                    GameProperties.RescaleSprite(mNoBadge);
                    mNoBadge.RotationZ = GameProperties.WorldRotation;
                    mNoBadge.X = /*SpriteManager.Camera.X*/ - 2.0f;
                    //mNoBadge.Y = SpriteManager.Camera.Y;
                    mNoBadge.Z = 0.1f;
                }
            }
            else
            {
                sname = "mission_failed";
            }

            mWin = SpriteManager.AddSprite(@"Content/Menus/" + sname, ContentManagerName);
            GameProperties.RescaleSprite(mWin);
            mWin.RotationZ = GameProperties.WorldRotation;

            //mWin.X = SpriteManager.Camera.X;
            //mWin.Y = SpriteManager.Camera.Y;

            mBack = new BackButton(ContentManagerName);
            mBack.X = -8.0f;
            mBack.Y = 11.0f;

            GestureManager.Initialize2();

			// AddToManagers should be called LAST in this method:
			if(addToManagers)
			{
				AddToManagers();
			}
        }

		public override void AddToManagers()
        {
		
		
		}
		
        #endregion

        #region Public Methods

        public override void Activity(bool firstTimeCalled)
        {
            if (!firstTimeCalled)
            {
                base.Activity(firstTimeCalled);

                GestureManager.Update2(0.0f, 0.0f);

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    //IsActivityFinished = true;
                    GameProperties.IsPaused = false;
                    GameProperties.JumpBack = false;
                    MoveToScreen(typeof(LevelScreen).FullName);
                }

                if (GestureManager.CurGesture == GestureManager.Gesture.Tap && !firstTimeCalled)
                {
                    float x = GestureManager.EndTouchWorld.X;
                    float y = GestureManager.EndTouchWorld.Y;

                    if (mBack.Collision.IsPointInside(x, y))
                    {
                        //IsActivityFinished = true;
                        GameProperties.IsPaused = false;
                        GameProperties.JumpBack = false;
                        MoveToScreen(typeof(LevelScreen).FullName);
                    }
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            SpriteManager.RemoveSprite(mWin);
            SpriteManager.RemoveSprite(mBadgeH);
            SpriteManager.RemoveSprite(mBadgeO);
            SpriteManager.RemoveSprite(mBadgeN);
            SpriteManager.RemoveSprite(mNoBadge);
            mBack.Destroy();

            GestureManager.Clean();
        }

        #endregion

		
        #endregion
    }
}

