using UnityEngine;
using UnityEngine.UI;
using Undermarch.Presentation.Sounds;

namespace Undermarch.Presentation.UI
{
    /// <summary>
    /// Controls the options menu functionality.
    /// Wire up the volume slider in the Inspector.
    /// </summary>
    public class OptionsController : MonoBehaviour
    {
        [Header("Volume Controls")]
        [Tooltip("The master volume slider")]
        public Slider volumeSlider;

        private void Start()
        {
            SetupVolumeSlider();
        }

        private void OnEnable()
        {
            // Refresh slider value when options panel is opened
            if (volumeSlider != null && UIAudioManager.Instance != null)
            {
                volumeSlider.value = UIAudioManager.Instance.GetMasterVolume();
            }
        }

        private void SetupVolumeSlider()
        {
            if (volumeSlider == null)
            {
                Debug.LogWarning("OptionsController: Volume slider not assigned!");
                return;
            }

            // Set slider range
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;

            // Load current volume
            float currentVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            volumeSlider.value = currentVolume;

            // Add listener for value changes
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        private void OnVolumeChanged(float value)
        {
            if (UIAudioManager.Instance != null)
            {
                UIAudioManager.Instance.SetMasterVolume(value);
            }
            else
            {
                // Fallback if UIAudioManager doesn't exist yet
                AudioListener.volume = value;
                PlayerPrefs.SetFloat("MasterVolume", value);
                PlayerPrefs.Save();
            }
        }

        private void OnDestroy()
        {
            // Clean up listener
            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
            }
        }
    }
}
