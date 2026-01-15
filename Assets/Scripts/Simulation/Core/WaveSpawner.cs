using System.Collections.Generic;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Events;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Entities.Characters.Heroes;
using Undermarch.Simulation.Core;
using ResourceType = Undermarch.Simulation.Interfaces.ResourceType;



namespace Undermarch.Simulation.Core

{
    public class HeroParty
    {
        public List<Character> Heroes { get; set; }
        public int SpawnTick { get; set; }
        public TilePos SpawnPosition { get; set; }
        public bool IsFinalWave { get; set; }
        public int TicksUntilNextWave { get; set; } = 30; // Delay after this wave before next (default ~15 sec @ 2 TPS)


        public HeroParty(List<Character> heroes, int spawnTick, TilePos spawnPos, bool isFinal = false)
        {
            Heroes = heroes;
            SpawnTick = spawnTick;
            SpawnPosition = spawnPos;
            IsFinalWave = isFinal;
        }
    }

    /// <summary>
    /// Manages wave-based hero spawning for dungeon defense gameplay.
    /// </summary>
    public class WaveSpawner
    {
        private List<HeroParty> scheduledWaves;
        private int currentWaveIndex;
        private int _nextSpawnTick = 0; // Track next spawn time for scheduling

        public int CurrentWave => currentWaveIndex;
        public int TotalWaves => scheduledWaves.Count;
        public bool AllWavesSpawned => currentWaveIndex >= scheduledWaves.Count;

        public WaveSpawner()
        {
            scheduledWaves = new List<HeroParty>();
            currentWaveIndex = 0;
        }

        /// <summary>
        /// Reset the scheduler to start from tick 0. Call when creating a new wave schedule.
        /// </summary>
        public void ResetScheduler()
        {
            _nextSpawnTick = 0;
        }

        /// <summary>
        /// Schedule a wave. SpawnTick is assigned automatically based on TicksUntilNextWave.
        /// </summary>
        public void ScheduleWave(HeroParty party)
        {
            party.SpawnTick = _nextSpawnTick;
            scheduledWaves.Add(party);
            // Next wave spawns after this wave's delay
            _nextSpawnTick += party.TicksUntilNextWave;
        }

       public List<Character> CheckSpawn(int currentTick, Board board)
{
    List<Character> spawnedHeroes = new List<Character>();

    while (currentWaveIndex < scheduledWaves.Count)
    {
        HeroParty wave = scheduledWaves[currentWaveIndex];

        if (currentTick >= wave.SpawnTick)
        {
            foreach (var hero in wave.Heroes)
            {
                // Final wave heroes never flee
                if (wave.IsFinalWave && hero is Hero h)
                {
                    h.FleeThreshold = int.MaxValue;
                }
                 hero.spawnSound = "humanmalegrunt";
                        hero.hurtSound = "humanMaleHurt";
                        hero.attackSound = "humanMaleGrunt";
                        hero.deathSound = "humanMaleHurt";

                        var pos = board.FindNearestFreeTile(wave.SpawnPosition);
                        board.AddEntity(pos, hero);

                        // Raise spawn event (audio handled by AudioController)
                        CharacterEvents.RaiseSpawn(hero);

                        spawnedHeroes.Add(hero);
            }

            SimulationLog.Log($"Wave {currentWaveIndex + 1} spawned: {wave.Heroes.Count} heroes at {wave.SpawnPosition}");

            currentWaveIndex++;
        }
        else
        {
            break;
        }
    }

    return spawnedHeroes;
}


        /// <summary>
        /// Manually spawn the next wave immediately, bypassing scheduled tick timing.
        /// Returns the list of spawned heroes, or empty if no waves remaining.
        /// </summary>
        public List<Character> ForceSpawnNextWave(Board board)
        {
            List<Character> spawnedHeroes = new List<Character>();

            if (currentWaveIndex >= scheduledWaves.Count)
            {
                SimulationLog.Log("WaveSpawner: All waves already spawned!");
                return spawnedHeroes;
            }

            HeroParty wave = scheduledWaves[currentWaveIndex];

            foreach (var hero in wave.Heroes)
            {
                // Final wave heroes never flee
                if (wave.IsFinalWave && hero is Hero h)
                {
                    h.FleeThreshold = int.MaxValue;
                }
                hero.spawnSound = "humanmalegrunt";
                hero.hurtSound = "humanMaleHurt";
                hero.attackSound = "humanMaleGrunt";
                hero.deathSound = "humanMaleHurt";

                var pos = board.FindNearestFreeTile(wave.SpawnPosition);
                board.AddEntity(pos, hero);

                CharacterEvents.RaiseSpawn(hero);
                spawnedHeroes.Add(hero);
            }

            SimulationLog.Log($"Wave {currentWaveIndex + 1} FORCE SPAWNED: {wave.Heroes.Count} heroes");
            currentWaveIndex++;

            return spawnedHeroes;
        }

        public void Reset()
        {
            currentWaveIndex = 0;
        }
    }
}
