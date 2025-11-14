using Undermarch.Simulation.Core;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Simulation.Entities
{
    public class Chest : IInteractable, IEntity
    {
        public TilePos Position { get; set; }
        public string Name { get; private set; }
        public int GoldAmount { get; private set; }
        public bool IsActive { get; private set; }
        public bool Looted { get; private set; }

        public Chest(TilePos position, int goldAmount)
        {
            Position = position;
            GoldAmount = goldAmount;
            Name = "Treasure Chest";
            IsActive = true;
            Looted = false;
        }

        public void Interact(ICharacter character)
        {
            // ICharacter interface method - won't be used until Character implements ICharacter
        }

        public void Interact(Character character)
        {
            if (!Looted)
            {
                character.gold += GoldAmount;
                Looted = true;
                IsActive = false;
                SimulationLog.Log($"{character.Name} looted {Name} and found {GoldAmount} gold!");
            }
        }
    }
}
