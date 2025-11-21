using System.Collections.Generic;
using System.Linq;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Simulation.Pathfinding
{
    public static class Pathfinder
    {
        public static List<TilePos> FindPath(IBoard board, TilePos startPos, TilePos endPos)
        {
            var startNode = new Node(startPos);
            var endNode = new Node(endPos);

            var openList = new List<Node> { startNode };
            var closedList = new HashSet<Node>();

            while (openList.Count > 0)
            {
                var currentNode = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].FCost < currentNode.FCost || openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost)
                    {
                        currentNode = openList[i];
                    }
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode.Position.Equals(endNode.Position))
                {
                    return RetracePath(startNode, currentNode);
                }

                foreach (var neighbourPos in GetNeighbours(board, currentNode.Position))
                {
                    if (closedList.Any(n => n.Position.Equals(neighbourPos)))
                    {
                        continue;
                    }
                    
                    // Don't allow pathing through other characters, unless it's the destination.
                    if (board.GetEntityAt(neighbourPos) != null && !neighbourPos.Equals(endPos))
                    {
                        continue;
                    }

                    var neighbourNode = new Node(neighbourPos)
                    {
                        GCost = currentNode.GCost + GetDistance(currentNode, new Node(neighbourPos)),
                        HCost = GetDistance(new Node(neighbourPos), endNode),
                        Parent = currentNode
                    };

                    if (openList.Any(n => n.Position.Equals(neighbourPos) && n.FCost < neighbourNode.FCost))
                    {
                        continue;
                    }

                    openList.Add(neighbourNode);
                }
            }

            return null; // No path found
        }

        private static List<TilePos> RetracePath(Node startNode, Node endNode)
        {
            var path = new List<TilePos>();
            var currentNode = endNode;

            while (currentNode != null && !currentNode.Position.Equals(startNode.Position))
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }

        private static IEnumerable<TilePos> GetNeighbours(IBoard board, TilePos pos)
        {
            var neighbours = new List<TilePos>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    
                    // No diagonal movement for now to keep it simple
                    if (System.Math.Abs(x) == System.Math.Abs(y)) continue;

                    var neighbourPos = new TilePos(pos.x + x, pos.y + y);
                    if (board.InBounds(neighbourPos) && !board.HasWallAt(neighbourPos))
                    {
                        neighbours.Add(neighbourPos);
                    }
                }
            }
            return neighbours;
        }

        private static int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = System.Math.Abs(nodeA.Position.x - nodeB.Position.x);
            int dstY = System.Math.Abs(nodeA.Position.y - nodeB.Position.y);
            return dstX + dstY; // Manhattan distance
        }
    }
}
