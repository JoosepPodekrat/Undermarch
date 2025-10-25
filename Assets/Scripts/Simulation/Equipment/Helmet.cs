namespace Undermarch
{
    public interface IHelmet
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
            target.charHelmet = this;
        }// equips a helmet on the character
        void Unequip(Character target)
        {
            target.charHelmet = null;
        }// unequips a helmet on the character

        void Apply(Character target);
    }
}