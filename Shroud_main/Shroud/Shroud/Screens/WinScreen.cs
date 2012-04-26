using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall;

using Shroud.Utilities;

namespace Shroud.Screens
{
    public class WinScreen : Screen
    {
        private Sprite mWin;


        #region Methods

        #region Constructor and Initialize

        public WinScreen()
            : base("WinScreen")
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

            mWin = SpriteManager.AddSprite(@"Content/Menus/", ContentManagerName, CameraManager.Pause);
            GameProperties.RescaleSprite(mWin);
            mWin.RotationZ = GameProperties.WorldRotation;
			
			
			
			
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

        #region Public Methods

        public override void Activity(bool firstTimeCalled)
        {
            base.Activity(firstTimeCalled);

            GestureManager.Update2(0.0f, 0.0f);

            if (GestureManager.CurGesture == GestureManager.Gesture.Tap)
            {
                IsActivityFinished = true;
                GameProperties.IsPaused = false;
                GameProperties.JumpBack = true;
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            SpriteManager.RemoveSprite(mWin);


        }

        #endregion

		
        #endregion
    }
}

