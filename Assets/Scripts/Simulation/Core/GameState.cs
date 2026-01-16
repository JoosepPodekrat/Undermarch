using System;
using System.Collections.Generic;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Simulation.Core
{
    public class GameState : IGameState
    {
        public GamePhase Phase { get; set; }

        private readonly Dictionary<ResourceType, int> _resources;

        public event Action OnResourcesChanged;

        // Example: placement costs using multiple resources
        public readonly Dictionary<string, Dictionary<ResourceType, int>> PlacementCosts = new()
        {
            {
                "SlimeMonster",
                new Dictionary<ResourceType, int>
                {
                    { ResourceType.Gold, 50 }
                }
            },
            {
                "ArcherMonster",
                new Dictionary<ResourceType, int>
                {
                    { ResourceType.Gold, 80 },
                    { ResourceType.Wood, 20 }
                }
            },
            {
                "SpikeTrap",
                new Dictionary<ResourceType, int>
                {
                    { ResourceType.Gold, 30 },
                    { ResourceType.Steel, 10 }
                }
            },
            {
                "Goblin",
                new Dictionary<ResourceType, int>
                {
                    { ResourceType.Gold, 50 }
                }
            },
            {
                "BearTrap",
                new Dictionary<ResourceType, int>
                {
                    { ResourceType.Gold, 50 }
                }
            }
        };

        public GameState(int startingGold = 200)
        {
            Phase = GamePhase.Placement;

            _resources = new Dictionary<ResourceType, int>
            {
                { ResourceType.Gold, startingGold },
                { ResourceType.Wood, 0 },
                { ResourceType.Steel, 100 },
                { ResourceType.Food, 0 },
                { ResourceType.Mana, 0 },
                { ResourceType.Corpse, 0 }
            };
        }

        // ----------------------
        // Resource access
        // ----------------------

        public int GetResource(ResourceType type)
        {
            return _resources.TryGetValue(type, out var amount) ? amount : 0;
        }

        public bool AddResource(ResourceType type, int amount)
        {
            if (amount < 0) return false;

            if (!_resources.ContainsKey(type))
                _resources[type] = 0;

            _resources[type] += amount;
            OnResourcesChanged?.Invoke();
            return true;
        }

        public bool SpendResource(ResourceType type, int amount)
        {
            if (amount < 0) return false;
            if (GetResource(type) < amount) return false;

            _resources[type] -= amount;
            OnResourcesChanged?.Invoke();
            return true;
        }

        // ----------------------
        // Multi-resource costs
        // ----------------------

        public bool CanAfford(Dictionary<ResourceType, int> cost)
        {
            foreach (var (type, amount) in cost)
            {
                if (GetResource(type) < amount)
                    return false;
            }
            return true;
        }

        public bool SpendResources(Dictionary<ResourceType, int> cost)
        {
            if (!CanAfford(cost))
                return false;

            foreach (var (type, amount) in cost)
            {
                _resources[type] -= amount;
            }

            OnResourcesChanged?.Invoke();
            return true;
        }
    }
}
