using NUnit.Framework;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Tests
{
    [TestFixture]
    public class TileEffectTests
    {
        [Test]
        public void TileEffect_PoisonCloudDealsDamage()
        {
            Character character = CharacterDatabase.peasant.Clone();
            int initialHP = character.currentHP;

            TileEffect poisonCloud = new TileEffect(EffectType.Poison, new TilePos(5, 5), duration: 5, intensity: 3);
            poisonCloud.ApplyTo(character);

            Assert.Less(character.currentHP, initialHP);
        }

        [Test]
        public void TileEffect_SlowZoneAppliesSlowDebuff()
        {
            Character character = CharacterDatabase.peasant.Clone();
            character.CalculateStats();

            int baseAgility = character.effectiveAgility;

            TileEffect slowZone = new TileEffect(EffectType.Slow, new TilePos(5, 5), duration: 5, intensity: 3);
            slowZone.ApplyTo(character);

            character.CalculateStats();

            Assert.Less(character.effectiveAgility, baseAgility);
            Assert.AreEqual(1, character.debuffs.Count);
        }

        [Test]
        public void TileEffect_ExpiresAfterDuration()
        {
            TileEffect effect = new TileEffect(EffectType.Poison, new TilePos(5, 5), duration: 2, intensity: 1);

            Assert.IsFalse(effect.IsExpired());

            effect.Tick();
            Assert.IsFalse(effect.IsExpired());

            effect.Tick();
            Assert.IsTrue(effect.IsExpired());
        }

        [Test]
        public void Character_AppliesTileEffectOnMove()
        {
            IBoard board = new Board(20, 20);
            Character character = CharacterDatabase.peasant.Clone();

            board.AddEntity(new TilePos(5, 5), character);

            // Place poison cloud at (6, 5)
            TileEffect poisonCloud = new TileEffect(EffectType.Poison, new TilePos(6, 5), duration: 5, intensity: 5);
            board.AddInteractable(new TilePos(6, 5), poisonCloud);

            int initialHP = character.currentHP;

            // Move character into poison cloud
            character.HandleMove(board, new TilePos(5, 5), new TilePos(6, 5));

            // Character should have taken poison damage
            Assert.Less(character.currentHP, initialHP);
        }
    }
}
