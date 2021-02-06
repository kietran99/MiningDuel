using System;
using System.Collections.Generic;
using UnityEngine;
namespace PathFinding
{
    public class Node
    {
        public Vector2Int index;
        public int hCost,gCost,fCost;
        public Node preNode = null;

        public Node(Vector2Int index)
        {
            this.index = index;
        }
        public Node (int x, int y)
        {
            index.x = x;
            index.y = y;
        }

        public void CalculateFCost() => fCost= hCost+gCost;
    }

    public class Grid
    {
        Node[,] grid;
        int height,width;

        private Func<Vector2Int,Vector2Int, bool> IsWalkable = null;

        private MapManager mapManager;

        public Grid(int height, int width, Func<Vector2Int,Vector2Int, bool> IsWalkable)
        {
            this.height = height;
            this.width = width;
            this.IsWalkable = IsWalkable;
            grid = new Node[width,height];
            for (int y= 0; y<height; ++y)
            {
                for (int x=0; x<width; ++x)
                {
                    Node node = new Node(x,y);
                    grid[x,y] = node; 
                }
            }
        }
        public int GetWidth() => width;
        public int GetHeight() => height;

        public void ResetGrid()
        {
            for (int y= 0; y<height; ++y)
            {
                for (int x=0; x<width; ++x)
                {
                    Node node = grid[x,y];
                    node.gCost = int.MaxValue;
                    node.CalculateFCost();
                    node.preNode = null;
                }
            }
        }

        public List<Node> GetWalkableNeighbourNodes(Vector2Int nodeIndex)
        {
            List<Node> result = new List<Node>();
            for (int x = nodeIndex.x -1; x<= nodeIndex.x + 1; ++x)
            {
                if (x < 0 || x>= width) continue;
                for (int y = nodeIndex.y -1;y<= nodeIndex.y +1; ++y)
                {
                    if (y <0 || y>= height) continue;
                    if (x == nodeIndex.x && y== nodeIndex.y) continue;

                    if (CanWalk(nodeIndex.x,nodeIndex.y,x,y)) result.Add(grid[x,y]);
                }
            }
            // if (CanWalk(nodeIndex.x,nodeIndex.y,nodeIndex.x,nodeIndex.y+1)) result.Add(grid[nodeIndex.x,nodeIndex.y+1]);
            // if (CanWalk(nodeIndex.x,nodeIndex.y,nodeIndex.x+1,nodeIndex.y)) result.Add(grid[nodeIndex.x +1,nodeIndex.y]);
            // if (CanWalk(nodeIndex.x,nodeIndex.y,nodeIndex.x,nodeIndex.y-1)) result.Add(grid[nodeIndex.x,nodeIndex.y-1]);
            // if (CanWalk(nodeIndex.x,nodeIndex.y,nodeIndex.x-1,nodeIndex.y)) result.Add(grid[nodeIndex.x -1,nodeIndex.y]);
            return result;
        }


        public Node GetNode(Vector2Int nodeIndex)
        {
            return grid[nodeIndex.x,nodeIndex.y];
        }
        public Node GetNode(int x, int y)
        {
            return grid[x,y];
        }

        public bool CanWalk(int fromX, int fromY, int toX, int toY)
        {
            if (IsWalkable != null)
            {
                return IsWalkable(new Vector2Int(fromX,fromY), new Vector2Int(toX,toY));
            }
            return true;
        }

        public bool IsIndexValid(Vector2Int index)
        {
            if (index.x >= 0 && index.x < grid.GetLength(0) && index.y >=0 && index.y < grid.GetLength(1)) return true;
            return false;
        }

    }

    public class AStar
    {
        private const int GO_STRAIGHT_COST = 10;
        private const int GO_DIAGONAL_COST = 14;
        private List<Node> openList;
        private List<Node> closedList;
        private Grid grid;

        public AStar(int height,int width, Func<Vector2Int,Vector2Int, bool> IsWalkable)
        {
            grid = new Grid(height, width, IsWalkable);
        }

        public List<Node> FindPath(Vector2Int start, Vector2Int end)
        {
            Debug.Log("finding path for index start" +start + " end" + end);
            grid.ResetGrid();
            if (!grid.IsIndexValid(start) || !grid.IsIndexValid(end)) return null;
            Node startNode = grid.GetNode(start);
            Node endNode = grid.GetNode(end);
            openList = new List<Node>{startNode};
            closedList = new List<Node>();

            startNode.gCost = 0;
            startNode.hCost = CalculateDistance(start, end);
            startNode.CalculateFCost();

            while (openList.Count > 0)
            {
                Node currentNode = GetLowestFCostNode(openList);
                if (currentNode == endNode)
                {
                    return CalculatePath(currentNode);
                }
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (Node neighbourNode in grid.GetWalkableNeighbourNodes(currentNode.index))
                {
                    if (closedList.Contains(neighbourNode)) continue;
                    int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode.index, neighbourNode.index);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.preNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistance(neighbourNode.index, endNode.index);
                        neighbourNode.CalculateFCost();

                        if (!openList.Contains(neighbourNode)) openList.Add(neighbourNode);
                    }
                }

            }

            //out of node 
            return null;
        }

        private List<Node> CalculatePath(Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;
            while (currentNode.preNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.preNode;
            }
            path.Add(currentNode);
            path.Reverse();
            return path;
        }

        private int CalculateDistance(Vector2Int start, Vector2Int goal)
        {
            int xDistance = Mathf.Abs(goal.x - start.x);
            int yDistance = Mathf.Abs(goal.y - start.y);
            int remaining = Mathf.Abs(xDistance - yDistance);
            return GO_DIAGONAL_COST*Mathf.Min(xDistance,yDistance) + GO_STRAIGHT_COST*remaining;
        }

        private Node GetLowestFCostNode(List<Node> nodesList)
        {
            Node minCostNode = nodesList[0];
            for (int i = 1; i< nodesList.Count ; ++i)
            {
                if (nodesList[i].fCost < minCostNode.fCost) minCostNode = nodesList[i];
            }
            return minCostNode;
        }   
 
    }
}

