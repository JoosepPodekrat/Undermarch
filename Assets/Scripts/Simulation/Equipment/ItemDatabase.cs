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
            name = "Crude Axe",
            description = "The rough Edge causes bleeding",
            damage = 8,
            damageType = DamageType.Bleed,
            strength = 3,
            intelligence = -2
        };
        public static readonly Weapon priestessStaff = new Weapon
        {
            name = "Priestess staff",
            description = "A blessed staff, wielded by priestesses",
            damage = 5,
            damageType = DamageType.Light,
        };
        public static readonly Weapon warlockStaff = new Weapon
        {
            name = "Priestess staff",
            description = "A cursed staff, wielded by sorcerers",
            damage = 20,
            damageType = DamageType.Dark,
        };
        public static readonly Weapon shortbow = new Weapon
        {
            name = "Shortbow",
            description = "It's a short bow buddy",
            damage = 5,
            damageType = DamageType.Physical,
        };
        public static readonly Weapon steelSword = new Weapon
        {
            name = "Steel Sword",
            description = "A well-balanced steel blade favored by trained soldiers.",
            damage = 13,
            damageType = DamageType.Physical,
            strength = 3,
            stamina = 2
        };

        public static readonly Weapon adeptStaff = new Weapon
        {
            name = "Adept Staff",
            description = "A refined staff that channels arcane power efficiently.",
            damage = 12,
            damageType = DamageType.Arcane,
            intelligence = 4,
            stamina = 0
        };

        public static readonly Weapon temperedDagger = new Weapon
        {
            name = "Tempered Dagger",
            description = "A finely crafted dagger with a razor-sharp edge.",
            damage = 10,
            damageType = DamageType.Physical,
            agility = 3
        };

        public static readonly Weapon longbow = new Weapon
        {
            name = "Longbow",
            description = "A powerful bow capable of piercing armor at range.",
            damage = 9,
            damageType = DamageType.Physical,
            agility = 2,
            stamina = 1
        };
        public static readonly Weapon heroSword = new Weapon
        {
            name = "Holy Sword",
            description = "A sword blessed by the saintess",
            damage = 25,
            damageType = DamageType.Light,
            strength = 10,
            stamina = 3,
            intelligence = 3,
            agility = 3,
            spirit = 3,
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
        public static readonly Armor priestessRobe = new Armor
        {
            name = "Apprentice Robe",
            description = "A simple robe worn by aspiring mages.",
            intelligence = 5,
            spirit = 4,
            stamina = 0,
            agility = 0,
            strength = -5
        };
        public static readonly Armor reinforcedLeatherArmor = new Armor
        {
            name = "Reinforced Leather Armor",
            description = "Leather armor strengthened with metal studs.",
            stamina = 2,
            agility = 2,
            strength = 1,
            intelligence = 0,
            spirit = 0
        };

        public static readonly Armor steelBreastplate = new Armor
        {
            name = "Steel Breastplate",
            description = "Heavy steel armor offering excellent protection.",
            stamina = 3,
            strength = 2,
            agility = -1,
            intelligence = 0,
            spirit = 1
        };

        public static readonly Armor mageRobe = new Armor
        {
            name = "Mage Robe",
            description = "A robe woven with protective enchantments.",
            intelligence = 4,
            spirit = 2,
            stamina = 0,
            agility = 0,
            strength = -2
        };

        public static readonly Armor heroArmor = new Armor
        {
            name = "Hero Armor",
            description = "Armor fitting of a true hero.",
            intelligence = 3,
            spirit = 3,
            stamina = 10,
            agility = 3,
            strength = 3
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
        public static readonly Helmet knightHelmet = new Helmet
        {
            name = "Knight Helmet",
            description = "A knight's steel helmet",
            stamina = 5,
            strength = 3,
            agility = -1,
            spirit = -1,
            intelligence = 0
        };
        public static readonly Helmet barbarianHelmet= new Helmet
        {
            name = "Barbarian's horned helmet",
            description = "A Helmet with horns, worn by berserkers",
            stamina = 3,
            strength = 2,
            agility = 1,
            spirit = 3,
            intelligence = -5
        };
        public static readonly Helmet steelHelmet = new Helmet
        {
            name = "Steel Helmet",
            description = "A durable helmet offering solid battlefield protection.",
            stamina = 3,
            strength = 2,
            agility = -1,
            intelligence = 0,
            spirit = 0
        };
        public static readonly Helmet heroHelmet = new Helmet
        {
            name = "Heroic Helmet",
            description = "A helmet fit for a hero.",
            stamina = 3,
            strength = 10,
            agility = 3,
            intelligence = 3,
            spirit = 3
        };

        public static readonly Helmet enchantedHood = new Helmet
        {
            name = "Enchanted Hood",
            description = "A hood enchanted to bolster magical focus.",
            intelligence = 3,
            spirit = 2,
            stamina = 0,
            agility = 0,
            strength = -1
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
        public static readonly Accessory silverRing = new Accessory
        {
            name = "Silver Ring",
            description = "A polished silver ring that enhances physical prowess.",
            strength = 2,
            stamina = 1,
            agility = 1,
            intelligence = 0,
            spirit = 0
        };

        public static readonly Accessory amuletOfClarity = new Accessory
        {
            name = "Amulet of Clarity",
            description = "An amulet that calms the mind and sharpens thought.",
            intelligence = 3,
            spirit = 2,
            stamina = 0,
            agility = 0,
            strength = -1
        };
        public static readonly Accessory heroTalisman = new Accessory
        {
            name = "Heroic Talisman",
            description = "A talisman fit for a hero",
            intelligence = 10,
            spirit = 10,
            stamina = 3,
            agility = 3,
            strength = 3
        };
    }
}
