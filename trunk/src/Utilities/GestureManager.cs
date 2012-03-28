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
        public static Vector3 CurTouchWorld;
        public static Vector3 StartTouchWorld;
        public static Vector3 EndTouchWorld;

        public static Vector3 CurTouchUI;
        public static Vector3 StartTouchUI;
        public static Vector3 EndTouchUI;

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

        private static float mMIN_SWIPE_LENGTH = 0.8f;

        // DEBUG VAR
        private static Circle Touch;

        public static void Initialize2()
        {
            StartTouchWorld = new Vector3();
            CurTouchWorld = new Vector3();
            EndTouchWorld = new Vector3();

            StartTouchUI = new Vector3();
            CurTouchUI = new Vector3();
            EndTouchUI = new Vector3();

            CurInputState = InputState.None;
            CurGesture = Gesture.None;

            Touch = ShapeManager.AddCircle();
        }

        public static void Update2(float worldZ, float uiZ)
        {
            if (InputManager.TouchScreen.ScreenPushed)
            {
                CurInputState = InputState.Pushed;

                StartTouchWorld.X = InputManager.TouchScreen.WorldXAt(worldZ);
                StartTouchWorld.Y = InputManager.TouchScreen.WorldYAt(worldZ);

                StartTouchUI.X = InputManager.TouchScreen.WorldXAt(uiZ);
                StartTouchUI.Y = InputManager.TouchScreen.WorldYAt(uiZ);
            }
            else if (InputManager.TouchScreen.ScreenReleased)
            {
                CurInputState = InputState.Released;

                EndTouchWorld.X = InputManager.TouchScreen.WorldXAt(worldZ);
                EndTouchWorld.Y = InputManager.TouchScreen.WorldYAt(worldZ);

                EndTouchUI.X = InputManager.TouchScreen.WorldXAt(uiZ);
                EndTouchUI.Y = InputManager.TouchScreen.WorldYAt(uiZ);

                float touchLength = (StartTouchWorld - EndTouchWorld).Length();
                float xDiff = StartTouchWorld.X - EndTouchWorld.X;
                float yDiff = StartTouchWorld.Y - EndTouchWorld.Y;

                if (touchLength > mMIN_SWIPE_LENGTH)
                {
                    if (xDiff > mMIN_SWIPE_LENGTH)
                        CurGesture = Gesture.SwipeDown;
                    else if (xDiff < -mMIN_SWIPE_LENGTH)
                        CurGesture = Gesture.SwipeUp;
                    else if (yDiff > mMIN_SWIPE_LENGTH)
                        CurGesture = Gesture.SwipeLeft;
                    else if (yDiff < -mMIN_SWIPE_LENGTH)
                        CurGesture = Gesture.SwipeRight;
                    else
                        CurGesture = Gesture.Swipe;
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

                Touch.X = CurTouchWorld.X = InputManager.TouchScreen.WorldXAt(worldZ);
                Touch.Y = CurTouchWorld.Y = InputManager.TouchScreen.WorldYAt(worldZ);

                CurTouchUI.X = InputManager.TouchScreen.WorldXAt(uiZ);
                CurTouchUI.Y = InputManager.TouchScreen.WorldYAt(uiZ);
            }
            else
            {
                CurInputState = InputState.None;

                CurGesture = Gesture.None;
            }
        }
    }
}
