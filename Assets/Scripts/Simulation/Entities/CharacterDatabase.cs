using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Entities.Characters.DungeonMaster;
using Undermarch.Simulation.Entities.Characters.Heroes;
using Undermarch.Simulation.Entities.Characters.Monsters;

namespace Undermarch.Simulation.Entities
{
    public static class CharacterDatabase
    {
        // ==========================
        // CharacterES
        // ==========================

        public static readonly Hero peasant = new Hero
        {
            Name = "Peasant",
            faction = Faction.Hero,
            strength = 5,
            stamina = 2,
            agility = 9,
            intelligence = 8,
            spirit = 10,
            charWeapon = ItemDatabase.ironSword,
            charArmor = ItemDatabase.chainmailArmor,
            charHelmet = ItemDatabase.ironHelmet,
            charAccessory = ItemDatabase.ironRing,
            spawnSound = "humanMaleGrunt",
            deathSound = "humanMaleHurt",
        };

        public static readonly Hero apprenticeMage = new Hero
        {
            faction = Faction.Hero,
            strength = 7,
            stamina = 9,
            agility = 9,
            intelligence = 13,
            spirit = 11,
            charWeapon = ItemDatabase.apprenticeStaff,
            charArmor = ItemDatabase.apprenticeRobe,
            charHelmet = ItemDatabase.mysticCowl,
            charAccessory = ItemDatabase.charmOfFocus
        };

        public static readonly Hero rogue = new Hero
        {
            faction = Faction.Hero,
            strength = 9,
            stamina = 10,
            agility = 13,
            intelligence = 8,
            spirit = 10,
            charWeapon = ItemDatabase.ironDagger,
            charArmor = ItemDatabase.leatherArmor,
            charHelmet = ItemDatabase.leatherCap,
            charAccessory = ItemDatabase.luckyPendant
        };

        // ==========================
        // CharacterS
        // ==========================

        public static readonly Monster slimeMonster = new Monster
        {
            Name = "Slime Monster",
            faction = Faction.Defender,
            strength = 9,
            stamina = 8,
            agility = 11,
            intelligence = 6,
            spirit = 7,
            charWeapon = ItemDatabase.ironDagger,
            charArmor = ItemDatabase.tatteredArmor,
            charHelmet = ItemDatabase.crackedHelm,
            charAccessory = ItemDatabase.cursedTalisman,
            spawnSound = "slimeonspawn",
            hurtSound = "slimeonspawn"
        };

        public static readonly Monster troll = new Monster
        {
            faction = Faction.Defender,
            strength = 14,
            stamina = 13,
            agility = 6,
            intelligence = 6,
            spirit = 8,
            charWeapon = ItemDatabase.swordAndShield,
            charArmor = ItemDatabase.chainmailArmor,
            charHelmet = ItemDatabase.ironHelmet,
            charAccessory = ItemDatabase.ironRing
        };

        public static readonly Monster skeletonMage = new Monster
        {
            faction = Faction.Defender,
            strength = 7,
            stamina = 8,
            agility = 8,
            intelligence = 12,
            spirit = 10,
            charWeapon = ItemDatabase.apprenticeStaff,
            charArmor = ItemDatabase.apprenticeRobe,
            charHelmet = ItemDatabase.mysticCowl,
            charAccessory = ItemDatabase.charmOfFocus
        };

        public static readonly ArcherMonster archerMonster = new ArcherMonster
        {
            Name = "Archer Monster",
            faction = Faction.Defender,
            strength = 7,
            stamina = 9,
            agility = 12,
            intelligence = 7,
            spirit = 8,
            charWeapon = ItemDatabase.ironDagger,
            charArmor = ItemDatabase.leatherArmor,
            charHelmet = ItemDatabase.leatherCap,
            charAccessory = ItemDatabase.ironRing,
            AttackRange = 5,
            ArrowSpeed = 3,
            ArrowRange = 10,
            CooldownTicks = 2
        };

        public static readonly DungeonMaster dungeonMaster = new DungeonMaster
        {
            faction = Faction.Defender,
            strength = 15,
            stamina = 20,
            agility = 10,
            intelligence = 23,
            spirit = 15,
            charWeapon = ItemDatabase.apprenticeStaff, // Placeholder
            charArmor = ItemDatabase.apprenticeRobe, // Placeholder
            charHelmet = ItemDatabase.mysticCowl, // Placeholder
            charAccessory = ItemDatabase.charmOfFocus // Placeholder
        };
    }
}
