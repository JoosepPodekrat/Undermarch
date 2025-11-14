using System;

namespace Undermarch.Simulation.Interfaces
{
    public enum GamePhase
    {
        Placement,
        Combat,
        GameOver
    }

    /// <summary>
    /// Interface for game state management (resources, phase, wave number, etc.).
    /// </summary>
    public interface IGameState
    {
        GamePhase Phase { get; set; }
        int CurrentGold { get; }
        int Wave { get; }

        bool CanAfford(int cost);
        void SpendGold(int amount);
        void EarnGold(int amount);

        event Action OnResourcesChanged;
    }
}
