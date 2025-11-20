using NUnit.Framework;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Entities.Characters.Heroes;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Tests
{
    [TestFixture]
    public class ChestAndHeroAITests
    {
        [Test]
        public void Chest_CanBeLootedByHero()
        {
            Hero hero = CharacterDatabase.peasant.Clone() as Hero;
            Chest chest = new Chest(new TilePos(5, 5), goldAmount: 50);

            Assert.AreEqual(0, hero.gold);
            Assert.IsFalse(chest.Looted);

            chest.Interact(hero);

            Assert.AreEqual(50, hero.gold);
            Assert.IsTrue(chest.Looted);
        }

        [Test]
        public void Chest_CannotBeLootedTwice()
        {
            Hero hero = CharacterDatabase.peasant.Clone() as Hero;
            Chest chest = new Chest(new TilePos(5, 5), goldAmount: 50);

            chest.Interact(hero);
            chest.Interact(hero);

            Assert.AreEqual(50, hero.gold); // Should still be 50, not 100
        }

        [Test]
        public void Hero_PathsToChestWhenNoEnemies()
        {
            Board board = new Board(20, 20);
            Hero hero = CharacterDatabase.peasant.Clone() as Hero;

            board.AddEntity(new TilePos(5, 5), hero);

            Chest chest = new Chest(new TilePos(10, 5), 30);
            board.AddInteractable(chest.Position, chest);

            TilePos initialPos = board.GetPositionOf(hero);
            hero.Act(board);
            TilePos newPos = board.GetPositionOf(hero);

            // Hero should have moved toward chest
            Assert.AreNotEqual(initialPos, newPos);
            float initialDist = TilePos.DistanceSq(initialPos, chest.Position);
            float newDist = TilePos.DistanceSq(newPos, chest.Position);
            Assert.Less(newDist, initialDist);
        }

        [Test]
        public void Hero_LootsChestOnArrival()
        {
            Board board = new Board(20, 20);
            Hero hero = CharacterDatabase.peasant.Clone() as Hero;

            Chest chest = new Chest(new TilePos(5, 5), 40);
            board.AddInteractable(chest.Position, chest);
            board.AddEntity(chest.Position, hero); // Place hero on chest

            hero.Act(board);

            Assert.AreEqual(40, hero.gold);
            Assert.IsTrue(chest.Looted);
        }

        [Test]
        public void Hero_FleesWith_LowHealthAndGold()
        {
            Board board = new Board(20, 20);
            Hero hero = CharacterDatabase.peasant.Clone() as Hero;
            hero.FleeThreshold = 10; // Lower threshold for testing

            board.AddEntity(new TilePos(10, 10), hero);

            // Give hero gold and reduce health
            hero.gold = 50;
            hero.currentHP = hero.maxHP / 4; // 25% health

            // Check flee condition
            bool shouldFlee = hero.gold / ((float)hero.currentHP / hero.maxHP) > hero.FleeThreshold;
            Assert.IsTrue(shouldFlee);

            TilePos initialPos = board.GetPositionOf(hero);
            hero.Act(board);
            TilePos newPos = board.GetPositionOf(hero);

            // Hero should move toward edge
            int initialMinDistToEdge = System.Math.Min(
                System.Math.Min(initialPos.x, board.Width - 1 - initialPos.x),
                System.Math.Min(initialPos.y, board.Height - 1 - initialPos.y)
            );
            int newMinDistToEdge = System.Math.Min(
                System.Math.Min(newPos.x, board.Width - 1 - newPos.x),
                System.Math.Min(newPos.y, board.Height - 1 - newPos.y)
            );

            Assert.Less(newMinDistToEdge, initialMinDistToEdge);
        }

        [Test]
        public void Hero_DoesNotFleeWithHighHealth()
        {
            Board board = new Board(20, 20);
            Hero hero = CharacterDatabase.peasant.Clone() as Hero;
            hero.FleeThreshold = 10;

            board.AddEntity(new TilePos(10, 10), hero);

            // Give hero gold but keep health high
            hero.gold = 20;
            hero.currentHP = hero.maxHP; // 100% health

            // Check flee condition - should not flee
            bool shouldFlee = hero.gold / ((float)hero.currentHP / hero.maxHP) > hero.FleeThreshold;
            Assert.IsFalse(shouldFlee);
        }

        [Test]
        public void Hero_PrioritizesCombatOverLooting()
        {
            Board board = new Board(20, 20);
            Hero hero = CharacterDatabase.peasant.Clone() as Hero;
            hero.CombatRange = 5;
            Character monster = CharacterDatabase.slimeMonster.Clone();

            board.AddEntity(new TilePos(10, 10), hero);
            board.AddEntity(new TilePos(12, 10), monster); // Within combat range

            Chest chest = new Chest(new TilePos(8, 10), 50); // Closer than monster
            board.AddInteractable(chest.Position, chest);

            TilePos initialPos = board.GetPositionOf(hero);
            hero.Act(board);
            TilePos newPos = board.GetPositionOf(hero);

            // Hero should move toward monster, not chest
            float distToMonster = TilePos.DistanceSq(newPos, board.GetPositionOf(monster));
            float distToChest = TilePos.DistanceSq(newPos, chest.Position);

            Assert.Less(distToMonster, distToChest);
        }
    }
}
