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
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
            }
        }
        public void Play(string name) {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            s.source.Play(); 
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
