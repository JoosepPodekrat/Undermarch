using Undermarch.Simulation.Entities;

namespace Undermarch.Simulation.Effects.Debuffs
{
    public class Debuff : ICharacterEffect
    {
        public string name { get; set; }
        public int duration { get; set; }
        public void Add(Character target)
        {
        }
        public void Remove(Character target) { }
        public void Apply(Character target)
        {
            target.debuffs.Add(this);
        }


    }
}