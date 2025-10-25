using Undermarch.Simulation.Combat;

namespace Undermarch
{
    public interface IWeapon
    {
        string name { get; }
        string description { get; }
        public int agility { get; } // Influences speed, increases armor, increases crit 
        public int intelligence { get; } // Increases maximum mana, casting modifiers
        public int stamina { get; } // hp and so on
        public int strength { get; } // melee damage
        public int spirit { get; } // regeneration 
        
        public int damage { get; }
        public DamageType damageType { get; }

        void Equip (Character target) // equips a weapon on the character
        {
            target.charWeapon = this;
        }
        void Unequip (Character target) // unequips a weapon on the character
        {
            target.charWeapon = null;
        }

        void Apply(Character target);
    }
}