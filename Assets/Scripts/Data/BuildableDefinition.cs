using UnityEngine;

namespace Undermarch.Data
{
    /// <summary>
    /// ScriptableObject that defines a single buildable monster or trap.
    /// Create one of these for each buildable type and assign to LevelDataSO.
    /// </summary>
    [CreateAssetMenu(fileName = "NewBuildable", menuName = "Undermarch/Buildable Definition", order = 2)]
    public class BuildableDefinition : ScriptableObject
    {
        [Header("Display")]
        [Tooltip("Name shown in the UI")]
        public string displayName = "Buildable";
        
        [Tooltip("Optional icon sprite for the buildable")]
        public Sprite icon;
        
        [Tooltip("Color tint for the button (if no icon is used)")]
        public Color buttonColor = new Color(0.1f, 0.1f, 0.1f);

        [Header("Placement")]
        [Tooltip("Which placement type this triggers in PlacementController")]
        public PlacementType placementType;
        
        [Tooltip("Gold cost to place this buildable")]
        public int goldCost = 50;

        [Header("Availability")]
        [Tooltip("If true, only available during BuildingPhase2 (grayed out with lock in Phase 1)")]
        public bool requiresPhase2 = false;
        
        [Tooltip("Optional description shown on hover or selection")]
        [TextArea(2, 4)]
        public string description = "";
        [Tooltip("Additional resource costs (e.g. Steel, Wood)")]
        public ResourceCost[] extraCosts;
    }

    [System.Serializable]
    public struct ResourceCost
    {
        public Undermarch.Simulation.Interfaces.ResourceType type;
        public int amount;
    }
}
