using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Grid;

namespace Undermarch.Simulation.Interfaces
{
    /// <summary>
    /// Interface for projectiles like arrows and spells.
    /// NOTE: Will be upgraded to sub-tile Bresenham movement in future.
    /// </summary>
    public interface IProjectile : IEntity
    {
        TilePos Direction { get; }
        int Speed { get; }
        Faction Faction { get; }
        DamagePacket Damage { get; }
        bool IsActive { get; }

        void Tick(IBoard board);
    }
}
