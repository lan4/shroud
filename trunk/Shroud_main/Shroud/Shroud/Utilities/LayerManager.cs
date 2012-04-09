using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shroud.Utilities
{
    public static class LayerManager
    {
        public enum MainLayer
        {
            Foreground,
            Entity1,
            Middleground,
            Entity2,
            Background
        };

        public enum DetailLayer
        {
            Front,
            Middle,
            Back
        };

        public static MainLayer GetMainLayer(float z)
        {
            if (z < -1.5f)
                return MainLayer.Foreground;
            else if (z < -0.5f)
                return MainLayer.Entity1;
            else if (z < 0.5f)
                return MainLayer.Middleground;
            else if (z < 1.5f)
                return MainLayer.Entity2;
            else
                return MainLayer.Background;
        }

        public static DetailLayer GetDetailLayer(float z)
        {
            float subz = Math.Abs(z) - (float)Math.Abs(Math.Round(z));

            if (subz < -0.05f)
                return DetailLayer.Front;
            else if (subz < 0.05f)
                return DetailLayer.Middle;
            else
                return DetailLayer.Back;
        }

        public static float SetLayer(MainLayer m, DetailLayer d)
        {
            return GetMainLayerValue(m) + GetDetailLayerValue(d);
        }

        private static float GetMainLayerValue(MainLayer m)
        {
            switch (m)
            {
                case MainLayer.Foreground:
                    return -2.0f;
                case MainLayer.Entity1:
                    return -1.0f;
                case MainLayer.Middleground:
                    return 0.0f;
                case MainLayer.Entity2:
                    return 1.0f;
                case MainLayer.Background:
                    return 2.0f;
                default:
                    return 123456.0f;
            }
        }

        private static float GetDetailLayerValue(DetailLayer d)
        {
            switch (d)
            {
                case DetailLayer.Front:
                    return -0.1f;
                case DetailLayer.Middle:
                    return 0.0f;
                case DetailLayer.Back:
                    return 0.1f;
                default:
                    return 123456.0f;
            }
        }
    }
}
