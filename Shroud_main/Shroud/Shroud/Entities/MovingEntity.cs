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
        protected List<Node> mPath;
        protected Node mStart;
        protected Node mEnd;
        protected Node mCur;
        protected bool mFacingRight;

        protected PositionedObject mTarget;

        protected List<Node> mPatrolPath;
        protected Node mLastPatrolNode;
        protected bool mPatrolling;
        private bool mPatrolReady;

        public enum PatrolMode
        {
            None,
            Sentry,
            Circular,
            Backtrack
        }
        protected PatrolMode mCurPatrolMode;

        protected MovingEntity(string contentManagerName)
            : base(contentManagerName)
        {
            mStart = Node.CreateNode();
            mEnd = Node.CreateNode();
            mPath = new List<Node>();
            mCur = null;
            mFacingRight = false;

            mTarget = null;
            mCurPatrolMode = PatrolMode.None;
            mPatrolPath = null;
            mLastPatrolNode = null;
            mPatrolling = false;
            mPatrolReady = false;
        }

        protected MovingEntity(string contentManagerName, List<Node> patrol)
            : base(contentManagerName)
        {
            mStart = Node.CreateNode();
            mEnd = Node.CreateNode();
            mPath = new List<Node>();
            mCur = null;
            mFacingRight = false;

            mTarget = null;
            mCurPatrolMode = PatrolMode.Backtrack;
            mPatrolPath = patrol;
            mLastPatrolNode = mPatrolPath[0];
            mPatrolling = false;
            mPatrolReady = false;
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

            this.Velocity *= PlayerProperties.MoveSpeed;
        }

        private void GetNextPatrolNode()
        {
            if (mPatrolPath != null && mPatrolPath.Count > 0)
            {
                if (mPatrolPath.Contains(mCur))
                {
                    int index = mPatrolPath.IndexOf(mCur);
                    index++;
                    
                    //THIS IS CIRCULAR BEHAVIOR
                    if (index >= mPatrolPath.Count)
                        index = 0;

                    mCur = mPatrolPath[index];
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

        public void Patrol()
        {
            if (mPatrolReady)
            {
                if ((mCur.Position - this.Position).Length() < PlayerProperties.MoveTolerance)
                {
                    GetNextPatrolNode();
                    MoveToNextNode();
                }
            }
            else
            {
                Move();
            }
        }

        protected abstract void SetIdle();

        public virtual void Destroy()
        {
            base.Destroy();

            mPath.Clear();
        }
    }
}
