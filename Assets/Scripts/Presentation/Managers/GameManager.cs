using System.Collections.Generic;
using System.Linq;
using Undermarch.Presentation.UI;
using Undermarch.Simulation.Combat;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Entities.Characters.DungeonMaster;
using Undermarch.Simulation.Grid;
using Undermarch.Simulation.Levels;
using UnityEngine;
using UnityEngine.SceneManagement;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Presentation.Managers
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public Board Board { get; private set; }
        public TickSystem TickSystem { get; private set; }
        public Simulation.Core.GameState GameState { get; private set; }
        public GamePhase CurrentPhase { get; private set; }
        public EndGameUI endGameUI;

        // Optional: Assign these in Unity Editor if you want UI displays
        public ResourceDisplay resourceDisplay;
        public TickControlUI tickControlUI;

        private WaveSpawner waveSpawner;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            CurrentPhase = GamePhase.Placement;
            Debug.Log("GameManager: Awake() - Phase: Placement");

            Board = new Board(20, 20);
            GameState = new Simulation.Core.GameState(startingGold: 200);

            Debug.Log("GameManager initialized. Board and GameState created.");
        }

        private void Start()
        {
            Debug.Log("GameManager: Start()");

            // Load the dungeon with new layout (4 rooms, 4 chests, DM, entrance)
            List<TilePos> entrances;
            List<TilePos> chestPositions;
            LevelLoader.LoadDungeon(Board, out entrances, out chestPositions);
            Debug.Log($"GameManager: Dungeon loaded. {chestPositions.Count} chests, {entrances.Count} entrance(s).");

            // Create wave spawner with 9-wave schedule
            waveSpawner = LevelLoader.CreateWaveSchedule(entrances);
            Debug.Log($"GameManager: Wave schedule created. {waveSpawner.TotalWaves} waves scheduled.");

            // Initialize TickSystem with new constructor
            TickSystem = new TickSystem(Board, GameState, ticksPerSecond: 2, waveSpawner);
            TickSystem.OnTick += HandleTick;
            TickSystem.Pause(); // Start paused in placement phase
            Debug.Log("GameManager: TickSystem initialized (2 TPS, paused).");

            // Initialize UI components if assigned
            if (resourceDisplay != null)
            {
                resourceDisplay.Initialize(GameState);
                Debug.Log("GameManager: ResourceDisplay initialized.");
            }

            if (tickControlUI != null)
            {
                tickControlUI.Initialize(TickSystem);
                Debug.Log("GameManager: TickControlUI initialized.");
            }

            // Find the EndGameUI in the scene
            endGameUI = FindObjectOfType<EndGameUI>();
            if (endGameUI == null)
            {
                Debug.LogWarning("GameManager: Could not find an EndGameUI component in the scene.");
            }
        }

        public void StartCombat()
        {
            if (CurrentPhase == GamePhase.Placement)
            {
                CurrentPhase = GamePhase.Combat;
                GameState.Phase = GamePhase.Combat;
                TickSystem.Resume(); // Start ticking - waves will spawn automatically
                Debug.Log("GameManager: StartCombat() - Phase: Combat, waves will spawn automatically");
            }
        }

        public void RestartGame()
        {
            // Reload the bootstrap scene to restart the game
            SceneManager.LoadScene("Bootstrap");
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
            if (CurrentPhase != GamePhase.Combat) return;

            // TickSystem.Tick() is already called by TickDriver - this is just for win condition checks
            var characters = Board.GetAllCharacters().ToList();
            bool dungeonMasterIsAlive = characters.Any(c => c is DungeonMaster && !c.IsDead);
            bool heroesAreAlive = characters.Any(c => c.faction == Faction.Hero && !c.IsDead);

            if (!dungeonMasterIsAlive)
            {
                Debug.Log("Game Over: The Dungeon Master has been defeated! YOU LOSE!");
                if (endGameUI != null)
                {
                    endGameUI.ShowEndGamePopup("You Lose!");
                }
                TickSystem.Pause();
                return;
            }

            // Win condition: All waves spawned AND no living heroes
            if (waveSpawner.AllWavesSpawned && !heroesAreAlive)
            {
                Debug.Log("Game Over: All waves defeated! YOU WIN!");
                if (endGameUI != null)
                {
                    endGameUI.ShowEndGamePopup("You Win!");
                }
                TickSystem.Pause();
                return;
            }
        }
    }
}
