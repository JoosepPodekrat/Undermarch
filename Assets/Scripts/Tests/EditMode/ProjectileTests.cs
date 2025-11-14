using NUnit.Framework;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Entities.Characters.Monsters;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Tests
{
    [TestFixture]
    public class ProjectileTests
    {
        [Test]
        public void Projectile_MovesInDirection()
        {
            IBoard board = new Board(20, 20);

            DamagePacket damage = new DamagePacket();
            damage.Add(DamageType.Physical, 5);

            Projectile projectile = new Projectile(
                "Arrow",
                new TilePos(5, 5),
                new TilePos(1, 0), // Moving right
                speed: 2,
                maxRange: 10,
                Faction.ProjectileDefender,
                damage
            );

            board.AddInteractable(projectile.Position, projectile);
            projectile.Tick(board);

            // Should move 2 tiles to the right
            Assert.AreEqual(new TilePos(7, 5), projectile.Position);
            Assert.IsTrue(projectile.IsActive);
        }

        [Test]
        public void Projectile_HitsWallAndDespawns()
        {
            IBoard board = new Board(20, 20);

            board.AddWall(new TilePos(7, 5));

            DamagePacket damage = new DamagePacket();
            damage.Add(DamageType.Physical, 5);

            Projectile projectile = new Projectile(
                "Arrow",
                new TilePos(5, 5),
                new TilePos(1, 0),
                speed: 3,
                maxRange: 10,
                Faction.ProjectileDefender,
                damage
            );

            board.AddInteractable(projectile.Position, projectile);
            projectile.Tick(board);

            Assert.IsFalse(projectile.IsActive);
        }

        [Test]
        public void Projectile_HitsEnemyAndDealsDamage()
        {
            IBoard board = new Board(20, 20);

            Character hero = CharacterDatabase.peasant.Clone();
            board.AddEntity(new TilePos(7, 5), hero);

            int initialHP = hero.currentHP;

            DamagePacket damage = new DamagePacket();
            damage.Add(DamageType.Physical, 10);

            Projectile projectile = new Projectile(
                "Arrow",
                new TilePos(5, 5),
                new TilePos(1, 0),
                speed: 3,
                maxRange: 10,
                Faction.ProjectileDefender,
                damage
            );

            board.AddInteractable(projectile.Position, projectile);
            projectile.Tick(board);

            Assert.IsFalse(projectile.IsActive);
            Assert.Less(hero.currentHP, initialHP);
        }

        [Test]
        public void Projectile_DespawnsAfterMaxRange()
        {
            IBoard board = new Board(20, 20);

            DamagePacket damage = new DamagePacket();
            damage.Add(DamageType.Physical, 5);

            Projectile projectile = new Projectile(
                "Arrow",
                new TilePos(5, 5),
                new TilePos(1, 0),
                speed: 3,
                maxRange: 5,
                Faction.ProjectileDefender,
                damage
            );

            board.AddInteractable(projectile.Position, projectile);

            // First tick: moves 3 tiles
            projectile.Tick(board);
            Assert.IsTrue(projectile.IsActive);

            // Second tick: moves 2 more tiles (total 5), hits max range
            projectile.Tick(board);
            Assert.IsFalse(projectile.IsActive);
        }

        [Test]
        public void ArcherMonster_ShootsWhenInRange()
        {
            IBoard board = new Board(20, 20);

            ArcherMonster archer = CharacterDatabase.archerMonster.Clone() as ArcherMonster;
            Character hero = CharacterDatabase.peasant.Clone();

            board.AddEntity(new TilePos(5, 5), archer);
            board.AddEntity(new TilePos(8, 5), hero); // 3 tiles away, within range

            archer.Act(board);

            // Check that a projectile was spawned
            object projectile = board.GetInteractableAt(new TilePos(5, 5));
            Assert.IsNotNull(projectile);
            Assert.IsInstanceOf<Projectile>(projectile);
        }

        [Test]
        public void ArcherMonster_MovesWhenOutOfRange()
        {
            IBoard board = new Board(20, 20);

            ArcherMonster archer = CharacterDatabase.archerMonster.Clone() as ArcherMonster;
            Character hero = CharacterDatabase.peasant.Clone();

            board.AddEntity(new TilePos(5, 5), archer);
            board.AddEntity(new TilePos(15, 15), hero); // Far away, out of range

            TilePos initialPos = board.GetPositionOf(archer);
            archer.Act(board);
            TilePos newPos = board.GetPositionOf(archer);

            // Archer should have moved toward hero
            Assert.AreNotEqual(initialPos, newPos);
        }
    }
}
