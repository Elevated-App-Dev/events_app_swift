using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.IO;

namespace EventPlannerSim.Editor
{
    public static class PrefabGenerator
    {
        private static string PrefabPath = "Assets/Prefabs/UI";

        [MenuItem("Tools/Generate UI Prefabs")]
        public static void GenerateAllPrefabs()
        {
            // Ensure directories exist
            EnsureDirectories();

            // Generate all prefabs
            CreateHUDPrefab();
            CreatePhoneOverlayPrefab();
            CreateBottomNavPrefab();
            CreateNotificationPrefab();
            CreateButtonPrefab();
            CreateCardPrefab();
            CreateAppIconPrefab();

            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("[PrefabGenerator] All UI prefabs generated!");
        }

        private static void EnsureDirectories()
        {
            if (!Directory.Exists(PrefabPath))
            {
                Directory.CreateDirectory(PrefabPath);
            }
            if (!Directory.Exists(PrefabPath + "/Components"))
            {
                Directory.CreateDirectory(PrefabPath + "/Components");
            }
            if (!Directory.Exists(PrefabPath + "/Overlays"))
            {
                Directory.CreateDirectory(PrefabPath + "/Overlays");
            }
        }

        #region HUD Prefab

        private static void CreateHUDPrefab()
        {
            var root = new GameObject("HUD");
            var rootRect = root.AddComponent<RectTransform>();
            SetFullStretch(rootRect);

            // Background
            var bg = root.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

            // Configure as top bar
            rootRect.anchorMin = new Vector2(0, 1);
            rootRect.anchorMax = new Vector2(1, 1);
            rootRect.pivot = new Vector2(0.5f, 1);
            rootRect.sizeDelta = new Vector2(0, 80);
            rootRect.anchoredPosition = Vector2.zero;

            // Date section (left)
            var dateObj = CreateTextElement("DateText", root.transform, "Jan 15", 24, Color.white, TextAlignmentOptions.Left);
            var dateRect = dateObj.GetComponent<RectTransform>();
            dateRect.anchorMin = new Vector2(0, 0);
            dateRect.anchorMax = new Vector2(0.25f, 1);
            dateRect.offsetMin = new Vector2(20, 10);
            dateRect.offsetMax = new Vector2(0, -10);

            // Money section (center-right)
            var moneyObj = CreateTextElement("MoneyText", root.transform, "$2,500", 28, new Color(0.3f, 0.85f, 0.4f), TextAlignmentOptions.Right);
            var moneyRect = moneyObj.GetComponent<RectTransform>();
            moneyRect.anchorMin = new Vector2(0.45f, 0);
            moneyRect.anchorMax = new Vector2(0.72f, 1);
            moneyRect.offsetMin = new Vector2(0, 10);
            moneyRect.offsetMax = new Vector2(-10, -10);

            // Reputation section (right)
            var repObj = CreateTextElement("ReputationText", root.transform, "50", 28, new Color(0.9f, 0.7f, 0.2f), TextAlignmentOptions.Right);
            var repRect = repObj.GetComponent<RectTransform>();
            repRect.anchorMin = new Vector2(0.75f, 0);
            repRect.anchorMax = new Vector2(1, 1);
            repRect.offsetMin = new Vector2(0, 10);
            repRect.offsetMax = new Vector2(-20, -10);

            // Rep icon/label
            var repLabel = CreateTextElement("RepLabel", root.transform, "Rep", 16, new Color(0.7f, 0.7f, 0.7f), TextAlignmentOptions.Right);
            var repLabelRect = repLabel.GetComponent<RectTransform>();
            repLabelRect.anchorMin = new Vector2(0.72f, 0);
            repLabelRect.anchorMax = new Vector2(0.78f, 1);
            repLabelRect.offsetMin = new Vector2(0, 10);
            repLabelRect.offsetMax = new Vector2(0, -10);

            SavePrefab(root, PrefabPath + "/HUD.prefab");
        }

        #endregion

        #region Phone Overlay Prefab

        private static void CreatePhoneOverlayPrefab()
        {
            var root = new GameObject("PhoneOverlay");
            var rootRect = root.AddComponent<RectTransform>();
            SetFullStretch(rootRect);

            // Semi-transparent background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(root.transform, false);
            var bgRect = bgObj.AddComponent<RectTransform>();
            SetFullStretch(bgRect);
            var bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0, 0, 0, 0.7f);

            // Phone frame
            var phoneFrame = new GameObject("PhoneFrame");
            phoneFrame.transform.SetParent(root.transform, false);
            var phoneRect = phoneFrame.AddComponent<RectTransform>();
            phoneRect.anchorMin = new Vector2(0.05f, 0.1f);
            phoneRect.anchorMax = new Vector2(0.95f, 0.9f);
            phoneRect.offsetMin = Vector2.zero;
            phoneRect.offsetMax = Vector2.zero;
            var phoneImg = phoneFrame.AddComponent<Image>();
            phoneImg.color = new Color(0.12f, 0.12f, 0.18f, 1f);

