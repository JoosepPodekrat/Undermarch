using Undermarch.Simulation.Levels;

namespace Undermarch.Simulation.Levels
{
    public static class PremadeLevels
    {
        public static Level LevelTutorial = new Level
        {
            Width = 20,
            Height = 20,
            LoadLayout = LevelLoader.LoadDungeon,
            CreateWaves = level => LevelLoader.CreateWaveScheduleTutorial(level.Entrances)
        };

        public static Level LevelOne = new Level
        {
            Width = 20,
            Height = 20,
            LoadLayout = LevelLoader.LoadDungeon,
            CreateWaves = level => LevelLoader.CreateWaveSchedule1Level1(level.Entrances),
            CreateSecondWaves = level =>
                LevelLoader.CreateWaveSchedule2Level1(level.Entrances)
        };

        public static Level LevelTwo = new Level
        {
            Width = 20,
            Height = 20,
            LoadLayout = LevelLoader.LoadDungeon,
            CreateWaves = level => LevelLoader.CreateWaveSchedule1Level2(level.Entrances),
            CreateSecondWaves = level =>
                LevelLoader.CreateWaveSchedule2Level2(level.Entrances)
        };

        public static Level LevelThree = new Level
        {
            Width = 20,
            Height = 20,
            LoadLayout = LevelLoader.LoadDungeon,
            CreateWaves = level => LevelLoader.CreateWaveSchedule1Level3(level.Entrances),
            CreateSecondWaves = level =>
                LevelLoader.CreateWaveSchedule2Level3(level.Entrances)
        };

        public static Level LevelFour = new Level
        {
            Width = 20,
            Height = 20,
            LoadLayout = LevelLoader.LoadDungeon,
            CreateWaves = level => LevelLoader.CreateWaveSchedule1Level4(level.Entrances),
            CreateSecondWaves = level =>
                LevelLoader.CreateWaveSchedule2Level4(level.Entrances)
        };
    }

       
}
