using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;

using Shroud.Utilities;

namespace Shroud.Screens
{
    public class PauseScreen : Screen
    {
        private Sprite mBg;
        private Sprite mCtrls;
        private ClickButton mContinue;
        private ClickButton mControls;
        private ClickButton mExit;
        private ClickButton mLevelSelect;

        private class ClickButton : PositionedObject
        {
            private Sprite mbg;
            private AxisAlignedRectangle mCollision;

            public AxisAlignedRectangle Collision
            {
                get { return mCollision; }
            }

            public ClickButton(string contentManagerName, string id, Layer l)
                : base()
            {
                SpriteManager.AddPositionedObject(this);

                mbg = SpriteManager.AddSprite(@"Content/Menus/Pause/" + id, contentManagerName, l);
                mbg.AttachTo(this, false);
                GameProperties.RescaleSprite(mbg);
                mbg.RelativeRotationZ = GameProperties.WorldRotation;

                mCollision = ShapeManager.AddAxisAlignedRectangle();
                mCollision.ScaleX = mbg.ScaleY;
                mCollision.ScaleY = mbg.ScaleX;
                mCollision.AttachTo(this, false);
            }

            public void Destroy()
            {
                SpriteManager.RemovePositionedObject(this);
                SpriteManager.RemoveSprite(mbg);
                ShapeManager.Remove(mCollision);
            }
        }


        #region Methods

        #region Constructor and Initialize

        public PauseScreen()
            : base("PauseScreen")
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

            mBg = SpriteManager.AddSprite(@"Content/Menus/Pause/pauseScreen", ContentManagerName, this.Layer);
            GameProperties.RescaleSprite(mBg);
            mBg.X = LevelManager.CurrentScene.WorldAnchor.X;
            mBg.Y = LevelManager.CurrentScene.WorldAnchor.Y;
            mBg.RotationZ = GameProperties.WorldRotation;

            mContinue = new ClickButton(ContentManagerName, "continue", this.Layer);
            mControls = new ClickButton(ContentManagerName, "controls", this.Layer);
            mExit = new ClickButton(ContentManagerName, "quit", this.Layer);
            mLevelSelect = new ClickButton(ContentManagerName, "lvlSelect", this.Layer);

            mCtrls = SpriteManager.AddSprite(@"Content/Menus/Pause/controls_screen", ContentManagerName, this.Layer);
            GameProperties.RescaleSprite(mCtrls);
            mCtrls.RotationZ = GameProperties.WorldRotation;
            mCtrls.X = LevelManager.CurrentScene.WorldAnchor.X;
            mCtrls.Y = LevelManager.CurrentScene.WorldAnchor.Y;
            mCtrls.Visible = false;

            mContinue.X = 1.0f + LevelManager.CurrentScene.WorldAnchor.X;
            mContinue.Y = 8.0f + LevelManager.CurrentScene.WorldAnchor.Y;
            //mContinue.Z = 0.01f;

            mControls.X = -2.0f + LevelManager.CurrentScene.WorldAnchor.X;
            mControls.Y = 8.0f + LevelManager.CurrentScene.WorldAnchor.Y;
            //mControls.Z = 0.01f;

            mLevelSelect.X = -5.0f + LevelManager.CurrentScene.WorldAnchor.X;
            mLevelSelect.Y = 8.0f + LevelManager.CurrentScene.WorldAnchor.Y;

            mExit.X = -8.0f + LevelManager.CurrentScene.WorldAnchor.X;
            mExit.Y = 8.0f + LevelManager.CurrentScene.WorldAnchor.Y;
            //mExit.Z = 0.01f;

            
			
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
            base.Activity(firstTimeCalled);

            GestureManager.Update2(0.0f, 0.0f);

            if (GestureManager.CurGesture == GestureManager.Gesture.Tap)
            {
                float x = GestureManager.EndTouchWorld.X;
                float y = GestureManager.EndTouchWorld.Y;

                if (mCtrls.Visible)
                {
                    mCtrls.Visible = false;
                }
                else
                {
                    if (mContinue.Collision.IsPointInside(x, y))
                    {
                        Destroy();
                        GameProperties.IsPaused = false;
                        IsActivityFinished = true;
                    }
                    else if (mExit.Collision.IsPointInside(x, y))
                    {
                        GameProperties.Quit();
                    }
                    else if (mControls.Collision.IsPointInside(x, y))
                    {
                        mCtrls.Visible = true;
                    }
                    else if (mLevelSelect.Collision.IsPointInside(x, y))
                    {
                        Destroy();
                        GameProperties.IsPaused = false;
                        GameProperties.JumpBack = true;
                        IsActivityFinished = true;
                    }
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            mContinue.Destroy();
            mControls.Destroy();
            mExit.Destroy();
            mLevelSelect.Destroy();

            SpriteManager.RemoveSprite(mBg);
            SpriteManager.RemoveSprite(mCtrls);
        }

        #endregion

		
        #endregion
    }
}