            // Header
            var header = new GameObject("Header");
            header.transform.SetParent(phoneFrame.transform, false);
            var headerRect = header.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.sizeDelta = new Vector2(0, 60);
            var headerImg = header.AddComponent<Image>();
            headerImg.color = new Color(0.15f, 0.15f, 0.22f, 1f);

            // Header title
            var title = CreateTextElement("Title", header.transform, "Phone", 24, Color.white, TextAlignmentOptions.Center);
            SetFullStretch(title.GetComponent<RectTransform>());

            // Close button
            var closeBtn = CreateButtonElement("CloseButton", header.transform, "X", new Color(0.8f, 0.3f, 0.3f));
            var closeRect = closeBtn.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(1, 0.5f);
            closeRect.anchorMax = new Vector2(1, 0.5f);
            closeRect.pivot = new Vector2(1, 0.5f);
            closeRect.sizeDelta = new Vector2(50, 40);
            closeRect.anchoredPosition = new Vector2(-10, 0);

            // App grid container
            var appGrid = new GameObject("AppGrid");
            appGrid.transform.SetParent(phoneFrame.transform, false);
            var gridRect = appGrid.AddComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0, 0);
            gridRect.anchorMax = new Vector2(1, 1);
            gridRect.offsetMin = new Vector2(20, 20);
            gridRect.offsetMax = new Vector2(-20, -80);

            var gridLayout = appGrid.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(100, 120);
            gridLayout.spacing = new Vector2(20, 20);
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.childAlignment = TextAnchor.UpperCenter;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 3;

