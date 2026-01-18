using System.Collections.Generic;
using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Entities.Characters.DungeonMaster;
using Undermarch.Simulation.Entities.Characters.Heroes;
using Undermarch.Simulation.Entities.Characters.Monsters;
using Undermarch.Simulation.Core;
using ResourceType = Undermarch.Simulation.Interfaces.ResourceType;


namespace Undermarch.Simulation.Entities
{

    // sound values are spawnSound, attackSound, hurtSound, deathSound
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
            spawnSound = "humanmalegrunt",
            deathSound = "humanmalehurt",
            ResourcesGiven = new Dictionary<ResourceType, int> { { ResourceType.Gold, 30 }, { ResourceType.Corpse, 1 } } 
        };

        public static readonly Hero apprenticeMage = new Hero
        {
            Name = "Apprentice Mage",
            faction = Faction.Hero,
            strength = 7,
            stamina = 9,
            agility = 9,
            intelligence = 13,
            spirit = 11,
            charWeapon = ItemDatabase.apprenticeStaff,
            charArmor = ItemDatabase.apprenticeRobe,
            charHelmet = ItemDatabase.mysticCowl,
            charAccessory = ItemDatabase.charmOfFocus,

            deathSound = "humanfemalehurt",
            ResourcesGiven = new Dictionary<ResourceType, int> { { ResourceType.Gold, 50 }, { ResourceType.Corpse, 1 } } 
        };
        public static readonly Hero Mage = new Hero
        {
            Name = "Mage",
            faction = Faction.Hero,
            strength = 7,
            stamina = 9,
            agility = 9,
            intelligence = 15,
            spirit = 16,
            charWeapon = ItemDatabase.adeptStaff,
            charArmor = ItemDatabase.mageRobe,
            charHelmet = ItemDatabase.mysticCowl,
            charAccessory = ItemDatabase.amuletOfClarity,

            deathSound = "humanfemalehurt",
            ResourcesGiven = new Dictionary<ResourceType, int> { { ResourceType.Gold, 75 }, { ResourceType.Corpse, 1 } } 
        };

        public static readonly Hero rogue = new Hero //female rogue (use deeper voicelines)
        {
            Name = "Rogue",
            faction = Faction.Hero,
            strength = 9,
            stamina = 10,
            agility = 13,
            intelligence = 8,
            spirit = 10,
            charWeapon = ItemDatabase.ironDagger,
            charArmor = ItemDatabase.leatherArmor,
            charHelmet = ItemDatabase.leatherCap,
            charAccessory = ItemDatabase.luckyPendant,
            deathSound = "humanfemalehurt",
            attackSound = "largehumanfemalegrunt",
            ResourcesGiven = new Dictionary<ResourceType, int> { { ResourceType.Gold, 120 }, { ResourceType.Corpse, 1 } }
        };
        public static readonly Hero knight = new Hero
        {
            Name = "Knight",
            faction = Faction.Hero,
            strength = 15,
            stamina = 25,
            agility = 2,
            intelligence = 5,
            spirit = 10,
            charWeapon = ItemDatabase.steelSword,
            charArmor = ItemDatabase.steelBreastplate,
            charHelmet = ItemDatabase.knightHelmet,
            charAccessory = ItemDatabase.silverRing,
            spawnSound = "humanmalegrunt",
            deathSound = "humanmalehurt",
            ResourcesGiven = new Dictionary<ResourceType, int> { { ResourceType.Gold, 200 }, { ResourceType.Corpse, 1 } }
        };
        public static readonly Hero warrior = new Hero
        {
            Name = "Warrior",
            faction = Faction.Hero,
            strength = 12,
            stamina = 20,
            agility = 10,
            intelligence = 5,
            spirit = 15,
            charWeapon = ItemDatabase.warriorAxe,
            charArmor = ItemDatabase.reinforcedLeatherArmor,
            charHelmet = ItemDatabase.barbarianHelmet,
            charAccessory = ItemDatabase.silverRing,
            spawnSound = "humanmalegrunt",
            deathSound = "humanmalehurt",
            ResourcesGiven = new Dictionary<ResourceType, int> { { ResourceType.Gold, 145 }, { ResourceType.Corpse, 1 } }
        };
        public static readonly Hero priestess = new Hero
        {
            Name = "Priestess",
            faction = Faction.Hero,
            isHealer = true,
            strength = 0,
            stamina = 4,
            agility = 5,
            intelligence = 13,
            spirit = 15,
            charWeapon = ItemDatabase.priestessStaff,
            charArmor = ItemDatabase.priestessRobe,
            charHelmet = ItemDatabase.enchantedHood,
            charAccessory = ItemDatabase.amuletOfClarity,
            spawnSound = "humanfemalegrunt",
            deathSound = "humanfemalehurt",
            ResourcesGiven = new Dictionary<ResourceType, int> { { ResourceType.Gold, 145 }, { ResourceType.Corpse, 1 } }
        };
        public static readonly Hero highPriestess = new Hero
        {
            Name = "High Priestess",
            faction = Faction.Hero,
            isHealer = true,
            strength = 3,
            stamina = 10,
            agility = 8,
            intelligence = 15,
            spirit = 20,
            charWeapon = ItemDatabase.warriorAxe,
            charArmor = ItemDatabase.reinforcedLeatherArmor,
            charHelmet = ItemDatabase.barbarianHelmet,
            charAccessory = ItemDatabase.silverRing,
            spawnSound = "humanfemalegrunt",
            deathSound = "humanfemalehurt",
            ResourcesGiven = new Dictionary<ResourceType, int> { { ResourceType.Gold, 145 },{ResourceType.Corpse, 1 } }
        };
        public static readonly Hero HolyPriestess = new Hero
        {
            Name = "Holy Priestess",
            faction = Faction.Hero,
            isHealer= true,
            strength = 5,
            stamina = 15,
            agility = 10,
            intelligence = 25,
            spirit = 25,
            charWeapon = ItemDatabase.warriorAxe,
            charArmor = ItemDatabase.reinforcedLeatherArmor,
            charHelmet = ItemDatabase.barbarianHelmet,
            charAccessory = ItemDatabase.silverRing,
            spawnSound = "humanfemalegrunt",
            deathSound = "humanfemalehurt",
            ResourcesGiven = new Dictionary<ResourceType, int> { { ResourceType.Gold, 145 }, { ResourceType.Corpse, 1 } }
        };
        public static readonly Hero isekai = new Hero
        {
            Name = "Legendary Hero",
            faction = Faction.Hero,
            strength = 20,
            stamina = 10,
            agility = 20,
            intelligence = 15,
            spirit = 20,
            charWeapon = ItemDatabase.heroSword,
            charArmor = ItemDatabase.heroArmor,
            charHelmet = ItemDatabase.heroHelmet,
            charAccessory = ItemDatabase.heroTalisman,
            spawnSound = "humanmalegrunt",
            deathSound = "humanmalehurt",
            ResourcesGiven = new Dictionary<ResourceType, int> { { ResourceType.Gold, 145 }, { ResourceType.Corpse, 1 } }
        };

        // ==========================
        // CharacterS
        // ==========================

        public static readonly Monster slimeMonster = new Monster
        {
            Name = "Slime",
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
        public static readonly Monster strongSlime = new Monster
        {
            Name = "Strong Slime",
            faction = Faction.Defender,
            strength = 14,
            stamina = 17,
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
        public static readonly Monster goblin1 = new Monster
        {
            Name = "Goblin",
            faction = Faction.Defender,
            strength = 12,
            stamina = 5,
            agility = 12,
            intelligence = 3,
            spirit = 3,
            charWeapon = ItemDatabase.ironDagger,
            charArmor = ItemDatabase.tatteredArmor,
            charHelmet = ItemDatabase.crackedHelm,
            charAccessory = ItemDatabase.cursedTalisman,
        };

        public static readonly Monster ghost = new Monster
        {
            Name = "Ghost",
            faction = Faction.Defender,
            strength = 10,
            stamina = 10,
            agility = 10,
            intelligence = 10,
            spirit = 10,
            charWeapon = ItemDatabase.ironDagger,
            charArmor = ItemDatabase.tatteredArmor,
            charHelmet = ItemDatabase.crackedHelm,
            charAccessory = ItemDatabase.cursedTalisman,
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
            Name = "Archer",
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
            Name = "DungeonMaster",
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

        public static readonly Monster weakerDemon = new Monster
        {
            Name = "Red Demon",
            faction = Faction.Defender,
            isHealer = true,
            strength = 3,
            stamina = 15,
            agility = 15,
            intelligence = 10,
            spirit = 50,
            charWeapon = ItemDatabase.priestessStaff,
            charArmor = ItemDatabase.priestessRobe,
            charHelmet = ItemDatabase.leatherCap,
            charAccessory = ItemDatabase.charmOfFocus
        };
        public static readonly Monster strongerDemon = new Monster
        {
            Name = "Purple Demon",
            faction = Faction.Defender,
            isHealer = true,
            strength = 10,
            stamina = 20,
            agility = 15,
            intelligence = 15,
            spirit = 100,
            charWeapon = ItemDatabase.priestessStaff,
            charArmor = ItemDatabase.priestessRobe,
            charHelmet = ItemDatabase.leatherCap,
            charAccessory = ItemDatabase.charmOfFocus
        };
        public static readonly Monster redSpider = new Monster
        {
            Name = "Red Spider",
            faction = Faction.Defender,
            strength = 15,
            stamina = 20,
            agility = 25,
            intelligence = 13,
            spirit = 15,
            charWeapon = ItemDatabase.warriorAxe,
            charArmor = ItemDatabase.leatherArmor,
            charHelmet = ItemDatabase.leatherCap,
            charAccessory = ItemDatabase.charmOfFocus

        };
        public static readonly Monster purpleSpider = new Monster
        {
            Name = "Purple Spider",
            faction = Faction.Defender,
            strength = 25,
            stamina = 20,
            agility = 35,
            intelligence = 13,
            spirit = 15,
            charWeapon = ItemDatabase.warriorAxe,
            charArmor = ItemDatabase.leatherArmor,
            charHelmet = ItemDatabase.leatherCap,
            charAccessory = ItemDatabase.charmOfFocus
        };
    }
}
