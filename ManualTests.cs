using System;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Entities.Characters.Heroes;
using Undermarch.Simulation.Entities.Characters.Monsters;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Interfaces;
using Undermarch.Simulation.Effects.Buffs;
using Undermarch.Simulation.Effects.Debuffs;

namespace Undermarch.Tests
{
    public class ManualTests
    {
        private static int passedTests = 0;
        private static int failedTests = 0;

        public static void Main(string[] args)
        {
            Console.WriteLine("=== UNDERMARCH SIMULATION TESTS ===\n");

            // Run all test categories
            TestRandomSource();
            TestGameState();
            TestBoard();
            TestBuffDebuff();
            TestProjectiles();
            TestTileEffects();
            TestChestAndHeroAI();
            TestTickSystem();

            // Summary
            Console.WriteLine("\n=== TEST SUMMARY ===");
            Console.WriteLine($"Passed: {passedTests}");
            Console.WriteLine($"Failed: {failedTests}");
            Console.WriteLine($"Total: {passedTests + failedTests}");

            if (failedTests == 0)
            {
                Console.WriteLine("\n✓ ALL TESTS PASSED!");
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine($"\n✗ {failedTests} TEST(S) FAILED");
                Environment.Exit(1);
            }
        }

        private static void Assert(bool condition, string testName)
        {
            if (condition)
            {
                passedTests++;
                Console.WriteLine($"  ✓ {testName}");
            }
            else
            {
                failedTests++;
                Console.WriteLine($"  ✗ {testName} FAILED");
            }
        }

        private static void AssertEqual<T>(T expected, T actual, string testName)
        {
            bool equal = expected == null ? actual == null : expected.Equals(actual);
            if (equal)
            {
                passedTests++;
                Console.WriteLine($"  ✓ {testName}");
            }
            else
            {
                failedTests++;
                Console.WriteLine($"  ✗ {testName} FAILED: Expected {expected}, got {actual}");
            }
        }

        private static void TestRandomSource()
        {
            Console.WriteLine("\n[RandomSource Tests]");

            // Test 1: Same seed produces same sequence
            SeededRandom rng1 = new SeededRandom(12345);
            SeededRandom rng2 = new SeededRandom(12345);
            bool sameSequence = true;
            for (int i = 0; i < 10; i++)
            {
                if (rng1.Next() != rng2.Next())
                {
                    sameSequence = false;
                    break;
                }
            }
            Assert(sameSequence, "Same seed produces same sequence");

            // Test 2: Reset works
            SeededRandom rng3 = new SeededRandom(999);
            int first = rng3.Next();
            rng3.Next();
            rng3.Reset();
            int afterReset = rng3.Next();
            AssertEqual(first, afterReset, "Reset restarts sequence");

            // Test 3: Range works
            SeededRandom rng4 = new SeededRandom(555);
            bool inRange = true;
            for (int i = 0; i < 20; i++)
            {
                int val = rng4.Next(5, 15);
                if (val < 5 || val >= 15)
                {
                    inRange = false;
                    break;
                }
            }
            Assert(inRange, "Next(min, max) stays in range");
        }

        private static void TestGameState()
        {
            Console.WriteLine("\n[GameState Tests]");

            GameState gs = new GameState(100, 1);
            AssertEqual(100, gs.CurrentGold, "Initial gold correct");
            AssertEqual(1, gs.Wave, "Initial wave correct");

            gs.SpendGold(30);
            AssertEqual(70, gs.CurrentGold, "Spend gold works");

            gs.EarnGold(50);
            AssertEqual(120, gs.CurrentGold, "Earn gold works");

            Assert(gs.CanAfford(100), "CanAfford returns true when sufficient");
            Assert(!gs.CanAfford(200), "CanAfford returns false when insufficient");

            bool eventFired = false;
            gs.OnResourcesChanged += () => eventFired = true;
            gs.SpendGold(10);
            Assert(eventFired, "OnResourcesChanged event fires");
        }

