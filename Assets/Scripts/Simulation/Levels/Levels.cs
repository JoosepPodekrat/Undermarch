using Undermarch.Simulation.Levels;

namespace Undermarch.Simulation.Levels
{
    public static class PremadeLevels
    {
        public static Level LevelOne = new Level
        {
            Width = 20,
            Height = 20,
            LoadLayout = LevelLoader.LoadDungeon,
            CreateWaves = LevelLoader.CreateWaveSchedule
        };

        public static Level LevelTwo = new Level
        {
            Width = 30,
            Height = 20,
            LoadLayout = LevelLoader.LoadDungeon,
            CreateWaves = LevelLoader.CreateWaveSchedule2
        };
    }

}
