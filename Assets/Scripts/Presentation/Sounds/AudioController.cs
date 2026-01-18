using UnityEngine;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Events;

namespace Undermarch.Presentation.Sounds
{
    // Simple wrapper for logging so we don't get errors
    public static class SimulationLog
    {
        public static void Log(string message)
        {
            Debug.Log(message);
        }
    }

    /// <summary>
    /// Handles character-related sound effects (grunts, hurt sounds, etc).
    /// Respects the global SFX enabled settings from UIAudioManager.
    /// </summary>
    public class AudioController : MonoBehaviour
    {
        [Header("Character Sounds")]
        public Sound slimeSound;
        public Sound humanmalegrunt;
        public Sound humanMaleHurt;
        public Sound femaleGrunt;
        public Sound femaleHurt;

        private void Awake()
        {
            // Keep AudioController at origin to prevent any spatial audio issues
            transform.position = Vector3.zero;

            InitializeSound(slimeSound);
            InitializeSound(humanmalegrunt);
            InitializeSound(humanMaleHurt);
            InitializeSound(femaleGrunt);
            InitializeSound(femaleHurt);
        }

        private void InitializeSound(Sound s)
        {
            if (s == null) return;

            if (s.clip == null)
            {
                Debug.LogWarning($"AudioController: Sound '{s.name}' has no AudioClip assigned!");
                return;
            }

            if (s.source == null)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.spatialBlend = 0f; // 2D sound
                s.source.playOnAwake = false;

                SimulationLog.Log($"AudioController: AudioSource for '{s.name}' initialized.");
            }
        }

        private void OnEnable()
        {
            CharacterEvents.OnCharacterSpawned += HandleSpawn;
            CharacterEvents.OnCharacterDied += HandleDeath;
            CharacterEvents.OnCharacterHurt += HandleHurt;
            CharacterEvents.OnCharacterAttacked += HandleAttack;
        }

        private void OnDisable()
        {
            CharacterEvents.OnCharacterSpawned -= HandleSpawn;
            CharacterEvents.OnCharacterDied -= HandleDeath;
            CharacterEvents.OnCharacterHurt -= HandleHurt;
            CharacterEvents.OnCharacterAttacked -= HandleAttack;
        }

        // Event handlers
        private void HandleSpawn(Character c) => PlayCharacterSound(c.spawnSound);
        private void HandleDeath(Character c) => PlayCharacterSound(c.deathSound);
        private void HandleHurt(Character c) => PlayCharacterSound(c.hurtSound);
        private void HandleAttack(Character c) => PlayCharacterSound(c.attackSound);

        private void PlayCharacterSound(string soundName)
        {
            if (string.IsNullOrEmpty(soundName)) return;

            Sound s = soundName switch
            
            {
                var n when n == slimeSound?.name => slimeSound,
                var n when n == humanmalegrunt?.name => humanmalegrunt,
                var n when n == humanMaleHurt?.name => humanMaleHurt,
                var n when n == femaleGrunt?.name => femaleGrunt,
                var n when n == femaleHurt?.name => femaleHurt,
                _ => null
            };

            if (s != null) PlaySound(s);
            else Debug.LogWarning($"AudioController: No matching Sound for '{soundName}'");
        }

        private void PlaySound(Sound s)
        {
            if (s == null || s.source == null)
            {
                Debug.LogWarning("AudioController: Sound or AudioSource is null, cannot play.");
                return;
            }

            // Check if SFX is enabled globally
            if (UIAudioManager.Instance != null && !UIAudioManager.Instance.IsSFXEnabled())
            {
                return; // SFX is disabled, don't play
            }

            // Apply global SFX volume multiplier
            float globalSfxVolume = UIAudioManager.Instance != null 
                ? UIAudioManager.Instance.GetSFXVolume() 
                : 1f;
            
            s.source.volume = s.volume * globalSfxVolume;
            s.source.Play();
            SimulationLog.Log($"AudioController: Playing '{s.name}'");
        }

        // Public methods for PlacementController or other scripts
        public void PlaySlimeSound() => PlaySound(slimeSound);
        public void PlayHumanMaleGrunt() => PlaySound(humanmalegrunt);
        public void PlayHumanMaleHurt() => PlaySound(humanMaleHurt);
        public void PlayFemaleGrunt() => PlaySound(femaleGrunt);
        public void PlayFemaleHurt() => PlaySound(femaleHurt);
    }
}
