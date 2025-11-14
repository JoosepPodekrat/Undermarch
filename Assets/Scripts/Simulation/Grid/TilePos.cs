namespace Undermarch.Simulation.Grid
{
    // pure simulation data
    public struct TilePos
    {
        public static readonly TilePos Invalid = new TilePos(-1, -1);

        public readonly int x;
        public int y;

        public TilePos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString() => $"({x},{y})";

        public bool IsValid() => x != -1 && y != -1;

        public static float DistanceSq(TilePos a, TilePos b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            return dx * dx + dy * dy;
        }

        public static int ManhattanDistance(TilePos a, TilePos b)
        {
            return System.Math.Abs(a.x - b.x) + System.Math.Abs(a.y - b.y);
        }

        public static TilePos operator +(TilePos a, TilePos b)
        {
            return new TilePos(a.x + b.x, a.y + b.y);
        }

        public override bool Equals(object obj)
        {
            if (obj is TilePos other)
            {
                return x == other.x && y == other.y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (x, y).GetHashCode();
        }

        public static bool operator ==(TilePos a, TilePos b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(TilePos a, TilePos b)
        {
            return !(a == b);
        }
    }

    public enum TileLayer
    {
        Ground = 0,
        Wall = 1,
        Interactable = 2,
        Entity = 3,
        Effects = 4
    }
}