using UnityEngine;
using UnityEngine.UI;
using Undermarch.Simulation.Core;
namespace Undermarch.Presentation.UI.TickCounter
{
    public class TickCounter : MonoBehaviour
    {
        [SerializeField] private Text tickText;   // Assign in inspector
        private TickSystem tickSystem;

        public void Initialize(TickSystem system)
        {
            tickSystem = system;
            tickSystem.OnTick += UpdateTickDisplay;
            UpdateTickDisplay(tickSystem.CurrentTick);
        }

        private void OnDestroy()
        {
            if (tickSystem != null)
                tickSystem.OnTick -= UpdateTickDisplay;
        }

        private void UpdateTickDisplay(int tick)
        {
            tickText.text = $"Tick: {tick}";
        }
    }
}
