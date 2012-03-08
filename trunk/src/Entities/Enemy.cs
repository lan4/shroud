using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Graphics.Animation;
using Point = FlatRedBall.Math.Geometry.Point;

using Microsoft.Xna.Framework;

using Shroud.Utilities;

namespace Shroud.Entities
{
    public class Enemy : PositionedObject
    {
        #region Fields

        private Sprite mVisibleRepresentation;
        private Polygon mCollision;

        // Keep the ContentManager for easy access:
        string mContentManagerName;

        // Variables for Patrolling
        private List<Node> mPatrolPath;
        private int mCurPatrolNode;
        private Node mInterruptNode; // Keeps track of where the patrol was interrupted
        private bool mBacktrack;
        private bool mStartBacktrack;
        private bool mMovingBack;
        private bool mPatroling;

        // Variables for Moving To
        private List<Node> mMoveToPath;
        private int mCurMoveNode;
        private Node mMoveToNode;
        private Node mMoveFromNode;
        private bool mMovingTo;

        private bool mAlive;
        private bool mIsFacingLeft;
        private bool mIsTarget;

        // Variables for Attacking
        private Circle mHitMarker;
        private bool mIsAttacking;

        #endregion

        #region Properties

        public Polygon Collision
        {
            get { return mCollision; }
        }

        public List<Node> PatrolPath
        {
            get { return mPatrolPath; }
        }

        public bool Backtrack
        {
            set { mBacktrack = value; }
        }

        public bool Alive
        {
            get { return mAlive; }
        }

        public bool IsAttacking
        {
            get { return mIsAttacking; }
        }

        #endregion

        #region Methods

        // Constructor
        public Enemy(string contentManagerName, bool isTarget)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            // If you don't want to add to managers, make an overriding constructor
            Initialize(true, isTarget);
        }

        protected virtual void Initialize(bool addToManagers, bool isTarget)
        {
            mPatrolPath = new List<Node>();
            mInterruptNode = new Node();
            mCurPatrolNode = 0;
            mBacktrack = false;
            mStartBacktrack = false;
            mMovingBack = false;
            mPatroling = true;

            mMoveToPath = new List<Node>();
            mMoveToNode = new Node();
            mMoveFromNode = new Node();
            mCurMoveNode = 0;
            mMovingTo = false;

            mAlive = true;
            mIsFacingLeft = false;

            mIsAttacking = false;

            mIsTarget = isTarget;

            if (addToManagers)
            {
                AddToManagers(null, isTarget);
            }
        }

        public virtual void AddToManagers(Layer layerToAddTo, bool isTarget)
        {
            SpriteManager.AddPositionedObject(this);

            //mVisibleRepresentation = SpriteManager.AddSprite("redball.png", mContentManagerName);
            //mVisibleRepresentation.AttachTo(this, false);

            if (isTarget)
                InitializeAnimationsAlt();
            else
                InitializeAnimation();

            Point[] pointA = 
            {
                new Point( 2.0f,  0.5f),
                new Point( 2.0f, -0.5f),
                new Point(-2.0f, -0.5f),
                new Point(-2.0f,  0.5f),
                new Point( 2.0f,  0.5f)
            };

            mCollision = ShapeManager.AddPolygon();
            mCollision.AttachTo(this, false);
            //mCollision.Radius = 1.5f;
            mCollision.Points = pointA;

            mHitMarker = ShapeManager.AddCircle();
            mHitMarker.AttachTo(this, false);
            mHitMarker.Radius = 0.5f;
        }

