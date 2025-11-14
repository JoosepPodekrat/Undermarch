using UnityEngine;
using TMPro;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Presentation.UI
{
    /// <summary>
    /// Displays current resources (gold) on screen.
    /// Attach this to a TextMeshProUGUI object in the UI scene.
    /// Assign the text field in the Inspector.
    /// </summary>
    public class ResourceDisplay : MonoBehaviour
    {
        public TextMeshProUGUI goldText;

        private IGameState gameState;

        public void Initialize(IGameState gameState)
        {
            this.gameState = gameState;
            gameState.OnResourcesChanged += UpdateDisplay;
            UpdateDisplay();
        }

        private void OnDestroy()
        {
            if (gameState != null)
            {
                gameState.OnResourcesChanged -= UpdateDisplay;
            }
        }

        private void UpdateDisplay()
        {
            if (goldText != null && gameState != null)
            {
                goldText.text = $"Gold: {gameState.CurrentGold}";
            }
        }
    }
}
