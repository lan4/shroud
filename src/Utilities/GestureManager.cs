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
        public static Vector3 DragStart;
        public static Vector3 DragEnd;

        private static Vector3 CameraStart;
        private static Vector3 CameraEnd;

        public static Vector3 TapPoint;

        public static Vector3 CurTouchPoint;

        public static Vector3 TouchAtZ;

        public enum InputState
        {
            None,
            Pushed,
            Down,
            Released
        };

        public static InputState CurInputState;

        public enum Gesture
        {
            None,
            Drag,
            Tap
        };

        public static Gesture CurGesture;

        public static Circle TouchCollision;

        private static float mDragTolerance = 0.3f;

        public static void Initialize()
        {
            DragStart = new Vector3();
            DragEnd = new Vector3();

            CameraStart = new Vector3();
            CameraEnd = new Vector3();

            TapPoint = new Vector3();

            CurTouchPoint = new Vector3();

            TouchAtZ = new Vector3();

            TouchCollision = ShapeManager.AddCircle();
            TouchCollision.Radius = 0.3f;

            CurInputState = InputState.None;
        }

        public static void GetPointAtZ(float zAt)
        {
            TouchAtZ.X = InputManager.TouchScreen.WorldXAt(zAt);
            TouchAtZ.Y = InputManager.TouchScreen.WorldYAt(zAt);
        }

        public static void Update(float zAt)
        {
            if (InputManager.TouchScreen.ScreenPushed)
            {
                CurInputState = InputState.Pushed;

                DragStart.X = TouchCollision.X = InputManager.TouchScreen.WorldXAt(zAt);
                DragStart.Y = TouchCollision.Y = InputManager.TouchScreen.WorldYAt(zAt);
 
                CameraStart.X = SpriteManager.Camera.X;
                CameraStart.Y = SpriteManager.Camera.Y;
            }
            else if (InputManager.TouchScreen.ScreenReleased)
            {
                CurInputState = InputState.Released;

                DragEnd.X = TapPoint.X = TouchCollision.X = InputManager.TouchScreen.WorldXAt(zAt);
                DragEnd.Y = TapPoint.Y = TouchCollision.Y = InputManager.TouchScreen.WorldYAt(zAt);

                CameraEnd.X = SpriteManager.Camera.X;
                CameraEnd.Y = SpriteManager.Camera.Y;

                if ((DragStart - DragEnd).Length() > mDragTolerance ||
                    (CameraStart - CameraEnd).Length() > mDragTolerance)
                {
                    CurGesture = Gesture.Drag;
                }
                else
                {
                    CurGesture = Gesture.Tap;
                }
            }
            else if (InputManager.TouchScreen.ScreenDown)
            {
                CurInputState = InputState.Down;

                CurGesture = Gesture.None;

                CurTouchPoint.X = TouchCollision.X = InputManager.TouchScreen.WorldXAt(zAt);
                CurTouchPoint.Y = TouchCollision.Y = InputManager.TouchScreen.WorldYAt(zAt);
            }
            else
            {
                CurInputState = InputState.None;

                CurGesture = Gesture.None;
            }
        }
    }
}