            SavePrefab(root, PrefabPath + "/Overlays/PhoneOverlay.prefab");
        }

        #endregion

        #region Bottom Nav Prefab

        private static void CreateBottomNavPrefab()
        {
            var root = new GameObject("BottomNav");
            var rootRect = root.AddComponent<RectTransform>();

            // Configure as bottom bar
            rootRect.anchorMin = new Vector2(0, 0);
            rootRect.anchorMax = new Vector2(1, 0);
            rootRect.pivot = new Vector2(0.5f, 0);
            rootRect.sizeDelta = new Vector2(0, 100);
            rootRect.anchoredPosition = Vector2.zero;

            var bg = root.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.98f);

            // Horizontal layout
            var layout = root.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.spacing = 10;
            layout.padding = new RectOffset(15, 15, 15, 15);

            // Phone button
            var phoneBtn = CreateNavButton("PhoneButton", root.transform, "Phone", new Color(0.25f, 0.5f, 0.85f));
            // Map button
            var mapBtn = CreateNavButton("MapButton", root.transform, "Map", new Color(0.25f, 0.5f, 0.85f));
            // Menu button
            var menuBtn = CreateNavButton("MenuButton", root.transform, "Menu", new Color(0.25f, 0.5f, 0.85f));

            SavePrefab(root, PrefabPath + "/BottomNav.prefab");
        }

        private static GameObject CreateNavButton(string name, Transform parent, string label, Color color)
        {
            var btn = new GameObject(name);
            btn.transform.SetParent(parent, false);

            var rect = btn.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 70);

            var img = btn.AddComponent<Image>();
            img.color = color;

            var button = btn.AddComponent<Button>();
            button.targetGraphic = img;

            var labelObj = CreateTextElement("Label", btn.transform, label, 20, Color.white, TextAlignmentOptions.Center);
            SetFullStretch(labelObj.GetComponent<RectTransform>());

            var layoutElem = btn.AddComponent<LayoutElement>();
            layoutElem.flexibleWidth = 1;

            return btn;
        }

        #endregion

        #region Notification Prefab

        private static void CreateNotificationPrefab()
        {
            var root = new GameObject("Notification");
            var rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(400, 100);

            var bg = root.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.22f, 0.98f);

            // Icon area
            var icon = new GameObject("Icon");
            icon.transform.SetParent(root.transform, false);
            var iconRect = icon.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0, 0);
            iconRect.anchorMax = new Vector2(0, 1);
            iconRect.pivot = new Vector2(0, 0.5f);
            iconRect.sizeDelta = new Vector2(80, 0);
            iconRect.anchoredPosition = new Vector2(10, 0);
            var iconImg = icon.AddComponent<Image>();
            iconImg.color = new Color(0.3f, 0.6f, 0.9f);

            // Title
            var title = CreateTextElement("Title", root.transform, "Notification Title", 20, Color.white, TextAlignmentOptions.Left);
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.5f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.offsetMin = new Vector2(100, 5);
            titleRect.offsetMax = new Vector2(-10, -10);

            // Message
            var message = CreateTextElement("Message", root.transform, "Notification message goes here", 16, new Color(0.7f, 0.7f, 0.7f), TextAlignmentOptions.Left);
            var msgRect = message.GetComponent<RectTransform>();
            msgRect.anchorMin = new Vector2(0, 0);
            msgRect.anchorMax = new Vector2(1, 0.5f);
            msgRect.offsetMin = new Vector2(100, 10);
            msgRect.offsetMax = new Vector2(-10, -5);

            SavePrefab(root, PrefabPath + "/Components/Notification.prefab");
        }

        #endregion

        #region Button Prefab

        private static void CreateButtonPrefab()
        {
            var root = new GameObject("UIButton");
            var rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(200, 50);

            var img = root.AddComponent<Image>();
            img.color = new Color(0.25f, 0.5f, 0.85f);

            var btn = root.AddComponent<Button>();
            btn.targetGraphic = img;

            var label = CreateTextElement("Label", root.transform, "Button", 20, Color.white, TextAlignmentOptions.Center);
            SetFullStretch(label.GetComponent<RectTransform>());

            SavePrefab(root, PrefabPath + "/Components/UIButton.prefab");
        }

        #endregion

        #region Card Prefab

        private static void CreateCardPrefab()
        {
            var root = new GameObject("Card");
            var rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(300, 120);

            var bg = root.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.22f, 1f);

            // Title
            var title = CreateTextElement("Title", root.transform, "Card Title", 22, Color.white, TextAlignmentOptions.Left);
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.6f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.offsetMin = new Vector2(15, 0);
            titleRect.offsetMax = new Vector2(-15, -10);

            // Subtitle
            var subtitle = CreateTextElement("Subtitle", root.transform, "Subtitle or description", 16, new Color(0.6f, 0.6f, 0.6f), TextAlignmentOptions.Left);
            var subRect = subtitle.GetComponent<RectTransform>();
            subRect.anchorMin = new Vector2(0, 0.3f);
            subRect.anchorMax = new Vector2(1, 0.6f);
            subRect.offsetMin = new Vector2(15, 0);
            subRect.offsetMax = new Vector2(-15, 0);

            // Value/Status
            var value = CreateTextElement("Value", root.transform, "$500", 20, new Color(0.3f, 0.85f, 0.4f), TextAlignmentOptions.Left);
            var valueRect = value.GetComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(0, 0);
            valueRect.anchorMax = new Vector2(0.5f, 0.35f);
            valueRect.offsetMin = new Vector2(15, 10);
            valueRect.offsetMax = new Vector2(0, 0);

            SavePrefab(root, PrefabPath + "/Components/Card.prefab");
        }

        #endregion

        #region App Icon Prefab

        private static void CreateAppIconPrefab()
        {
            var root = new GameObject("AppIcon");
            var rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(100, 120);

            // Icon background
            var iconBg = new GameObject("IconBg");
            iconBg.transform.SetParent(root.transform, false);
            var iconRect = iconBg.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f, 0.25f);
            iconRect.anchorMax = new Vector2(0.9f, 0.95f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            var iconImg = iconBg.AddComponent<Image>();
            iconImg.color = new Color(0.25f, 0.5f, 0.85f);

            var btn = iconBg.AddComponent<Button>();
            btn.targetGraphic = iconImg;

            // Badge
            var badge = new GameObject("Badge");
            badge.transform.SetParent(iconBg.transform, false);
            var badgeRect = badge.AddComponent<RectTransform>();
            badgeRect.anchorMin = new Vector2(1, 1);
            badgeRect.anchorMax = new Vector2(1, 1);
            badgeRect.pivot = new Vector2(1, 1);
            badgeRect.sizeDelta = new Vector2(24, 24);
            badgeRect.anchoredPosition = new Vector2(5, 5);
            var badgeImg = badge.AddComponent<Image>();
            badgeImg.color = new Color(0.9f, 0.3f, 0.3f);
            badge.SetActive(false); // Hidden by default

            var badgeText = CreateTextElement("Count", badge.transform, "0", 14, Color.white, TextAlignmentOptions.Center);
            SetFullStretch(badgeText.GetComponent<RectTransform>());

            // Label
            var label = CreateTextElement("Label", root.transform, "App", 14, Color.white, TextAlignmentOptions.Center);
            var labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 0.25f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            SavePrefab(root, PrefabPath + "/Components/AppIcon.prefab");
        }

        #endregion

        #region Helpers

        private static GameObject CreateTextElement(string name, Transform parent, string text, int fontSize, Color color, TextAlignmentOptions alignment)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.AddComponent<RectTransform>();

            var tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;

            return obj;
        }

        private static GameObject CreateButtonElement(string name, Transform parent, string label, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.AddComponent<RectTransform>();

            var img = obj.AddComponent<Image>();
            img.color = color;

            var btn = obj.AddComponent<Button>();
            btn.targetGraphic = img;

            var labelObj = CreateTextElement("Label", obj.transform, label, 18, Color.white, TextAlignmentOptions.Center);
            SetFullStretch(labelObj.GetComponent<RectTransform>());

            return obj;
        }

        private static void SetFullStretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void SavePrefab(GameObject obj, string path)
        {
            // Ensure parent directory exists
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            PrefabUtility.SaveAsPrefabAsset(obj, path);
            Object.DestroyImmediate(obj);
            UnityEngine.Debug.Log($"[PrefabGenerator] Created: {path}");
        }

        // Command line entry point
        public static void GenerateFromCommandLine()
        {
            GenerateAllPrefabs();
            EditorApplication.Exit(0);
        }

        #endregion
    }
}
