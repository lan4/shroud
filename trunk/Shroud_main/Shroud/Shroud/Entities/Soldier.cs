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

        private float mMinDetect = 0.7f;

        private double mStunStart;
        private static double mStunLimit = 1.5;
        private bool mIsStunned;

        public bool IsAlive
        {
            get
            {
                return !mCurAnimationState.Equals(AnimationState.Dead) && !mCurAnimationState.Equals(AnimationState.Dying);
            }
        }

        public bool IsClimbing
        {
            get { return mAppearance.CurrentChainName == "Climbing"; }
        }

        public bool IsFalling
        {
            get { return mAppearance.CurrentChainName == "Fall"; }
        }

        public Soldier(string contentManagerName, List<Node> patrol, float speed, Layer layer)
            : base(contentManagerName, patrol, speed, PlayerProperties.WeaponSize, PlayerProperties.WeaponRange)
        {
            mPatrolPath = patrol;
            mCurAnimationState = AnimationState.Idle;
            mCurPatrolMode = PatrolMode.Backtrack;

            //SpriteManager.AddPositionedObject(this);

            InitializeAnimations(layer);

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);
            mCollision.Radius = 2.0f;

            mStunStart = TimeManager.CurrentTime;
            mIsStunned = false;

            //StartAttack(WorldManager.PlayerInstance);
        }

        private void InitializeAnimations(Layer layer)
        {
            AnimationChainList animations = new AnimationChainList();

            AnimationChain idle = new AnimationChain();
            AnimationChain move = new AnimationChain();
            AnimationChain attack = new AnimationChain();
            AnimationChain drawweapon = new AnimationChain();
            AnimationChain chasing = new AnimationChain();
            AnimationChain dying = new AnimationChain();
            AnimationChain dead = new AnimationChain();
            AnimationChain climb = new AnimationChain();
            AnimationChain stunned = new AnimationChain();
            AnimationChain fall = new AnimationChain();

            string type = "Soldier1";

            int framenum = 0;
            float frametime = 0.083f;

            int idleFrameTotal = 1;
            for (framenum = 0; framenum < idleFrameTotal; framenum++)
            {
                idle.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/idle" + framenum, frametime, ContentManagerName));
            }
            idle.Name = "Idle";
            animations.Add(idle);

            int moveFrameTotal = 8;
            for (framenum = 0; framenum < moveFrameTotal; framenum++)
            {
                move.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/moving" + framenum, frametime, ContentManagerName));
            }
            move.Name = "Moving";
            animations.Add(move);

            int attackFrameTotal = 5;
            for (framenum = 0; framenum < attackFrameTotal; framenum++)
            {
                attack.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/attacking" + framenum, frametime, ContentManagerName));
            }
            attack.Name = "Attacking";
            animations.Add(attack);

            int climbFrameTotal = 8;
            for (framenum = 0; framenum < climbFrameTotal; framenum++)
            {
                climb.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/climb" + framenum, frametime, ContentManagerName));
            }
            climb.Name = "Climbing";
            animations.Add(climb);

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

            int dyingFrameTotal = 5;
            for (framenum = 0; framenum < dyingFrameTotal; framenum++)
            {
                dying.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/dying" + framenum, frametime, ContentManagerName));
            }
            dying.Name = "Dying";
            animations.Add(dying);

            int deadFrameTotal = 1;
            for (framenum = 0; framenum < deadFrameTotal; framenum++)
            {
                dead.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/dead" + framenum, frametime, ContentManagerName));
            }
            dead.Name = "Dead";
            animations.Add(dead);

            int stunFrameTotal = 1;
            for (framenum = 0; framenum < stunFrameTotal; framenum++)
            {
                stunned.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/stunned_unarmed" + framenum, frametime, ContentManagerName));
            }
            stunned.Name = "Stunned";
            animations.Add(stunned);

            int fallFrameTotal = 1;
            for (framenum = 0; framenum < fallFrameTotal; framenum++)
            {
                fall.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/fall" + framenum, frametime, ContentManagerName));
            }
            fall.Name = "Fall";
            animations.Add(fall);

            mAppearance = SpriteManager.AddSprite(animations);
            SpriteManager.AddToLayer(mAppearance, layer);
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

        private void SetAnimation()
        {
            mAppearance.Animate = true;

            if (mAppearance.CurrentChainName == "Fall")
            {
                return;
            }

            if (mIsStunned)
            {
                mAppearance.CurrentChainName = "Stunned";
                return;
            }

            switch (mCurAnimationState)
            {
                case AnimationState.Attacking:
                    mAppearance.CurrentChainName = "Attacking";
                    break;
                case AnimationState.Climbing:
                    mAppearance.CurrentChainName = "Climbing";
                    break;
                case AnimationState.Idle:
                    if (mAppearance.CurrentChainName == "Climbing")
                    {
                        mAppearance.Animate = false;
                    }
                    else
                    {
                        mAppearance.CurrentChainName = "Idle";
                    }
                    break;
                case AnimationState.Jumping:
                    //mAppearance.CurrentChainName = "Jumping";
                    break;
                case AnimationState.Chasing:
                case AnimationState.Patrolling:
                    if (this.XVelocity > 1.0f || this.XVelocity < -1.0f)
                    {
                        mAppearance.CurrentChainName = "Climbing";
                    }
                    else
                    {
                        if (Math.Abs(this.YVelocity) < 0.1f)
                        {
                            mAppearance.CurrentChainName = "Idle";
                        }
                        else
                        {
                            mAppearance.CurrentChainName = "Moving";
                        }
                    }
                    break;
                case AnimationState.Dying:
                    mAppearance.CurrentChainName = "Dying";
                    break;
                case AnimationState.Dead:
                    mAppearance.CurrentChainName = "Dead";
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Error: Player AnimationState not valid");
                    break;
            }
        }

        #region Behaviors

        private void IdleBehavior()
        {
            mPatrolling = false;
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
            if (IsPlayerVisible())
            {
                Chase1();

                if ((this.Position - mTarget.Position).Length() < PlayerProperties.WeaponRange && mAppearance.CurrentChainName != "Climbing")
                {
                    mCurAnimationState = AnimationState.Attacking;
                }
            }
            else
            {
                mPatrolling = false;
                mCurAnimationState = AnimationState.Patrolling;
            }
        }

        private void AttackingBehavior()
        {
            this.Velocity.X = 0.0f;
            this.Velocity.Y = 0.0f;
            this.Velocity.Z = 0.0f;

            //System.Diagnostics.Debug.WriteLine(WorldManager.PlayerInstance.IsAlive);

            if (!WorldManager.PlayerInstance.IsAlive)
            {
                mCurAnimationState = AnimationState.Idle;
                mPlayerDetected = false;
            }
            else
            {

                Attack();

                if (this.mAttackCollision.CollideAgainst(WorldManager.PlayerInstance.Collision) && WorldManager.PlayerInstance.IsAlive)
                {
                    WorldManager.PlayerInstance.Die();
                }
                else
                {
                    if (mAppearance.CurrentChainName == "Attacking" && mAppearance.JustCycled)
                    {
                        mPatrolling = false;
                        mCurAnimationState = AnimationState.Patrolling;
                    }
                }

                mPlayerDetected = false;
            }
        }

        private void DyingBehavior()
        {
            this.Velocity.X = 0.0f;
            this.Velocity.Y = 0.0f;
            this.Velocity.Z = 0.0f;

            if (mAppearance.CurrentChainName == "Dying" && mAppearance.JustCycled)
            {
                mCurAnimationState = AnimationState.Dead;
            }
        }

        #endregion

        private bool mPlayerDetected = false;

        public bool IsPlayerVisible()
        {
            if (Math.Abs(WorldManager.PlayerInstance.Position.X - this.Position.X) < 4.0f && 
                Math.Abs(WorldManager.PlayerInstance.Position.Y - this.Position.Y) < 7.0f)
            {
                float yDiff = WorldManager.PlayerInstance.Position.Y - this.Position.Y;

                return (((yDiff < 0 && mFacingRight) || (yDiff > 0 && !mFacingRight)) && 
                        WorldManager.PlayerInstance.Opacity > mMinDetect) || 
                       mPlayerDetected;
            }

            return mPlayerDetected;
        }

        public void StartAttack(Player2 p)
        {
            mCurAnimationState = AnimationState.Chasing;
            mTarget = p;
            //StartMoving();

            mStart.Position = this.Position;
            mEnd.Position = mTarget.Position;
            Node.NodeListToUse = MyScene.Nodes;
            Node n = Node.FindNextNodeToward(mStart, mEnd);
            mEnd.Position = n.Position;
        }

        public void Stunned()
        {
            if (this.IsAlive && mAppearance.CurrentChainName != "Fall" && mAppearance.CurrentChainName != "Climbing")
            {
                mStunStart = TimeManager.CurrentTime;
                mIsStunned = true;

                this.Velocity.X = 0.0f;
                this.Velocity.Y = 0.0f;
                this.Velocity.Z = 0.0f;

                mAppearance.Animate = true;
                mPlayerDetected = false;
            }
        }

        public void Fall()
        {
            mCurAnimationState = AnimationState.Idle;
            mAppearance.CurrentChainName = "Fall";
            mAppearance.Animate = true;
            this.Velocity.X = 0.0f;
            this.Velocity.Y = 0.0f;
            this.Acceleration.X = -20.0f;
            mStart.X = this.X;
            mStart.Y = this.Y;

            Node n = Node.FindFallNode(mStart);
            mEnd.Position = n.Position;
        }

        public void Die()
        {
            if (mCurAnimationState != AnimationState.Dead)
            {
                mCurAnimationState = AnimationState.Dying;
                mIsStunned = false;
            }

            GameProperties.OneKillBadge = false;
        }

        public virtual void Activity()
        {
            if ((this.Position - WorldManager.PlayerInstance.Position).Length() > 15.0f)
            {
                mPlayerDetected = false;
            }

            if (IsPlayerVisible() && this.IsAlive && !mIsStunned && mAppearance.CurrentChainName != "Fall" && 
                mCurAnimationState != AnimationState.Chasing && mCurAnimationState != AnimationState.Attacking &&
                WorldManager.PlayerInstance.IsAlive)
            {
                mPlayerDetected = true;
                GameProperties.HiddenBadge = false;
                StartAttack(WorldManager.PlayerInstance);
            }

            if (!mIsStunned && mAppearance.CurrentChainName != "Fall")
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
            }

            if (this.Velocity.Y > 2.0f)
                mFacingRight = false;
            else if (this.Velocity.Y < -2.0f)
                mFacingRight = true;

            mAppearance.FlipHorizontal = mFacingRight;

            SetAnimation();

            if (TimeManager.CurrentTime - mStunStart > mStunLimit && mIsStunned)
            {
                mIsStunned = false;
            }

            if (mAppearance.CurrentChainName == "Fall" && this.X - mEnd.X < -mAppearance.ScaleX / 2.0f)
            {
                mAppearance.CurrentChainName = "Dead";
                mCurAnimationState = AnimationState.Dead;
                this.X = mEnd.X;
                this.Y = mEnd.Y;
                this.Acceleration.X = 0.0f;
                this.Velocity.X = 0.0f;
            }
        }

        public virtual void Destroy()
        {
            base.Destroy();
        }
    }
}
