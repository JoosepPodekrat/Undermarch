using System;
using System.Collections.Generic;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Grid;

namespace Undermarch
{
    public class Level
    {
        public int Width;
        public int Height;

        public List<TilePos> Entrances { get; set; } = new();
        public List<TilePos> ChestPositions { get; set; } = new();

        public Action<Board, Level, string[]> LoadLayout;

        public Func<Level, WaveSpawner> CreateWaves;

        public Func<Level, WaveSpawner> CreateSecondWaves;
    }
}
