using TMPro;
using Undermarch.Presentation.Managers;
using UnityEngine;

namespace Undermarch.Presentation.UI
{
    public class EndGameUI : MonoBehaviour
    {
        public GameObject endGamePanel;
        public TextMeshProUGUI endGameText;

        public void ShowEndGamePopup(string message)
        {
            if (endGamePanel != null && endGameText != null)
            {
                endGamePanel.SetActive(true);
                endGameText.text = message;
            }
        }

        public void OnClick_PlayAgain()
        {
            GameManager.Instance.RestartGame();
        }
    }
}
