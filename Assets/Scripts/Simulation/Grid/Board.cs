using System.Collections.Generic;

namespace Undermarch.Simulation.Grid
{
    public sealed class Board
    {
        public readonly int Width;
        public readonly int Height;
        public bool HasWallAt(TilePos pos) => _wall.ContainsKey(IndexOf(pos));


        // One occupancy map per layer (simple approach; can optimize later)
        private readonly Dictionary<int, object> _wall;          // key = index, value = wall data or null
        private readonly Dictionary<int, object> _interactable;
        private readonly Dictionary<int, object> _entity;
        private readonly Dictionary<int, List<object>> _effects; // multiple per tile

        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            _wall          = new Dictionary<int, object>();
            _interactable  = new Dictionary<int, object>();
            _entity        = new Dictionary<int, object>();
            _effects       = new Dictionary<int, List<object>>();
        }

        public int IndexOf(TilePos p) => p.y * Width + p.x;
        public bool InBounds(TilePos p) => p.x >= 0 && p.x < Width && p.y >= 0 && p.y < Height;

        // Add getters/setters as needed for layers. Use data interfaces (e.g., IWall, IInteractable, IEntity, IEffect).
    }
}