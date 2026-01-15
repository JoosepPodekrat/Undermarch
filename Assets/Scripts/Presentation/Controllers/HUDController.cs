using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Undermarch.Presentation.Managers;
using Undermarch.Simulation.Interfaces;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Grid;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

namespace Undermarch.Presentation.Controllers
{
    public class HUDController : MonoBehaviour
    {
        [Header("Top HUD")]
        public TextMeshProUGUI coinText;
        public TextMeshProUGUI turnIndicatorText;
        public TextMeshProUGUI waveText;
        public TextMeshProUGUI enemiesText;
        public Image turnIndicatorImage;
        public Button settingsButton;
        public Button levelSelectButton;
        public Button startCombatButton;
        public TextMeshProUGUI startButtonText;
        public Button pauseButton;

        [Header("Bottom HUD")]
        public Button buildSlimeButton;
        public TextMeshProUGUI slimePriceText;
        public Button buildTrapButton;
        public TextMeshProUGUI trapPriceText;
        public Button buildGoblinButton;
        public TextMeshProUGUI goblinPriceText;
        public Button buildBearTrapButton;
        public TextMeshProUGUI bearTrapPriceText;

        [Header("References")]
        public PlacementController placementController;

        [Header("Character Info Panel")]
        private GameObject characterInfoPanel;
        private TextMeshProUGUI charNameText;
        private TextMeshProUGUI charHPText;
        private TextMeshProUGUI charDamageText;
        private TextMeshProUGUI charAgilityText;
        private TextMeshProUGUI charFactionText;
        private Undermarch.Simulation.Entities.Character selectedCharacter;

        [Header("Pause Overlay")]
        private GameObject pauseOverlay;
        private TextMeshProUGUI pauseText;

        private void Awake()
        {
            Debug.Log("HUDController: Awake started.");

            // 1. EventSystem Check
            var eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                Debug.Log("HUDController: No EventSystem found. Creating one.");
                var eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<InputSystemUIInputModule>();
            }
            else
            {
                Debug.Log($"HUDController: Found existing EventSystem on {eventSystem.gameObject.name}");
            }

            // 2. HUD Structure Check
            // Ensure GraphicRaycaster exists regardless of rebuild state
            if (GetComponent<GraphicRaycaster>() == null)
            {
                Debug.Log("HUDController: GraphicRaycaster missing. Adding one.");
                gameObject.AddComponent<GraphicRaycaster>();
            }

            // Always rebuild HUD to ensure proper raycastTarget settings on background images
            Debug.Log("HUDController: Rebuilding HUD structure...");
            BuildHUD();

            // 3. Reference Assignment
            RefreshReferences();
        }

