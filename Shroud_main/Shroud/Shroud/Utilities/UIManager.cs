using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Shroud.Entities.UI;

using FlatRedBall;

namespace Shroud.Utilities
{
    public static class UIManager
    {
        private static Dictionary<string, GenericButton> mManagedButtons = new Dictionary<string, GenericButton>();
        private static float DEFAULT_CAMERA_Z = 40.0f;
        
        public static float zUI 
        {
            get { return SpriteManager.Camera.Z - DEFAULT_CAMERA_Z + 5.0f; }
        }

        public static PressButton RETRIEVE_PRESSBUTTON(string id)
        {
            if (mManagedButtons.ContainsKey(id))
            {
                if (mManagedButtons[id].GetType().Equals(typeof(PressButton)))
                    return (PressButton)mManagedButtons[id];
            }

            return null;
        }

        public static bool CheckButtonPressed()
        {
            foreach (GenericButton b in mManagedButtons.Values)
            {
                if (b.IsActive && b.Collision.IsPointInside(GestureManager.mEndTouchUI.X, GestureManager.mEndTouchUI.Y))
                {
                    if (b.GetType().Equals(typeof(ToggleButton)))
                    {
                        ToggleButton t = (ToggleButton)b;
                        t.Toggle();
                    }
                    else if (b.GetType().Equals(typeof(PressButton)))
                    {
                        PressButton p = (PressButton)b;
                        p.Fire();
                    }

                    return true;
                }
            }

            return false;
        }

        public static void RegisterToggleButton(string id, string type)
        {
            mManagedButtons.Add(id, new ToggleButton("Global", type));
        }

        public static void RegisterPressButton(string id, string type, Action m)
        {
            mManagedButtons.Add(id, new PressButton("Global", type, m));
        }

        public static void DisplayButton(string id, float x, float y)
        {
            if (mManagedButtons.ContainsKey(id))
            {
                mManagedButtons[id].X = x;
                mManagedButtons[id].Y = y;
                mManagedButtons[id].Z = zUI;
                mManagedButtons[id].IsActive = true;
            }
        }

        public static void HideButton(string id)
        {
            if (mManagedButtons.ContainsKey(id))
            {
                mManagedButtons[id].IsActive = false;
            }
        }

        public static bool IsButtonOn(string id)
        {
            GenericButton b = mManagedButtons[id];

            if (b.GetType().Equals(typeof(ToggleButton)))
            {
                return ((ToggleButton)b).IsOn;
            }

            return false;
        }

        public static void Clear()
        {
            foreach (GenericButton b in mManagedButtons.Values)
            {
                b.Destroy();
            }

            mManagedButtons.Clear();


        }
    }
}
