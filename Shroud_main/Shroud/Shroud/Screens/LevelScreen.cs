using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall;
using FlatRedBall.Math.Geometry;

using Shroud.Utilities;
using Shroud.Entities;
using System.Text.RegularExpressions;

namespace Shroud.Screens
{
    public class LevelScreen : Screen
    {
        private Sprite mBg;
        private Sprite mTitle;
        private List<LevelButton> mLevels;
        private BackButton mBack;

        private class LevelButton : PositionedObject
        {
            private Sprite mbg;
            private Sprite mFace;
            private Sprite mDone;
            private Sprite mBadge1;
            private Sprite mBadge2;
            private Sprite mBadge3;

            private string mLevelString;
            public string mLevelToken;

            public string LevelString
            {
                get { return mLevelString; }
            }

            private AxisAlignedRectangle mCollision;

            public AxisAlignedRectangle Collision
            {
                get { return mCollision; }
            }

            public LevelButton(string tags) : base()
            {
                mbg = SpriteManager.AddSprite(@"Content/Menus/Levels/scrollBase", "Global");
                mbg.AttachTo(this, false);
                GameProperties.RescaleSprite(mbg);
                mbg.RelativeRotationZ = GameProperties.WorldRotation;

                string face;

                if (tags.Contains('g'))
                {
                    face = "general";
                }
                else
                {
                    face = "target";
                }

                mFace = SpriteManager.AddSprite(@"Content/Menus/Levels/" + face, "Global");
                mFace.AttachTo(this, false);
                GameProperties.RescaleSprite(mFace);
                mFace.RelativeRotationZ = GameProperties.WorldRotation;

                mDone = SpriteManager.AddSprite(@"Content/Menus/Levels/crossedOut", "Global");
                mDone.AttachTo(this, false);
                GameProperties.RescaleSprite(mDone);
                mDone.RelativeRotationZ = GameProperties.WorldRotation;

                mBadge1 = SpriteManager.AddSprite(@"Content/Menus/Levels/notSeenBadge", "Global");
                mBadge1.AttachTo(this, false);
                GameProperties.RescaleSprite(mBadge1);
                mBadge1.RelativeRotationZ = GameProperties.WorldRotation;

                mBadge2 = SpriteManager.AddSprite(@"Content/Menus/Levels/killedOnlyTargetBadge", "Global");
                mBadge2.AttachTo(this, false);
                GameProperties.RescaleSprite(mBadge2);
                mBadge2.RelativeRotationZ = GameProperties.WorldRotation;

                mBadge3 = SpriteManager.AddSprite(@"Content/Menus/Levels/didNotDieBadge", "Global");
                mBadge3.AttachTo(this, false);
                GameProperties.RescaleSprite(mBadge3);
                mBadge3.RelativeRotationZ = GameProperties.WorldRotation;

                mBadge1.Visible = tags.Contains('a');
                mBadge2.Visible = tags.Contains('b');
                mBadge3.Visible = tags.Contains('c');
                mDone.Visible = tags.Contains('d');

                mCollision = ShapeManager.AddAxisAlignedRectangle();
                mCollision.ScaleX = mbg.ScaleY;
                mCollision.ScaleY = mbg.ScaleX;
                mCollision.AttachTo(this, false);
                mCollision.RelativeRotationZ = GameProperties.WorldRotation;

                mLevelString = "level" + Regex.Match(tags, @"\d+").Value + ".txt";
            }

            public void Destroy()
            {
                SpriteManager.RemovePositionedObject(this);
                SpriteManager.RemoveSprite(mbg);
                SpriteManager.RemoveSprite(mFace);
                SpriteManager.RemoveSprite(mDone);
                SpriteManager.RemoveSprite(mBadge1);
                SpriteManager.RemoveSprite(mBadge2);
                SpriteManager.RemoveSprite(mBadge3);
                ShapeManager.Remove(mCollision);
            }
        }

        private class BackButton : PositionedObject
        {
            private AxisAlignedRectangle mCollision;
            private Sprite mButton;

            public AxisAlignedRectangle Collision
            {
                get { return mCollision; }
            }

