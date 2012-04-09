using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall.Math.Geometry;

namespace Shroud.Entities.UI
{
    public abstract class GenericButton : UIElement
    {
        protected GenericButton(string contentManagerName)
            : base(contentManagerName)
        {
            mCollision = ShapeManager.AddCircle();
            mCollision.AttachTo(this, false);
            mCollision.Radius = 2.0f;
        }
    }
}
