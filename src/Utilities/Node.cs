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
        private List<float> mCosts;
        private Vector3 mPosition;

        // Dijkstra helper variables
        private Node mPrevious;
        private float mDistance;

        public List<Node> Neighbors
        {
            get { return mNeighbors; }
        }

        public List<float> Costs
        {
            get { return mCosts; }
        }

        public Vector3 Position
        {
            get { return mPosition; }
        }

        public Node Previous
        {
            get { return mPrevious; }
            set { mPrevious = value; }
        }

        public float Distance
        {
            get { return mDistance; }
            set { mDistance = value; }
        }

        public Node()
        {
            mNeighbors = new List<Node>();
            mCosts = new List<float>();

            mPosition = new Vector3();

            Initialize();
        }

        public Node(float x, float y, float z)
        {
            mNeighbors = new List<Node>();
            mCosts = new List<float>();

            mPosition.X = x;
            mPosition.Y = y;
            mPosition.Z = z;

            Initialize();
        }

        public void Initialize()
        {
            mDistance = 10000.0f;
            mPrevious = null;
        }

        public void SetPosition(float x, float y, float z)
        {
            mPosition.X = x;
            mPosition.Y = y;
            mPosition.Z = z;
        }
    }
}
