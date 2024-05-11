using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Harris.AI
{
    public class PathNode
    {
        private int x;

        public int X => x;
        private int y;
        public int Y => y;

        public int gCost;
        public int hCost;
        public int fCost;

        public PathNode cameFromNode;
        public PathNode(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void calculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}