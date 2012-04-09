using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shroud.Entities;

using FlatRedBall;

namespace Shroud.Utilities
{
    public static class HUDManager
    {
        public static Dictionary<string, Button>.ValueCollection ManagedButtons
        { 
            get { return ManagedButtons2.Values; } 
        }

        private static Dictionary<string, Button> ManagedButtons2 = new Dictionary<string, Button>();
        public static float zUI = 40.0f;

        public static void Update()
        {
            if (WorldManager.InteractTarget != null && WorldManager.InteractTarget.GetType().Equals(typeof(Button)))
            {
                Button b = (Button)WorldManager.InteractTarget;

                if (!b.On)
                    b.Toggle();
            }
        }

        public static void Destroy()
        {
            foreach (KeyValuePair<string, Button> p in ManagedButtons2)
            {
                p.Value.Destroy();
            }

            ManagedButtons2.Clear();
        }

        public static void RegisterButton(string id)
        {
            if (!ManagedButtons2.ContainsKey(id))
            {
                ManagedButtons2.Add(id, new Button("Global", id));

                if (ManagedButtons2.ContainsKey(id))
                {
                    ManagedButtons2[id].AttachTo(SpriteManager.Camera, false);
                    ManagedButtons2[id].RelativeZ = -zUI;
                }
            }
        }

        public static bool IsButtonDown(string id)
        {
            if (ManagedButtons2.ContainsKey(id))
            {
                return ManagedButtons2[id].On;
            }

            return false;
        }

        public static void HideButton(string id)
        {
            if (ManagedButtons2.ContainsKey(id))
            {
                if (ManagedButtons2[id].On)
                    ManagedButtons2[id].Toggle();

                ManagedButtons2[id].Visible = false;
            }
        }

        public static void ShowButton(string id)
        {
            if (ManagedButtons2.ContainsKey(id))
            {
                ManagedButtons2[id].Visible = true;
            }
        }

        public static void PlaceButton(string id, float x, float y)
        {
            if (ManagedButtons2.ContainsKey(id))
            {
                ManagedButtons2[id].RelativeX = x;
                ManagedButtons2[id].RelativeY = y;
            }
        }
    }
}
