using UnityEngine;
using UnityEngine.UI;
using Undermarch.Presentation.Sounds;

namespace Undermarch.Presentation.UI
{
    /// <summary>
    /// Controls the options menu functionality.
    /// Wire up the volume slider and toggles in the Inspector.
    /// </summary>
    public class OptionsController : MonoBehaviour
    {
        [Header("Volume Controls")]
        [Tooltip("The master volume slider")]
        public Slider volumeSlider;

        [Header("Audio Toggles")]
        [Tooltip("Toggle for SFX on/off (checkmark style)")]
        public Toggle sfxToggle;
        
        [Tooltip("Toggle for Music on/off (checkmark style)")]
        public Toggle musicToggle;

        private void Start()
        {
            SetupVolumeSlider();
            SetupToggles();
        }

        private void OnEnable()
        {
            // Refresh all controls when options panel is opened
            RefreshControls();
        }

        private void RefreshControls()
        {
            if (UIAudioManager.Instance == null) return;

            // Refresh slider
            if (volumeSlider != null)
            {
                volumeSlider.SetValueWithoutNotify(UIAudioManager.Instance.GetMasterVolume());
            }

            // Refresh toggles to match current state (without triggering callbacks)
            if (sfxToggle != null)
            {
                sfxToggle.SetIsOnWithoutNotify(UIAudioManager.Instance.IsSFXEnabled());
            }

            if (musicToggle != null)
            {
                musicToggle.SetIsOnWithoutNotify(UIAudioManager.Instance.IsMusicEnabled());
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

        private void SetupToggles()
        {
            // Setup SFX toggle
            if (sfxToggle != null)
            {
                // Set initial state from saved settings
                if (UIAudioManager.Instance != null)
                {
                    sfxToggle.SetIsOnWithoutNotify(UIAudioManager.Instance.IsSFXEnabled());
                }
                
                sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
            }

            // Setup Music toggle
            if (musicToggle != null)
            {
                // Set initial state from saved settings
                if (UIAudioManager.Instance != null)
                {
                    musicToggle.SetIsOnWithoutNotify(UIAudioManager.Instance.IsMusicEnabled());
                }
                
                musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
            }
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

        private void OnSFXToggleChanged(bool isOn)
        {
            if (UIAudioManager.Instance != null)
            {
                UIAudioManager.Instance.SetSFXEnabled(isOn);
                
                // Play click sound to give feedback (if SFX was just enabled)
                if (isOn)
                {
                    UIAudioManager.Instance.PlayButtonClick();
                }
            }
        }

        private void OnMusicToggleChanged(bool isOn)
        {
            UIAudioManager.Instance?.PlayButtonClick();
            
            if (UIAudioManager.Instance != null)
            {
                UIAudioManager.Instance.SetMusicEnabled(isOn);
            }
        }

        private void OnDestroy()
        {
            // Clean up listeners
            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
            }

            if (sfxToggle != null)
            {
                sfxToggle.onValueChanged.RemoveListener(OnSFXToggleChanged);
            }

            if (musicToggle != null)
            {
                musicToggle.onValueChanged.RemoveListener(OnMusicToggleChanged);
            }
        }
    }
}
