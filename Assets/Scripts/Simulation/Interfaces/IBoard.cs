using System.Collections.Generic;
using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Grid;

namespace Undermarch.Simulation.Interfaces
{
    /// <summary>
    /// Interface for board operations (grid, entities, walls, interactables).
    /// </summary>
    public interface IBoard
    {
        int Width { get; }
        int Height { get; }

        // Position queries
        bool InBounds(TilePos pos);
        int IndexOf(TilePos pos);

        // Wall layer
        bool HasWallAt(TilePos pos);
        void AddWall(TilePos pos);

        // Entity layer
        Character GetEntityAt(TilePos pos);
        void AddEntity(TilePos pos, Character character);
        void RemoveEntity(TilePos pos);
        void MoveEntity(TilePos from, TilePos to);
        IEnumerable<Character> GetAllCharacters();
        TilePos GetPositionOf(Character character);
        Character FindClosestTarget(Character self, Faction factionToTarget);

        // Interactable layer
        object GetInteractableAt(TilePos pos);
        void AddInteractable(TilePos pos, object interactable);
        void RemoveInteractable(TilePos pos);
    }
}
