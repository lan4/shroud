using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Math.Geometry;

using Shroud.Utilities;

namespace Shroud.Entities.UI
{
    public class ToggleButton : GenericButton
    {
        public bool IsOn
        {
            get { return mAppearance.CurrentChainName == "ON"; }
        }

        public ToggleButton(string contentManagerName, string assetname)
            : base(contentManagerName)
        {
            InitializeAnimations(assetname);
        }

        private void InitializeAnimations(string assetname)
        {
            AnimationChainList buttonSprites = new AnimationChainList();

            AnimationChain buttonOn = new AnimationChain();

            buttonOn.Add(new AnimationFrame(@"Content/Entities/Button/" + assetname + "_ON", 0.0833f, ContentManagerName));

            buttonOn.Name = "ON";

            AnimationChain buttonOff = new AnimationChain();

            buttonOff.Add(new AnimationFrame(@"Content/Entities/Button/" + assetname + "_OFF", 0.0833f, ContentManagerName));

            buttonOff.Name = "OFF";

            buttonSprites.Add(buttonOn);
            buttonSprites.Add(buttonOff);

            mAppearance = SpriteManager.AddSprite(buttonSprites);
            mAppearance.AttachTo(this, false);
            mAppearance.RelativeRotationZ = GameProperties.WorldRotation;
            mAppearance.CurrentChainName = "OFF";

            GameProperties.RescaleSprite(mAppearance);
        }

        public void Toggle()
        {
            if (mAppearance.CurrentChainName == "OFF")
                mAppearance.CurrentChainName = "ON";
            else if (mAppearance.CurrentChainName == "ON")
                mAppearance.CurrentChainName = "OFF";
        }
    }
}
