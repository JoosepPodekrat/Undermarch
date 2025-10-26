
using Undermarch.Simulation.Combat;

namespace Undermarch
{
    public abstract class Trap
    {
        public string Name { get; protected set; }
        public int Durability { get; set; }
        public DamagePacket DamagePacket { get; protected set; }

        protected Trap(string name, int durability, DamagePacket damagePacket)
        {
            Name = name;
            Durability = durability;
            DamagePacket = damagePacket;
        }
    
    
    
    
    
    
    
    
    }


}