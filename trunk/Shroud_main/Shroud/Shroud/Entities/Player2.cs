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
    public class Player2 : AttackingEntity
    {
        #region Fields

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

        // Variable(s) needed for PLACING/RETRIEVING TRAPS
        /*private List<Trap> mTraps;
        private TrapType mTrapSelected;
        private TrapType mTrap1;
        private TrapType mTrap2;*/

        private class Flash : Entity
        {
            private double cooldownTime;
            private double lastUsed;

            public float PercentReady
            {
                get { return (float)((TimeManager.CurrentTime - lastUsed) / cooldownTime); }
            }

            public Flash(string contentManagerName)
                : base(contentManagerName)
            {
                AnimationChain a = new AnimationChain();

                for (int i = 0; i < 7; i++)
                {
                    a.Add(new AnimationFrame(@"Content/Entities/Player/explosion" + i, 0.0415f, ContentManagerName));
                }
                a.Name = "Flash";

                //mAppearance = SpriteManager.AddSprite(@"Content/Entities/Player/Flash", ContentManagerName);
                mAppearance = SpriteManager.AddSprite(a);
                SpriteManager.AddToLayer(mAppearance, CameraManager.Entity1);
                mAppearance.AttachTo(this, false);
                GameProperties.RescaleSprite(mAppearance);
                mAppearance.RelativeRotationZ = GameProperties.WorldRotation;
                mAppearance.RelativeX = mAppearance.ScaleX;
                mAppearance.Visible = false;
                mAppearance.Animate = false;

                mCollision = ShapeManager.AddCircle();
                mCollision.Radius = mAppearance.ScaleX;
                mCollision.AttachTo(this, false);
                mCollision.RelativeX = mAppearance.ScaleX;

                cooldownTime = 3.0f;
                lastUsed = TimeManager.CurrentTime - 3.0f;
            }

            public void Pop(float x, float y, float z)
            {
                if (TimeManager.CurrentTime - lastUsed > cooldownTime)
                {
                    mAppearance.Alpha = 1.0f;
                    this.X = x;
                    this.Y = y;
                    this.Z = z;
                    mAppearance.Visible = true;
                    mAppearance.Animate = true;
                    mAppearance.AlphaRate = -3.0f;
                    lastUsed = TimeManager.CurrentTime;
                }
            }
        }

        private Flash mFlash;

        private class LeaveButton : PositionedObject
        {
            private Sprite mAppearance;
            private AxisAlignedRectangle mCollision;
            private int mDirection;
            public Node MoveTo;

            public AxisAlignedRectangle Collision
            {
                get { return mCollision; }
            }

            public bool Active
            {
                get { return mAppearance.Visible; }
                set { mAppearance.Visible = value; }
            }

            public LeaveButton(string contentManagerName)
                : base()
            {
                SpriteManager.AddPositionedObject(this);

                mAppearance = SpriteManager.AddSprite(@"Content/Entities/Player/advanceArrowBtn", contentManagerName, CameraManager.UI);
                mAppearance.AttachTo(this, false);
                GameProperties.RescaleSprite(mAppearance);
                mAppearance.RelativeRotationZ = GameProperties.WorldRotation;
                mDirection = 0;

                mCollision = ShapeManager.AddAxisAlignedRectangle();
                mCollision.ScaleX = mAppearance.ScaleY;
                mCollision.ScaleY = mAppearance.ScaleX;
                mCollision.AttachTo(this, false);

                MoveTo = null;
            }

            public void OrientUp()
            {
                mAppearance.RelativeRotationZ = 0.0f;
                mDirection = 3;
            }

            public void OrientDown()
            {
                mAppearance.RelativeRotationZ = (float)Math.PI;
                mDirection = 1;
            }

            public void OrientLeft()
            {
                mAppearance.RelativeRotationZ = (float)Math.PI / 2.0f;
                mDirection = 2;
            }

            public void OrientRight()
            {
                mAppearance.RelativeRotationZ = GameProperties.WorldRotation;
                mDirection = 0;
            }

            public int Activate(PositionedObject po, ref Vector3 vec)
            {
                //Node setPos = MoveTo;

                switch (mDirection)
                {
                    case 0:
                        //LevelManager.SceneMoveRight();
                        vec.X = 0.0f;
                        vec.Y = -1.0f;
                        break;
                    case 1:
                        //LevelManager.SceneMoveDown();
                        vec.X = -1.0f;
                        vec.Y = 0.0f;
                        break;
                    case 2:
                        //LevelManager.SceneMoveLeft();
                        vec.X = 0.0f;
                        vec.Y = 1.0f;
                        break;
                    case 3:
                        //LevelManager.SceneMoveUp();
                        vec.X = 1.0f;
                        vec.Y = 0.0f;
                        break;
                }

                //po.Position = setPos.Position;
                //MoveTo = null;

                return mDirection;
            }

            public void Destroy()
            {
                SpriteManager.RemovePositionedObject(this);
                SpriteManager.RemoveSprite(mAppearance);
                ShapeManager.Remove(mCollision);
            }
        }

        private LeaveButton mLB;

        private class FlashTimer : PositionedObject
        {
            private Sprite mOutline;
            private Sprite mColor;
            private Sprite mFuse;

            public FlashTimer(string contentManagerName)
                : base()
            {
                SpriteManager.AddPositionedObject(this);

                mColor = SpriteManager.AddSprite(@"Content/Entities/Player/bomb_color", contentManagerName, CameraManager.UI);
                mColor.AttachTo(this, false);
                GameProperties.RescaleSprite(mColor);
                mColor.RelativeRotationZ = GameProperties.WorldRotation;

                mOutline = SpriteManager.AddSprite(@"Content/Entities/Player/bomb_outline", contentManagerName, CameraManager.UI);
                mOutline.AttachTo(this, false);
                GameProperties.RescaleSprite(mOutline);
                mOutline.RelativeRotationZ = GameProperties.WorldRotation;

                mFuse = SpriteManager.AddSprite(@"Content/Entities/Player/bomb_fuse", contentManagerName, CameraManager.UI);
                mFuse.AttachTo(this, false);
                GameProperties.RescaleSprite(mFuse);
                mFuse.RelativeRotationZ = GameProperties.WorldRotation;
            }

            public void Update(float per)
            {
                if (per >= 1.0f)
                {
                    mColor.Alpha = 1.0f;
                    mFuse.Visible = !mFuse.Visible;
                }
                else
                {
                    mFuse.Visible = false;
                    mColor.Alpha = per;
                }
            }

            public void Destroy()
            {
                SpriteManager.RemovePositionedObject(this);
                SpriteManager.RemoveSprite(mOutline);
                SpriteManager.RemoveSprite(mColor);
                SpriteManager.RemoveSprite(mFuse);
            }
        }

        private FlashTimer mTimer;

        private bool mBusyMoving;
        private Vector3 mMoveVec;
        private int mD;

        private double mStunStart;
        private static double mStunLimit = 1.0;
        private bool mIsStunned;

        private double mDeadStart;
        private static double mDeadTolerance = 1.0;

        #endregion

        #region Properties

        public bool IsHiding
        {
            get { return mCurAnimationState.Equals(AnimationState.Hidden) && !mIsStunned; }
        }

        public bool IsAlive
        {
            get { return !mCurAnimationState.Equals(AnimationState.Dying) &&
                         !mCurAnimationState.Equals(AnimationState.Dead); }
        }

        public bool IsReallyDead
        {
            get { return mCurAnimationState.Equals(AnimationState.Dead) && TimeManager.CurrentTime - mDeadStart > mDeadTolerance; }
        }

        public bool IsStunned
        {
            get { return mIsStunned; }
        }

        /*public TrapType TrapSelected
        {
            set { mTrapSelected = value; }
        }*/

        public float Opacity
        {
            get { return mAppearance.Alpha; }
        }

        #endregion

        #region Methods

        // Constructor
        public Player2(string contentManagerName, float speed)
            : base(contentManagerName, speed, PlayerProperties.WeaponSize, PlayerProperties.WeaponRange)
        {
            Initialize(true);
        }

        protected virtual void Initialize(bool addToManagers)
        {
            /*mTraps = new List<Trap>();
            mTraps.Add(new Trap(ContentManagerName, TrapType.Bomb));
            mTraps.Add(new Trap(ContentManagerName, TrapType.Smoke));
            mTrapSelected = TrapType.Bomb;
            mTrap1 = TrapType.Bomb;
            mTrap2 = TrapType.Smoke;*/

            mFlash = new Flash(ContentManagerName);
            mLB = new LeaveButton(ContentManagerName);
            mLB.AttachTo(this, false);
            mLB.RelativeX = 3.0f;

            mTimer = new FlashTimer(ContentManagerName);

            MyScene = LevelManager.CurrentScene;

            mBusyMoving = false;
            mMoveVec = new Vector3();
            mD = 0;

            mStunStart = TimeManager.CurrentTime;
            mIsStunned = false;

            mDeadStart = TimeManager.CurrentTime;

            if (addToManagers)
            {
                AddToManagers(null);
            }
        }

        public virtual void AddToManagers(Layer layerToAddTo)
        {
            //SpriteManager.AddPositionedObject(this);

            InitializeAnimations();
            InitializeTrapDisplay();

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);
            mCollision.Radius = 3.0f;
        }

        private void InitializeAnimations()
        {
            AnimationChainList animations = new AnimationChainList();
            float frametime = 0.083f;
            int framenum = 0;

            AnimationChain idle = new AnimationChain();
            int idleTotalFrames = 8;
            for (framenum = 0; framenum < idleTotalFrames; framenum++)
            {
                idle.Add(new AnimationFrame(@"Content/Entities/Player/idle" + framenum, frametime, ContentManagerName)); 
            }
            idle.Name = "Idle";
            animations.Add(idle);
            
            AnimationChain hiding = new AnimationChain();
            int hidingTotalFrames = 4;
            for (framenum = 0; framenum < hidingTotalFrames; framenum++)
            {
                hiding.Add(new AnimationFrame(@"Content/Entities/Player/hiding" + framenum, frametime, ContentManagerName)); 
            }
            hiding.Name = "Hiding";
            animations.Add(hiding);

            AnimationChain hide = new AnimationChain();
            int hideTotalFrames = 1;
            for (framenum = 0; framenum < hideTotalFrames; framenum++)
            {
                hide.Add(new AnimationFrame(@"Content/Entities/Player/hide" + framenum, frametime, ContentManagerName)); 
            }
            hide.Name = "Hide";
            animations.Add(hide);

            AnimationChain moving = new AnimationChain();
            int movingTotalFrames = 6;
            for (framenum = 0; framenum < movingTotalFrames; framenum++)
            {
                moving.Add(new AnimationFrame(@"Content/Entities/Player/moving" + framenum, frametime, ContentManagerName)); 
            }
            moving.Name = "Moving";
            animations.Add(moving);
            
            AnimationChain climbing = new AnimationChain();
            int climbingTotalFrames = 6;
            for (framenum = 0; framenum < climbingTotalFrames; framenum++)
            {
                climbing.Add(new AnimationFrame(@"Content/Entities/Player/climb" + framenum, frametime, ContentManagerName)); 
            }
            climbing.Name = "Climbing";
            animations.Add(climbing);
            
            AnimationChain placingtrap = new AnimationChain();
            int placingtrapTotalFrames = 4;
            for (framenum = 0; framenum < placingtrapTotalFrames; framenum++)
            {
                placingtrap.Add(new AnimationFrame(@"Content/Entities/Player/placingtrap" + framenum, frametime, ContentManagerName));
            }
            placingtrap.Name = "PlacingTrap";
            animations.Add(placingtrap);

            AnimationChain attacking = new AnimationChain();
            int attackingTotalFrames = 7;
            for (framenum = 0; framenum < attackingTotalFrames; framenum++)
            {
                attacking.Add(new AnimationFrame(@"Content/Entities/Player/attacking" + framenum, frametime, ContentManagerName));
            }
            attacking.Name = "Attacking";
            animations.Add(attacking);

            AnimationChain stunned = new AnimationChain();
            int stunnedTotalFrames = 1;
            for (framenum = 0; framenum < stunnedTotalFrames; framenum++)
            {
                stunned.Add(new AnimationFrame(@"Content/Entities/Player/stun" + framenum, frametime, ContentManagerName));
            }
            stunned.Name = "Stunned";
            animations.Add(stunned);

            AnimationChain fall = new AnimationChain();
            int fallTotalFrames = 1;
            for (framenum = 0; framenum < fallTotalFrames; framenum++)
            {
                fall.Add(new AnimationFrame(@"Content/Entities/Player/falling" + framenum, frametime, ContentManagerName));
            }
            fall.Name = "Fall";
            animations.Add(fall);

            AnimationChain dying = new AnimationChain();
            int dyingTotalFrames = 2;
            for (framenum = 0; framenum < dyingTotalFrames; framenum++)
            {
                dying.Add(new AnimationFrame(@"Content/Entities/Player/dying" + framenum, frametime, ContentManagerName));
            }
            dying.Name = "Dying";
            animations.Add(dying);

            AnimationChain dead = new AnimationChain();
            int deadTotalFrames = 1;
            for (framenum = 0; framenum < deadTotalFrames; framenum++)
            {
                dead.Add(new AnimationFrame(@"Content/Entities/Player/dead" + framenum, frametime, ContentManagerName));
            }
            dead.Name = "Dead";
            animations.Add(dead);

            mAppearance = SpriteManager.AddSprite(animations);
            SpriteManager.AddToLayer(mAppearance, CameraManager.Entity1);
            mAppearance.AttachTo(this, false);
            GameProperties.RescaleSprite(mAppearance);
            mAppearance.CurrentChainName = "Idle";
            mAppearance.RelativeRotationZ = GameProperties.WorldRotation;
        }

        private void InitializeTrapDisplay()
        {
            /*UIManager.RegisterPressButton(TypeToString(mTrap1), TypeToString(mTrap1), PlaceTrap);
            UIManager.RegisterPressButton(TypeToString(mTrap2), TypeToString(mTrap2), PlaceTrap);
            UIManager.HideButton(TypeToString(mTrap1));
            UIManager.HideButton(TypeToString(mTrap2));*/
        }

        /*private string TypeToString(TrapType t)
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
        }*/

        #region Main Functions

        private void DisplayTraps()
        {
            /*bool shown1 = false;
            bool shown2 = false;

            foreach (Trap t in mTraps)
            {
                if (t.TType == mTrap1 && !shown1)
                {
                    //if (this.X < 2.0f)

                    UIManager.DisplayButton(TypeToString(mTrap1), this.X - SpriteManager.Camera.X, this.Y - SpriteManager.Camera.Y + 4.0f);
                    shown1 = true;
                }
                else if (t.TType == mTrap2 && !shown2)
                {
                    UIManager.DisplayButton(TypeToString(mTrap2), this.X - SpriteManager.Camera.X, this.Y - SpriteManager.Camera.Y - 4.0f);
                    shown2 = true;
                }
            }*/
        }

        private void HideTraps()
        {
            /*UIManager.HideButton(TypeToString(mTrap1));
            UIManager.HideButton(TypeToString(mTrap2));*/
        }

        private void PlaceTrap()
        {
            /*mTrapSelected = UIManager.RETRIEVE_PRESSBUTTON(TypeToString(mTrap1)).Equals(WorldManager.justFired) ? mTrap1 : mTrap2;

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

            mAppearance.CurrentFrameIndex = 0;
            mCurAnimationState = AnimationState.Idle;
            mAppearance.Animate = true;

            HideTraps();*/
        }

        private void RetrieveTrap(Trap retrievedTrap)
        {
            /*if (retrievedTrap != null)
            {
                retrievedTrap.Deactivate();
                WorldManager.ManagedTraps.Remove(retrievedTrap);
                mTraps.Add(retrievedTrap);
            }*/
        }

        public void Die()
        {
            if (!mCurAnimationState.Equals(AnimationState.Dead))
            {
                mCurAnimationState = AnimationState.Dying;
                mIsStunned = false;
                mAppearance.Animate = true;
                mAppearance.Alpha = 1.0f;
                mAppearance.AlphaRate = 0.0f;
            }

            GameProperties.NoDieBadge = false;
        }

        public void Stunned()
        {
            if (this.IsAlive)
            {
                mStunStart = TimeManager.CurrentTime;
                mIsStunned = true;

                this.Velocity.X = 0.0f;
                this.Velocity.Y = 0.0f;
                this.Velocity.Z = 0.0f;

                mAppearance.Animate = true;
                mCurAnimationState = AnimationState.Idle;
                mAppearance.Alpha = 1.0f;
                mAppearance.AlphaRate = 0.0f;
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

        #endregion

        #region Helper Functions

        #region Movement

        protected override void SetIdle()
        {
            mCurAnimationState = AnimationState.Idle;
        }

        #endregion

        #region Animations

        private void SetAnimation()
        {
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
                case AnimationState.Hidden:
                    mAppearance.CurrentChainName = "Hide";
                    break;
                case AnimationState.Hiding:
                    mAppearance.CurrentChainName = "Hiding";
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

        #region Input Behaviors

        private void TapBehavior()
        {
            mTarget = WorldManager.InteractTarget;

            if (mLB.Collision.IsPointInside(GestureManager.EndTouchWorld.X, GestureManager.EndTouchWorld.Y) && mLB.Active)
            {
                //LevelManager.SceneMoveRight();
                //this.Position = LevelManager.CurrentScene.Nodes[0].Position;
                mD = mLB.Activate(this, ref mMoveVec);
                mBusyMoving = true;
                mCurAnimationState = AnimationState.Moving;
            }
            else if (WorldManager.InteractTarget == null)
            {
                StartMoving();
                mCurAnimationState = AnimationState.Moving;
            }
            else if (WorldManager.InteractTarget.GetType().Equals(typeof(Button)))
            {

            }
            else if (WorldManager.InteractTarget.GetType().Equals(typeof(Player2)))
            {
                if (!mAppearance.CurrentChainName.Equals("Climbing"))
                {
                    if (mCurAnimationState != AnimationState.Hidden)
                        mCurAnimationState = AnimationState.Hiding;
                    else
                        mCurAnimationState = AnimationState.Idle;
                }
            }
            else if (WorldManager.InteractTarget.GetType().Equals(typeof(Soldier)) || 
                     WorldManager.InteractTarget.GetType().Equals(typeof(Noble)) ||
                     WorldManager.InteractTarget.GetType().Equals(typeof(Ninja)))
            {
                if (!mAppearance.CurrentChainName.Equals("Climbing"))
                {
                    float xDiff = mTarget.X - this.X;
                    float yDiff = mTarget.Y - this.Y;

                    //System.Diagnostics.Debug.WriteLine(xDiff + ", " + yDiff);

                    Soldier e = null;
                    Noble b = null;
                    Ninja c = null;
                    bool climbing = false;

                    if (WorldManager.InteractTarget.GetType().Equals(typeof(Soldier)))
                    {
                        e = (Soldier)WorldManager.InteractTarget;
                        climbing = e.IsClimbing;
                    }
                    else if (WorldManager.InteractTarget.GetType().Equals(typeof(Noble)))
                    {
                        b = (Noble)WorldManager.InteractTarget;
                        climbing = b.IsClimbing;
                    }
                    else if (WorldManager.InteractTarget.GetType().Equals(typeof(Ninja)))
                    {
                        c = (Ninja)WorldManager.InteractTarget;
                        climbing = c.IsClimbing;
                    }

                    if (!climbing)
                    {
                        if (Math.Abs(yDiff) < 10.0f && Math.Abs(xDiff) < 1.0f)
                        {
                            if (yDiff > 0)
                            {
                                this.Y = mTarget.Y - PlayerProperties.WeaponRange;
                                mFacingRight = true;
                            }
                            else
                            {
                                this.Y = mTarget.Y + PlayerProperties.WeaponRange;
                                mFacingRight = false;
                            }
                            //this.X = mTarget.X;
                            mCurAnimationState = AnimationState.Attacking;
                        }
                        else
                        {
                            StartMoving();
                            mCurAnimationState = AnimationState.Moving;
                        }
                    }
                    else
                    {
                        if (Math.Abs(yDiff) < 10.0f && Math.Abs(xDiff) < 7.0f)
                        {
                            this.Y = mTarget.Y;
                            this.X = mTarget.X;
                            mAppearance.CurrentChainName = "Climbing";
                            mCurAnimationState = AnimationState.Idle;

                            if (e != null)
                                e.Fall();
                            else if (b != null)
                                b.Fall();
                            else if (c != null)
                                c.Fall();
                        }
                        else
                        {
                            StartMoving();
                            mCurAnimationState = AnimationState.Moving;
                        }
                    }
                }
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
                if (mAppearance.CurrentChainName != "Climbing")
                {
                    mCurAnimationState = AnimationState.Idle;
                    mAppearance.Animate = true;
                }
            }
            else if (WorldManager.InteractTarget.GetType().Equals(typeof(Soldier)))
            {
                //StartMoving();
                //mCurAnimationState = AnimationState.Chasing;

                /*if ((mTarget.Position - this.Position).Length() < PlayerProperties.WeaponRange * 2)
                {
                    this.Y = mTarget.Y;
                    this.X = mTarget.X;
                    mCurAnimationState = AnimationState.Attacking;
                }*/

                mAppearance.Animate = true;
            }
            else
            {
                //StartMoving();
                //mCurAnimationState = AnimationState.Moving;
                //mAppearance.Animate = true;
            }
        }

        private void SwipeDownBehavior()
        {
            mTarget = WorldManager.InteractTarget;

            if (WorldManager.InteractTarget != null && WorldManager.InteractTarget.GetType().Equals(typeof(Player2)))
            {
                /*if (mTraps.Count > 0)
                {
                    mCurAnimationState = AnimationState.PlacingTrap;
                }*/

                if (mAppearance.CurrentChainName != "Climbing")
                {
                    mFlash.Pop(this.X - mAppearance.ScaleX, this.Y, this.Z + 0.1f);

                    foreach (Soldier s in WorldManager.Soldiers)
                    {
                        if (mFlash.Collision.CollideAgainst(s.Collision))
                        {
                            s.Stunned();
                        }
                    }

                    foreach (Ninja n in WorldManager.Ninjas)
                    {
                        if (mFlash.Collision.CollideAgainst(n.Collision))
                        {
                            n.Stunned();
                        }
                    }

                    if (mFlash.Collision.CollideAgainst(WorldManager.Target.Collision))
                    {
                        WorldManager.Target.Stunned();
                    }

                    mAppearance.Animate = true;
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

            if (WorldManager.InteractTarget != null && WorldManager.InteractTarget.GetType().Equals(typeof(Player2)))
            {
                if (mAppearance.CurrentChainName != "Climbing")
                {
                    //mCurAnimationState = AnimationState.Hiding;
                    /*if (mTraps.Count > 0)
                    {
                        mCurAnimationState = AnimationState.PlacingTrap;
                    }*/

                    //this.Velocity += Vector3.Normalize(GestureManager.EndTouchWorld - GestureManager.StartTouchWorld) * 15.0f;
                    //this.Velocity.Normalize();
                    //this.Velocity *= 10.0f;
                    //this.Acceleration.X = -20.0f;

                    //mCurAnimationState = AnimationState.Jumping;

                    mAppearance.Animate = true;
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
            if (mAppearance.CurrentChainName == "Fall")
            {
                return;
            }

            this.Velocity = Vector3.Zero;

            switch (GestureManager.CurGesture)
            {
                case Gesture.Tap:
                    TapBehavior();
                    mAppearance.Animate = true;
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
            if (!mBusyMoving)
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
            else
            {
                MoveInVec(mMoveVec);

                float xDiff = this.X - LevelManager.CurrentScene.WorldAnchor.X;
                float yDiff = this.Y - LevelManager.CurrentScene.WorldAnchor.Y;

                if (Math.Abs(xDiff) > 10.0f || Math.Abs(yDiff) > 18.0f)
                {
                    Teleport();
                }
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
                    if (mTarget != null)
                    {
                        if (mTarget.GetType().Equals(typeof(Soldier)))
                        {
                            if ((mTarget.Position - this.Position).Length() < PlayerProperties.WeaponRange)
                            {
                                mCurAnimationState = AnimationState.Attacking;
                            }
                        }
                    }

                    Chase();
                    break;
            }
        }

        private void PlacingTrapBehavior()
        {
            this.Velocity = Vector3.Zero;

            if (mAppearance.CurrentChainName == "PlacingTrap" && mAppearance.JustCycled)
            {
                //mCurAnimationState = AnimationState.Idle;
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
                switch (GestureManager.CurGesture)
                {
                    case Gesture.Tap:
                        TapBehavior();
                        mAppearance.CurrentFrameIndex = 0;
                        mAppearance.Animate = true;
                        HideTraps();
                        break;
                    case Gesture.Swipe:
                        DragBehavior();
                        mAppearance.CurrentFrameIndex = 0;
                        mAppearance.Animate = true;
                        HideTraps();
                        break;
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
                mAppearance.Alpha = GetStealthOpacity();
            }
            else
            {
                if (GestureManager.CurGesture.Equals(Gesture.Tap))
                {
                    //TapBehavior();
                }
                else if (GestureManager.CurGesture.Equals(Gesture.Swipe))
                {
                    //DragBehavior();
                }
                else
                {
                    // 0.5 is Minimum Transparency, 3.0f is number of frames in Hiding animation
                    mAppearance.AlphaRate = -(GetStealthOpacity() / 3.0f);
                }
            }
        }

        private void HiddenBehavior()
        {
            if (GestureManager.CurGesture.Equals(Gesture.Tap))
            {
                TapBehavior();

                if (!mCurAnimationState.Equals(AnimationState.Hidden))
                {
                    mAppearance.Alpha = 1.0f;
                }
            }
            else if (GestureManager.CurGesture.Equals(Gesture.SwipeDown))
            {
                //SwipeDownBehavior();
                //mAppearance.Alpha = 1.0f;
            }
            else if (GestureManager.CurGesture.Equals(Gesture.Swipe))
            {
                //DragBehavior();
                //mAppearance.Alpha = 1.0f;
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

                if (mTarget == null)
                {

                }
                else if (mTarget.GetType().Equals(typeof(Soldier)))
                {
                    Soldier s = (Soldier)mTarget;

                    if (s.Collision.CollideAgainst(this.mAttackCollision))
                    {
                        s.Die();
                    }
                }
                else if (mTarget.GetType().Equals(typeof(Noble)))
                {
                    Noble s = (Noble)mTarget;

                    if (s.Collision.CollideAgainst(this.mAttackCollision))
                    {
                        s.Die();
                    }
                }
                else if (mTarget.GetType().Equals(typeof(Ninja)))
                {
                    Ninja s = (Ninja)mTarget;

                    if (s.Collision.CollideAgainst(this.mAttackCollision))
                    {
                        s.Die();
                    }
                }
            }
            else
            {
                ResetAttack();
            }
        }

        private void ClimbingBehavior()
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

        private void JumpingBehavior()
        {
            if (this.X < -6.0f)
            {
                this.Velocity.X = 0;
                this.Velocity.Y = 0;
                this.Acceleration.X = 0;
                mCurAnimationState = AnimationState.Idle;
            }
        }

        private void DyingBehavior()
        {
            this.Velocity = Vector3.Zero;
            
            if (mAppearance.CurrentChainName == "Dying" && mAppearance.JustCycled)
            {
                mDeadStart = TimeManager.CurrentTime;
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
            //System.Diagnostics.Debug.WriteLine(GestureManager.CurGesture.ToString());
            System.Diagnostics.Debug.WriteLine(mAppearance.CurrentChainName);
        }

        private void Teleport()
        {
            switch (mD)
            {
                case 0:
                    LevelManager.SceneMoveRight();
                    break;
                case 1:
                    LevelManager.SceneMoveDown();
                    break;
                case 2:
                    LevelManager.SceneMoveLeft();
                    break;
                case 3:
                    LevelManager.SceneMoveUp();
                    break;
            }

            this.Position = mLB.MoveTo.Position;
            mLB.MoveTo = null;
            mBusyMoving = false;

            SetIdle();
        }

        private void ManageMoveArrow()
        {
            float xDiff = this.X - LevelManager.CurrentScene.WorldAnchor.X;
            float yDiff = this.Y - LevelManager.CurrentScene.WorldAnchor.Y;

            Node.NodeListToUse = LevelManager.CurrentScene.Nodes;
            Node n = Node.FindClosestNode(this.Position);

            //System.Diagnostics.Debug.WriteLine(xDiff + " " + yDiff);

            if (n.Link != null)
            {
                mLB.MoveTo = n.Link;

                if (xDiff > 5.0f && LevelManager.CurrentScene.Up != null) // UP
                {
                    mLB.OrientUp();
                    mLB.Active = true;
                }
                else if (xDiff < -5.0f && LevelManager.CurrentScene.Down != null) // DOWN
                {
                    mLB.OrientDown();
                    mLB.Active = true;
                }
                else if (yDiff > 10.0f && LevelManager.CurrentScene.Left != null) // LEFT
                {
                    mLB.OrientLeft();
                    mLB.Active = true;
                }
                else if (yDiff < -10.0f && LevelManager.CurrentScene.Right != null) // RIGHT
                {
                    mLB.OrientRight();
                    mLB.Active = true;
                }
                else
                {
                    mLB.Active = false;
                }
            }
            else
            {
                mLB.Active = false;
                mLB.MoveTo = null;
            }
        }

        private void LadderCheck()
        {
            foreach (Soldier s in WorldManager.Soldiers)
            {
                if (!s.IsFalling && s.IsClimbing && s.Collision.CollideAgainst(this.Collision))
                {
                    Fall();
                }
            }

            foreach (Ninja n in WorldManager.Ninjas)
            {
                if (!n.IsFalling && n.IsClimbing && n.Collision.CollideAgainst(this.Collision))
                {
                    Fall();
                }
            }

            if (!WorldManager.Target.IsFalling && WorldManager.Target.IsClimbing && WorldManager.Target.Collision.CollideAgainst(this.Collision))
            {
                Fall();
            }
        }

        private float GetStealthOpacity()
        {
            Sprite closest = null;
            float cdist = 0.0f, ndist;

            foreach (Sprite s in MyScene.StealthObjects)
            {
                ndist = Vector3.Distance(this.Position, s.Position);

                if (closest == null || ndist < cdist)
                {
                    closest = s;
                    cdist = ndist;
                }
            }

            if (closest != null)
            {
                return MathHelper.Clamp(cdist / 8.0f, 0.0f, 1.0f);
            }
            else
            {
                return 1.0f;
            }
        }

        public virtual void Activity()
        {
            //PrintOutState();
            if (this.IsReallyDead)
            {
                this.Position = Shroud.Utilities.Scene.Find(0, 0, 0).Nodes[0].Position;
                LevelManager.CurrentScene = Shroud.Utilities.Scene.Find(0, 0, 0);
                mCurAnimationState = AnimationState.Idle;
            }

            MyScene = LevelManager.CurrentScene;

            ManageMoveArrow();

            if (mAppearance.CurrentChainName == "Climbing")
            {
                LadderCheck();
            }

            if (!mIsStunned)
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
                        ClimbingBehavior();
                        break;
                    case AnimationState.Jumping:
                        JumpingBehavior();
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
            }

            if (HasAnimationChanged() || true)
            {
                SetAnimation();
            }

            if (this.Velocity.Y > 2.0f)
                mFacingRight = true;
            else if (this.Velocity.Y < -2.0f)
                mFacingRight = false;

            mAppearance.FlipHorizontal = mFacingRight;

            if (TimeManager.CurrentTime - mStunStart > mStunLimit && mIsStunned)
            {
                mIsStunned = false;
            }

            if (mAppearance.CurrentChainName == "Fall" && this.X - mEnd.X < -mAppearance.ScaleX / 2.0f)
            {
                mAppearance.CurrentChainName = "Idle";
                this.X = mEnd.X;
                this.Y = mEnd.Y;
                this.Acceleration.X = 0.0f;
                this.Velocity.X = 0.0f;
            }

            mTimer.Update(mFlash.PercentReady);
            mTimer.X = LevelManager.CurrentScene.WorldAnchor.X + 8.0f;
            mTimer.Y = LevelManager.CurrentScene.WorldAnchor.Y - 11.0f;
        }

        public virtual void Destroy()
        {
            base.Destroy();

            /*foreach (Trap t in mTraps)
            {
                t.Destroy();
            }

            mTraps.Clear();*/

            mLB.Destroy();
            mFlash.Destroy();
            mTimer.Destroy();
        }

        #endregion
    }
}