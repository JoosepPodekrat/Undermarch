using UnityEngine;

namespace Undermarch.Presentation.Sounds
{
    /// <summary>
    /// Singleton that manages all UI sounds (button clicks, selection, placement)
    /// and background music. Persists across scene loads.
    /// </summary>
    public class UIAudioManager : MonoBehaviour
    {
        public static UIAudioManager Instance { get; private set; }

        [Header("UI Sound Effects")]
        [Tooltip("Sound played when clicking any button")]
        public AudioClip buttonClickClip;
        
        [Tooltip("Sound played when selecting a buildable")]
        public AudioClip selectBuildableClip;
        
        [Tooltip("Sound played when placing a buildable")]
        public AudioClip placementClip;

        [Header("Music")]
        [Tooltip("Background music for the main menu")]
        public AudioClip menuMusicClip;
        
        [Tooltip("Should menu music loop?")]
        public bool loopMenuMusic = true;

        [Header("Volume Settings")]
        [Range(0f, 1f)]
        public float sfxVolume = 1f;
        
        [Range(0f, 1f)]
        public float musicVolume = 0.5f;

        // Mute states (separate from volume so we can restore)
        private bool sfxEnabled = true;
        private bool musicEnabled = true;

        // Audio sources
        private AudioSource sfxSource;
        private AudioSource musicSource;

        // PlayerPrefs keys
        private const string PREF_SFX_ENABLED = "SFXEnabled";
        private const string PREF_MUSIC_ENABLED = "MusicEnabled";

        private void Awake()
        {
            // Singleton pattern with DontDestroyOnLoad
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Create audio sources
            SetupAudioSources();

            // Load saved volume settings
            LoadVolumeSettings();
        }

        private void SetupAudioSources()
        {
            // SFX source for UI sounds
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f; // 2D sound
            sfxSource.ignoreListenerPause = true; // Play even when game is paused

            // Music source for background music
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.spatialBlend = 0f; // 2D sound
            musicSource.loop = loopMenuMusic;
        }

        private void LoadVolumeSettings()
        {
            // Load master volume from PlayerPrefs
            float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            SetMasterVolume(savedVolume);

            // Load SFX and Music enabled states
            sfxEnabled = PlayerPrefs.GetInt(PREF_SFX_ENABLED, 1) == 1;
            musicEnabled = PlayerPrefs.GetInt(PREF_MUSIC_ENABLED, 1) == 1;
        }

        #region Public Methods - Sound Playback

        /// <summary>
        /// Play button click sound. Call this from any button's onClick handler.
        /// </summary>
        public void PlayButtonClick()
        {
            PlaySFX(buttonClickClip);
        }

        /// <summary>
        /// Play sound when selecting a buildable from the slider.
        /// </summary>
        public void PlaySelectSound()
        {
            PlaySFX(selectBuildableClip);
        }

        /// <summary>
        /// Play sound when successfully placing a buildable.
        /// </summary>
        public void PlayPlacementSound()
        {
            PlaySFX(placementClip);
        }

        /// <summary>
        /// Play any SFX clip.
        /// </summary>
        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || sfxSource == null) return;
            if (!sfxEnabled) return; // Don't play if SFX is disabled
            
            sfxSource.volume = sfxVolume;
            sfxSource.PlayOneShot(clip);
        }

        #endregion

        #region Public Methods - Music

        /// <summary>
        /// Start playing menu background music.
        /// </summary>
        public void PlayMenuMusic()
        {
            if (menuMusicClip == null || musicSource == null) return;
            
            if (musicSource.clip == menuMusicClip && musicSource.isPlaying)
                return; // Already playing

            musicSource.clip = menuMusicClip;
            musicSource.volume = musicEnabled ? musicVolume : 0f;
            musicSource.loop = loopMenuMusic;
            musicSource.Play();
        }

        /// <summary>
        /// Stop background music.
        /// </summary>
        public void StopMusic()
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }

        /// <summary>
        /// Check if music is currently playing.
        /// </summary>
        public bool IsMusicPlaying()
        {
            return musicSource != null && musicSource.isPlaying;
        }

        #endregion

        #region Public Methods - Volume Control

        /// <summary>
        /// Set master volume (0-1). This affects AudioListener.volume.
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            AudioListener.volume = volume;
            PlayerPrefs.SetFloat("MasterVolume", volume);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Get current master volume.
        /// </summary>
        public float GetMasterVolume()
        {
            return AudioListener.volume;
        }

        /// <summary>
        /// Set SFX volume (0-1).
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }

        /// <summary>
        /// Get current SFX volume.
        /// </summary>
        public float GetSFXVolume()
        {
            return sfxVolume;
        }

        /// <summary>
        /// Set music volume (0-1).
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null && musicEnabled)
            {
                musicSource.volume = musicVolume;
            }
        }

        #endregion

        #region Public Methods - Enable/Disable Toggle

        /// <summary>
        /// Enable or disable SFX. When disabled, no SFX will play.
        /// </summary>
        public void SetSFXEnabled(bool enabled)
        {
            sfxEnabled = enabled;
            PlayerPrefs.SetInt(PREF_SFX_ENABLED, enabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Toggle SFX enabled state.
        /// </summary>
        public void ToggleSFX()
        {
            SetSFXEnabled(!sfxEnabled);
        }

        /// <summary>
        /// Check if SFX is enabled.
        /// </summary>
        public bool IsSFXEnabled()
        {
            return sfxEnabled;
        }

        /// <summary>
        /// Enable or disable music. When disabled, music volume is set to 0.
        /// </summary>
        public void SetMusicEnabled(bool enabled)
        {
            musicEnabled = enabled;
            PlayerPrefs.SetInt(PREF_MUSIC_ENABLED, enabled ? 1 : 0);
            PlayerPrefs.Save();

            if (musicSource != null)
            {
                musicSource.volume = enabled ? musicVolume : 0f;
            }
        }

        /// <summary>
        /// Toggle music enabled state.
        /// </summary>
        public void ToggleMusic()
        {
            SetMusicEnabled(!musicEnabled);
        }

        /// <summary>
        /// Check if music is enabled.
        /// </summary>
        public bool IsMusicEnabled()
        {
            return musicEnabled;
        }

        #endregion
    }
}
