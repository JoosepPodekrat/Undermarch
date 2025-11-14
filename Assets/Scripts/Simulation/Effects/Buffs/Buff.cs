using System.Collections.Generic;
using Undermarch.Simulation.Entities;

namespace Undermarch.Simulation.Effects.Buffs
{
    public class Buff : ICharacterEffect
    {
        public string name { get; set; }
        public int duration { get; set; }
        public Dictionary<StatType, int> statModifiers = new();

        public void Add(Character target)
        {
            target.buffs.Add(this);
        }

        public void Remove(Character target)
        {
            target.buffs.Remove(this);
        }

        public virtual void Apply(Character target)
        {
            // Called each tick while buff is active - for damage-over-time or other per-tick effects
            // Stat modifiers are applied passively through Character.GetStatModifier()
        }

        public void Tick()
        {
            if (duration > 0)
            {
                duration--;
            }
        }

        public bool IsExpired()
        {
            return duration <= 0;
        }
    }
}