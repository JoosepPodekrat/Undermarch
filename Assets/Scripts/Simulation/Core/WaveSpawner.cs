using System.Collections.Generic;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Events;
using Undermarch.Simulation.Grid;

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
                    // Spawn this wave
                    foreach (var hero in wave.Heroes)
                    {
                        var pos = board.FindNearestFreeTile(wave.SpawnPosition);
                        board.AddEntity(pos, hero);

                        board.AddEntity(wave.SpawnPosition, hero);
                        CharacterEvents.RaiseSpawn(hero);
                        spawnedHeroes.Add(hero);
                    }

                    SimulationLog.Log($"Wave {currentWaveIndex + 1} spawned: {wave.Heroes.Count} heroes at {wave.SpawnPosition}");

                    if (wave.IsFinalWave)
                    {
                        SimulationLog.Log("FINAL WAVE! These heroes will not flee!");
                    }

                    currentWaveIndex++;
                }
                else
                {
                    break; // Haven't reached this wave's spawn time yet
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
