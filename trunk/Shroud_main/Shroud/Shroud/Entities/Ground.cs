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
        private int mXSize;
        private int mYSize;
        private Sprite[,] mGrid;

        private string mName;

        #endregion

        #region Properties

        public Vector3 LeftEnd
        {
            get
            {
                Vector3 temp = mGrid[0, 0].Position;
                temp.Y += mGrid[0, 0].ScaleY;
                return temp;
            }
        }

        public Vector3 RightEnd
        {
            get
            {
                Vector3 temp = mGrid[mXSize - 1, 0].Position;
                temp.Y += mGrid[mXSize - 1, 0].ScaleY;
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

        public Ground(string contentManagerName, int xsize, int ysize)
            : base(contentManagerName)
        {
            mName = "hill";

            // Set the ContentManagerName and call Initialize:
            mXSize = xsize;
            mYSize = ysize;

            // If you don't want to add to managers, make an overriding constructor
            Initialize(true);
        }

        protected virtual void Initialize(bool addToManagers)
        {
            mGrid = new Sprite[mXSize, mYSize];

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
            for (int v = 0; v < mYSize; v++)
            {
                for (int u = 0; u < mXSize; u++)
                {
                    if (u == 0)
                    {
                        if (v == (mYSize - 1))
                                name = @"Content/Entities/Background/Ground/topleft_" + mName;
                        else if (v == 0)
                                name = @"Content/Entities/Background/Ground/bottomleft_" + mName;
                        else
                                name = @"Content/Entities/Background/Ground/middleleft_" + mName;
                    }
                    else if (u == (mXSize - 1))
                    {
                        if (v == (mYSize - 1))
                            name = @"Content/Entities/Background/Ground/topright_" + mName;
                        else if (v == 0)
                            name = @"Content/Entities/Background/Ground/bottomright_" + mName;
                        else
                            name = @"Content/Entities/Background/Ground/middleright_" + mName;
                    }
                    else
                    {
                        if (v == (mYSize - 1))
                            name = @"Content/Entities/Background/Ground/topcenter_" + mName;
                        else if (v == 0)
                            name = @"Content/Entities/Background/Ground/bottomcenter_" + mName;
                        else
                            name = @"Content/Entities/Background/Ground/middlecenter_" + mName;
                    }
                    mGrid[u, v] = SpriteManager.AddSprite(name, ContentManagerName);
                    mGrid[u, v].AttachTo(this, false);
                    GameProperties.RescaleSprite(mGrid[u, v]);
                    SetWorldPosition(u, v);
                }
            }

            this.RotationZ = GameProperties.WorldRotation;
        }

        private void SetWorldPosition(int u, int v)
        {
            float uGridSize = mGrid[u, v].ScaleX;
            float vGridSize = mGrid[u, v].ScaleY;
            float uStart = uGridSize * mXSize;
            float vStart = vGridSize * mYSize;

            mGrid[u, v].RelativeX = -uStart / 2 + (2 * uGridSize * u); 
            mGrid[u, v].RelativeY = -vStart / 2 + (2 * vGridSize * v);
        }

        public virtual void Destroy()
        {
            base.Destroy();
        }

        public Vector3 GetUnitCoord(int pos) 
        {
            Vector3 temp = mGrid[pos, mYSize - 1].RelativePosition;
            temp.Y -= (Position.Y + mGrid[pos, mYSize - 1].ScaleY);
            temp.X += this.Position.X;
            return temp;
        }

        #endregion
    }
}
