using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Entities;

namespace Undermarch
{
    public class Accessory : IEquipment
    {
        // fields that can be assigned by subclasses
        public string name { get;  set; }
        public string description { get;  set; }

        public int agility { get;  set; }       // influences speed, increases armor, increases crit
        public int intelligence { get;  set; }  // increases maximum mana, casting modifiers
        public int stamina { get;  set; }       // hp and so on
        public int strength { get;  set; }      // melee damage
        public int spirit { get;  set; }        // regeneration

        public int damage { get;  set; }
        public DamageType damageType { get;  set; }

        public void equip(Character target)              // equips a weapon on the character
        {
            target.charAccessory = this;
        }

        public void unequip(Character target)            // unequips a weapon from the character
        {
            target.charAccessory = null;
        }

        public virtual void apply(Character target)      // optionally overridden for unique effects
        {
            // placeholder logic for special weapon effects
        }
    }
}
