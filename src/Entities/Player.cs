using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Input;
using FlatRedBall.Graphics.Animation;

using Microsoft.Xna.Framework;

using Shroud.Utilities;
using Gesture = Shroud.Utilities.GestureManager.Gesture;
using InputState = Shroud.Utilities.GestureManager.InputState;
using OType = Shroud.Entities.Obstacle.OType;

namespace Shroud.Entities
{
    public class Player : PositionedObject
    {
        #region Fields

        // Here you'd define things that your Entity contains, like Sprites
        // or Circles:
        private Sprite mVisibleRepresentation;
        private Sprite mArms;
        private Sprite mBody;
        private Sprite mHead;
        private Circle mCollision;

        // Keep the ContentManager for easy access:
        string mContentManagerName;

        List<Projectile> mQuiver;

        private bool mIsBowDrawn;
        private bool mIsMoving;
        private bool mIsShooting;
        private bool mGoingToMove;
        private bool mIsFacingLeft;
        private bool mAlive;
        private bool mIsHiding;

        private Vector3 mPullVector;

        private HUD mPlayerHUD;

        private List<Node> mPath;
        private int mNextIndex;

        public Node StartNode;
        public Node EndNode;

        private Obstacle mInteractObstacle;

        #endregion

        #region Properties


        // Here you'd define properties for things
        // you want to give other Entities and game code
        // access to, like your Collision property:
        //public Circle Collision
        //{
        //    get { return mCollision; }
        //}

        public bool IsBowDrawn
        {
            get { return mIsBowDrawn; }
        }

        public bool Alive
        {
            get { return mAlive; }
        }

        public bool IsHiding
        {
            get { return mIsHiding; }
        }

        #endregion

        #region Methods

        // Constructor
        public Player(string contentManagerName)
        {
            // Set the ContentManagerName and call Initialize:
            mContentManagerName = contentManagerName;

            mQuiver = new List<Projectile>();

            mPullVector = new Vector3();

            mIsBowDrawn = false;
            mIsMoving = false;
            mIsShooting = false;
            mGoingToMove = false;
            mIsFacingLeft = false;
            mAlive = true;
            mIsHiding = false;

            mPath = new List<Node>();
            mNextIndex = 0;

            StartNode = new Node();
            EndNode = new Node();

            // If you don't want to add to managers, make an overriding constructor
            Initialize(true);
        }

        protected virtual void Initialize(bool addToManagers)
        {
            // Here you can preload any content you will be using
            // like .scnx files or texture files.

            InitializeQuiver();

            if (addToManagers)
            {
                AddToManagers(null);
            }
        }

        private void InitializeQuiver()
        {
            for (int i = 0; i < 10; i++)
            {
                mQuiver.Add(new Projectile(mContentManagerName));
            }
        }

        public virtual void AddToManagers(Layer layerToAddTo)
        {
            SpriteManager.AddPositionedObject(this);

            //mVisibleRepresentation = SpriteManager.AddSprite(@"Content\Entities\Player\TEMP_ninja", mContentManagerName);
            //mVisibleRepresentation.AttachTo(this, false);
            //mVisibleRepresentation.RelativeRotationZ = GameProperties.WorldRotation;

            InitializeAnimation();

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);
            mCollision.Radius = 3.0f;
            CollisionManager.PlayerCollision = mCollision;

            mPlayerHUD = new HUD(mContentManagerName);
            mPlayerHUD.AttachTo(SpriteManager.Camera, false);
            mPlayerHUD.RelativeZ = -30.0f;
        }

