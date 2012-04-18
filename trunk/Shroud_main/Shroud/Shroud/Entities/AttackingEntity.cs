using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall;
using FlatRedBall.Math.Geometry;

using Shroud.Utilities;

namespace Shroud.Entities
{
    public abstract class AttackingEntity : MovingEntity
    {
        protected Circle mAttackCollision;
        protected float mAttackRange;

        protected AttackingEntity(string contentManagerName, float size, float range) : base(contentManagerName)
        {
            mAttackCollision = ShapeManager.AddCircle();
            mAttackCollision.AttachTo(this, false);
            mAttackCollision.Radius = size;
            mAttackRange = range;
        }

        protected AttackingEntity(string contentManagerName, List<Node> patrol, float size, float range)
            : base(contentManagerName, patrol)
        {
            mAttackCollision = ShapeManager.AddCircle();
            mAttackCollision.AttachTo(this, false);
            mAttackCollision.Radius = size;
            mAttackRange = range;
        }

        protected void Attack()
        {
            // RelativeY used because screen is sideways in game
            if (mFacingRight)
                mAttackCollision.RelativeY = mAttackRange;
            else
                mAttackCollision.RelativeY = -mAttackRange;
        }

        protected void ResetAttack()
        {
            // RelativeY used because screen is sideways in game
            mAttackCollision.RelativeY = 0.0f;
        }

        public virtual void Destroy()
        {
            base.Destroy();

            ShapeManager.Remove(mAttackCollision);
        }
    }
}
