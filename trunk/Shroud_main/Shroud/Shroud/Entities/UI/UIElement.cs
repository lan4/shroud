using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shroud.Entities.UI
{
    public abstract class UIElement : Entity
    {
        public bool IsActive
        {
            get { return mAppearance.Visible; }
            set { mAppearance.Visible = value; }
        }

        protected UIElement(string contentManagerName)
            : base(contentManagerName)
        {

        }
    }
}
