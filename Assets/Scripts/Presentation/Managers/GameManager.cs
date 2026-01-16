using System.Collections.Generic;
using System.Linq;
using Undermarch.Data;
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
using Undermarch.Presentation.UI.TickCounter;
using Undermarch.Presentation.Controllers;

namespace Undermarch.Presentation.Managers
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public Board Board { get; private set; }
        public TickSystem TickSystem { get; private set; }
        public TickCounter TickCounter { get; private set; }

        public Simulation.Core.GameState GameState { get; private set; }
        private Level currentLevel;

        public GamePhase CurrentPhase { get; private set; }
        public EndGameUI endGameUI;

        // Optional: Assign these in Unity Editor if you want UI displays
        public ResourceDisplay resourceDisplay;
        public TickControlUI tickControlUI;

        private WaveSpawner waveSpawner;
        private List<TilePos> _entrances;
        public bool IsSecondStage { get; private set; }
        public bool CanBuild => CurrentPhase == GamePhase.Placement || CurrentPhase == GamePhase.Combat || CurrentPhase == GamePhase.BuildingPhase2;

        [Header("Level Selection")]
        [Tooltip("Registry containing all available levels")]
        public LevelRegistry levelRegistry;
        
        private int currentLevelIndex = -1;


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
            GameState = new Simulation.Core.GameState(startingGold: 400);

            Debug.Log("GameManager initialized. Board and GameState created.");
        }
        /* possible fix to tickdriver mess
        * var driver = FindObjectOfType<Undermarch.Presentation.Bootstrap.TickDriver>();
           if (driver != null)
           {
               InitializeTickDriver(driver);
               Debug.Log("GameManager: TickDriver connected.");
           }
           else
           {
               Debug.LogError("GameManager: No TickDriver found in scene!");
           }
        */
        private void Start()
        {
            Debug.Log("GameManager: Start()");
            
            // Ensure PlacementController exists
            if (FindObjectOfType<PlacementController>() == null)
            {
                 var pcObj = new GameObject("PlacementController");
                 pcObj.AddComponent<PlacementController>();
            }

            // Ensure UI exists
            var hud = FindObjectOfType<HUDController>();
            if (hud == null)
            {
                Debug.Log("GameManager: Creating HUDController...");
                var hudObj = new GameObject("HUDController");
                hudObj.AddComponent<HUDController>();
            }
            else
            {
                // Force rebuild if it exists but might be empty/broken
                Debug.Log("GameManager: HUDController found. Ensuring HUD...");
                hud.SendMessage("BuildHUD", SendMessageOptions.DontRequireReceiver);
            }

            // Setup Camera
            AdjustAllCameras();

            // Add Camera Controller to GameManager (persistent object)
            if (gameObject.GetComponent<CameraController>() == null)
            {
                gameObject.AddComponent<CameraController>();
                Debug.Log("GameManager: CameraController added to GameManager.");
            }

            // Create Background
            // CreateBackground();

            // Load the selected level from the registry
            currentLevelIndex = LevelSelectorUI.SelectedLevelIndex;
            if (levelRegistry != null && currentLevelIndex >= 0 && currentLevelIndex < levelRegistry.LevelCount)
            {
                var levelData = levelRegistry.GetLevel(currentLevelIndex);
                currentLevel = levelData.Load(Board);
                Debug.Log($"GameManager: Loaded level '{levelData.displayName}' (index {currentLevelIndex}).");
            }
            else
            {
                // Fallback to level one if no valid selection
                Debug.LogWarning("GameManager: No valid level selected, falling back to Level One.");
                currentLevel = LevelLoader.LoadLevelOne(Board);
                currentLevelIndex = 1;
            }

            _entrances = currentLevel.Entrances;

            Debug.Log($"GameManager: Dungeon loaded. " +
                      $"{currentLevel.ChestPositions.Count} chests, " +
                      $"{_entrances.Count} entrance(s).");


            // Create wave spawner with 9-wave schedule
            waveSpawner = LevelLoader.CreateWaveSchedule(_entrances);
            Debug.Log($"GameManager: Wave schedule created. {waveSpawner.TotalWaves} waves scheduled.");

            // Initialize TickSystem with new constructor
            TickSystem = new TickSystem(Board, GameState, ticksPerSecond: 2, waveSpawner);
            TickCounter = FindObjectOfType<TickCounter>();   // ← FIX
            if (TickCounter == null)
            {
                Debug.LogError("GameManager: No TickCounter found in scene!");
            }
            else
            {
                TickCounter.Initialize(TickSystem);
            }
            TickSystem.OnTick += HandleTick;
            TickSystem.Pause(); // Start paused in placement phase
            Debug.Log("GameManager: TickSystem initialized (2 TPS, paused).");
            var driver = FindObjectOfType<Undermarch.Presentation.Bootstrap.TickDriver>();
            if (driver != null)
            {
                InitializeTickDriver(driver);
                Debug.Log("GameManager: TickDriver connected.");
            }
            else
            {
                Debug.LogError("GameManager: No TickDriver found in scene!");
            }

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

        private void CreateBackground()
        {
             GameObject bg = new GameObject("Background");
             bg.transform.position = new Vector3(0, 0, 10); // Behind board
             var sr = bg.AddComponent<SpriteRenderer>();
             
             // Create a simple texture
             Texture2D tex = new Texture2D(1, 1);
             tex.SetPixel(0, 0, new Color(0.15f, 0.15f, 0.2f)); // Dark Blue/Grey
             tex.Apply();
             
             sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
             bg.transform.localScale = new Vector3(100, 100, 1);
        }

        private void LateUpdate()
        {
            // Continuously adjust camera to handle screen resize or scene loads
            // AdjustAllCameras(); // Disabled to allow manual camera control
        }

        private void AdjustAllCameras()
        {
            var cameras = FindObjectsOfType<Camera>();
            if (cameras.Length == 0) return;

            float boardHeight = Board.Height;
            float boardWidth = Board.Width;
            
            // Reserve ~30% vertical space for UI (Top+Bottom)
            float targetSizeY = boardHeight / 1.4f;
            
            // Check aspect ratio for width
            float screenRatio = (float)Screen.width / Screen.height;
            float targetSizeX = (boardWidth + 2) / (2 * screenRatio);
            
            float orthoSize = Mathf.Max(targetSizeY, targetSizeX);

            foreach (var cam in cameras)
            {
                // Skip if it's a specific camera we shouldn't touch? 
                // User said "literally all".
                
                if (cam.transform.position.z != -10 || cam.orthographicSize != orthoSize)
                {
                    cam.transform.position = new Vector3(0, 0, -10);
                    cam.orthographic = true;
                    cam.orthographicSize = orthoSize;
                }
            }
        }

        public void StartCombat()
{
    if (CurrentPhase == GamePhase.Placement)
    {
        CurrentPhase = GamePhase.Combat;
        GameState.Phase = GamePhase.Combat;
        TickSystem.Resume();
        Debug.Log("Combat started. You can now build during combat.");
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
                if (!IsSecondStage)
                {
                    Debug.Log("First stage cleared! Entering Building Phase 2.");
                    CurrentPhase = GamePhase.BuildingPhase2;
                    GameState.Phase = GamePhase.BuildingPhase2;
                    TickSystem.Pause();
                    IsSecondStage = true;
                    TickSystem.Reset();
                }
                else
                {
                    Debug.Log("Game Over: All waves defeated! YOU WIN!");
                    
                    // Mark level as completed for progression
                    if (currentLevelIndex >= 0)
                    {
                        LevelProgressManager.MarkLevelCompleted(currentLevelIndex);
                    }
                    
                    // Reset selection so returning to menu works correctly
                    LevelSelectorUI.ResetSelection();
                    
                    if (endGameUI != null)
                    {
                        endGameUI.ShowEndGamePopup("You Win!");
                    }
                    TickSystem.Pause();
                }
                return;
            }
        }

        public void StartSecondWave()
        {
            if (CurrentPhase == GamePhase.BuildingPhase2)
            {
                waveSpawner = LevelLoader.CreateWaveSchedule2(_entrances);
                TickSystem.SetWaveSpawner(waveSpawner);

                CurrentPhase = GamePhase.Combat;
                GameState.Phase = GamePhase.Combat;
                TickSystem.Resume();
                Debug.Log("Second Wave Combat started.");
            }
        }

        /// <summary>
        /// Manually trigger the next wave spawn during combat phase.
        /// </summary>
        public void StartNextWave()
        {
            if (CurrentPhase != GamePhase.Combat)
            {
                Debug.LogWarning("GameManager: StartNextWave called outside Combat phase!");
                return;
            }

            if (TickSystem?.WaveSpawner == null)
            {
                Debug.LogError("GameManager: WaveSpawner is null!");
                return;
            }

            if (TickSystem.WaveSpawner.AllWavesSpawned)
            {
                Debug.Log("GameManager: All waves already spawned!");
                return;
            }

            // Force spawn the next wave immediately
            TickSystem.WaveSpawner.ForceSpawnNextWave(Board);
        }
    }
}
