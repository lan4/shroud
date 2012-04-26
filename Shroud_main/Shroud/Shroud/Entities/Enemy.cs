using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall;

using Shroud.Utilities;

namespace Shroud.Entities
{
    public abstract class Enemy : AttackingEntity
    {
        private float mMinDetect;

        public Enemy(string contentManagerName, List<Node> patrols, float speed, float size, float range)
            : base(contentManagerName, patrols, speed, size, range)
        {
            // NOTHING HERE?

            mMinDetect = 1.0f;
        }

        public abstract void Die();

        public bool IsPlayerVisible()
        {
            if (Math.Abs(WorldManager.PlayerInstance.Position.X - this.Position.X) < 1.0f && Math.Abs(WorldManager.PlayerInstance.Position.Y - this.Position.Y) < 3.0f)
            {
                float yDiff = WorldManager.PlayerInstance.Position.Y - this.Position.Y;

                return ((yDiff < 0 && mFacingRight) || (yDiff > 0 && !mFacingRight)) && WorldManager.PlayerInstance.Opacity > mMinDetect;
            }

            return false;
        }

    }
}
