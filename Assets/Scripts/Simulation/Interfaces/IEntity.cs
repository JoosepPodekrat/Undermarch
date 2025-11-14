using Undermarch.Simulation.Grid;

namespace Undermarch.Simulation.Interfaces
{
    /// <summary>
    /// Base interface for all entities that can exist on the board.
    /// </summary>
    public interface IEntity
    {
        TilePos Position { get; set; }
        string Name { get; }
    }
}
