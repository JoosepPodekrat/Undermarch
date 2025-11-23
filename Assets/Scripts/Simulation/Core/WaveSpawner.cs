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
        public int CurrentWave => currentWaveIndex;
        public int TotalWaves => scheduledWaves.Count;
        public bool AllWavesSpawned => currentWaveIndex >= scheduledWaves.Count;

        public WaveSpawner()
        {
            scheduledWaves = new List<HeroParty>();
            currentWaveIndex = 0;
        }

        public void ScheduleWave(HeroParty party)
        {
            scheduledWaves.Add(party);
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

                var pos = board.FindNearestFreeTile(wave.SpawnPosition);
                board.AddEntity(pos, hero);
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


        public void Reset()
        {
            currentWaveIndex = 0;
        }
    }
}
