using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Math.Geometry;

using Shroud.Utilities;

using Microsoft.Xna.Framework;

namespace Shroud.Entities
{
    public class Ninja : AttackingEntity
    {
        private enum AnimationState
        {
            Idle,
            Hiding,
            Hidden,
            Attacking,
            Climbing,
            Jumping,
            Moving,
            Chasing,
            Dying,
            Dead
        };
        private AnimationState mCurAnimationState;

        public bool IsAlive
        {
            get
            {
                return !mCurAnimationState.Equals(AnimationState.Dead) && !mCurAnimationState.Equals(AnimationState.Dying);
            }
        }

        public bool IsHidden
        {
            get { return mCurAnimationState == AnimationState.Hidden && !mIsStunned; }
        }

        public bool IsClimbing
        {
            get { return mAppearance.CurrentChainName == "Climbing"; }
        }

        public bool IsFalling
        {
            get { return mAppearance.CurrentChainName == "Fall"; }
        }

        private double mStunStart;
        private static double mStunLimit = 1.5;
        private bool mIsStunned;

        private PositionedObject mHidePoint;

        public Ninja(string contentManagerName, float speed, Layer layer)
            : base(contentManagerName, speed, PlayerProperties.WeaponSize, PlayerProperties.WeaponRange)
        {
            mCurAnimationState = AnimationState.Moving;

            //SpriteManager.AddPositionedObject(this);

            InitializeAnimations(layer);

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);
            mCollision.Radius = 3.0f;

            mStunStart = TimeManager.CurrentTime;
            mIsStunned = false;

            mHidePoint = new PositionedObject();

            //StartRunning(WorldManager.PlayerInstance);
        }

        private void InitializeAnimations(Layer layer)
        {
            AnimationChainList animations = new AnimationChainList();

            AnimationChain idle = new AnimationChain();
            AnimationChain move = new AnimationChain();
            AnimationChain chasing = new AnimationChain();
            AnimationChain dying = new AnimationChain();
            AnimationChain dead = new AnimationChain();
            AnimationChain climb = new AnimationChain();
            AnimationChain stun = new AnimationChain();
            AnimationChain attack = new AnimationChain();
            AnimationChain hide = new AnimationChain();
            AnimationChain hiding = new AnimationChain();
            AnimationChain fall = new AnimationChain();

            string type = "Ninja";

            int framenum = 0;
            float frametime = 0.083f;

            int idleFrameTotal = 11;
            for (framenum = 0; framenum < idleFrameTotal; framenum++)
            {
                idle.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/idle" + framenum, frametime, ContentManagerName));
            }
            idle.Name = "Idle";
            animations.Add(idle);

            int moveFrameTotal = 8;
            for (framenum = 0; framenum < moveFrameTotal; framenum++)
            {
                move.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/run" + framenum, frametime, ContentManagerName));
            }
            move.Name = "Moving";
            animations.Add(move);

            int climbFrameTotal = 6;
            for (framenum = 0; framenum < climbFrameTotal; framenum++)
            {
                climb.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/climb" + framenum, frametime, ContentManagerName));
            }
            climb.Name = "Climbing";
            animations.Add(climb);

            int hidingTotalFrames = 3;
            for (framenum = 0; framenum < hidingTotalFrames; framenum++)
            {
                hiding.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/hiding" + framenum, frametime, ContentManagerName));
            }
            hiding.Name = "Hiding";
            animations.Add(hiding);

            int hideTotalFrames = 1;
            for (framenum = 0; framenum < hideTotalFrames; framenum++)
            {
                hide.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/hidden" + framenum, frametime, ContentManagerName));
            }
            hide.Name = "Hide";
            animations.Add(hide);

            int dyingFrameTotal = 1;
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
                stun.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/stun" + framenum, frametime, ContentManagerName));
            }
            stun.Name = "Stunned";
            animations.Add(stun);

