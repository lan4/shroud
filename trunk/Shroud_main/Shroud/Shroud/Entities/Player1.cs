using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Math.Geometry;

using Shroud.Utilities;
using Gesture = Shroud.Utilities.GestureManager.Gesture;
using TrapType = Shroud.Entities.Trap.TrapType;

using Microsoft.Xna.Framework;

namespace Shroud.Entities
{
    public class Player1 : PositionedObject
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
            Hiding,
            Hidden,
            Moving,
            Climbing,
            Jumping,
            PlacingTrap,
            Attacking,
            Chasing,
            Dying,
            Dead
        };

        // Variable(s) needed for LIVING ENTITIES
        private AnimationState mCurAnimationState;
        private bool mFacingRight;

        // Variable(s) needed for PLACING/RETRIEVING TRAPS
        private List<Trap> mTraps;
        private TrapType mTrapSelected;
        private TrapType mTrap1;
        private TrapType mTrap2;

        // Variable(s) needed for MOVING and CHASING
        private List<Node> mPath;
        private Node mStart;
        private Node mEnd;
        private Node mCur;

        // Variable(s) needed for ATTACKING
        private Circle mAttackCollision;

        // Variable(s) needed for INTERACTING and ATTACKING
        private PositionedObject mTarget;

        #endregion

        #region Properties

        public Circle Collision
        {
            get { return mCollision; }
        }

        public bool IsHiding
        {
            get { return mCurAnimationState.Equals(AnimationState.Hidden); }
        }

        public bool IsAlive
        {
            get { return mCurAnimationState.Equals(AnimationState.Dying) ||
                         mCurAnimationState.Equals(AnimationState.Dead); }
        }

        public TrapType TrapSelected
        {
            set { mTrapSelected = value; }
        }

        #endregion

        #region Methods

        // Constructor
        public Player1(string contentManagerName)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            Initialize(true);
        }

        protected virtual void Initialize(bool addToManagers)
        {
            mTraps = new List<Trap>();
            mTraps.Add(new Trap(mContentManagerName, TrapType.Bomb));
            mTraps.Add(new Trap(mContentManagerName, TrapType.Smoke));
            mTrapSelected = TrapType.Bomb;
            mTrap1 = TrapType.Bomb;
            mTrap2 = TrapType.Smoke;

            mStart = Node.CreateNode();
            mEnd = Node.CreateNode();
            mPath = new List<Node>();
            mCur = null;

            mFacingRight = true;

            if (addToManagers)
            {
                AddToManagers(null);
            }
        }

        public virtual void AddToManagers(Layer layerToAddTo)
        {
            SpriteManager.AddPositionedObject(this);

            InitializeAnimations();
            InitializeTrapDisplay();

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);
            mCollision.Radius = 2.0f;

            mAttackCollision = ShapeManager.AddCircle();
            mAttackCollision.AttachTo(this, false);
            mAttackCollision.Radius = PlayerProperties.WeaponSize;
        }

        private void InitializeAnimations()
        {
            AnimationChainList animations = new AnimationChainList();
            float frametime = 0.012f;
            int framenum = 0;

            AnimationChain idle = new AnimationChain();
            int idleTotalFrames = 8;
            for (framenum = 0; framenum < idleTotalFrames; framenum++)
            {
                idle.Add(new AnimationFrame(@"Content/Entities/Player/idle" + framenum, frametime, mContentManagerName)); 
            }
            idle.Name = "Idle";
            animations.Add(idle);
            
            AnimationChain hiding = new AnimationChain();
            int hidingTotalFrames = 4;
            for (framenum = 0; framenum < hidingTotalFrames; framenum++)
            {
                hiding.Add(new AnimationFrame(@"Content/Entities/Player/hiding" + framenum, frametime, mContentManagerName)); 
            }
            hiding.Name = "Hiding";
            animations.Add(hiding);

            AnimationChain hide = new AnimationChain();
            int hideTotalFrames = 1;
            for (framenum = 0; framenum < hideTotalFrames; framenum++)
            {
                hide.Add(new AnimationFrame(@"Content/Entities/Player/hide" + framenum, frametime, mContentManagerName)); 
            }
            hide.Name = "Hide";
            animations.Add(hide);

            AnimationChain moving = new AnimationChain();
            int movingTotalFrames = 6;
            for (framenum = 0; framenum < movingTotalFrames; framenum++)
            {
                moving.Add(new AnimationFrame(@"Content/Entities/Player/moving" + framenum, frametime, mContentManagerName)); 
            }
            moving.Name = "Moving";
            animations.Add(moving);
            /*
            AnimationChain climbing = new AnimationChain();
            int climbingTotalFrames = 10;
            for (framenum = 0; framenum < climbingTotalFrames; framenum++)
            {
                climbing.Add(new AnimationFrame(@"Content/Player/climbing" + framenum, frametime, mContentManagerName)); 
            }
            climbing.Name = "Climbing";
            animations.Add(climbing);

            AnimationChain jumping = new AnimationChain();
            int jumpingTotalFrames = 10;
            for (framenum = 0; framenum < jumpingTotalFrames; framenum++)
            {
                jumping.Add(new AnimationFrame(@"Content/Player/jumping" + framenum, frametime, mContentManagerName)); 
            }
            jumping.Name = "Jumping";
            animations.Add(jumping);
            */
            AnimationChain placingtrap = new AnimationChain();
            int placingtrapTotalFrames = 4;
            for (framenum = 0; framenum < placingtrapTotalFrames; framenum++)
            {
                placingtrap.Add(new AnimationFrame(@"Content/Entities/Player/placingtrap" + framenum, frametime, mContentManagerName));
            }
            placingtrap.Name = "PlacingTrap";
            animations.Add(placingtrap);

            AnimationChain attacking = new AnimationChain();
            int attackingTotalFrames = 7;
            for (framenum = 0; framenum < attackingTotalFrames; framenum++)
            {
                attacking.Add(new AnimationFrame(@"Content/Entities/Player/attacking" + framenum, frametime, mContentManagerName));
            }
            attacking.Name = "Attacking";
            animations.Add(attacking);

            AnimationChain dying = new AnimationChain();
            int dyingTotalFrames = 2;
            for (framenum = 0; framenum < dyingTotalFrames; framenum++)
            {
                dying.Add(new AnimationFrame(@"Content/Entities/Player/dying" + framenum, frametime, mContentManagerName));
            }
            dying.Name = "Dying";
            animations.Add(dying);

            AnimationChain dead = new AnimationChain();
            int deadTotalFrames = 1;
            for (framenum = 0; framenum < deadTotalFrames; framenum++)
            {
                dead.Add(new AnimationFrame(@"Content/Entities/Player/dead" + framenum, frametime, mContentManagerName));
            }
            dead.Name = "Dead";
            animations.Add(dead);

            mAppearance = SpriteManager.AddSprite(animations);
            mAppearance.AttachTo(this, false);
            GameProperties.RescaleSprite(mAppearance);
            mAppearance.CurrentChainName = "Idle";
            mAppearance.RelativeRotationZ = GameProperties.WorldRotation;
        }

        private void InitializeTrapDisplay()
        {
            HUDManager.RegisterButton(TypeToString(mTrap1));
            HUDManager.RegisterButton(TypeToString(mTrap2));
            HUDManager.HideButton(TypeToString(mTrap1));
            HUDManager.HideButton(TypeToString(mTrap2));
        }

        private string TypeToString(TrapType t)
        {
            switch (t)
            {
                case TrapType.Bomb:
                    return "Bomb";
                case TrapType.Smoke:
                    return "Smoke";
                default:
                    return "None";
            }
        }

        #region Main Functions

        private void Move()
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
                    mCurAnimationState = AnimationState.Idle;
                }
            }
        }

        private void Chase()
        {
            if ((mTarget.Position - mEnd.Position).Length() > PlayerProperties.MoveTolerance)
            {
                bool pathChanged = Node.ChangePath(mTarget.Position, mCur, mEnd, ref mPath);

                if (!pathChanged)
                {
                    StartMoving();
                }
            }
            else
            {
                Move();
            }
        }

        private void Attack()
        {
            // RelativeY used because screen is sideways in game
            mAttackCollision.RelativeY = PlayerProperties.WeaponRange;
        }

        private void DisplayTraps()
        {
            bool shown1 = false;
            bool shown2 = false;

            foreach (Trap t in mTraps)
            {
                if (t.TType == mTrap1 && !shown1)
                {
                    HUDManager.PlaceButton(TypeToString(mTrap1), this.X - SpriteManager.Camera.X, this.Y - SpriteManager.Camera.Y + 4.0f);
                    HUDManager.ShowButton(TypeToString(mTrap1));
                    shown1 = true;
                }
                else if (t.TType == mTrap2 && !shown2)
                {
                    HUDManager.PlaceButton(TypeToString(mTrap2), this.X - SpriteManager.Camera.X, this.Y - SpriteManager.Camera.Y - 4.0f);
                    HUDManager.ShowButton(TypeToString(mTrap2));
                    shown2 = false;
                }
            }
        }

        private void HideTraps()
        {
            HUDManager.HideButton(TypeToString(mTrap1));
            HUDManager.HideButton(TypeToString(mTrap2));
        }

        private void PlaceTrap()
        {
            Trap placedTrap = null;

            foreach (Trap t in mTraps)
            {
                if (t.TType.Equals(mTrapSelected))
                {
                    placedTrap = t;
                    break;
                }
            }

            if (placedTrap != null)
            {
                placedTrap.Activate();
                placedTrap.Position = this.Position;
                placedTrap.X += placedTrap.YOffset;
                WorldManager.ManagedTraps.Add(placedTrap);
                mTraps.Remove(placedTrap);
            }
        }

        private void RetrieveTrap(Trap retrievedTrap)
        {
            if (retrievedTrap != null)
            {
                retrievedTrap.Deactivate();
                WorldManager.ManagedTraps.Remove(retrievedTrap);
                mTraps.Add(retrievedTrap);
            }
        }

        public void Die()
        {
            mCurAnimationState = AnimationState.Dying;
        }

        private void Interact(PositionedObject p)
        {

        }

        #endregion

        #region Helper Functions

        #region Movement

        // Resets/Sets Variables for NodeManager
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

        private void StartMoving()
        {
            PrepareMovement();

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
                mCurAnimationState = AnimationState.Idle;
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

        #endregion

        #region Animations

        private void SetAnimation()
        {
            switch (mCurAnimationState)
            {
                case AnimationState.Attacking:
                    mAppearance.CurrentChainName = "Attacking";
                    break;
                case AnimationState.Climbing:
                    mAppearance.CurrentChainName = "Climbing";
                    break;
                case AnimationState.Hidden:
                    mAppearance.CurrentChainName = "Hide";
                    break;
                case AnimationState.Hiding:
                    mAppearance.CurrentChainName = "Hiding";
                    break;
                case AnimationState.Idle:
                    mAppearance.CurrentChainName = "Idle";
                    break;
                case AnimationState.Jumping:
                    mAppearance.CurrentChainName = "Jumping";
                    break;
                case AnimationState.Chasing:
                case AnimationState.Moving:
                    mAppearance.CurrentChainName = "Moving";
                    break;
                case AnimationState.PlacingTrap:
                    mAppearance.CurrentChainName = "PlacingTrap";
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

        private bool HasAnimationChanged()
        {
            switch (mCurAnimationState)
            {
                case AnimationState.Attacking:
                    return mAppearance.CurrentChainName != "Attacking";
                case AnimationState.Climbing:
                    return mAppearance.CurrentChainName != "Climbing";
                case AnimationState.Hidden:
                    return mAppearance.CurrentChainName != "Hide";
                case AnimationState.Hiding:
                    return mAppearance.CurrentChainName != "Hiding";
                case AnimationState.Idle:
                    return mAppearance.CurrentChainName != "Idle";
                case AnimationState.Jumping:
                    return mAppearance.CurrentChainName != "Jumping";
                case AnimationState.Chasing:
                case AnimationState.Moving:
                    return mAppearance.CurrentChainName != "Moving";
                case AnimationState.PlacingTrap:
                    return mAppearance.CurrentChainName != "PlacingTrap";
                case AnimationState.Dying:
                    return mAppearance.CurrentChainName != "Dying";
                case AnimationState.Dead:
                    return mAppearance.CurrentChainName != "Dead";
                default:
                    System.Diagnostics.Debug.WriteLine("Error: Player AnimationState not valid");
                    return false;
            }
        }

        #endregion

        #region Attacking

        private void ResetAttack()
        {
            // RelativeY used because screen is sideways in game
            mAttackCollision.RelativeY = 0.0f;
        }

        #endregion

        #region Input Behaviors

        private void TapBehavior()
        {
            mTarget = WorldManager.InteractTarget;

            if (WorldManager.InteractTarget == null)
            {
                StartMoving();
                mCurAnimationState = AnimationState.Moving;
            }
            else if (WorldManager.InteractTarget.GetType().Equals(typeof(Button)))
            {

            }
            else if (WorldManager.InteractTarget.GetType().Equals(typeof(Player1)))
            {
                mCurAnimationState = AnimationState.Hiding;
            }
            else
            {
                StartMoving();
                mCurAnimationState = AnimationState.Moving;
            }
        }

        private void DragBehavior()
        {
            mTarget = WorldManager.InteractTarget;

            if (WorldManager.InteractTarget == null)
            {
                mCurAnimationState = AnimationState.Idle;
            }
            /*else if (WorldManager.InteractTarget.GetType().Equals(typeof(Enemy1)))
            {
                StartMoving();
                mCurAnimationState = AnimationState.Chasing;
            }*/
            else
            {
                StartMoving();
                mCurAnimationState = AnimationState.Moving;
            }
        }

        private void SwipeDownBehavior()
        {
            mTarget = WorldManager.InteractTarget;

            if (WorldManager.InteractTarget != null && WorldManager.InteractTarget.GetType().Equals(typeof(Player1)))
            {
                if (mTraps.Count > 0)
                {
                    mCurAnimationState = AnimationState.PlacingTrap;
                }
            }
            else
            {
                DragBehavior();
            }
        }

        private void SwipeUpBehavior()
        {
            mTarget = WorldManager.InteractTarget;

            if (WorldManager.InteractTarget != null && WorldManager.InteractTarget.GetType().Equals(typeof(Player1)))
            {
                //mCurAnimationState = AnimationState.Hiding;
                if (mTraps.Count > 0)
                {
                    mCurAnimationState = AnimationState.PlacingTrap;
                }
            }
            else
            {
                DragBehavior();
            }
        }

        #endregion

        #region States

        private void IdleBehavior()
        {
            this.Velocity = Vector3.Zero;

            switch (GestureManager.CurGesture)
            {
                case Gesture.Tap:
                    TapBehavior();
                    break;
                case Gesture.SwipeUp:
                    SwipeUpBehavior();
                    break;
                case Gesture.SwipeDown:
                    SwipeDownBehavior();
                    break;
                case Gesture.Swipe:
                case Gesture.SwipeLeft:
                case Gesture.SwipeRight:
                    DragBehavior();
                    break;
            }
        }

        private void MovingBehavior()
        {
            switch (GestureManager.CurGesture)
            {
                case Gesture.Tap:
                    TapBehavior();
                    break;
                case Gesture.SwipeUp:
                    SwipeUpBehavior();
                    break;
                case Gesture.SwipeDown:
                    SwipeDownBehavior();
                    break;
                case Gesture.Swipe:
                case Gesture.SwipeLeft:
                case Gesture.SwipeRight:
                    DragBehavior();
                    break;
                default:
                    if (mTarget != null)
                    {
                        if (mTarget.GetType().Equals(typeof(Trap)))
                        {
                            Trap t = (Trap)mTarget;

                            if (t.Collision.CollideAgainst(this.Collision))
                            {
                                RetrieveTrap(t);
                                mTarget = null;
                            }
                        }
                    }

                    Move();
                    break;
            }
        }

        private void ChasingBehavior()
        {
            switch (GestureManager.CurGesture)
            {
                case Gesture.Tap:
                    TapBehavior();
                    break;
                case Gesture.SwipeUp:
                    SwipeUpBehavior();
                    break;
                case Gesture.SwipeDown:
                    SwipeDownBehavior();
                    break;
                case Gesture.Swipe:
                case Gesture.SwipeLeft:
                case Gesture.SwipeRight:
                    DragBehavior();
                    break;
                default:
                    Chase();
                    break;
            }
        }

        private void PlacingTrapBehavior()
        {
            this.Velocity = Vector3.Zero;

            if (mAppearance.CurrentChainName == "PlacingTrap" && mAppearance.CurrentChainIndex == mAppearance.CurrentChain.Count - 1)
            {
                mCurAnimationState = AnimationState.Idle;
            }
            else if (mAppearance.CurrentFrameIndex == 1)
            {
                // If Placing Trap deemed interruptible place this in a nested if statement
                mAppearance.Animate = false;
                mAppearance.CurrentFrameIndex = 2;
                DisplayTraps();
            }
            else if (mAppearance.CurrentFrameIndex == 2)
            {
                if (HUDManager.IsButtonDown(TypeToString(mTrap1)))
                {
                    mTrapSelected = mTrap1;
                    PlaceTrap();
                    mAppearance.Animate = true;
                    HideTraps();
                }
                else if (HUDManager.IsButtonDown(TypeToString(mTrap2)))
                {
                    mTrapSelected = mTrap2;
                    PlaceTrap();
                    mAppearance.Animate = true;
                    HideTraps();
                }
                else
                {
                    switch (GestureManager.CurGesture)
                    {
                        case Gesture.Tap:
                            TapBehavior();
                            mAppearance.Animate = true;
                            HideTraps();
                            break;
                        case Gesture.Swipe:
                            DragBehavior();
                            mAppearance.Animate = true;
                            HideTraps();
                            break;
                    }
                }
            }
        }

        private void HidingBehavior()
        {
            this.Velocity = Vector3.Zero;

            if (mAppearance.CurrentChainName == "Hiding" && mAppearance.JustCycled)
            {
                mCurAnimationState = AnimationState.Hidden;
                mAppearance.AlphaRate = 0.0f;
                mAppearance.Alpha = 0.5f;
            }
            else
            {
                if (GestureManager.CurGesture.Equals(Gesture.Tap))
                {
                    TapBehavior();
                }
                else if (GestureManager.CurGesture.Equals(Gesture.Swipe))
                {
                    DragBehavior();
                }
                else
                {
                    // 0.5 is Minimum Transparency, 3.0f is number of frames in Hiding animation
                    mAppearance.AlphaRate = -(0.5f / 3.0f);
                }
            }
        }

        private void HiddenBehavior()
        {
            if (GestureManager.CurGesture.Equals(Gesture.Tap))
            {
                TapBehavior();
                mAppearance.Alpha = 1.0f;
            }
            else if (GestureManager.CurGesture.Equals(Gesture.SwipeDown))
            {
                SwipeDownBehavior();
                mAppearance.Alpha = 1.0f;
            }
            else if (GestureManager.CurGesture.Equals(Gesture.Swipe))
            {
                DragBehavior();
                mAppearance.Alpha = 1.0f;
            }
        }

        private void AttackingBehavior()
        {
            this.Velocity = Vector3.Zero;

            if (mAppearance.CurrentChainName == "Attacking" && mAppearance.JustCycled)
            {
                ResetAttack();
                mCurAnimationState = AnimationState.Idle;
            }
            else if (mAppearance.CurrentFrameIndex > 1 || mAppearance.CurrentFrameIndex < 4)
            {
                Attack();
            }
        }

        private void ClimbingJumpingBehavior()
        {
            // UNUSED FOR NOW
            // Wait till finished to switch to Moving
            if (GestureManager.CurGesture.Equals(Gesture.Tap))
            {
                StartMoving();
            }
            else if (GestureManager.CurGesture.Equals(Gesture.Swipe))
            {
                StartMoving();
            }
        }

        private void DyingBehavior()
        {
            this.Velocity = Vector3.Zero;

            if (mAppearance.CurrentChainName == "Dying" && mAppearance.JustCycled)
            {
                mCurAnimationState = AnimationState.Dead;
            }
        }

        private void DeadBehavior()
        {
            if (GestureManager.CurGesture.Equals(Gesture.Tap))
            {
                // Pop up menu
            }
        }

        #endregion

        #endregion

        private void PrintOutState()
        {
            //System.Diagnostics.Debug.WriteLine(mCurAnimationState.ToString() + ", " + ((mTarget == null)? "null" : mTarget.GetType().ToString()));
            System.Diagnostics.Debug.WriteLine(GestureManager.CurGesture.ToString());
        }

        public virtual void Activity()
        {
            //PrintOutState();
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
                case AnimationState.PlacingTrap:
                    PlacingTrapBehavior();
                    break;
                case AnimationState.Hiding:
                    HidingBehavior();
                    break;
                case AnimationState.Hidden:
                    HiddenBehavior();
                    break;
                case AnimationState.Attacking:
                    AttackingBehavior();
                    break;
                case AnimationState.Climbing:
                case AnimationState.Jumping:
                    ClimbingJumpingBehavior();
                    break;
                case AnimationState.Dying:
                    DyingBehavior(); 
                    break;
                case AnimationState.Dead:
                    DeadBehavior();
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Error: Player AnimationState invalid");
                    break;
            }

            if (HasAnimationChanged())
            {
                SetAnimation();
            }

            if (Math.Abs(this.Velocity.Y) > 0.01f)
                mAppearance.FlipHorizontal = this.Velocity.Y > 0;
        }

        public virtual void Destroy()
        {
            // Remove self from the SpriteManager:
            SpriteManager.RemovePositionedObject(this);

            // Remove any other objects you've created:
            SpriteManager.RemoveSprite(mAppearance);
            ShapeManager.Remove(mCollision);
            ShapeManager.Remove(mAttackCollision);

            foreach (Trap t in mTraps)
            {
                t.Destroy();
            }

            mTraps.Clear();
            mPath.Clear();
        }

        #endregion
    }
}