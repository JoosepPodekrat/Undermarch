namespace Undermarch
{
    public interface IArmor
    {
        string name { get; }
        string description { get; }
        public int agility { get; } // Influences speed, increases armor, increases crit 
        public int intelligence { get; } // Increases maximum mana, casting modifiers
        public int stamina { get; } // hp and so on
        public int strength { get; } // melee damage
        public int spirit { get; } // regeneration 
        void Equip(Character target)
        {
            target.charArmor = this;
        }
        void Unequip(Character target)
        {
            target.charArmor = null;
        }

        void Apply(Character target);
    }
}