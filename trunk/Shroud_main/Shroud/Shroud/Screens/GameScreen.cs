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
        private bool mGameOver;
        private bool mWon;

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

            mGameOver = false;
            mWon = false;

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
            CameraManager.Initialize();
            WorldManager.Initialize();
            GestureManager.Initialize2();

            GameProperties.JumpBack = false;
        }

        #region Public Methods

        public override void Activity(bool firstTimeCalled)
        {
            base.Activity(firstTimeCalled);

            mPause.X = LevelManager.CurrentScene.WorldAnchor.X - 8.0f;
            mPause.Y = LevelManager.CurrentScene.WorldAnchor.Y - 14.0f;

            if (GameProperties.JumpBack)
            {
                SpriteManager.Camera.X = 0.0f;
                SpriteManager.Camera.Y = 0.0f;
                GameProperties.JumpBack = false;
                GameProperties.IsPaused = false;
                MoveToScreen(typeof(LevelScreen).FullName);
            }

            if (!mIsPaused)
            {
                GestureManager.Update2(WorldManager.PlayerInstance.Z, HUDManager.zUI - SpriteManager.Camera.Z);

                //mPause.Z = WorldManager.PlayerInstance.Z;

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
                }
            }

            if (!GameProperties.IsPaused && mIsPaused)
            {
                InstructionManager.UnpauseEngine();
                mIsPaused = false;
            }

            mGameOver = WorldManager.PlayerInstance.IsReallyDead || WorldManager.Target.IsReallyDead;

            if (mGameOver)
            {
                GameProperties.PlayerAlive = WorldManager.PlayerInstance.IsAlive;

                if (!WorldManager.PlayerInstance.IsAlive)
                {
                    //Destroy();
                    //System.Diagnostics.Debug.WriteLine(SpriteManager.AutomaticallyUpdatedSprites[0]);
                    //SpriteManager.Camera.X = 0.0f;
                    //SpriteManager.Camera.Y = 0.0f;
                    //mIsPaused = true;
                    //GameProperties.IsPaused = true;
                    //LoadPopup(typeof(WinScreen).FullName, CameraManager.Pause);
                    //MoveToScreen(typeof(WinScreen).FullName);
                }
                else
                {
                    /*GameProperties.IsPaused = true;
                    mIsPaused = true;
                    InstructionManager.PauseEngine();
                    LoadPopup(typeof(WinScreen).FullName, CameraManager.Pause);*/

                    string lvl = GameProperties.LevelToken;
                    bool changed = false;
                    
                    if (!lvl.Contains('d') && mIsPaused == false)
                    {
                        lvl = lvl + "d";
                        changed = true;

                        GameProperties.ProfileString = GameProperties.ProfileString.Replace("p" + GameProperties.NumLevels.ToString(), "p" + (GameProperties.NumLevels + 1).ToString());
                        GameProperties.NumLevels++;

                        if (GameProperties.NumLevels < GameProperties.TotalLevels)
                        {
                            GameProperties.ProfileString += " l" + (GameProperties.NumLevels + 1).ToString();
                        }
                    }

                    if (!lvl.Contains('a') && GameProperties.HiddenBadge)
                    {
                        lvl = lvl + "a";
                        changed = true;
                    }

                    if (!lvl.Contains('b') && GameProperties.OneKillBadge)
                    {
                        lvl = lvl + "b";
                        changed = true;
                    }

                    if (!lvl.Contains('c') && GameProperties.NoDieBadge)
                    {
                        lvl = lvl + "c";
                        changed = true;
                    }

                    if (!lvl.Equals(GameProperties.LevelToken) || changed)
                    {
                        GameProperties.ProfileString = GameProperties.ProfileString.Replace(GameProperties.LevelToken, lvl);
                        //System.Diagnostics.Debug.WriteLine(GameProperties.ProfileString);
                        GameProperties.Save();
                        GameProperties.LevelToken = lvl;
                    }

                    //Destroy();
                    SpriteManager.Camera.X = 0.0f;
                    SpriteManager.Camera.Y = 0.0f;
                    mIsPaused = true;
                    GameProperties.IsPaused = true;
                    //LoadPopup(typeof(WinScreen).FullName, CameraManager.Pause);
                    MoveToScreen(typeof(WinScreen).FullName);
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            mPause.Destroy();

            WorldManager.Destroy();
            GestureManager.Clean();
            LevelManager.Clean();
            SpriteManager.Camera.RemoveLayer(CameraManager.Background);
            SpriteManager.Camera.RemoveLayer(CameraManager.Entity1);
            SpriteManager.Camera.RemoveLayer(CameraManager.Entity2);
            SpriteManager.Camera.RemoveLayer(CameraManager.Middleground);
            SpriteManager.Camera.RemoveLayer(CameraManager.Foreground);
            SpriteManager.Camera.RemoveLayer(CameraManager.Pause);
            SpriteManager.Camera.RemoveLayer(CameraManager.UI);
            //SpriteManager.Camera.RemoveLayer(CameraManager.Background);
        }

        #endregion

		
        #endregion
    }
}

