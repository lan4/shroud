using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using FlatRedBall;

using Shroud.Utilities;

namespace Shroud.Entities
{
    public abstract class MovingEntity : Entity
    {
        private float mSpeed;

        protected List<Node> mPath;
        protected Node mStart;
        protected Node mEnd;
        protected Node mCur;
        public bool mFacingRight;

        protected PositionedObject mTarget;

        protected List<Node> mPatrolPath;
        protected Node mLastPatrolNode;
        protected bool mPatrolling;
        private bool mPatrolReady;
        private bool mCanBacktrack;
        private bool mBacktracking;

        public enum PatrolMode
        {
            None,
            Sentry,
            Circular,
            Backtrack
        }
        protected PatrolMode mCurPatrolMode;

        protected MovingEntity(string contentManagerName, float speed)
            : base(contentManagerName)
        {
            /*mStart = Node.CreateNode();
            mEnd = Node.CreateNode();
            mPath = new List<Node>();
            mCur = null;
            mFacingRight = false;

            mTarget = null;*/
            Initialize();
            mSpeed = speed;
            mCurPatrolMode = PatrolMode.None;
            mPatrolPath = null;
            mLastPatrolNode = null;
            mPatrolling = false;
            mPatrolReady = false;
            mCanBacktrack = false;
            mBacktracking = false;
        }

        protected MovingEntity(string contentManagerName, List<Node> patrol, float speed)
            : base(contentManagerName)
        {
            /*mStart = Node.CreateNode();
            mEnd = Node.CreateNode();
            mPath = new List<Node>();
            mCur = null;
            mFacingRight = false;

            mTarget = null;*/
            Initialize();
            mSpeed = speed;
            mCurPatrolMode = PatrolMode.Backtrack;
            mPatrolPath = patrol;
            mLastPatrolNode = mPatrolPath[0];
            mPatrolling = false;
            mPatrolReady = false;

            mCanBacktrack = mPatrolPath.Count != 1 && !mPatrolPath[0].IsNeighbor(mPatrolPath[mPatrolPath.Count - 1]);
            mBacktracking = false;
        }

        private void Initialize()
        {
            mStart = Node.CreateNode();
            mEnd = Node.CreateNode();
            mPath = new List<Node>();
            mCur = null;
            mFacingRight = false;

            mTarget = null;

            mStartSentry = TimeManager.CurrentTime;
            mSentryLimit = 3.0f;
        }

        #region Helpers

        private void PrepareMovement()
        {
            mStart.Position = this.Position;

            if (mTarget != null)
            {
                if (mTarget.GetType().Equals(typeof(InteractObject)))
                {
                    InteractObject iobject = (InteractObject)WorldManager.InteractTarget;

                    mEnd.Position = iobject.Position;
                }
                else if (mTarget.GetType().Equals(typeof(DestructibleObject)))
                {
                    DestructibleObject dobject = (DestructibleObject)WorldManager.InteractTarget;

                    mEnd.Position = dobject.Position;
                }

                mEnd.Position = mTarget.Position;
            }
            else
            {
                mEnd.Position = GestureManager.EndTouchWorld;
            }

            this.Velocity = Vector3.Zero;
        }

        protected void StartMoving()
        {
            PrepareMovement();

            if (mPatrolling)
            {
                mEnd.Position = mLastPatrolNode.Position;
            }

            Node.NodeListToUse = MyScene.Nodes;
            Node.GetPathBetween(mStart, mEnd, ref mPath);

            mCur = mPath[0];
            mPath.RemoveAt(0);

            if ((mCur.Position - this.Position).Length() > PlayerProperties.MoveTolerance)
            {
                MoveToNextNode();
            }
            else
            {
                this.Velocity = Vector3.Zero;

                if (!mPatrolling)
                {
                    SetIdle();
                }
                else
                {
                    mPatrolReady = true;
                    mCur = mLastPatrolNode;
                }
            }
        }

        private void MoveToNextNode()
        {
            this.Velocity = mCur.Position - this.Position;
            
            if (this.Velocity.Length() > 0.0f)
            {
                this.Velocity.Normalize();
            }

            this.Velocity *= mSpeed;
        }

