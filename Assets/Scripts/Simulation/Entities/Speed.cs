namespace Undermarch.Simulation.Entities
{
    // Rational speed: numerator/denominator tiles per tick
    public readonly struct Speed
    {
        public readonly int Numerator;   // tiles per tick numerator
        public readonly int Denominator; // tiles per tick denominator
        public Speed(int num, int den) { Numerator = num; Denominator = den; }
        public static Speed TilesPerTick(int tiles) => new Speed(tiles, 1); // e.g., arrow 6/1
    }

    public sealed class MoveAccumulator
    {
        private int _acc;
        private readonly int _den;

        public MoveAccumulator(int denominator) { _den = denominator; _acc = 0; }
        public int AddAndExtractMoves(int numeratorToAdd)
        {
            _acc += numeratorToAdd;
            int moves = _acc / _den;
            _acc -= moves * _den;
            return moves; // how many single-tile steps to execute this tick
        }
    }
}