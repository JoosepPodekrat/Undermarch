
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Grid;
using UnityEngine;

namespace Undermarch.Presentation.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public Board Board { get; private set; }
        public ITickSystem TickSystem { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Initialize the simulation state
            Board = new Board(20, 20); // Using a 20x20 board for now.
            TickSystem = new TickSystem();
            
            Debug.Log("GameManager initialized. Board and TickSystem created.");
        }
    }
}
