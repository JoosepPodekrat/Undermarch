using System;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Simulation.Core
{
    /// <summary>
    /// Deterministic random number generator using a seed for reproducible simulations.
    /// </summary>
    public class SeededRandom : IRandomSource
    {
        private Random random;
        public int Seed { get; private set; }

        public SeededRandom(int seed)
        {
            Seed = seed;
            random = new Random(seed);
        }

        public int Next()
        {
            return random.Next();
        }

        public int Next(int maxValue)
        {
            return random.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        public float NextFloat()
        {
            return (float)random.NextDouble();
        }

        public float NextFloat(float minValue, float maxValue)
        {
            return minValue + (float)random.NextDouble() * (maxValue - minValue);
        }

        public bool NextBool()
        {
            return random.Next(2) == 1;
        }

        public void Reset()
        {
            random = new Random(Seed);
        }
    }
}
