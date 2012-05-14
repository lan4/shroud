using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using FlatRedBall;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Graphics;

using Shroud.Entities;
using MainLayer = Shroud.Utilities.LayerManager.MainLayer;
using DetailLayer = Shroud.Utilities.LayerManager.DetailLayer;
using ObjectType = Shroud.Entities.WorldObject.ObjectType;

namespace Shroud.Utilities
{
    public class Scene
    {
        private static float WIDTH = 350.0f;
        private static float HEIGHT = 200.0f;
        private static float DEPTH = 10.0f;

        private static List<Scene> mScenes = new List<Scene>();
        private static int mRealSize = 0;

        public List<Node> Nodes;
        public List<WorldObject> WorldObjects;
        public List<Ground> Grounds;
        public List<Ladder> Ladders;
        public List<Sprite> SceneryObjects;
        public List<Sprite> StealthObjects;

        public Node LeftStart;
        public Node RightStart;
        public Node UpStart;
        public Node DownStart;

        private Vector3 mAnchor;
        private List<Scene> Neighbors;
        private int SceneX;
        private int SceneY;
        private int SceneZ;
        public Sprite mBG;

        public Vector3 WorldAnchor
        {
            get { return mAnchor; }
        }

        public Scene Left
        {
            get
            {
                foreach (Scene b in Neighbors)
                {
                    if (b.SceneX == SceneX - 1)
                    {
                        return b;
                    }
                }

                return null;
            }

            set
            {
                Scene b = value;
                if (Left == null && b.Right == null)
                {
                    Neighbors.Add(b);
                    b.Neighbors.Add(this);
                    b.SceneX = SceneX - 1;
                    b.SceneY = SceneY;
                    b.SceneZ = SceneZ;
                    b.mAnchor.X = mAnchor.X - WIDTH;
                    b.mAnchor.Y = mAnchor.Y;
                    b.mAnchor.Z = mAnchor.Z;
                }
            }
        }

        public Scene Right
        {
            get
            {
                foreach (Scene b in Neighbors)
                {
                    if (b.SceneX == SceneX + 1)
                    {
                        return b;
                    }
                }

                return null;
            }

            set
            {
                Scene b = value;
                if (Right == null && b.Left == null)
                {
                    Neighbors.Add(b);
                    b.Neighbors.Add(this);
                    b.SceneX = SceneX + 1;
                    b.SceneY = SceneY;
                    b.SceneZ = SceneZ;
                    b.mAnchor.X = mAnchor.X + WIDTH;
                    b.mAnchor.Y = mAnchor.Y;
                    b.mAnchor.Z = mAnchor.Z;
                }
            }
        }

        public Scene Up
        {
            get
            {
                foreach (Scene b in Neighbors)
                {
                    if (b.SceneY == SceneY + 1)
                    {
                        return b;
                    }
                }

                return null;
            }

            set
            {
                Scene b = value;
                if (Up == null && b.Down == null)
                {
                    Neighbors.Add(b);
                    b.Neighbors.Add(this);
                    b.SceneX = SceneX;
                    b.SceneY = SceneY + 1;
                    b.SceneZ = SceneZ;
                    b.mAnchor.X = mAnchor.X;
                    b.mAnchor.Y = mAnchor.Y + HEIGHT;
                    b.mAnchor.Z = mAnchor.Z;
                }
            }
        }

        public Scene Down
        {
            get
            {
                foreach (Scene b in Neighbors)
                {
                    if (b.SceneY == SceneY - 1)
                    {
                        return b;
                    }
                }

                return null;
            }

            set
            {
                Scene b = value;
                if (Down == null && b.Up == null)
                {
                    Neighbors.Add(b);
                    b.Neighbors.Add(this);
                    b.SceneX = SceneX;
                    b.SceneY = SceneY - 1;
                    b.SceneZ = SceneZ;
                    b.mAnchor.X = mAnchor.X;
                    b.mAnchor.Y = mAnchor.Y - HEIGHT;
                    b.mAnchor.Z = mAnchor.Z;
                }
            }
        }

        public Scene Front
        {
            get
            {
                foreach (Scene b in Neighbors)
                {
                    if (b.SceneZ == SceneZ - 1)
                    {
                        return b;
                    }
                }

                return null;
            }

            set
            {
                Scene b = value;
                if (Front == null && b.Back == null)
                {
                    Neighbors.Add(b);
                    b.Neighbors.Add(this);
                    b.SceneX = SceneX;
                    b.SceneY = SceneY;
                    b.SceneZ = SceneZ - 1;
                    b.mAnchor.X = mAnchor.X;
                    b.mAnchor.Y = mAnchor.Y;
                    b.mAnchor.Z = mAnchor.Z - DEPTH;
                }
            }
        }