        private void RefreshReferences()
        {
            Debug.Log("HUDController: Refreshing references...");

            // Helper closure for logging
            T FindAndLog<T>(string path, ref T field, string fieldName) where T : Component
            {
                if (field != null) 
                {
                    Debug.Log($"HUDController: {fieldName} was already assigned.");
                    return field;
                }
                
                var found = transform.Find(path)?.GetComponent<T>();
                if (found != null) Debug.Log($"HUDController: Found {fieldName} at '{path}'.");
                else Debug.LogError($"HUDController: FAILED to find {fieldName} at '{path}'!");
                
                field = found;
                return found;
            }

            FindAndLog("TopHUD/MiddlePanel/TurnIndicatorText", ref turnIndicatorText, "turnIndicatorText");
            FindAndLog("TopHUD/MiddlePanel/StatsRow/WaveText", ref waveText, "waveText");
            FindAndLog("TopHUD/MiddlePanel/StatsRow/EnemiesText", ref enemiesText, "enemiesText");
            FindAndLog("TopHUD/MiddlePanel/StatsRow/CoinText", ref coinText, "coinText");
            FindAndLog("TopHUD/SettingsButton", ref settingsButton, "settingsButton");
            FindAndLog("TopHUD/LevelSelectButton", ref levelSelectButton, "levelSelectButton");
            FindAndLog("TopHUD/StartWaveButton", ref startCombatButton, "startCombatButton");
            FindAndLog("TopHUD/PauseButton", ref pauseButton, "pauseButton");

            // Get button text component
            if (startCombatButton != null && startButtonText == null)
            {
                startButtonText = startCombatButton.GetComponentInChildren<TextMeshProUGUI>();
                if (startButtonText == null)
                    Debug.LogError("HUDController: Could not find StartButton Text child!");
            }

            FindAndLog("BottomHUD/PlaceSlimeButton", ref buildSlimeButton, "buildSlimeButton");
            FindAndLog("BottomHUD/PlaceTrapButton", ref buildTrapButton, "buildTrapButton");
            FindAndLog("BottomHUD/PlaceGoblinButton", ref buildGoblinButton, "buildGoblinButton");
            FindAndLog("BottomHUD/PlaceBearTrapButton", ref buildBearTrapButton, "buildBearTrapButton");

            if (buildSlimeButton != null && slimePriceText == null)
            {
                slimePriceText = buildSlimeButton.transform.Find("SlimePriceText")?.GetComponent<TextMeshProUGUI>();
                if(slimePriceText == null) Debug.LogError("HUDController: Could not find SlimePriceText child!");
            }

            if (buildTrapButton != null && trapPriceText == null)
            {
                trapPriceText = buildTrapButton.transform.Find("TrapPriceText")?.GetComponent<TextMeshProUGUI>();
                if(trapPriceText == null) Debug.LogError("HUDController: Could not find TrapPriceText child!");
            }

            if (buildGoblinButton != null && goblinPriceText == null)
            {
                goblinPriceText = buildGoblinButton.transform.Find("GoblinPriceText")?.GetComponent<TextMeshProUGUI>();
                if(goblinPriceText == null) Debug.LogError("HUDController: Could not find GoblinPriceText child!");
            }

            if (buildBearTrapButton != null && bearTrapPriceText == null)
            {
                bearTrapPriceText = buildBearTrapButton.transform.Find("BearTrapPriceText")?.GetComponent<TextMeshProUGUI>();
                if(bearTrapPriceText == null) Debug.LogError("HUDController: Could not find BearTrapPriceText child!");
            }

            if (placementController == null)
            {
                placementController = FindObjectOfType<PlacementController>();
                Debug.Log($"HUDController: PlacementController found: {placementController != null}");
            }
        }

        private void Start()
        {
            Debug.Log("HUDController: Start() called.");

            // 4. Listener Binding
            BindButton(settingsButton, OnSettingsClicked, "SettingsButton");
            BindButton(levelSelectButton, OnLevelSelectClicked, "LevelSelectButton");
            BindButton(startCombatButton, OnStartCombatClicked, "StartCombatButton");
            BindButton(pauseButton, OnPauseClicked, "PauseButton");
            BindButton(buildSlimeButton, OnBuildSlimeClicked, "BuildSlimeButton");
            BindButton(buildTrapButton, OnBuildTrapClicked, "BuildTrapButton");
            BindButton(buildGoblinButton, OnBuildGoblinClicked, "BuildGoblinButton");
            BindButton(buildBearTrapButton, OnBuildBearTrapClicked, "BuildBearTrapButton");

            // 5. Logic Init
            if (GameManager.Instance != null && GameManager.Instance.GameState != null)
            {
                var gameState = GameManager.Instance.GameState as GameState;
                if (gameState != null)
                {
                    Debug.Log("HUDController: Connecting to GameState events.");
                    if (slimePriceText) slimePriceText.text = $"{gameState.PlacementCosts["SlimeMonster"]} G";
                    if (trapPriceText) trapPriceText.text = $"{gameState.PlacementCosts["SpikeTrap"]} G";
                    if (goblinPriceText) goblinPriceText.text = $"{gameState.PlacementCosts["Goblin"]} G";
                    if (bearTrapPriceText) bearTrapPriceText.text = $"{gameState.PlacementCosts["BearTrap"]} G";
                    GameManager.Instance.GameState.OnResourcesChanged += UpdateResourceDisplay;
                }
            }
            else
            {
                Debug.LogWarning("HUDController: GameManager or GameState is null in Start.");
            }

            UpdateResourceDisplay();
            UpdateTurnIndicator();
        }

        private void BindButton(Button btn, UnityEngine.Events.UnityAction action, string debugName)
        {
            if (btn == null)
            {
                Debug.LogError($"HUDController: Cannot bind {debugName} - Button reference is NULL!");
                return;
            }

            // Check if interactable
            Debug.Log($"HUDController: Binding {debugName}... (Interactable: {btn.interactable}, Active: {btn.gameObject.activeInHierarchy})");

            btn.onClick.RemoveListener(action);
            btn.onClick.AddListener(action);
            
            // Verify (We can't easily check listener count via API, but we can assume success if no exception)
            Debug.Log($"HUDController: Successfully added listener to {debugName}.");
        }

