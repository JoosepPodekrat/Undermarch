using Undermarch.Simulation.Combat;

namespace Undermarch
{
    public static class ItemDatabase
    {
        public static readonly Weapon IronSword = new Weapon
        {
            name = "Iron Sword",
            description = "A sturdy and reliable blade.",
            damage = 10,
            damageType = DamageType.Physical,
            strength = 2,
            agility = 1
        };

        public static readonly Weapon ApprenticeStaff = new Weapon
        {
            name = "Apprentice Staff",
            description = "A beginner's staff for novice mages.",
            damage = 8,
            damageType = DamageType.Arcane,
            intelligence = 3
        };
    }

}