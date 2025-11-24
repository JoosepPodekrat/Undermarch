
/*
using Mono.Cecil;
using System;
using Undermarch.Presentation;
using Undermarch.Simulation.Events;
using UnityEngine;
using UnityEngine.Audio;
using Undermarch.Simulation.Entities;

namespace Undermarch.Presentation.Controllers
{
    public class AudioController : MonoBehaviour
    {

        private AudioSource _source;

        public Sound[] sounds;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            foreach (Sound s in sounds) 
            {

                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                if (s.clip == null)
                {
                    Debug.LogWarning($"Sound '{s.name}' has no AudioClip assigned!");
                    continue;
                }
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.spatialBlend = 0f;
                s.source.playOnAwake = false;
                Debug.Log($"AudioSource for '{s.name}' initialized.");
            }
        }
        public void Play(string name)
        {

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("AudioController.Play called with null or empty name.");
                return;
            }

            Sound s = Array.Find(sounds, sound => sound.name == name);

            if (s == null)
            {
                Debug.LogWarning($"AudioController: Sound with name '{name}' not found in the sounds array!");
                return;
            }

            if (s.source == null)
            {
                Debug.LogWarning($"AudioController: AudioSource for sound '{name}' is not initialized!");
                return;
            }

            s.source.Play();
            Debug.Log($"AudioController: Playing sound '{name}'");
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

        void HandleSpawn(Character c) => Play(c.spawnSound);
        void HandleDeath(Character c) => Play(c.deathSound);
        void HandleHurt(Character c) => Play(c.hurtSound);
        void HandleAttack(Character c) => Play(c.attackSound);
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
*/
using UnityEngine;

namespace Undermarch.Presentation.Controllers
{
    public class AudioController : MonoBehaviour
    {
        // Make these assignable in the inspector
        [Header("Character Sounds")]
        public Sound slimeSound;
        public Sound humanMaleGrunt;
        public Sound humanMaleHurt;
        public Sound femaleGrunt;
        public Sound femaleHurt;

        private void Awake()
        {
            // Initialize each sound's AudioSource
            InitializeSound(slimeSound);
            InitializeSound(humanMaleGrunt);
            InitializeSound(humanMaleHurt);
            InitializeSound(femaleGrunt);
            InitializeSound(femaleHurt);
        }

        private void InitializeSound(Sound s)
        {
            if (s == null)
            {
                Debug.LogWarning("AudioController: Sound is null and cannot be initialized.");
                return;
            }

            if (s.clip == null)
            {
                Debug.LogWarning($"AudioController: Sound '{s.name}' has no AudioClip assigned!");
                return;
            }

            // Only create a new AudioSource if one hasn't been assigned already
            if (s.source == null)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.spatialBlend = 0f; // 2D sound
                s.source.playOnAwake = false;

                Debug.Log($"AudioController: AudioSource for '{s.name}' initialized.");
            }
            else
            {
                // Use the already assigned AudioSource
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.spatialBlend = 0f;
                s.source.playOnAwake = false;

                Debug.Log($"AudioController: Using preassigned AudioSource for '{s.name}'.");
            }
        }


        // Dedicated play functions for each sound
        public void PlaySlimeSound()
        {
            PlaySound(slimeSound);
        }

        public void PlayHumanMaleGrunt()
        {
            PlaySound(humanMaleGrunt);
        }

        public void PlayHumanMaleHurt()
        {
            PlaySound(humanMaleHurt);
        }

        public void PlayFemaleGrunt()
        {
            PlaySound(femaleGrunt);
        }

        public void PlayFemaleHurt()
        {
            PlaySound(femaleHurt);
        }

        // Helper function to play a sound safely
        private void PlaySound(Sound s)
        {
            if (s == null || s.source == null)
            {
                Debug.LogWarning("AudioController: Sound or AudioSource is null, cannot play.");
                return;
            }

            s.source.Play();
            Debug.Log($"AudioController: Playing '{s.name}'");
        }
    }

}

