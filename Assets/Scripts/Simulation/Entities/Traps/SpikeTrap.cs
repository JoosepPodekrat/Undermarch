using Undermarch.Simulation.Combat;

namespace Undermarch.Simulation.Entities.Traps
{
    public class SpikeTrap : Trap
    {
        public SpikeTrap() : base("Spike Trap", 1, CreateDamagePacket())
        {
        }

        private static DamagePacket CreateDamagePacket()
        {
            var packet = new DamagePacket();
            packet.Add(DamageType.Physical, 10); // Deals 10 physical damage
            return packet;
        }
    }
}
