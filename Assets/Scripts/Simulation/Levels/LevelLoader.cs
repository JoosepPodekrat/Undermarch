using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Entities.Traps;
using Undermarch.Simulation.Grid;

namespace Undermarch.Simulation.Levels
{
    public static class LevelLoader
    {
        public static void LoadLevel1(Board board)
        {
            // # = Wall, . = Floor, H = Hero, M = Monster, D = Dungeon Master, T = Trap
            string[] levelLayout = new string[]
            {
                "####################",
                "######      ########",
                "#####   D    #######",
                "######      ########",
                "#########.##########",
                "#########.##########",
                "#######.....########",
                "###.....###.....####",
                "###.....###.....####",
                "###.....###.....####",
                "###.....###.M...####",
                "###.....###.....####",
                "###.....###.....####",
                "#######.....########",
                "#########.##########",
                "#########T##########",
                "######      ########",
                "#####   H    #######",
                "######      ########",
                "####################",
            };

            for (int y = 0; y < levelLayout.Length; y++)
            {
                // The map is defined top-to-bottom, board is bottom-to-top.
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
                        case 'H':
                            var hero = CharacterDatabase.peasant.Clone();
                            board.AddEntity(pos, hero);
                            break;
                        case 'M':
                            var monster = CharacterDatabase.slimeMonster.Clone();
                            board.AddEntity(pos, monster);
                            break;
                        case 'D':
                            var dm = CharacterDatabase.dungeonMaster.Clone();
                            board.AddEntity(pos, dm);
                            break;
                        case 'T':
                            board.AddInteractable(pos, new SpikeTrap());
                            break;
                        // '.', 'E', and ' ' are empty spaces, so we do nothing.
                    }
                }
            }

        }
    }
}
