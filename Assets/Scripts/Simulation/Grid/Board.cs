using System.Collections.Generic;

namespace Undermarch.Simulation.Grid
{
    public sealed class Board
    {
        public readonly int Width;
        public readonly int Height;

        private readonly Dictionary<int, object> _wall = new();
        private readonly Dictionary<int, object> _interactable = new();
        private readonly Dictionary<int, Entity> _entity = new();
        private readonly Dictionary<int, List<object>> _effects = new();

        public Board(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int IndexOf(TilePos p) => p.y * Width + p.x;
        public bool InBounds(TilePos p) => p.x >= 0 && p.x < Width && p.y >= 0 && p.y < Height;

        // Walls
        public bool HasWallAt(TilePos pos) => _wall.ContainsKey(IndexOf(pos));
        public void PlaceWall(TilePos pos)
        {
            if (!InBounds(pos)) return;
            _wall[IndexOf(pos)] = new object();
        }

        // Entities
        public bool TryPlaceEntity(Entity e, TilePos pos)
        {
            if (!InBounds(pos)) return false;
            int idx = IndexOf(pos);
            if (_entity.ContainsKey(idx) || HasWallAt(pos)) return false;

            _entity[idx] = e;
            e.Position = pos;
            return true;
        }

        public void RemoveEntity(TilePos pos) => _entity.Remove(IndexOf(pos));
        public bool HasEntity(TilePos pos) => _entity.ContainsKey(IndexOf(pos));
        public Entity GetEntity(TilePos pos) => _entity.ContainsKey(IndexOf(pos)) ? _entity[IndexOf(pos)] : null;

        public bool TryMoveEntity(Entity e, TilePos target)
        {
            if (!InBounds(target)) return false;
            int idx = IndexOf(target);
            if (HasWallAt(target) || _entity.ContainsKey(idx)) return false;

            RemoveEntity(e.Position);
            _entity[idx] = e;
            e.Position = target;
            return true;
        }

        // Interactables
        public bool HasInteractable(TilePos pos) => _interactable.ContainsKey(IndexOf(pos));
        public void PlaceInteractable(TilePos pos)
        {
            if (!InBounds(pos)) return;
            _interactable[IndexOf(pos)] = new object();
        }

        // Effects
        public void AddEffect(TilePos pos, object effect)
        {
            if (!InBounds(pos)) return;
            int idx = IndexOf(pos);
            if (!_effects.ContainsKey(idx)) _effects[idx] = new List<object>();
            _effects[idx].Add(effect);
        }

        public List<object> GetEffects(TilePos pos)
        {
            int idx = IndexOf(pos);
            return _effects.ContainsKey(idx) ? _effects[idx] : new List<object>();
        }
    }

    // Minimal Entity wrapper for now
    public class Entity
    {
        public TilePos Position { get; set; }
        public Character CharacterData { get; set; }

        public Entity(Character c, TilePos startPos)
        {
            CharacterData = c;
            Position = startPos;
        }
    }
}
