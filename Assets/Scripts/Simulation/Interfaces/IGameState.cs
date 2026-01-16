using System;
using System.Collections.Generic;

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
        Corpse
    }

    /// <summary>
    /// Interface for game state management (resources, phase, etc.).
    /// </summary>
    public interface IGameState
    {
        GamePhase Phase { get; set; }

        // ----------------------
        // Resource access
        // ----------------------

        int GetResource(ResourceType type);

        bool AddResource(ResourceType type, int amount);
        bool SpendResource(ResourceType type, int amount);

        // ----------------------
        // Multi-resource costs
        // ----------------------

        bool CanAfford(Dictionary<ResourceType, int> cost);
        bool SpendResources(Dictionary<ResourceType, int> cost);

        event Action OnResourcesChanged;
    }
}
