using System.Collections.Generic;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Grid;

namespace Undermarch.Simulation.Levels
{
    public static class LevelLoader
    {
        /// <summary>
        /// Loads the main dungeon level.
        /// Layout: Entrance Room -> Corridor -> Treasure Room -> Boss Room
        /// Legend:
        /// # = Wall
        /// E = Entrance (hero spawn point)
        /// C = Chest
        /// D = Dungeon Master
        /// X = Exit door (heroes can escape here)
        /// </summary>
        public static void LoadDungeon(Board board, out List<TilePos> entrances, out List<TilePos> chestPositions)
        {
            entrances = new List<TilePos>();
            chestPositions = new List<TilePos>();

            string[] levelLayout = new string[]
            {
                "####################",
                "####################",
                "###              ###",  // Boss Room
                "###      D       ###",
                "###              ###",
                "########  ##########",
                "########  ##########",  // Corridor to boss
                "###              ###",
                "### C        C   ###",  // Treasure Room (2 chests)
                "###              ###",
                "########  ##########",
                "########  ##########",  // Main corridor
                "####          ######",
                "###            #####",  // Entrance Room
                "###  C      C   ####",  // 2 chests near entrance
                "###            #####",
                "####          ######",
                "########  ##########",
                "######## E ##########",  // Entrance - heroes spawn here
                "####################",
            };

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
                            entrances.Add(pos);
                            break;
                        case 'D':
                            var dm = CharacterDatabase.dungeonMaster.Clone();
                            board.AddEntity(pos, dm);
                            break;
                        case 'C':
                            var chest = new Chest(pos, goldAmount: 30);
                            board.AddInteractable(pos, chest);
                            chestPositions.Add(pos);
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
        /// Creates wave schedule for 5 minutes of gameplay.
        /// Waves spawn progressively harder hero parties.
        /// Assumes 2 TPS = 600 ticks for 5 minutes.
        /// </summary>
        public static WaveSpawner CreateWaveSchedule(List<TilePos> entrances)
        {
            WaveSpawner spawner = new WaveSpawner();
            TilePos entrance = entrances.Count > 0 ? entrances[0] : new TilePos(9, 1);

            // Wave 1: 1 peasant (spawn at tick 0 - start immediately)
            spawner.ScheduleWave(new HeroParty(
                new List<Character> { CharacterDatabase.peasant.Clone() },
                spawnTick: 0,
                spawnPos: entrance
            ));

            // Wave 2: 2 peasants (30 seconds = 60 ticks)
            spawner.ScheduleWave(new HeroParty(
                new List<Character> {
                    CharacterDatabase.peasant.Clone(),
                    CharacterDatabase.peasant.Clone()
                },
                spawnTick: 60,
                spawnPos: entrance
            ));

            // Wave 3: 1 rogue (60 seconds = 120 ticks)
            spawner.ScheduleWave(new HeroParty(
                new List<Character> { CharacterDatabase.rogue.Clone() },
                spawnTick: 120,
                spawnPos: entrance
            ));

            // Wave 4: 1 mage + 1 peasant (90 seconds = 180 ticks)
            spawner.ScheduleWave(new HeroParty(
                new List<Character> {
                    CharacterDatabase.apprenticeMage.Clone(),
                    CharacterDatabase.peasant.Clone()
                },
                spawnTick: 180,
                spawnPos: entrance
            ));

            // Wave 5: 2 rogues (2 minutes = 240 ticks)
            spawner.ScheduleWave(new HeroParty(
                new List<Character> {
                    CharacterDatabase.rogue.Clone(),
                    CharacterDatabase.rogue.Clone()
                },
                spawnTick: 240,
                spawnPos: entrance
            ));

            // Wave 6: Mixed party (2.5 minutes = 300 ticks)
            spawner.ScheduleWave(new HeroParty(
                new List<Character> {
                    CharacterDatabase.peasant.Clone(),
                    CharacterDatabase.rogue.Clone(),
                    CharacterDatabase.apprenticeMage.Clone()
                },
                spawnTick: 300,
                spawnPos: entrance
            ));

            // Wave 7: 3 peasants (3 minutes = 360 ticks)
            spawner.ScheduleWave(new HeroParty(
                new List<Character> {
                    CharacterDatabase.peasant.Clone(),
                    CharacterDatabase.peasant.Clone(),
                    CharacterDatabase.peasant.Clone()
                },
                spawnTick: 360,
                spawnPos: entrance
            ));

            // Wave 8: 2 mages (3.5 minutes = 420 ticks)
            spawner.ScheduleWave(new HeroParty(
                new List<Character> {
                    CharacterDatabase.apprenticeMage.Clone(),
                    CharacterDatabase.apprenticeMage.Clone()
                },
                spawnTick: 420,
                spawnPos: entrance
            ));

            // Wave 9: FINAL WAVE - Big party (4 minutes = 480 ticks)
            var finalWave = new HeroParty(
                new List<Character> {
                    CharacterDatabase.rogue.Clone(),
                    CharacterDatabase.rogue.Clone(),
                    CharacterDatabase.apprenticeMage.Clone(),
                    CharacterDatabase.peasant.Clone()
                },
                spawnTick: 480,
                spawnPos: entrance,
                isFinal: true
            );

            // Make final wave heroes not flee
            foreach (var hero in finalWave.Heroes)
            {
                if (hero is Entities.Characters.Heroes.Hero h)
                {
                    h.FleeThreshold = 999999; // Won't flee
                }
            }

            spawner.ScheduleWave(finalWave);

            return spawner;
        }
    }
}
