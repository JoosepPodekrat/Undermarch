namespace Undermarch
{
    public interface IEquipment
    {
        public int agility { get; } // Influences speed, increases armor, increases crit 
        public int intelligence { get; } // Increases maximum mana, casting modifiers
        public int stamina { get; } // hp and so on
        public int strength { get; } // melee damage
        public int spirit { get; } // regeneration 

    }
}