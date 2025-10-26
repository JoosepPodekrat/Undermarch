using Undermarch.Simulation.Combat;

namespace Undermarch
{
    public class Armor : IEquipment
    {
        // fields that can be assigned by subclasses
        public string name { get; protected set; }
        public string description { get; protected set; }

        public int agility { get; protected set; }       // influences speed, increases armor, increases crit
        public int intelligence { get; protected set; }  // increases maximum mana, casting modifiers
        public int stamina { get; protected set; }       // hp and so on
        public int strength { get; protected set; }      // melee damage
        public int spirit { get; protected set; }        // regeneration

        public int damage { get; protected set; }
        public DamageType damageType { get; protected set; }

        public void equip(Character target)              // equips a Armor on the character
        {
            target.charArmor = this;
        }

        public void unequip(Character target)            // unequips a Armor from the character
        {
            target.charArmor = null;
        }

        public virtual void apply(Character target)      // optionally overridden for unique effects
        {
            // placeholder logic for special Armor effects
        }
    }
}
