using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Shroud.Utilities
{
    public static class NodeManager
    {
        public static List<Node> Nodes;

        private static List<Node> Q;

        public static void Initialize()
        {
            Nodes = new List<Node>();
            Q = new List<Node>();
        }

        public static void AddNode(Node n)
        {
            Nodes.Add(n);
        }

        public static void RemoveNode(Node n)
        {
            if (Nodes.Contains(n))
            {
                Nodes.Remove(n);
            }
        }

        public static void AddEdge(Node n1, Node n2)
        {
            if (!n1.Neighbors.Contains(n2))
            {
                n1.Neighbors.Add(n2);
                n1.Costs.Add((n1.Position - n2.Position).Length());
            }

            if (!n2.Neighbors.Contains(n1))
            {
                n2.Neighbors.Add(n1);
                n2.Costs.Add((n2.Position - n1.Position).Length());
            }
        }

        // Finds path from s to e and puts that in path
        public static void FindPathTo(Node s, Node e, ref List<Node> path)
        {
            // The two nodes s is between
            Node s1 = null;
            Node s2 = null;

            FindClosestLinePoints(s, Nodes, ref s1, ref s2);

            // The two nodes e is between
            Node e1 = null;
            Node e2 = null;
            Node beforeEnd = null;

            FindClosestLinePoints(e, Nodes, ref e1, ref e2);

            // Calculate and Enqueue real end
            Vector3 e1e2 = e2.Position - e1.Position;
            Vector3 e1e = e.Position - e1.Position;
            float scalar = (Vector3.Dot(e1e, e1e2) / Vector3.Dot(e1e2, e1e2));

            if (scalar < 0.0f)
                scalar = 0.0f;
            else if (scalar > 1.0f)
                scalar = 1.0f;

            Vector3 projectedPosition = e1.Position + scalar * e1e2;

            // e is now the real end point
            e.SetPosition(projectedPosition.X,
                          projectedPosition.Y,
                          projectedPosition.Z);

            // Special Case Start and End between same two nodes
            if ((e1.Equals(s1) && e2.Equals(s2)) || (e1.Equals(s2) && e2.Equals(s1)))
            {
                path.Add(e);
                return;
            }

            // Prep Q for new nodes
            Q.Clear();
            
            // Add all nodes and reset values
            foreach (Node n in Nodes)
            {
                n.Initialize();
                Q.Add(n);
            }

            // Arbitrarily set starting node
            float distS1 = Vector3.Distance(s1.Position, s.Position);
            float distS2 = Vector3.Distance(s2.Position, s.Position);

            s1.Distance = distS1;
            s2.Distance = distS2;

            Node nextNode;

            // Find best path
            while (Q.Count > 0)
            {
                // THIS IS VERY INEFFICIENT!!!!! O(# of Nodes)
                nextNode = FindNext(Q);

                // SOMETHING BAD HAPPENED
                if (nextNode.Distance >= 10000.0f)
                {
                    System.Diagnostics.Debug.WriteLine("THERE WAS AN ERROR");
                    break;
                }

                Q.Remove(nextNode);

                // Propagate distance to goal
                foreach (Node m in nextNode.Neighbors)
                {
                    float altDist = nextNode.Distance + (nextNode.Position - m.Position).Length();

                    if (altDist < m.Distance)
                    {
                        m.Distance = altDist;
                        m.Previous = nextNode;
                    }
                }

                // Checks for end case
                if (nextNode.Equals(e1))
                {
                    beforeEnd = e1;
                    break;
                }
                else if (nextNode.Equals(e2))
                {
                    beforeEnd = e2;
                    break;
                }
            }

            Node recurse = beforeEnd;

            // Recursively adds all nodes
            while (recurse != null)
            {
                path.Add(recurse);
                recurse = recurse.Previous;
            }

            // Removes extra start node if needed
            if (path.Contains(s2))
            {
                //path.Remove(s1);
            }

            // Corrects ordering
            path.Reverse();

            // Adds ending node as long as its not the same as the last node
            if (!e.Position.Equals(path[path.Count - 1].Position))
                path.Add(e);
        }

        // TODO: MAKE THIS EFFICIENT BY IMPLEMENTING A PRIORITY QUEUE/BINARY HEAP
        private static Node FindNext(List<Node> nodes)
        {
            Node next = nodes[0];
            Node curNode;

            foreach (Node n in nodes)
            {
                curNode = n;

                if (curNode.Distance < next.Distance)
                    next = curNode;
            }

            return next;
        }

        private static void FindClosestLinePoints(Node n, List<Node> nodes, ref Node lp1, ref Node lp2)
        {
            float shortest = 10000.0f;
            Node one = nodes[0];
            Node two = nodes[0].Neighbors[0];

            float dist = 0.0f;

            foreach (Node m in nodes)
            {
                foreach (Node mn in m.Neighbors)
                {
                    dist = CalculateDistance(m, mn, n);

                    if (dist < shortest)
                    {
                        //System.Diagnostics.Debug.WriteLine("m = " + m.Position + ", mn = " + mn.Position + ", dist = " + dist);
                        one = m;
                        two = mn;
                        shortest = dist;
                    }
                }
            }

            lp1 = one;
            lp2 = two;
        }


        /* Algorithm adopted from http://stackoverflow.com/a/1501725
         * Calculates Distance between a line and point
         */
        private static float CalculateDistance(Node p1, Node p2, Node p)
        {
            float len_sq = Vector3.DistanceSquared(p1.Position, p2.Position);

            if (len_sq == 0.0f)
                return Vector3.Distance(p1.Position, p.Position);

            float t = Vector3.Dot(p.Position - p1.Position, p2.Position - p1.Position) / len_sq;

            //System.Diagnostics.Debug.WriteLine("t = " + t);

            if (t < 0.0f)
            {
                return Vector3.Distance(p.Position, p1.Position);
            }
            else if (t > 1.0f)
            {
                return Vector3.Distance(p.Position, p2.Position);
            }
            else
            {
                Vector3 proj = p1.Position + t * (p2.Position - p1.Position);
                return Vector3.Distance(p.Position, proj);
            }
        }
    }
}
