using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Math.Geometry;

using Microsoft.Xna.Framework;

namespace Shroud.Utilities
{
    public static class GestureManager
    {
        public static Vector3 CurTouchWorld
        {
            get { return mCurTouchWorld; }
        }
        public static Vector3 mCurTouchWorld;

        public static Vector3 StartTouchWorld
        {
            get { return mStartTouchWorld; }
        }
        public static Vector3 mStartTouchWorld;

        public static Vector3 EndTouchWorld
        {
            get { return mEndTouchWorld; }
        }
        public static Vector3 mEndTouchWorld;

        public static Vector3 CurTouchUI
        {
            get { return mCurTouchUI; }
        }
        public static Vector3 mCurTouchUI;

        public static Vector3 StartTouchUI
        {
            get { return mStartTouchUI; }
        }
        public static Vector3 mStartTouchUI;

        public static Vector3 EndTouchUI
        {
            get { return mEndTouchUI; }
        }
        public static Vector3 mEndTouchUI;

        public static Line DragLine
        {
            get { return dragLine; }
        }
        private static Line dragLine;

        public enum InputState
        {
            None,
            Pushed,
            Down,
            Released
        };
        public static InputState CurInputState;

        [Flags]
        public enum Gesture
        {
            None = 0,
            Tap = 1,
            Swipe = 2,
            Swiping = 4,
            SwipeUp = 8,
            SwipeDown = 16,
            SwipeLeft = 32,
            SwipeRight = 64
        };
        public static Gesture CurGesture;

        private static float mMIN_SWIPE_LENGTH = 1.3f;

        // DEBUG VAR
        private static Circle Touch;

        public static void Initialize2()
        {
            mStartTouchWorld = new Vector3();
            mCurTouchWorld = new Vector3();
            mEndTouchWorld = new Vector3();

            mStartTouchUI = new Vector3();
            mCurTouchUI = new Vector3();
            mEndTouchUI = new Vector3();

            CurInputState = InputState.None;
            CurGesture = Gesture.None;

            Touch = ShapeManager.AddCircle();
            dragLine = ShapeManager.AddLine();
        }

        public static void Update2(float worldZ, float uiZ)
        {
            if (InputManager.TouchScreen.ScreenPushed)
            {
                CurInputState = InputState.Pushed;

                mStartTouchWorld.X = InputManager.TouchScreen.WorldXAt(worldZ);
                mStartTouchWorld.Y = InputManager.TouchScreen.WorldYAt(worldZ);

                mStartTouchUI.X = InputManager.TouchScreen.WorldXAt(uiZ);
                mStartTouchUI.Y = InputManager.TouchScreen.WorldYAt(uiZ);
            }
            else if (InputManager.TouchScreen.ScreenReleased)
            {
                CurInputState = InputState.Released;

                mEndTouchWorld.X = InputManager.TouchScreen.WorldXAt(worldZ);
                mEndTouchWorld.Y = InputManager.TouchScreen.WorldYAt(worldZ);

                mEndTouchUI.X = InputManager.TouchScreen.WorldXAt(uiZ);
                mEndTouchUI.Y = InputManager.TouchScreen.WorldYAt(uiZ);

                float touchLength = (StartTouchWorld - EndTouchWorld).Length();
                float xDiff = StartTouchWorld.X - EndTouchWorld.X;
                float yDiff = StartTouchWorld.Y - EndTouchWorld.Y;

                if (touchLength > mMIN_SWIPE_LENGTH)
                {
                    if (xDiff > mMIN_SWIPE_LENGTH)
                        CurGesture = Gesture.SwipeDown;
                    else if (xDiff < -mMIN_SWIPE_LENGTH)
                        CurGesture = Gesture.SwipeUp;
                    /*else if (yDiff > mMIN_SWIPE_LENGTH)
                        CurGesture = Gesture.SwipeLeft;
                    else if (yDiff < -mMIN_SWIPE_LENGTH)
                        CurGesture = Gesture.SwipeRight;*/
                    else
                        CurGesture = Gesture.Swipe;

                    dragLine.X = GestureManager.StartTouchWorld.X;
                    dragLine.Y = GestureManager.StartTouchWorld.Y;

                    dragLine.RelativePoint1.X = 0.0f;
                    dragLine.RelativePoint1.Y = 0.0f;

                    dragLine.RelativePoint2.X = GestureManager.EndTouchWorld.X - GestureManager.StartTouchWorld.X;
                    dragLine.RelativePoint2.Y = GestureManager.EndTouchWorld.Y - GestureManager.StartTouchWorld.Y;
                }
                else
                {
                    CurGesture = Gesture.Tap;
                }
            }
            else if (InputManager.TouchScreen.ScreenDown)
            {
                CurInputState = InputState.Down;

                CurGesture = Gesture.Swiping;

                Touch.X = mCurTouchWorld.X = InputManager.TouchScreen.WorldXAt(worldZ);
                Touch.Y = mCurTouchWorld.Y = InputManager.TouchScreen.WorldYAt(worldZ);

                mCurTouchUI.X = InputManager.TouchScreen.WorldXAt(uiZ);
                mCurTouchUI.Y = InputManager.TouchScreen.WorldYAt(uiZ);
            }
            else
            {
                CurInputState = InputState.None;

                CurGesture = Gesture.None;
            }
        }

        public static void Clean()
        {
            ShapeManager.Remove(Touch);
            ShapeManager.Remove(dragLine);
        }
    }
}
