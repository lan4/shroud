using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;

using Microsoft.Xna.Framework;

using Shroud.Utilities;

// Be sure to replace:
// 1.  The namespace
// 2.  The class name
// 3.  The constructor (should be the same as the class name)


namespace Shroud.Entities
{
    public class Ground : Entity
    {
        #region Fields
        private int mWidth;
        private int mHeight;
        private float mTileWidth;
        private float mTileHeight;
        private Sprite[,] mTiles;

        #endregion

        #region Properties

        public float TileWidth
        {
            get { return 2 * mTileWidth; }
        }

        public float TileHeight
        {
            get { return 2 * mTileHeight; }
        }

        public Vector3 LeftEnd
        {
            get
            {
                Vector3 temp = mTiles[mHeight - 1, 0].Position;
                temp.X += mTiles[mHeight - 1, 0].ScaleX;
                return temp;
            }
        }

        public Vector3 RightEnd
        {
            get
            {
                Vector3 temp = mTiles[mHeight - 1, mWidth -1].Position;
                temp.X += mTiles[mHeight - 1, mWidth - 1].ScaleX;
                return temp;
            }
        }

        #endregion

        #region Methods

        // Constructor
        public Ground(string contentManagerName)
            : base(contentManagerName)
        {
            // Set the ContentManagerName and call Initialize:

            // If you don't want to add to managers, make an overriding constructor
            Initialize(true);
        }

        public Ground(string contentManagerName, int height, int width)
            : base(contentManagerName)
        {
            mName = "hill";

            // Set the ContentManagerName and call Initialize:
            mWidth = width;
            mHeight = height;

            // If you don't want to add to managers, make an overriding constructor
            Initialize(true);
        }

        protected virtual void Initialize(bool addToManagers)
        {
            mTiles = new Sprite[mHeight, mWidth];

            if (addToManagers)
            {
                AddToManagers(null);
            }
        }

        public virtual void AddToManagers(Layer layerToAddTo)
        {
            InitializeGrid();

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);
        }

        private void InitializeGrid()
        {
            string name;
            for (int u = 0; u < mWidth; u++) {
                for (int v = 0; v < mHeight; v++) {
                    if (u == 0)
                    {
                        if (v == (mHeight - 1))
                            name = @"Content/Entities/Background/Ground/topleft_" + mName;
                        else if (v == 0)
                            name = @"Content/Entities/Background/Ground/bottomleft_" + mName;
                        else
                            name = @"Content/Entities/Background/Ground/middleleft_" + mName;
                    }
                    else if (u == mWidth - 1)
                    {
                        if (v == (mHeight - 1))
                            name = @"Content/Entities/Background/Ground/topright_" + mName;
                        else if (v == 0)
                            name = @"Content/Entities/Background/Ground/bottomright_" + mName;
                        else
                            name = @"Content/Entities/Background/Ground/middleright_" + mName;
                    }
                    else
                    {
                        if (v == (mHeight - 1))
                            name = @"Content/Entities/Background/Ground/topcenter_" + mName;
                        else if (v == 0)
                            name = @"Content/Entities/Background/Ground/bottomcenter_" + mName;
                        else
                            name = @"Content/Entities/Background/Ground/middlecenter_" + mName;
                    }

                    mTiles[v, u] = SpriteManager.AddSprite(name, ContentManagerName, CameraManager.Middleground);
                    mTiles[v, u].AttachTo(this, false);
                    GameProperties.RescaleSprite(mTiles[v, u]);
                    SetWorldPosition(u, v);
                    mTiles[v, u].RelativeRotationZ = GameProperties.WorldRotation;
                }
            }
            mTileWidth = mTiles[0, 0].ScaleY;
            mTileHeight = mTiles[0, 0].ScaleX;

        }

        private void SetWorldPosition(int u, int v)
        {
            mTileWidth = mTiles[v, u].ScaleY;
            mTileHeight = mTiles[v, u].ScaleX;
            float xPos = -mTiles[v, u].RelativeY - mTileWidth - (2 * mTileWidth * u);
            float yPos = mTiles[v, u].RelativeX + mTileHeight + (2 * mTileHeight * v);

            mTiles[v, u].RelativeX = yPos;
            mTiles[v, u].RelativeY = xPos;
        }

        public virtual void Destroy()
        {
            base.Destroy();
            foreach (Sprite s in mTiles)
            {
                SpriteManager.RemoveSprite(s);
            }

            //ShapeManager.Remove(mCollision);
        }

        public Vector3 GetTilePosition(int pos) 
        {
            Vector3 temp = mTiles[mHeight - 1, pos].RelativePosition;
            temp.Y += this.Position.Y;
            temp.X += this.Position.X;

            return temp;
        }

        #endregion
    }
}
