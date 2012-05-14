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

using Shroud.Utilities;
using Gesture = Shroud.Utilities.GestureManager.Gesture;

namespace Shroud.Screens
{
    public class StartScreen : Screen
    {
        private Sprite mBg;
        private Sprite mLogo;
        private Sprite mClickText;

        #region Methods

        #region Constructor and Initialize

        public StartScreen()
            : base("StartScreen")
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
            mLogo = SpriteManager.AddSprite(@"Content/Menus/logo", "Global");
            mClickText = SpriteManager.AddSprite(@"Content/Menus/clickScreenBtn", "Global");

            GameProperties.RescaleSprite(mBg);
            GameProperties.RescaleSprite(mLogo);
            GameProperties.RescaleSprite(mClickText);

            mBg.RotationZ = GameProperties.WorldRotation;
            mLogo.RotationZ = GameProperties.WorldRotation;
            mClickText.RotationZ = GameProperties.WorldRotation;

            InitializeManagers();
            IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

            if (!myIsolatedStorage.FileExists("profiles2.txt"))
            {
                myIsolatedStorage.Dispose();
                GameProperties.CreateProfiles();
            }
            //GameProperties.Save();
            //System.Diagnostics.Debug.WriteLine(GameProperties.Read());

			// AddToManagers should be called LAST in this method:
			if(addToManagers)
			{
				AddToManagers();
			}
        }

		public override void AddToManagers()
        {
		
		
		}
		
        #endregion

        private void Fade()
        {
            if (mClickText.Alpha >= 1.0f)
            {
                mClickText.AlphaRate = -1.0f;
            }
            else if (mClickText.Alpha <= 0.0f)
            {
                mClickText.AlphaRate = 1.0f;
            }
        }

        private void InitializeManagers()
        {
            GestureManager.Initialize2();
        }

        #region Public Methods

        public override void Activity(bool firstTimeCalled)
        {
            if (!firstTimeCalled)
            {
                base.Activity(firstTimeCalled);

                Fade();
                GestureManager.Update2(0.0f, 0.0f);

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    GameProperties.Quit();
                }

                if (GestureManager.CurGesture == Gesture.Tap)
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
            SpriteManager.RemoveSprite(mLogo);
            SpriteManager.RemoveSprite(mClickText);

            GestureManager.Clean();
        }

        #endregion

		
        #endregion
    }
}
