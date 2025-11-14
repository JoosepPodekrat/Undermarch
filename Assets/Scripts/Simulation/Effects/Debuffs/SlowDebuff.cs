using Undermarch.Simulation.Entities;

namespace Undermarch.Simulation.Effects.Debuffs
{
    public class SlowDebuff : Debuff
    {
        public SlowDebuff(int duration)
        {
            name = "Slow";
            this.duration = duration;
            // Slow reduces agility - future speed system will use this
            statModifiers[StatType.Agility] = -3;
        }
    }
}
