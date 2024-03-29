using System;
using System.Collections.Generic;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Math.Geometry;


// Be sure to replace:
// 1.  The namespace
// 2.  The class name
// 3.  The constructor (should be the same as the class name)


namespace Shroud.Entities
{
    public class Building : Entity
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Methods

        public Building(string contentManagerName) : base(contentManagerName)
        {
            Initialize(true);
        }

        protected virtual void Initialize(bool addToManagers)
        {
            if (addToManagers)
            {
                AddToManagers(null);
            }
        }

        public virtual void AddToManagers(Layer layerToAddTo)
        {
            
        }

        public virtual void Activity()
        {

        }

        public virtual void Destroy()
        {
            base.Destroy();
        }

        #endregion
    }
}
