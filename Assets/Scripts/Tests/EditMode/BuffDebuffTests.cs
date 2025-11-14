using NUnit.Framework;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Effects.Buffs;
using Undermarch.Simulation.Effects.Debuffs;
using Undermarch.Simulation.Equipment;

namespace Undermarch.Tests
{
    [TestFixture]
    public class BuffDebuffTests
    {
        [Test]
        public void Buff_IncreasesCharacterStrength()
        {
            Character character = new Character
            {
                strength = 10,
                charWeapon = ItemDatabase.ironSword,
                charArmor = ItemDatabase.leatherArmor,
                charHelmet = ItemDatabase.leatherCap,
                charAccessory = ItemDatabase.ironRing
            };
            character.CalculateStats();

            int baseStrength = character.effectiveStrength;

            StrengthBuff buff = new StrengthBuff(5, 3);
            buff.Add(character);
            character.CalculateStats();

            Assert.AreEqual(baseStrength + 5, character.effectiveStrength);
        }

        [Test]
        public void Debuff_DecreasesCharacterStrength()
        {
            Character character = new Character
            {
                strength = 10,
                charWeapon = ItemDatabase.ironSword,
                charArmor = ItemDatabase.leatherArmor,
                charHelmet = ItemDatabase.leatherCap,
                charAccessory = ItemDatabase.ironRing
            };
            character.CalculateStats();

            int baseStrength = character.effectiveStrength;

            WeakDebuff debuff = new WeakDebuff(3, 2);
            debuff.Add(character);
            character.CalculateStats();

            Assert.AreEqual(baseStrength - 3, character.effectiveStrength);
        }

        [Test]
        public void BuffsAndDebuffs_Stack()
        {
            Character character = new Character
            {
                strength = 10,
                charWeapon = ItemDatabase.ironSword,
                charArmor = ItemDatabase.leatherArmor,
                charHelmet = ItemDatabase.leatherCap,
                charAccessory = ItemDatabase.ironRing
            };
            character.CalculateStats();

            int baseStrength = character.effectiveStrength;

            StrengthBuff buff = new StrengthBuff(5, 3);
            WeakDebuff debuff = new WeakDebuff(2, 2);

            buff.Add(character);
            debuff.Add(character);
            character.CalculateStats();

            Assert.AreEqual(baseStrength + 5 - 2, character.effectiveStrength);
        }

        [Test]
        public void Buff_ExpiresAfterDuration()
        {
            Character character = new Character
            {
                strength = 10,
                charWeapon = ItemDatabase.ironSword,
                charArmor = ItemDatabase.leatherArmor,
                charHelmet = ItemDatabase.leatherCap,
                charAccessory = ItemDatabase.ironRing
            };
            character.CalculateStats();

            int baseStrength = character.effectiveStrength;

            StrengthBuff buff = new StrengthBuff(5, 2);
            buff.Add(character);
            character.CalculateStats();

            Assert.AreEqual(baseStrength + 5, character.effectiveStrength);

            // Tick once
            character.TickBuffsAndDebuffs();
            Assert.AreEqual(1, buff.duration);
            Assert.AreEqual(baseStrength + 5, character.effectiveStrength);

            // Tick twice
            character.TickBuffsAndDebuffs();
            Assert.AreEqual(0, buff.duration);

            // Buff should be removed now
            Assert.AreEqual(0, character.buffs.Count);
            Assert.AreEqual(baseStrength, character.effectiveStrength);
        }
    }
}
