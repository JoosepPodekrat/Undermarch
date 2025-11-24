using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Interfaces;
using Undermarch.Simulation.Grid;

namespace Undermarch.Simulation.Entities.Traps
{
    public class BearTrap : Trap, IInteractable
    {
        public TilePos Position { get; set; }
        public bool IsActive => Durability > 0;

        public BearTrap() : base("Bear Trap", 1, CreateDamagePacket())
        {
        }

        private static DamagePacket CreateDamagePacket()
        {
            var packet = new DamagePacket();
            packet.Add(DamageType.Physical, 100); 
            return packet;
        }

        public void Interact(ICharacter character)
        {
            if (!IsActive) return;

            character.TakeDamage(DamagePacket);
            Durability--;
        }
    }
}
