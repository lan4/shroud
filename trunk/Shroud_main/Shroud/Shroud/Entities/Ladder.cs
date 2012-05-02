
using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;

using Microsoft.Xna.Framework;

using Shroud.Utilities;


namespace Shroud.Entities
{
    public class Ladder : Entity
    {
        #region Fields
        private int mHeight;
        private Sprite[] mGrid;

        private float mTileHeight;

        #endregion

        #region Properties

        #endregion

        #region Methods

        // Constructor
        public Ladder(string contentManagerName)
            : base(contentManagerName)
        {
            // Set the ContentManagerName and call Initialize:

            // If you don't want to add to managers, make an overriding constructor
            Initialize(true);
        }

        public Ladder(string contentManagerName, Vector3 pos1, Vector3 pos2, float tileSize, Layer layer)
            : base(contentManagerName)
        {
            mTileHeight = tileSize;
            mHeight = CalcHeight(pos1.X, pos2.X);
            // If you don't want to add to managers, make an overriding constructor
            Initialize(true, layer);
        }

        protected virtual void Initialize(bool addToManagers, Layer layer)
        {
            mGrid = new Sprite[mHeight];

            if (addToManagers)
            {
                AddToManagers(layer);
            }
        }

        public virtual void AddToManagers(Layer layerToAddTo)
        {
            InitializeGrid(layerToAddTo);

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);
        }

        private void InitializeGrid(Layer layer)
        {
            string name;
            for (int v = 0; v < mHeight; v++)
            {
                if (v == 0)
                {
                    name = @"Content/Entities/Background/Ladder/ladder_bottom";
                }
                else if (v == (mHeight - 1))
                {
                    name = @"Content/Entities/Background/Ladder/ladder_top";
                }
                else
                {
                    name = @"Content/Entities/Background/Ladder/ladder_middle";
                }
                mGrid[v] = SpriteManager.AddSprite(name, ContentManagerName, layer);
                mGrid[v].AttachTo(this, false);
                GameProperties.RescaleSprite(mGrid[v]);
                SetWorldPosition(v);
                mGrid[v].RelativeRotationZ = GameProperties.WorldRotation;
            }
        }

        private int CalcHeight(float y1, float y2)
        {
            float realHeight = (y2 - y1) / mTileHeight;
            int unitHeight = (int)Math.Round(realHeight);

            /*if (realHeight - unitHeight > 0.0f)
            {
                unitHeight += 1;
            }*/

            return Math.Abs(unitHeight);
        }

        private void SetWorldPosition(int v)
        {
            float tileHeight = mGrid[v].ScaleX;

            mGrid[v].RelativeX = (2 * tileHeight * v);
        }

        public virtual void Destroy()
        {
            base.Destroy();

            foreach (Sprite s in mGrid)
            {
                SpriteManager.RemoveSprite(s);
            }

            //ShapeManager.Remove(mCollision);
        }

        #endregion
    }
}
