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

            // Spawn a player character (warrior)
            var player = CharacterDatabase.warrior.Clone();
            board.AddEntity(new TilePos(3, 10), player);

            // Spawn an enemy (goblin)
            var enemy = CharacterDatabase.goblin.Clone();
            board.AddEntity(new TilePos(17, 10), enemy);
        }
    }
}
