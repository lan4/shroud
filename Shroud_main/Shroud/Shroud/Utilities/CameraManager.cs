using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall;

using Gesture = Shroud.Utilities.GestureManager.Gesture;

namespace Shroud.Utilities
{
    public static class CameraManager
    {
        private static float XTolerance = 5.0f;
        private static float YTolerance = 2.5f;
        private static Scene mCurBlock;
        private static Sprite mTransitionCover;

        public static void Initialize(Scene b)
        {
            //mCurBlock = b;
            //mTransitionCover = SpriteManager.AddSprite(@"Content/Entities/fuck", "Global");
            //mTransitionCover.AttachTo(SpriteManager.Camera, false);
            //mTransitionCover.Visible = false;
        }

        public static void UpdateCamera()
        {
            if (Math.Abs(SpriteManager.Camera.Position.Y - WorldManager.PlayerInstance.Position.Y) > XTolerance)
            {
                float xDiff = SpriteManager.Camera.Y - WorldManager.PlayerInstance.Y;


                if (xDiff > 0)
                {
                    SpriteManager.Camera.Velocity.Y = -PlayerProperties.CameraMoveSpeed;
                }
                else
                {
                    SpriteManager.Camera.Velocity.Y = PlayerProperties.CameraMoveSpeed;
                }
            }
            else
            {
                SpriteManager.Camera.Velocity.Y = 0.0f;
            }

            if (Math.Abs(SpriteManager.Camera.Position.X - WorldManager.PlayerInstance.Position.X) > YTolerance)
            {
                float yDiff = SpriteManager.Camera.X - WorldManager.PlayerInstance.X;


                if (yDiff > 0)
                {
                    SpriteManager.Camera.Velocity.X = -PlayerProperties.CameraMoveSpeed;
                }
                else
                {
                    SpriteManager.Camera.Velocity.X = PlayerProperties.CameraMoveSpeed;
                }
            }
            else
            {
                SpriteManager.Camera.Velocity.X = 0.0f;
            }
        }

        public static void UpdateCamera2()
        {
            /*switch (GestureManager.CurGesture)
            {
                case Gesture.SwipeUp:
                    PeekUp();
                    break;
                case Gesture.SwipeDown:
                    PeekDown();
                    break;
                case Gesture.SwipeRight:
                    PeekRight();
                    break;
                case Gesture.SwipeLeft:
                    PeekLeft();
                    break;
            }*/

            SpriteManager.Camera.X = LevelManager.CurrentScene.WorldAnchor.X;
            SpriteManager.Camera.Y = LevelManager.CurrentScene.WorldAnchor.Y;
        }

        #region Helper Functions

        private static void PeekLeft()
        {
            if (mCurBlock.Left != null)
            {
                mCurBlock = mCurBlock.Left;
                SpriteManager.Camera.X = mCurBlock.WorldAnchor.X;
                SpriteManager.Camera.Y = mCurBlock.WorldAnchor.Y;
            }
        }

        private static void PeekRight()
        {
            if (mCurBlock.Right != null)
            {
                mCurBlock = mCurBlock.Right;
                SpriteManager.Camera.X = mCurBlock.WorldAnchor.X;
                SpriteManager.Camera.Y = mCurBlock.WorldAnchor.Y;
            }
        }

        private static void PeekUp()
        {
            if (mCurBlock.Up != null)
            {
                mCurBlock = mCurBlock.Up;
                SpriteManager.Camera.X = mCurBlock.WorldAnchor.X;
                SpriteManager.Camera.Y = mCurBlock.WorldAnchor.Y;
            }
        }

        private static void PeekDown()
        {
            if (mCurBlock.Down != null)
            {
                mCurBlock = mCurBlock.Down;
                SpriteManager.Camera.X = mCurBlock.WorldAnchor.X;
                SpriteManager.Camera.Y = mCurBlock.WorldAnchor.Y;
            }
        }

        #endregion
    }
}
