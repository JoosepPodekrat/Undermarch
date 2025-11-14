using Undermarch.Simulation.Combat;

namespace Undermarch.Simulation.Interfaces
{
    /// <summary>
    /// Interface for characters that can move, fight, and act on the board.
    /// </summary>
    public interface ICharacter : IEntity
    {
        Faction Faction { get; }
        int CurrentHP { get; }
        int MaxHP { get; }
        bool IsAlive { get; }

        void Act(IBoard board, IGameState gameState);
        void TakeDamage(DamagePacket damage);
    }
}
