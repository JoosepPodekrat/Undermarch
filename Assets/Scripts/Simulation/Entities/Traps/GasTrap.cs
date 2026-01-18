using Undermarch.Simulation.Combat;

namespace Undermarch.Simulation.Entities.Traps
{
    public class GasTrap : Trap
    {
        public GasTrap() : base("Gas Trap", 1, CreateDamagePacket())
        {
        }

        private static DamagePacket CreateDamagePacket()
        {
            var packet = new DamagePacket();
            packet.Add(DamageType.Poison, 1000); 
            return packet;
        }
    }
}