        private void Update()
        {
            // Check for spacebar pause toggle
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                TogglePause();
            }

            // Update pause overlay visibility
            UpdatePauseOverlay();

            UpdateTurnIndicator();
            UpdateWaveInfo();

            if (GameManager.Instance != null)
            {
                bool isPhase2 = GameManager.Instance.CurrentPhase == GamePhase.BuildingPhase2 || GameManager.Instance.IsSecondStage;

                UpdateButtonState(buildGoblinButton, isPhase2, "Goblin", goblinPriceText);
                UpdateButtonState(buildBearTrapButton, isPhase2, "Bear Trap", bearTrapPriceText);
            }

            // NOTE: Old debug code commented out - it was interfering with click detection
            // // DEBUG: Check for clicks and UI hits
            // if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            // {
            //     bool isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
            //     Debug.Log($"DEBUG_INPUT: Click detected. Pointer over UI: {isPointerOverUI}");
            //
            //     if (isPointerOverUI)
            //     {
            //         var selected = EventSystem.current.currentSelectedGameObject;
            //         Debug.Log($"DEBUG_INPUT: Current Selected GameObject: {(selected != null ? selected.name : "null")}");
            //
            //         // Raycast check manually to see what we are hitting
            //         var pointerData = new PointerEventData(EventSystem.current)
            //         {
            //             position = UnityEngine.InputSystem.Mouse.current.position.ReadValue()
            //         };
            //         var results = new System.Collections.Generic.List<RaycastResult>();
            //         EventSystem.current.RaycastAll(pointerData, results);
            //         foreach(var result in results)
            //         {
            //             Debug.Log($"DEBUG_INPUT: Raycast Hit: {result.gameObject.name} (Layer: {LayerMask.LayerToName(result.gameObject.layer)})");
            //         }
            //     }
            // }

