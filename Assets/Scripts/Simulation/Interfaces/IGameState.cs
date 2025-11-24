using System;

namespace Undermarch.Simulation.Interfaces
{
    public enum GamePhase
    {
        Placement,
        Combat,
        BuildingPhase2,
        GameOver
    }
    public enum ResourceType
    {
        Gold,
        Wood,
        Steel,
        Food,
        Mana,
    }

    /// <summary>
    /// Interface for game state management (resources, phase, wave number, etc.).
    /// </summary>
    public interface IGameState
    {
        GamePhase Phase { get; set; }
        int CurrentGold { get; }
        int CurrentWood { get; }
        int CurrentSteel { get; }
        int CurrentFood { get; }
        int CurrentMana { get; }
        int Wave { get; }


        bool CanAfford(int cost);
        void SpendGold(int amount);
        void EarnGold(int amount);

        int GetResource(ResourceType type);
        bool AddResource(ResourceType type, int amount);
        bool SpendResource(ResourceType type, int amount);


        event Action OnResourcesChanged;
    }
}