            int attackFrameTotal = 6;
            for (framenum = 0; framenum < attackFrameTotal; framenum++)
            {
                attack.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/attack" + framenum, frametime, ContentManagerName));
            }
            attack.Name = "Attacking";
            animations.Add(attack);

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
            mAppearance.CurrentChainName = "Idle";
            GameProperties.RescaleSprite(mAppearance);
            mCurAnimationState = AnimationState.Idle;
            mAppearance.RelativeRotationZ = GameProperties.WorldRotation;
        }

        protected override void SetIdle()
        {
            mCurAnimationState = AnimationState.Hiding;
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
                case AnimationState.Chasing:
                case AnimationState.Moving:
                    if (this.XVelocity > 1.0f || this.XVelocity < -1.0f)
                    {
                        mAppearance.CurrentChainName = "Climbing";
                    }
                    else
                    {
                        mAppearance.CurrentChainName = "Moving";
                    }
                    break;
                case AnimationState.Attacking:
                    mAppearance.CurrentChainName = "Attacking";
                    break;
                case AnimationState.Hiding:
                    mAppearance.CurrentChainName = "Hiding";
                    break;
                case AnimationState.Hidden:
                    mAppearance.CurrentChainName = "Hide";
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
            this.Velocity.X = 0.0f;
            this.Velocity.Y = 0.0f;
            this.Velocity.Z = 0.0f;

            FindStealthArea(false);
            mTarget = mHidePoint;
            StartMoving();
            mCurAnimationState = AnimationState.Moving;
        }

        private void MovingBehavior()
        {
            Move();
        }

        private void AttackingBehavior()
        {
            this.Velocity.X = 0.0f;
            this.Velocity.Y = 0.0f;
            this.Velocity.Z = 0.0f;

            if (!WorldManager.PlayerInstance.IsAlive)
            {
                mCurAnimationState = AnimationState.Idle;
            }

            Attack();

            if (this.mAttackCollision.CollideAgainst(WorldManager.PlayerInstance.Collision))
            {
                WorldManager.PlayerInstance.Stunned();
            }

            if (mAppearance.CurrentChainName == "Attacking" && mAppearance.JustCycled)
            {
                ResetAttack();
                FindStealthArea(true);
                mTarget = mHidePoint;
                StartMoving();
                mCurAnimationState = AnimationState.Moving;
                mPlayerDetected = false;
            }
        }

        private void ChasingBehavior()
        {
            if (IsPlayerVisible() && WorldManager.PlayerInstance.IsAlive)
            {
                Chase1();

                if ((this.Position - mTarget.Position).Length() < PlayerProperties.WeaponRange && mAppearance.CurrentChainName != "Climbing")
                {
                    mCurAnimationState = AnimationState.Attacking;
                }
            }
            else
            {
                FindStealthArea(false);
                mTarget = mHidePoint;
                StartMoving();
                mCurAnimationState = AnimationState.Moving;
                mPlayerDetected = false;
            }
        }

        private void HidingBehavior()
        {
            this.Velocity.X = 0.0f;
            this.Velocity.Y = 0.0f;
            this.Velocity.Z = 0.0f;

            if (mAppearance.CurrentChainName == "Hiding" && mAppearance.JustCycled)
            {
                mCurAnimationState = AnimationState.Hidden;
            }
        }

        private void DyingBehavior()
        {
            this.Velocity.X = 0.0f;
            this.Velocity.Y = 0.0f;

            if (mAppearance.CurrentChainName == "Dying" && mAppearance.JustCycled)
            {
                mCurAnimationState = AnimationState.Dead;
            }
        }

        #endregion

        public float mMinDetect = 0.4f;
        private bool mPlayerDetected = false;

        public bool IsPlayerVisible()
        {
            if (Math.Abs(WorldManager.PlayerInstance.Position.X - this.Position.X) < 3.0f && 
                Math.Abs(WorldManager.PlayerInstance.Position.Y - this.Position.Y) < 7.0f)
            {
                float yDiff = WorldManager.PlayerInstance.Position.Y - this.Position.Y;

                return (WorldManager.PlayerInstance.Opacity > mMinDetect) || mPlayerDetected;
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

        private void FindStealthArea(bool largest)
        {
            Sprite closest = null;
            float cdist = 0.0f, ndist;

            foreach (Sprite s in MyScene.StealthObjects)
            {
                ndist = Vector3.Distance(this.Position, s.Position);

                if (closest == null || (ndist < cdist && !largest) || (ndist > cdist && largest))
                {
                    closest = s;
                    cdist = ndist;
                }
            }

            if (closest != null)
            {
                mHidePoint.X = closest.X;
                mHidePoint.Y = closest.Y;
            }
            else
            {
                mHidePoint.X = this.X;
                mHidePoint.Y = this.Y;
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
        }

        public virtual void Activity()
        {
            if ((this.Position - WorldManager.PlayerInstance.Position).Length() > 15.0f)
            {
                mPlayerDetected = false;
            }

            /*if (MyScene.Equals(LevelManager.CurrentScene))
            {
                System.Diagnostics.Debug.WriteLine(mCurAnimationState.ToString());
            }*/

            if (IsPlayerVisible() && this.IsAlive && !mIsStunned && mAppearance.CurrentChainName != "Fall" && 
                mCurAnimationState != AnimationState.Chasing && mCurAnimationState != AnimationState.Moving && 
                mCurAnimationState != AnimationState.Attacking && !WorldManager.PlayerInstance.IsStunned)
            {
                mPlayerDetected = true;
                StartAttack(WorldManager.PlayerInstance);
            }

            if (!mIsStunned && mAppearance.CurrentChainName != "Fall")
            {
                switch (mCurAnimationState)
                {
                    case AnimationState.Idle:
                        IdleBehavior();
                        break;
                    case AnimationState.Moving:
                        MovingBehavior();
                        break;
                    case AnimationState.Chasing:
                        ChasingBehavior();
                        break;
                    case AnimationState.Hiding:
                        HidingBehavior();
                        break;
                    case AnimationState.Attacking:
                        AttackingBehavior();
                        break;
                    case AnimationState.Dying:
                        DyingBehavior();
                        break;
                    case AnimationState.Dead:
                    case AnimationState.Hidden:
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
                GameProperties.OneKillBadge = false;
            }
        }

        public virtual void Destroy()
        {
            base.Destroy();
        }
    }
}

