﻿using UnityEngine;

namespace Undermarch.Data
{
    /// <summary>
    /// Static class for managing level progression using PlayerPrefs.
    /// Tracks which levels are unlocked based on completion.
    /// </summary>
    public static class LevelProgressManager
    {
        private const string HighestCompletedKey = "HighestCompletedLevel";

        /// <summary>
        /// Gets the index of the highest completed level.
        /// Returns -1 if no levels have been completed.
        /// </summary>
        public static int HighestCompletedLevel
        {
            get => PlayerPrefs.GetInt(HighestCompletedKey, -1);
            private set
            {
                PlayerPrefs.SetInt(HighestCompletedKey, value);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Checks if a level is unlocked.
        /// Tutorial (0) and Level 1 (1) are always unlocked.
        /// Other levels unlock when the previous level is completed.
        /// </summary>
        public static bool IsLevelUnlocked(int levelIndex)
        {
            // Tutorial and Level 1 are always unlocked
            if (levelIndex <= 1)
            {
                return true;
            }

            // Other levels require the previous level to be completed
            return levelIndex <= HighestCompletedLevel + 1;
        }

        /// <summary>
        /// Marks a level as completed. Updates the highest completed level if necessary.
        /// </summary>
        public static void MarkLevelCompleted(int levelIndex)
        {
            if (levelIndex > HighestCompletedLevel)
            {
                HighestCompletedLevel = levelIndex;
                Debug.Log($"LevelProgressManager: Level {levelIndex} completed! New highest: {HighestCompletedLevel}");
            }
        }

        /// <summary>
        /// Gets the highest level that is currently unlocked.
        /// </summary>
        public static int GetHighestUnlockedLevel()
        {
            // At minimum, levels 0 and 1 are unlocked
            return Mathf.Max(1, HighestCompletedLevel + 1);
        }

        /// <summary>
        /// Resets all progress. Useful for testing or "New Game" functionality.
        /// </summary>
        public static void ResetProgress()
        {
            PlayerPrefs.DeleteKey(HighestCompletedKey);
            PlayerPrefs.Save();
            Debug.Log("LevelProgressManager: Progress reset.");
        }

        /// <summary>
        /// Debug: Unlocks all levels.
        /// </summary>
        public static void UnlockAllLevels(int maxLevelIndex)
        {
            HighestCompletedLevel = maxLevelIndex;
            Debug.Log($"LevelProgressManager: All levels unlocked up to index {maxLevelIndex}.");
        }
    }
}

