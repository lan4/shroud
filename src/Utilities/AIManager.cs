using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shroud.Entities;

using Microsoft.Xna.Framework;

namespace Shroud.Utilities
{
    public static class AIManager
    {
        public static List<Enemy> ManagedEnemies;
        public static Player PlayerRef;
        public static Vector3 PlayerPosition;

        public enum AlertState
        {
            None,
            Low,
            Medium,
            High
        };

        private static AlertState mCurAlert;

        private static float mVisibilityLimit = 15.0f;
        private static float mAttackRange = 1.5f;

        public static void Initialize()
        {
            mCurAlert = AlertState.None;

            ManagedEnemies = new List<Enemy>();

            PlayerPosition = new Vector3();
        }

        public static void Update()
        {
            float distBetween = 11.0f;

            if (!PlayerRef.IsHiding && PlayerRef.Alive)
            {
                foreach (Enemy e in ManagedEnemies)
                {
                    if (e.Alive)
                    {
                        distBetween = Vector3.Distance(e.Position, PlayerPosition);

                        if (distBetween < mAttackRange)
                        {
                            e.Attack();
                        }
                        else if (distBetween < mVisibilityLimit)
                        {
                            e.MoveTo(PlayerPosition);
                        }

                        if (e.CheckHit(PlayerRef))
                        {
                            PlayerRef.Die();
                        }
                    }
                }
            }
        }
    }
}
