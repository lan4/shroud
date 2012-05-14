using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;

using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using FlatRedBall;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Graphics;

using Shroud.Utilities;
using Shroud.Entities;

namespace Shroud.Screens
{
    public class ProfileScreen : Screen
    {
        private Sprite mloadText;
        private Sprite mBg;
        private List<ProfileButton> mProfiles;
        private bool mFirstNew;

        private class ProfileButton : PositionedObject
        {
            private static int mTotal = 0;

            public static void ResetTotal()
            {
                mTotal = 0;
            }

            private Sprite mbg;
            //private Text mNumber;
            private Text mPercent;
            private AxisAlignedRectangle mCollision;
            private string mStoredString;
            private string mNumLvls;

            public AxisAlignedRectangle Collision
            {
                get { return mCollision; }
            }

            public string StoredString
            {
                get { return mStoredString; }
                set
                {
                    if (value != null)
                        mStoredString = value;
                    else
                        mStoredString = "";
                }

            }

            public string NumLevels
            {
                get { return mNumLvls; }
                set { mNumLvls = value; }
            }

            public ProfileButton(float percent) : base()
            {
                mTotal++;

                /*if (mTotal > 6)
                {
                    mTotal = 1;
                }*/

                SpriteManager.AddPositionedObject(this);

                BitmapFont fnt = new BitmapFont(@"Content\Fonts\blambot", @"Content\Fonts\blambot.fnt", "Global");

                if (percent == 100.0f)
                {
                    mPercent = TextManager.AddText("END", fnt);
                }
                else
                {
                    mPercent = TextManager.AddText(((int)percent).ToString() + "%", fnt);
                }

                mPercent.AttachTo(this, false);
                mPercent.RelativeX = -1.0f;
                mPercent.RelativeY = 1.5f;
                mPercent.RelativeZ = 0.1f;
                mPercent.RelativeRotationZ = GameProperties.WorldRotation;
                mPercent.Red = 0.0f;
                mPercent.Blue = 0.0f;
                mPercent.Green = 0.0f;
                mPercent.Scale = 1.0f;
                mPercent.Spacing = mPercent.Scale;

                mbg = SpriteManager.AddSprite(@"Content/Menus/Profiles/scroll" + mTotal + "-1", "Global");
                GameProperties.RescaleSprite(mbg);
                mbg.AttachTo(this, false);
                mbg.RelativeRotationZ = GameProperties.WorldRotation;
                
                /*mNumber = TextManager.AddText(mTotal.ToString());
                mNumber.AttachTo(this, false);
                mNumber.RelativeY = 2.0f;
                mNumber.RelativeRotationZ = GameProperties.WorldRotation;*/

                mCollision = ShapeManager.AddAxisAlignedRectangle();
                mCollision.ScaleX = mbg.ScaleY;
                mCollision.ScaleY = mbg.ScaleX;
                mCollision.AttachTo(this, false);
                mCollision.RelativeRotationZ = GameProperties.WorldRotation;

                mStoredString = "";
            }

            public ProfileButton()
                : base()
            {
                mTotal++;

                /*if (mTotal > 6)
                {
                    mTotal = 1;
                }*/

                SpriteManager.AddPositionedObject(this);

                mbg = SpriteManager.AddSprite(@"Content/Menus/Profiles/scroll" + mTotal + "-0", "Global");
                GameProperties.RescaleSprite(mbg);
                mbg.AttachTo(this, false);
                mbg.RelativeRotationZ = GameProperties.WorldRotation;

                /*mNumber = TextManager.AddText(mTotal.ToString());
                mNumber.AttachTo(this, false);
                mNumber.RelativeY = 2.0f;
                mNumber.RelativeRotationZ = GameProperties.WorldRotation;*/

                mPercent = TextManager.AddText("");
                mPercent.AttachTo(this, false);
                mPercent.RelativeX = -2.0f;
                mPercent.RelativeRotationZ = GameProperties.WorldRotation;
                mPercent.Visible = false;

                mCollision = ShapeManager.AddAxisAlignedRectangle();
                mCollision.ScaleX = mbg.ScaleY;
                mCollision.ScaleY = mbg.ScaleX;
                mCollision.AttachTo(this, false);
                mCollision.RelativeRotationZ = GameProperties.WorldRotation;

                mStoredString = "";
            }

            public void Destroy()
            {
                SpriteManager.RemovePositionedObject(this);

                SpriteManager.RemoveSprite(mbg);
                //TextManager.RemoveText(mNumber);
                TextManager.RemoveText(mPercent);
                ShapeManager.Remove(mCollision);
            }
        }
	
        #region Methods

        #region Constructor and Initialize

        public ProfileScreen()
            : base("ProfileScreen")
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

