using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Undermarch.Data;
using Undermarch.Presentation.Controllers;
using Undermarch.Presentation.Managers;
using Undermarch.Presentation.Sounds;
using Undermarch.Simulation.Interfaces;

namespace Undermarch.Presentation.UI
{
    /// <summary>
    /// UI component that displays a scrollable slider of available buildables.
    /// Shows 4-5 items at once with left/right arrows to scroll through more.
    /// Phase-locked items appear grayed out with a lock indicator.
    /// </summary>
    public class BuildableSliderUI : MonoBehaviour
    {
        [Header("References")]
        public PlacementController placementController;
        
        [Header("Configuration")]
        [Tooltip("How many buildable slots are visible at once")]
        public int visibleSlots = 4;
        
        [Tooltip("Size of each buildable button")]
        public Vector2 slotSize = new Vector2(100, 100);
        
        [Tooltip("Spacing between slots")]
        public float slotSpacing = 10f;
        
        [Tooltip("Size of arrow buttons")]
        public Vector2 arrowSize = new Vector2(40, 100);

        [Tooltip("Sprite for the lock icon")]
        public Sprite lockIconSprite;

        // UI Elements created at runtime
        private Button leftArrowButton;
        private Button rightArrowButton;
        private GameObject slotsContainer;
        private List<BuildableSlot> slots = new List<BuildableSlot>();
        
        // State
        private BuildableDefinition[] availableBuildables;
        private int currentStartIndex = 0;
        private int selectedSlotIndex = -1;

        // Helper class to track each slot's UI elements
        private class BuildableSlot
        {
            public GameObject root;
            public Button button;
            public Image background;
            public Image iconImage;
            public TextMeshProUGUI nameText;
            public TextMeshProUGUI priceText;
            public GameObject lockOverlay;
            public BuildableDefinition buildable;
        }

        /// <summary>
        /// Initialize the slider with the buildables available for the current level.
        /// Call this when loading a level.
        /// </summary>
        public void Initialize(BuildableDefinition[] buildables)
        {
            if (buildables == null || buildables.Length == 0)
            {
                Debug.LogWarning("BuildableSliderUI: No buildables provided!");
                gameObject.SetActive(false);
                return;
            }

            availableBuildables = buildables;
            currentStartIndex = 0;
            selectedSlotIndex = -1;
            
            // Clear any existing slots
            ClearSlots();
            
            // Create the UI
            CreateSliderUI();
            
            // Initial display update
            UpdateDisplay();
            
            gameObject.SetActive(true);
            Debug.Log($"BuildableSliderUI: Initialized with {buildables.Length} buildables");
        }

        private void ClearSlots()
        {
            foreach (var slot in slots)
            {
                if (slot.root != null)
                    Destroy(slot.root);
            }
            slots.Clear();
            
            if (leftArrowButton != null) Destroy(leftArrowButton.gameObject);
            if (rightArrowButton != null) Destroy(rightArrowButton.gameObject);
            if (slotsContainer != null) Destroy(slotsContainer);
        }

        private void CreateSliderUI()
        {
            // Setup horizontal layout on this object
            var layoutGroup = GetComponent<HorizontalLayoutGroup>();
            if (layoutGroup == null)
                layoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
            
            layoutGroup.spacing = slotSpacing;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.padding = new RectOffset(10, 10, 5, 5);

            // Create left arrow
            leftArrowButton = CreateArrowButton("LeftArrow", "<");
            leftArrowButton.onClick.AddListener(OnLeftArrowClicked);

            // Create slots container
            slotsContainer = new GameObject("SlotsContainer", typeof(RectTransform));
            slotsContainer.transform.SetParent(transform, false);
            
            var containerLayout = slotsContainer.AddComponent<HorizontalLayoutGroup>();
            containerLayout.spacing = slotSpacing;
            containerLayout.childAlignment = TextAnchor.MiddleCenter;
            containerLayout.childControlWidth = false;
            containerLayout.childControlHeight = false;

            // Add ContentSizeFitter to ensure the container expands to fit the slots
            var contentSizeFitter = slotsContainer.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Create the visible slots
            for (int i = 0; i < visibleSlots; i++)
            {
                var slot = CreateBuildableSlot(i);
                slots.Add(slot);
            }

            // Create right arrow
            rightArrowButton = CreateArrowButton("RightArrow", ">");
            rightArrowButton.onClick.AddListener(OnRightArrowClicked);
        }

        private Button CreateArrowButton(string name, string text)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            obj.transform.SetParent(transform, false);

            var rt = obj.GetComponent<RectTransform>();
            rt.sizeDelta = arrowSize;

            var img = obj.GetComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.2f);
            img.raycastTarget = true;