        public Scene Back
        {
            get
            {
                foreach (Scene b in Neighbors)
                {
                    if (b.SceneZ == SceneZ + 1)
                    {
                        return b;
                    }
                }

                return null;
            }

            set
            {
                Scene b = value;
                if (Back == null && b.Front == null)
                {
                    Neighbors.Add(b);
                    b.Neighbors.Add(this);
                    b.SceneX = SceneX;
                    b.SceneY = SceneY;
                    b.SceneZ = SceneZ + 1;
                    b.mAnchor.X = mAnchor.X;
                    b.mAnchor.Y = mAnchor.Y;
                    b.mAnchor.Z = mAnchor.Z + DEPTH;
                }
            }
        }

        private Scene()
        {
            Nodes = new List<Node>();
            mAnchor = new Vector3();
            WorldObjects = new List<WorldObject>();
            Neighbors = new List<Scene>();
            Grounds = new List<Ground>();
            Ladders = new List<Ladder>();
            SceneryObjects = new List<Sprite>();
            StealthObjects = new List<Sprite>();

            SceneX = -123456;
            SceneY = -123456;
            SceneZ = -123456;

            mBG = null;

            mScenes.Add(this);
        }

        private Scene(Vector3 anchor)
        {
            Nodes = new List<Node>();
            mAnchor = anchor;
            WorldObjects = new List<WorldObject>();
            Neighbors = new List<Scene>();
            Grounds = new List<Ground>();
            Ladders = new List<Ladder>();
            SceneryObjects = new List<Sprite>();
            StealthObjects = new List<Sprite>();

            SceneX = 0;
            SceneY = 0;
            SceneZ = 0;

            mBG = null;

            mScenes.Add(this);
        }

        /*public void AddEnemy(Enemy1.EnemyType et, Enemy1.PatrolMode pm, float x, float y, MainLayer m, DetailLayer d)
        {
            Enemy1 e = new Enemy1("Global", et, pm);
            Enemies.Add(e);
            e.X += mAnchor.X;
            e.Y += mAnchor.Y;
            e.Z = mAnchor.Z + LayerManager.SetLayer(m, d);

            if (!WorldManager.ManagedEnemies.Contains(e))
                WorldManager.ManagedEnemies.Add(e);
        }

        public void RemoveEnemy(Enemy1 e)
        {
            Enemies.Remove(e);
        }*/

		#region Graph Creation
		
		public Node AddNode(float x, float y, float zOffset)
        {
            Node n = Node.AddGraphNode(x + mAnchor.X, y + mAnchor.Y, mAnchor.Z + zOffset);
            Nodes.Add(n);

            return n;
        }

        public void CreateLink(float x1, float y1, float x2, float y2, float z1, float z2)
        {
            Node n1 = AddNode(x1, y1, z1);
            Node n2 = AddNode(x2, y2, z2);

            n1.AddUndirectedEdge(n2);
        }

        public void CreateMovementGraph()
        {
            foreach (WorldObject w in WorldObjects)
            {
                if (w.OType.Equals(ObjectType.Ground))
                {
                    Unwrap(w.Collision);
                }
            }

            foreach (Ground g in Grounds)
            {
                
            }

            Node n1 = null;
            Node n2 = null;

            foreach (Node n in Nodes)
            {
                if (!n.HasNeighbors && n.IsLink)
                {
                    Node.FindClosestLinePoints(n, Nodes, ref n1, ref n2);

                    n1.RemoveUndirectedEdge(n2);
                    n1.AddUndirectedEdge(n);
                    n2.AddUndirectedEdge(n);
                }
            }
        }

        // PLACES NODES AT TOP POINTS OF RECTANGLE
        private void Unwrap(AxisAlignedRectangle r)
        {
            Node n1 = AddNode(r.X + r.Right, r.Y + r.Top, r.Z);
            Node n2 = AddNode(r.X + r.Right, r.Y + r.Bottom, r.Z);

            n1.AddUndirectedEdge(n2);
        }

        #endregion

        public void AddGround(float x, float y, int width, int height, string tileSet, Layer layer, float zOffset)
        {
            Ground g = new Ground("Global", height, width, tileSet, layer);
            Grounds.Add(g);
            g.X = y + mAnchor.X;
            g.Y = -x + mAnchor.Y;
            g.Z = mAnchor.Z + zOffset;
        }

        public void AddGround(int dx, int dy, int width, int height, string tileSet, Ground relativeG, Layer layer, float zOffset)
        {
            Ground g = new Ground("Global", height, width, tileSet, layer);
            Grounds.Add(g);
            g.X = relativeG.X + (dy * Ground.TileHeight);
            g.Y = relativeG.Y - (dx * Ground.TileWidth);
            g.Z = mAnchor.Z + zOffset;
        }

        public void AddLadder(Vector3 p1, Vector3 p2, Layer layer, float zOffset)
        {
            Ladder l = new Ladder("Global", p1, p2, Ground.TileHeight, layer);
            Ladders.Add(l);
            l.X = p1.X + Ground.TileHeight * .85f;
            l.Y = p1.Y;
            l.Z = mAnchor.Z + zOffset;
        }

        public void AddBuilding(Vector3 p, int type, MainLayer m, DetailLayer d)
        {

        }

