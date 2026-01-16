using UnityEngine;
using TMPro;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Presentation.UI
{
    /// <summary>
    /// Displays current resources (gold) on screen.
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
            if (goldText == null || gameState == null)
                return;

            int gold = gameState.GetResource(ResourceType.Gold);
            goldText.text = $"Gold: {gold}";
        }
    }
}
