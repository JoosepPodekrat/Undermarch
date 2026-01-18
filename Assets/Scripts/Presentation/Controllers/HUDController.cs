using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Undermarch;
using Undermarch.Presentation.Managers;
using Undermarch.Presentation.UI;
using Undermarch.Simulation.Interfaces;
using Undermarch.Simulation.Core;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Grid;
using Undermarch.Data;
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
        public Button menuButton;
        public Button startCombatButton;
        public TextMeshProUGUI startButtonText;
        public Button pauseButton;

        [Header("Bottom HUD - Buildable Slider")]
        private BuildableSliderUI buildableSlider;
        public Sprite lockIconSprite; // Assign in Inspector

        [Header("References")]
        public PlacementController placementController;
        
        [Header("Confirmation Dialog")]
        private ConfirmationDialogUI confirmationDialog;

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

        // Set to false for release builds to disable dev panel
        private const bool ENABLE_DEV_PANEL = true;

        [Header("Dev Panel")]
        private GameObject devPanel;
        private const float DEV_PANEL_WIDTH = 150f;

        private void Awake()
        {
            Debug.Log("HUDController: Awake started.");

            // 1. EventSystem Check
            // 1. EventSystem Check
            var eventSystem = FindFirstObjectByType<EventSystem>();
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
            FindAndLog("TopHUD/MenuButton", ref menuButton, "menuButton");
            FindAndLog("TopHUD/StartWaveButton", ref startCombatButton, "startCombatButton");
            FindAndLog("TopHUD/PauseButton", ref pauseButton, "pauseButton");

            // Get button text component
            if (startCombatButton != null && startButtonText == null)
            {
                startButtonText = startCombatButton.GetComponentInChildren<TextMeshProUGUI>();
                if (startButtonText == null)
                    Debug.LogError("HUDController: Could not find StartButton Text child!");
            }

            FindAndLog("BottomHUD/BuildableSlider", ref buildableSlider, "buildableSlider");

            if (placementController == null)
            if (placementController == null)
            {
                placementController = FindFirstObjectByType<PlacementController>();
                Debug.Log($"HUDController: PlacementController found: {placementController != null}");
            }
        }

        private void Start()
        {
            Debug.Log("HUDController: Start() called.");

            // 4. Listener Binding
            BindButton(menuButton, OnMenuClicked, "MenuButton");
            BindButton(startCombatButton, OnStartCombatClicked, "StartCombatButton");
            BindButton(pauseButton, OnPauseClicked, "PauseButton");
            // Initialize slider
            if (buildableSlider != null && GameManager.Instance != null && GameManager.Instance.CurrentLevelData != null)
            {
                buildableSlider.Initialize(GameManager.Instance.CurrentLevelData.availableBuildables);
            }
            else
            {
                Debug.LogWarning("HUDController: Could not initialize slider. Missing reference or level data.");
            }

            // Find or create confirmation dialog
            confirmationDialog = FindFirstObjectByType<ConfirmationDialogUI>();
            if (confirmationDialog == null)
            {
                Debug.Log("HUDController: Creating confirmation dialog...");
                CreateConfirmationDialog();
            }

            // 5. Logic Init
            if (GameManager.Instance != null && GameManager.Instance.GameState != null)
            {
                IGameState gameState = GameManager.Instance.GameState;

                if (gameState != null)
                {
                    // Resource display listener
                    gameState.OnResourcesChanged += UpdateResourceDisplay;
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

            // Phase update handled by slider UI

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

            // Show stats for selected placement type
            UpdatePlacementPreviewStats();
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
            if (GameManager.Instance?.GameState == null || coinText == null)
                return;

            int gold = GameManager.Instance.GameState.GetResource(ResourceType.Gold);
            coinText.text = $"Gold: {gold}";
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

        private void OnMenuClicked()
        {
            Debug.Log("HUD_CLICK: Menu Clicked");
            if (confirmationDialog != null)
            {
                confirmationDialog.Show(
                    "Return to main menu?\nProgress will not be saved.",
                    () => {
                        // On confirm - go to main menu
                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.GoToMainMenu();
                        }
                    }
                );
            }
            else
            {
                // Fallback if no confirmation dialog exists
                Debug.LogWarning("HUDController: No confirmation dialog found, going to main menu directly.");
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.GoToMainMenu();
                }
            }
        }
        
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
            menuButton = null;
            startCombatButton = null;
            pauseButton = null;
            buildableSlider = null;

            RefreshReferences();

            Debug.Log("HUDController: BuildHUD complete.");
        }

        void SetupHUD(GameObject canvasObj)
        {
            // 2. Top HUD
            GameObject topPanel = CreatePanel(canvasObj.transform, "TopHUD", new Color(0.18f, 0.18f, 0.18f, 1f), 110, true);
            SetupHorizontalLayout(topPanel, 20, 15, TextAnchor.MiddleCenter);

            // Top Elements - Single Menu button
            CreateButton(topPanel.transform, "MenuButton", "Menu", new Vector2(80, 60));

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

            // Bottom Elements - Slider
            // Configure parent layout to allow expansion
            var bottomLayout = bottomPanel.GetComponent<HorizontalLayoutGroup>();
            if (bottomLayout != null)
            {
                bottomLayout.childControlWidth = true;
                bottomLayout.childForceExpandWidth = true;
                bottomLayout.childControlHeight = true;
                bottomLayout.childForceExpandHeight = true;
            }

            // Create slider container
            GameObject sliderObj = new GameObject("BuildableSlider", typeof(RectTransform));
            sliderObj.transform.SetParent(bottomPanel.transform, false);

            // Add layout element to expand
            LayoutElement sliderLE = sliderObj.AddComponent<LayoutElement>();
            sliderLE.flexibleWidth = 1;
            sliderLE.flexibleHeight = 1;

            // Add the slider component
            buildableSlider = sliderObj.AddComponent<BuildableSliderUI>();
            buildableSlider.lockIconSprite = lockIconSprite;
            
            Debug.Log("HUDController: Created BuildableSlider.");

            // 4. Character Info Panel (right side, initially hidden)
            CreateCharacterInfoPanel(canvasObj.transform);

            // 5. Pause Overlay (initially hidden)
            CreatePauseOverlay(canvasObj.transform);

            // 6. Dev Panel (for development)
            if (ENABLE_DEV_PANEL)
            {
                CreateDevPanel(canvasObj.transform);
            }
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

        void CreateConfirmationDialog()
        {
            // Create dialog root
            GameObject dialogRoot = new GameObject("ConfirmationDialog", typeof(RectTransform), typeof(ConfirmationDialogUI));
            dialogRoot.transform.SetParent(transform, false);
            
            RectTransform rootRT = dialogRoot.GetComponent<RectTransform>();
            rootRT.anchorMin = Vector2.zero;
            rootRT.anchorMax = Vector2.one;
            rootRT.offsetMin = Vector2.zero;
            rootRT.offsetMax = Vector2.zero;

            // Create dark overlay/panel
            GameObject panel = new GameObject("DialogPanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(dialogRoot.transform, false);
            
            RectTransform panelRT = panel.GetComponent<RectTransform>();
            panelRT.anchorMin = Vector2.zero;
            panelRT.anchorMax = Vector2.one;
            panelRT.offsetMin = Vector2.zero;
            panelRT.offsetMax = Vector2.zero;
            
            Image panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f);
            panelImage.raycastTarget = true;

            // Create center box
            GameObject box = new GameObject("DialogBox", typeof(RectTransform), typeof(Image));
            box.transform.SetParent(panel.transform, false);
            
            RectTransform boxRT = box.GetComponent<RectTransform>();
            boxRT.anchorMin = new Vector2(0.5f, 0.5f);
            boxRT.anchorMax = new Vector2(0.5f, 0.5f);
            boxRT.pivot = new Vector2(0.5f, 0.5f);
            boxRT.sizeDelta = new Vector2(400, 200);
            
            Image boxImage = box.GetComponent<Image>();
            boxImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            // Create message text
            GameObject messageObj = new GameObject("MessageText", typeof(RectTransform), typeof(TextMeshProUGUI));
            messageObj.transform.SetParent(box.transform, false);
            
            RectTransform messageRT = messageObj.GetComponent<RectTransform>();
            messageRT.anchorMin = new Vector2(0.1f, 0.4f);
            messageRT.anchorMax = new Vector2(0.9f, 0.9f);
            messageRT.offsetMin = Vector2.zero;
            messageRT.offsetMax = Vector2.zero;
            
            TextMeshProUGUI messageText = messageObj.GetComponent<TextMeshProUGUI>();
            messageText.text = "Are you sure?";
            messageText.fontSize = 24;
            messageText.color = Color.white;
            messageText.alignment = TextAlignmentOptions.Center;

            // Create Yes button
            GameObject yesBtn = CreateDialogButton(box.transform, "YesButton", "Yes", new Vector2(-70, -70));
            Button yesButton = yesBtn.GetComponent<Button>();

            // Create No button
            GameObject noBtn = CreateDialogButton(box.transform, "NoButton", "No", new Vector2(70, -70));
            Button noButton = noBtn.GetComponent<Button>();

            // Wire up the ConfirmationDialogUI component
            confirmationDialog = dialogRoot.GetComponent<ConfirmationDialogUI>();
            confirmationDialog.dialogPanel = panel;
            confirmationDialog.messageText = messageText;
            confirmationDialog.yesButton = yesButton;
            confirmationDialog.noButton = noButton;

            // Initialize button listeners now that references are assigned
            confirmationDialog.Initialize();

            // Start hidden
            panel.SetActive(false);

            Debug.Log("HUDController: Confirmation dialog created.");
        }

        GameObject CreateDialogButton(Transform parent, string name, string text, Vector2 position)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            obj.transform.SetParent(parent, false);
            
            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = new Vector2(100, 40);
            
            Image img = obj.GetComponent<Image>();
            img.color = new Color(0.3f, 0.3f, 0.3f);
            img.raycastTarget = true;

            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(obj.transform, false);
            
            RectTransform textRT = textObj.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = Vector2.zero;
            textRT.offsetMax = Vector2.zero;
            
            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 20;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;

            return obj;
        }

        GameObject CreateContainer(Transform parent, string name)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);
            return obj;
        }

        #region Character Info Panel

        private PlacementType _lastShownPlacementType = PlacementType.None;

        void UpdatePlacementPreviewStats()
        {
            if (placementController == null) return;

            var selectedType = placementController.SelectedType;

            // If a placement type is selected, show its stats
            if (selectedType != PlacementType.None)
            {
                // Only update if the selection changed
                if (selectedType != _lastShownPlacementType)
                {
                    _lastShownPlacementType = selectedType;
                    ShowPlacementStats(selectedType);
                }
            }
            else
            {
                // Clear if we were showing placement stats
                if (_lastShownPlacementType != PlacementType.None)
                {
                    _lastShownPlacementType = PlacementType.None;
                    // Don't close the panel if we're showing a placed character
                    if (selectedCharacter == null)
                    {
                        CloseCharacterInfo();
                    }
                }
            }
        }

        void ShowPlacementStats(PlacementType placementType)
        {
            Character previewCharacter = null;

            int cost = 0;
            if (placementController != null && placementController.SelectedDefinition != null)
            {
                cost = placementController.SelectedDefinition.goldCost;
            }

            // Create a temporary character based on the placement type
            switch (placementType)
            {
                case PlacementType.Slime:
                    previewCharacter = CharacterDatabase.slimeMonster.Clone();
                    break;
                case PlacementType.GreenSlime:
                    previewCharacter = CharacterDatabase.strongSlime.Clone();
                    break;
                case PlacementType.BlueSlime:
                    previewCharacter = CharacterDatabase.strongSlime.Clone(); 
                    break;
                case PlacementType.Archer:
                    previewCharacter = CharacterDatabase.archerMonster.Clone();
                    break;
                case PlacementType.Goblin:
                    previewCharacter = CharacterDatabase.goblin1.Clone();
                    break;
                case PlacementType.RedSpider:
                    previewCharacter = CharacterDatabase.redSpider.Clone();
                    break;
                case PlacementType.PurpleSpider:
                    previewCharacter = CharacterDatabase.purpleSpider.Clone();
                    break;
                case PlacementType.RedDemon:
                    previewCharacter = CharacterDatabase.weakerDemon.Clone();
                    break;
                case PlacementType.PurpleDemon:
                    previewCharacter = CharacterDatabase.strongerDemon.Clone();
                    break;
                
                case PlacementType.SpikeTrap:
                    ShowTrapStats("Spike Trap", cost, "Damages enemies that step on it");
                    return;
                case PlacementType.BearTrap:
                    ShowTrapStats("Bear Trap", cost, "Immobilizes and damages enemies");
                    return;
                case PlacementType.GasTrap:
                    ShowTrapStats("Gas Trap", cost, "Releases poison gas cloud");
                    return;
                case PlacementType.MetalSpikeTrap:
                    ShowTrapStats("Metal Spike Trap", cost, "Heavy physical damage");
                    return;
            }

            if (previewCharacter != null)
            {
                // Initialize the character's stats
                previewCharacter.CalculateStats();
                previewCharacter.InitStats();
                
                // Show in the info panel
                ShowCharacterInfo(previewCharacter);

                // OVERRIDE usage of HP text to show Cost + HP for buildables
                if (charHPText != null)
                {
                    charHPText.text = $"Cost: {cost} Gold  |  HP: {previewCharacter.maxHP}";
                }
                
                // Mark as preview (not a placed character)
                selectedCharacter = null;
            }
        }

        void ShowTrapStats(string trapName, int cost, string description)
        {
            if (characterInfoPanel == null) return;

            characterInfoPanel.SetActive(true);

            if (charNameText != null)
                charNameText.text = trapName;

            if (charHPText != null)
                charHPText.text = $"Cost: {cost} {ResourceType.Gold}";


            if (charDamageText != null)
                charDamageText.text = description;

            if (charAgilityText != null)
                charAgilityText.text = "";

            if (charFactionText != null)
                charFactionText.text = "Type: Trap";

            // Mark as preview
            selectedCharacter = null;
        }

        void ShowTrapInfo(Trap trap)
        {
            if (characterInfoPanel == null) return;

            characterInfoPanel.SetActive(true);

            if (charNameText != null)
                charNameText.text = trap.Name;

            if (charHPText != null)
                charHPText.text = $"Durability: {trap.Durability}";

            if (charDamageText != null)
            {
                // Show damage info from the trap's damage packet
                string damageInfo = "Damage: ";
                if (trap.DamagePacket != null && trap.DamagePacket.TotalDamage() > 0)
                {
                    damageInfo += trap.DamagePacket.TotalDamage().ToString();
                }
                else
                {
                    damageInfo += "0";
                }
                charDamageText.text = damageInfo;
            }

            if (charAgilityText != null)
                charAgilityText.text = "";

            if (charFactionText != null)
                charFactionText.text = "Type: Trap (Placed)";

            // Mark that we're not showing a character
            selectedCharacter = null;
        }

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
                // Also clear placement selection
                _lastShownPlacementType = PlacementType.None;
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
                        _lastShownPlacementType = PlacementType.None;
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
                    // Clear placement preview tracking since we're now showing a real character
                    _lastShownPlacementType = PlacementType.None;
                }
                else
                {
                    // Check for traps if no character found
                    Trap trap = GetTrapAtMousePosition();
                    if (trap != null)
                    {
                        ShowTrapInfo(trap);
                        // Clear placement preview tracking since we're now showing a placed trap
                        _lastShownPlacementType = PlacementType.None;
                    }
                    else
                    {
                        // Clicked on empty space - only close if not showing a placement preview
                        if (placementController == null || placementController.SelectedType == PlacementType.None)
                        {
                            CloseCharacterInfo();
                        }
                    }
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

        Trap GetTrapAtMousePosition()
        {
            if (GameManager.Instance?.Board == null || Camera.main == null)
            {
                return null;
            }

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

            int boardW = GameManager.Instance.Board.Width;
            int boardH = GameManager.Instance.Board.Height;

            mouseWorldPos.x += boardW / 2f;
            mouseWorldPos.y += boardH / 2f;

            TilePos tilePos = new TilePos(
                Mathf.FloorToInt(mouseWorldPos.x),
                Mathf.FloorToInt(mouseWorldPos.y)
            );

            var interactable = GameManager.Instance.Board.GetInteractableAt(tilePos);
            if (interactable is Trap trap)
            {
                Debug.Log($"CharacterInfo: Trap at tile: {trap.Name}");
                return trap;
            }

            Debug.Log("CharacterInfo: No trap at tile");
            return null;
        }

        #endregion

        #region Dev Panel

        void CreateDevPanel(Transform canvasTransform)
        {
            devPanel = new GameObject("DevPanel", typeof(RectTransform));
            devPanel.transform.SetParent(canvasTransform, false);

            RectTransform panelRect = devPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0.5f);  // Left edge, vertically centered
            panelRect.anchorMax = new Vector2(0, 0.5f);
            panelRect.pivot = new Vector2(0, 0.5f);
            panelRect.anchoredPosition = new Vector2(10, 0);  // Offset from left edge
            panelRect.sizeDelta = new Vector2(DEV_PANEL_WIDTH, 120);

            // Semi-transparent background
            Image bg = devPanel.AddComponent<Image>();
            bg.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);

            // Create buttons
            CreateDevButton(devPanel.transform, "Kill Enemies", 40, OnKillAllEnemies);
            CreateDevButton(devPanel.transform, "+1000 Gold", -40, OnAddGold);

            Debug.Log("HUDController: Created dev panel.");
        }

        GameObject CreateDevButton(Transform parent, string text, float yOffset, UnityEngine.Events.UnityAction onClick)
        {
            GameObject btn = new GameObject(text.Replace(" ", "") + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
            btn.transform.SetParent(parent, false);

            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0.5f);  // Left edge
            rect.anchorMax = new Vector2(0, 0.5f);  // Left edge
            rect.pivot = new Vector2(0, 0.5f);      // Pivot at left-center
            rect.anchoredPosition = new Vector2(5, yOffset);  // 5px from left edge
            rect.sizeDelta = new Vector2(DEV_PANEL_WIDTH - 10, 40);  // Width with margins

            btn.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
            btn.GetComponent<Button>().onClick.AddListener(onClick);

            // Button text
            GameObject txt = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            txt.transform.SetParent(btn.transform, false);

            RectTransform txtRect = txt.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = Vector2.zero;
            txtRect.offsetMax = Vector2.zero;

            TextMeshProUGUI tmp = txt.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 14;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.font = Resources.GetBuiltinResource<TMP_FontAsset>("LegacyRuntime.ttf");
            tmp.color = Color.white;

            return btn;
        }

        private void OnKillAllEnemies()
        {
            if (GameManager.Instance?.Board == null) return;

            var heroes = GameManager.Instance.Board.GetAllCharacters()
                .Where(c => c.faction == Undermarch.Simulation.Combat.Faction.Hero)
                .ToList();

            foreach (var hero in heroes)
            {
                // Deal lethal damage to kill the hero
                var damagePacket = new Undermarch.Simulation.Combat.DamagePacket();
                damagePacket.Add(Undermarch.Simulation.Combat.DamageType.Physical, 9999);
                hero.TakeDamage(damagePacket);
            }

            Debug.Log($"Dev: Killed {heroes.Count} enemies");
        }

        private void OnAddGold()
        {
            if (GameManager.Instance?.GameState == null) return;

            GameManager.Instance.GameState.AddResource(ResourceType.Gold, 1000);
            Debug.Log("Dev: Added 1000 gold");
        }


        #endregion
    }
}