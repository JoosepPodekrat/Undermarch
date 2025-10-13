namespace Undermarch.Simulation.Grid
{
    public readonly struct TilePos
    {
        public readonly int x;
        public readonly int y;
        public TilePos(int x, int y) { this.x = x; this.y = y; }
        public override string ToString() => $"({x},{y})";
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