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
            Width = 20,
            Height = 20,
            LoadLayout = LevelLoader.LoadDungeon,
            CreateWaves = LevelLoader.CreateWaveSchedule2
        };
        public static Level LevelThree = new Level
        {
            Width = 20,
            Height = 20,
            LoadLayout = LevelLoader.LoadDungeon,
            CreateWaves = LevelLoader.CreateWaveSchedule3
        };
        public static Level LevelFour = new Level
        {
            Width = 20,
            Height = 20,
            LoadLayout = LevelLoader.LoadDungeon,
            CreateWaves = LevelLoader.CreateWaveSchedule4
        };

        public static Level LevelFive = new Level
        {
            Width = 20,
            Height = 20,
            LoadLayout = LevelLoader.LoadDungeon,
            CreateWaves = LevelLoader.CreateWaveSchedule5
        };

        public static Level LevelSix = new Level
        {
            Width = 20,
            Height = 20,
            LoadLayout = LevelLoader.LoadDungeon,
            CreateWaves = LevelLoader.CreateWaveSchedule6
        };

        public static Level LevelSeven = new Level
        {
            Width = 20,
            Height = 20,
            LoadLayout = LevelLoader.LoadDungeon,
            CreateWaves = LevelLoader.CreateWaveSchedule7
        };

    }

}
