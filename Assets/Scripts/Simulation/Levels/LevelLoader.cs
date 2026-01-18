using log4net.Core;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Events;
using Undermarch.Simulation.Grid;



namespace Undermarch.Simulation.Levels
{
    public static class LevelLoader
    {
        /// <summary>
        /// Loads the main dungeon level.
        /// Legend:
        /// # = Wall
        /// E = Entrance (hero spawn point)
        /// C = Chest
        /// D = Dungeon Master
        /// X = Exit door
        /// </summary>
        public static void LoadDungeon(Board board, Level level, string[] levelLayout)
        {
            // Ensure lists are ready (defensive, in case caller forgot)
            level.Entrances.Clear();
            level.ChestPositions.Clear();

            for (int y = 0; y < levelLayout.Length; y++)
            {
                int boardY = board.Height - 1 - y;

                for (int x = 0; x < levelLayout[y].Length; x++)
                {
                    var pos = new TilePos(x, boardY);
                    char tileType = levelLayout[y][x];

                    switch (tileType)
                    {
                        case '#':
                            board.AddWall(pos);
                            break;

                        case 'E':
                            level.Entrances.Add(pos);
                            break;

                        case 'D':
                            var dm = CharacterDatabase.dungeonMaster.Clone();
                            board.AddEntity(pos, dm);
                            CharacterEvents.RaiseSpawn(dm);
                            break;

                        case 'C':
                            var chest = new Chest(pos, goldAmount: 30);
                            board.AddInteractable(pos, chest);
                            level.ChestPositions.Add(pos);
                            break;

                        case 'X':
                            var exitDoor = new Door(pos, -1, TilePos.Invalid, isExit: true);
                            board.AddInteractable(pos, exitDoor);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Helper: Creates a HeroParty with cloned characters.
        /// </summary>
        private static HeroParty CreateHeroParty(List<Character> heroes, TilePos position, bool isFinal = false)
        {
            var clonedHeroes = new List<Character>();
            foreach (var hero in heroes)
            {
                var cloned = hero.Clone();
                // Set audio properties for all heroes
                cloned.spawnSound = "humanmalegrunt";
                cloned.hurtSound = "humanMaleHurt";
                cloned.attackSound = "humanMaleGrunt";
                cloned.deathSound = "humanMaleHurt";
                clonedHeroes.Add(cloned);
            }

            var party = new HeroParty(clonedHeroes, 0, position, isFinal);

            // Final wave heroes never flee
            if (isFinal)
            {
                foreach (var hero in party.Heroes)
                {
                    if (hero is Entities.Characters.Heroes.Hero h)
                    {
                        h.FleeThreshold = int.MaxValue;
                    }
                }
            }

            return party;
        }

        /// <summary>
        /// Helper: Schedules a wave with the spawner.
        /// </summary>
        private static void ScheduleWave(HeroParty wave, WaveSpawner spawner)
        {
            spawner.ScheduleWave(wave);
        }

        /// <summary>
        /// Creates the first wave schedule with 9 waves.
        /// Waves auto-spawn every ~15 seconds (30 ticks @ 2 TPS).
        /// Player can click "NEXT WAVE" to spawn early.
        /// </summary>
        /// 
        public static WaveSpawner CreateWaveScheduleTutorial(List<TilePos> entrances)
        {
            WaveSpawner spawner = new WaveSpawner();
            spawner.ResetScheduler(); // Start at tick 0

            TilePos entrance = entrances.Count > 0 ? entrances[0] : new TilePos(9, 1);

            // Wave 1: Single peasant (tutorial wave)
            var wave1 = CreateHeroParty(new List<Character> { CharacterDatabase.peasant }, entrance);
            wave1.TicksUntilNextWave = 20;
            ScheduleWave(wave1, spawner);

            // Wave 2: Two peasants
            var wave2 = CreateHeroParty(new List<Character> { CharacterDatabase.peasant, CharacterDatabase.peasant }, entrance);
            wave2.TicksUntilNextWave = 20;
            ScheduleWave(wave2, spawner);

            // Wave 3: Three peasants.
            var wave3 = CreateHeroParty(new List<Character> { CharacterDatabase.peasant, CharacterDatabase.peasant, CharacterDatabase.peasant, CharacterDatabase.peasant }, entrance);
            wave3.TicksUntilNextWave = 20; 
            ScheduleWave(wave3, spawner);
            //Boss wave. Should kill DM if not defended.
            var wave4 = CreateHeroParty(new List<Character> { CharacterDatabase.knight, CharacterDatabase.apprenticeMage }, entrance);
            wave4.TicksUntilNextWave = 20;
            ScheduleWave(wave4, spawner);

            return spawner;
        }
        public static WaveSpawner CreateWaveSchedule1Level1(List<TilePos> entrances)
        {
            WaveSpawner spawner = new WaveSpawner();
            spawner.ResetScheduler(); // Start at tick 0

            TilePos entrance = entrances.Count > 0 ? entrances[0] : new TilePos(9, 1);

            // Wave 1: Single peasant (tutorial wave)
            var wave1 = CreateHeroParty(new List<Character> { CharacterDatabase.peasant }, entrance);
            wave1.TicksUntilNextWave = 15; // ~15 seconds
            ScheduleWave(wave1, spawner);

            // Wave 2: Two peasants
            var wave2 = CreateHeroParty(new List<Character> { CharacterDatabase.peasant, CharacterDatabase.peasant }, entrance);
            wave2.TicksUntilNextWave = 30;
            ScheduleWave(wave2, spawner);

            // Wave 3: First rogue
            var wave3 = CreateHeroParty(new List<Character> { CharacterDatabase.rogue }, entrance);
            wave3.TicksUntilNextWave = 35; // Slightly longer for harder waves
            ScheduleWave(wave3, spawner);

            // Wave 4: Mage + peasant combo
            var wave4 = CreateHeroParty(new List<Character> { CharacterDatabase.apprenticeMage, CharacterDatabase.peasant }, entrance);
            wave4.TicksUntilNextWave = 35;
            ScheduleWave(wave4, spawner);

            // Wave 5: Double rogues
            var wave5 = CreateHeroParty(new List<Character> { CharacterDatabase.rogue, CharacterDatabase.rogue }, entrance);
            wave5.TicksUntilNextWave = 40;
            ScheduleWave(wave5, spawner);

            // Wave 6: Mixed group
            var wave6 = CreateHeroParty(new List<Character> {
                CharacterDatabase.peasant, CharacterDatabase.rogue, CharacterDatabase.apprenticeMage
            }, entrance);
            wave6.TicksUntilNextWave = 40;
            ScheduleWave(wave6, spawner);

            // Wave 7: Triple peasants (swarm)
            var wave7 = CreateHeroParty(new List<Character> {
                CharacterDatabase.peasant, CharacterDatabase.peasant, CharacterDatabase.peasant
            }, entrance);
            wave7.TicksUntilNextWave = 20;
            ScheduleWave(wave7, spawner);

            // Wave 8: Double mages
            var wave8 = CreateHeroParty(new List<Character> {
                CharacterDatabase.apprenticeMage, CharacterDatabase.apprenticeMage
            }, entrance);
            wave8.TicksUntilNextWave = 20;
            ScheduleWave(wave8, spawner);

            // Wave 9: Final wave - full squad, never flees
            var wave9 = CreateHeroParty(new List<Character> {
                CharacterDatabase.rogue, CharacterDatabase.rogue,
                CharacterDatabase.apprenticeMage, CharacterDatabase.peasant
            }, entrance, isFinal: true);
            wave9.TicksUntilNextWave = 0; // No delay after final wave
            ScheduleWave(wave9, spawner);

            return spawner;
        }

        /// <summary>
        /// Creates the second stage wave schedule with 5 harder waves.
        /// </summary>
        public static WaveSpawner CreateWaveSchedule2Level1(List<TilePos> entrances)
        {
            WaveSpawner spawner = new WaveSpawner();
            spawner.ResetScheduler();

            TilePos entrance = entrances.Count > 0 ? entrances[0] : new TilePos(9, 1);

            // Wave 1: Double rogues immediately
            var wave1 = CreateHeroParty(new List<Character> {
                CharacterDatabase.rogue, CharacterDatabase.rogue
            }, entrance);
            wave1.TicksUntilNextWave = 20;
            ScheduleWave(wave1, spawner);

            // Wave 2: Double mages
            var wave2 = CreateHeroParty(new List<Character> {
                CharacterDatabase.apprenticeMage, CharacterDatabase.apprenticeMage
            }, entrance);
            wave2.TicksUntilNextWave = 20;
            ScheduleWave(wave2, spawner);

            // Wave 3: Peasant swarm 8
            var wave3 = CreateHeroParty(new List<Character> {
                CharacterDatabase.peasant, CharacterDatabase.peasant,
                CharacterDatabase.peasant, CharacterDatabase.peasant,CharacterDatabase.peasant, CharacterDatabase.peasant,
                CharacterDatabase.peasant, CharacterDatabase.peasant
            }, entrance);
            wave3.TicksUntilNextWave = 20;
            ScheduleWave(wave3, spawner);

            // Wave 4: Mixed dangerous group
            var wave4 = CreateHeroParty(new List<Character> {
                CharacterDatabase.knight, CharacterDatabase.rogue, CharacterDatabase.apprenticeMage,
                
            }, entrance);
            wave4.TicksUntilNextWave = 20;
            ScheduleWave(wave4, spawner);

            // Wave 5: Final wave - largest group, never flees
            var wave5 = CreateHeroParty(new List<Character> {
                CharacterDatabase.rogue, CharacterDatabase.rogue,
                CharacterDatabase.apprenticeMage, CharacterDatabase.priestess,
                CharacterDatabase.knight, CharacterDatabase.warrior
            }, entrance, isFinal: true);
            wave5.TicksUntilNextWave = 0;
            ScheduleWave(wave5, spawner);

            return spawner;
        }

        public static WaveSpawner CreateWaveSchedule1Level2(List<TilePos> entrances)
        {
            WaveSpawner spawner = new WaveSpawner();
            spawner.ResetScheduler();

            TilePos entrance = entrances.Count > 0 ? entrances[0] : new TilePos(9, 1);
            var wave1 = CreateHeroParty(new List<Character> {
                 CharacterDatabase.peasant, CharacterDatabase.peasant
            }, entrance);
            wave1.TicksUntilNextWave = 20;
            ScheduleWave(wave1, spawner);
            var wave2 = CreateHeroParty(new List<Character>
            {
                CharacterDatabase.priestess,  CharacterDatabase.peasant
            }, entrance);
            wave2.TicksUntilNextWave = 20;
            ScheduleWave(wave2, spawner);
            var wave3 = CreateHeroParty(new List<Character>
            {
                CharacterDatabase.warrior, CharacterDatabase.warrior
            }, entrance);
            wave3.TicksUntilNextWave = 20;
            ScheduleWave(wave3, spawner);
            var wave4 = CreateHeroParty(new List<Character>
            {
                CharacterDatabase.warrior, CharacterDatabase.priestess
            }, entrance);
            wave4.TicksUntilNextWave = 20;
            ScheduleWave(wave4, spawner);
            var wave5 = CreateHeroParty(new List<Character>
            {
                CharacterDatabase.Mage,CharacterDatabase.rogue
            }, entrance);
            wave5.TicksUntilNextWave = 20;
            ScheduleWave(wave5, spawner);
            var wave6 = CreateHeroParty(new List<Character>
            {

            }, entrance);
            wave6.TicksUntilNextWave = 20;
            ScheduleWave(wave6, spawner);
            var wave7 = CreateHeroParty(new List<Character>
            {

            }, entrance);
            wave7.TicksUntilNextWave = 20;
            ScheduleWave(wave7, spawner);
            return spawner;
        }
        public static WaveSpawner CreateWaveSchedule2Level2(List<TilePos> entrances)
        {
            WaveSpawner spawner = new WaveSpawner();
            spawner.ResetScheduler();

            TilePos entrance = entrances.Count > 0 ? entrances[0] : new TilePos(9, 1);

            // Wave 1: Double rogues immediately
            var wave1 = CreateHeroParty(new List<Character> {
                CharacterDatabase.rogue, CharacterDatabase.rogue
            }, entrance);
            wave1.TicksUntilNextWave = 20;
            ScheduleWave(wave1, spawner);

            // Wave 2: Double mages
            var wave2 = CreateHeroParty(new List<Character> {
                CharacterDatabase.apprenticeMage, CharacterDatabase.apprenticeMage,CharacterDatabase.knight
            }, entrance);
            wave2.TicksUntilNextWave = 20;
            ScheduleWave(wave2, spawner);

            // Wave 3: Peasant swarm 8
            var wave3 = CreateHeroParty(new List<Character> {
                CharacterDatabase.peasant, CharacterDatabase.peasant,
                CharacterDatabase.peasant, CharacterDatabase.peasant,CharacterDatabase.peasant, CharacterDatabase.peasant,
                CharacterDatabase.peasant, CharacterDatabase.peasant, CharacterDatabase.peasant
            }, entrance);
            wave3.TicksUntilNextWave = 20;
            ScheduleWave(wave3, spawner);

            // Wave 4: Mixed dangerous group
            var wave4 = CreateHeroParty(new List<Character> {
                CharacterDatabase.knight, CharacterDatabase.rogue, CharacterDatabase.Mage,CharacterDatabase.priestess

            }, entrance);
            wave4.TicksUntilNextWave = 20;
            ScheduleWave(wave4, spawner);

            // Wave 5: Final wave - largest group, never flees
            var wave5 = CreateHeroParty(new List<Character> {
                CharacterDatabase.rogue, CharacterDatabase.rogue,
                CharacterDatabase.apprenticeMage, CharacterDatabase.priestess,
                CharacterDatabase.knight, CharacterDatabase.warrior
            }, entrance, isFinal: true);
            wave5.TicksUntilNextWave = 0;
            ScheduleWave(wave5, spawner);

            return spawner;
        }
        public static WaveSpawner CreateWaveSchedule1Level3(List<TilePos> entrances)
        {
            WaveSpawner spawner = new WaveSpawner();
            spawner.ResetScheduler();

            TilePos entrance = entrances.Count > 0 ? entrances[0] : new TilePos(9, 1);
            var wave1 = CreateHeroParty(new List<Character>
            {
                CharacterDatabase.rogue
            }, entrance);
            wave1.TicksUntilNextWave = 20;
            ScheduleWave(wave1, spawner);
            var wave2 = CreateHeroParty(new List<Character>
            {
                CharacterDatabase.apprenticeMage, CharacterDatabase.priestess
            }, entrance);
            wave2.TicksUntilNextWave = 20;
            ScheduleWave(wave2, spawner);
            var wave3 = CreateHeroParty(new List<Character>
            {
                 CharacterDatabase.peasant, CharacterDatabase.peasant, CharacterDatabase.peasant, CharacterDatabase.peasant,CharacterDatabase.rogue
            }, entrance);
            wave3.TicksUntilNextWave = 20;
            ScheduleWave(wave3, spawner);
            var wave4 = CreateHeroParty(new List<Character>
            {

            }, entrance);
            wave4.TicksUntilNextWave = 20;
            ScheduleWave(wave4, spawner);
            var wave5 = CreateHeroParty(new List<Character>
            {

            }, entrance);
            wave5.TicksUntilNextWave = 20;
            ScheduleWave(wave5, spawner);
            var wave6 = CreateHeroParty(new List<Character>
            {

            }, entrance);
            wave6.TicksUntilNextWave = 20;
            ScheduleWave(wave6, spawner);
            var wave7 = CreateHeroParty(new List<Character>
            {

            }, entrance);
            wave7.TicksUntilNextWave = 20;
            ScheduleWave(wave7, spawner);
            return spawner;
        }
        public static WaveSpawner CreateWaveSchedule1Level4(List<TilePos> entrances)
        {
            WaveSpawner spawner = new WaveSpawner();
            spawner.ResetScheduler();

            TilePos entrance = entrances.Count > 0 ? entrances[0] : new TilePos(9, 1);

            // Wave 1 – Light scouting party
            var wave1 = CreateHeroParty(new List<Character>
    {
        CharacterDatabase.peasant,
        CharacterDatabase.peasant,
        CharacterDatabase.apprenticeMage
    }, entrance);
            wave1.TicksUntilNextWave = 20;
            ScheduleWave(wave1, spawner);

            // Wave 2 – Mobility & magic
            var wave2 = CreateHeroParty(new List<Character>
    {
        CharacterDatabase.rogue,
        CharacterDatabase.apprenticeMage,
        CharacterDatabase.peasant
    }, entrance);
            wave2.TicksUntilNextWave = 20;
            ScheduleWave(wave2, spawner);

            // Wave 3 – Sustained fight
            var wave3 = CreateHeroParty(new List<Character>
    {
        CharacterDatabase.warrior,
        CharacterDatabase.priestess,
        CharacterDatabase.peasant,
        CharacterDatabase.peasant
    }, entrance);
            wave3.TicksUntilNextWave = 20;
            ScheduleWave(wave3, spawner);

            // Wave 4 – Burst threat
            var wave4 = CreateHeroParty(new List<Character>
    {
        CharacterDatabase.rogue,
        CharacterDatabase.rogue,
        CharacterDatabase.apprenticeMage,
        CharacterDatabase.priestess
    }, entrance);
            wave4.TicksUntilNextWave = 20;
            ScheduleWave(wave4, spawner);

            // Wave 5 – Heavy frontline
            var wave5 = CreateHeroParty(new List<Character>
    {
        CharacterDatabase.knight,
        CharacterDatabase.warrior,
        CharacterDatabase.priestess
    }, entrance);
            wave5.TicksUntilNextWave = 20;
            ScheduleWave(wave5, spawner);

            // Wave 6 – Elite casters & support
            var wave6 = CreateHeroParty(new List<Character>
    {
        CharacterDatabase.Mage,
        CharacterDatabase.highPriestess,
        CharacterDatabase.rogue
    }, entrance);
            wave6.TicksUntilNextWave = 20;
            ScheduleWave(wave6, spawner);

            // Wave 7 – Mini-boss mixed elite group
            var wave7 = CreateHeroParty(new List<Character>
    {
        CharacterDatabase.knight,
        CharacterDatabase.warrior,
        CharacterDatabase.Mage,
        CharacterDatabase.highPriestess
    }, entrance);
            wave7.TicksUntilNextWave = 20;
            ScheduleWave(wave7, spawner);

            return spawner;
        }

        public static WaveSpawner CreateWaveSchedule2Level3(List<TilePos> entrances)
        {
            WaveSpawner spawner = new WaveSpawner();
            spawner.ResetScheduler();

            TilePos entrance = entrances.Count > 0 ? entrances[0] : new TilePos(9, 1);
            // Wave 1 : Single rogue
            var wave1 = CreateHeroParty(new List<Character> {
                CharacterDatabase.rogue
            }, entrance);
            wave1.TicksUntilNextWave = 20;
            ScheduleWave(wave1, spawner);

            // Wave 2: Warrior and Mage
            var wave2 = CreateHeroParty(new List<Character> {
                CharacterDatabase.priestess, CharacterDatabase.warrior
            }, entrance);
            wave2.TicksUntilNextWave = 20;
            ScheduleWave(wave2, spawner);

            // Wave 3: Peasant swarm 8
            var wave3 = CreateHeroParty(new List<Character> {
                CharacterDatabase.peasant, CharacterDatabase.peasant,
                CharacterDatabase.peasant, CharacterDatabase.peasant,CharacterDatabase.peasant, CharacterDatabase.peasant,
                CharacterDatabase.peasant, CharacterDatabase.peasant
            }, entrance);
            wave3.TicksUntilNextWave = 20;
            ScheduleWave(wave3, spawner);

            // Wave 4: Mixed dangerous group
            var wave4 = CreateHeroParty(new List<Character> {
                CharacterDatabase.knight, CharacterDatabase.rogue, CharacterDatabase.apprenticeMage,

            }, entrance);
            wave4.TicksUntilNextWave = 20;
            ScheduleWave(wave4, spawner);

            // Wave 5: Final wave - largest group, never flees
            var wave5 = CreateHeroParty(new List<Character> {
                CharacterDatabase.rogue, CharacterDatabase.rogue,
                CharacterDatabase.apprenticeMage, CharacterDatabase.priestess,
                CharacterDatabase.knight, CharacterDatabase.warrior
            }, entrance, isFinal: true);
            wave5.TicksUntilNextWave = 0;
            ScheduleWave(wave5, spawner);

            return spawner;
        }
        public static WaveSpawner CreateWaveSchedule2Level4(List<TilePos> entrances)
        {
            WaveSpawner spawner = new WaveSpawner();
            spawner.ResetScheduler();

            TilePos entrance = entrances.Count > 0 ? entrances[0] : new TilePos(9, 1);

            // Wave 1 : 3 Peasants
            var wave1 = CreateHeroParty(new List<Character> {
        CharacterDatabase.peasant, CharacterDatabase.peasant, CharacterDatabase.peasant
    }, entrance);
            wave1.TicksUntilNextWave = 40;
            ScheduleWave(wave1, spawner);

            // Wave 2: Warrior and Mage
            var wave2 = CreateHeroParty(new List<Character> {
        CharacterDatabase.priestess, CharacterDatabase.warrior
    }, entrance);
            wave2.TicksUntilNextWave = 40;
            ScheduleWave(wave2, spawner);

            // Wave 3: Peasant swarm 8
            var wave3 = CreateHeroParty(new List<Character> {
        CharacterDatabase.peasant, CharacterDatabase.peasant,
        CharacterDatabase.peasant, CharacterDatabase.peasant,
        CharacterDatabase.peasant, CharacterDatabase.peasant,
        CharacterDatabase.peasant, CharacterDatabase.peasant
    }, entrance);
            wave3.TicksUntilNextWave = 40;
            ScheduleWave(wave3, spawner);

            // Wave 4: Mixed dangerous group
            var wave4 = CreateHeroParty(new List<Character> {
        CharacterDatabase.knight, CharacterDatabase.rogue,
        CharacterDatabase.apprenticeMage, CharacterDatabase.highPriestess,
        CharacterDatabase.rogue
    }, entrance);
            wave4.TicksUntilNextWave = 40;
            ScheduleWave(wave4, spawner);

            var wave5 = CreateHeroParty(new List<Character> {
        CharacterDatabase.rogue, CharacterDatabase.apprenticeMage,
        CharacterDatabase.rogue, CharacterDatabase.apprenticeMage,
        CharacterDatabase.rogue, CharacterDatabase.apprenticeMage
    }, entrance);
            wave5.TicksUntilNextWave = 40;
            ScheduleWave(wave5, spawner);

            var wave6 = CreateHeroParty(new List<Character> {
        CharacterDatabase.highPriestess, CharacterDatabase.knight,
        CharacterDatabase.highPriestess, CharacterDatabase.knight
    }, entrance);
            wave6.TicksUntilNextWave = 40;
            ScheduleWave(wave6, spawner);

            var wave7 = CreateHeroParty(new List<Character> {
        CharacterDatabase.highPriestess,
        CharacterDatabase.warrior, CharacterDatabase.warrior,
        CharacterDatabase.warrior, CharacterDatabase.warrior
    }, entrance);
            wave7.TicksUntilNextWave = 40;
            ScheduleWave(wave7, spawner);

            var wave8 = CreateHeroParty(new List<Character> {
            CharacterDatabase.warrior, CharacterDatabase.warrior,
        CharacterDatabase.warrior, CharacterDatabase.warrior,CharacterDatabase.warrior, CharacterDatabase.warrior,
        CharacterDatabase.warrior, CharacterDatabase.warrior}, entrance);
            wave8.TicksUntilNextWave = 40;
            ScheduleWave(wave8, spawner);

            var wave9 = CreateHeroParty(new List<Character> {CharacterDatabase.highPriestess, CharacterDatabase.knight,
        CharacterDatabase.highPriestess, CharacterDatabase.knight,CharacterDatabase.rogue,CharacterDatabase.rogue,CharacterDatabase.rogue }, entrance);
            wave9.TicksUntilNextWave = 40;
            ScheduleWave(wave9, spawner);

            var wave10 = CreateHeroParty(new List<Character> { CharacterDatabase.peasant, CharacterDatabase.peasant, CharacterDatabase.peasant, CharacterDatabase.peasant, CharacterDatabase.peasant,
            CharacterDatabase.peasant,CharacterDatabase.peasant}, entrance);
            wave10.TicksUntilNextWave = 10;
            ScheduleWave(wave10, spawner);
            ScheduleWave(wave10, spawner);
            ScheduleWave(wave10, spawner);
            ScheduleWave(wave10, spawner);


            var wave11 = CreateHeroParty(new List<Character> { CharacterDatabase.isekai}, entrance);
            wave11.TicksUntilNextWave = 40;
            ScheduleWave(wave11, spawner);

            var wave12 = CreateHeroParty(new List<Character> {CharacterDatabase.isekai,CharacterDatabase.highPriestess }, entrance);
            wave12.TicksUntilNextWave = 40;
            ScheduleWave(wave12, spawner);

            var wave13 = CreateHeroParty(new List<Character> { CharacterDatabase.isekai, CharacterDatabase.HolyPriestess}, entrance);
            wave13.TicksUntilNextWave = 40;
            ScheduleWave(wave13, spawner);

            var wave14 = CreateHeroParty(new List<Character> {CharacterDatabase.knight, CharacterDatabase.knight, CharacterDatabase.knight, CharacterDatabase.knight, CharacterDatabase.knight, CharacterDatabase.HolyPriestess}, entrance);
            wave14.TicksUntilNextWave = 40;
            ScheduleWave(wave14, spawner);

            // Wave 15: Final wave - largest group, never flees
            var wave15 = CreateHeroParty(new List<Character> {
        CharacterDatabase.isekai, CharacterDatabase.HolyPriestess,
        CharacterDatabase.Mage, CharacterDatabase.rogue,
        CharacterDatabase.highPriestess
    }, entrance, isFinal: true);
            wave15.TicksUntilNextWave = 0;
            ScheduleWave(wave15, spawner);

            return spawner;
        }


        public static WaveSpawner CreateWaveScheduleDualEntrance(List<TilePos> entrances)
        {
            WaveSpawner spawner = new WaveSpawner();
            spawner.ResetScheduler();

            TilePos entranceA = entrances.Count > 0 ? entrances[0] : new TilePos(9, 1);
            TilePos entranceB = entrances.Count > 1 ? entrances[1] : new TilePos(9, 18);


            var wave1A = CreateHeroParty(new List<Character>
            {
                CharacterDatabase.rogue
            }, entranceA);

                    var wave1B = CreateHeroParty(new List<Character>
            {
                CharacterDatabase.rogue
            }, entranceB);

            wave1A.TicksUntilNextWave = 0;
            wave1B.TicksUntilNextWave = 30;

            ScheduleWave(wave1A, spawner);
            ScheduleWave(wave1B, spawner);

            var wave2A = CreateHeroParty(new List<Character>
            {
                CharacterDatabase.rogue,
                CharacterDatabase.rogue
            }, entranceA);

                    var wave2B = CreateHeroParty(new List<Character>
            {
                CharacterDatabase.rogue,
                CharacterDatabase.rogue
            }, entranceB);

            wave2A.TicksUntilNextWave = 0;
            wave2B.TicksUntilNextWave = 35;

            ScheduleWave(wave2A, spawner);
            ScheduleWave(wave2B, spawner);

            var wave3A = CreateHeroParty(new List<Character>
            {
                CharacterDatabase.rogue,
                CharacterDatabase.rogue,
                CharacterDatabase.rogue
            }, entranceA);

            var wave3B = CreateHeroParty(new List<Character>
            {
                CharacterDatabase.rogue,
                CharacterDatabase.rogue,
                CharacterDatabase.rogue
            }, entranceB);

            wave3A.TicksUntilNextWave = 0;
            wave3B.TicksUntilNextWave = 40;

            ScheduleWave(wave3A, spawner);
            ScheduleWave(wave3B, spawner);
            var finalA = CreateHeroParty(new List<Character>
            {
                CharacterDatabase.rogue,
                CharacterDatabase.rogue,
                CharacterDatabase.rogue,
                CharacterDatabase.rogue
            }, entranceA, isFinal: true);

            var finalB = CreateHeroParty(new List<Character>
            {
                CharacterDatabase.rogue,
                CharacterDatabase.rogue,
                CharacterDatabase.rogue,
                CharacterDatabase.rogue
            }, entranceB, isFinal: true);

            finalA.TicksUntilNextWave = 0;
            finalB.TicksUntilNextWave = 0;

            ScheduleWave(finalA, spawner);
            ScheduleWave(finalB, spawner);

            return spawner;
        }



        public static Level LoadLevelOne(Board board)
        {
            Level level = PremadeLevels.LevelOne;

            string[] levelLayout =
            {
                "####################",
                "####################",
                "###              ###",
                "###      D       ###",
                "####             ###",
                "##C#####  ##########",
                "## #####  ##########",
                "##               ###",
                "###              ###",
                "###         ########",
                "########  ####C  ###",
                "########  ###### ###",
                "####          ## ###",
                "###           C# ###",
                "###          ### ###",
                "###              ###",
                "####          ######",
                "########  ##########",
                "#######X  E#########",
                "####################",
            };

            LoadDungeon(board, level, levelLayout);
            return level;
        }
        public static Level LoadLevelTutorial(Board board)
        {
            Level level = PremadeLevels.LevelTutorial;

            string[] levelLayout =
            {
                "####################",
                "####################",
                "####################",
                "####################",
                "####################",
                "####################",
                "####################",
                "#### C######D ######",
                "####  ######  ######",
                "####  ######  ######",
                "####  ######  ######",
                "####  ######  ######",
                "####          ######",
                "########  ##########",
                "########  ##########",
                "########  ##########",
                "########  ##########",
                "########  ##########",
                "#######X  E#########",
                "####################",
            };

            LoadDungeon(board, level, levelLayout);
            return level;
        }
        public static Level LoadLevelTwo(Board board)
        {
            Level level = PremadeLevels.LevelTwo;

            string[] levelLayout =
            {
                "####################",
                "####################",
                "##############C#####",
                "############## #####",
                "############## #####",
                "###     ###      ###",
                "###C###     ###  ###",
                "#######    ####  ###",
                "#X         #       #",
                "#E         #       #",
                "############       #",
                "###############  ###",
                "###############  ###",
                "#######  D  ###  ###",
                "#######     ###  ###",
                "#######          ###",
                "###########      ###",
                "####################",
                "####################",
                "####################",
            };

            LoadDungeon(board, level, levelLayout);
            return level;
        }
        public static Level LoadLevelThree(Board board)
        {
            Level level = PremadeLevels.LevelThree;

            string[] levelLayout =
            {
                "####################",
                "#E                X#",
                "#                  #",
                "#########  #########",
                "#########  #########",
                "#########  #########",
                "#######      #######",
                "#######  C   #######",
                "#######      #######",
                "#########  #########",
                "#########  #########",
                "######        ######",
                "######   C    ######",
                "######        ######",
                "#########  #########",
                "#########  #########",
                "######        ######",
                "######   D    ######",
                "######        ######",
                "####################",
            };

            LoadDungeon(board, level, levelLayout);
            return level;
        }
        public static Level LoadLevelFour(Board board)
        {
            Level level = PremadeLevels.LevelFour;

            string[] levelLayout =
            {
                "####################",
                "###############  C #",
                "##                 #",
                "##D         ##    ##",
                "##          ##    ##",
                "##############    ##",
                "##############    ##",
                "E      #            ",
                "X      #            ",
                "##     #         ###",
                "##     ####      ###",
                "##     ####      ###",
                "##     ####      ###",
                "##             #####",
                "##            C#####",
                "#### #      #  #####",
                "####           #####",
                "#### #      #  #####",
                "####           #####",
                "####################",
            };

            LoadDungeon(board, level, levelLayout);
            return level;
        }
    }

}