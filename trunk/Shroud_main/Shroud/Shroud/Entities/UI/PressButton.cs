using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall;

using Shroud.Utilities;

namespace Shroud.Entities.UI
{
    public class PressButton : GenericButton
    {
        private delegate void Fired();
        private Fired Press;

        public PressButton(string contentManagerName, string assetname, Action m) : base(contentManagerName)
        {
            Press = new Fired(m);

            InitializeSprite(assetname);
        }

        private void InitializeSprite(string assetname)
        {
            mAppearance = SpriteManager.AddSprite(@"Content/Entities/Button/" + assetname, ContentManagerName);
            mAppearance.AttachTo(this, false);
            mAppearance.RelativeRotationZ = GameProperties.WorldRotation;
            GameProperties.RescaleSprite(mAppearance);
        }

        public void Fire()
        {
            WorldManager.justFired = this;
            Press.Invoke();
        }
    }
}
