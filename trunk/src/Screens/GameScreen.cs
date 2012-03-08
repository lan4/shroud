using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shroud.Entities;
using Shroud.Utilities;

using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Input;

namespace Shroud.Screens
{
    public class GameScreen : Screen
    {
        Player player1;
        List<Enemy> enemies;
        Enemy target;
        Text gameOverText;

        List<Sprite> clouds;

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
            // Set the screen up here instead of in the Constructor to avoid
            // exceptions occurring during the constructor.

            player1 = new Player("Global");
            enemies = new List<Enemy>();
            target = new Enemy("Global", true);
            CollisionManager.TargetRef = target;

            gameOverText = TextManager.AddText("");
            gameOverText.Visible = false;
            gameOverText.AttachTo(SpriteManager.Camera, false);
            gameOverText.RelativeZ = -15.0f;
            gameOverText.RelativeRotationZ = GameProperties.WorldRotation;
            //gameOverText.SetPixelPerfectScale(SpriteManager.Camera);

            NodeManager.Initialize();
            /*
            NodeManager.AddNode(new Node(0.0f, 0.0f, 0.0f));
            NodeManager.AddNode(new Node(0.0f, 4.0f, 0.0f));
            NodeManager.AddEdge(NodeManager.Nodes[0], NodeManager.Nodes[1]);

            NodeManager.AddNode(new Node(0.0f, 10.0f, 0.0f));
            NodeManager.AddEdge(NodeManager.Nodes[1], NodeManager.Nodes[2]);
            player1.Position = NodeManager.Nodes[2].Position;

            NodeManager.AddNode(new Node(2.0f, -10.0f, 0.0f));
            NodeManager.AddEdge(NodeManager.Nodes[1], NodeManager.Nodes[3]);
            NodeManager.AddEdge(NodeManager.Nodes[0], NodeManager.Nodes[3]);
             * */

            NodeManager.AddNode(new Node(0.0f, 0.0f, 0.0f));
            NodeManager.AddNode(new Node(0.0f, -8.0f, 0.0f));
            NodeManager.AddNode(new Node(0.0f, -16.0f, 0.0f));
            NodeManager.AddNode(new Node(8.0f, -24.0f, 0.0f));
            NodeManager.AddNode(new Node(-8.0f, -24.0f, 0.0f));
            NodeManager.AddNode(new Node(8.0f, -32.0f, 0.0f));
            NodeManager.AddNode(new Node(-8.0f, -32.0f, 0.0f));
            NodeManager.AddNode(new Node(0.0f, -40.0f, 0.0f));
            NodeManager.AddNode(new Node(0.0f, -48.0f, 0.0f));
            NodeManager.AddNode(new Node(0.0f, -56.0f, 0.0f));

            NodeManager.AddEdge(NodeManager.Nodes[0], NodeManager.Nodes[1]);
            NodeManager.AddEdge(NodeManager.Nodes[1], NodeManager.Nodes[2]);
            NodeManager.AddEdge(NodeManager.Nodes[2], NodeManager.Nodes[3]);
            NodeManager.AddEdge(NodeManager.Nodes[2], NodeManager.Nodes[4]);
            NodeManager.AddEdge(NodeManager.Nodes[3], NodeManager.Nodes[5]);
            NodeManager.AddEdge(NodeManager.Nodes[4], NodeManager.Nodes[6]);
            NodeManager.AddEdge(NodeManager.Nodes[7], NodeManager.Nodes[5]);
            NodeManager.AddEdge(NodeManager.Nodes[7], NodeManager.Nodes[6]);
            NodeManager.AddEdge(NodeManager.Nodes[7], NodeManager.Nodes[8]);
            NodeManager.AddEdge(NodeManager.Nodes[8], NodeManager.Nodes[9]);

            SpriteManager.Camera.BackgroundColor = Color.CadetBlue;
            /*
            foreach (Node na in NodeManager.Nodes)
            {
                Sprite temp = SpriteManager.AddSprite("redball.png", "Global");
                temp.Position = na.Position;
                temp.ScaleX = temp.ScaleY = 0.1f;
            }
            */
            CollisionManager.Initialize();

            CollisionManager.ManagedObstacles.Add(new Obstacle("Global", Obstacle.OType.Hide, null));
            CollisionManager.ManagedObstacles.Add(new Obstacle("Global", Obstacle.OType.Hide, null));
            CollisionManager.ManagedObstacles.Add(new Obstacle("Global", Obstacle.OType.Hide, null));
            CollisionManager.ManagedObstacles.Add(new Obstacle("Global", Obstacle.OType.Hide, null));
            CollisionManager.ManagedObstacles.Add(new Obstacle("Global", Obstacle.OType.Hide, null));

            CollisionManager.ManagedObstacles[1].Position = NodeManager.Nodes[2].Position;
            CollisionManager.ManagedObstacles[2].Position = NodeManager.Nodes[4].Position;
            CollisionManager.ManagedObstacles[3].Position = NodeManager.Nodes[5].Position;
            CollisionManager.ManagedObstacles[4].Position = NodeManager.Nodes[6].Position;
            CollisionManager.ManagedObstacles[5].Position = NodeManager.Nodes[8].Position;

            GestureManager.Initialize();

            AIManager.Initialize();

            AIManager.ManagedEnemies.Add(new Enemy("Global", false));
            AIManager.ManagedEnemies.Add(new Enemy("Global", false));

