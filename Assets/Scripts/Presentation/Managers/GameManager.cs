using System.Linq;
using System.Linq;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Levels; // Added this using statement
using UnityEngine;

namespace Undermarch.Presentation.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public Board Board { get; private set; }
        public TickSystem TickSystem { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Debug.Log("GameManager: Awake()");

            // Initialize the simulation state
            Board = new Board(20, 20); // Using a 20x20 board for now.
            TickSystem = new TickSystem();
            
            Debug.Log("GameManager initialized. Board and TickSystem created.");
        }

        private void Start()
        {
            Debug.Log("GameManager: Start()");
            // Load the level content onto the board
            LevelLoader.LoadLevel1(Board);
            Debug.Log("GameManager: Level 1 loaded.");

            if (TickSystem != null)
            {
                TickSystem.OnTick += HandleTick;
                Debug.Log("GameManager: Subscribed to OnTick event.");
            }
            else
            {
                Debug.LogError("GameManager: TickSystem is null, cannot subscribe to OnTick event.");
            }
        }

        public void InitializeTickDriver(Bootstrap.TickDriver driver)
        {
            if (driver != null)
            {
                driver.SetTickSystem(TickSystem);
                Debug.Log("GameManager: Successfully initialized TickDriver.");
            }
            else
            {
                Debug.LogError("GameManager: Attempted to initialize a null TickDriver.");
            }
        }

        private void OnDestroy()
        {
            if (TickSystem != null)
            {
                TickSystem.OnTick -= HandleTick;
            }
        }

        private void HandleTick(int tick)
        {
            var characters = Board.GetAllCharacters().ToList();
            Debug.Log($"HandleTick: Processing {characters.Count} characters.");

            // .ToList() creates a copy of the collection, so we can modify the original
            // collection inside the loop without causing an error.
            foreach (var character in characters)
            {
                if (!character.IsDead)
                {
                    character.Act(Board);
                }
            }
        }
    }
}