            // Character info panel handling
            HandleCharacterInfoPanel();
        }

        private void UpdateButtonState(Button btn, bool unlocked, string name, TextMeshProUGUI priceText)
        {
            if (btn == null) return;
            
            btn.gameObject.SetActive(true);
            btn.interactable = unlocked;

            var nameText = btn.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = unlocked ? name : "LOCKED";
                nameText.color = unlocked ? Color.white : Color.gray;
            }
            
            if (priceText != null)
            {
                priceText.gameObject.SetActive(unlocked);
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null && GameManager.Instance.GameState != null)
            {
                GameManager.Instance.GameState.OnResourcesChanged -= UpdateResourceDisplay;
            }
        }

        // ... [Rest of the methods with added logs] ...

        private void UpdateResourceDisplay()
        {
            if (GameManager.Instance != null && coinText != null)
            {
                coinText.text = $"Coins: {GameManager.Instance.GameState.CurrentGold}";
            }
        }

        private void UpdateTurnIndicator()
        {
            if (GameManager.Instance == null) return;

            var phase = GameManager.Instance.GameState.Phase;
            string statusText = "";
            Color statusColor = Color.white;

            switch (phase)
            {
                case GamePhase.Placement:
                    statusText = "BUILDING PHASE";
                    statusColor = Color.green;
                    if (startCombatButton) startCombatButton.interactable = true;
                    if (startButtonText) startButtonText.text = "START";
                    break;
                case GamePhase.Combat:
                    statusText = "ENEMY WAVE";
                    statusColor = Color.red;

                    // Enable "Next Wave" button if waves remain
                    var waveSpawner = GameManager.Instance.TickSystem?.WaveSpawner;
                    bool hasMoreWaves = waveSpawner != null && !waveSpawner.AllWavesSpawned;

                    if (startCombatButton) startCombatButton.interactable = hasMoreWaves;
                    if (startButtonText)
                    {
                        startButtonText.text = hasMoreWaves ? "NEXT WAVE" : "WAITING";
                    }
                    break;
                case GamePhase.BuildingPhase2:
                    statusText = "BUILDING PHASE 2";
                    statusColor = Color.cyan;
                    if (startCombatButton) startCombatButton.interactable = true;
                    if (startButtonText) startButtonText.text = "START";
                    break;
                case GamePhase.GameOver:
                    statusText = "GAME OVER";
                    statusColor = Color.gray;
                    if (startCombatButton) startCombatButton.interactable = false;
                    break;
            }

            if (turnIndicatorText) turnIndicatorText.text = statusText;
            if (turnIndicatorImage) turnIndicatorImage.color = statusColor;
        }

        private void UpdateWaveInfo()
        {
            if (GameManager.Instance == null) return;

            // Update wave display
            if (waveText != null)
            {
                var waveSpawner = GameManager.Instance.TickSystem?.WaveSpawner;
                if (waveSpawner != null)
                {
                    // CurrentWaveIndex is 0-based, increments after each wave spawns
                    // Use max(1, index) so we show "1" before any waves spawn, and "1" after wave 1 spawns
                    int currentWave = Mathf.Max(1, waveSpawner.CurrentWave);
                    waveText.text = $"Wave: {currentWave}/{waveSpawner.TotalWaves}";
                }
                else
                {
                    waveText.text = "Wave: 1/9";
                }
            }

            // Update enemy count display
            if (enemiesText != null && GameManager.Instance.Board != null)
            {
                var allCharacters = GameManager.Instance.Board.GetAllCharacters();
                int heroCount = allCharacters.Count(c => c.faction == Undermarch.Simulation.Combat.Faction.Hero);
                enemiesText.text = $"Enemies: {heroCount}";
            }
        }

        private void OnSettingsClicked() { Debug.Log("HUD_CLICK: Settings Clicked"); }
        private void OnLevelSelectClicked() { Debug.Log("HUD_CLICK: Level Select Clicked"); }
        
        private void OnStartCombatClicked()
        {
            Debug.Log("HUD_CLICK: Start Combat Clicked");
            if (GameManager.Instance)
            {
                if (GameManager.Instance.CurrentPhase == GamePhase.BuildingPhase2)
                {
                    GameManager.Instance.StartSecondWave();
                }
                else if (GameManager.Instance.CurrentPhase == GamePhase.Combat)
                {
                    // Start next wave early
                    GameManager.Instance.StartNextWave();
                }
                else
                {
                    GameManager.Instance.StartCombat();
                }
            }
        }

        private void TogglePause()
        {
            var tickSystem = GameManager.Instance?.TickSystem;
            if (tickSystem != null)
            {
                if (tickSystem.Mode == TickMode.Paused)
                    tickSystem.Resume();
                else
                    tickSystem.Pause();
            }
        }

        private void OnPauseClicked()
        {
            Debug.Log("HUD_CLICK: Pause Clicked");
            TogglePause();
        }

        private void OnBuildSlimeClicked()
        {
            Debug.Log("HUD_CLICK: Build Slime Clicked");
            if (placementController) 
            {
                placementController.SelectSlime();
                Debug.Log("HUD_CLICK: Called SelectSlime on PlacementController");
            }
            else Debug.LogError("HUD_CLICK: PlacementController is NULL!");
        }

        private void OnBuildTrapClicked()
        {
            Debug.Log("HUD_CLICK: Build Trap Clicked");
            if (placementController) 
            {
                placementController.SelectSpikeTrap();
                Debug.Log("HUD_CLICK: Called SelectSpikeTrap on PlacementController");
            }
            else Debug.LogError("HUD_CLICK: PlacementController is NULL!");
        }

        private void OnBuildGoblinClicked()
        {
            Debug.Log("HUD_CLICK: Build Goblin Clicked");
            if (placementController) 
            {
                placementController.SelectGoblin();
                Debug.Log("HUD_CLICK: Called SelectGoblin on PlacementController");
            }
            else Debug.LogError("HUD_CLICK: PlacementController is NULL!");
        }

        private void OnBuildBearTrapClicked()
        {
            Debug.Log("HUD_CLICK: Build Bear Trap Clicked");
            if (placementController) 
            {
                placementController.SelectBearTrap();
                Debug.Log("HUD_CLICK: Called SelectBearTrap on PlacementController");
            }
            else Debug.LogError("HUD_CLICK: PlacementController is NULL!");
        }

        public void BuildHUD()
        {
            Debug.Log("HUDController: Building HUD elements from scratch...");
            
            // Ensure Canvas settings
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; 

            CanvasScaler scaler = GetComponent<CanvasScaler>();
            if (scaler == null) scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            if (GetComponent<GraphicRaycaster>() == null) gameObject.AddComponent<GraphicRaycaster>();

            // Clear existing
            Transform top = transform.Find("TopHUD");
            if (top != null) DestroyImmediate(top.gameObject); // Immediate for cleaner rebuild logic
            Transform bottom = transform.Find("BottomHUD");
            if (bottom != null) DestroyImmediate(bottom.gameObject);

            SetupHUD(gameObject);

            // Reset references to ensure they are re-acquired from the new objects
            coinText = null;
            turnIndicatorText = null;
            waveText = null;
            enemiesText = null;
            settingsButton = null;
            levelSelectButton = null;
            startCombatButton = null;
            pauseButton = null;
            buildSlimeButton = null;
            buildTrapButton = null;
            slimePriceText = null;
            trapPriceText = null;

            RefreshReferences();

            Debug.Log("HUDController: BuildHUD complete.");
        }

        void SetupHUD(GameObject canvasObj)
        {
            // 2. Top HUD
            GameObject topPanel = CreatePanel(canvasObj.transform, "TopHUD", new Color(0.18f, 0.18f, 0.18f, 1f), 110, true);
            SetupHorizontalLayout(topPanel, 20, 15, TextAnchor.MiddleCenter);

            // Top Elements
            CreateButton(topPanel.transform, "SettingsButton", "Set", new Vector2(60, 60));
            CreateButton(topPanel.transform, "LevelSelectButton", "Lvl", new Vector2(60, 60));

            // === Middle Section - Simple RectTransform positioning ===
            GameObject middlePanel = new GameObject("MiddlePanel", typeof(RectTransform));
            middlePanel.transform.SetParent(topPanel.transform, false);

            RectTransform middleRT = middlePanel.GetComponent<RectTransform>();
            middleRT.sizeDelta = new Vector2(600, 65);  // Wider for phase text

            // Phase text at top-center
            CreateText(middlePanel.transform, "TurnIndicatorText", "BUILDING PHASE", 20, Color.white, true);  // Reduced font
            RectTransform phaseTextRT = middlePanel.transform.Find("TurnIndicatorText") as RectTransform;
            if (phaseTextRT != null)
            {
                phaseTextRT.anchorMin = new Vector2(0.5f, 1f);
                phaseTextRT.anchorMax = new Vector2(0.5f, 1f);
                phaseTextRT.pivot = new Vector2(0.5f, 1f);
                phaseTextRT.anchoredPosition = new Vector2(0, 4);  // Slightly down from top
            }
            var tmp = middlePanel.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.enableAutoSizing = false;
                tmp.overflowMode = TextOverflowModes.Overflow;
            }

            // Stats row - MANUALLY POSITION each text to avoid LayoutGroup gap issues
            GameObject statsRow = new GameObject("StatsRow", typeof(RectTransform));
            statsRow.transform.SetParent(middlePanel.transform, false);
            RectTransform statsRT = statsRow.GetComponent<RectTransform>();
            statsRT.sizeDelta = new Vector2(600, 25);
            statsRT.anchorMin = new Vector2(0.5f, 0f);
            statsRT.anchorMax = new Vector2(0.5f, 0f);
            statsRT.pivot = new Vector2(0.5f, 0f);
            statsRT.anchoredPosition = new Vector2(0, 0);  // At bottom edge

            // Manually position each stat text for tight spacing
            CreateText(statsRow.transform, "WaveText", "Wave: 1/9", 14, Color.cyan, false);
            CreateText(statsRow.transform, "EnemiesText", "Enemies: 0", 14, Color.red, false);
            CreateText(statsRow.transform, "CoinText", "Coins: 0", 14, Color.yellow, false);

            // Position each text manually
            RectTransform waveRT = statsRow.transform.Find("WaveText") as RectTransform;
            RectTransform enemiesRT = statsRow.transform.Find("EnemiesText") as RectTransform;
            RectTransform coinRT = statsRow.transform.Find("CoinText") as RectTransform;

            if (waveRT != null)
            {
                waveRT.anchorMin = new Vector2(0.5f, 0.5f);
                waveRT.anchorMax = new Vector2(0.5f, 0.5f);
                waveRT.pivot = new Vector2(0.5f, 0.5f);
                waveRT.anchoredPosition = new Vector2(-150, 0);  // Left of center
            }
            if (enemiesRT != null)
            {
                enemiesRT.anchorMin = new Vector2(0.5f, 0.5f);
                enemiesRT.anchorMax = new Vector2(0.5f, 0.5f);
                enemiesRT.pivot = new Vector2(0.5f, 0.5f);
                enemiesRT.anchoredPosition = new Vector2(0, 0);  // Center
            }
            if (coinRT != null)
            {
                coinRT.anchorMin = new Vector2(0.5f, 0.5f);
                coinRT.anchorMax = new Vector2(0.5f, 0.5f);
                coinRT.pivot = new Vector2(0.5f, 0.5f);
                coinRT.anchoredPosition = new Vector2(150, 0);  // Right of center
            }

            // === Right Buttons ===
            CreateButton(topPanel.transform, "StartWaveButton", "START", new Vector2(120, 60), new Color(0.3f, 0.7f, 0.3f));
            CreateButton(topPanel.transform, "PauseButton", "||", new Vector2(60, 60));

            // 3. Bottom HUD
            GameObject bottomPanel = CreatePanel(canvasObj.transform, "BottomHUD", new Color(0.18f, 0.18f, 0.18f, 1f), 140, false);
            SetupHorizontalLayout(bottomPanel, 20, 40, TextAnchor.MiddleCenter);

            // Bottom Elements
            CreateBuildButton(bottomPanel.transform, "PlaceSlimeButton", "Slime", "50 G");
            CreateBuildButton(bottomPanel.transform, "PlaceTrapButton", "Trap", "30 G");
            CreateBuildButton(bottomPanel.transform, "PlaceGoblinButton", "Goblin", "50 G");
            CreateBuildButton(bottomPanel.transform, "PlaceBearTrapButton", "Bear Trap", "50 G");

            // 4. Character Info Panel (right side, initially hidden)
            CreateCharacterInfoPanel(canvasObj.transform);

            // 5. Pause Overlay (initially hidden)
            CreatePauseOverlay(canvasObj.transform);
        }

        GameObject CreatePanel(Transform parent, string name, Color color, float height, bool isTop)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(parent, false);

            Image img = obj.GetComponent<Image>();
            img.color = color;
            img.raycastTarget = false; // Don't block clicks to game world

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = isTop ? new Vector2(0, 1) : new Vector2(0, 0);
            rt.anchorMax = isTop ? new Vector2(1, 1) : new Vector2(1, 0);
            rt.pivot = isTop ? new Vector2(0.5f, 1) : new Vector2(0.5f, 0);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(0, height);

            return obj;
        }

        void SetupHorizontalLayout(GameObject obj, int padding, int spacing, TextAnchor alignment)
        {
            var group = obj.AddComponent<HorizontalLayoutGroup>();
            group.padding = new RectOffset(padding, padding, 12, 12);
            group.spacing = spacing;
            group.childAlignment = alignment;
            group.childControlWidth = false;
            group.childControlHeight = false;
            group.childForceExpandWidth = false;
            group.childForceExpandHeight = false;
        }

        GameObject CreateButton(Transform parent, string name, string text, Vector2 size, Color? color = null)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            obj.transform.SetParent(parent, false);

            Image img = obj.GetComponent<Image>();
            img.color = color ?? new Color(0.1f, 0.1f, 0.1f);
            img.raycastTarget = true; // Ensure buttons are clickable

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.sizeDelta = size;

            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(obj.transform, false);
            
            RectTransform textRT = textObj.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = Vector2.zero;
            textRT.offsetMax = Vector2.zero;

            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 20;
            tmp.color = Color.white;

            return obj;
        }

        void CreateBuildButton(Transform parent, string name, string label, string price)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            obj.transform.SetParent(parent, false);

            Image img = obj.GetComponent<Image>();
            img.color = new Color(0.1f, 0.1f, 0.1f);
            img.raycastTarget = true; // Ensure buttons are clickable

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(100, 100);

            var vGroup = obj.AddComponent<VerticalLayoutGroup>();
            vGroup.childAlignment = TextAnchor.MiddleCenter;
            vGroup.padding = new RectOffset(5, 5, 5, 5);

            CreateText(obj.transform, "NameText", label, 18, Color.white, false);
            
            string priceTextName = "PriceText";
            if (name == "PlaceSlimeButton") priceTextName = "SlimePriceText";
            else if (name == "PlaceTrapButton") priceTextName = "TrapPriceText";
            else if (name == "PlaceGoblinButton") priceTextName = "GoblinPriceText";
            else if (name == "PlaceBearTrapButton") priceTextName = "BearTrapPriceText";

            CreateText(obj.transform, priceTextName, price, 16, Color.yellow, false);
            
            Debug.Log($"HUDController: Created BuildButton '{name}' under {parent.name}");
        }

        void CreateText(Transform parent, string name, string content, float fontSize, Color color, bool bold)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            obj.transform.SetParent(parent, false);
            
            TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
            // FIX: Assign a default font to prevent exceptions if no default is set in the project.
            tmp.font = Resources.GetBuiltinResource<TMP_FontAsset>("LegacyRuntime.ttf");
            tmp.text = content;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            if (bold) tmp.fontStyle = FontStyles.Bold;
        }

        void CreateSpacer(Transform parent)
        {
            GameObject obj = new GameObject("Spacer", typeof(RectTransform), typeof(LayoutElement));
            obj.transform.SetParent(parent, false);
            LayoutElement le = obj.GetComponent<LayoutElement>();
            le.flexibleWidth = 1;
        }

        GameObject CreateContainer(Transform parent, string name)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);
            return obj;
        }

        #region Character Info Panel

        void CreateCharacterInfoPanel(Transform canvasTransform)
        {
            // Create panel background
            characterInfoPanel = new GameObject("CharacterInfoPanel", typeof(RectTransform), typeof(Image));
            characterInfoPanel.transform.SetParent(canvasTransform, false);

            RectTransform panelRect = characterInfoPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(1, 0.5f);  // Right-center anchor
            panelRect.anchorMax = new Vector2(1, 0.5f);
            panelRect.pivot = new Vector2(1, 0.5f);
            panelRect.anchoredPosition = new Vector2(-10, 0);  // 10px from right edge
            panelRect.sizeDelta = new Vector2(250, 200);

            Image panelImage = characterInfoPanel.GetComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
            panelImage.raycastTarget = true; // Allow detecting clicks inside panel

            // Add vertical layout for content
            var layoutGroup = characterInfoPanel.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            layoutGroup.spacing = 5;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;

            // Create text elements
            charNameText = CreateInfoText(characterInfoPanel.transform, "NameText", "Name", 18, Color.white, true);
            charHPText = CreateInfoText(characterInfoPanel.transform, "HPText", "HP: 0/0", 14, Color.red, false);
            charDamageText = CreateInfoText(characterInfoPanel.transform, "DamageText", "Damage: 0", 14, Color.yellow, false);
            charAgilityText = CreateInfoText(characterInfoPanel.transform, "AgilityText", "Agility: 0", 14, Color.cyan, false);
            charFactionText = CreateInfoText(characterInfoPanel.transform, "FactionText", "Faction: None", 14, Color.gray, false);

            // Start hidden
            characterInfoPanel.SetActive(false);

            Debug.Log("HUDController: Created character info panel.");
        }

        TextMeshProUGUI CreateInfoText(Transform parent, string name, string content, float fontSize, Color color, bool bold)
        {
            GameObject textObj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(parent, false);

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(230, 25);

            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Left;
            if (bold) tmp.fontStyle = FontStyles.Bold;

            return tmp;
        }

        void CreatePauseOverlay(Transform canvasTransform)
        {
            // Create container for pause text
            pauseOverlay = new GameObject("PauseOverlay", typeof(RectTransform), typeof(CanvasGroup));
            pauseOverlay.transform.SetParent(canvasTransform, false);
            pauseOverlay.SetActive(false); // Start hidden

            RectTransform overlayRect = pauseOverlay.GetComponent<RectTransform>();
            overlayRect.anchorMin = new Vector2(0.5f, 0.5f);  // Center anchors
            overlayRect.anchorMax = new Vector2(0.5f, 0.5f);
            overlayRect.pivot = new Vector2(0.5f, 0.5f);
            overlayRect.anchoredPosition = new Vector2(0, 380);  // Offset from center (near top edge)
            overlayRect.sizeDelta = new Vector2(300, 80);

            // Add semi-transparent background
            Image bg = pauseOverlay.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.5f);
            bg.raycastTarget = false; // Don't block clicks

            // Create text
            GameObject textObj = new GameObject("PauseText", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(pauseOverlay.transform, false);

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            pauseText = textObj.GetComponent<TextMeshProUGUI>();
            pauseText.text = "PAUSED";
            pauseText.fontSize = 48;
            pauseText.fontStyle = FontStyles.Bold;
            pauseText.alignment = TextAlignmentOptions.Center;
            pauseText.color = Color.white;

            // Get TMP font (same as other text)
            pauseText.font = Resources.GetBuiltinResource<TMP_FontAsset>("LegacyRuntime.ttf");

            Debug.Log("HUDController: Created pause overlay.");
        }

        private void UpdatePauseOverlay()
        {
            if (pauseOverlay == null) return;

            bool isPaused = GameManager.Instance?.TickSystem?.Mode == TickMode.Paused;
            pauseOverlay.SetActive(isPaused);
        }

        void HandleCharacterInfoPanel()
        {
            // Escape to close
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Debug.Log("CharacterInfo: Escape pressed, closing panel");
                CloseCharacterInfo();
                return;
            }

            // Left click handling
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Check if click is inside our own panel (close it if so)
                if (characterInfoPanel != null && characterInfoPanel.activeSelf)
                {
                    RectTransform panelRect = characterInfoPanel.GetComponent<RectTransform>();
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                    if (RectTransformUtility.RectangleContainsScreenPoint(panelRect, mousePos))
                    {
                        Debug.Log("CharacterInfo: Click inside panel, closing");
                        CloseCharacterInfo();
                        return;
                    }
                }

                // Try to select character at mouse position
                // Do this regardless of UI - Unity's button onClick will handle button clicks separately
                Character clicked = GetEntityAtMousePosition();
                Debug.Log($"CharacterInfo: Click resulted in entity: {clicked?.Name ?? "null"}");

                if (clicked != null)
                {
                    ShowCharacterInfo(clicked);
                }
                else
                {
                    // Clicked on empty space - close panel
                    CloseCharacterInfo();
                }
            }
        }

        void ShowCharacterInfo(Character character)
        {
            if (character == null || characterInfoPanel == null) return;

            selectedCharacter = character;
            characterInfoPanel.SetActive(true);

            if (charNameText != null)
                charNameText.text = character.Name;

            if (charHPText != null)
                charHPText.text = $"HP: {character.currentHP}/{character.maxHP}";

            if (charDamageText != null)
                charDamageText.text = $"Damage: {character.effectiveStrength}";

            if (charAgilityText != null)
                charAgilityText.text = $"Agility: {character.effectiveAgility}";

            if (charFactionText != null)
                charFactionText.text = $"Faction: {character.faction}";
        }

        void CloseCharacterInfo()
        {
            selectedCharacter = null;
            if (characterInfoPanel != null)
            {
                characterInfoPanel.SetActive(false);
            }
        }

        Character GetEntityAtMousePosition()
        {
            if (GameManager.Instance?.Board == null || Camera.main == null)
            {
                Debug.Log("CharacterInfo: Board or Camera is null!");
                return null;
            }

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

            // Debug board dimensions
            int boardW = GameManager.Instance.Board.Width;
            int boardH = GameManager.Instance.Board.Height;

            // Add board centering offset to match TilemapRenderer coordinate system
            // TilemapRenderer renders at (pos.x - Width/2, pos.y - Height/2)
            mouseWorldPos.x += boardW / 2f;
            mouseWorldPos.y += boardH / 2f;

            TilePos tilePos = new TilePos(
                Mathf.FloorToInt(mouseWorldPos.x),
                Mathf.FloorToInt(mouseWorldPos.y)
            );

            Debug.Log($"CharacterInfo: Board={boardW}x{boardH}, Mouse world=({mouseWorldPos.x - boardW/2f:F1},{mouseWorldPos.y - boardH/2f:F1}), tile={tilePos.x},{tilePos.y}");

            Character entity = GameManager.Instance.Board.GetEntityAt(tilePos);
            Debug.Log($"CharacterInfo: Entity at tile: {entity?.Name ?? "null"}");
            return entity;
        }

        #endregion
    }
}