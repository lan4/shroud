using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall;

namespace Shroud.Utilities
{
    public static class GameProperties
    {
        // Global Enemy Vars
        public static float EnemyMoveSpeed = 5.0f;
        public static float EnemyNodeTolerance = 0.3f;
        public static float WorldRotation = (float)Math.PI*3.0f/2.0f;

        public static void RescaleSprite(Sprite s)
        {
            float pixelsPerUnit = SpriteManager.Camera.PixelsPerUnitAt(s.Z);
            s.ScaleX = .5f * s.Texture.Width / pixelsPerUnit;
            s.ScaleY = .5f * s.Texture.Height / pixelsPerUnit;
        }
    }
}
