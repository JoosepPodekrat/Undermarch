using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Undermarch.Simulation.Entities;
using Undermarch.Simulation.Grid;
using Undermarch.Presentation.Managers;

namespace Undermarch.Presentation.UI
{
    /// <summary>
    /// Displays a tooltip with character stats when hovering over entities.
    /// Attach this to a UI panel GameObject in the scene.
    /// </summary>
    public class CharacterTooltip : MonoBehaviour
    {
        [Header("Tooltip Panel")]
        public GameObject tooltipPanel;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI hpText;
        public TextMeshProUGUI damageText;
        public TextMeshProUGUI agilityText;
        public TextMeshProUGUI factionText;

        [Header("Settings")]
        public float tooltipOffset = 20f; // Offset from mouse cursor
        public bool showOnHover = true; // Show on hover vs click

        private Canvas parentCanvas;
        private RectTransform rectTransform;

        private void Awake()
        {
            // Get or add required components
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = gameObject.AddComponent<RectTransform>();
            }

            parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                Debug.LogError("CharacterTooltip: Must be placed under a Canvas!");
            }

            // Create tooltip panel if it doesn't exist
            if (tooltipPanel == null)
            {
                CreateTooltipPanel();
            }

            // Start with tooltip hidden
            HideTooltip();
        }

        private void Update()
        {
            if (!showOnHover)
            {
                HideTooltip();
                return;
            }

            // Check if mouse is over an entity
            Character hoveredCharacter = GetEntityAtMousePosition();

            if (hoveredCharacter != null)
            {
                ShowTooltip(hoveredCharacter);
                UpdateTooltipPosition();
            }
            else
            {
                HideTooltip();
            }
        }

        private Character GetEntityAtMousePosition()
        {
            if (GameManager.Instance?.Board == null || Camera.main == null)
                return null;

            // Get mouse position in world coordinates
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Convert to tile position
            TilePos tilePos = new TilePos(
                Mathf.RoundToInt(mouseWorldPos.x),
                Mathf.RoundToInt(mouseWorldPos.y)
            );

            // Get entity at that position
            return GameManager.Instance.Board.GetEntityAt(tilePos);
        }

        private void ShowTooltip(Character character)
        {
            if (tooltipPanel == null) return;

            tooltipPanel.SetActive(true);

            if (nameText != null)
                nameText.text = $"{character.Name}";

            if (hpText != null)
                hpText.text = $"HP: {character.currentHP}/{character.maxHP}";

            if (damageText != null)
                damageText.text = $"Damage: {character.effectiveStrength}";

            if (agilityText != null)
                agilityText.text = $"Agility: {character.effectiveAgility}";

            if (factionText != null)
                factionText.text = $"Faction: {character.faction}";
        }

        private void HideTooltip()
        {
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }
        }

        private void UpdateTooltipPosition()
        {
            if (tooltipPanel == null || parentCanvas == null) return;

            // Get mouse position
            Vector2 mousePos = Input.mousePosition;

            // Convert to canvas coordinates
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                mousePos,
                parentCanvas.worldCamera,
                out localPoint
            );

            // Apply offset
            localPoint += new Vector2(tooltipOffset, -tooltipOffset);

            // Set tooltip position
            tooltipPanel.transform.localPosition = localPoint;
        }

        private void CreateTooltipPanel()
        {
            // Create panel
            tooltipPanel = new GameObject("TooltipPanel", typeof(RectTransform), typeof(Image));
            tooltipPanel.transform.SetParent(transform, false);

            // Setup panel
            RectTransform panelRect = tooltipPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 1);
            panelRect.sizeDelta = new Vector2(200, 150);

            Image panelImage = tooltipPanel.GetComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            // Add vertical layout group
            var layoutGroup = tooltipPanel.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            layoutGroup.spacing = 5;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;

            // Create text elements
            nameText = CreateText(tooltipPanel.transform, "NameText", "Name", 18, Color.white, true);
            hpText = CreateText(tooltipPanel.transform, "HPText", "HP: 0/0", 14, Color.red, false);
            damageText = CreateText(tooltipPanel.transform, "DamageText", "Damage: 0", 14, Color.yellow, false);
            agilityText = CreateText(tooltipPanel.transform, "AgilityText", "Agility: 0", 14, Color.cyan, false);
            factionText = CreateText(tooltipPanel.transform, "FactionText", "Faction: None", 14, Color.gray, false);

            Debug.Log("CharacterTooltip: Created tooltip panel with text elements.");
        }

        private TextMeshProUGUI CreateText(Transform parent, string name, string content, float fontSize, Color color, bool bold)
        {
            GameObject textObj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(parent, false);

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(180, 25);

            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            tmp.font = Resources.GetBuiltinResource<TMP_FontAsset>("LegacyRuntime.ttf");
            tmp.text = content;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Left;
            if (bold) tmp.fontStyle = FontStyles.Bold;

            return tmp;
        }
    }
}