            mBg = SpriteManager.AddSprite(@"Content/Menus/bg", "Global");
            GameProperties.RescaleSprite(mBg);
            mBg.RotationZ = GameProperties.WorldRotation;

            mloadText = SpriteManager.AddSprite(@"Content/Menus/SelectProfile", "Global");
            GameProperties.RescaleSprite(mloadText);
            mloadText.RotationZ = GameProperties.WorldRotation;
            mloadText.X = 7.0f;

            GestureManager.Initialize2();

            mProfiles = new List<ProfileButton>();

            mFirstNew = false;
            ProfileButton.ResetTotal();

            LoadProfiles();
            FormatProfiles();
			
			// AddToManagers should be called LAST in this method:
			if(addToManagers)
			{
				AddToManagers();
			}
        }

		public override void AddToManagers()
        {
		
		
		}

        private void LoadProfiles()
        {
            string line;
            string numCom;

            IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("profiles2.txt", FileMode.Open, FileAccess.Read);
            using (StreamReader reader = new StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    //System.Diagnostics.Debug.WriteLine(line);

                    if (line.Length > 0)
                    {
                        numCom = "";

                        for (int i = 0; i < line.Length; i++)
                        {
                            if (line[i].Equals(' ') || line[i].Equals('\n') || line[i].Equals('\r'))
                            {
                                int levelsDone = -99;

                                levelsDone = int.Parse(numCom.Replace("p", ""));

                                if (levelsDone >= 0)
                                {
                                    mProfiles.Add(new ProfileButton((levelsDone / (float) GameProperties.TotalLevels) * 100.0f));
                                    mProfiles[mProfiles.Count - 1].StoredString = line.Trim();
                                    mProfiles[mProfiles.Count - 1].NumLevels = numCom;
                                }
                                else
                                {
                                    //mFirstNew = true;
                                    mProfiles.Add(new ProfileButton());
                                    mProfiles[mProfiles.Count - 1].StoredString = line.Trim();
                                    mProfiles[mProfiles.Count - 1].NumLevels = numCom;
                                }
                                break;
                            }
                            else
                            {
                                numCom += line[i];
                            }
                        }
                    }

                }

                reader.Close();
                myIsolatedStorage.Dispose();
            }
        }

        private void FormatProfiles()
        {
            switch (mProfiles.Count)
            {
                case 6:
                    mProfiles[5].X = -6.0f;
                    mProfiles[5].Y = -10.0f;
                    goto case 5;
                case 5:
                    mProfiles[4].X = -6.0f;
                    mProfiles[4].Y = 0.0f;
                    goto case 4;
                case 4:
                    mProfiles[3].X = -6.0f;
                    mProfiles[3].Y = 10.0f;
                    goto case 3;
                case 3:
                    mProfiles[2].X = 2.0f;
                    mProfiles[2].Y = -10.0f;
                    goto case 2;
                case 2:
                    mProfiles[1].X = 2.0f;
                    mProfiles[1].Y = 0.0f;
                    goto case 1;
                case 1:
                    mProfiles[0].X = 2.0f;
                    mProfiles[0].Y = 10.0f;
                    break;
            }
        }
		
        #endregion

        #region Public Methods

        public override void Activity(bool firstTimeCalled)
        {
            if (!firstTimeCalled)
            {
                base.Activity(firstTimeCalled);

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    GameProperties.Quit();
                }

                GestureManager.Update2(0.0f, 0.0f);

                if (GestureManager.CurGesture == GestureManager.Gesture.Tap && !firstTimeCalled)
                {
                    float x = GestureManager.EndTouchWorld.X;
                    float y = GestureManager.EndTouchWorld.Y;

                    foreach (ProfileButton p in mProfiles)
                    {
                        if (p.Collision.IsPointInside(x, y))
                        {
                            if (p.NumLevels[1] == '-')
                            {
                                GameProperties.ProfileString = p.StoredString.Replace(p.NumLevels, "p0");

                                GameProperties.NumLevels = 0;
                            }
                            else
                            {
                                GameProperties.ProfileString = p.StoredString;
                                GameProperties.NumLevels = int.Parse(p.NumLevels.Replace("p", ""));
                            }

                            GameProperties.OldProfileString = p.StoredString;

                            GameProperties.Save();
                            Destroy();
                            MoveToScreen(typeof(Screens.LevelScreen).FullName);
                        }
                    }
                }
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            SpriteManager.RemoveSprite(mBg);
            SpriteManager.RemoveSprite(mloadText);

            foreach (ProfileButton p in mProfiles)
            {
                p.Destroy();
            }

            GestureManager.Clean();
        }

        #endregion

		
        #endregion
    }
}

