using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlatRedBall.Math.Geometry;
using FlatRedBall;

using Shroud.Entities;
using OType = Shroud.Entities.Obstacle.OType;

namespace Shroud.Utilities
{
    public static class CollisionManager
    {
        public static List<Obstacle> ManagedObstacles;
        public static List<Projectile> ManagedProjectiles;
        private static List<Projectile> ProjectilesToRemove;

        public static Circle PlayerCollision;
        public static Enemy TargetRef;

        public static void Initialize()
        {
            ManagedProjectiles = new List<Projectile>();
            ProjectilesToRemove = new List<Projectile>();

            ManagedObstacles = new List<Obstacle>();

            Point[] pointarray = 
            {
                new Point(-2.0f, 0.0f),
                new Point(-2.0f, -16.0f),
                new Point(-10.0f, -24.0f),
                new Point(-10.0f, -32.0f),
                new Point(-2.0f, -40.0f),
                new Point(-2.0f, -56.0f),
                new Point(-20.0f, -56.0f),
                new Point(-20.0f, -0.0f),
                new Point(-2.0f, 0.0f)
            };

            Obstacle worldCollision = new Obstacle("Global", OType.Solid, pointarray);
            //worldCollision.Position.X = -2.0f;

            ManagedObstacles.Add(worldCollision);
        }

        public static void CheckProjectileCollisions(List<Enemy> enemies)
        {
            foreach (Projectile p in ManagedProjectiles)
            {
                foreach (Enemy e in enemies)
                {
                    if (e.Alive && p.Collision.CollideAgainst(e.Collision))
                    {
                        p.Deactivate();
                        p.AttachTo(e, true);
                        ProjectilesToRemove.Add(p);
                        e.Kill();
                        p.X -= 1.0f;
                        break;
                    }
                }

                if (TargetRef.Alive && p.Collision.CollideAgainst(TargetRef.Collision))
                {
                    p.Deactivate();
                    p.AttachTo(TargetRef, true);
                    ProjectilesToRemove.Add(p);
                    TargetRef.Kill();
                    p.X -= 1.0f;
                }

                if (!p.IsReady)
                {
                    foreach (Obstacle o in ManagedObstacles)
                    {
                        if (p.Collision.CollideAgainst(o.Collision) && o.myType.Equals(OType.Solid))
                        {
                            p.Deactivate();
                            ProjectilesToRemove.Add(p);
                            break;
                        }
                    }
                }
            }

            foreach (Projectile rp in ProjectilesToRemove)
            {
                ManagedProjectiles.Remove(rp);
            }

            ProjectilesToRemove.Clear();
        }

        public static void AnimateProjectiles()
        {
            foreach (Projectile p in ManagedProjectiles)
            {
                p.Activity();
            }
        }

    }
}
