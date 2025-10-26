using Undermarch.Simulation.Grid;

namespace Undermarch.Simulation.Pathfinding
{
    public class Node
    {
        public TilePos Position { get; }
        public Node Parent { get; set; }
        public int GCost { get; set; } // Distance from starting node
        public int HCost { get; set; } // Heuristic distance to end node
        public int FCost => GCost + HCost; // Total cost

        public Node(TilePos position)
        {
            Position = position;
        }
    }
}
