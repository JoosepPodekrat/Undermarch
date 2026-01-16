using TMPro;
using Undermarch.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Undermarch.Presentation.UI
{
    /// <summary>
    /// Manages the level selector UI. Dynamically generates level buttons
    /// based on the LevelRegistry and handles level selection.
    /// </summary>
    public class LevelSelectorUI : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The LevelRegistry ScriptableObject containing all levels")]
        public LevelRegistry levelRegistry;

        [Tooltip("Container where level buttons will be spawned")]
        public Transform buttonContainer;

        [Tooltip("Prefab for level buttons (should have Button + TextMeshProUGUI)")]
        public GameObject levelButtonPrefab;

        [Tooltip("The Back button to return to main menu")]
        public Button backButton;

        [Header("Settings")]
        [Tooltip("Text to display for locked levels")]
        public string lockedText = "Locked";

        /// <summary>
        /// Static property to store the selected level index.
        /// Read by GameManager to load the correct level.
        /// -1 means no level selected (show main menu).
        /// </summary>
        public static int SelectedLevelIndex { get; private set; } = -1;

        /// <summary>
        /// Resets the selected level to -1 (no selection).
        /// </summary>
        public static void ResetSelection()
        {
            SelectedLevelIndex = -1;
        }

        private void Start()
        {
            GenerateLevelButtons();

            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackButtonClicked);
            }
        }

        /// <summary>
        /// Generates level buttons dynamically based on the registry.
        /// </summary>
        public void GenerateLevelButtons()
        {
            if (levelRegistry == null)
            {
                Debug.LogError("LevelSelectorUI: LevelRegistry is not assigned!");
                return;
            }

            if (buttonContainer == null)
            {
                Debug.LogError("LevelSelectorUI: ButtonContainer is not assigned!");
                return;
            }

            if (levelButtonPrefab == null)
            {
                Debug.LogError("LevelSelectorUI: LevelButtonPrefab is not assigned!");
                return;
            }

            // Clear existing buttons (except the back button if it's in the container)
            foreach (Transform child in buttonContainer)
            {
                if (child.GetComponent<Button>() != backButton)
                {
                    Destroy(child.gameObject);
                }
            }

            // Generate a button for each level
            for (int i = 0; i < levelRegistry.LevelCount; i++)
            {
                LevelDataSO levelData = levelRegistry.GetLevel(i);
                if (levelData == null) continue;

                CreateLevelButton(levelData, i);
            }
        }

        private void CreateLevelButton(LevelDataSO levelData, int index)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, buttonContainer);
            buttonObj.name = $"Level_{index}_{levelData.displayName}";

            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            bool isUnlocked = LevelProgressManager.IsLevelUnlocked(index);

            if (buttonText != null)
            {
                buttonText.text = isUnlocked ? levelData.displayName : lockedText;
            }

            if (button != null)
            {
                button.interactable = isUnlocked;

                if (isUnlocked)
                {
                    int levelIndex = index; // Capture for closure
                    button.onClick.AddListener(() => OnLevelSelected(levelIndex));
                }
            }
        }

        private void OnLevelSelected(int levelIndex)
        {
            Debug.Log($"LevelSelectorUI: Level {levelIndex} selected.");
            SelectedLevelIndex = levelIndex;

            // Load the Bootstrap scene which will then load the game
            SceneManager.LoadScene("Bootstrap");
        }

        private void OnBackButtonClicked()
        {
            // Tell MenuManager to go back to main menu
            var menuManager = FindFirstObjectByType<MenuManager>();
            if (menuManager != null)
            {
                menuManager.OpenMainMenu();
            }
        }

        /// <summary>
        /// Refreshes the level buttons (e.g., after completing a level).
        /// </summary>
        public void RefreshButtons()
        {
            GenerateLevelButtons();
        }
    }
}

