using System.Collections.Generic;
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
        public static void LoadDungeon(Board board, out List<TilePos> entrances, out List<TilePos> chestPositions)
        {
            entrances = new List<TilePos>();
            chestPositions = new List<TilePos>();

            string[] levelLayout = new string[]
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
                            CharacterEvents.RaiseSpawn(dm); // spawn event after placement
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
        /// Creates a wave schedule with spawn events triggered after placement.
        /// </summary>
        public static WaveSpawner CreateWaveSchedule(List<TilePos> entrances)
{
    WaveSpawner spawner = new WaveSpawner();
    TilePos entrance = entrances.Count > 0 ? entrances[0] : new TilePos(9, 1);

    void ScheduleHeroWave(List<Character> heroes, int spawnTick)
    {
        var clonedHeroes = new List<Character>();
        foreach (var hero in heroes)
        {
            clonedHeroes.Add(hero.Clone());
        }
        var party = new HeroParty(clonedHeroes, spawnTick, entrance);
        spawner.ScheduleWave(party);
    }

    // 9 waves, spaced ~65 ticks apart (~32.5 seconds per wave)
    int tickInterval = 65;

    ScheduleHeroWave(new List<Character> { CharacterDatabase.peasant }, 0);
ScheduleHeroWave(new List<Character> { CharacterDatabase.peasant, CharacterDatabase.peasant }, 120); // was 60
ScheduleHeroWave(new List<Character> { CharacterDatabase.rogue }, 240); // was 120
ScheduleHeroWave(new List<Character> { CharacterDatabase.apprenticeMage, CharacterDatabase.peasant }, 360); // was 180
ScheduleHeroWave(new List<Character> { CharacterDatabase.rogue, CharacterDatabase.rogue }, 480); // was 240
ScheduleHeroWave(new List<Character> { CharacterDatabase.peasant, CharacterDatabase.rogue, CharacterDatabase.apprenticeMage }, 600); // was 300
ScheduleHeroWave(new List<Character> { CharacterDatabase.peasant, CharacterDatabase.peasant, CharacterDatabase.peasant }, 720); // was 360
ScheduleHeroWave(new List<Character> { CharacterDatabase.apprenticeMage, CharacterDatabase.apprenticeMage }, 840); // was 420

var finalHeroes = new List<Character> {
    CharacterDatabase.rogue.Clone(),
    CharacterDatabase.rogue.Clone(),
    CharacterDatabase.apprenticeMage.Clone(),
    CharacterDatabase.peasant.Clone()
};
foreach (var hero in finalHeroes)
{
    if (hero is Entities.Characters.Heroes.Hero h)
        h.FleeThreshold = 999999;
}
ScheduleHeroWave(finalHeroes, 960); // was 480

    return spawner;
}

    }
}
