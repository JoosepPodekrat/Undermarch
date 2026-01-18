using Undermarch.Simulation.Combat;

namespace Undermarch.Simulation.Entities.Traps
{
    public class MetalSpikeTrap : Trap
    {
        public MetalSpikeTrap() : base("Metal Spike Trap", 100, CreateDamagePacket())
        {
        }

        private static DamagePacket CreateDamagePacket()
        {
            var packet = new DamagePacket();
            packet.Add(DamageType.Physical, 30); 
            return packet;
        }
    }
}
