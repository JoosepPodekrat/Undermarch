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
        public static WaveSpawner CreateWaveSchedule(List<TilePos> entrances)
        {
            WaveSpawner spawner = new WaveSpawner();
            spawner.ResetScheduler(); // Start at tick 0

            TilePos entrance = entrances.Count > 0 ? entrances[0] : new TilePos(9, 1);

            // Wave 1: Single peasant (tutorial wave)
            var wave1 = CreateHeroParty(new List<Character> { CharacterDatabase.peasant }, entrance);
            wave1.TicksUntilNextWave = 30; // ~15 seconds
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
            wave7.TicksUntilNextWave = 40;
            ScheduleWave(wave7, spawner);

            // Wave 8: Double mages
            var wave8 = CreateHeroParty(new List<Character> {
                CharacterDatabase.apprenticeMage, CharacterDatabase.apprenticeMage
            }, entrance);
            wave8.TicksUntilNextWave = 45;
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
        public static WaveSpawner CreateWaveSchedule2(List<TilePos> entrances)
        {
            WaveSpawner spawner = new WaveSpawner();
            spawner.ResetScheduler();

            TilePos entrance = entrances.Count > 0 ? entrances[0] : new TilePos(9, 1);

            // Wave 1: Double rogues immediately
            var wave1 = CreateHeroParty(new List<Character> {
                CharacterDatabase.rogue, CharacterDatabase.rogue
            }, entrance);
            wave1.TicksUntilNextWave = 30;
            ScheduleWave(wave1, spawner);

            // Wave 2: Double mages
            var wave2 = CreateHeroParty(new List<Character> {
                CharacterDatabase.apprenticeMage, CharacterDatabase.apprenticeMage
            }, entrance);
            wave2.TicksUntilNextWave = 35;
            ScheduleWave(wave2, spawner);

            // Wave 3: Peasant swarm (4)
            var wave3 = CreateHeroParty(new List<Character> {
                CharacterDatabase.peasant, CharacterDatabase.peasant,
                CharacterDatabase.peasant, CharacterDatabase.peasant
            }, entrance);
            wave3.TicksUntilNextWave = 35;
            ScheduleWave(wave3, spawner);

            // Wave 4: Mixed dangerous group
            var wave4 = CreateHeroParty(new List<Character> {
                CharacterDatabase.rogue, CharacterDatabase.apprenticeMage,
                CharacterDatabase.rogue
            }, entrance);
            wave4.TicksUntilNextWave = 40;
            ScheduleWave(wave4, spawner);

            // Wave 5: Final wave - largest group, never flees
            var wave5 = CreateHeroParty(new List<Character> {
                CharacterDatabase.rogue, CharacterDatabase.rogue,
                CharacterDatabase.apprenticeMage, CharacterDatabase.apprenticeMage,
                CharacterDatabase.peasant, CharacterDatabase.peasant
            }, entrance, isFinal: true);
            wave5.TicksUntilNextWave = 0;
            ScheduleWave(wave5, spawner);

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
            Level level = PremadeLevels.LevelOne;

            string[] levelLayout =
            {
                "####################",
                "####################",
                "####################",
                "####################",
                "####################",
                "####################",
                "####################",
                "#####C######D#######",
                "##### ###### #######",
                "##### ###### #######",
                "##### ###### #######",
                "##### ###### #######",
                "#####        #######",
                "######## ###########",
                "######## ###########",
                "######## ###########",
                "######## ###########",
                "######## ###########",
                "#######X E##########",
                "####################",
            };

            LoadDungeon(board, level, levelLayout);
            return level;
        }
        public static Level LoadLevelTwo(Board board)
        {
            Level level = PremadeLevels.LevelOne;

            string[] levelLayout =
            {
                "####################",
                "####################",
                "##############C#####",
                "############## #####",
                "############## #####",
                "###     ###      ###",
                "###C###  #  #### ###",
                "#######  # ##### ###",
                "#X       # #       #",
                "#E         #       #",
                "############       #",
                "################ ###",
                "################ ###",
                "#######  D  #### ###",
                "#######     #### ###",
                "#######          ###",
                "####################",
                "####################",
                "####################",
                "####################",
            };

            LoadDungeon(board, level, levelLayout);
            return level;
        }
        public static Level LoadLevelThree(Board board)
        {
            Level level = PremadeLevels.LevelOne;

            string[] levelLayout =
            {
                "####################",
                "#E                X#",
                "######### ##########",
                "######### ##########",
                "######### ##########",
                "######### ##########",
                "#######      #######",
                "#######      #######",
                "#######      #######",
                "#########C##########",
                "######### ##########",
                "######        ######",
                "######        ######",
                "######        ######",
                "#########C##########",
                "######### ##########",
                "######        ######",
                "######   D    ######",
                "######        ######",
                "####################",
            };

            LoadDungeon(board, level, levelLayout);
            return level;
        }
    }

}
