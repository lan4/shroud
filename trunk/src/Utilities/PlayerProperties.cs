using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shroud.Utilities
{
    public static class PlayerProperties
    {
        // Global Bow Vars
        public static float MinDrawLength = 2.0f;
        public static float MaxDrawLength = 20.0f;

        // Global Move Vars
        public static float MoveSpeed = 10.0f;
        public static float MoveTolerance = 0.3f;
        public static float ChaseTolerance = 2.0f;

        // Global Attack Vars
        public static float WeaponSize = 0.3f;
        public static float WeaponRange = 0.5f;

        // Global Trap Vars
        

        // Global Camera Vars
        public static float MinDragLength = 3.0f;
        public static float MaxCameraXFromPlayer = 8.0f;
        public static float MaxCameraYFromPlayer = 15.0f;
        public static float CameraMoveSpeed = 8.0f;
        public static float CameraPosTolerance = 0.1f;
    }
}