        public void AddScenery(Vector3 p, string name, Layer layer, float zOffset, bool UsedForStealth, float scale)
        {
            Sprite temp = SpriteManager.AddSprite(@"Content/Entities/Background/Scenery/" + name, "Global", layer);

            if (scale > 0.0f)
                GameProperties.RescaleSprite(temp, scale);
            else
                GameProperties.RescaleSprite(temp);                         

            temp.Position = p;
            temp.X += mAnchor.X;
            temp.Y += mAnchor.Y;
            temp.Z = mAnchor.Z + zOffset;
            temp.RotationZ = GameProperties.WorldRotation;
            SceneryObjects.Add(temp);

            if (UsedForStealth)
                StealthObjects.Add(temp);
        }

        public void AddScenery(int groundNum, int tileNum, string name, Layer layer, float zOffset, bool UsedForStealth, float scale)
        {
            Sprite temp = SpriteManager.AddSprite(@"Content/Entities/Background/Scenery/" + name, "Global", layer);
            if (scale > 0.0f)
                GameProperties.RescaleSprite(temp, scale);
            else
                GameProperties.RescaleSprite(temp);
            temp.Position = Grounds[groundNum].GetTilePosition(tileNum);
            temp.X += Ground.TileHeight / 1.25f;
            temp.Z = mAnchor.Z + zOffset;
            temp.RotationZ = GameProperties.WorldRotation;
            SceneryObjects.Add(temp);

            if (UsedForStealth)
                StealthObjects.Add(temp);
        }

        public void SetBackground(string filename)
        {
            if (mBG == null)
            {
                mBG = SpriteManager.AddSprite(@"Content/Entities/Background/" + filename, "Global", CameraManager.Background);
                GameProperties.RescaleSprite(mBG);
                mBG.RotationZ = GameProperties.WorldRotation;
                mBG.X = WorldAnchor.X;
                mBG.Y = WorldAnchor.Y;
                //mBG.Z = WorldAnchor.Z + LayerManager.SetLayer(MainLayer.Background, DetailLayer.Back) + 1.0f;
            }
            else
                System.Diagnostics.Debug.WriteLine("Warning: Could not set background of Scene (" + SceneX + ", " + SceneY + ", " + SceneZ + ") because it was already set.");
        }

        private void Destroy()
        {
            Nodes.Clear();
            //Enemies.Clear();
            WorldObjects.Clear();
            Neighbors.Clear();
            SpriteManager.RemoveSprite(mBG);
            mBG = null;

            foreach (Ground g in Grounds)
            {
                g.Destroy();
            }

            Grounds.Clear();

            foreach (Ladder l in Ladders)
            {
                l.Destroy();
            }

            Ladders.Clear();

            foreach (Sprite s in SceneryObjects)
            {
                SpriteManager.RemoveSprite(s);
            }

            SceneryObjects.Clear();

            StealthObjects.Clear();
        }

        public static Scene Create()
        {
            mRealSize++;

            if (mScenes.Count == 0)
                return new Scene(new Vector3(0.0f, 0.0f, 0.0f));
            else if (mRealSize >= mScenes.Count)
                return new Scene();
            else
                return mScenes[mRealSize - 1];
        }

        // WARNING RETURNS NULL IF IT COULD NOT CREATE AT DESIRED LOCATION
        public static Scene Create(int x, int y, int z)
        {
            if (x == 0 && y == 0 && z == 0)
                return Create();

            Scene baseScene;

            baseScene = Scene.Find(x - 1, y, z);
            if (baseScene != null)
            {
                baseScene.Right = Create();
                return baseScene.Right;
            }
            
            baseScene = Scene.Find(x + 1, y, z);
            if (baseScene != null)
            {
                baseScene.Left = Create();
                return baseScene.Left;
            }
            
            baseScene = Scene.Find(x, y - 1, z);
            if (baseScene != null)
            {
                baseScene.Up = Create();
                return baseScene.Up;
            }
            
            baseScene = Scene.Find(x, y + 1, z);
            if (baseScene != null)
            {
                baseScene.Down = Create();
                return baseScene.Down;
            }
            
            baseScene = Scene.Find(x, y, z - 1);
            if (baseScene != null)
            {
                baseScene.Back = Create();
                return baseScene.Back;
            }
            
            baseScene = Scene.Find(x, y, z + 1);
            if (baseScene != null)
            {
                baseScene.Front = Create();
                return baseScene.Front;
            }

            return null;
        }

        public static void Clear()
        {
            foreach (Scene b in mScenes)
            {
                b.Destroy();
            }

            mRealSize = 0;
        }

        public static Scene Find(int x, int y, int z)
        {
            Scene b = mScenes[0];

            while (x > 0 && b != null)
            {
                b = b.Right;
                x--;
            }

            while (x < 0 && b != null)
            {
                b = b.Left;
                x++;
            }

            while (y > 0 && b != null)
            {
                b = b.Up;
                y--;
            }

            while (y < 0 && b != null)
            {
                b = b.Down;
                y++;
            }

            while (z > 0 && b != null)
            {
                b = b.Back;
                z--;
            }

            while (z < 0 && b != null)
            {
                b = b.Front;
                z++;
            }

            return b;
        }
    }
}
