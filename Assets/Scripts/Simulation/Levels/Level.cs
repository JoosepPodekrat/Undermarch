using System.Collections.Generic;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Grid;

namespace Undermarch
{
    public class Level
    {
        public int Width;
        public int Height;

        public List<TilePos> Entrances { get; private set; } = new();
        public List<TilePos> ChestPositions { get; private set; } = new();

        public System.Action<Board, Level, string[]> LoadLayout;
        public System.Func<List<TilePos>, WaveSpawner> CreateWaves;
    }


}
