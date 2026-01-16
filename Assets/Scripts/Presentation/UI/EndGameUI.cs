using TMPro;
using Undermarch.Presentation.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Undermarch.Presentation.UI
{
    public class EndGameUI : MonoBehaviour
    {
        public GameObject endGamePanel;
        public TextMeshProUGUI endGameText;
        
        [Header("Buttons")]
        public Button playAgainButton;
        public Button mainMenuButton;
        public Button nextLevelButton;

        private bool isWin;

        private void Awake()
        {
            // Wire up button listeners
            if (playAgainButton != null)
            {
                playAgainButton.onClick.AddListener(OnClick_PlayAgain);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnClick_MainMenu);
            }

            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.AddListener(OnClick_NextLevel);
            }
        }

        /// <summary>
        /// Shows the end game popup with the specified message.
        /// </summary>
        /// <param name="message">The message to display (e.g., "You Win!" or "You Lose!")</param>
        public void ShowEndGamePopup(string message)
        {
            if (endGamePanel != null && endGameText != null)
            {
                endGamePanel.SetActive(true);
                endGameText.text = message;
                
                // Determine if this is a win
                isWin = message.Contains("Win");
                
                // Update button visibility
                UpdateButtonVisibility();
            }
        }

        private void UpdateButtonVisibility()
        {
            // Next Level button should only be visible on win AND if there's a next level available
            if (nextLevelButton != null)
            {
                bool hasNextLevel = GameManager.Instance != null && GameManager.Instance.HasNextLevel();
                nextLevelButton.gameObject.SetActive(isWin && hasNextLevel);
            }
        }

        public void OnClick_PlayAgain()
        {
            GameManager.Instance.RestartGame();
        }

        public void OnClick_MainMenu()
        {
            GameManager.Instance.GoToMainMenu();
        }

        public void OnClick_NextLevel()
        {
            if (isWin && GameManager.Instance != null)
            {
                GameManager.Instance.LoadNextLevel();
            }
        }
    }
}
