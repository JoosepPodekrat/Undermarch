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

        // Audio sources
        private AudioSource sfxSource;
        private AudioSource musicSource;

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
            musicSource.volume = musicVolume;
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
        /// Set music volume (0-1).
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
            }
        }

        #endregion
    }
}