        private static void TestBoard()
        {
            Console.WriteLine("\n[Board Tests]");

            Board board = new Board(10, 10);
            AssertEqual(10, board.Width, "Board width correct");
            AssertEqual(10, board.Height, "Board height correct");

            Assert(board.InBounds(new TilePos(5, 5)), "InBounds works for valid pos");
            Assert(!board.InBounds(new TilePos(-1, 5)), "InBounds works for invalid pos");

            board.AddWall(new TilePos(3, 3));
            Assert(board.HasWallAt(new TilePos(3, 3)), "AddWall works");

            Character char1 = new Character { Name = "TestChar" };
            board.AddEntity(new TilePos(5, 5), char1);
            AssertEqual(char1, board.GetEntityAt(new TilePos(5, 5)), "AddEntity works");

            board.MoveEntity(new TilePos(5, 5), new TilePos(6, 6));
            AssertEqual(char1, board.GetEntityAt(new TilePos(6, 6)), "MoveEntity works");
            Assert(board.GetEntityAt(new TilePos(5, 5)) == null, "MoveEntity clears old position");
        }

        private static void TestBuffDebuff()
        {
            Console.WriteLine("\n[Buff/Debuff Tests]");

            Character character = CharacterDatabase.peasant.Clone();
            character.CalculateStats();
            int baseStrength = character.effectiveStrength;

            StrengthBuff buff = new StrengthBuff(5, 3);
            buff.Add(character);
            character.CalculateStats();
            AssertEqual(baseStrength + 5, character.effectiveStrength, "Buff increases stat");

            WeakDebuff debuff = new WeakDebuff(2, 2);
            debuff.Add(character);
            character.CalculateStats();
            AssertEqual(baseStrength + 5 - 2, character.effectiveStrength, "Debuff decreases stat");

            // Tick down
            character.TickBuffsAndDebuffs();
            AssertEqual(2, buff.duration, "Buff ticks down");

            character.TickBuffsAndDebuffs();
            character.TickBuffsAndDebuffs();
            AssertEqual(0, character.buffs.Count, "Buff removed after expiry");
            AssertEqual(0, character.debuffs.Count, "Debuff removed after expiry");

            character.CalculateStats();
            AssertEqual(baseStrength, character.effectiveStrength, "Stats return to normal after expiry");
        }

        private static void TestProjectiles()
        {
            Console.WriteLine("\n[Projectile Tests]");

            Board board = new Board(20, 20);

            // Test projectile movement
            DamagePacket damage = new DamagePacket();
            damage.Add(DamageType.Physical, 5);

            Projectile proj = new Projectile(
                "Arrow",
                new TilePos(5, 5),
                new TilePos(1, 0),
                speed: 2,
                maxRange: 10,
                Faction.ProjectileDefender,
                damage
            );

            board.AddInteractable(proj.Position, proj);
            proj.Tick(board);
            AssertEqual(new TilePos(7, 5), proj.Position, "Projectile moves correctly");

            // Test projectile hits wall
            Board board2 = new Board(20, 20);
            board2.AddWall(new TilePos(7, 5));

            Projectile proj2 = new Projectile(
                "Arrow",
                new TilePos(5, 5),
                new TilePos(1, 0),
                speed: 3,
                maxRange: 10,
                Faction.ProjectileDefender,
                damage
            );

            board2.AddInteractable(proj2.Position, proj2);
            proj2.Tick(board2);
            Assert(!proj2.IsActive, "Projectile despawns on wall hit");

            // Test archer shoots
            Board board3 = new Board(20, 20);
            ArcherMonster archer = CharacterDatabase.archerMonster.Clone() as ArcherMonster;
            Character hero = CharacterDatabase.peasant.Clone();

            board3.AddEntity(new TilePos(5, 5), archer);
            board3.AddEntity(new TilePos(8, 5), hero);

            archer.Act(board3);
            object projectile = board3.GetInteractableAt(new TilePos(5, 5));
            Assert(projectile is Projectile, "Archer shoots projectile when in range");
        }

