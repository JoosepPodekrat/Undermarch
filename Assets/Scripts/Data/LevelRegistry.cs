﻿using System.Collections.Generic;
using UnityEngine;

namespace Undermarch.Data
{
    /// <summary>
    /// ScriptableObject holding an ordered list of all available levels.
    /// The UI iterates this list to generate level buttons.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelRegistry", menuName = "Undermarch/Level Registry", order = 2)]
    public class LevelRegistry : ScriptableObject
    {
        [Tooltip("Ordered list of all levels. Index determines unlock progression.")]
        public List<LevelDataSO> levels = new List<LevelDataSO>();

        /// <summary>
        /// Gets the total number of levels.
        /// </summary>
        public int LevelCount => levels.Count;

        /// <summary>
        /// Gets a level by index.
        /// </summary>
        public LevelDataSO GetLevel(int index)
        {
            if (index >= 0 && index < levels.Count)
            {
                return levels[index];
            }
            return null;
        }
    }
}
