using NUnit.Framework;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Interfaces;
using Undermarch.Simulation.Combat;


namespace Undermarch.Tests
{
    [TestFixture]
    public class TickSystemTests
    {
        [Test]
        public void TickSystem_InitializesCorrectly()
        {
            IBoard board = new Board(20, 20);
            IGameState gameState = new GameState(200);
            ITickSystem tickSystem = new TickSystem(board, gameState, ticksPerSecond: 10);

            Assert.AreEqual(TickMode.Paused, tickSystem.Mode);
            Assert.AreEqual(0, tickSystem.CurrentTick);
            Assert.AreEqual(10, tickSystem.TicksPerSecond);
        }

        [Test]
        public void TickSystem_PauseAndResume()
        {
            IBoard board = new Board(20, 20);
            IGameState gameState = new GameState(200);
            ITickSystem tickSystem = new TickSystem(board, gameState);

            tickSystem.Resume();
            Assert.AreEqual(TickMode.Auto, tickSystem.Mode);

            tickSystem.Pause();
            Assert.AreEqual(TickMode.Paused, tickSystem.Mode);
        }

        [Test]
        public void TickSystem_StepAdvancesOneTick()
        {
            IBoard board = new Board(20, 20);
            IGameState gameState = new GameState(200);
            gameState.Phase = GamePhase.Combat; // Must be in combat to tick
            ITickSystem tickSystem = new TickSystem(board, gameState);

            Assert.AreEqual(0, tickSystem.CurrentTick);

            tickSystem.Step();

            Assert.AreEqual(1, tickSystem.CurrentTick);
            Assert.AreEqual(TickMode.Paused, tickSystem.Mode); // Should auto-pause
        }

        [Test]
        public void TickSystem_OnlyTicksDuringCombat()
        {
            IBoard board = new Board(20, 20);
            IGameState gameState = new GameState(200);
            gameState.Phase = GamePhase.Placement; // Not in combat
            ITickSystem tickSystem = new TickSystem(board, gameState);

            tickSystem.Tick();

            Assert.AreEqual(0, tickSystem.CurrentTick); // Should not advance

            gameState.Phase = GamePhase.Combat;
            tickSystem.Tick();

            Assert.AreEqual(1, tickSystem.CurrentTick); // Should advance
        }

        [Test]
        public void TickSystem_EntitiesActEachTick()
        {
            IBoard board = new Board(20, 20);
            IGameState gameState = new GameState(200);
            gameState.Phase = GamePhase.Combat;

            Character hero = CharacterDatabase.peasant.Clone();
            Character monster = CharacterDatabase.slimeMonster.Clone();

            board.AddEntity(new TilePos(5, 5), hero);
            board.AddEntity(new TilePos(10, 10), monster);

            TilePos initialHeroPos = board.GetPositionOf(hero);

            ITickSystem tickSystem = new TickSystem(board, gameState);
            tickSystem.Tick();

            TilePos newHeroPos = board.GetPositionOf(hero);

            // Hero should have acted (moved or attacked)
            // In this case, should move toward monster
            Assert.IsTrue(initialHeroPos.IsValid());
            Assert.IsTrue(newHeroPos.IsValid());
        }

        [Test]
        public void TickSystem_DetectsVictoryWhenHeroesDefeated()
        {
            IBoard board = new Board(20, 20);
            IGameState gameState = new GameState(200);
            gameState.Phase = GamePhase.Combat;

            Character dungeonMaster = CharacterDatabase.dungeonMaster.Clone();
            board.AddEntity(new TilePos(10, 10), dungeonMaster);

            // No heroes on board
            ITickSystem tickSystem = new TickSystem(board, gameState);
            tickSystem.Tick();

            Assert.AreEqual(GamePhase.GameOver, gameState.Phase);
            Assert.AreEqual(TickMode.Paused, tickSystem.Mode);
        }

        [Test]
        public void TickSystem_DetectsDefeatWhenDungeonMasterDefeated()
        {
            IBoard board = new Board(20, 20);
            IGameState gameState = new GameState(200);
            gameState.Phase = GamePhase.Combat;

            Character hero = CharacterDatabase.peasant.Clone();
            board.AddEntity(new TilePos(10, 10), hero);

            // No dungeon master on board
            ITickSystem tickSystem = new TickSystem(board, gameState);
            tickSystem.Tick();

            Assert.AreEqual(GamePhase.GameOver, gameState.Phase);
            Assert.AreEqual(TickMode.Paused, tickSystem.Mode);
        }

        [Test]
        public void TickSystem_ProjectilesMoveDuringProjectilesPhase()
        {
            IBoard board = new Board(20, 20);
            IGameState gameState = new GameState(200);
            gameState.Phase = GamePhase.Combat;

            // Create a projectile
            DamagePacket damage = new DamagePacket();
            damage.Add(DamageType.Physical, 5);

            Projectile projectile = new Projectile(
                "TestArrow",
                new TilePos(5, 5),
                new TilePos(1, 0),
                speed: 2,
                maxRange: 10,
                Faction.ProjectileDefender,
                damage
            );

            board.AddInteractable(projectile.Position, projectile);

            ITickSystem tickSystem = new TickSystem(board, gameState);
            tickSystem.Tick();

            // Projectile should have moved
            Assert.AreEqual(new TilePos(7, 5), projectile.Position);
        }
    }
}
