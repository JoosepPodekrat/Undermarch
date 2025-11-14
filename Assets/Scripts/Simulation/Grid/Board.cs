using System;
using System.Collections.Generic;
using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Simulation.Grid
{
    public sealed class Board : IBoard
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        // Event to notify listeners (like the renderer) of changes
        public event Action<TilePos> OnBoardChanged;

        // One occupancy map per layer
        private readonly Dictionary<int, object> _wall = new();
        private readonly Dictionary<int, object> _interactable = new();
        private readonly Dictionary<int, Character> _entity = new(); // Changed to Character for type safety
        private readonly Dictionary<int, List<object>> _effects = new();

        public Board(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int IndexOf(TilePos p) => p.y * Width + p.x;
        public bool InBounds(TilePos p) => p.x >= 0 && p.x < Width && p.y >= 0 && p.y < Height;

        // === Wall Layer ===
        public bool HasWallAt(TilePos pos) => _wall.ContainsKey(IndexOf(pos));
        public void AddWall(TilePos pos)
        {
            if (!InBounds(pos)) return;
            _wall[IndexOf(pos)] = true; // Using a simple boolean for now
            OnBoardChanged?.Invoke(pos);
        }

        // === Interactable Layer ===
        public object GetInteractableAt(TilePos pos)
        {
            _interactable.TryGetValue(IndexOf(pos), out var interactable);
            return interactable;
        }

        public void AddInteractable(TilePos pos, object interactable)
        {
            if (!InBounds(pos)) return;
            _interactable[IndexOf(pos)] = interactable;
            OnBoardChanged?.Invoke(pos);
        }

        public void RemoveInteractable(TilePos pos)
        {
            if (_interactable.Remove(IndexOf(pos)))
            {
                OnBoardChanged?.Invoke(pos);
            }
        }

        // === Entity Layer ===
        public Character GetEntityAt(TilePos pos)
        {
            _entity.TryGetValue(IndexOf(pos), out var character);
            return character;
        }

        public void AddEntity(TilePos pos, Character character)
        {
            if (!InBounds(pos)) return;
            _entity[IndexOf(pos)] = character;
            OnBoardChanged?.Invoke(pos);
        }

        public void RemoveEntity(TilePos pos)
        {
            if (_entity.Remove(IndexOf(pos)))
            {
                OnBoardChanged?.Invoke(pos);
            }
        }

        public void MoveEntity(TilePos from, TilePos to)
        {
            var entity = GetEntityAt(from);
            if (entity == null) return;

            RemoveEntity(from);
            AddEntity(to, entity);
            // Note: RemoveEntity and AddEntity already invoke OnBoardChanged,
            // so we don't need to do it again here.
        }

        public IEnumerable<Character> GetAllCharacters()
        {
            return _entity.Values;
        }

        public TilePos GetPositionOf(Character character)
        {
            foreach (var pair in _entity)
            {
                if (pair.Value == character)
                {
                    int index = pair.Key;
                    int y = index / Width;
                    int x = index % Width;
                    return new TilePos(x, y);
                }
            }
            return TilePos.Invalid;
        }

        public Character FindClosestTarget(Character self, Faction factionToTarget)
        {
            TilePos selfPos = GetPositionOf(self);
            if (!selfPos.IsValid()) return null;

            Character closestTarget = null;
            float minDistanceSq = float.MaxValue;

            foreach (Character target in GetAllCharacters())
            {
                if (target.faction == factionToTarget)
                {
                    TilePos targetPos = GetPositionOf(target);
                    if (!targetPos.IsValid()) continue;

                    float distanceSq = TilePos.DistanceSq(selfPos, targetPos);
                    if (distanceSq < minDistanceSq)
                    {
                        minDistanceSq = distanceSq;
                        closestTarget = target;
                    }
                }
            }

            return closestTarget;
        }
    }
}