        private void GetNextPatrolNode()
        {
            if (mPatrolPath != null && mPatrolPath.Count > 0)
            {
                if (mPatrolPath.Contains(mCur))
                {
                    int index = mPatrolPath.IndexOf(mCur);

                    if (index <= 0 && mBacktracking)
                    {
                        mBacktracking = false;
                        index = 0;
                    }

                    if (mBacktracking)
                        index--;
                    else
                        index++;

                    //THIS IS CIRCULAR BEHAVIOR
                    if (index >= mPatrolPath.Count && !mCanBacktrack)
                        index = 0;
                    else if (index >= mPatrolPath.Count && mCanBacktrack)
                    {
                        mBacktracking = true;
                        index--;
                    }

                    mLastPatrolNode = mCur;
                    mCur = mPatrolPath[index];
                }
                else
                {
                    mCur = mLastPatrolNode;
                }
            }
        }

        protected void StartPatrol()
        {
            if (mLastPatrolNode != null)
            {
                mPatrolling = true;
                StartMoving();
            }
        }

        #endregion

        protected void Move()
        {
            if ((mCur.Position - this.Position).Length() < PlayerProperties.MoveTolerance)
            {
                if (mPath.Count > 0)
                {
                    mCur = mPath[0];
                    mPath.RemoveAt(0);
                    MoveToNextNode();
                }
                else
                {
                    this.Velocity = Vector3.Zero;

                    if (!mPatrolling)
                    {
                        SetIdle();
                    }
                    else
                    {
                        mPatrolReady = true;
                        mCur = mLastPatrolNode;
                    }
                }
            }
        }

        protected void Chase()
        {
            if ((mTarget.Position - mEnd.Position).Length() > PlayerProperties.MoveTolerance)
            {
                bool pathChanged = Node.ChangePath(mTarget.Position, mCur, mEnd, ref mPath);
                
                if (!pathChanged)
                {
                    StartMoving();
                }
            }

            Move();
        }

        public void Chase1()
        {
            if ((mEnd.Position - this.Position).Length() < 0.3f)
            {
                mStart.Position = this.Position;
                mEnd.Position = mTarget.Position;
                Node.NodeListToUse = MyScene.Nodes;
                Node n = Node.FindNextNodeToward(mStart, mEnd);
                mEnd.Position = n.Position;
            }
            else
            {
                this.Velocity = mEnd.Position - this.Position;

                if (this.Velocity.Length() > 0.0f)
                {
                    this.Velocity.Normalize();
                }

                this.Velocity *= mSpeed * 1.5f;
            }
        }

        public void Run()
        {
            if ((mEnd.Position - this.Position).Length() < PlayerProperties.MoveTolerance)
            {
                //this.Velocity = Vector3.Zero;

                //SetIdle();

                mStart.Position = this.Position;
                mEnd.Position = mTarget.Position;
                Node.NodeListToUse = MyScene.Nodes;
                Node n = Node.FindNextNodeAway(mStart, mEnd);
                mEnd.Position = n.Position;
            }
            if ((mTarget.Position - this.Position).Length() < 5.0f)
            {
                mStart.Position = this.Position;
                mEnd.Position = mTarget.Position;
                Node.NodeListToUse = MyScene.Nodes;
                Node n = Node.FindNextNodeAway(mStart, mEnd);
                mEnd.Position = n.Position;
            }
            else
            {
                this.Velocity = mEnd.Position - this.Position;

                if (this.Velocity.Length() > 0.0f)
                {
                    this.Velocity.Normalize();
                }

                this.Velocity *= mSpeed;
            }
        }

        private double mStartSentry;
        private double mSentryLimit;
        private void Sentry()
        {
            this.Velocity.X = 0.0f;
            this.Velocity.Y = 0.0f;

            if (TimeManager.CurrentTime - mStartSentry > mSentryLimit)
            {
                mStartSentry = TimeManager.CurrentTime;
                mFacingRight = !mFacingRight;
            }
        }

        public void Patrol()
        {
            if (mPatrolReady)
            {
                if (mPatrolPath.Count > 1)
                {
                    if ((mCur.Position - this.Position).Length() < PlayerProperties.MoveTolerance)
                    {
                        GetNextPatrolNode();
                        MoveToNextNode();
                    }
                }
                else
                {
                    Sentry();
                }
            }
            else
            {
                Move();
            }
        }

        protected abstract void SetIdle();

        protected void MoveInVec(Vector3 vec)
        {
            this.Velocity = vec * mSpeed;
        }

        public virtual void Destroy()
        {
            base.Destroy();

            mTarget = null;
            mPath.Clear();
        }
    }
}
