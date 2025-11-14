using System;
using System.Collections.Generic;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Simulation.Core
{
    public class GameState : IGameState
    {
        public GamePhase Phase { get; set; }
        public int CurrentGold { get; private set; }
        public int Wave { get; private set; }

        public event Action OnResourcesChanged;

        public readonly Dictionary<string, int> PlacementCosts = new()
        {
            { "SlimeMonster", 50 },
            { "ArcherMonster", 80 },
            { "SpikeTrap", 30 }
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

        public void NextWave()
        {
            Wave++;
        }
    }
}
