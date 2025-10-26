using Undermarch.Simulation.Entities;

namespace Undermarch.Simulation.Effects
{
    public interface ICharacterEffect
    {
        string name { get; set; } // name of the effect, for finding in dicts and the like.
        int duration { get; set; } // duration in ticks, how long the effect should last on character

        void Add(Character target)
        {
        } // adds this effect to a target
        void Apply(Character target); // applies the effect of this effect to the target
        void Remove(Character target); // removes this effect from a target
    }
}