        private void InitializeAnimation()
        {
            AnimationChainList fullBodyAnim = new AnimationChainList();

            AnimationChain walking = new AnimationChain();

            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0001", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0002", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0003", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0004", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0005", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0006", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0007", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0008", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0009", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0010", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0011", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0012", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0013", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0014", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/samani_walk0015", 0.0833f, "Global"));

            walking.Name = "WALK";

            AnimationChain attacking = new AnimationChain();

            attacking.Add(new AnimationFrame(@"Content/Entities/Enemy/samurai_attack0001", 0.0833f, "Global"));
            attacking.Add(new AnimationFrame(@"Content/Entities/Enemy/samurai_attack0002", 0.0833f, "Global"));
            attacking.Add(new AnimationFrame(@"Content/Entities/Enemy/samurai_attack0003", 0.0833f, "Global"));
            attacking.Add(new AnimationFrame(@"Content/Entities/Enemy/samurai_attack0004", 0.0833f, "Global"));
            attacking.Add(new AnimationFrame(@"Content/Entities/Enemy/samurai_attack0005", 0.0833f, "Global"));
            attacking.Add(new AnimationFrame(@"Content/Entities/Enemy/samurai_attack0006", 0.0833f, "Global"));
            attacking.Add(new AnimationFrame(@"Content/Entities/Enemy/samurai_attack0007", 0.0833f, "Global"));

            attacking.Name = "ATTACK";

            AnimationChain idle = new AnimationChain();

            idle.Add(new AnimationFrame(@"Content/Entities/Enemy/samurai_stand", 0.0833f, "Global"));

            idle.Name = "IDLE";

            fullBodyAnim.Add(walking);
            fullBodyAnim.Add(attacking);
            fullBodyAnim.Add(idle);

            mVisibleRepresentation = SpriteManager.AddSprite(fullBodyAnim);
            mVisibleRepresentation.AttachTo(this, false);
            mVisibleRepresentation.RelativeRotationZ = GameProperties.WorldRotation;
            mVisibleRepresentation.CurrentChainName = "IDLE";
            GameProperties.RescaleSprite(mVisibleRepresentation);
        }

