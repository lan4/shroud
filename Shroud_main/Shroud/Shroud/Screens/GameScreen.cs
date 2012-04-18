using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shroud.Entities;
using Shroud.Utilities;
using Scene = Shroud.Utilities.Scene;

using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Input;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Instructions;

namespace Shroud.Screens
{
    public class GameScreen : Screen
    {
        private PauseButton mPause;
        private bool mIsPaused;

        private class PauseButton : PositionedObject
        {
            private Sprite mbg;
            private AxisAlignedRectangle mCollision;

            public AxisAlignedRectangle Collision
            {
                get { return mCollision; }
            }

            public PauseButton(string contentManagerName)
            {
                SpriteManager.AddPositionedObject(this);

                mbg = SpriteManager.AddSprite(@"Content/Menus/pause_btn", contentManagerName, CameraManager.UI);
                mbg.AttachTo(this, false);
                GameProperties.RescaleSprite(mbg);
                mbg.RelativeRotationZ = GameProperties.WorldRotation;

                mCollision = ShapeManager.AddAxisAlignedRectangle();
                mCollision.AttachTo(this, false);
                mCollision.ScaleX = mbg.ScaleY;
                mCollision.ScaleY = mbg.ScaleX;
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

        public GameScreen() : base("GameScreen")
        {
            // Don't put initialization code here, do it in
            // the Initialize method below
            //   |   |   |   |   |   |   |   |   |   |   |
            //   |   |   |   |   |   |   |   |   |   |   |
            //   V   V   V   V   V   V   V   V   V   V   V

        }

        public override void Initialize(bool addToManagers)
        {
            InitializeManagers();

            SpriteManager.Camera.BackgroundColor = Color.CadetBlue;

            LevelManager.Load(GameProperties.LevelString);

            mPause = new PauseButton(ContentManagerName);
            mPause.X = 8.0f;
            mPause.Y = -14.0f;
            //mPause.Z = HUDManager.zUI - SpriteManager.Camera.Z;
            mIsPaused = false;

            //SpriteManager.Camera.DrawsShapes = false;

			// AddToManagers should be called LAST in this method:
			if(addToManagers)
			{
				AddToManagers();
			}
        }

		public override void AddToManagers()
        {
		    // Nothing to Do Here
		
		}
		
        #endregion

        private void InitializeManagers()
        {
            WorldManager.Initialize();
            GestureManager.Initialize2();
        }

        #region Public Methods

        public override void Activity(bool firstTimeCalled)
        {
            base.Activity(firstTimeCalled);

            if (!mIsPaused)
            {
                GestureManager.Update2(WorldManager.PlayerInstance.Z, HUDManager.zUI - SpriteManager.Camera.Z);

                mPause.Z = WorldManager.PlayerInstance.Z;

                if (GestureManager.CurGesture == GestureManager.Gesture.Tap)
                {
                    if (mPause.Collision.IsPointInside(GestureManager.EndTouchWorld.X, GestureManager.EndTouchWorld.Y) && !mIsPaused)
                    {
                        GameProperties.IsPaused = true;
                        mIsPaused = true;
                        InstructionManager.PauseEngine();
                        LoadPopup(typeof(PauseScreen).FullName, CameraManager.Pause);
                    }
                }

                if (!mIsPaused)
                {
                    WorldManager.Update();
                    CameraManager.UpdateCamera2();
                    //HUDManager.Update();
                }
            }

            if (!GameProperties.IsPaused && mIsPaused)
            {
                InstructionManager.UnpauseEngine();
                mIsPaused = false;
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            WorldManager.Destroy();
        }

        #endregion

		
        #endregion
    }
}

