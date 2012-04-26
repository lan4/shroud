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
            
            var resource = System.Windows.Application.GetResourceStream(new Uri(@"/Shroud;component/Data/level1.txt", UriKind.Relative));

            StreamReader streamReader = new StreamReader(resource.Stream);
            //string x = streamReader.ReadToEnd();

            string s;
            string[] tokens;
            Scene sc = null;
            while (!streamReader.EndOfStream)
            {
                s = streamReader.ReadLine();

                tokens = s.Split(' ');

                switch (tokens[0])
                {
                    case "s":
                        int x = int.Parse(tokens[1]);
                        int y = int.Parse(tokens[2]);
                        int z = int.Parse(tokens[3]);

                        if (x == 0 && y == 0 && z == 0)
                        {
                            sc = AddScene();
                            mCurScene = sc;
                        }
                        else
                        {
                            sc = AddScene(x, y, z);
                        }

                        sc.SetBackground(tokens[4]);
                        
                        break;
                    case "g":
                        if (tokens[1] == "r")
                        {
                            int dx = int.Parse(tokens[2]);
                            int dy = int.Parse(tokens[3]);
                            int w = int.Parse(tokens[4]);
                            int h = int.Parse(tokens[5]);
                            int gindex = int.Parse(tokens[6]);

                            sc.AddGround(dx, dy, w, h, tokens[7], sc.Grounds[gindex], CameraManager.Middleground);
                        }
                        else
                        {
                            float ax = float.Parse(tokens[1]);
                            float ay = float.Parse(tokens[2]);
                            int tw = int.Parse(tokens[3]);
                            int th = int.Parse(tokens[4]);

                            sc.AddGround(ax, ay, tw, th, tokens[5], CameraManager.Middleground);
                        }

                        break;
                    case "l":
                        int g1i = int.Parse(tokens[1]);
                        int t1i = int.Parse(tokens[2]);
                        int g2i = int.Parse(tokens[3]);
                        int t2i = int.Parse(tokens[4]);

                        sc.AddLadder(sc.Grounds[g1i].GetTilePosition(t1i), sc.Grounds[g2i].GetTilePosition(t2i), LayerManager.MainLayer.Middleground, LayerManager.DetailLayer.Back);

                        break;
                    case "n":
                        float nx = float.Parse(tokens[1]);
                        float ny = float.Parse(tokens[2]);

                        Node n = sc.AddNode(nx, ny, LayerManager.MainLayer.Background);

                        switch (tokens[3])
                        {
                            case "r":
                                sc.RightStart = n;
                                break;
                            case "l":
                                sc.LeftStart = n;
                                break;
                            case "u":
                                sc.UpStart = n;
                                break;
                            case "d":
                                sc.DownStart = n;
                                break;
                            case "f":
                                break;
                            case "b":
                                break;
                        }

                        break;
                    case "e":
                        int ne1 = int.Parse(tokens[1]);
                        int ne2 = int.Parse(tokens[2]);

                        Node.AddUndirectedEdge(sc.Nodes[ne1], sc.Nodes[ne2]);

                        break;
                    case "en":
                        int ni = int.Parse(tokens[1]);

                        List<Node> p = new List<Node>();
                        
                        int j;
                        for (int i = 2; i < tokens.Length; i++)
                        {
                            j = int.Parse(tokens[i]);

                            p.Add(sc.Nodes[j]);
                        }

                        WorldManager.Soldiers.Add(new Soldier("Global", p, 7.0f, CameraManager.Entity1));
                        WorldManager.Soldiers[WorldManager.Soldiers.Count - 1].Position = mCurScene.Nodes[ni].Position;
                        WorldManager.Soldiers[WorldManager.Soldiers.Count - 1].MyScene = sc;
                        break;
                }
                
            }
            

            /*Scene origin = AddScene();
            origin.SetBackground("bg1");
            origin.AddGround(-16.0f, -21.0f, 6, 3, "hill", LayerManager.MainLayer.Background, LayerManager.DetailLayer.Back);
            origin.AddGround(1, 2, 4, 3, "hill", origin.Grounds[0], LayerManager.MainLayer.Middleground, LayerManager.DetailLayer.Front);
            origin.AddLadder(origin.Grounds[0].GetTilePosition(1), origin.Grounds[1].GetTilePosition(0),
                             LayerManager.MainLayer.Middleground, LayerManager.DetailLayer.Back);
            origin.AddLadder(origin.Grounds[0].GetTilePosition(4), origin.Grounds[1].GetTilePosition(1),
                             LayerManager.MainLayer.Middleground, LayerManager.DetailLayer.Back);

            origin.AddScenery(0, 2, "bush0", LayerManager.MainLayer.Middleground, LayerManager.DetailLayer.Front); 

            mCurScene = origin;

            Scene next = AddScene(1, 0, 0);
            next.SetBackground("bg1");
            next.AddGround(-16.0f, -21.0f, 6, 3, "hill", LayerManager.MainLayer.Background, LayerManager.DetailLayer.Back);
            next.AddGround(1, 2, 4, 3, "hill", next.Grounds[0], LayerManager.MainLayer.Middleground, LayerManager.DetailLayer.Front);
            next.AddLadder(next.Grounds[0].GetTilePosition(1), next.Grounds[1].GetTilePosition(0),
                             LayerManager.MainLayer.Background, LayerManager.DetailLayer.Back);
            next.AddLadder(next.Grounds[0].GetTilePosition(4), next.Grounds[1].GetTilePosition(1),
                             LayerManager.MainLayer.Middleground, LayerManager.DetailLayer.Back);

            //SceneMoveRight();

            #region Create Nodes and Edges

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

            origin.LeftStart = n5;

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

            next.RightStart = n0b;

            #endregion

            //Node.DEBUG_VIEW();*/

            WorldManager.PlayerInstance = new Player2("Global", 10.0f);
            WorldManager.PlayerInstance.Position = mCurScene.Nodes[0].Position;
            WorldManager.PlayerInstance.Z = LayerManager.SetLayer(LayerManager.MainLayer.Entity2, LayerManager.DetailLayer.Middle);

            /*List<Node> patrol1 = new List<Node>();
            patrol1.Add(mCurScene.Nodes[1]);
            patrol1.Add(mCurScene.Nodes[2]);
            patrol1.Add(mCurScene.Nodes[3]);
            patrol1.Add(mCurScene.Nodes[4]);*/

            List<Node> patrol2 = new List<Node>();
            patrol2.Add(mCurScene.Right.Nodes[1]);
            patrol2.Add(mCurScene.Right.Nodes[2]);
            patrol2.Add(mCurScene.Right.Nodes[3]);
            patrol2.Add(mCurScene.Right.Nodes[4]);

            /*List<Node> patrol3 = new List<Node>();
            patrol3.Add(mCurScene.Nodes[4]);
            patrol3.Add(mCurScene.Nodes[3]);
            patrol3.Add(mCurScene.Nodes[2]);
            patrol3.Add(mCurScene.Nodes[1]);*/

            /*WorldManager.Soldiers.Add(new Soldier("Global", patrol1, 7.0f, CameraManager.Entity1));
            WorldManager.Soldiers[0].Position = mCurScene.Nodes[4].Position;
            WorldManager.Soldiers[0].MyScene = mCurScene;*/
            //WorldManager.Soldiers[0].KeepTrackOfReal = true;

            /*WorldManager.Soldiers.Add(new Soldier("Global", patrol3, 7.0f, CameraManager.Entity1));
            WorldManager.Soldiers[1].Position = mCurScene.Nodes[5].Position;
            WorldManager.Soldiers[1].MyScene = mCurScene;*/

            WorldManager.Target = new Noble("Global", patrol2, 5.0f, CameraManager.Entity1);
            WorldManager.Target.Position = mCurScene.Right.Nodes[4].Position;
            WorldManager.Target.MyScene = mCurScene.Right;

            Node.NodeListToUse = mCurScene.Nodes;
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
            if (mCurScene.Right != null)
                mCurScene = mCurScene.Right;
        }

        public static void SceneMoveLeft()
        {
            if (mCurScene.Left != null)
                mCurScene = mCurScene.Left;
        }

        public static void SceneMoveUp()
        {
            if (mCurScene.Up != null)
                mCurScene = mCurScene.Up;
        }

        public static void SceneMoveDown()
        {
            if (mCurScene.Down != null)
                mCurScene = mCurScene.Down;
        }

        public static void Clean()
        {
            Scene.Clear();
        }
    }
}
