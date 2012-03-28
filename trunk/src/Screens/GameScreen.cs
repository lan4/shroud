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

            #region Create Nodes and Edges
            Node n0 = Node.AddGraphNode(0.0f, 0.0f, 0.0f);
            Node n1 = Node.AddGraphNode(0.0f, -8.0f, 0.0f);
            Node n2 = Node.AddGraphNode(0.0f, -16.0f, 0.0f);
            Node n3 = Node.AddGraphNode(8.0f, -24.0f, 0.0f);
            Node n4 = Node.AddGraphNode(-8.0f, -24.0f, 0.0f);
            Node n5 = Node.AddGraphNode(8.0f, -32.0f, 0.0f);
            Node n6 = Node.AddGraphNode(-8.0f, -32.0f, 0.0f);
            Node n7 = Node.AddGraphNode(0.0f, -40.0f, 0.0f);
            Node n8 = Node.AddGraphNode(0.0f, -48.0f, 0.0f);
            Node n9 = Node.AddGraphNode(0.0f, -56.0f, 0.0f);

            Node.AddUndirectedEdge(n0, n1);
            Node.AddUndirectedEdge(n1, n2);
            Node.AddUndirectedEdge(n2, n3);
            Node.AddUndirectedEdge(n2, n4);
            Node.AddUndirectedEdge(n3, n5);
            Node.AddUndirectedEdge(n4, n6);
            Node.AddUndirectedEdge(n7, n5);
            Node.AddUndirectedEdge(n7, n6);
            Node.AddUndirectedEdge(n7, n8);
            Node.AddUndirectedEdge(n8, n9);
            #endregion

            Node.DEBUG_VIEW();

            WorldManager.PlayerInstance = new Player1("Global");
            WorldManager.PlayerInstance.Position = n0.Position;

            SpriteManager.Camera.BackgroundColor = Color.CadetBlue;

            LevelManager.Load("test.txt");

            //SpriteManager.Camera.DrawsShapes = false;

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
            GestureManager.Update2(WorldManager.PlayerInstance.Z, HUDManager.zUI);
            CameraManager.UpdateCamera();
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

