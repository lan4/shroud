using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Math.Geometry;

using Shroud.Utilities;
using AlertState = Shroud.Utilities.StealthManager.AlertState;

namespace Shroud.Entities
{
    public class Enemy1 : PositionedObject
    {
        #region Fields

        // Basic Entity Properties
        private Sprite mAppearance;
        private Circle mCollision;

        // Keep the ContentManager for easy access:
        string mContentManagerName;

        private enum AnimationState
        {
            Idle,
            Patroling,
            Climbing,
            Jumping,
            Attacking,
            DrawWeapon,
            Chasing,
            Dying,
            Dead
        };

        // Variable(s) needed for LIVING ENTITIES
        private AnimationState mCurAnimationState;
        private bool mFacingRight;

        // Variable(s) needed for MOVING and CHASING
        private List<Node> mPath;
        private List<Node> mPatrolPath;
        private Node mStart;
        private Node mEnd;
        private Node mCur;

        // Variable(s) needed for ATTACKING
        private Circle mAttackCollision;

        // Variable(s) needed for CHASING
        private PositionedObject mTarget;

        // Variable(s) needed for STEALTH
        private float mLoSLimit;
        private AlertState mCurAlertState;

        public enum EnemyType
        {
            Soldier1,
            Soldier2,
            Noble,
            General,
            Ninja1,
            Ninja2
        };
        private EnemyType mType;

        public enum PatrolMode
        {
            None,
            Sentry,
            Circular,
            Backtrack
        }
        private PatrolMode mCurPatrolMode;

        #endregion

        #region Properties

        public Circle Collision
        {
            get { return mCollision; }
        }

        public bool IsAlive
        {
            get
            {
                return mCurAnimationState.Equals(AnimationState.Dead) ||
                       mCurAnimationState.Equals(AnimationState.Dying);
            }
        }

        public float MaxLOS
        {
            get { return mLoSLimit; }
        }

        public float DetectRating
        {
            get { return 4.0f; }
        }

        #endregion

        #region Methods

        // Constructor
        public Enemy1(string contentManagerName, EnemyType t, PatrolMode m)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            mType = t;
            mCurPatrolMode = m;

            // If you don't want to add to managers, make an overriding constructor
            Initialize(true);
        }

        protected virtual void Initialize(bool addToManagers)
        {
            mPath = new List<Node>();
            mPatrolPath = new List<Node>();
            mStart = Node.CreateNode();
            mEnd = Node.CreateNode();
            mCur = null;

            mFacingRight = true;

            mLoSLimit = 5.0f;
            mCurAlertState = AlertState.None;

            if (addToManagers)
            {
                AddToManagers(null);
            }
        }

        public virtual void AddToManagers(Layer layerToAddTo)
        {
            SpriteManager.AddPositionedObject(this);

            InitializeAnimations();

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);

