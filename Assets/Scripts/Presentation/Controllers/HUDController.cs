using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Undermarch.Presentation.Managers;
using Undermarch.Simulation.Interfaces;
using Undermarch.Simulation.Core;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Undermarch.Presentation.Controllers
{
    public class HUDController : MonoBehaviour
    {
        [Header("Top HUD")]
        public TextMeshProUGUI coinText;
        public TextMeshProUGUI turnIndicatorText;
        public Image turnIndicatorImage; // Optional: Change color
        public Button settingsButton;
        public Button levelSelectButton;
        public Button startCombatButton;
        public Button pauseButton;

        [Header("Bottom HUD")]
        public Button buildSlimeButton;
        public TextMeshProUGUI slimePriceText;
        public Button buildTrapButton;
        public TextMeshProUGUI trapPriceText;

        [Header("References")]
        public PlacementController placementController;

        private void Awake()
        {
            // Ensure EventSystem exists
            if (FindObjectOfType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<InputSystemUIInputModule>();
            }

            // Ensure HUD exists
            if (transform.Find("TopHUD") == null || transform.Find("BottomHUD") == null)
            {
                BuildHUD();
            }

            // Auto-find references if missing
            // Top HUD
            if (coinText == null) coinText = transform.Find("TopHUD/InfoPanel/CoinText")?.GetComponent<TextMeshProUGUI>();
            if (turnIndicatorText == null) turnIndicatorText = transform.Find("TopHUD/InfoPanel/TurnIndicatorText")?.GetComponent<TextMeshProUGUI>();
            if (settingsButton == null) settingsButton = transform.Find("TopHUD/SettingsButton")?.GetComponent<Button>();
            if (levelSelectButton == null) levelSelectButton = transform.Find("TopHUD/LevelSelectButton")?.GetComponent<Button>();
            if (startCombatButton == null) startCombatButton = transform.Find("TopHUD/StartWaveButton")?.GetComponent<Button>();
            if (pauseButton == null) pauseButton = transform.Find("TopHUD/PauseButton")?.GetComponent<Button>();
            
            // Bottom HUD
            if (buildSlimeButton == null) buildSlimeButton = transform.Find("BottomHUD/PlaceSlimeButton")?.GetComponent<Button>();
            if (slimePriceText == null && buildSlimeButton != null) slimePriceText = buildSlimeButton.transform.Find("SlimePriceText")?.GetComponent<TextMeshProUGUI>();
            
            if (buildTrapButton == null) buildTrapButton = transform.Find("BottomHUD/PlaceTrapButton")?.GetComponent<Button>();
            if (trapPriceText == null && buildTrapButton != null) trapPriceText = buildTrapButton.transform.Find("TrapPriceText")?.GetComponent<TextMeshProUGUI>();

            if (placementController == null) placementController = FindObjectOfType<PlacementController>();
        }

        private void Start()
        {
            Debug.Log("HUDController: Start()");

            // Initialize Buttons
            if (settingsButton) { settingsButton.onClick.AddListener(OnSettingsClicked); Debug.Log("HUD: Settings Button Bound"); }
            if (levelSelectButton) { levelSelectButton.onClick.AddListener(OnLevelSelectClicked); Debug.Log("HUD: Level Select Button Bound"); }
            if (startCombatButton) { startCombatButton.onClick.AddListener(OnStartCombatClicked); Debug.Log("HUD: Start Combat Button Bound"); }
            if (pauseButton) { pauseButton.onClick.AddListener(OnPauseClicked); Debug.Log("HUD: Pause Button Bound"); }
            
            if (buildSlimeButton) { buildSlimeButton.onClick.AddListener(OnBuildSlimeClicked); Debug.Log("HUD: Build Slime Button Bound"); }
            else Debug.LogError("HUD: Build Slime Button NOT FOUND");

            if (buildTrapButton) { buildTrapButton.onClick.AddListener(OnBuildTrapClicked); Debug.Log("HUD: Build Trap Button Bound"); }
            else Debug.LogError("HUD: Build Trap Button NOT FOUND");

            // Initialize Prices
            if (GameManager.Instance != null && GameManager.Instance.GameState != null)
            {
                var gameState = GameManager.Instance.GameState as GameState;
                
                if (gameState != null)
                {
                    if (slimePriceText) slimePriceText.text = $"{gameState.PlacementCosts["SlimeMonster"]} G";
                    if (trapPriceText) trapPriceText.text = $"{gameState.PlacementCosts["SpikeTrap"]} G";
                }
                
                GameManager.Instance.GameState.OnResourcesChanged += UpdateResourceDisplay;
            }

            UpdateResourceDisplay();
            UpdateTurnIndicator();
        }

        private void Update()
        {
            UpdateTurnIndicator();
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null && GameManager.Instance.GameState != null)
            {
                GameManager.Instance.GameState.OnResourcesChanged -= UpdateResourceDisplay;
            }
        }

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
                    break;
                case GamePhase.Combat:
                    statusText = "ENEMY WAVE";
                    statusColor = Color.red;
                    if (startCombatButton) startCombatButton.interactable = false;
                    break;
                case GamePhase.GameOver:
                    statusText = "GAME OVER";
                    statusColor = Color.gray;
                    break;
            }

            if (turnIndicatorText) turnIndicatorText.text = statusText;
            if (turnIndicatorImage) turnIndicatorImage.color = statusColor;
        }

        // Button Handlers
        private void OnSettingsClicked()
        {
            Debug.Log("HUD: Settings Clicked");
        }

        private void OnLevelSelectClicked()
        {
            Debug.Log("HUD: Level Select Clicked");
        }

        private void OnStartCombatClicked()
        {
            Debug.Log("HUD: Start Combat Clicked");
            GameManager.Instance.StartCombat();
        }

        private void OnPauseClicked()
        {
            Debug.Log("HUD: Pause Clicked");
            var tickSystem = GameManager.Instance.TickSystem;
            if (tickSystem != null)
            {
                if (tickSystem.Mode == TickMode.Paused)
                    tickSystem.Resume();
                else
                    tickSystem.Pause();
            }
        }

        private void OnBuildSlimeClicked()
        {
            Debug.Log("HUD: Build Slime Clicked");
            if (placementController) placementController.SelectSlime();
        }

        private void OnBuildTrapClicked()
        {
            Debug.Log("HUD: Build Trap Clicked");
            if (placementController) placementController.SelectSpikeTrap();
        }

        public void BuildHUD()
        {
            // Ensure Canvas settings
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // Ensure on top

            CanvasScaler scaler = GetComponent<CanvasScaler>();
            if (scaler == null) scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            // Add GraphicRaycaster
            if (GetComponent<GraphicRaycaster>() == null) gameObject.AddComponent<GraphicRaycaster>();

            // Clear existing if partial
            Transform top = transform.Find("TopHUD");
            if (top != null) Destroy(top.gameObject);
            Transform bottom = transform.Find("BottomHUD");
            if (bottom != null) Destroy(bottom.gameObject);

            SetupHUD(gameObject);
        }

        void SetupHUD(GameObject canvasObj)
        {
            // 2. Top HUD
            GameObject topPanel = CreatePanel(canvasObj.transform, "TopHUD", new Color(0.18f, 0.18f, 0.18f, 1f), 100, true);
            SetupHorizontalLayout(topPanel, 20, 20, TextAnchor.MiddleCenter);

            // Top Elements
            CreateButton(topPanel.transform, "SettingsButton", "Set", new Vector2(60, 60));
            CreateButton(topPanel.transform, "LevelSelectButton", "Lvl", new Vector2(60, 60));
            
            CreateSpacer(topPanel.transform); // Spacer Left

            GameObject infoPanel = CreateContainer(topPanel.transform, "InfoPanel");
            var infoLayout = infoPanel.AddComponent<VerticalLayoutGroup>();
            infoLayout.childAlignment = TextAnchor.MiddleCenter;
            infoLayout.childControlWidth = true;
            infoLayout.childControlHeight = true;
            
            CreateText(infoPanel.transform, "TurnIndicatorText", "BUILD PHASE", 24, Color.white, true);
            CreateText(infoPanel.transform, "CoinText", "Coins: 0", 20, Color.yellow, false);

            CreateSpacer(topPanel.transform); // Spacer Right

            CreateButton(topPanel.transform, "StartWaveButton", "START", new Vector2(120, 60), new Color(0.3f, 0.7f, 0.3f));
            CreateButton(topPanel.transform, "PauseButton", "||", new Vector2(60, 60));


            // 3. Bottom HUD
            GameObject bottomPanel = CreatePanel(canvasObj.transform, "BottomHUD", new Color(0.18f, 0.18f, 0.18f, 1f), 140, false);
            SetupHorizontalLayout(bottomPanel, 20, 40, TextAnchor.MiddleCenter);

            // Bottom Elements
            CreateBuildButton(bottomPanel.transform, "PlaceSlimeButton", "Slime", "50 G");
            CreateBuildButton(bottomPanel.transform, "PlaceTrapButton", "Trap", "30 G");
        }

        GameObject CreatePanel(Transform parent, string name, Color color, float height, bool isTop)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(parent, false);
            
            Image img = obj.GetComponent<Image>();
            img.color = color;

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
            group.padding = new RectOffset(padding, padding, 10, 10);
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

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(100, 100);

            var vGroup = obj.AddComponent<VerticalLayoutGroup>();
            vGroup.childAlignment = TextAnchor.MiddleCenter;
            vGroup.padding = new RectOffset(5, 5, 5, 5);

            CreateText(obj.transform, "NameText", label, 18, Color.white, false);
            CreateText(obj.transform, name == "PlaceSlimeButton" ? "SlimePriceText" : "TrapPriceText", price, 16, Color.yellow, false);
        }

        void CreateText(Transform parent, string name, string content, float fontSize, Color color, bool bold)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            obj.transform.SetParent(parent, false);
            
            TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
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
    }
}
