using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shroud.Entities;
using Shroud.Utilities;
using Scene = Shroud.Utilities.Scene;

using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Input;

namespace Shroud.Screens
{
    public class GameScreen : Screen
    {
        #region Methods

        #region Constructor and Initialize

        public GameScreen() : base("GameScreen")
        {
            // Don't put initialization code here, do it in
            // the Initialize method below
            //   |   |   |   |   |   |   |   |   |   |   |
            //   |   |   |   |   |   |   |   |   |   |   |
            //   V   V   V   V   V   V   V   V   V   V   V

        }

        public override void Initialize(bool addToManagers)
        {
            InitializeManagers();

            SpriteManager.Camera.BackgroundColor = Color.CadetBlue;

            LevelManager.Load("test.txt");

            SpriteManager.Camera.DrawsShapes = false;

			// AddToManagers should be called LAST in this method:
			if(addToManagers)
			{
				AddToManagers();
			}
        }

		public override void AddToManagers()
        {
		    // Nothing to Do Here
		
		}
		
        #endregion

        private void InitializeManagers()
        {
            WorldManager.Initialize();
            GestureManager.Initialize2();
        }

        #region Public Methods

        public override void Activity(bool firstTimeCalled)
        {
            base.Activity(firstTimeCalled);
            
            WorldManager.Update();
            GestureManager.Update2(WorldManager.PlayerInstance.Z, HUDManager.zUI - SpriteManager.Camera.Z);
            CameraManager.UpdateCamera2();
            HUDManager.Update();
        }

        public override void Destroy()
        {
            base.Destroy();

            WorldManager.Destroy();
        }

        #endregion

		
        #endregion
    }
}

