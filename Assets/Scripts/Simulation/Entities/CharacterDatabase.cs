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
            faction = Faction.Hero,
            strength = 12,
            stamina = 11,
            agility = 9,
            intelligence = 8,
            spirit = 10,
            charWeapon = ItemDatabase.ironSword,
            charArmor = ItemDatabase.chainmailArmor,
            charHelmet = ItemDatabase.ironHelmet,
            charAccessory = ItemDatabase.ironRing
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
            faction = Faction.Defender,
            strength = 9,
            stamina = 8,
            agility = 11,
            intelligence = 6,
            spirit = 7,
            charWeapon = ItemDatabase.ironDagger,
            charArmor = ItemDatabase.tatteredArmor,
            charHelmet = ItemDatabase.crackedHelm,
            charAccessory = ItemDatabase.cursedTalisman
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

        public static readonly DungeonMaster dungeonMaster = new DungeonMaster
        {
            faction = Faction.Defender,
            strength = 15,
            stamina = 20,
            agility = 10,
            intelligence = 18,
            spirit = 15,
            charWeapon = ItemDatabase.apprenticeStaff, // Placeholder
            charArmor = ItemDatabase.apprenticeRobe, // Placeholder
            charHelmet = ItemDatabase.mysticCowl, // Placeholder
            charAccessory = ItemDatabase.charmOfFocus // Placeholder
        };
    }
}
