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
            if (startPos.Equals(endPos)) return new List<TilePos>();

            var startNode = new Node(startPos);
            
            // Use dictionary for O(1) lookup by position
            var openList = new Dictionary<TilePos, Node>();
            var closedList = new HashSet<TilePos>();

            openList[startPos] = startNode;

            while (openList.Count > 0)
            {
                // Find node with lowest FCost
                Node currentNode = null;
                foreach (var node in openList.Values)
                {
                    if (currentNode == null || node.FCost < currentNode.FCost || (node.FCost == currentNode.FCost && node.HCost < currentNode.HCost))
                    {
                        currentNode = node;
                    }
                }

                if (currentNode == null) break;

                openList.Remove(currentNode.Position);
                closedList.Add(currentNode.Position);

                if (currentNode.Position.Equals(endPos))
                {
                    return RetracePath(startNode, currentNode);
                }

                foreach (var neighbourPos in GetNeighbours(board, currentNode.Position))
                {
                    if (closedList.Contains(neighbourPos))
                    {
                        continue;
                    }
                    
                    // Don't allow pathing through other characters, unless it's the destination.
                    if (board.GetEntityAt(neighbourPos) != null && !neighbourPos.Equals(endPos))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode.Position, neighbourPos);
                    
                    if (!openList.TryGetValue(neighbourPos, out Node neighbourNode) || newMovementCostToNeighbour < neighbourNode.GCost)
                    {
                        if (neighbourNode == null)
                        {
                            neighbourNode = new Node(neighbourPos);
                            openList[neighbourPos] = neighbourNode;
                        }
                        
                        neighbourNode.GCost = newMovementCostToNeighbour;
                        neighbourNode.HCost = GetDistance(neighbourPos, endPos);
                        neighbourNode.Parent = currentNode;
                    }
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
            TilePos[] potentials = {
                new TilePos(pos.x + 1, pos.y),
                new TilePos(pos.x - 1, pos.y),
                new TilePos(pos.x, pos.y + 1),
                new TilePos(pos.x, pos.y - 1)
            };

            foreach (var neighbourPos in potentials)
            {
                if (board.InBounds(neighbourPos) && !board.HasWallAt(neighbourPos))
                {
                    yield return neighbourPos;
                }
            }
        }

        private static int GetDistance(TilePos posA, TilePos posB)
        {
            return System.Math.Abs(posA.x - posB.x) + System.Math.Abs(posA.y - posB.y);
        }
    }
}
