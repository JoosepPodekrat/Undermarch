﻿using UnityEngine;

namespace Undermarch.Data
{
    /// <summary>
    /// ScriptableObject defining a single level's metadata and loader reference.
    /// </summary>
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Undermarch/Level Data", order = 1)]
    public class LevelDataSO : ScriptableObject
    {
        [Tooltip("Display name shown in the level selector UI")]
        public string displayName = "Level";

        [Tooltip("The order/index of this level (0 = Tutorial, 1 = Level 1, etc.)")]
        public int levelIndex;

        [Tooltip("Which level loader method to use")]
        public LevelLoaderType loaderType = LevelLoaderType.LevelOne;

        /// <summary>
        /// Loads this level onto the given board.
        /// </summary>
        public Level Load(Simulation.Grid.Board board)
        {
            return loaderType switch
            {
                LevelLoaderType.Tutorial => Simulation.Levels.LevelLoader.LoadLevelTutorial(board),
                LevelLoaderType.LevelOne => Simulation.Levels.LevelLoader.LoadLevelOne(board),
                LevelLoaderType.LevelTwo => Simulation.Levels.LevelLoader.LoadLevelTwo(board),
                LevelLoaderType.LevelThree => Simulation.Levels.LevelLoader.LoadLevelThree(board),
                _ => Simulation.Levels.LevelLoader.LoadLevelOne(board)
            };
        }
    }

    public enum LevelLoaderType
    {
        Tutorial,
        LevelOne,
        LevelTwo,
        LevelThree
    }
}

