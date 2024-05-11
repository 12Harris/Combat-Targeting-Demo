using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Harris.AI
{
    public class Pathfinding
    {

        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        private PathNode[,] nodeMap = new PathNode[10,10];
        public PathNode[,] NodeMap => nodeMap;

        private List<PathNode> openList;
        private List<PathNode> closedList;
        public Pathfinding()
        {
            CreateNodeMap();
        }

        public List<PathNode> FindPath(int startX, int startZ, int endX, int endZ)
        {
            PathNode startNode = nodeMap[startZ, startX];
            PathNode endNode = nodeMap[endZ, endX];
            
            //openList = new List<PathNode>(startNode);
            openList = new List<PathNode>();
            openList.Add(startNode);
            closedList = new List<PathNode>();


            //Initialize
            for(int y = 0; y < nodeMap.GetLength(0);y++)
            {
                for(int x = 0; x < nodeMap.GetLength(1);x++)
                {
                    PathNode pathNode = nodeMap[y,x];
                    pathNode.gCost = int.MaxValue;
                    pathNode.calculateFCost();
                    pathNode.cameFromNode = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            startNode.calculateFCost();

            while(openList.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(openList);
                if(currentNode == endNode)
                {
                    //Reached final node
                    return CalculatePath(endNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach(PathNode neighbourNode in GetNeighbourList(currentNode))
                {
                    if(closedList.Contains(neighbourNode)) continue;
                    if(!neighbourNode.isWalkable){
                        closedList.Add(neighbourNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if(tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.calculateFCost();

                        if(!openList.Contains(neighbourNode))
                        {
                            openList.Add(neighbourNode);
                        }
                    }
                }
            }


            //out of nodes on the open list(we could not find a path)
            Debug.Log("couldnt find a path!");
            return null;

        }

        private List<PathNode> GetNeighbourList(PathNode currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();

            if(currentNode.X-1 >= 0)
            {
                //left
                neighbourList.Add(GetNode(currentNode.X-1,currentNode.Y));
                //left down
                if(currentNode.Y-1 >= 0) neighbourList.Add(GetNode(currentNode.X-1,currentNode.Y-1));
                //left up
                if(currentNode.Y+1 < 10) neighbourList.Add(GetNode(currentNode.X-1,currentNode.Y+1));

            }


            if(currentNode.X+1 < 10)
            {
                //right
                neighbourList.Add(GetNode(currentNode.X+1,currentNode.Y));
                //right down
                if(currentNode.Y-1 >= 0) neighbourList.Add(GetNode(currentNode.X+1,currentNode.Y-1));
                //right up
                if(currentNode.Y+1 < 10) neighbourList.Add(GetNode(currentNode.X+1,currentNode.Y+1));

            }

            //down
            if(currentNode.Y - 1 >= 0) neighbourList.Add(GetNode(currentNode.X, currentNode.Y - 1));
            //up
            if(currentNode.Y + 1 < 10) neighbourList.Add(GetNode(currentNode.X, currentNode.Y + 1));

            return neighbourList;
        }

        private PathNode GetNode(int x, int y)
        {
            return nodeMap[y,x];
        }

        private List<PathNode> CalculatePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();

            path.Add(endNode);
            PathNode currentNode = endNode;
            while(currentNode.cameFromNode != null)
            {
                path.Add(currentNode.cameFromNode);
                currentNode = currentNode.cameFromNode;
            }

            path.Reverse();
            return path;
        }

        private int CalculateDistanceCost(PathNode a, PathNode b)
        {
            int xDistance = Mathf.Abs(a.X - b.X);
            int yDistance = Mathf.Abs(a.Y - b.Y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        
        }

        private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostNode = pathNodeList[0];
            for(int i = 1; i < pathNodeList.Count; i++)
            {
                if(pathNodeList[i].fCost < lowestFCostNode.fCost)
                {
                    lowestFCostNode = pathNodeList[i];
                }
            }
            return lowestFCostNode;
        }

        private void CreateNodeMap()
        {
            for(int y = 0; y < GameMap.Map.GetLength(0);y++)
            {
                for(int x = 0; x < GameMap.Map.GetLength(1);x++)
                {
                    nodeMap[y,x] = new PathNode(x,y);
      
                    if(GameMap.Map[y,x] == ' ')
                    {
                        nodeMap[y,x].isWalkable = false;
                    }
                }
            }
        }
    }
}