            public BackButton(string contentManagerName)
                : base()
            {
                SpriteManager.AddPositionedObject(this);

                mButton = SpriteManager.AddSprite(@"Content/Menus/goBack", contentManagerName);
                mButton.AttachTo(this, false);
                GameProperties.RescaleSprite(mButton);
                mButton.RelativeRotationZ = GameProperties.WorldRotation;

                mCollision = ShapeManager.AddAxisAlignedRectangle();
                mCollision.ScaleX = mButton.ScaleY;
                mCollision.ScaleY = mButton.ScaleX;
                mCollision.AttachTo(this, false);
            }

            public void Destroy()
            {
                SpriteManager.RemovePositionedObject(this);
                SpriteManager.RemoveSprite(mButton);
                ShapeManager.Remove(mCollision);
            }
        }

        #region Methods
        
        #region Constructor and Initialize

        public LevelScreen()
            : base("$safeitemname$")
        {
            // Don't put initialization code here, do it in
            // the Initialize method below
            //   |   |   |   |   |   |   |   |   |   |   |
            //   |   |   |   |   |   |   |   |   |   |   |
            //   V   V   V   V   V   V   V   V   V   V   V

        }

        public override void Initialize(bool addToManagers)
        {
            // Set the screen up here instead of in the Constructor to avoid
            // exceptions occurring during the constructor.

            mLevels = new List<LevelButton>();

            mBg = SpriteManager.AddSprite(@"Content/Menus/bg", ContentManagerName);
            GameProperties.RescaleSprite(mBg);
            mBg.RotationZ = GameProperties.WorldRotation;

            mTitle = SpriteManager.AddSprite(@"Content/Menus/lvlSelect", ContentManagerName);
            GameProperties.RescaleSprite(mTitle);
            mTitle.RotationZ = GameProperties.WorldRotation;
            mTitle.X = 7.0f;

            mBack = new BackButton(ContentManagerName);
            mBack.X = -7.0f;
            mBack.Y = 8.0f;
			
			string[] levelStrings = GameProperties.ProfileString.Split(' ');

            foreach (string level in levelStrings)
            {
                if (level.Contains('l'))
                {
                    mLevels.Add(new LevelButton(level));
                    mLevels[mLevels.Count - 1].mLevelToken = level;
                }
            }

            FormatLevels();

            GestureManager.Initialize2();
			
			// AddToManagers should be called LAST in this method:
			if(addToManagers)
			{
				AddToManagers();
			}
        }

		public override void AddToManagers()
        {
		
		
		}

        private void FormatLevels()
        {
            int i = 0;
            for (int y = 2; y > -5; y -= 8)
            {
                for (int x = 10; x > -11; x -= 10)
                {
                    if (i < mLevels.Count)
                    {
                        mLevels[i].X = y;
                        mLevels[i].Y = x;
                    }
                    else
                    {
                        return;
                    }

                    i++;
                }
            }
        }

        #endregion

        #region Public Methods

        public override void Activity(bool firstTimeCalled)
        {
            base.Activity(firstTimeCalled);

            GestureManager.Update2(0.0f, 0.0f);

            if (GestureManager.CurGesture == GestureManager.Gesture.Tap)
            {
                float x = GestureManager.EndTouchWorld.X;
                float y = GestureManager.EndTouchWorld.Y;

                foreach (LevelButton b in mLevels)
                {
                    if (b.Collision.IsPointInside(x, y))
                    {
                        GameProperties.LevelString = b.LevelString;
                        GameProperties.LevelToken = b.mLevelToken;
                        Destroy();
                        MoveToScreen(typeof(Screens.GameScreen).FullName);
                    }
                }

                if (mBack.Collision.IsPointInside(x, y))
                {
                    Destroy();
                    MoveToScreen(typeof(Screens.ProfileScreen).FullName);
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            SpriteManager.RemoveSprite(mBg);
            SpriteManager.RemoveSprite(mTitle);

            foreach (LevelButton b in mLevels)
            {
                b.Destroy();
            }

            mBack.Destroy();

            GestureManager.Clean();
        }

        #endregion

		
        #endregion
    }
}

