using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Grid;

namespace Undermarch.Simulation.Levels
{
    public static class LevelLoader
    {
        public static void LoadLevel1(Board board)
        {
            // Create a simple room
            // Top and bottom walls
            for (int x = 0; x < board.Width; x++)
            {
                board.AddWall(new TilePos(x, 0));
                board.AddWall(new TilePos(x, board.Height - 1));
            }

            // Left and right walls
            for (int y = 1; y < board.Height - 1; y++)
            {
                board.AddWall(new TilePos(0, y));
                board.AddWall(new TilePos(board.Width - 1, y));
            }

            // Add some inner obstacles
            board.AddWall(new TilePos(5, 5));
            board.AddWall(new TilePos(5, 6));
            board.AddWall(new TilePos(5, 7));
            board.AddWall(new TilePos(5, 8));

            board.AddWall(new TilePos(15, 5));
            board.AddWall(new TilePos(15, 6));
            board.AddWall(new TilePos(15, 7));
            board.AddWall(new TilePos(15, 8));

            // Spawn a hero (warrior)
            var hero = CharacterDatabase.warrior.Clone();
            board.AddEntity(new TilePos(3, 10), hero);

            // Spawn a monster (goblin)
            var monster = CharacterDatabase.goblin.Clone();
            board.AddEntity(new TilePos(17, 10), monster);

            // Spawn the Dungeon Master
            var dm = CharacterDatabase.dungeonMaster.Clone();
            board.AddEntity(new TilePos(10, 18), dm);
        }
    }
}
