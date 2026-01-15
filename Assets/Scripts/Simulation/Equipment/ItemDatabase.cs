using Undermarch.Simulation.Combat;

namespace Undermarch
{
    public static class ItemDatabase
    {
        // =============================
        // Weapons
        // =============================

        public static readonly Weapon ironSword = new Weapon
        {
            name = "Iron Sword",
            description = "A sturdy and reliable blade.",
            damage = 10,
            damageType = DamageType.Physical,
            strength = 2,
            stamina = 1,
        };

        public static readonly Weapon apprenticeStaff = new Weapon
        {
            name = "Apprentice Staff",
            description = "A beginner's staff for novice mages.",
            damage = 8,
            damageType = DamageType.Arcane,
            intelligence = 3,
            stamina = -1
        };

        public static readonly Weapon ironDagger = new Weapon
        {
            name = "Iron Dagger",
            description = "A small iron dagger, quick but deadly.",
            damage = 7,
            damageType = DamageType.Physical,
            agility = 2
        };

        public static readonly Weapon swordAndShield = new Weapon
        {
            name = "Iron Shortsword and Shield",
            description = "A balanced combination of offense and defense.",
            damage = 5,
            damageType = DamageType.Physical,
            stamina = 2,
            agility = 1
        };
        public static readonly Weapon holySword = new Weapon
        {
            name = "Holy Sword",
            description = "A sword blessed by the saintess",
            damage = 15,
            damageType = DamageType.Light,
        };
        public static readonly Weapon warriorAxe = new Weapon
        {
            name = "Holy Sword",
            description = "A sword blessed by the saintess",
            damage = 8,
            damageType = DamageType.Bleed,
            strength = 3,
            intelligence = -2
        };

        // =============================
        // Armors
        // =============================

        public static readonly Armor leatherArmor = new Armor
        {
            name = "Leather Armor",
            description = "Light armor that offers basic protection.",
            stamina = 1,
            agility = 1,
            strength = 0,
            intelligence = 0,
            spirit = 0
        };

        public static readonly Armor chainmailArmor = new Armor
        {
            name = "Chainmail Armor",
            description = "A heavy armor made of interlocking metal rings.",
            stamina = 2,
            strength = 1,
            agility = -1,
            spirit = 0,
            intelligence = 0
        };

        public static readonly Armor apprenticeRobe = new Armor
        {
            name = "Apprentice Robe",
            description = "A simple robe worn by aspiring mages.",
            intelligence = 2,
            spirit = 1,
            stamina = -1,
            agility = 0,
            strength = -1
        };

        public static readonly Armor tatteredArmor = new Armor
        {
            name = "Tattered Armor",
            description = "Old and worn-out armor that barely offers protection.",
            stamina = -1,
            agility = -1,
            strength = -1,
            spirit = -1,
            intelligence = -1
        };

        // =============================
        // Helmets
        // =============================

        public static readonly Helmet ironHelmet = new Helmet
        {
            name = "Iron Helmet",
            description = "A solid helmet forged from iron.",
            stamina = 2,
            strength = 1,
            agility = -1,
            intelligence = 0,
            spirit = 0
        };

        public static readonly Helmet leatherCap = new Helmet
        {
            name = "Leather Cap",
            description = "A light cap offering minimal protection but comfort.",
            agility = 1,
            stamina = 0,
            strength = 0,
            intelligence = 0,
            spirit = 0
        };

        public static readonly Helmet mysticCowl = new Helmet
        {
            name = "Mystic Cowl",
            description = "A hood imbued with faint magical energy.",
            intelligence = 2,
            spirit = 1,
            stamina = -1,
            agility = 0,
            strength = -1
        };

        public static readonly Helmet crackedHelm = new Helmet
        {
            name = "Cracked Helm",
            description = "A damaged helmet with poor protection.",
            stamina = -2,
            strength = -1,
            agility = 0,
            spirit = 0,
            intelligence = 0
        };

        // =============================
        // Accessories
        // =============================

        public static readonly Accessory ironRing = new Accessory
        {
            name = "Iron Ring",
            description = "A simple iron ring that inspires determination.",
            strength = 1,
            stamina = 1,
            spirit = 0,
            agility = 0,
            intelligence = 0
        };

        public static readonly Accessory charmOfFocus = new Accessory
        {
            name = "Charm of Focus",
            description = "A magical charm that sharpens the mind.",
            intelligence = 2,
            spirit = 1,
            stamina = -1,
            agility = 0,
            strength = -1
        };

        public static readonly Accessory luckyPendant = new Accessory
        {
            name = "Lucky Pendant",
            description = "An old pendant said to bring luck in battle.",
            agility = 2,
            spirit = 1,
            strength = 0,
            intelligence = 0,
            stamina = 0
        };

        public static readonly Accessory cursedTalisman = new Accessory
        {
            name = "Cursed Talisman",
            description = "It radiates dark energy that weakens its wearer, while whispering promises of wisdom",
            strength = -2,
            spirit = -1,
            stamina = 0,
            agility = 0,
            intelligence = 3
        };
    }
}