            var btn = obj.GetComponent<Button>();

            // Add text
            var textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(obj.transform, false);

            var textRT = textObj.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = Vector2.zero;
            textRT.offsetMax = Vector2.zero;

            var tmp = textObj.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 28;
            tmp.color = Color.white;

            return btn;
        }

        private BuildableSlot CreateBuildableSlot(int index)
        {
            var slot = new BuildableSlot();

            // Root object with button
            slot.root = new GameObject($"Slot_{index}", typeof(RectTransform), typeof(Image), typeof(Button));
            slot.root.transform.SetParent(slotsContainer.transform, false);

            var rt = slot.root.GetComponent<RectTransform>();
            rt.sizeDelta = slotSize;

            slot.background = slot.root.GetComponent<Image>();
            slot.background.color = new Color(0.1f, 0.1f, 0.1f);
            slot.background.raycastTarget = true;

            slot.button = slot.root.GetComponent<Button>();
            int capturedIndex = index;
            slot.button.onClick.AddListener(() => OnSlotClicked(capturedIndex));

            // Vertical layout for content
            var vLayout = slot.root.AddComponent<VerticalLayoutGroup>();
            vLayout.childAlignment = TextAnchor.MiddleCenter;
            vLayout.padding = new RectOffset(5, 5, 5, 5);
            vLayout.spacing = 2;
            vLayout.childControlWidth = true;
            vLayout.childControlHeight = false;

            // Icon (optional, hidden if no sprite)
            var iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconObj.transform.SetParent(slot.root.transform, false);
            slot.iconImage = iconObj.GetComponent<Image>();
            slot.iconImage.preserveAspect = true;
            var iconLE = iconObj.AddComponent<LayoutElement>();
            iconLE.preferredHeight = 40;
            iconLE.preferredWidth = 40;

            // Name text
            var nameObj = new GameObject("NameText", typeof(RectTransform), typeof(TextMeshProUGUI));
            nameObj.transform.SetParent(slot.root.transform, false);
            slot.nameText = nameObj.GetComponent<TextMeshProUGUI>();
            slot.nameText.alignment = TextAlignmentOptions.Center;
            slot.nameText.fontSize = 14;
            slot.nameText.color = Color.white;
            var nameLE = nameObj.AddComponent<LayoutElement>();
            nameLE.preferredHeight = 20;

            // Price text
            var priceObj = new GameObject("PriceText", typeof(RectTransform), typeof(TextMeshProUGUI));
            priceObj.transform.SetParent(slot.root.transform, false);
            slot.priceText = priceObj.GetComponent<TextMeshProUGUI>();
            slot.priceText.alignment = TextAlignmentOptions.Center;
            slot.priceText.fontSize = 12;
            slot.priceText.color = Color.yellow;
            var priceLE = priceObj.AddComponent<LayoutElement>();
            priceLE.preferredHeight = 18;

            // Lock overlay (for phase-locked items)
            slot.lockOverlay = new GameObject("LockOverlay", typeof(RectTransform), typeof(Image));
            slot.lockOverlay.transform.SetParent(slot.root.transform, false);
            
            var lockRT = slot.lockOverlay.GetComponent<RectTransform>();
            lockRT.anchorMin = Vector2.zero;
            lockRT.anchorMax = Vector2.one;
            lockRT.offsetMin = Vector2.zero;
            lockRT.offsetMax = Vector2.zero;

            var lockImg = slot.lockOverlay.GetComponent<Image>();
            lockImg.color = new Color(0, 0, 0, 0.6f);
            lockImg.raycastTarget = false;

            // Add LayoutElement to ignore parent layout logic
            var lockLE = slot.lockOverlay.AddComponent<LayoutElement>();
            lockLE.ignoreLayout = true;

            // Lock icon image
            var lockIconObj = new GameObject("LockIcon", typeof(RectTransform), typeof(Image));
            lockIconObj.transform.SetParent(slot.lockOverlay.transform, false);
            
            var lockIconRT = lockIconObj.GetComponent<RectTransform>();
            lockIconRT.anchorMin = new Vector2(0.5f, 0.5f);
            lockIconRT.anchorMax = new Vector2(0.5f, 0.5f);
            lockIconRT.sizeDelta = new Vector2(40, 40); // Adjust size as needed

            var lockIconImg = lockIconObj.GetComponent<Image>();
            lockIconImg.sprite = lockIconSprite;
            lockIconImg.preserveAspect = true;
            lockIconImg.raycastTarget = false;

            slot.lockOverlay.SetActive(false);

            return slot;
        }

        /// <summary>
        /// Update the display based on current state.
        /// Call this in Update() or when state changes.
        /// </summary>
        public void UpdateDisplay()
        {
            if (availableBuildables == null) return;

            bool isPhase2 = IsPhase2();
            int totalBuildables = availableBuildables.Length;

            // Update arrow visibility
            bool showLeftArrow = currentStartIndex > 0;
            bool showRightArrow = currentStartIndex + visibleSlots < totalBuildables;
            
            leftArrowButton.gameObject.SetActive(showLeftArrow);
            rightArrowButton.gameObject.SetActive(showRightArrow);

            // Update each slot
            for (int i = 0; i < slots.Count; i++)
            {
                int buildableIndex = currentStartIndex + i;
                var slot = slots[i];

                if (buildableIndex < totalBuildables)
                {
                    var buildable = availableBuildables[buildableIndex];
                    slot.buildable = buildable;
                    slot.root.SetActive(true);

                    // Update display
                    slot.nameText.text = buildable.displayName;
                    slot.priceText.text = $"{buildable.goldCost} G";
                    
                    // Icon
                    if (buildable.icon != null)
                    {
                        slot.iconImage.sprite = buildable.icon;
                        slot.iconImage.gameObject.SetActive(true);
                    }
                    else
                    {
                        slot.iconImage.gameObject.SetActive(false);
                    }

                    // Background color
                    slot.background.color = buildable.buttonColor;

                    // Check if locked (requires phase 2 but we're not in phase 2)
                    bool isLocked = buildable.requiresPhase2 && !isPhase2;
                    slot.lockOverlay.SetActive(isLocked);
                    slot.button.interactable = !isLocked;
                    
                    // Gray out text and icon if locked
                    slot.nameText.color = isLocked ? Color.gray : Color.white;
                    slot.priceText.color = isLocked ? Color.gray : Color.yellow;
                    
                    if (slot.iconImage != null)
                    {
                        slot.iconImage.color = isLocked ? new Color(0.3f, 0.3f, 0.3f, 1f) : Color.white;
                    }

                    // Highlight selected slot
                    if (buildableIndex == selectedSlotIndex && !isLocked)
                    {
                        slot.background.color = new Color(0.3f, 0.5f, 0.3f); // Green tint for selected
                    }
                }
                else
                {
                    // No buildable for this slot
                    slot.root.SetActive(false);
                    slot.buildable = null;
                }
            }
        }

        private void Update()
        {
            // Keep display updated (for phase changes, etc.)
            UpdateDisplay();
        }

        private void OnLeftArrowClicked()
        {
            UIAudioManager.Instance?.PlayButtonClick();
            if (currentStartIndex > 0)
            {
                currentStartIndex--;
                UpdateDisplay();
                Debug.Log($"BuildableSliderUI: Scrolled left, now showing from index {currentStartIndex}");
            }
        }

        private void OnRightArrowClicked()
        {
            UIAudioManager.Instance?.PlayButtonClick();
            if (currentStartIndex + visibleSlots < availableBuildables.Length)
            {
                currentStartIndex++;
                UpdateDisplay();
                Debug.Log($"BuildableSliderUI: Scrolled right, now showing from index {currentStartIndex}");
            }
        }

        private void OnSlotClicked(int slotIndex)
        {
            int buildableIndex = currentStartIndex + slotIndex;
            
            if (buildableIndex >= availableBuildables.Length)
                return;

            var buildable = availableBuildables[buildableIndex];
            
            // Check if locked
            if (buildable.requiresPhase2 && !IsPhase2())
            {
                Debug.Log($"BuildableSliderUI: {buildable.displayName} is locked until Phase 2");
                return;
            }

            UIAudioManager.Instance?.PlaySelectSound();
            selectedSlotIndex = buildableIndex;
            SelectBuildable(buildable);
            UpdateDisplay();
        }

        private bool IsPhase2()
        {
            if (GameManager.Instance == null) return false;
            return GameManager.Instance.CurrentPhase == GamePhase.BuildingPhase2 
                || GameManager.Instance.IsSecondStage;
        }

        private void SelectBuildable(BuildableDefinition buildable)
        {
            if (placementController == null)
            {
                placementController = FindFirstObjectByType<PlacementController>();
                if (placementController == null)
                {
                    Debug.LogError("BuildableSliderUI: No PlacementController found!");
                    return;
                }
            }

            Debug.Log($"BuildableSliderUI: Selected {buildable.displayName} (Cost: {buildable.goldCost}G)");

            placementController.SelectBuildable(buildable);
        }

        /// <summary>
        /// Clear the current selection (e.g., after placing a buildable)
        /// </summary>
        public void ClearSelection()
        {
            selectedSlotIndex = -1;
            UpdateDisplay();
        }
    }
}