        private static void TestTileEffects()
        {
            Console.WriteLine("\n[TileEffect Tests]");

            Character character = CharacterDatabase.peasant.Clone();
            int initialHP = character.currentHP;

            TileEffect poisonCloud = new TileEffect(EffectType.Poison, new TilePos(5, 5), duration: 3, intensity: 5);
            poisonCloud.ApplyTo(character);
            Assert(character.currentHP < initialHP, "Poison effect deals damage");

            Character character2 = CharacterDatabase.peasant.Clone();
            character2.CalculateStats();
            int baseAgility = character2.effectiveAgility;

            TileEffect slowZone = new TileEffect(EffectType.Slow, new TilePos(5, 5), duration: 5, intensity: 3);
            slowZone.ApplyTo(character2);
            character2.CalculateStats();
            Assert(character2.effectiveAgility < baseAgility, "Slow effect reduces agility");

            TileEffect effect = new TileEffect(EffectType.Poison, new TilePos(5, 5), duration: 2, intensity: 1);
            effect.Tick();
            effect.Tick();
            Assert(effect.IsExpired(), "Tile effect expires after duration");
        }

        private static void TestChestAndHeroAI()
        {
            Console.WriteLine("\n[Chest & Hero AI Tests]");

            // Test chest looting
            Hero hero = CharacterDatabase.peasant.Clone() as Hero;
            Chest chest = new Chest(new TilePos(5, 5), goldAmount: 50);

            AssertEqual(0, hero.gold, "Hero starts with no gold");
            chest.Interact(hero);
            AssertEqual(50, hero.gold, "Hero loots chest");
            Assert(chest.Looted, "Chest marked as looted");

            chest.Interact(hero);
            AssertEqual(50, hero.gold, "Chest can't be looted twice");

            // Test hero flee condition
            Hero hero2 = CharacterDatabase.peasant.Clone() as Hero;
            hero2.FleeThreshold = 10;
            hero2.gold = 50;
            hero2.currentHP = hero2.maxHP / 4;

            float healthPercent = (float)hero2.currentHP / hero2.maxHP;
            float ratio = hero2.gold / healthPercent;
            Assert(ratio > hero2.FleeThreshold, "Hero should flee with low health and gold");
        }

        private static void TestTickSystem()
        {
            Console.WriteLine("\n[TickSystem Tests]");

            Board board = new Board(20, 20);
            GameState gameState = new GameState(200);
            TickSystem tickSystem = new TickSystem(board, gameState, ticksPerSecond: 10);

            AssertEqual(TickMode.Paused, tickSystem.Mode, "TickSystem starts paused");
            AssertEqual(0, tickSystem.CurrentTick, "TickSystem starts at tick 0");

            tickSystem.Resume();
            AssertEqual(TickMode.Auto, tickSystem.Mode, "Resume sets mode to Auto");

            tickSystem.Pause();
            AssertEqual(TickMode.Paused, tickSystem.Mode, "Pause sets mode to Paused");

            gameState.Phase = GamePhase.Combat;
            tickSystem.Step();
            AssertEqual(1, tickSystem.CurrentTick, "Step advances one tick");
            AssertEqual(TickMode.Paused, tickSystem.Mode, "Step auto-pauses");

            // Test victory condition
            Board board2 = new Board(20, 20);
            GameState gameState2 = new GameState(200);
            gameState2.Phase = GamePhase.Combat;
            Character dm = CharacterDatabase.dungeonMaster.Clone();
            board2.AddEntity(new TilePos(10, 10), dm);

            TickSystem tickSystem2 = new TickSystem(board2, gameState2);
            tickSystem2.Tick();

            AssertEqual(GamePhase.GameOver, gameState2.Phase, "Game ends when no heroes");
            AssertEqual(TickMode.Paused, tickSystem2.Mode, "TickSystem pauses on game over");
        }
    }
}
