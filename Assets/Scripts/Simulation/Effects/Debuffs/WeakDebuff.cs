using Undermarch.Simulation.Entities;

namespace Undermarch.Simulation.Effects.Debuffs
{
    public class WeakDebuff : Debuff
    {
        public WeakDebuff(int strengthPenalty, int duration)
        {
            name = "Weakness";
            this.duration = duration;
            statModifiers[StatType.Strength] = -strengthPenalty;
        }
    }
}
