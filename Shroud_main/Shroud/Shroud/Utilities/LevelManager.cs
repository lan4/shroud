using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;

using FlatRedBall;

using Shroud.Entities;

namespace Shroud.Utilities
{
    public static class LevelManager
    {
        private static List<WorldObject> mManagedWObjects = new List<WorldObject>();
        private static List<Enemy1> mManagedEnemies = new List<Enemy1>();
        private static List<Trap> mManagedTraps = new List<Trap>();
        private static Player1 mPlayer;
        private static Scene mCurScene;

        private enum Direction
        {
            Left,
            Right,
            Up,
            Down,
            Front,
            Back
        };

        public static Scene CurrentScene
        {
            get { return mCurScene; }
        }

        public static void Load(string filename)
        {
            /*
            var resource = System.Windows.Application.GetResourceStream(new Uri(@"/Shroud;component/Data/test.txt", UriKind.Relative));

            StreamReader streamReader = new StreamReader(resource.Stream);
            string x = streamReader.ReadToEnd();

            string s;
            while (!streamReader.EndOfStream)
            {
                s = streamReader.ReadLine();
                System.Diagnostics.Debug.WriteLine(s);
            }
            */

            Scene origin = AddScene();
            origin.mBG = SpriteManager.AddSprite(@"Content/Entities/Background/bg1", "Global");
            GameProperties.RescaleSprite(origin.mBG);
            origin.mBG.Z = -0.1f;
            origin.mBG.RotationZ = GameProperties.WorldRotation;
            origin.AddGround(-14.8f, 8.0f, 8, 3, LayerManager.MainLayer.Background, LayerManager.DetailLayer.Back);
            origin.AddGround(-4.2f, 2.5f, 4, 3, LayerManager.MainLayer.Middleground, LayerManager.DetailLayer.Front);
            origin.AddLadder(origin.Grounds[0].GetUnitCoord(4), origin.Grounds[1].GetUnitCoord(1),
                             LayerManager.MainLayer.Background, LayerManager.DetailLayer.Back);
            origin.AddLadder(origin.Grounds[0].GetUnitCoord(6), origin.Grounds[1].GetUnitCoord(1),
                             LayerManager.MainLayer.Background, LayerManager.DetailLayer.Back);

            #region Create Nodes and Edges
            Node n0 = Node.AddGraphNode(-4.0f,  12.0f, 0.0f);
            Node n1 = Node.AddGraphNode(-4.0f,  8.0f, 0.0f);
            Node n2 = Node.AddGraphNode( 6.0f,  8.0f, 0.0f);
            Node n3 = Node.AddGraphNode( 6.0f, -8.0f, 0.0f);
            Node n4 = Node.AddGraphNode(-4.0f, -8.0f, 0.0f);
            Node n5 = Node.AddGraphNode(-4.0f, -12.0f, 0.0f);

            Node.AddUndirectedEdge(n0, n1);
            Node.AddUndirectedEdge(n1, n2);
            Node.AddUndirectedEdge(n2, n3);
            Node.AddUndirectedEdge(n1, n4);
            Node.AddUndirectedEdge(n4, n5);
            Node.AddUndirectedEdge(n3, n4);
            #endregion

            Node.DEBUG_VIEW();

            WorldManager.PlayerInstance = new Player2("Global");
            WorldManager.PlayerInstance.Position = n0.Position;

            List<Node> patrol1 = new List<Node>();
            patrol1.Add(n1);
            patrol1.Add(n2);
            patrol1.Add(n3);
            patrol1.Add(n4);

            WorldManager.Soldiers.Add(new Soldier("Global", patrol1));
            WorldManager.Soldiers[0].Position = n0.Position;
            WorldManager.Soldiers[0].KeepTrackOfReal = true;
        }

        private static Scene AddScene()
        {
            mCurScene = Scene.Create();
            return mCurScene;
        }

        private static Scene AddScene(int x, int y, int z)
        {
            return Scene.Create(x, y, z);
        }

        // MIGHT BE DEPRECATED
        private static Scene AddScene(Scene baseScene, Direction d)
        {
            switch (d)
            {
                case Direction.Left:
                    baseScene.Left = Scene.Create(); 
                    return baseScene.Left;
                case Direction.Right:
                    baseScene.Right = Scene.Create();
                    return baseScene.Right;
                case Direction.Up:
                    baseScene.Up = Scene.Create();
                    return baseScene.Up;
                case Direction.Down:
                    baseScene.Down = Scene.Create();
                    return baseScene.Down;
                case Direction.Front:
                    baseScene.Front = Scene.Create();
                    return baseScene.Front;
                case Direction.Back:
                    baseScene.Back = Scene.Create();
                    return baseScene.Back;
            }

            return null;
        }

        private static void CreatePlayer(float x, float y, float z)
        {
            if (mPlayer == null)
            {
                mPlayer = new Player1("Global");
                mPlayer.X = x;
                mPlayer.Y = y;
                mPlayer.Z = z;
            }
            else
                System.Diagnostics.Debug.WriteLine("Warning: Could not create a Player instance because there is already a player instance");
        }

        public static void Update()
        {

        }
    }
}
