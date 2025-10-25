namespace Undermarch
{
    public interface IAccessory
    {
        string name { get; }
        string description { get; }
        public int agility { get; } // Influences speed, increases armor, increases crit 
        public int intelligence { get; } // Increases maximum mana, casting modifiers
        public int stamina { get; } // hp and so on
        public int strength { get; } // melee damage
        public int spirit { get; } // regeneration 
        void Equip(Character target) // equips accessory on target
        {
            target.charAccessory = this;
        }
        void Unequip(Character target) // unequips accessory on target
        {
            target.charAccessory = null;
        }

        void Apply(Character target);
    }
}