        private void InitializeAnimation()
        {
            AnimationChainList fullBodyAnim = new AnimationChainList();

            AnimationChain sneaking = new AnimationChain();
            
            sneaking.Add(new AnimationFrame(@"Content/Entities/Player/shroud_sneak0001", 0.0833f, "Global"));
            sneaking.Add(new AnimationFrame(@"Content/Entities/Player/shroud_sneak0002", 0.0833f, "Global"));
            sneaking.Add(new AnimationFrame(@"Content/Entities/Player/shroud_sneak0003", 0.0833f, "Global"));
            sneaking.Add(new AnimationFrame(@"Content/Entities/Player/shroud_sneak0004", 0.0833f, "Global"));
            sneaking.Add(new AnimationFrame(@"Content/Entities/Player/shroud_sneak0005", 0.0833f, "Global"));
            sneaking.Add(new AnimationFrame(@"Content/Entities/Player/shroud_sneak0006", 0.0833f, "Global"));
            sneaking.Add(new AnimationFrame(@"Content/Entities/Player/shroud_sneak0007", 0.0833f, "Global"));
            sneaking.Add(new AnimationFrame(@"Content/Entities/Player/shroud_sneak0008", 0.0833f, "Global"));
            sneaking.Add(new AnimationFrame(@"Content/Entities/Player/shroud_sneak0009", 0.0833f, "Global"));
            sneaking.Add(new AnimationFrame(@"Content/Entities/Player/shroud_sneak0010", 0.0833f, "Global"));
            sneaking.Add(new AnimationFrame(@"Content/Entities/Player/shroud_sneak0011", 0.0833f, "Global"));
            sneaking.Add(new AnimationFrame(@"Content/Entities/Player/shroud_sneak0012", 0.0833f, "Global"));
            sneaking.Name = "SNEAK";

            AnimationChain bowDraw = new AnimationChain();

            bowDraw.Add(new AnimationFrame(@"Content/Entities/Player/bowpull0001", 0.0833f, "Global"));
            bowDraw.Add(new AnimationFrame(@"Content/Entities/Player/bowpull0002", 0.0833f, "Global"));
            bowDraw.Add(new AnimationFrame(@"Content/Entities/Player/bowpull0003", 0.0833f, "Global"));
            bowDraw.Add(new AnimationFrame(@"Content/Entities/Player/bowpull0004", 0.0833f, "Global"));
            bowDraw.Add(new AnimationFrame(@"Content/Entities/Player/bowpull0005", 0.0833f, "Global"));
            bowDraw.Add(new AnimationFrame(@"Content/Entities/Player/bowpull0006", 0.0833f, "Global"));
            bowDraw.Add(new AnimationFrame(@"Content/Entities/Player/bowpull0007", 0.0833f, "Global"));
            bowDraw.Add(new AnimationFrame(@"Content/Entities/Player/bowpull0008", 0.0833f, "Global"));
            bowDraw.Name = "DRAW_BOW";

            AnimationChain idle = new AnimationChain();

            idle.Add(new AnimationFrame(@"Content/Entities/Player/shroudstill", 0.0833f, "Global"));
            idle.Name = "IDLE";

            fullBodyAnim.Add(idle);
            fullBodyAnim.Add(bowDraw);
            fullBodyAnim.Add(sneaking);
            /*
            AnimationChainList bodyAnim = new AnimationChainList();
            
            AnimationChain standing = new AnimationChain();

            standing.Add(new AnimationFrame(@"Content/Entities/Player/shroud_still_with_bow", 0.0833f, "Global"));
            standing.Name = "STAND";

            bodyAnim.Add(standing);

            AnimationChainList armsAnim = new AnimationChainList();

            AnimationChain bowPull = new AnimationChain();

            bowPull.Add(new AnimationFrame(@"Content/Entities/Player/bow_ready_for_release", 0.0833f, "Global"));
            bowPull.Name = "PULL_BOW";

            armsAnim.Add(bowPull);
            */
            mVisibleRepresentation = SpriteManager.AddSprite(fullBodyAnim);
            mVisibleRepresentation.AttachTo(this, false);
            mVisibleRepresentation.RelativeRotationZ = GameProperties.WorldRotation;
            mVisibleRepresentation.CurrentChainName = "IDLE";

            GameProperties.RescaleSprite(mVisibleRepresentation);

            mArms = SpriteManager.AddSprite(@"Content/Entities/Player/bow_ready_for_release", "Global");
            mArms.AttachTo(this, false);
            mArms.RelativeRotationZ = GameProperties.WorldRotation;
            mArms.Visible = false;
            //mArms.FlipHorizontal = true;
            mArms.RelativeX = 1.4f;

            GameProperties.RescaleSprite(mArms);

            mBody = SpriteManager.AddSprite(@"Content/Entities/Player/shroud_still_with_bow", "Global");
            mBody.AttachTo(this, false);
            mBody.RelativeRotationZ = GameProperties.WorldRotation;
            mBody.Visible = false;

            GameProperties.RescaleSprite(mBody);

            mHead = SpriteManager.AddSprite(@"Content/Entities/Player/shroud_head", "Global");
            mHead.AttachTo(this, false);
            mHead.RelativeRotationZ = GameProperties.WorldRotation;
            mHead.Visible = false;
            mHead.RelativeX = 2.25f;
            //mHead.FlipHorizontal = true;

            GameProperties.RescaleSprite(mHead);
        }

