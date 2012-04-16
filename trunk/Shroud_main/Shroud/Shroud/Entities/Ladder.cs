
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
    public class Ladder : Entity
    {
        #region Fields
        private int mHeight;
        private Sprite[] mGrid;

        private string mName;

        private const float TEXTURE_HEIGHT = 2.650967f;

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

        public Ladder(string contentManagerName, Vector3 pos1, Vector3 pos2)
            : base(contentManagerName)
        {
            mHeight = CalcHeight(pos1.Y, pos2.Y);
            this.X = pos1.Y;
            this.Y = pos1.X;
            // If you don't want to add to managers, make an overriding constructor
            Initialize(true);
        }

        protected virtual void Initialize(bool addToManagers)
        {
            mGrid = new Sprite[mHeight];

            if (addToManagers)
            {
                AddToManagers(null);
            }
        }

        public virtual void AddToManagers(Layer layerToAddTo)
        {
            SpriteManager.AddPositionedObject(this);

            InitializeGrid();

            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);
        }

        private void InitializeGrid()
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
                mGrid[v] = SpriteManager.AddSprite(name, ContentManagerName);
                mGrid[v].AttachTo(this, false);
                GameProperties.RescaleSprite(mGrid[v]);
                SetWorldPosition(v);
            }

            this.RotationZ = GameProperties.WorldRotation;
        }

        private int CalcHeight(float y1, float y2)
        {
            float realHeight = y2 - y1;
            int unitHeight = (int)(realHeight / TEXTURE_HEIGHT);

            return Math.Abs(unitHeight);
        }

        private void SetWorldPosition(int v)
        {
            float vGridSize = mGrid[v].ScaleY;

            mGrid[v].RelativeY = (2 * vGridSize * v);
        }

        public virtual void Destroy()
        {
            base.Destroy();
        }

        #endregion
    }
}
