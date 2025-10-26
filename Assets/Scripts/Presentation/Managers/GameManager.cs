using System.Linq;
using System.Linq;
using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Entities;
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

        private bool isHeroTurn = true;

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
                    Debug.Log($"HandleTick: Tick {tick}, Processing {characters.Count} characters. It is {(isHeroTurn ? "Hero" : "Monster")} turn.");
        
                    if (isHeroTurn)
                    {
                        // Heroes' turn
                        foreach (var character in characters.Where(c => c.faction == Faction.Hero && !c.IsDead))
                        {
                            character.Act(Board);
                        }
                    }
                    else
                    {
                        // Monsters' and Dungeon Master's turn
                        foreach (var character in characters.Where(c => c.faction == Faction.Defender && !c.IsDead))
                        {
                            character.Act(Board);
                        }
                    }
        
                    // Remove dead characters from the board.
                    // We iterate through the original list of characters we fetched at the start of the tick.
                    // If any of them died during the turn, their `IsDead` flag will be true.
                    foreach (var character in characters)
                    {
                        if (character.IsDead)
                        {
                            var pos = Board.GetPositionOf(character);
                            if (pos.IsValid())
                            {
                                Board.RemoveEntity(pos);
                                Debug.Log($"Removed dead character at {pos.x},{pos.y}");
                            }
                        }
                    }
        
                    // Switch turns for the next tick
                    isHeroTurn = !isHeroTurn;
                }    }
}