        private void InitializeAnimationsAlt()
        {
            AnimationChainList fullBodyAnim = new AnimationChainList();

            AnimationChain walking = new AnimationChain();

            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/target0001", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/target0002", 0.0833f, "Global"));
            walking.Add(new AnimationFrame(@"Content/Entities/Enemy/target0003", 0.0833f, "Global"));

            walking.Name = "WALK";

            AnimationChain idle = new AnimationChain();

            idle.Add(new AnimationFrame(@"Content/Entities/Enemy/target0001", 0.0833f, "Global"));

            idle.Name = "IDLE";

            fullBodyAnim.Add(walking);
            fullBodyAnim.Add(idle);

            mVisibleRepresentation = SpriteManager.AddSprite(fullBodyAnim);
            mVisibleRepresentation.AttachTo(this, false);
            mVisibleRepresentation.RelativeRotationZ = GameProperties.WorldRotation;
            mVisibleRepresentation.CurrentChainName = "IDLE";
            GameProperties.RescaleSprite(mVisibleRepresentation);
        }

        public void MoveTo(Vector3 pos)
        {
            mMoveToPath.Clear();

            mMoveToNode.Initialize();
            mMoveToNode.SetPosition(pos.X, pos.Y, 0.0f);
            mMoveFromNode.SetPosition(X, Y, Z);

            if (!mMovingBack && mPatroling)
            {
                mInterruptNode.Initialize();
                mInterruptNode.SetPosition(X, Y, Z);
            }

            NodeManager.FindPathTo(mMoveFromNode, mMoveToNode, ref mMoveToPath);

            mMovingTo = true;
            mPatroling = false;

            mCurMoveNode = 0;

            this.Velocity = Vector3.Zero;
        }

        private void Move()
        {
            if (mCurMoveNode < mMoveToPath.Count)
            {
                if ((mMoveToPath[mCurMoveNode].Position - this.Position).Length() < GameProperties.EnemyNodeTolerance)
                {
                    mCurMoveNode++;

                    if (mCurMoveNode < mMoveToPath.Count)
                    {
                        this.Velocity = Vector3.Normalize(mMoveToPath[mCurMoveNode].Position - this.Position) * GameProperties.EnemyMoveSpeed;
                    }
                    else
                    {
                        if (mMovingBack)
                        {
                            mMovingBack = false;
                            mMovingTo = false;
                            //mInterruptNode.Distance = 12345.67f;
                        }
                        else
                        {
                            mMovingBack = true;
                            //mMoveToPath.Reverse();
                            //mMoveToPath.Add(mInterruptNode);
                            MoveTo(mInterruptNode.Position);
                            //mCurMoveNode = 0;
                        }

                        this.Velocity = Vector3.Zero;
                    }
                }
                else if (mCurMoveNode == 0)
                {
                    // Needed to bootstrap movement
                    this.Velocity = Vector3.Normalize(mMoveToPath[mCurMoveNode].Position - this.Position) * GameProperties.EnemyMoveSpeed;
                }
            }
            else
            {
                if (mMovingBack)
                {
                    mMovingBack = false;
                    mMovingTo = false;
                    //mInterruptNode.Distance = 12345.67f;
                }
                else
                {
                    mMovingBack = true;
                    //mMoveToPath.Reverse();
                    //mMoveToPath.Add(mInterruptNode);
                    MoveTo(mInterruptNode.Position);
                    //mCurMoveNode = 0;
                }

                this.Velocity = Vector3.Zero;
            }

        }

        private void Patrol()
        {
            if ((mPatrolPath[mCurPatrolNode].Position - this.Position).Length() < GameProperties.EnemyNodeTolerance)
            {
                if (mStartBacktrack)
                    mCurPatrolNode--;
                else
                    mCurPatrolNode++;
            }

            if (mCurPatrolNode >= mPatrolPath.Count)
            {
                if (mBacktrack)
                {
                    mStartBacktrack = true;
                    mCurPatrolNode--;
                }
                else
                {
                    mCurPatrolNode = 0;
                }
            }
            else if (mCurPatrolNode < 0)
            {
                if (mBacktrack)
                {
                    mCurPatrolNode = 1;
                    mStartBacktrack = false;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("SOMETHING WENT HORRIBLY WRONG");
                }
            }

            this.Velocity = Vector3.Normalize(mPatrolPath[mCurPatrolNode].Position - this.Position) * GameProperties.EnemyMoveSpeed;
        }

        public void Kill()
        {
            mAlive = false;
            //mVisibleRepresentation.Visible = false;
            this.RotationZ = (float)-Math.PI / 2.0f;
            this.Position.X -= 1.0f;
            this.Velocity = Vector3.Zero;
            mVisibleRepresentation.CurrentChainName = "IDLE";
        }

        public void Attack()
        {
            mVisibleRepresentation.CurrentChainName = "ATTACK";
            GameProperties.RescaleSprite(mVisibleRepresentation);

            if (!mIsAttacking)
            {
                mIsAttacking = true;

                if (mIsFacingLeft)
                {
                    mHitMarker.RelativePosition.Y = 1.5f;
                }
                else
                {
                    mHitMarker.RelativePosition.Y = -1.5f;
                }

                Velocity = Vector3.Zero;
            }
        }

        public bool CheckHit(Player p)
        {
            return mHitMarker.CollideAgainst(CollisionManager.PlayerCollision) && mIsAttacking;
        }

        public virtual void Activity()
        {
            if (!mIsAttacking)
            {
                // Need to trigger MoveTo in order to move towards target
                if (mMovingTo)
                {
                    Move();
                    mVisibleRepresentation.CurrentChainName = "WALK";
                }
                else
                {
                    if (mPatrolPath.Count > 0)
                    {
                        mPatroling = true;
                        Patrol();
                        mVisibleRepresentation.CurrentChainName = "WALK";
                    }
                    else
                        mVisibleRepresentation.CurrentChainName = "IDLE";
                }

                GameProperties.RescaleSprite(mVisibleRepresentation);
            }
            else
            {
                // NEED TO CHECK THAT ATTACK ANIMATION ENDED

                if (mVisibleRepresentation.JustCycled)
                {
                    mIsAttacking = false;
                }
            }

            // Change which way the guard is facing
            if (Velocity.Y > 0.0f)
            {
                mIsFacingLeft = true;
            }
            else if (Velocity.Y < 0.0f)
            {
                mIsFacingLeft = false;
            }

            if (!mIsTarget)
                mVisibleRepresentation.FlipHorizontal = mIsFacingLeft;
            else
                mVisibleRepresentation.FlipHorizontal = !mIsFacingLeft;
        }

        public virtual void Destroy()
        {
            // Remove self from the SpriteManager:
            SpriteManager.RemovePositionedObject(this);

            // Remove any other objects you've created:
            SpriteManager.RemoveSprite(mVisibleRepresentation);
            ShapeManager.Remove(mCollision);
        }

        #endregion
    }
}
