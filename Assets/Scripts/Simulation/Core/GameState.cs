using System;
using System.Collections.Generic;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Simulation.Core
{
    public class GameState : IGameState
    {
        public GamePhase Phase { get; set; }
        public int CurrentGold { get; private set; }
        public int CurrentWood { get; private set; }
        public int CurrentSteel { get; private set; }
        public int CurrentFood { get; private set; }
        public int CurrentMana { get; private set; }

        public int Wave { get; private set; }

        public event Action OnResourcesChanged;

        public readonly Dictionary<string, int> PlacementCosts = new()
        {
            { "SlimeMonster", 50 },
            { "ArcherMonster", 80 },
            { "SpikeTrap", 30 },
            { "BearTrap", 50 },
            { "Goblin", 50 }
        };

        public GameState(int startingGold = 200, int startingWave = 1)
        {
            CurrentGold = startingGold;
            Wave = startingWave;
            Phase = GamePhase.Placement;
        }

        public bool CanAfford(int cost)
        {
            return CurrentGold >= cost;
        }

        public void SpendGold(int amount)
        {
            if (amount < 0) throw new ArgumentException("Cannot spend negative gold");
            if (!CanAfford(amount)) throw new InvalidOperationException("Insufficient gold");

            CurrentGold -= amount;
            OnResourcesChanged?.Invoke();
        }

        public void EarnGold(int amount)
        {
            if (amount < 0) throw new ArgumentException("Cannot earn negative gold");

            CurrentGold += amount;
            OnResourcesChanged?.Invoke();
        }

        public int GetResource(ResourceType type)
        {
            return type switch
            {
                ResourceType.Gold => CurrentGold,
                ResourceType.Wood => CurrentWood,
                ResourceType.Steel => CurrentSteel,
                ResourceType.Food => CurrentFood,
                ResourceType.Mana => CurrentMana,
                _ => throw new ArgumentOutOfRangeException(nameof(type), "Unknown resource type"),
            };
        }
        public bool AddResource(ResourceType type, int amount)
        {
            if (amount < 0) return false;
            switch (type)
            {
                case ResourceType.Gold:
                    CurrentGold += amount;
                    break;
                case ResourceType.Wood:
                    CurrentWood += amount;
                    break;
                case ResourceType.Steel:
                    CurrentSteel += amount;
                    break;
                case ResourceType.Food:
                    CurrentFood += amount;
                    break;
                case ResourceType.Mana:
                    CurrentMana += amount;
                    break;
                default:
                    return false;
            }
            OnResourcesChanged?.Invoke();
            return true;
        }

        public bool SpendResource(ResourceType type, int amount)
        {
            if (amount < 0) return false;
            switch (type)
            {
                case ResourceType.Gold:
                    if (CurrentGold < amount) return false;
                    CurrentGold -= amount;
                    break;
                case ResourceType.Wood:
                    if (CurrentWood < amount) return false;
                    CurrentWood -= amount;
                    break;
                case ResourceType.Steel:
                    if (CurrentSteel < amount) return false;
                    CurrentSteel -= amount;
                    break;
                case ResourceType.Food:
                    if (CurrentFood < amount) return false;
                    CurrentFood -= amount;
                    break;
                case ResourceType.Mana:
                    if (CurrentMana < amount) return false;
                    CurrentMana -= amount;
                    break;
                default:
                    return false;
            }
            OnResourcesChanged?.Invoke();
            return true;
        }


        public void NextWave()
        {
            Wave++;
        }
    }
}
