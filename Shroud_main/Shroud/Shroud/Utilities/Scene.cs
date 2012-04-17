using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using FlatRedBall;
using FlatRedBall.Math.Geometry;

using Shroud.Entities;
using MainLayer = Shroud.Utilities.LayerManager.MainLayer;
using DetailLayer = Shroud.Utilities.LayerManager.DetailLayer;
using ObjectType = Shroud.Entities.WorldObject.ObjectType;

namespace Shroud.Utilities
{
    public class Scene
    {
        private static float WIDTH = 30.0f;
        private static float HEIGHT = 16.0f;
        private static float DEPTH = 5.0f;

        private static List<Scene> mScenes = new List<Scene>();
        private static int mRealSize = 0;

        public List<Node> Nodes;
        public List<Enemy1> Enemies;
        public List<WorldObject> WorldObjects;
        public List<Ground> Grounds;
        public List<Ladder> Ladders;

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
            Enemies = new List<Enemy1>();
            WorldObjects = new List<WorldObject>();
            Neighbors = new List<Scene>();
            Grounds = new List<Ground>();
            Ladders = new List<Ladder>();

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
            Enemies = new List<Enemy1>();
            WorldObjects = new List<WorldObject>();
            Neighbors = new List<Scene>();
            Grounds = new List<Ground>();
            Ladders = new List<Ladder>();

            SceneX = 0;
            SceneY = 0;
            SceneZ = 0;

            mBG = null;

            mScenes.Add(this);
        }

        public void AddEnemy(Enemy1.EnemyType et, Enemy1.PatrolMode pm, float x, float y, MainLayer m, DetailLayer d)
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
        }


        public void AddGround(float x, float y, int width, int height, MainLayer m, DetailLayer d)
        {
            Ground g = new Ground("Global", height, width);
            Grounds.Add(g);
            g.X = y;
            g.Y = -x;
            g.Z = mAnchor.Z + LayerManager.SetLayer(m, d);
        }

        public void AddGround(int dx, int dy, int width, int height, Ground relativeG, MainLayer m, DetailLayer d)
        {
            Ground g = new Ground("Global", height, width);
            Grounds.Add(g);
            g.X = relativeG.X + (dy * relativeG.TileHeight);
            g.Y = relativeG.Y - (dx * relativeG.TileWidth);
            g.Z = mAnchor.Z + LayerManager.SetLayer(m, d);
        }

        public void AddLadder(Vector3 p1, Vector3 p2, float tileSize, MainLayer m, DetailLayer d)
        {
            Ladder l = new Ladder("Global", p1, p2, tileSize);
            Ladders.Add(l);
            l.X = p1.X + tileSize * .8f;
            l.Y = p1.Y;
            l.Z = mAnchor.Z + LayerManager.SetLayer(m, d);
        }

        public void AddWorldObject(float x, float y, MainLayer m, DetailLayer d)
        {
            WorldObject w = new WorldObject("Global");
            WorldObjects.Add(w);
            w.X += mAnchor.X;
            w.Y += mAnchor.Y;
            w.Z = mAnchor.Z + LayerManager.SetLayer(m, d);

            WorldManager.ManagedWorldObjects.Add(w);
        }

        public void SetBackground(string filename)
        {
            if (mBG == null)
            {
                mBG = SpriteManager.AddSprite(@"Content/Entities/Background/" + filename, "Global");
                mBG.X = WorldAnchor.X;
                mBG.Y = WorldAnchor.Y;
                mBG.Z = WorldAnchor.Z + LayerManager.SetLayer(MainLayer.Background, DetailLayer.Back) + 1.0f;
            }
            else
                System.Diagnostics.Debug.WriteLine("Warning: Could not set background of Scene (" + SceneX + ", " + SceneY + ", " + SceneZ + ") because it was already set.");
        }

        private void Destroy()
        {
            Nodes.Clear();
            Enemies.Clear();
            WorldObjects.Clear();
            Neighbors.Clear();
            SpriteManager.RemoveSprite(mBG);
            mBG = null;
        }

        public static Scene Create()
        {
            mRealSize++;

            if (mScenes.Count == 0)
                return new Scene(new Vector3(0.0f, 0.0f, 0.0f));
            else if (mRealSize == mScenes.Count)
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
