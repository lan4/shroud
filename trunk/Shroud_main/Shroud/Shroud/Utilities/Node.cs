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

        public Node Link;

        #region Constructors

        private Node(bool addToGraph)
        {
            mNeighbors = new List<Node>();

            mPosition = new Vector3(0.0f, 0.0f, 0.0f);

            Initialize();

            Link = null;

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

            Link = null;

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

        public bool IsNeighbor(Node n)
        {
            foreach (Node m in mNeighbors)
            {
                if (m.Equals(n))
                    return true;
            }

            return false;
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
        public static List<Node> NodeListToUse = Nodes;

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

            FindClosestLinePoints(s, NodeListToUse, ref s1, ref s2);

            // The two nodes e is between
            Node e1 = null;
            Node e2 = null;
            Node beforeEnd = null;

            FindClosestLinePoints(e, NodeListToUse, ref e1, ref e2);

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
            foreach (Node n in NodeListToUse)
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

        private static List<Node> TestPath1 = new List<Node>();
        //private static List<Node> TestPath2 = new List<Node>();
        private static Dictionary<Node, float> NodeDict = new Dictionary<Node, float>();

        public static Node FindNextNodeToward(Node s, Node e)
        {
            NodeDict.Clear();

            Node s1 = null;
            Node s2 = null;

            FindClosestLinePoints(s, NodeListToUse, ref s1, ref s2);

            Node e1 = null;
            Node e2 = null;

            FindClosestLinePoints(e, NodeListToUse, ref e1, ref e2);

            if ((e1.Equals(s1) && e2.Equals(s2)) || (e1.Equals(s2) && e2.Equals(s1)))
            {
                return e;
            }

            if ((s.Position - s1.Position).Length() < 0.2f)
            {
                foreach (Node n in s1.mNeighbors)
                {
                    float d = (e.Position - n.Position).Length();
                    //TestPath1.Clear();
                    //GetPathBetween(n, e, ref TestPath1);

                    /*foreach (Node m in TestPath1)
                    {
                        d += n.mDistance;
                    }
                    
                    d += (s.Position - s1.Position).Length();*/

                    NodeDict.Add(n, d);
                }

                Node min = null;
                float minValue = 10000.0f;

                foreach (Node o in NodeDict.Keys)
                {
                    if (NodeDict[o] < minValue)
                    {
                        min = o;
                        minValue = NodeDict[o];
                    }
                }

                return min;
            }
            else if ((s.Position - s2.Position).Length() < 0.2f)
            {
                //System.Diagnostics.Debug.WriteLine("THERE");

                foreach (Node n in s2.mNeighbors)
                {
                    float d = (e.Position - n.Position).Length();
                    /*TestPath1.Clear();
                    GetPathBetween(n, e, ref TestPath1);

                    foreach (Node m in TestPath1)
                    {
                        d += n.mDistance;
                    }

                    d += (s.Position - s2.Position).Length();*/

                    NodeDict.Add(n, d);
                }

                Node min = null;
                float minValue = 10000.0f;

                foreach (Node o in NodeDict.Keys)
                {
                    //System.Diagnostics.Debug.WriteLine(NodeDict[o]);

                    if (NodeDict[o] < minValue)
                    {
                        min = o;
                        minValue = NodeDict[o];
                    }
                }

                //System.Diagnostics.Debug.WriteLine("END");

                return min;
            }
            else
            {
                //TestPath1.Clear();
                //GetPathBetween(s1, e, ref TestPath1);

                float dist1 = (e.Position - s1.Position).Length();

                /*foreach (Node n in TestPath1)
                {
                    dist1 += n.mDistance;
                }

                dist1 += (s.Position - s1.Position).Length();*/

                //TestPath1.Clear();
                //GetPathBetween(s2, e, ref TestPath1);

                float dist2 = (e.Position - s2.Position).Length();

                /*foreach (Node n in TestPath1)
                {
                    dist2 += n.mDistance;
                }

                dist2 += (s.Position - s2.Position).Length();*/

                if (dist1 < dist2)
                {
                    return s1;
                }
                else
                {
                    return s2;
                }
            }
        }

        public static Node FindNextNodeAway(Node s, Node e)
        {
            NodeDict.Clear();

            Node s1 = null;
            Node s2 = null;

            FindClosestLinePoints(s, NodeListToUse, ref s1, ref s2);

            /*Node e1 = null;
            Node e2 = null;

            FindClosestLinePoints(e, LevelManager.CurrentScene.Nodes, ref e1, ref e2);

            if ((e1.Equals(s1) && e2.Equals(s2)) || (e1.Equals(s2) && e2.Equals(s1)))
            {
                return e;
            }*/

            if ((s.Position - s1.Position).Length() < 0.5f)
            {
                foreach (Node n in s1.mNeighbors)
                {
                    float d = (e.Position - n.Position).Length();
                    //TestPath1.Clear();
                    //GetPathBetween(n, e, ref TestPath1);

                    /*foreach (Node m in TestPath1)
                    {
                        d += n.mDistance;
                    }
                    
                    d += (s.Position - s1.Position).Length();*/

                    NodeDict.Add(n, d);
                }

                Node max = null;
                float maxValue = -10000.0f;

                foreach (Node o in NodeDict.Keys)
                {
                    if (NodeDict[o] > maxValue)
                    {
                        max = o;
                        maxValue = NodeDict[o];
                    }
                }

                return max;
            }
            else if ((s.Position - s2.Position).Length() < 0.5f)
            {
                //System.Diagnostics.Debug.WriteLine("THERE");

                foreach (Node n in s2.mNeighbors)
                {
                    float d = (e.Position - n.Position).Length();
                    /*TestPath1.Clear();
                    GetPathBetween(n, e, ref TestPath1);

                    foreach (Node m in TestPath1)
                    {
                        d += n.mDistance;
                    }

                    d += (s.Position - s2.Position).Length();*/

                    NodeDict.Add(n, d);
                }

                Node max = null;
                float maxValue = -10000.0f;

                foreach (Node o in NodeDict.Keys)
                {
                    //System.Diagnostics.Debug.WriteLine(NodeDict[o]);

                    if (NodeDict[o] > maxValue)
                    {
                        max = o;
                        maxValue = NodeDict[o];
                    }
                }

                //System.Diagnostics.Debug.WriteLine("END");

                return max;
            }
            else
            {
                //TestPath1.Clear();
                //GetPathBetween(s1, e, ref TestPath1);

                float dist1 = (e.Position - s1.Position).Length();

                /*foreach (Node n in TestPath1)
                {
                    dist1 += n.mDistance;
                }

                dist1 += (s.Position - s1.Position).Length();*/

                //TestPath1.Clear();
                //GetPathBetween(s2, e, ref TestPath1);

                float dist2 = (e.Position - s2.Position).Length();

                /*foreach (Node n in TestPath1)
                {
                    dist2 += n.mDistance;
                }

                dist2 += (s.Position - s2.Position).Length();*/

                if (dist1 < dist2)
                {
                    return s2;
                }
                else
                {
                    return s1;
                }
            }
        }

        // METHOD ONLY WORKS IF NEW END IS CLOSE ENOUGH (1 NODE AWAY) TO OLD END
        // RETURNS FALSE IF PATH UNCHANGED, RETURNS TRUE IF PATH CHANGED
        // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!DEPRECATED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public static bool ChangePath(Vector3 pos, Node cur, Node end, ref List<Node> path)
        {
            Node last;

            if (path.Count > 0)
                last = path[path.Count - 1];
            else
                last = cur;

            TempNode.X = pos.X;
            TempNode.Y = pos.Y;
            TempNode.Z = pos.Z;

            Node l1 = null, l2 = null;
            FindClosestLinePoints(last, NodeListToUse, ref l1, ref l2);

            Node ne1 = null, ne2 = null;
            FindClosestLinePoints(TempNode, NodeListToUse, ref ne1, ref ne2);

            if ((l1.Equals(ne1) || l1.Equals(ne2)) && (l2.Equals(ne1) || l2.Equals(ne2)))
            {
                if (last.Equals(end))
                    last.Position = TempNode.Position;
                else
                {
                    last = end;
                    last.Position = TempNode.Position;
                }
                    
                
                return true;
            }
            else if (l1.Equals(ne1) || l1.Equals(ne2))
            {
                if (path.Contains(l2))
                {
                    path.Remove(last);

                    if (last.Equals(end))
                        last.Position = TempNode.Position;
                    else
                    {
                        last = end;
                        last.Position = TempNode.Position;
                    }

                    path.Add(l1);
                    path.Add(last);
                }
                else
                {
                    return false;
                }

                return true;
            }
            else if (l2.Equals(ne1) || l2.Equals(ne2))
            {
                if (path.Contains(l1))
                {
                    //System.Diagnostics.Debug.WriteLine("Case 2b");
                    path.Remove(last);

                    if (last.Equals(end))
                        last.Position = TempNode.Position;
                    else
                    {
                        last = end;
                        last.Position = TempNode.Position;
                    }

                    path.Add(l2);
                    path.Add(last);
                }
                else
                {
                    return false;
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

        public static Node FindClosestNode(Vector3 pos)
        {
            Node closest = NodeListToUse[0];
            float dist = Vector3.Distance(pos, closest.Position);
            float newdist;

            foreach (Node m in NodeListToUse)
            {
                newdist = Vector3.Distance(pos, m.Position);

                if (newdist < dist)
                {
                    closest = m;
                    dist = newdist;
                }
            }

            return closest;
        }

        public static Node FindFallNode(Node n)
        {
            Node l1 = null;
            Node l2 = null;

            FindClosestLinePoints(n, NodeListToUse, ref l1, ref l2);

            if (l1.X < l2.X)
                return l1;
            else
                return l2;
        }
    }
}