        private void StartMoving()
        {
            mPath.Clear();

            StartNode.Initialize();
            StartNode.SetPosition(this.X, this.Y, this.Z);

            EndNode.Initialize();

            if (mInteractObstacle != null && mInteractObstacle.myType.Equals(OType.Hide))
            {
                EndNode.SetPosition(mInteractObstacle.InteractPoint.X,
                                    mInteractObstacle.InteractPoint.Y,
                                    mInteractObstacle.InteractPoint.Z);
            }
            else
            {
                EndNode.SetPosition(GestureManager.TapPoint.X,
                                    GestureManager.TapPoint.Y,
                                    GestureManager.TapPoint.Z);
            }


            NodeManager.FindPathTo(StartNode,
                                   EndNode,
                                   ref mPath);

            this.Velocity = Vector3.Zero;

            /*
            foreach (Node n in mPath)
            {
                System.Diagnostics.Debug.WriteLine(n.Position);
            }
             */

            mNextIndex = 0;

            mIsMoving = true;
        }

        private void Move()
        {
            if (mNextIndex < mPath.Count)
            {
                if ((mPath[mNextIndex].Position - this.Position).Length() < PlayerProperties.MoveTolerance)
                {
                    mNextIndex++;

                    if (mNextIndex < mPath.Count)
                    {
                        //System.Diagnostics.Debug.WriteLine("MOVING TO: " + mPath[mNextIndex].Position);
                        this.Velocity = Vector3.Normalize(mPath[mNextIndex].Position - this.Position) * PlayerProperties.MoveSpeed;
                    }
                    else
                    {
                        this.Velocity = Vector3.Zero;
                        mIsMoving = false;

                        mVisibleRepresentation.CurrentChainName = "IDLE";
                        GameProperties.RescaleSprite(mVisibleRepresentation);
                    }
                }
                if (mNextIndex == 0)
                {
                    //System.Diagnostics.Debug.WriteLine("MOVING TO: " + mPath[mNextIndex].Position);
                    this.Velocity = Vector3.Normalize(mPath[mNextIndex].Position - this.Position) * PlayerProperties.MoveSpeed;
                }
                
            }
            else
            {
                this.Velocity = Vector3.Zero;
                mIsMoving = false;

                mVisibleRepresentation.CurrentChainName = "IDLE";
                GameProperties.RescaleSprite(mVisibleRepresentation);
            }

            if (mInteractObstacle != null && mInteractObstacle.myType.Equals(OType.Hide))
            {
                if (mInteractObstacle.Collision.CollideAgainst(mCollision) && !mIsMoving)
                {
                    this.Z = mInteractObstacle.Z - 0.01f;
                    mIsHiding = true;
                }
            }
        }

