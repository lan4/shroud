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
            set { mCurScene = value; }
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
            /*origin.mBG = SpriteManager.AddSprite(@"Content/Entities/Background/bg1", "Global", CameraManager.Background);
            GameProperties.RescaleSprite(origin.mBG);
           	
            //origin.mBG.Z = -3.0f;
            origin.mBG.RotationZ = GameProperties.WorldRotation;*/
            origin.SetBackground("bg1");
            origin.AddGround(-16.0f, -21.0f, 6, 3, LayerManager.MainLayer.Background, LayerManager.DetailLayer.Back);
            origin.AddGround(1, 2, 4, 3, origin.Grounds[0], LayerManager.MainLayer.Middleground, LayerManager.DetailLayer.Front);
            origin.AddLadder(origin.Grounds[0].GetTilePosition(1), origin.Grounds[1].GetTilePosition(0), origin.Grounds[0].TileHeight,
                             LayerManager.MainLayer.Middleground, LayerManager.DetailLayer.Back);
            origin.AddLadder(origin.Grounds[0].GetTilePosition(4), origin.Grounds[1].GetTilePosition(1), origin.Grounds[0].TileHeight,
                             LayerManager.MainLayer.Middleground, LayerManager.DetailLayer.Back);

            mCurScene = origin;

            Scene next = AddScene(1, 0, 0);
            /*next.mBG = SpriteManager.AddSprite(@"Content/Entities/Background/bg1", "Global");//, CameraManager.Background);
            GameProperties.RescaleSprite(next.mBG);
            next.mBG.RotationZ = GameProperties.WorldRotation;*/
            next.SetBackground("bg1");
            //next.mBG.Z = -3.0f;
            //next.mBG.X += next.WorldAnchor.X;
            //next.mBG.Y += next.WorldAnchor.Y;
            next.AddGround(-16.0f, -21.0f, 6, 3, LayerManager.MainLayer.Background, LayerManager.DetailLayer.Back);
            next.AddGround(1, 2, 4, 3, next.Grounds[0], LayerManager.MainLayer.Middleground, LayerManager.DetailLayer.Front);
            next.AddLadder(next.Grounds[0].GetTilePosition(1), next.Grounds[1].GetTilePosition(0), next.Grounds[0].TileHeight,
                             LayerManager.MainLayer.Background, LayerManager.DetailLayer.Back);
            next.AddLadder(next.Grounds[0].GetTilePosition(4), next.Grounds[1].GetTilePosition(1), next.Grounds[0].TileHeight,
                             LayerManager.MainLayer.Middleground, LayerManager.DetailLayer.Back);

            //SceneMoveRight();

            #region Create Nodes and Edges
            float z = 0.0f;//LayerManager.SetLayer(LayerManager.MainLayer.Entity2, LayerManager.DetailLayer.Middle);

            Node n0 = origin.AddNode(-4.0f,  12.0f, LayerManager.MainLayer.Background);
            Node n1 = origin.AddNode(-4.0f, 8.0f, LayerManager.MainLayer.Background);
            Node n2 = origin.AddNode(6.0f, 8.0f, LayerManager.MainLayer.Background);
            Node n3 = origin.AddNode(6.0f, -8.0f, LayerManager.MainLayer.Background);
            Node n4 = origin.AddNode(-4.0f, -8.0f, LayerManager.MainLayer.Background);
            Node n5 = origin.AddNode(-4.0f, -12.0f, LayerManager.MainLayer.Background);

            Node.AddUndirectedEdge(n0, n1);
            Node.AddUndirectedEdge(n1, n2);
            Node.AddUndirectedEdge(n2, n3);
            Node.AddUndirectedEdge(n1, n4);
            Node.AddUndirectedEdge(n4, n5);
            Node.AddUndirectedEdge(n3, n4);


            Node n0b = next.AddNode(-4.0f, 12.0f, LayerManager.MainLayer.Background);
            Node n1b = next.AddNode(-4.0f, 8.0f, LayerManager.MainLayer.Background);
            Node n2b = next.AddNode(6.0f, 8.0f, LayerManager.MainLayer.Background);
            Node n3b = next.AddNode(6.0f, -8.0f, LayerManager.MainLayer.Background);
            Node n4b = next.AddNode(-4.0f, -8.0f, LayerManager.MainLayer.Background);
            Node n5b = next.AddNode(-4.0f, -12.0f, LayerManager.MainLayer.Background);

            Node.AddUndirectedEdge(n0b, n1b);
            Node.AddUndirectedEdge(n1b, n2b);
            Node.AddUndirectedEdge(n2b, n3b);
            Node.AddUndirectedEdge(n1b, n4b);
            Node.AddUndirectedEdge(n4b, n5b);
            Node.AddUndirectedEdge(n3b, n4b);

            #endregion

            Node.DEBUG_VIEW();

            WorldManager.PlayerInstance = new Player2("Global");
            WorldManager.PlayerInstance.Position = n5.Position;
            WorldManager.PlayerInstance.Z = LayerManager.SetLayer(LayerManager.MainLayer.Entity2, LayerManager.DetailLayer.Middle);

            List<Node> patrol1 = new List<Node>();
            patrol1.Add(n1);
            patrol1.Add(n2);
            patrol1.Add(n3);
            patrol1.Add(n4);

            WorldManager.Soldiers.Add(new Soldier("Global", patrol1));
            WorldManager.Soldiers[0].Position = n1.Position;
            //WorldManager.Soldiers[0].KeepTrackOfReal = true;
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

        public static void SceneMoveRight()
        {
            mCurScene = mCurScene.Right;
        }
    }
}
