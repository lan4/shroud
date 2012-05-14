using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall;
using FlatRedBall.Math.Geometry;

using Shroud.Entities;
using Shroud.Entities.UI;
using Gesture = Shroud.Utilities.GestureManager.Gesture;

namespace Shroud.Utilities
{
    public static class WorldManager
    {
        // Managed Entities
        //public static List<DestructibleObject> ManagedDestructibleObjects;
        //public static List<InteractObject> ManagedInteractObjects;
        //public static List<WorldObject> ManagedWorldObjects;
        //public static List<Trap> ManagedTraps;
        //public static List<Enemy1> ManagedEnemies;
        public static List<Sprite> ManagedSprites;
        public static Player2 PlayerInstance;
        //public static Enemy1 Target;
        public static PositionedObject InteractTarget;
        public static PressButton justFired = null;
        //public static Button FAKE_BUTTON = new Button("Global", "bomb");
        public static List<Soldier> Soldiers;
        public static List<Ninja> Ninjas;
        public static Noble Target;

        #region Main Functions

        public static void Initialize()
        {
            //ManagedDestructibleObjects = new List<DestructibleObject>();
            //ManagedInteractObjects = new List<InteractObject>();
            //ManagedWorldObjects = new List<WorldObject>();
            //ManagedTraps = new List<Trap>();
            ManagedSprites = new List<Sprite>();
            //ManagedEnemies = new List<Enemy1>();
            Soldiers = new List<Soldier>();
            Ninjas = new List<Ninja>();
            //FAKE_BUTTON.Visible = false;
        }

        public static void Update()
        {
            InteractTarget = FindClickedObject();

            PlayerInstance.Activity();

            foreach (Soldier s in Soldiers)
            {
                s.Activity();
            }

            foreach (Ninja n in Ninjas)
            {
                n.Activity();
            }

            Target.Activity();
        }

        public static void Destroy()
        {
            InteractTarget = null;

            /*foreach (WorldObject w in ManagedWorldObjects)
            {
                w.Destroy();
            }
            ManagedWorldObjects.Clear();

            foreach (Trap t in ManagedTraps)
            {
                t.Destroy();
            }
            ManagedTraps.Clear();*/

            /*foreach (Enemy1 e in ManagedEnemies)
            {
                e.Destroy();
            }
            ManagedEnemies.Clear();*/

            foreach (Sprite s in ManagedSprites)
            {
                SpriteManager.RemoveSprite(s);
            }
            ManagedSprites.Clear();

            foreach (Soldier d in Soldiers)
            {
                d.Destroy();
            }
            Soldiers.Clear();

            foreach (Ninja n in Ninjas)
            {
                n.Destroy();
            }
            Ninjas.Clear();

            PlayerInstance.Destroy();
            Target.Destroy();
            //FAKE_BUTTON.Destroy();
        }

        #endregion

        #region Interface Functions

        //NONE RIGHT NOW

        #endregion

        #region Helper Functions

        // NEED TO CHANGE SO IT IS Z-ORIENTED
        // MEANING YOU CHOOSE THE CLOSEST Z
        private static PositionedObject FindClickedObject()
        {
            if (GestureManager.CurGesture.Equals(Gesture.Tap))
            {
                /*if (UIManager.CheckButtonPressed())
                {
                    return FAKE_BUTTON;
                }*/

                /*foreach (Enemy1 e in ManagedEnemies)
                {
                    if (!e.IsAlive && e.Collision.IsPointInside(GestureManager.EndTouchWorld.X, GestureManager.EndTouchWorld.Y))
                    {
                        return e;
                    }
                }*/

                if (Target.IsAlive && Target.Collision.IsPointInside(GestureManager.EndTouchWorld.X, GestureManager.EndTouchWorld.Y))
                {
                    return Target;
                }

                foreach (Soldier s in Soldiers)
                {
                    if (s.IsAlive && s.Collision.IsPointInside(GestureManager.EndTouchWorld.X, GestureManager.EndTouchWorld.Y))
                    {
                        return s;
                    }
                }

                foreach (Ninja n in Ninjas)
                {
                    if (n.IsAlive && !n.IsHidden && n.Collision.IsPointInside(GestureManager.EndTouchWorld.X, GestureManager.EndTouchWorld.Y))
                    {
                        return n;
                    }
                }

                /*foreach (Trap t in ManagedTraps)
                {
                    if (t.Collision.IsPointInside(GestureManager.EndTouchWorld.X, GestureManager.EndTouchWorld.Y))
                    {
                        return t;
                    }
                }*/

                /*foreach (InteractObject i in ManagedInteractObjects)
                {
                    if (i.Collision.IsPointInside(GestureManager.EndTouchWorld.X, GestureManager.EndTouchWorld.Y))
                    {
                        return i;
                    }
                }*/

                if (PlayerInstance.Collision.IsPointInside(GestureManager.EndTouchWorld.X, GestureManager.EndTouchWorld.Y))
                {
                    return PlayerInstance;
                }
            }
            else if (GestureManager.CurGesture.Equals(Gesture.Swipe) || 
                     GestureManager.CurGesture.Equals(Gesture.SwipeDown) || 
                     GestureManager.CurGesture.Equals(Gesture.SwipeUp) ||
                     GestureManager.CurGesture.Equals(Gesture.SwipeLeft) ||
                     GestureManager.CurGesture.Equals(Gesture.SwipeRight))
            {

                if (PlayerInstance.Collision.CollideAgainst(GestureManager.DragLine))
                {
                    return PlayerInstance;
                }

                /*foreach (Enemy1 e in ManagedEnemies)
                {
                    if (e.Collision.CollideAgainst(GestureManager.DragLine))
                    {
                        return e;
                    }
                }*/

                foreach (Soldier s in Soldiers)
                {
                    if (s.IsAlive && s.Collision.CollideAgainst(GestureManager.DragLine))
                    {
                        return s;
                    }
                }

                /*foreach (DestructibleObject d in ManagedDestructibleObjects)
                {
                    if (d.Collision.CollideAgainst(GestureManager.DragLine))
                    {
                        return d;
                    }
                }*/
            }

            return null;
        }

        #endregion
    }
}
