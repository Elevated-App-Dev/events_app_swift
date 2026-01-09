using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;

namespace EventPlannerSim.Debug
{
    /// <summary>
    /// Quick setup script to create a basic UI for testing.
    /// Attach this to an empty GameObject in a new scene to see the UI.
    /// </summary>
    public class QuickUISetup : MonoBehaviour
    {
        [Header("Auto-Create UI")]
        [SerializeField] private bool createOnStart = true;

        private Canvas _canvas;
        private GameObject _hudPanel;
        private GameObject _phoneButton;

        private void Start()
        {
            if (createOnStart)
            {
                CreateBasicUI();
            }
        }

        [ContextMenu("Create Basic UI")]
        public void CreateBasicUI()
        {
            // Create Canvas
            var canvasObj = new GameObject("MainCanvas");
            _canvas = canvasObj.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>();

            // Create HUD Panel (top bar)
            CreateHUD();

            // Create bottom navigation
            CreateBottomNav();

            // Create center content area
            CreateContentArea();

            UnityEngine.Debug.Log("[QuickUISetup] Basic UI created! You should see a HUD at top and navigation at bottom.");
        }

        private void CreateHUD()
        {
            _hudPanel = CreatePanel("HUD", _canvas.transform);
            var rect = _hudPanel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, 80);
            rect.anchoredPosition = Vector2.zero;

            // Background
            var bg = _hudPanel.GetComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

            // Date text (left)
            var dateText = CreateText("DateText", _hudPanel.transform, "Jan 15", TextAlignmentOptions.Left);
            var dateRect = dateText.GetComponent<RectTransform>();
            dateRect.anchorMin = new Vector2(0, 0);
            dateRect.anchorMax = new Vector2(0.3f, 1);
            dateRect.offsetMin = new Vector2(20, 10);
            dateRect.offsetMax = new Vector2(0, -10);

            // Money text (center-right)
            var moneyText = CreateText("MoneyText", _hudPanel.transform, "$2,500", TextAlignmentOptions.Right);
            moneyText.color = new Color(0.3f, 0.8f, 0.3f); // Green
            var moneyRect = moneyText.GetComponent<RectTransform>();
            moneyRect.anchorMin = new Vector2(0.5f, 0);
            moneyRect.anchorMax = new Vector2(0.75f, 1);
            moneyRect.offsetMin = new Vector2(0, 10);
            moneyRect.offsetMax = new Vector2(-10, -10);

            // Reputation text (right)
            var repText = CreateText("ReputationText", _hudPanel.transform, "Rep: 50", TextAlignmentOptions.Right);
            repText.color = new Color(0.8f, 0.6f, 0.2f); // Gold
            var repRect = repText.GetComponent<RectTransform>();
            repRect.anchorMin = new Vector2(0.75f, 0);
            repRect.anchorMax = new Vector2(1, 1);
            repRect.offsetMin = new Vector2(0, 10);
            repRect.offsetMax = new Vector2(-20, -10);
        }

        private void CreateBottomNav()
        {
            var navPanel = CreatePanel("BottomNav", _canvas.transform);
            var rect = navPanel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(0, 100);
            rect.anchoredPosition = Vector2.zero;

            var bg = navPanel.GetComponent<Image>();
            bg.color = new Color(0.12f, 0.12f, 0.18f, 0.98f);

            // Phone button
            _phoneButton = CreateButton("PhoneBtn", navPanel.transform, "Phone", () => {
                UnityEngine.Debug.Log("Phone tapped! (Phone overlay would open here)");
                ShowMessage("Phone app coming soon!");
            });
            PositionNavButton(_phoneButton, 0, 3);

            // Map button
            var mapBtn = CreateButton("MapBtn", navPanel.transform, "Map", () => {
                UnityEngine.Debug.Log("Map tapped!");
                ShowMessage("Map overlay coming soon!");
            });
            PositionNavButton(mapBtn, 1, 3);

            // Settings button
            var settingsBtn = CreateButton("SettingsBtn", navPanel.transform, "Menu", () => {
                UnityEngine.Debug.Log("Settings tapped!");
                ShowMessage("Settings coming soon!");
            });
            PositionNavButton(settingsBtn, 2, 3);
        }

        private void PositionNavButton(GameObject btn, int index, int total)
        {
            var rect = btn.GetComponent<RectTransform>();
            float width = 1f / total;
            rect.anchorMin = new Vector2(width * index, 0);
            rect.anchorMax = new Vector2(width * (index + 1), 1);
            rect.offsetMin = new Vector2(10, 10);
            rect.offsetMax = new Vector2(-10, -10);
        }

        private void CreateContentArea()
        {
            var contentPanel = CreatePanel("ContentArea", _canvas.transform);
            var rect = contentPanel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = new Vector2(20, 120); // Above bottom nav
            rect.offsetMax = new Vector2(-20, -100); // Below HUD

            var bg = contentPanel.GetComponent<Image>();
            bg.color = new Color(0.08f, 0.08f, 0.12f, 0.9f);

            // Welcome message
            var welcomeText = CreateText("WelcomeText", contentPanel.transform,
                "Event Planner Simulator\n\nUI Framework Ready!\n\nTap the buttons below to test.",
                TextAlignmentOptions.Center);
            welcomeText.fontSize = 28;
            var welcomeRect = welcomeText.GetComponent<RectTransform>();
            welcomeRect.anchorMin = Vector2.zero;
            welcomeRect.anchorMax = Vector2.one;
            welcomeRect.offsetMin = new Vector2(20, 20);
            welcomeRect.offsetMax = new Vector2(-20, -20);
        }

        private GameObject _messagePanel;
        private TextMeshProUGUI _messageText;

        private void ShowMessage(string message)
        {
            if (_messagePanel == null)
            {
                _messagePanel = CreatePanel("MessagePanel", _canvas.transform);
                var rect = _messagePanel.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.1f, 0.4f);
                rect.anchorMax = new Vector2(0.9f, 0.6f);

                var bg = _messagePanel.GetComponent<Image>();
                bg.color = new Color(0.2f, 0.2f, 0.3f, 0.98f);

                _messageText = CreateText("MessageText", _messagePanel.transform, "", TextAlignmentOptions.Center);
                _messageText.fontSize = 24;
                var textRect = _messageText.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = new Vector2(20, 20);
                textRect.offsetMax = new Vector2(-20, -20);
            }

            _messageText.text = message;
            _messagePanel.SetActive(true);
            Invoke(nameof(HideMessage), 2f);
        }

        private void HideMessage()
        {
            if (_messagePanel != null)
                _messagePanel.SetActive(false);
        }

        #region UI Helpers

        private GameObject CreatePanel(string name, Transform parent)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            panel.AddComponent<RectTransform>();
            panel.AddComponent<Image>();
            return panel;
        }

        private TextMeshProUGUI CreateText(string name, Transform parent, string text, TextAlignmentOptions alignment)
        {
            var textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            textObj.AddComponent<RectTransform>();

            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.alignment = alignment;
            tmp.fontSize = 24;
            tmp.color = Color.white;

            return tmp;
        }

        private GameObject CreateButton(string name, Transform parent, string label, UnityEngine.Events.UnityAction onClick)
        {
            var btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            btnObj.AddComponent<RectTransform>();

            var img = btnObj.AddComponent<Image>();
            img.color = new Color(0.3f, 0.5f, 0.8f, 1f); // Blue button

            var btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(onClick);

            // Button label
            var labelText = CreateText("Label", btnObj.transform, label, TextAlignmentOptions.Center);
            var labelRect = labelText.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            return btnObj;
        }

        #endregion
    }
}
