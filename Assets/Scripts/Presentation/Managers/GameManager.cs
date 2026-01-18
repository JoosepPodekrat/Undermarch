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

        public ResourceDisplay resourceDisplay;
        public TickControlUI tickControlUI;

        private WaveSpawner waveSpawner;
        public bool IsSecondStage { get; private set; }
        public bool CanBuild => CurrentPhase == GamePhase.Placement || CurrentPhase == GamePhase.Combat || CurrentPhase == GamePhase.BuildingPhase2;

        [Header("Level Selection")]
        [Tooltip("Registry containing all available levels")]
        public LevelRegistry levelRegistry;

        private int currentLevelIndex = -1;

        /// <summary>
        /// Gets the current level's data including available buildables.
        /// </summary>
        public LevelDataSO CurrentLevelData => 
            levelRegistry != null && currentLevelIndex >= 0 && currentLevelIndex < levelRegistry.LevelCount
                ? levelRegistry.GetLevel(currentLevelIndex)
                : null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            CurrentPhase = GamePhase.Placement;

            Board = new Board(20, 20);
            GameState = new Simulation.Core.GameState(startingGold: 400);
        }

        private void Start()
        {
            // Ensure PlacementController exists
            if (FindFirstObjectByType<PlacementController>() == null)
            {
                var pcObj = new GameObject("PlacementController");
                pcObj.AddComponent<PlacementController>();
            }

            // Ensure HUDController exists
            var hud = FindFirstObjectByType<HUDController>();
            if (hud == null)
            {
                var hudObj = new GameObject("HUDController");
                hudObj.AddComponent<HUDController>();
            }
            else
            {
                hud.SendMessage("BuildHUD", SendMessageOptions.DontRequireReceiver);
            }

            AdjustAllCameras();

            if (gameObject.GetComponent<CameraController>() == null)
            {
                gameObject.AddComponent<CameraController>();
            }

            // Load level
            currentLevelIndex = LevelSelectorUI.SelectedLevelIndex;
            if (levelRegistry != null && currentLevelIndex >= 0 && currentLevelIndex < levelRegistry.LevelCount)
            {
                var levelData = levelRegistry.GetLevel(currentLevelIndex);
                currentLevel = levelData.Load(Board); // LoadLayout is called here
            }
            else
            {
                currentLevel = LevelLoader.LoadLevelOne(Board);
                currentLevelIndex = 1;
            }

            // Create wave spawner using the loaded level (entrances are already populated)
            waveSpawner = currentLevel.CreateWaves(currentLevel);

            // Initialize TickSystem
            TickSystem = new TickSystem(Board, GameState, ticksPerSecond: 2, waveSpawner);
            TickCounter = FindObjectOfType<TickCounter>();
            TickCounter?.Initialize(TickSystem);

            TickSystem.OnTick += HandleTick;
            TickSystem.Pause();

            var driver = FindObjectOfType<Undermarch.Presentation.Bootstrap.TickDriver>();
            if (driver != null)
            {
                InitializeTickDriver(driver);
            }

            resourceDisplay?.Initialize(GameState);
            tickControlUI?.Initialize(TickSystem);

            endGameUI = FindObjectOfType<EndGameUI>();
        }

        private void AdjustAllCameras()
        {
            var cameras = FindObjectsOfType<Camera>();
            if (cameras.Length == 0) return;

            float boardHeight = Board.Height;
            float boardWidth = Board.Width;

            float targetSizeY = boardHeight / 1.4f;
            float screenRatio = (float)Screen.width / Screen.height;
            float targetSizeX = (boardWidth + 2) / (2 * screenRatio);
            float orthoSize = Mathf.Max(targetSizeY, targetSizeX);

            foreach (var cam in cameras)
            {
                cam.transform.position = new Vector3(0, 0, -10);
                cam.orthographic = true;
                cam.orthographicSize = orthoSize;
            }
        }

        public void StartCombat()
        {
            if (CurrentPhase != GamePhase.Placement) return;

            CurrentPhase = GamePhase.Combat;
            GameState.Phase = GamePhase.Combat;
            TickSystem.Resume();
        }

        public void RestartGame() => SceneManager.LoadScene("Bootstrap");
        public void GoToMainMenu()
        {
            LevelSelectorUI.ResetSelection();
            SceneManager.LoadScene("Bootstrap");
        }

        public bool HasNextLevel() => levelRegistry != null && currentLevelIndex + 1 < levelRegistry.LevelCount;

        public void LoadNextLevel()
        {
            if (!HasNextLevel()) return;

            int nextLevelIndex = currentLevelIndex + 1;
            LevelSelectorUI.SetSelectedLevel(nextLevelIndex);
            SceneManager.LoadScene("Bootstrap");
        }

        public void InitializeTickDriver(Bootstrap.TickDriver driver)
        {
            driver?.SetTickSystem(TickSystem);
        }

        private void OnDestroy()
        {
            if (TickSystem != null)
                TickSystem.OnTick -= HandleTick;
        }

        private void HandleTick(int tick)
        {
            if (CurrentPhase != GamePhase.Combat) return;

            var characters = Board.GetAllCharacters().ToList();
            bool dungeonMasterIsAlive = characters.Any(c => c is DungeonMaster && !c.IsDead);
            bool heroesAreAlive = characters.Any(c => c.faction == Faction.Hero && !c.IsDead);

            if (!dungeonMasterIsAlive)
            {
                endGameUI?.ShowEndGamePopup("You Lose!");
                TickSystem.Pause();
                return;
            }

            if (waveSpawner.AllWavesSpawned && !heroesAreAlive)
            {
                if (!IsSecondStage)
                {
                    CurrentPhase = GamePhase.BuildingPhase2;
                    GameState.Phase = GamePhase.BuildingPhase2;
                    TickSystem.Pause();
                    IsSecondStage = true;
                    TickSystem.Reset();
                }
                else
                {
                    LevelProgressManager.MarkLevelCompleted(currentLevelIndex);
                    LevelSelectorUI.ResetSelection();
                    endGameUI?.ShowEndGamePopup("You Win!");
                    TickSystem.Pause();
                }
            }
        }

        public void StartSecondWave()
        {
            if (CurrentPhase != GamePhase.BuildingPhase2)
                return;

            if (currentLevel.CreateSecondWaves == null)
            {
                Debug.LogWarning("This level has no second wave stage.");
                return;
            }

            waveSpawner = currentLevel.CreateSecondWaves(currentLevel);
            TickSystem.SetWaveSpawner(waveSpawner);

            CurrentPhase = GamePhase.Combat;
            GameState.Phase = GamePhase.Combat;
            TickSystem.Resume();

            Debug.Log("Second wave combat started.");
        }


        public void StartNextWave()
        {
            if (CurrentPhase != GamePhase.Combat || TickSystem?.WaveSpawner == null) return;

            if (!TickSystem.WaveSpawner.AllWavesSpawned)
                TickSystem.WaveSpawner.ForceSpawnNextWave(Board);
        }
    }
}