        private void ShootArrow()
        {
            // Find first ready projectile
            foreach (Projectile p in mQuiver)
            {
                if (p.IsReady)
                {
                    p.Detach();
                    p.Position = mArms.Position;

                    // Shoot arrow at max length
                    if (mPullVector.Length() < PlayerProperties.MaxDrawLength)
                        p.Velocity = mPullVector * 3.0f;
                    else
                    {
                        mPullVector.Normalize();
                        p.Velocity = mPullVector * PlayerProperties.MaxDrawLength * 3.0f;
                    }

                    p.Activate();
                    CollisionManager.ManagedProjectiles.Add(p);
                    break;
                }
            }
        }

        private void DragCamera()
        {
            float PrevCameraX = SpriteManager.Camera.X;
            float PrevCameraY = SpriteManager.Camera.Y;

            InputManager.TouchScreen.ControlCamera(SpriteManager.Camera);

            // Limit distance of camera from player
            if (Math.Abs(this.X - SpriteManager.Camera.X) > PlayerProperties.MaxCameraXFromPlayer)
                SpriteManager.Camera.X = PrevCameraX;

            if (Math.Abs(this.Y - SpriteManager.Camera.Y) > PlayerProperties.MaxCameraYFromPlayer)
                SpriteManager.Camera.Y = PrevCameraY;
        }

        private void PushCamera()
        {
            float xDiff = this.X - SpriteManager.Camera.X;

            if (Math.Abs(xDiff) > PlayerProperties.MaxCameraXFromPlayer)
            {
                SpriteManager.Camera.X += xDiff;

                if (xDiff > 0.0f)
                    SpriteManager.Camera.X -= PlayerProperties.MaxCameraXFromPlayer;
                else if (xDiff < 0.0f)
                    SpriteManager.Camera.X += PlayerProperties.MaxCameraXFromPlayer;
            }

            float yDiff = this.Y - SpriteManager.Camera.Y;

            if (Math.Abs(yDiff) > PlayerProperties.MaxCameraYFromPlayer)
            {
                SpriteManager.Camera.Y += yDiff;

                if (yDiff > 0.0f)
                    SpriteManager.Camera.Y -= PlayerProperties.MaxCameraYFromPlayer;
                else if (yDiff < 0.0f)
                    SpriteManager.Camera.Y += PlayerProperties.MaxCameraYFromPlayer;
            }
        }

        private Obstacle GetObstacleCollision()
        {
            foreach (Obstacle o in CollisionManager.ManagedObstacles)
            {
                if (o.Collision.IsPointInside(ref GestureManager.TapPoint))
                {
                    return o;
                }
            }

            return null;
        }

        public void Die()
        {
            mArms.Visible = false;
            mHead.Visible = false;
            mBody.Visible = false;
            mVisibleRepresentation.Visible = true;
            mVisibleRepresentation.CurrentChainName = "IDLE";
            mAlive = false;

            GameProperties.RescaleSprite(mVisibleRepresentation);

            this.Velocity = Vector3.Zero;

            this.RotationZ = (float)Math.PI / 2.0f;
            this.X -= 1.0f;
        }

