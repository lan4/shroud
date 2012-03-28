using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using FlatRedBall;
using FlatRedBall.Math.Geometry;

namespace Shroud.Utilities
{
    public class Node
    {
        private List<Node> mNeighbors;
        private Vector3 mPosition;

        // Dijkstra helper variables
        private Node mPrevious;
        private float mDistance;

        #region Public Interface

        public Vector3 Position
        {
            get { return mPosition; }
            set
            {
                mPosition.X = value.X;
                mPosition.Y = value.Y;
                mPosition.Z = value.Z;
            }
        }

        public float X
        {
            get { return mPosition.X; }
            set { mPosition.X = value; }
        }

        public float Y
        {
            get { return mPosition.Y; }
            set { mPosition.Y = value; }
        }

        public float Z
        {
            get { return mPosition.Z; }
            set { mPosition.Z = value; }
        }

        public bool HasNeighbors
        {
            get { return mNeighbors.Count > 0; }
        }

        public bool IsLink
        {
            get
            {
                foreach (Node n in mNeighbors)
                {
                    if (Math.Abs(n.X - this.X) > 1.0f)
                        return true;
                }

                return false;
            }
        }

        #endregion

        #region Constructors

        private Node(bool addToGraph)
        {
            mNeighbors = new List<Node>();

            mPosition = new Vector3(0.0f, 0.0f, 0.0f);

            Initialize();

            if (addToGraph)
                Nodes.Add(this);
        }

        private Node(bool addToGraph, float x, float y, float z)
        {
            mNeighbors = new List<Node>();

            mPosition.X = x;
            mPosition.Y = y;
            mPosition.Z = z;

            Initialize();

            if (addToGraph)
                Nodes.Add(this);
        }

        #endregion

        #region Instance Methods

        private void Initialize()
        {
            mDistance = 10000.0f;
            mPrevious = null;
        }

        public void AddUndirectedEdge(Node n)
        {
            if (!this.mNeighbors.Contains(n))
            {
                this.mNeighbors.Add(n);
            }

            if (!n.mNeighbors.Contains(this))
            {
                n.mNeighbors.Add(this);
            }
        }

        public void AddDirectedEdge(Node n)
        {
            if (!this.mNeighbors.Contains(n))
            {
                this.mNeighbors.Add(n);
            }
        }

        public void RemoveUndirectedEdge(Node n)
        {
            if (this.mNeighbors.Contains(n))
            {
                this.mNeighbors.Remove(n);
            }

            if (n.mNeighbors.Contains(this))
            {
                n.mNeighbors.Remove(this);
            }
        }

        public void RemoveDirectedEdge(Node n)
        {
            if (this.mNeighbors.Contains(n))
            {
                this.mNeighbors.Remove(n);
            }
        }

        #endregion

        private static List<Node> Nodes = new List<Node>();
        private static List<Node> Q = new List<Node>();
        private static Node TempNode = new Node(false);

        public static Node CreateNode()
        {
            return new Node(false);
        }

        public static Node CreateNode(float x, float y, float z)
        {
            return new Node(false, x, y, z);
        }

        public static Node AddGraphNode(float x, float y, float z)
        {
            return new Node(true, x, y, z);
        }

        public static Node AddGraphNode(Node n)
        {
            if (!Nodes.Contains(n))
            {
                Nodes.Add(n);
                return n;
            }

            return null;
        }

        public static void DEBUG_VIEW()
        {
            foreach (Node n in Nodes)
            {
                Sprite s = SpriteManager.AddSprite("redball.png", "Global");
                s.Position = n.Position;
                s.ScaleX = 0.1f;
                s.ScaleY = 0.1f;
            }
        }

        public static void AddUndirectedEdge(Node n1, Node n2)
        {
            if (!n1.mNeighbors.Contains(n2))
            {
                n1.mNeighbors.Add(n2);
            }

            if (!n2.mNeighbors.Contains(n1))
            {
                n2.mNeighbors.Add(n1);
            }
        }

        public static void AddDirectedEdge(Node n1, Node n2)
        {
            if (!n1.mNeighbors.Contains(n2))
            {
                n1.mNeighbors.Add(n2);
            }
        }

        public static void RemoveUndirectedEdge(Node n1, Node n2)
        {
            if (n1.mNeighbors.Contains(n2))
            {
                n1.mNeighbors.Remove(n2);
            }

            if (n2.mNeighbors.Contains(n1))
            {
                n2.mNeighbors.Remove(n1);
            }
        }