            AIManager.ManagedEnemies[0].PatrolPath.Add(NodeManager.Nodes[5]);
            AIManager.ManagedEnemies[0].PatrolPath.Add(NodeManager.Nodes[7]);
            AIManager.ManagedEnemies[0].PatrolPath.Add(NodeManager.Nodes[6]);
            AIManager.ManagedEnemies[0].PatrolPath.Add(NodeManager.Nodes[4]);
            AIManager.ManagedEnemies[0].PatrolPath.Add(NodeManager.Nodes[2]);
            AIManager.ManagedEnemies[0].PatrolPath.Add(NodeManager.Nodes[3]);
            AIManager.ManagedEnemies[0].Backtrack = false;
            AIManager.ManagedEnemies[0].Position = NodeManager.Nodes[5].Position;

            AIManager.ManagedEnemies[1].PatrolPath.Add(NodeManager.Nodes[2]);
            AIManager.ManagedEnemies[1].PatrolPath.Add(NodeManager.Nodes[4]);
            AIManager.ManagedEnemies[1].PatrolPath.Add(NodeManager.Nodes[6]);
            AIManager.ManagedEnemies[1].PatrolPath.Add(NodeManager.Nodes[7]);
            AIManager.ManagedEnemies[1].Backtrack = true;
            AIManager.ManagedEnemies[1].Position = NodeManager.Nodes[2].Position;

            target.Position = NodeManager.Nodes[8].Position;
            target.PatrolPath.Add(NodeManager.Nodes[8]);
            target.PatrolPath.Add(NodeManager.Nodes[9]);
            target.Backtrack = true;

            AIManager.PlayerRef = player1;

            InitializeWorld();

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

        private void InitializeWorld()
        {
            List<Sprite> grounds = new List<Sprite>();

            for (int i = 0; i < 10; i++)
            {
                grounds.Add(SpriteManager.AddSprite(@"Content/Entities/Background/ground", "Global"));
                grounds[i].ScaleY = 16.0f;
                grounds[i].Z = -0.1f;
            }

            grounds[0].Position.Y = -4.0f;
            grounds[0].Position.X = -3.0f;

            grounds[1].Position.Y = -12.0f;
            grounds[1].Position.X = -3.0f;

            grounds[2].Position.Y = -20.0f;
            grounds[2].Position.X = -7.0f;
            grounds[2].RotationZ = (float)-Math.PI / 4.0f;

            grounds[3].Position.Y = -20.0f;
            grounds[3].Position.X = 0.0f;
            grounds[3].RotationZ = (float)Math.PI / 4.0f;

            grounds[4].Position.Y = -28.0f;
            grounds[4].Position.X = -10.0f;

            grounds[5].Position.Y = -28.0f;
            grounds[5].Position.X = 5.0f;

            grounds[6].Position.Y = -36.0f;
            grounds[6].Position.X = -7.0f;
            grounds[6].RotationZ = (float)Math.PI / 4.0f;

            grounds[7].Position.Y = -36.0f;
            grounds[7].Position.X = 0.0f;
            grounds[7].RotationZ = (float)-Math.PI / 4.0f;

            grounds[8].Position.Y = -42.0f;
            grounds[8].Position.X = -3.0f;

            grounds[9].Position.Y = -50.0f;
            grounds[9].Position.X = -3.0f;

            clouds = new List<Sprite>();

            for (int i = 0; i < 10; i++)
            {
                clouds.Add(SpriteManager.AddSprite(@"Content/Entities/Background/cloud" + ((i % 3) + 1), "Global"));
                GameProperties.RescaleSprite(clouds[i]);

                clouds[i].X = FlatRedBallServices.Random.Next(8, 20);
                clouds[i].Y = -FlatRedBallServices.Random.Next(0, 40);
                clouds[i].Velocity.Y = (float)FlatRedBallServices.Random.NextDouble() * -3.0f - 3.0f;
                clouds[i].RotationZ = GameProperties.WorldRotation;
                clouds[i].Z = -0.5f;
                clouds[i].FlipHorizontal = true;
            }

            Sprite m = SpriteManager.AddSprite(@"Content/Entities/Background/mountain", "Global");
            m.Z = -0.5f;
            m.RotationZ = GameProperties.WorldRotation;
            m.Y = -10.0f;
            GameProperties.RescaleSprite(m);

            Sprite mn = SpriteManager.AddSprite(@"Content/Entities/Background/mountain2", "Global");
            mn.Z = -0.5f;
            mn.RotationZ = GameProperties.WorldRotation;
            mn.Y = -40.0f;
            GameProperties.RescaleSprite(mn);
        }

        #region Public Methods

        public override void Activity(bool firstTimeCalled)
        {
            base.Activity(firstTimeCalled);

            if (player1.Alive)
                player1.Activity();
            else
            {
                gameOverText.Visible = true;
                gameOverText.DisplayText = "YOU LOSE";
            }

            if (target.Alive)
                target.Activity();
            else
            {
                gameOverText.Visible = true;
                gameOverText.DisplayText = "YOU WIN";
            }

            AIManager.PlayerPosition.X = player1.X;
            AIManager.PlayerPosition.Y = player1.Y;
            AIManager.PlayerPosition.Z = player1.Z;

            AIManager.Update();
            CollisionManager.CheckProjectileCollisions(AIManager.ManagedEnemies);
            CollisionManager.AnimateProjectiles();

            foreach (Enemy e in AIManager.ManagedEnemies)
            {
                if (e.Alive)
                    e.Activity();
            }

            foreach (Sprite s in clouds)
            {   
                if (s.Y < -50.0f)
                    s.Y = 10.0f;
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            player1.Destroy();

            foreach (Enemy e in enemies)
            {
                e.Destroy();
            }

            target.Destroy();
        }

        #endregion

		
        #endregion
    }
}

