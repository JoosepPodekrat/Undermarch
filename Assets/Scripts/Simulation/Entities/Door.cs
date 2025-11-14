using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Simulation.Entities
{
    /// <summary>
    /// Door that transports characters between rooms or serves as dungeon entrance.
    /// </summary>
    public class Door : IInteractable, IEntity
    {
        public TilePos Position { get; set; }
        public string Name { get; private set; }
        public bool IsActive { get; private set; }

        public int TargetRoomId { get; private set; }
        public TilePos TargetPosition { get; private set; }
        public bool IsEntrance { get; private set; } // Heroes spawn here
        public bool IsExit { get; private set; } // Heroes escape here

        public Door(TilePos position, int targetRoomId, TilePos targetPosition, bool isEntrance = false, bool isExit = false)
        {
            Position = position;
            TargetRoomId = targetRoomId;
            TargetPosition = targetPosition;
            IsEntrance = isEntrance;
            IsExit = isExit;
            IsActive = true;
            Name = isEntrance ? "Entrance" : (isExit ? "Exit" : "Door");
        }

        public void Interact(ICharacter character)
        {
            // Will be called when character steps on door
        }

        public void Interact(Character character)
        {
            if (IsExit && character.faction == Combat.Faction.Hero)
            {
                // Hero escapes - handled by game logic
            }
            // Room transitions handled by dungeon manager
        }
    }
}