        public static void RemoveDirectedEdge(Node n1, Node n2)
        {
            if (n1.mNeighbors.Contains(n2))
            {
                n1.mNeighbors.Remove(n2);
            }
        }

        // TODO: Optimize for queue
        public static void GetPathBetween(Node s, Node e, ref List<Node> path)
        {
            s.Initialize();
            e.Initialize();
            path.Clear();

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
            e.Position = projectedPosition;

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

            s1.mDistance = distS1;
            s2.mDistance = distS2;

            Node nextNode;

            // Find best path
            while (Q.Count > 0)
            {
                // THIS IS VERY INEFFICIENT!!!!! O(# of Nodes)
                nextNode = FindNext(Q);

                // SOMETHING BAD HAPPENED
                if (nextNode.mDistance >= 10000.0f)
                {
                    System.Diagnostics.Debug.WriteLine("THERE WAS AN ERROR");
                    break;
                }

                Q.Remove(nextNode);

                // Propagate distance to goal
                foreach (Node m in nextNode.mNeighbors)
                {
                    float altDist = nextNode.mDistance + (nextNode.Position - m.Position).Length();

                    if (altDist < m.mDistance)
                    {
                        m.mDistance = altDist;
                        m.mPrevious = nextNode;
                    }
                }
            }

            if (CalculateTotalLength(e1) > CalculateTotalLength(e2))
            {
                beforeEnd = e2;
            }
            else
            {
                beforeEnd = e1;
            }

            Node recurse = beforeEnd;

            // Recursively adds all nodes
            while (recurse != null)
            {
                path.Add(recurse);
                recurse = recurse.mPrevious;
            }

            // Corrects ordering
            path.Reverse();

            // Adds ending node as long as its not the same as the last node
            if ((e.Position - path[path.Count - 1].Position).Length() > 0.1f)
                path.Add(e);
        }

        // METHOD ONLY WORKS IF NEW END IS CLOSE ENOUGH (1 NODE AWAY) TO OLD END
        // RETURNS FALSE IF PATH UNCHANGED, RETURNS TRUE IF PATH CHANGED
        public static bool ChangePath(Vector3 pos, ref List<Node> path)
        {
            Node last = path[path.Count - 1];
            TempNode.X = pos.X;
            TempNode.Y = pos.Y;
            TempNode.Z = pos.Z;

            Node l1 = null, l2 = null;
            FindClosestLinePoints(last, Nodes, ref l1, ref l2);

            Node ne1 = null, ne2 = null;
            FindClosestLinePoints(TempNode, Nodes, ref ne1, ref ne2);

            if ((l1.Equals(ne1) || l1.Equals(ne2)) && (l2.Equals(ne1) || l2.Equals(ne2)))
            {
                last.Position = TempNode.Position;

                return true;
            }
            else if (l1.Equals(ne1) || l1.Equals(ne2))
            {
                if (path.Contains(l2))
                {
                    path.Remove(l1);
                    last.Position = TempNode.Position;
                }
                else
                {
                    path.Add(l1);
                }

                return true;
            }
            else if (l2.Equals(ne1) || l2.Equals(ne2))
            {
                if (path.Contains(l1))
                {
                    path.Remove(l2);
                    last.Position = TempNode.Position;
                }
                else
                {
                    path.Add(l2);
                }

                return true;
            }

            return false;
        }

        // TODO: MAKE THIS EFFICIENT BY IMPLEMENTING A PRIORITY QUEUE/BINARY HEAP
        private static Node FindNext(List<Node> nodes)
        {
            Node next = nodes[0];
            Node curNode;

            foreach (Node n in nodes)
            {
                curNode = n;

                if (curNode.mDistance < next.mDistance)
                    next = curNode;
            }

            return next;
        }

        public static void FindClosestLinePoints(Node n, List<Node> nodes, ref Node lp1, ref Node lp2)
        {
            float shortest = 10000.0f;
            Node one = nodes[0];
            Node two = nodes[0].mNeighbors[0];

            float dist = 0.0f;

            foreach (Node m in nodes)
            {
                foreach (Node mn in m.mNeighbors)
                {
                    dist = CalculateDistance(m, mn, n);

                    if (dist < shortest)
                    {
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

        private static float CalculateTotalLength(Node n)
        {
            float dist = 0.0f;

            while (n != null)
            {
                dist += n.mDistance;
                n = n.mPrevious;
            }

            return dist;
        }
    }
}
