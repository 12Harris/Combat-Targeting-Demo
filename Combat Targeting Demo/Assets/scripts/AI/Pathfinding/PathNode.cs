using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.AI.PathFinding
{
    public class PathNode
    {
        private float x;

        public float X => x;
        private float y;
        public float Y => y;

        public float gCost;
        public float hCost;
        public float fCost;

        public bool isWalkable;
        public PathNode cameFromNode;
        public PathNode(float x, float y)
        {
            this.x = x;
            this.y = y;
            isWalkable = true;
        }

        public void calculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}