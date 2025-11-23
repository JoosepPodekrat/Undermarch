using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Undermarch.Presentation.Controllers;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Undermarch.Presentation.Tools
{
    [ExecuteInEditMode]
    public class HUDBuilder : MonoBehaviour
    {
        private void Start()
        {
            BuildHUD();
        }

        public void BuildHUD()
        {
            GameObject canvasObj = GameObject.Find("Canvas");
            if (canvasObj == null)
            {
                Debug.LogError("Canvas not found!");
                return;
            }

            Debug.Log("HUDBuilder: Starting setup...");

            // Force delete existing HUD elements
            Transform top = canvasObj.transform.Find("TopHUD");
            if (top != null) DestroyImmediate(top.gameObject);
            
            Transform bottom = canvasObj.transform.Find("BottomHUD");
            if (bottom != null) DestroyImmediate(bottom.gameObject);

            SetupHUD(canvasObj);
            
            // Self-destruct builder
            DestroyImmediate(this.gameObject);
        }

        void SetupHUD(GameObject canvasObj)
        {
            // 1. Canvas Setup
            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            if (scaler == null) scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

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

            // 4. Cleanup Old Junk
            string[] oldNames = { "PlaceSlimeButton", "PlaceTrapButton", "StartWaveButton", "CoinText", "TurnIndicatorText", "SettingsButton", "LevelSelectButton", "PauseButton" };
            foreach (string name in oldNames)
            {
                Transform t = canvasObj.transform.Find(name);
                if (t != null) DestroyImmediate(t.gameObject);
            }

            // 5. Attach Controller
            HUDController hud = canvasObj.GetComponent<HUDController>();
            if (hud == null) hud = canvasObj.AddComponent<HUDController>();
            
#if UNITY_EDITOR
            EditorUtility.SetDirty(canvasObj);
#endif
            Debug.Log("HUDBuilder: Setup Complete.");
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