            mAttackCollision = ShapeManager.AddCircle();
            mAttackCollision.AttachTo(this, false);
        }

        #region Animation Setup

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

            string type = "";

            switch (mType)
            {
                case EnemyType.Soldier1:
                    type = "Soldier1";
                    break;
                case EnemyType.Soldier2:
                    type = "Soldier2";
                    break;
                case EnemyType.Ninja1:
                    type = "Ninja1";
                    break;
                case EnemyType.Ninja2:
                    type = "Ninja2";
                    break;
                case EnemyType.Noble:
                    type = "Noble";
                    break;
                case EnemyType.General:
                    type = "General";
                    break;
            }

            int framenum = 0;
            float frametime = 0.012f;

            int idleFrameTotal = 2;
            for (framenum = 0; framenum < idleFrameTotal; framenum++)
            {
                idle.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/idle" + framenum, frametime, mContentManagerName));
            }
            idle.Name = "Idle";
            animations.Add(idle);

            int moveFrameTotal = 3;
            for (framenum = 0; framenum < moveFrameTotal; framenum++)
            {
                move.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/moving" + framenum, frametime, mContentManagerName));
            }
            move.Name = "Moving";
            animations.Add(move);

            int attackFrameTotal = 3;
            for (framenum = 0; framenum < attackFrameTotal; framenum++)
            {
                attack.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/attacking" + framenum, frametime, mContentManagerName));
            }
            attack.Name = "Attacking";
            animations.Add(attack);

            int drawweaponFrameTotal = 3;
            for (framenum = 0; framenum < drawweaponFrameTotal; framenum++)
            {
                drawweapon.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/drawweapon" + framenum, frametime, mContentManagerName));
            }
            drawweapon.Name = "DrawWeapon";
            animations.Add(drawweapon);

            int chaseFrameTotal = 3;
            for (framenum = 0; framenum < chaseFrameTotal; framenum++)
            {
                chasing.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/chasing" + framenum, frametime, mContentManagerName));
            }
            chasing.Name = "Chasing";
            animations.Add(chasing);

            int dyingFrameTotal = 3;
            for (framenum = 0; framenum < dyingFrameTotal; framenum++)
            {
                dying.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/dying" + framenum, frametime, mContentManagerName));
            }
            dying.Name = "Dying";
            animations.Add(dying);

            int deadFrameTotal = 3;
            for (framenum = 0; framenum < deadFrameTotal; framenum++)
            {
                dead.Add(new AnimationFrame(@"Content/Entities/Enemy/" + type + "/dead" + framenum, frametime, mContentManagerName));
            }
            dead.Name = "Dead";
            animations.Add(dead); 

            mAppearance = SpriteManager.AddSprite(animations);
            mAppearance.AttachTo(this, false);
            GameProperties.RescaleSprite(mAppearance);
            mCurAnimationState = AnimationState.Idle;
        }

        #endregion

        #region Helper Functions

        #region Enemy Type Behaviors

        private void SoldierBehavior()
        {
            switch (mCurAnimationState)
            {
                case AnimationState.Idle:
                    break;
                case AnimationState.Patroling:
                    break;
                case AnimationState.Chasing:
                    break;
                case AnimationState.DrawWeapon:
                    break;
                case AnimationState.Attacking:
                    break;
                case AnimationState.Climbing:
                case AnimationState.Jumping:
                    break;
                case AnimationState.Dying:
                    break;
                case AnimationState.Dead:
                    break;
                default:
                    break;
            }
        }

        private void NinjaBehavior()
        {
            switch (mCurAnimationState)
            {
                case AnimationState.Idle:
                    break;
                case AnimationState.Patroling:
                    break;
                case AnimationState.Chasing:
                    break;
                case AnimationState.DrawWeapon:
                    break;
                case AnimationState.Attacking:
                    break;
                case AnimationState.Climbing:
                case AnimationState.Jumping:
                    break;
                case AnimationState.Dying:
                    break;
                case AnimationState.Dead:
                    break;
                default:
                    break;
            }
        }

        private void NobleBehavior()
        {
            switch (mCurAnimationState)
            {
                case AnimationState.Idle:
                    break;
                case AnimationState.Patroling:
                    break;
                case AnimationState.Chasing:
                    break;
                case AnimationState.DrawWeapon:
                    break;
                case AnimationState.Attacking:
                    break;
                case AnimationState.Climbing:
                case AnimationState.Jumping:
                    break;
                case AnimationState.Dying:
                    break;
                case AnimationState.Dead:
                    break;
                default:
                    break;
            }
        }

        #endregion

        #endregion

        public virtual void Activity()
        {
            switch (mType)
            {
                case EnemyType.Soldier1:
                case EnemyType.Soldier2:
                case EnemyType.General:
                    SoldierBehavior();
                    break;
                case EnemyType.Ninja1:
                case EnemyType.Ninja2:
                    NinjaBehavior();
                    break;
                case EnemyType.Noble:
                    NobleBehavior();
                    break;
                default:
                    break;
            }
        }

        public virtual void Destroy()
        {
            // Remove self from the SpriteManager:
            SpriteManager.RemovePositionedObject(this);

            // Remove any other objects you've created:
            SpriteManager.RemoveSprite(mAppearance);
            ShapeManager.Remove(mCollision);
        }

        #endregion
    }
}
