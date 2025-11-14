using Undermarch.Simulation.Entities;

namespace Undermarch.Simulation.Effects.Buffs
{
    public class StrengthBuff : Buff
    {
        public StrengthBuff(int strengthBonus, int duration)
        {
            name = "Strength Boost";
            this.duration = duration;
            statModifiers[StatType.Strength] = strengthBonus;
        }
    }
}
