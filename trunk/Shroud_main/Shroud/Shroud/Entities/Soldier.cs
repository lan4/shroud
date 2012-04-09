using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Math.Geometry;

using Shroud.Utilities;

namespace Shroud.Entities
{
    public class Soldier : AttackingEntity
    {
        private enum AnimationState
        {
            Idle,
            Patrolling,
            Climbing,
            Jumping,
            Attacking,
            DrawWeapon,
            Chasing,
            Dying,
            Dead
        };
        private AnimationState mCurAnimationState;

        public bool IsAlive
        {
            get
            {
                return !mCurAnimationState.Equals(AnimationState.Dead) ||
                       !mCurAnimationState.Equals(AnimationState.Dying);
            }
        }

        public Soldier(string contentManagerName, List<Node> patrol)
            : base(contentManagerName, patrol, PlayerProperties.WeaponSize, PlayerProperties.WeaponRange)
        {
            mPatrolPath = patrol;
            mCurAnimationState = AnimationState.Idle;
            mCurPatrolMode = PatrolMode.Backtrack;

            SpriteManager.AddPositionedObject(this);

            InitializeAnimations();

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);
            mCollision.Radius = 2.0f;
        }

        private void InitializeAnimations()
        {
            AnimationChainList animations = new AnimationChainList();

            AnimationChain idle = new AnimationChain();
            AnimationChain move = new AnimationChain();
            AnimationChain attack = new AnimationChain();
            AnimationChain drawweapon = new AnimationChain();
            AnimationChain chasing = new AnimationChain();
            AnimationChain dying = new AnimationChain();
            AnimationChain dead = new AnimationChain();

            string type = "Soldier1";

            int framenum = 0;
            float frametime = 0.083f;

            /*int idleFrameTotal = 2;
            for (framenum = 0; framenum < idleFrameTotal; framenum++)
            {
                idle.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/idle" + framenum, frametime, ContentManagerName));
            }
            idle.Name = "Idle";
            animations.Add(idle);*/

            int moveFrameTotal = 8;
            for (framenum = 0; framenum < moveFrameTotal; framenum++)
            {
                move.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/moving" + framenum, frametime, ContentManagerName));
            }
            move.Name = "Moving";
            animations.Add(move);

            /*int attackFrameTotal = 3;
            for (framenum = 0; framenum < attackFrameTotal; framenum++)
            {
                attack.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/attacking" + framenum, frametime, ContentManagerName));
            }
            attack.Name = "Attacking";
            animations.Add(attack);*/

            /*int drawweaponFrameTotal = 0;
            for (framenum = 0; framenum < drawweaponFrameTotal; framenum++)
            {
                drawweapon.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/drawweapon" + framenum, frametime, ContentManagerName));
            }
            drawweapon.Name = "DrawWeapon";
            animations.Add(drawweapon);*/

            /*int chaseFrameTotal = 0;
            for (framenum = 0; framenum < chaseFrameTotal; framenum++)
            {
                chasing.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/chasing" + framenum, frametime, ContentManagerName));
            }
            chasing.Name = "Chasing";
            animations.Add(chasing);*/

            /*int dyingFrameTotal = 3;
            for (framenum = 0; framenum < dyingFrameTotal; framenum++)
            {
                dying.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/dying" + framenum, frametime, ContentManagerName));
            }
            dying.Name = "Dying";
            animations.Add(dying);*/

            /*int deadFrameTotal = 3;
            for (framenum = 0; framenum < deadFrameTotal; framenum++)
            {
                dead.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/dead" + framenum, frametime, ContentManagerName));
            }
            dead.Name = "Dead";
            animations.Add(dead);*/

            mAppearance = SpriteManager.AddSprite(animations);
            mAppearance.AttachTo(this, false);
            mAppearance.CurrentChainName = "Moving";
            GameProperties.RescaleSprite(mAppearance);
            mCurAnimationState = AnimationState.Idle;
            mAppearance.RelativeRotationZ = GameProperties.WorldRotation;
        }

        protected override void SetIdle()
        {
            mCurAnimationState = AnimationState.Idle;
        }

        #region Behaviors

        private void IdleBehavior()
        {
            mCurAnimationState = AnimationState.Patrolling;
        }

        private void PatrollingBehavior()
        {
            if (mPatrolling)
            {
                Patrol();
            }
            else
            {
                StartPatrol();
            }
        }

        private void ChasingBehavior()
        {
            StartMoving();
            Chase();
        }

        private void AttackingBehavior()
        {
            Attack();
        }

        private void DyingBehavior()
        {
            if (mAppearance.CurrentChainName == "Dying" && mAppearance.JustCycled)
            {
                mCurAnimationState = AnimationState.Dead;
            }
        }

        #endregion

        public virtual void Activity()
        {
            switch (mCurAnimationState)
            {
                case AnimationState.Idle:
                    IdleBehavior();
                    break;
                case AnimationState.Patrolling:
                    PatrollingBehavior();
                    break;
                case AnimationState.Chasing:
                    ChasingBehavior();
                    break;
                case AnimationState.DrawWeapon:
                    break;
                case AnimationState.Attacking:
                    AttackingBehavior();
                    break;
                case AnimationState.Climbing:
                case AnimationState.Jumping:
                    break;
                case AnimationState.Dying:
                    DyingBehavior();
                    break;
                case AnimationState.Dead:
                    break;
                default:
                    break;
            }

            if (Math.Abs(this.Velocity.Y) > 0.01f)
                mAppearance.FlipHorizontal = this.Velocity.Y < 0;
        }

        public virtual void Destroy()
        {
            base.Destroy();
        }
    }
}
