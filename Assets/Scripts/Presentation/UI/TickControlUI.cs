using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Presentation.UI
{
    /// <summary>
    /// UI controls for the tick system (pause, resume, step, speed).
    /// Unity Integration Steps:
    /// 1. Create UI buttons in your UI scene for: Pause, Resume, Step
    /// 2. Create UI text to display current tick count
    /// 3. Attach this script to a GameObject in the UI scene
    /// 4. Assign button references in the Inspector
    /// 5. Call Initialize() from GameManager after creating TickSystem
    /// </summary>
    public class TickControlUI : MonoBehaviour
    {
        public Button pauseButton;
        public Button resumeButton;
        public Button stepButton;
        public TextMeshProUGUI tickCountText;
        public TextMeshProUGUI modeText;

        private ITickSystem tickSystem;

        public void Initialize(ITickSystem tickSystem)
        {
            this.tickSystem = tickSystem;

            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseClicked);
            }

            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(OnResumeClicked);
            }

            if (stepButton != null)
            {
                stepButton.onClick.AddListener(OnStepClicked);
            }

            UpdateDisplay();
        }

        private void Update()
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (tickSystem == null) return;

            if (tickCountText != null)
            {
                tickCountText.text = $"Tick: {tickSystem.CurrentTick}";
            }

            if (modeText != null)
            {
                modeText.text = $"Mode: {tickSystem.Mode}";
            }
        }

        private void OnPauseClicked()
        {
            tickSystem?.Pause();
        }

        private void OnResumeClicked()
        {
            tickSystem?.Resume();
        }

        private void OnStepClicked()
        {
            tickSystem?.Step();
        }

        private void OnDestroy()
        {
            if (pauseButton != null) pauseButton.onClick.RemoveListener(OnPauseClicked);
            if (resumeButton != null) resumeButton.onClick.RemoveListener(OnResumeClicked);
            if (stepButton != null) stepButton.onClick.RemoveListener(OnStepClicked);
        }
    }
}
