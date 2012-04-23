using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using FlatRedBall;
using FlatRedBall.Math.Geometry;

using Shroud.Entities;

namespace Shroud.Utilities
{
    public static class StealthManager
    {
        /* Stealth Variables needed for Enemies:
         * -LOS limit
         * -AlertState
         * -
         */

        public static List<PositionedObject> ManagedStealthObjects;
        public static List<Vector3> StealthPoints;

        public enum AlertState
        {
            None,
            Cautious,
            Full
        };
        public static AlertState GlobalAlertState = AlertState.None;

        private static Line lineOfSight;

        public static void Initialize()
        {
            //lineOfSight = ShapeManager.AddLine();
        }

        /*public static bool IsVisibleTo(Enemy1 e, Player1 p)
        {
            return VisibilityHelper(e, p);
        }

        public static bool IsVisibleTo(Enemy1 e, Trap t)
        {
            return VisibilityHelper(e, t);
        }

        public static bool IsVisibleTo(Enemy1 e, Enemy1 downed)
        {
            return VisibilityHelper(e, downed);
        }

        private static bool VisibilityHelper(Enemy1 e, PositionedObject po)
        {
            Vector3 los = e.Position - po.Position;

            if (los.Length() < e.MaxLOS)
            {
                if (IsClearLOS(e.Position, po.Position))
                {
                    float stealthRating = GetStealthRating(po);

                    if (stealthRating > e.DetectRating)
                    {
                        return true;
                    }
                }
            }

            return false;
        }*/

        private static float GetStealthRating(PositionedObject po)
        {
            Vector3 closestsStealthPoint = StealthPoints[0];
            float dist = Vector3.Distance(closestsStealthPoint, po.Position);
            float newDist;

            foreach (Vector3 pt in StealthPoints)
            {
                newDist = Vector3.Distance(pt, po.Position);

                if (newDist < dist)
                {
                    dist = newDist;
                    closestsStealthPoint = pt;
                }
            }

            return dist;
        }

        private static bool IsClearLOS(Vector3 from, Vector3 to)
        {
            lineOfSight.Position = from;
            lineOfSight.RelativePoint1.X = 0.0f;

            return false;
        }
    }
}