        public virtual void Activity()
        {
            GestureManager.Update(this.Z);
            //System.Diagnostics.Debug.WriteLine(GestureManager.CurInputState + " :: " + GestureManager.CurGesture);
            GestureManager.GetPointAtZ(mPlayerHUD.Z);

            if (GestureManager.CurGesture.Equals(Gesture.Tap))
            {
                Button b = mPlayerHUD.CheckButtonPressed(GestureManager.TouchAtZ);

                if (b != null)
                {
                    if (b.ID.Equals("BOW_BUTTON"))
                    {
                        mIsBowDrawn = !mIsBowDrawn;
                        b.Toggle();

                        if (mIsBowDrawn)
                        {
                            this.Velocity = Vector3.Zero;
                            mIsMoving = false;

                            mVisibleRepresentation.CurrentChainName = "DRAW_BOW";
                            GameProperties.RescaleSprite(mVisibleRepresentation);
                        }
                        else
                        {
                            mVisibleRepresentation.Visible = true;
                            mVisibleRepresentation.CurrentChainName = "IDLE";
                            GameProperties.RescaleSprite(mVisibleRepresentation);

                            mHead.Visible = false;
                            mArms.Visible = false;
                            mBody.Visible = false;
                        }
                    }
                }
                else if (!mIsBowDrawn)
                {
                    if (mInteractObstacle != null && 
                        mInteractObstacle.Collision.IsPointInside(GestureManager.TapPoint.X, GestureManager.TapPoint.Y) &&
                        mInteractObstacle.myType.Equals(OType.Hide))
                    {
                        mGoingToMove = false;
                    }
                    else
                    {
                        mGoingToMove = true;
                    }

                    mInteractObstacle = GetObstacleCollision();

                    if (mGoingToMove)
                    {
                        this.Z = (float)Math.Round(this.Z) + 0.01f;
                        StartMoving();

                        mIsHiding = false;
                        mVisibleRepresentation.CurrentChainName = "SNEAK";
                        GameProperties.RescaleSprite(mVisibleRepresentation);
                    }
                    else
                    {
                        mVisibleRepresentation.CurrentChainName = "IDLE";
                        GameProperties.RescaleSprite(mVisibleRepresentation);
                    }
                }
            }
            else if (GestureManager.CurGesture.Equals(Gesture.Drag))
            {
                if (mIsBowDrawn && mIsShooting && mArms.Visible)
                {
                    mPullVector = GestureManager.DragStart - GestureManager.DragEnd;

                    if (mPullVector.Length() > PlayerProperties.MinDrawLength) 
                        ShootArrow();

                    mIsShooting = false;
                }
            }
            else if (GestureManager.CurGesture.Equals(Gesture.None))
            {
                if (GestureManager.CurInputState.Equals(InputState.Pushed))
                {
                    if (mCollision.IsPointInside(GestureManager.DragStart.X, GestureManager.DragStart.Y) && mIsBowDrawn)
                    {
                        mIsShooting = true;
                    }
                }
                else if (GestureManager.CurInputState.Equals(InputState.Down))
                {
                    if (!mIsShooting)
                    {
                        DragCamera();
                    }
                    else
                    {
                        if ((GestureManager.CurTouchPoint - GestureManager.DragStart).Length() > PlayerProperties.MinDrawLength)
                        {
                            mIsFacingLeft = !(GestureManager.CurTouchPoint.Y - GestureManager.DragStart.Y > 0.0f);
                        }

                            float faceAtX = this.X - GestureManager.CurTouchPoint.X;
                            float faceAtY = this.Y - GestureManager.CurTouchPoint.Y;

                            float movementRotation = (float)Math.Atan2(faceAtY, 
                                                                       faceAtX);

                            mArms.RelativeRotationZ = movementRotation;
                            mHead.RelativeRotationZ = movementRotation;
                        
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("There is a problem with the GestureManager");
            }

            if (mIsMoving)
            {
                Move();
                PushCamera();
            }

            if (Velocity.Y > 0.0f)
            {
                mIsFacingLeft = true;
            }
            else if (Velocity.Y < 0.0f)
            {
                mIsFacingLeft = false;
            }

            mVisibleRepresentation.FlipHorizontal = mIsFacingLeft;
            mArms.FlipVertical = mIsFacingLeft;
            mBody.FlipHorizontal = mIsFacingLeft;
            mHead.FlipVertical = mIsFacingLeft;

            if (mVisibleRepresentation.JustCycled && mVisibleRepresentation.CurrentChainName.Equals("DRAW_BOW"))
            {
                mVisibleRepresentation.Visible = false;

                mHead.Visible = true;
                mArms.Visible = true;
                mBody.Visible = true;
            }
        }

        public virtual void Destroy()
        {
            // Remove self from the SpriteManager:
            SpriteManager.RemovePositionedObject(this);

            // Remove any other objects you've created:
            SpriteManager.RemoveSprite(mVisibleRepresentation);
            ShapeManager.Remove(mCollision);
            CollisionManager.PlayerCollision = null;

            foreach (Projectile p in mQuiver)
            {
                p.Destroy();
            }

            mPlayerHUD.Destroy();
        }

        #endregion
    }
}
