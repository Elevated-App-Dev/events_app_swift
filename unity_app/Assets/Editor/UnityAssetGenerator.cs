using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.IO;

namespace EventPlannerSim.Editor
{
    /// <summary>
    /// Comprehensive Unity asset generator that creates all scenes and prefabs
    /// required by the Event Planner Simulator UI layer.
    ///
    /// This script addresses the limitation that Unity assets (scenes, prefabs)
    /// cannot be directly written as files - they must be created through Unity's APIs.
    ///
    /// Run via: Unity -batchmode -executeMethod EventPlannerSim.Editor.UnityAssetGenerator.GenerateAll -quit
    /// Or use menu: Tools > Generate All UI Assets
    /// </summary>
    public static class UnityAssetGenerator
    {
        // Path constants
        private const string PrefabRoot = "Assets/Prefabs/UI";
        private const string SceneRoot = "Assets/Scenes";
        private const string ScriptableObjectRoot = "Assets/ScriptableObjects";

        // Color palette (from design.md)
        private static readonly Color BgDark = new Color(0.1f, 0.1f, 0.15f, 1f);
        private static readonly Color BgPanel = new Color(0.12f, 0.12f, 0.18f, 0.95f);
        private static readonly Color BgCard = new Color(0.15f, 0.15f, 0.22f, 1f);
        private static readonly Color BgContent = new Color(0.08f, 0.08f, 0.12f, 0.9f);
        private static readonly Color AccentBlue = new Color(0.25f, 0.5f, 0.85f, 1f);
        private static readonly Color AccentGreen = new Color(0.3f, 0.85f, 0.4f, 1f);
        private static readonly Color AccentGold = new Color(0.9f, 0.7f, 0.2f, 1f);
        private static readonly Color AccentRed = new Color(0.85f, 0.3f, 0.3f, 1f);
        private static readonly Color TextWhite = Color.white;
        private static readonly Color TextGray = new Color(0.7f, 0.7f, 0.7f, 1f);
        private static readonly Color TextDimmed = new Color(0.5f, 0.5f, 0.5f, 1f);

        [MenuItem("Tools/Generate All UI Assets")]
        public static void GenerateAll()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Starting comprehensive asset generation...");

            EnsureAllDirectories();

            // Generate all prefabs by category
            GenerateCommonPrefabs();
            GenerateHUDPrefabs();
            GeneratePhonePrefabs();
            GenerateMapPrefabs();
            GenerateEventPlanningPrefabs();
            GenerateEventExecutionPrefabs();
            GenerateResultsPrefabs();
            GenerateTutorialPrefabs();
            GenerateNotificationPrefabs();
            GenerateSettingsPrefabs();
            GenerateMilestonePrefabs();
            GenerateMainMenuPrefabs();
            GenerateLoadingPrefabs();

            // Generate scenes
            GenerateMainMenuScene();
            GenerateGameplayMainScene();

            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("[UnityAssetGenerator] Asset generation complete!");
        }

        #region Directory Setup

        private static void EnsureAllDirectories()
        {
            string[] dirs = {
                PrefabRoot,
                $"{PrefabRoot}/Common",
                $"{PrefabRoot}/HUD",
                $"{PrefabRoot}/Phone",
                $"{PrefabRoot}/Map",
                $"{PrefabRoot}/EventPlanning",
                $"{PrefabRoot}/EventExecution",
                $"{PrefabRoot}/Results",
                $"{PrefabRoot}/Tutorial",
                $"{PrefabRoot}/Notifications",
                $"{PrefabRoot}/Settings",
                $"{PrefabRoot}/Milestone",
                $"{PrefabRoot}/MainMenu",
                $"{PrefabRoot}/Loading",
                SceneRoot,
                ScriptableObjectRoot,
                $"{ScriptableObjectRoot}/Venues",
                $"{ScriptableObjectRoot}/Vendors",
                $"{ScriptableObjectRoot}/EventTypes",
                $"{ScriptableObjectRoot}/Monetization"
            };

            foreach (var dir in dirs)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    UnityEngine.Debug.Log($"[UnityAssetGenerator] Created directory: {dir}");
                }
            }
        }

        #endregion

        #region Common Prefabs

        private static void GenerateCommonPrefabs()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating common prefabs...");

            // Button.prefab
            CreateButtonPrefab();
            // Slider.prefab
            CreateSliderPrefab();
            // Toggle.prefab
            CreateTogglePrefab();
            // Card.prefab
            CreateCardPrefab();
            // Badge.prefab
            CreateBadgePrefab();
        }

        private static void CreateButtonPrefab()
        {
            var root = new GameObject("Button");
            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 50);

            var img = root.AddComponent<Image>();
            img.color = AccentBlue;

            var btn = root.AddComponent<Button>();
            btn.targetGraphic = img;

            var label = CreateTMP("Label", root.transform, "Button", 20, TextWhite, TextAlignmentOptions.Center);
            StretchFill(label.GetComponent<RectTransform>());

            SavePrefab(root, $"{PrefabRoot}/Common/Button.prefab");
        }

        private static void CreateSliderPrefab()
        {
            var root = new GameObject("Slider");
            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 30);

            // Background track
            var bg = new GameObject("Background");
            bg.transform.SetParent(root.transform, false);
            var bgRect = bg.AddComponent<RectTransform>();
            StretchFill(bgRect);
            bgRect.offsetMin = new Vector2(0, 10);
            bgRect.offsetMax = new Vector2(0, -10);
            var bgImg = bg.AddComponent<Image>();
            bgImg.color = BgCard;

            // Fill area
            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(root.transform, false);
            var fillAreaRect = fillArea.AddComponent<RectTransform>();
            StretchFill(fillAreaRect);
            fillAreaRect.offsetMin = new Vector2(5, 12);
            fillAreaRect.offsetMax = new Vector2(-5, -12);

            var fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            var fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0.5f, 1);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            var fillImg = fill.AddComponent<Image>();
            fillImg.color = AccentBlue;

            // Handle area
            var handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(root.transform, false);
            var handleAreaRect = handleArea.AddComponent<RectTransform>();
            StretchFill(handleAreaRect);
            handleAreaRect.offsetMin = new Vector2(10, 0);
            handleAreaRect.offsetMax = new Vector2(-10, 0);

            var handle = new GameObject("Handle");
            handle.transform.SetParent(handleArea.transform, false);
            var handleRect = handle.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 30);
            var handleImg = handle.AddComponent<Image>();
            handleImg.color = TextWhite;

            // Slider component
            var slider = root.AddComponent<Slider>();
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImg;
            slider.direction = Slider.Direction.LeftToRight;

            SavePrefab(root, $"{PrefabRoot}/Common/Slider.prefab");
        }

        private static void CreateTogglePrefab()
        {
            var root = new GameObject("Toggle");
            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 40);

            // Background
            var bg = new GameObject("Background");
            bg.transform.SetParent(root.transform, false);
            var bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0.5f);
            bgRect.anchorMax = new Vector2(0, 0.5f);
            bgRect.pivot = new Vector2(0, 0.5f);
            bgRect.sizeDelta = new Vector2(40, 40);
            bgRect.anchoredPosition = Vector2.zero;
            var bgImg = bg.AddComponent<Image>();
            bgImg.color = BgCard;

            // Checkmark
            var checkmark = new GameObject("Checkmark");
            checkmark.transform.SetParent(bg.transform, false);
            var checkRect = checkmark.AddComponent<RectTransform>();
            StretchFill(checkRect);
            checkRect.offsetMin = new Vector2(5, 5);
            checkRect.offsetMax = new Vector2(-5, -5);
            var checkImg = checkmark.AddComponent<Image>();
            checkImg.color = AccentGreen;

            // Label
            var label = CreateTMP("Label", root.transform, "Toggle", 18, TextWhite, TextAlignmentOptions.Left);
            var labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 1);
            labelRect.offsetMin = new Vector2(50, 0);
            labelRect.offsetMax = Vector2.zero;

            // Toggle component
            var toggle = root.AddComponent<Toggle>();
            toggle.targetGraphic = bgImg;
            toggle.graphic = checkImg;
            toggle.isOn = true;

            SavePrefab(root, $"{PrefabRoot}/Common/Toggle.prefab");
        }

        private static void CreateCardPrefab()
        {
            var root = new GameObject("Card");
            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(320, 120);

            var bg = root.AddComponent<Image>();
            bg.color = BgCard;

            // Title
            var title = CreateTMP("Title", root.transform, "Card Title", 22, TextWhite, TextAlignmentOptions.Left);
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.6f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.offsetMin = new Vector2(15, 0);
            titleRect.offsetMax = new Vector2(-15, -10);

            // Subtitle
            var subtitle = CreateTMP("Subtitle", root.transform, "Description text", 16, TextGray, TextAlignmentOptions.Left);
            var subRect = subtitle.GetComponent<RectTransform>();
            subRect.anchorMin = new Vector2(0, 0.3f);
            subRect.anchorMax = new Vector2(1, 0.6f);
            subRect.offsetMin = new Vector2(15, 0);
            subRect.offsetMax = new Vector2(-15, 0);

            // Value
            var value = CreateTMP("Value", root.transform, "$500", 20, AccentGreen, TextAlignmentOptions.Left);
            var valueRect = value.GetComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(0, 0);
            valueRect.anchorMax = new Vector2(0.5f, 0.35f);
            valueRect.offsetMin = new Vector2(15, 10);
            valueRect.offsetMax = Vector2.zero;

            SavePrefab(root, $"{PrefabRoot}/Common/Card.prefab");
        }

        private static void CreateBadgePrefab()
        {
            var root = new GameObject("Badge");
            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(24, 24);

            var img = root.AddComponent<Image>();
            img.color = AccentRed;

            var count = CreateTMP("Count", root.transform, "0", 14, TextWhite, TextAlignmentOptions.Center);
            StretchFill(count.GetComponent<RectTransform>());

            SavePrefab(root, $"{PrefabRoot}/Common/Badge.prefab");
        }

        #endregion

        #region HUD Prefabs

        private static void GenerateHUDPrefabs()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating HUD prefabs...");

            // HUD.prefab (complete top+bottom bar)
            CreateHUDPrefab();
            // TopBar.prefab
            CreateTopBarPrefab();
            // BottomBar.prefab
            CreateBottomBarPrefab();
        }

        private static void CreateHUDPrefab()
        {
            var root = new GameObject("HUD");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            // This is just a container - TopBar and BottomBar are separate prefabs
            // to be instantiated as children

            SavePrefab(root, $"{PrefabRoot}/HUD/HUD.prefab");
        }

        private static void CreateTopBarPrefab()
        {
            var root = new GameObject("TopBar");
            var rect = root.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, 80);
            rect.anchoredPosition = Vector2.zero;

            var bg = root.AddComponent<Image>();
            bg.color = BgPanel;

            // Date (left)
            var date = CreateTMP("DateText", root.transform, "Jan 15", 24, TextWhite, TextAlignmentOptions.Left);
            var dateRect = date.GetComponent<RectTransform>();
            dateRect.anchorMin = new Vector2(0, 0);
            dateRect.anchorMax = new Vector2(0.3f, 1);
            dateRect.offsetMin = new Vector2(20, 10);
            dateRect.offsetMax = new Vector2(0, -10);

            // Money (center-right)
            var money = CreateTMP("MoneyText", root.transform, "$2,500", 28, AccentGreen, TextAlignmentOptions.Right);
            var moneyRect = money.GetComponent<RectTransform>();
            moneyRect.anchorMin = new Vector2(0.4f, 0);
            moneyRect.anchorMax = new Vector2(0.7f, 1);
            moneyRect.offsetMin = new Vector2(0, 10);
            moneyRect.offsetMax = new Vector2(-10, -10);

            // Reputation (right)
            var repLabel = CreateTMP("RepLabel", root.transform, "Rep:", 16, TextGray, TextAlignmentOptions.Right);
            var repLabelRect = repLabel.GetComponent<RectTransform>();
            repLabelRect.anchorMin = new Vector2(0.7f, 0);
            repLabelRect.anchorMax = new Vector2(0.82f, 1);
            repLabelRect.offsetMin = new Vector2(0, 10);
            repLabelRect.offsetMax = new Vector2(0, -10);

            var rep = CreateTMP("ReputationText", root.transform, "50", 28, AccentGold, TextAlignmentOptions.Right);
            var repRect = rep.GetComponent<RectTransform>();
            repRect.anchorMin = new Vector2(0.82f, 0);
            repRect.anchorMax = new Vector2(1, 1);
            repRect.offsetMin = new Vector2(0, 10);
            repRect.offsetMax = new Vector2(-20, -10);

            SavePrefab(root, $"{PrefabRoot}/HUD/TopBar.prefab");
        }

        private static void CreateBottomBarPrefab()
        {
            var root = new GameObject("BottomBar");
            var rect = root.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(0, 100);
            rect.anchoredPosition = Vector2.zero;

            var bg = root.AddComponent<Image>();
            bg.color = BgPanel;

            var layout = root.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.spacing = 15;
            layout.padding = new RectOffset(20, 20, 15, 15);

            // Phone button
            CreateNavButton("PhoneButton", root.transform, "Phone");
            // Map button
            CreateNavButton("MapButton", root.transform, "Map");
            // Settings button
            CreateNavButton("SettingsButton", root.transform, "Menu");

            SavePrefab(root, $"{PrefabRoot}/HUD/BottomBar.prefab");
        }

        private static void CreateNavButton(string name, Transform parent, string label)
        {
            var btn = new GameObject(name);
            btn.transform.SetParent(parent, false);

            var rect = btn.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 70);

            var img = btn.AddComponent<Image>();
            img.color = AccentBlue;

            var button = btn.AddComponent<Button>();
            button.targetGraphic = img;

            var labelObj = CreateTMP("Label", btn.transform, label, 20, TextWhite, TextAlignmentOptions.Center);
            StretchFill(labelObj.GetComponent<RectTransform>());

            var layoutElem = btn.AddComponent<LayoutElement>();
            layoutElem.flexibleWidth = 1;
        }

        #endregion

        #region Phone Prefabs

        private static void GeneratePhonePrefabs()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating Phone prefabs...");

            CreatePhoneOverlayPrefab();
            CreateAppIconPrefab();
            CreateCalendarAppPrefab();
            CreateMessagesAppPrefab();
            CreateBankAppPrefab();
            CreateContactsAppPrefab();
            CreateReviewsAppPrefab();
            CreateTasksAppPrefab();
            CreateClientsAppPrefab();
        }

        private static void CreatePhoneOverlayPrefab()
        {
            var root = new GameObject("PhoneOverlay");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            // Dim background
            var dimBg = CreatePanel("DimBackground", root.transform, new Color(0, 0, 0, 0.7f));
            StretchFill(dimBg.GetComponent<RectTransform>());

            // Phone frame
            var phoneFrame = CreatePanel("PhoneFrame", root.transform, BgPanel);
            var frameRect = phoneFrame.GetComponent<RectTransform>();
            frameRect.anchorMin = new Vector2(0.05f, 0.08f);
            frameRect.anchorMax = new Vector2(0.95f, 0.92f);
            frameRect.offsetMin = Vector2.zero;
            frameRect.offsetMax = Vector2.zero;

            // Header
            var header = CreatePanel("Header", phoneFrame.transform, BgCard);
            var headerRect = header.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.sizeDelta = new Vector2(0, 60);
            headerRect.anchoredPosition = Vector2.zero;

            var title = CreateTMP("Title", header.transform, "Phone", 24, TextWhite, TextAlignmentOptions.Center);
            StretchFill(title.GetComponent<RectTransform>());

            // Back button
            var backBtn = CreateButtonObj("BackButton", header.transform, "<", new Color(0.4f, 0.4f, 0.5f));
            var backRect = backBtn.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0, 0.5f);
            backRect.anchorMax = new Vector2(0, 0.5f);
            backRect.pivot = new Vector2(0, 0.5f);
            backRect.sizeDelta = new Vector2(50, 40);
            backRect.anchoredPosition = new Vector2(10, 0);
            backBtn.SetActive(false); // Hidden on home screen

            // Close button
            var closeBtn = CreateButtonObj("CloseButton", header.transform, "X", AccentRed);
            var closeRect = closeBtn.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(1, 0.5f);
            closeRect.anchorMax = new Vector2(1, 0.5f);
            closeRect.pivot = new Vector2(1, 0.5f);
            closeRect.sizeDelta = new Vector2(50, 40);
            closeRect.anchoredPosition = new Vector2(-10, 0);

            // Home screen with app grid
            var homeScreen = new GameObject("HomeScreen");
            homeScreen.transform.SetParent(phoneFrame.transform, false);
            var homeRect = homeScreen.AddComponent<RectTransform>();
            homeRect.anchorMin = Vector2.zero;
            homeRect.anchorMax = Vector2.one;
            homeRect.offsetMin = new Vector2(20, 20);
            homeRect.offsetMax = new Vector2(-20, -80);

            var grid = homeScreen.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(100, 120);
            grid.spacing = new Vector2(20, 20);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 3;

            // App content area (hidden by default)
            var appContent = new GameObject("AppContentArea");
            appContent.transform.SetParent(phoneFrame.transform, false);
            var appRect = appContent.AddComponent<RectTransform>();
            appRect.anchorMin = Vector2.zero;
            appRect.anchorMax = Vector2.one;
            appRect.offsetMin = new Vector2(10, 10);
            appRect.offsetMax = new Vector2(-10, -70);
            appContent.SetActive(false);

            SavePrefab(root, $"{PrefabRoot}/Phone/PhoneOverlay.prefab");
        }

        private static void CreateAppIconPrefab()
        {
            var root = new GameObject("AppIcon");
            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 120);

            // Icon background
            var iconBg = CreatePanel("IconBg", root.transform, AccentBlue);
            var iconRect = iconBg.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f, 0.25f);
            iconRect.anchorMax = new Vector2(0.9f, 0.95f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            var btn = iconBg.AddComponent<Button>();
            btn.targetGraphic = iconBg.GetComponent<Image>();

            // Badge
            var badge = CreatePanel("Badge", iconBg.transform, AccentRed);
            var badgeRect = badge.GetComponent<RectTransform>();
            badgeRect.anchorMin = new Vector2(1, 1);
            badgeRect.anchorMax = new Vector2(1, 1);
            badgeRect.pivot = new Vector2(1, 1);
            badgeRect.sizeDelta = new Vector2(24, 24);
            badgeRect.anchoredPosition = new Vector2(5, 5);
            badge.SetActive(false);

            var badgeCount = CreateTMP("Count", badge.transform, "0", 14, TextWhite, TextAlignmentOptions.Center);
            StretchFill(badgeCount.GetComponent<RectTransform>());

            // Label
            var label = CreateTMP("Label", root.transform, "App", 14, TextWhite, TextAlignmentOptions.Center);
            var labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 0.25f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            SavePrefab(root, $"{PrefabRoot}/Phone/AppIcon.prefab");
        }

        private static void CreateCalendarAppPrefab()
        {
            var root = CreateAppViewTemplate("CalendarApp", "Calendar");
            // Add calendar-specific content placeholder
            var content = root.transform.Find("Content");
            CreateTMP("Placeholder", content, "Calendar events will appear here", 16, TextGray, TextAlignmentOptions.Center);
            SavePrefab(root, $"{PrefabRoot}/Phone/CalendarApp.prefab");
        }

        private static void CreateMessagesAppPrefab()
        {
            var root = CreateAppViewTemplate("MessagesApp", "Messages");
            var content = root.transform.Find("Content");
            CreateTMP("Placeholder", content, "Message threads will appear here", 16, TextGray, TextAlignmentOptions.Center);
            SavePrefab(root, $"{PrefabRoot}/Phone/MessagesApp.prefab");
        }

        private static void CreateBankAppPrefab()
        {
            var root = CreateAppViewTemplate("BankApp", "Bank");
            var content = root.transform.Find("Content");

            // Balance display
            var balanceLabel = CreateTMP("BalanceLabel", content, "Current Balance", 16, TextGray, TextAlignmentOptions.Center);
            var blRect = balanceLabel.GetComponent<RectTransform>();
            blRect.anchorMin = new Vector2(0.1f, 0.8f);
            blRect.anchorMax = new Vector2(0.9f, 0.9f);
            blRect.offsetMin = Vector2.zero;
            blRect.offsetMax = Vector2.zero;

            var balance = CreateTMP("BalanceText", content, "$2,500", 42, AccentGreen, TextAlignmentOptions.Center);
            var bRect = balance.GetComponent<RectTransform>();
            bRect.anchorMin = new Vector2(0.1f, 0.65f);
            bRect.anchorMax = new Vector2(0.9f, 0.8f);
            bRect.offsetMin = Vector2.zero;
            bRect.offsetMax = Vector2.zero;

            SavePrefab(root, $"{PrefabRoot}/Phone/BankApp.prefab");
        }

        private static void CreateContactsAppPrefab()
        {
            var root = CreateAppViewTemplate("ContactsApp", "Contacts");
            var content = root.transform.Find("Content");
            CreateTMP("Placeholder", content, "Vendor contacts will appear here", 16, TextGray, TextAlignmentOptions.Center);
            SavePrefab(root, $"{PrefabRoot}/Phone/ContactsApp.prefab");
        }

        private static void CreateReviewsAppPrefab()
        {
            var root = CreateAppViewTemplate("ReviewsApp", "Reviews");
            var content = root.transform.Find("Content");
            CreateTMP("Placeholder", content, "Client reviews will appear here", 16, TextGray, TextAlignmentOptions.Center);
            SavePrefab(root, $"{PrefabRoot}/Phone/ReviewsApp.prefab");
        }

        private static void CreateTasksAppPrefab()
        {
            var root = CreateAppViewTemplate("TasksApp", "Tasks");
            var content = root.transform.Find("Content");
            CreateTMP("Placeholder", content, "Event tasks will appear here", 16, TextGray, TextAlignmentOptions.Center);
            SavePrefab(root, $"{PrefabRoot}/Phone/TasksApp.prefab");
        }

        private static void CreateClientsAppPrefab()
        {
            var root = CreateAppViewTemplate("ClientsApp", "Clients");
            var content = root.transform.Find("Content");
            CreateTMP("Placeholder", content, "Client history will appear here", 16, TextGray, TextAlignmentOptions.Center);
            SavePrefab(root, $"{PrefabRoot}/Phone/ClientsApp.prefab");
        }

        private static GameObject CreateAppViewTemplate(string name, string title)
        {
            var root = new GameObject(name);
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            var bg = root.AddComponent<Image>();
            bg.color = BgContent;

            // Header
            var header = CreatePanel("Header", root.transform, BgCard);
            var headerRect = header.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(1, 1);
            headerRect.pivot = new Vector2(0.5f, 1);
            headerRect.sizeDelta = new Vector2(0, 50);
            headerRect.anchoredPosition = Vector2.zero;

            var titleText = CreateTMP("Title", header.transform, title, 22, TextWhite, TextAlignmentOptions.Center);
            StretchFill(titleText.GetComponent<RectTransform>());

            // Content area
            var content = new GameObject("Content");
            content.transform.SetParent(root.transform, false);
            var contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.offsetMin = new Vector2(10, 10);
            contentRect.offsetMax = new Vector2(-10, -60);

            return root;
        }

        #endregion

        #region Map Prefabs

        private static void GenerateMapPrefabs()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating Map prefabs...");

            CreateMapOverlayPrefab();
            CreateLocationPinPrefab();
            CreatePreviewCardPrefab();
        }

        private static void CreateMapOverlayPrefab()
        {
            var root = new GameObject("MapOverlay");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            // Map background
            var mapBg = CreatePanel("MapBackground", root.transform, new Color(0.2f, 0.25f, 0.2f));
            StretchFill(mapBg.GetComponent<RectTransform>());

            // Zone buttons container (top)
            var zoneBar = CreatePanel("ZoneBar", root.transform, BgPanel);
            var zoneRect = zoneBar.GetComponent<RectTransform>();
            zoneRect.anchorMin = new Vector2(0, 1);
            zoneRect.anchorMax = new Vector2(1, 1);
            zoneRect.pivot = new Vector2(0.5f, 1);
            zoneRect.sizeDelta = new Vector2(0, 60);
            zoneRect.anchoredPosition = Vector2.zero;

            var zoneLayout = zoneBar.AddComponent<HorizontalLayoutGroup>();
            zoneLayout.childAlignment = TextAnchor.MiddleCenter;
            zoneLayout.spacing = 10;
            zoneLayout.padding = new RectOffset(10, 10, 5, 5);

            // Filter bar (bottom)
            var filterBar = CreatePanel("FilterBar", root.transform, BgPanel);
            var filterRect = filterBar.GetComponent<RectTransform>();
            filterRect.anchorMin = new Vector2(0, 0);
            filterRect.anchorMax = new Vector2(1, 0);
            filterRect.pivot = new Vector2(0.5f, 0);
            filterRect.sizeDelta = new Vector2(0, 50);
            filterRect.anchoredPosition = Vector2.zero;

            // Location pins container
            var pinsContainer = new GameObject("LocationPins");
            pinsContainer.transform.SetParent(root.transform, false);
            var pinsRect = pinsContainer.AddComponent<RectTransform>();
            StretchFill(pinsRect);

            // Preview card (hidden)
            var previewCard = CreatePanel("PreviewCard", root.transform, BgCard);
            var previewRect = previewCard.GetComponent<RectTransform>();
            previewRect.anchorMin = new Vector2(0.1f, 0.1f);
            previewRect.anchorMax = new Vector2(0.9f, 0.4f);
            previewRect.offsetMin = Vector2.zero;
            previewRect.offsetMax = Vector2.zero;
            previewCard.SetActive(false);

            // Close button
            var closeBtn = CreateButtonObj("CloseButton", root.transform, "X", AccentRed);
            var closeRect = closeBtn.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(1, 1);
            closeRect.anchorMax = new Vector2(1, 1);
            closeRect.pivot = new Vector2(1, 1);
            closeRect.sizeDelta = new Vector2(50, 50);
            closeRect.anchoredPosition = new Vector2(-10, -70);

            SavePrefab(root, $"{PrefabRoot}/Map/MapOverlay.prefab");
        }

        private static void CreateLocationPinPrefab()
        {
            var root = new GameObject("LocationPin");
            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(40, 50);

            var img = root.AddComponent<Image>();
            img.color = AccentBlue;

            var btn = root.AddComponent<Button>();
            btn.targetGraphic = img;

            SavePrefab(root, $"{PrefabRoot}/Map/LocationPin.prefab");
        }

        private static void CreatePreviewCardPrefab()
        {
            var root = new GameObject("PreviewCard");
            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 200);

            var bg = root.AddComponent<Image>();
            bg.color = BgCard;

            // Name
            var name = CreateTMP("Name", root.transform, "Location Name", 24, TextWhite, TextAlignmentOptions.Left);
            var nameRect = name.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.7f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.offsetMin = new Vector2(15, 0);
            nameRect.offsetMax = new Vector2(-15, -10);

            // Description
            var desc = CreateTMP("Description", root.transform, "Description of this location", 16, TextGray, TextAlignmentOptions.Left);
            var descRect = desc.GetComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0.35f);
            descRect.anchorMax = new Vector2(1, 0.7f);
            descRect.offsetMin = new Vector2(15, 0);
            descRect.offsetMax = new Vector2(-15, 0);

            // Visit button
            var visitBtn = CreateButtonObj("VisitButton", root.transform, "Visit", AccentBlue);
            var visitRect = visitBtn.GetComponent<RectTransform>();
            visitRect.anchorMin = new Vector2(0.6f, 0.05f);
            visitRect.anchorMax = new Vector2(0.95f, 0.3f);
            visitRect.offsetMin = Vector2.zero;
            visitRect.offsetMax = Vector2.zero;

            SavePrefab(root, $"{PrefabRoot}/Map/PreviewCard.prefab");
        }

        #endregion

        #region Event Planning Prefabs

        private static void GenerateEventPlanningPrefabs()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating Event Planning prefabs...");

            CreateClientInquiryPanelPrefab();
            CreateBudgetAllocationPanelPrefab();
            CreateVenueSelectionPanelPrefab();
            CreateVendorSelectionPanelPrefab();
            CreateVenueCardPrefab();
            CreateVendorCardPrefab();
        }

        private static void CreateClientInquiryPanelPrefab()
        {
            var root = new GameObject("ClientInquiryPanel");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            var bg = root.AddComponent<Image>();
            bg.color = BgContent;

            // Header
            var header = CreateTMP("Header", root.transform, "New Client Inquiry!", 28, AccentGold, TextAlignmentOptions.Center);
            var headerRect = header.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 0.85f);
            headerRect.anchorMax = new Vector2(1, 0.95f);
            headerRect.offsetMin = new Vector2(20, 0);
            headerRect.offsetMax = new Vector2(-20, 0);

            // Client info section
            var clientName = CreateTMP("ClientName", root.transform, "Client Name", 22, TextWhite, TextAlignmentOptions.Left);
            SetAnchoredRect(clientName.GetComponent<RectTransform>(), 0.05f, 0.72f, 0.95f, 0.82f);

            var eventType = CreateTMP("EventType", root.transform, "Birthday Party", 18, TextGray, TextAlignmentOptions.Left);
            SetAnchoredRect(eventType.GetComponent<RectTransform>(), 0.05f, 0.64f, 0.95f, 0.72f);

            var budget = CreateTMP("Budget", root.transform, "Budget: $2,000", 20, AccentGreen, TextAlignmentOptions.Left);
            SetAnchoredRect(budget.GetComponent<RectTransform>(), 0.05f, 0.54f, 0.5f, 0.62f);

            var guests = CreateTMP("Guests", root.transform, "50 guests", 20, TextWhite, TextAlignmentOptions.Right);
            SetAnchoredRect(guests.GetComponent<RectTransform>(), 0.5f, 0.54f, 0.95f, 0.62f);

            var eventDate = CreateTMP("EventDate", root.transform, "Event Date: Jan 20", 18, TextWhite, TextAlignmentOptions.Left);
            SetAnchoredRect(eventDate.GetComponent<RectTransform>(), 0.05f, 0.44f, 0.95f, 0.52f);

            var expiration = CreateTMP("Expiration", root.transform, "Expires in: 2 days", 16, AccentRed, TextAlignmentOptions.Center);
            SetAnchoredRect(expiration.GetComponent<RectTransform>(), 0.05f, 0.34f, 0.95f, 0.42f);

            // Buttons
            var acceptBtn = CreateButtonObj("AcceptButton", root.transform, "Accept", AccentGreen);
            SetAnchoredRect(acceptBtn.GetComponent<RectTransform>(), 0.55f, 0.08f, 0.95f, 0.18f);

            var declineBtn = CreateButtonObj("DeclineButton", root.transform, "Decline", AccentRed);
            SetAnchoredRect(declineBtn.GetComponent<RectTransform>(), 0.05f, 0.08f, 0.45f, 0.18f);

            SavePrefab(root, $"{PrefabRoot}/EventPlanning/ClientInquiryPanel.prefab");
        }

        private static void CreateBudgetAllocationPanelPrefab()
        {
            var root = new GameObject("BudgetAllocationPanel");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            var bg = root.AddComponent<Image>();
            bg.color = BgContent;

            // Header
            var header = CreateTMP("Header", root.transform, "Budget Allocation", 28, TextWhite, TextAlignmentOptions.Center);
            SetAnchoredRect(header.GetComponent<RectTransform>(), 0, 0.9f, 1, 0.98f);

            // Total budget
            var total = CreateTMP("TotalBudget", root.transform, "Total: $2,000", 24, AccentGreen, TextAlignmentOptions.Center);
            SetAnchoredRect(total.GetComponent<RectTransform>(), 0, 0.82f, 1, 0.9f);

            // Sliders area - placeholder text
            var slidersNote = CreateTMP("SlidersNote", root.transform, "Category allocation sliders will be here:\n\nVenue | Catering | Entertainment\nDecorations | Staffing | Contingency", 16, TextGray, TextAlignmentOptions.Center);
            SetAnchoredRect(slidersNote.GetComponent<RectTransform>(), 0.05f, 0.3f, 0.95f, 0.8f);

            // Remaining
            var remaining = CreateTMP("RemainingBudget", root.transform, "Remaining: $0", 20, TextWhite, TextAlignmentOptions.Center);
            SetAnchoredRect(remaining.GetComponent<RectTransform>(), 0, 0.2f, 1, 0.28f);

            // Confirm button
            var confirmBtn = CreateButtonObj("ConfirmButton", root.transform, "Confirm Allocations", AccentBlue);
            SetAnchoredRect(confirmBtn.GetComponent<RectTransform>(), 0.2f, 0.05f, 0.8f, 0.15f);

            SavePrefab(root, $"{PrefabRoot}/EventPlanning/BudgetAllocationPanel.prefab");
        }

        private static void CreateVenueSelectionPanelPrefab()
        {
            var root = new GameObject("VenueSelectionPanel");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            var bg = root.AddComponent<Image>();
            bg.color = BgContent;

            // Header
            var header = CreateTMP("Header", root.transform, "Select Venue", 28, TextWhite, TextAlignmentOptions.Center);
            SetAnchoredRect(header.GetComponent<RectTransform>(), 0, 0.9f, 1, 0.98f);

            // Venue list container (scroll view placeholder)
            var listContainer = CreatePanel("VenueList", root.transform, BgDark);
            SetAnchoredRect(listContainer.GetComponent<RectTransform>(), 0.03f, 0.15f, 0.97f, 0.88f);

            // Confirm button
            var confirmBtn = CreateButtonObj("ConfirmButton", root.transform, "Confirm Venue", AccentBlue);
            SetAnchoredRect(confirmBtn.GetComponent<RectTransform>(), 0.2f, 0.03f, 0.8f, 0.12f);

            SavePrefab(root, $"{PrefabRoot}/EventPlanning/VenueSelectionPanel.prefab");
        }

        private static void CreateVendorSelectionPanelPrefab()
        {
            var root = new GameObject("VendorSelectionPanel");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            var bg = root.AddComponent<Image>();
            bg.color = BgContent;

            // Header
            var header = CreateTMP("Header", root.transform, "Select Vendors", 28, TextWhite, TextAlignmentOptions.Center);
            SetAnchoredRect(header.GetComponent<RectTransform>(), 0, 0.9f, 1, 0.98f);

            // Category tabs placeholder
            var tabsContainer = CreatePanel("CategoryTabs", root.transform, BgPanel);
            SetAnchoredRect(tabsContainer.GetComponent<RectTransform>(), 0, 0.82f, 1, 0.9f);

            // Vendor list container
            var listContainer = CreatePanel("VendorList", root.transform, BgDark);
            SetAnchoredRect(listContainer.GetComponent<RectTransform>(), 0.03f, 0.15f, 0.97f, 0.8f);

            // Complete button
            var completeBtn = CreateButtonObj("CompleteButton", root.transform, "Complete Planning", AccentGreen);
            SetAnchoredRect(completeBtn.GetComponent<RectTransform>(), 0.2f, 0.03f, 0.8f, 0.12f);

            SavePrefab(root, $"{PrefabRoot}/EventPlanning/VendorSelectionPanel.prefab");
        }

        private static void CreateVenueCardPrefab()
        {
            var root = new GameObject("VenueCard");
            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 140);

            var bg = root.AddComponent<Image>();
            bg.color = BgCard;

            var btn = root.AddComponent<Button>();
            btn.targetGraphic = bg;

            // Venue name
            var name = CreateTMP("VenueName", root.transform, "Venue Name", 22, TextWhite, TextAlignmentOptions.Left);
            SetAnchoredRect(name.GetComponent<RectTransform>(), 0.03f, 0.65f, 0.7f, 0.95f);

            // Capacity
            var capacity = CreateTMP("Capacity", root.transform, "Capacity: 50", 16, TextGray, TextAlignmentOptions.Left);
            SetAnchoredRect(capacity.GetComponent<RectTransform>(), 0.03f, 0.35f, 0.5f, 0.6f);

            // Type
            var type = CreateTMP("Type", root.transform, "Backyard", 16, TextGray, TextAlignmentOptions.Left);
            SetAnchoredRect(type.GetComponent<RectTransform>(), 0.03f, 0.1f, 0.5f, 0.35f);

            // Price
            var price = CreateTMP("Price", root.transform, "$500", 24, AccentGreen, TextAlignmentOptions.Right);
            SetAnchoredRect(price.GetComponent<RectTransform>(), 0.6f, 0.5f, 0.97f, 0.9f);

            SavePrefab(root, $"{PrefabRoot}/EventPlanning/VenueCard.prefab");
        }

        private static void CreateVendorCardPrefab()
        {
            var root = new GameObject("VendorCard");
            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 120);

            var bg = root.AddComponent<Image>();
            bg.color = BgCard;

            // Vendor name
            var name = CreateTMP("VendorName", root.transform, "Vendor Name", 20, TextWhite, TextAlignmentOptions.Left);
            SetAnchoredRect(name.GetComponent<RectTransform>(), 0.03f, 0.6f, 0.65f, 0.95f);

            // Tier
            var tier = CreateTMP("Tier", root.transform, "Standard", 14, AccentGold, TextAlignmentOptions.Left);
            SetAnchoredRect(tier.GetComponent<RectTransform>(), 0.03f, 0.35f, 0.4f, 0.55f);

            // Quality
            var quality = CreateTMP("Quality", root.transform, "Quality: 4.0", 14, TextGray, TextAlignmentOptions.Left);
            SetAnchoredRect(quality.GetComponent<RectTransform>(), 0.03f, 0.1f, 0.4f, 0.35f);

            // Price
            var price = CreateTMP("Price", root.transform, "$300", 22, AccentGreen, TextAlignmentOptions.Right);
            SetAnchoredRect(price.GetComponent<RectTransform>(), 0.5f, 0.5f, 0.97f, 0.9f);

            // Book button
            var bookBtn = CreateButtonObj("BookButton", root.transform, "Book", AccentBlue);
            SetAnchoredRect(bookBtn.GetComponent<RectTransform>(), 0.7f, 0.1f, 0.97f, 0.45f);

            SavePrefab(root, $"{PrefabRoot}/EventPlanning/VendorCard.prefab");
        }

        #endregion

        #region Event Execution Prefabs

        private static void GenerateEventExecutionPrefabs()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating Event Execution prefabs...");

            CreateEventExecutionPanelPrefab();
            CreateRandomEventCardPrefab();
        }

        private static void CreateEventExecutionPanelPrefab()
        {
            var root = new GameObject("EventExecutionPanel");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            var bg = root.AddComponent<Image>();
            bg.color = BgContent;

            // Header
            var header = CreateTMP("Header", root.transform, "Event In Progress", 28, AccentGold, TextAlignmentOptions.Center);
            SetAnchoredRect(header.GetComponent<RectTransform>(), 0, 0.9f, 1, 0.98f);

            // Progress bar background
            var progressBg = CreatePanel("ProgressBarBg", root.transform, BgCard);
            SetAnchoredRect(progressBg.GetComponent<RectTransform>(), 0.1f, 0.8f, 0.9f, 0.85f);

            // Progress bar fill
            var progressFill = CreatePanel("ProgressBarFill", progressBg.transform, AccentBlue);
            var fillRect = progressFill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0.5f, 1);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            // Status updates area
            var statusArea = CreatePanel("StatusArea", root.transform, BgDark);
            SetAnchoredRect(statusArea.GetComponent<RectTransform>(), 0.05f, 0.3f, 0.95f, 0.75f);

            var statusText = CreateTMP("StatusText", statusArea.transform, "Event updates appear here...", 16, TextGray, TextAlignmentOptions.TopLeft);
            StretchFill(statusText.GetComponent<RectTransform>());
            statusText.GetComponent<RectTransform>().offsetMin = new Vector2(10, 10);
            statusText.GetComponent<RectTransform>().offsetMax = new Vector2(-10, -10);

            // Contingency budget
            var contingency = CreateTMP("Contingency", root.transform, "Contingency: $200", 18, AccentGreen, TextAlignmentOptions.Center);
            SetAnchoredRect(contingency.GetComponent<RectTransform>(), 0, 0.2f, 1, 0.28f);

            // Random event card area (hidden by default)
            var randomEventArea = new GameObject("RandomEventArea");
            randomEventArea.transform.SetParent(root.transform, false);
            SetAnchoredRect(randomEventArea.AddComponent<RectTransform>(), 0.05f, 0.35f, 0.95f, 0.7f);
            randomEventArea.SetActive(false);

            SavePrefab(root, $"{PrefabRoot}/EventExecution/EventExecutionPanel.prefab");
        }

        private static void CreateRandomEventCardPrefab()
        {
            var root = new GameObject("RandomEventCard");
            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(450, 250);

            var bg = root.AddComponent<Image>();
            bg.color = new Color(0.2f, 0.15f, 0.1f, 0.98f);

            // Title
            var title = CreateTMP("Title", root.transform, "Random Event!", 24, AccentGold, TextAlignmentOptions.Center);
            SetAnchoredRect(title.GetComponent<RectTransform>(), 0, 0.8f, 1, 0.95f);

            // Description
            var desc = CreateTMP("Description", root.transform, "Something unexpected has happened at the event!", 18, TextWhite, TextAlignmentOptions.Center);
            SetAnchoredRect(desc.GetComponent<RectTransform>(), 0.05f, 0.45f, 0.95f, 0.75f);

            // Impact
            var impact = CreateTMP("Impact", root.transform, "Impact: -10 satisfaction", 16, AccentRed, TextAlignmentOptions.Center);
            SetAnchoredRect(impact.GetComponent<RectTransform>(), 0, 0.32f, 1, 0.42f);

            // Mitigation buttons
            var option1 = CreateButtonObj("Option1", root.transform, "Spend $50 to fix", AccentBlue);
            SetAnchoredRect(option1.GetComponent<RectTransform>(), 0.05f, 0.05f, 0.48f, 0.25f);

            var option2 = CreateButtonObj("Option2", root.transform, "Ignore", new Color(0.4f, 0.4f, 0.4f));
            SetAnchoredRect(option2.GetComponent<RectTransform>(), 0.52f, 0.05f, 0.95f, 0.25f);

            SavePrefab(root, $"{PrefabRoot}/EventExecution/RandomEventCard.prefab");
        }

        #endregion

        #region Results Prefabs

        private static void GenerateResultsPrefabs()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating Results prefabs...");

            var root = new GameObject("ResultsPanel");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            var bg = root.AddComponent<Image>();
            bg.color = BgContent;

            // Header
            var header = CreateTMP("Header", root.transform, "Event Complete!", 32, AccentGold, TextAlignmentOptions.Center);
            SetAnchoredRect(header.GetComponent<RectTransform>(), 0, 0.88f, 1, 0.98f);

            // Satisfaction score (large)
            var scoreLabel = CreateTMP("ScoreLabel", root.transform, "Client Satisfaction", 18, TextGray, TextAlignmentOptions.Center);
            SetAnchoredRect(scoreLabel.GetComponent<RectTransform>(), 0, 0.78f, 1, 0.85f);

            var score = CreateTMP("SatisfactionScore", root.transform, "85%", 64, AccentGreen, TextAlignmentOptions.Center);
            SetAnchoredRect(score.GetComponent<RectTransform>(), 0, 0.62f, 1, 0.78f);

            // Profit/loss
            var profit = CreateTMP("Profit", root.transform, "Profit: +$350", 24, AccentGreen, TextAlignmentOptions.Center);
            SetAnchoredRect(profit.GetComponent<RectTransform>(), 0, 0.52f, 1, 0.6f);

            // Reputation change
            var repChange = CreateTMP("ReputationChange", root.transform, "Reputation: +5", 20, AccentGold, TextAlignmentOptions.Center);
            SetAnchoredRect(repChange.GetComponent<RectTransform>(), 0, 0.44f, 1, 0.52f);

            // Client feedback
            var feedback = CreateTMP("Feedback", root.transform, "\"Great party! Would recommend.\"", 18, TextGray, TextAlignmentOptions.Center);
            SetAnchoredRect(feedback.GetComponent<RectTransform>(), 0.1f, 0.3f, 0.9f, 0.42f);

            // Continue button
            var continueBtn = CreateButtonObj("ContinueButton", root.transform, "Continue", AccentBlue);
            SetAnchoredRect(continueBtn.GetComponent<RectTransform>(), 0.25f, 0.05f, 0.75f, 0.15f);

            SavePrefab(root, $"{PrefabRoot}/Results/ResultsPanel.prefab");
        }

        #endregion

        #region Tutorial Prefabs

        private static void GenerateTutorialPrefabs()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating Tutorial prefabs...");

            CreateTutorialOverlayPrefab();
            CreateTipBubblePrefab();
        }

        private static void CreateTutorialOverlayPrefab()
        {
            var root = new GameObject("TutorialOverlay");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            // Dim background
            var dimBg = CreatePanel("DimBackground", root.transform, new Color(0, 0, 0, 0.8f));
            StretchFill(dimBg.GetComponent<RectTransform>());

            // Instruction panel
            var instructionPanel = CreatePanel("InstructionPanel", root.transform, BgCard);
            SetAnchoredRect(instructionPanel.GetComponent<RectTransform>(), 0.05f, 0.6f, 0.95f, 0.9f);

            var instructionText = CreateTMP("InstructionText", instructionPanel.transform, "Tutorial instruction text goes here.\n\nThis will guide the player through the game.", 20, TextWhite, TextAlignmentOptions.Center);
            StretchFill(instructionText.GetComponent<RectTransform>());
            instructionText.GetComponent<RectTransform>().offsetMin = new Vector2(20, 60);
            instructionText.GetComponent<RectTransform>().offsetMax = new Vector2(-20, -20);

            // Continue button
            var continueBtn = CreateButtonObj("ContinueButton", instructionPanel.transform, "Continue", AccentBlue);
            var contRect = continueBtn.GetComponent<RectTransform>();
            contRect.anchorMin = new Vector2(0.55f, 0);
            contRect.anchorMax = new Vector2(0.95f, 0);
            contRect.pivot = new Vector2(0.5f, 0);
            contRect.sizeDelta = new Vector2(0, 45);
            contRect.anchoredPosition = new Vector2(0, 10);

            // Skip button
            var skipBtn = CreateButtonObj("SkipButton", instructionPanel.transform, "Skip Tutorial", new Color(0.4f, 0.4f, 0.4f));
            var skipRect = skipBtn.GetComponent<RectTransform>();
            skipRect.anchorMin = new Vector2(0.05f, 0);
            skipRect.anchorMax = new Vector2(0.45f, 0);
            skipRect.pivot = new Vector2(0.5f, 0);
            skipRect.sizeDelta = new Vector2(0, 45);
            skipRect.anchoredPosition = new Vector2(0, 10);

            // Highlight mask (placeholder - would use custom masking in real implementation)
            var highlightArea = new GameObject("HighlightArea");
            highlightArea.transform.SetParent(root.transform, false);
            highlightArea.AddComponent<RectTransform>();

            SavePrefab(root, $"{PrefabRoot}/Tutorial/TutorialOverlay.prefab");
        }

        private static void CreateTipBubblePrefab()
        {
            var root = new GameObject("TipBubble");
            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 100);

            var bg = root.AddComponent<Image>();
            bg.color = new Color(0.95f, 0.9f, 0.7f, 1f);

            var tipText = CreateTMP("TipText", root.transform, "Tip: This is helpful advice!", 16, new Color(0.2f, 0.2f, 0.2f), TextAlignmentOptions.Center);
            StretchFill(tipText.GetComponent<RectTransform>());
            tipText.GetComponent<RectTransform>().offsetMin = new Vector2(10, 10);
            tipText.GetComponent<RectTransform>().offsetMax = new Vector2(-10, -10);

            SavePrefab(root, $"{PrefabRoot}/Tutorial/TipBubble.prefab");
        }

        #endregion

        #region Notification Prefabs

        private static void GenerateNotificationPrefabs()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating Notification prefabs...");

            var root = new GameObject("NotificationPopup");
            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(450, 100);

            var bg = root.AddComponent<Image>();
            bg.color = BgCard;

            // Icon
            var icon = CreatePanel("Icon", root.transform, AccentBlue);
            var iconRect = icon.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0, 0);
            iconRect.anchorMax = new Vector2(0, 1);
            iconRect.pivot = new Vector2(0, 0.5f);
            iconRect.sizeDelta = new Vector2(80, 0);
            iconRect.anchoredPosition = new Vector2(10, 0);
            iconRect.offsetMin = new Vector2(10, 15);
            iconRect.offsetMax = new Vector2(90, -15);

            // Title
            var title = CreateTMP("Title", root.transform, "Notification", 20, TextWhite, TextAlignmentOptions.Left);
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.5f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.offsetMin = new Vector2(100, 5);
            titleRect.offsetMax = new Vector2(-50, -10);

            // Message
            var message = CreateTMP("Message", root.transform, "Notification message goes here", 16, TextGray, TextAlignmentOptions.Left);
            var msgRect = message.GetComponent<RectTransform>();
            msgRect.anchorMin = new Vector2(0, 0);
            msgRect.anchorMax = new Vector2(1, 0.5f);
            msgRect.offsetMin = new Vector2(100, 10);
            msgRect.offsetMax = new Vector2(-50, -5);

            // Dismiss button
            var dismissBtn = CreateButtonObj("DismissButton", root.transform, "X", new Color(0.5f, 0.5f, 0.5f));
            var dismissRect = dismissBtn.GetComponent<RectTransform>();
            dismissRect.anchorMin = new Vector2(1, 0.5f);
            dismissRect.anchorMax = new Vector2(1, 0.5f);
            dismissRect.pivot = new Vector2(1, 0.5f);
            dismissRect.sizeDelta = new Vector2(40, 40);
            dismissRect.anchoredPosition = new Vector2(-5, 0);

            SavePrefab(root, $"{PrefabRoot}/Notifications/NotificationPopup.prefab");
        }

        #endregion

        #region Settings Prefabs

        private static void GenerateSettingsPrefabs()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating Settings prefabs...");

            CreatePauseMenuPrefab();
            CreateSettingsPanelPrefab();
        }

        private static void CreatePauseMenuPrefab()
        {
            var root = new GameObject("PauseMenu");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            // Dim background
            var dimBg = CreatePanel("DimBackground", root.transform, new Color(0, 0, 0, 0.7f));
            StretchFill(dimBg.GetComponent<RectTransform>());

            // Menu panel
            var panel = CreatePanel("MenuPanel", root.transform, BgPanel);
            SetAnchoredRect(panel.GetComponent<RectTransform>(), 0.15f, 0.25f, 0.85f, 0.75f);

            // Title
            var title = CreateTMP("Title", panel.transform, "Paused", 32, TextWhite, TextAlignmentOptions.Center);
            SetAnchoredRect(title.GetComponent<RectTransform>(), 0, 0.8f, 1, 0.95f);

            // Resume button
            var resumeBtn = CreateButtonObj("ResumeButton", panel.transform, "Resume", AccentGreen);
            SetAnchoredRect(resumeBtn.GetComponent<RectTransform>(), 0.15f, 0.55f, 0.85f, 0.7f);

            // Settings button
            var settingsBtn = CreateButtonObj("SettingsButton", panel.transform, "Settings", AccentBlue);
            SetAnchoredRect(settingsBtn.GetComponent<RectTransform>(), 0.15f, 0.35f, 0.85f, 0.5f);

            // Quit button
            var quitBtn = CreateButtonObj("QuitButton", panel.transform, "Quit to Menu", AccentRed);
            SetAnchoredRect(quitBtn.GetComponent<RectTransform>(), 0.15f, 0.15f, 0.85f, 0.3f);

            SavePrefab(root, $"{PrefabRoot}/Settings/PauseMenu.prefab");
        }

        private static void CreateSettingsPanelPrefab()
        {
            var root = new GameObject("SettingsPanel");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            var bg = root.AddComponent<Image>();
            bg.color = BgContent;

            // Header
            var header = CreateTMP("Header", root.transform, "Settings", 28, TextWhite, TextAlignmentOptions.Center);
            SetAnchoredRect(header.GetComponent<RectTransform>(), 0, 0.88f, 1, 0.98f);

            // Audio section
            var audioLabel = CreateTMP("AudioLabel", root.transform, "Audio", 20, TextGray, TextAlignmentOptions.Left);
            SetAnchoredRect(audioLabel.GetComponent<RectTransform>(), 0.05f, 0.75f, 0.95f, 0.82f);

            var musicLabel = CreateTMP("MusicLabel", root.transform, "Music Volume", 16, TextWhite, TextAlignmentOptions.Left);
            SetAnchoredRect(musicLabel.GetComponent<RectTransform>(), 0.05f, 0.67f, 0.4f, 0.74f);

            var sfxLabel = CreateTMP("SFXLabel", root.transform, "SFX Volume", 16, TextWhite, TextAlignmentOptions.Left);
            SetAnchoredRect(sfxLabel.GetComponent<RectTransform>(), 0.05f, 0.57f, 0.4f, 0.64f);

            // Notifications section
            var notifLabel = CreateTMP("NotifLabel", root.transform, "Notifications", 20, TextGray, TextAlignmentOptions.Left);
            SetAnchoredRect(notifLabel.GetComponent<RectTransform>(), 0.05f, 0.45f, 0.95f, 0.52f);

            // Text size section
            var textLabel = CreateTMP("TextLabel", root.transform, "Text Size", 20, TextGray, TextAlignmentOptions.Left);
            SetAnchoredRect(textLabel.GetComponent<RectTransform>(), 0.05f, 0.33f, 0.95f, 0.4f);

            // Version
            var version = CreateTMP("Version", root.transform, "Version 1.0.0", 14, TextDimmed, TextAlignmentOptions.Center);
            SetAnchoredRect(version.GetComponent<RectTransform>(), 0, 0.02f, 1, 0.08f);

            // Back button
            var backBtn = CreateButtonObj("BackButton", root.transform, "Back", AccentBlue);
            SetAnchoredRect(backBtn.GetComponent<RectTransform>(), 0.3f, 0.1f, 0.7f, 0.18f);

            SavePrefab(root, $"{PrefabRoot}/Settings/SettingsPanel.prefab");
        }

        #endregion

        #region Milestone Prefabs

        private static void GenerateMilestonePrefabs()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating Milestone prefabs...");

            var root = new GameObject("MilestoneOverlay");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            var bg = root.AddComponent<Image>();
            bg.color = BgDark;

            // Title
            var title = CreateTMP("Title", root.transform, "Stage Complete!", 36, AccentGold, TextAlignmentOptions.Center);
            SetAnchoredRect(title.GetComponent<RectTransform>(), 0, 0.85f, 1, 0.95f);

            // Career summary view
            var summaryView = new GameObject("CareerSummaryView");
            summaryView.transform.SetParent(root.transform, false);
            SetAnchoredRect(summaryView.AddComponent<RectTransform>(), 0.05f, 0.25f, 0.95f, 0.8f);
            var summaryBg = summaryView.AddComponent<Image>();
            summaryBg.color = BgContent;

            var statsText = CreateTMP("StatsText", summaryView.transform, "Events Completed: 10\nTotal Revenue: $25,000\nAverage Satisfaction: 85%\nReputation: 75", 20, TextWhite, TextAlignmentOptions.Center);
            StretchFill(statsText.GetComponent<RectTransform>());
            statsText.GetComponent<RectTransform>().offsetMin = new Vector2(20, 20);
            statsText.GetComponent<RectTransform>().offsetMax = new Vector2(-20, -20);

            // Continue button
            var continueBtn = CreateButtonObj("ContinueButton", root.transform, "Continue", AccentBlue);
            SetAnchoredRect(continueBtn.GetComponent<RectTransform>(), 0.25f, 0.08f, 0.75f, 0.18f);

            // Skip button
            var skipBtn = CreateButtonObj("SkipButton", root.transform, "Skip", new Color(0.4f, 0.4f, 0.4f));
            var skipRect = skipBtn.GetComponent<RectTransform>();
            skipRect.anchorMin = new Vector2(1, 1);
            skipRect.anchorMax = new Vector2(1, 1);
            skipRect.pivot = new Vector2(1, 1);
            skipRect.sizeDelta = new Vector2(80, 40);
            skipRect.anchoredPosition = new Vector2(-20, -20);

            SavePrefab(root, $"{PrefabRoot}/Milestone/MilestoneOverlay.prefab");
        }

        #endregion

        #region Main Menu Prefabs

        private static void GenerateMainMenuPrefabs()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating Main Menu prefabs...");

            var root = new GameObject("MainMenuPanel");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            var bg = root.AddComponent<Image>();
            bg.color = BgDark;

            // Title/Logo
            var title = CreateTMP("Title", root.transform, "Event Planner\nSimulator", 48, TextWhite, TextAlignmentOptions.Center);
            SetAnchoredRect(title.GetComponent<RectTransform>(), 0.1f, 0.65f, 0.9f, 0.9f);

            // New Game button
            var newGameBtn = CreateButtonObj("NewGameButton", root.transform, "New Game", AccentGreen);
            SetAnchoredRect(newGameBtn.GetComponent<RectTransform>(), 0.2f, 0.45f, 0.8f, 0.55f);

            // Continue button
            var continueBtn = CreateButtonObj("ContinueButton", root.transform, "Continue", AccentBlue);
            SetAnchoredRect(continueBtn.GetComponent<RectTransform>(), 0.2f, 0.32f, 0.8f, 0.42f);

            // Settings button
            var settingsBtn = CreateButtonObj("SettingsButton", root.transform, "Settings", new Color(0.4f, 0.4f, 0.5f));
            SetAnchoredRect(settingsBtn.GetComponent<RectTransform>(), 0.2f, 0.19f, 0.8f, 0.29f);

            // Credits button
            var creditsBtn = CreateButtonObj("CreditsButton", root.transform, "Credits", new Color(0.4f, 0.4f, 0.5f));
            SetAnchoredRect(creditsBtn.GetComponent<RectTransform>(), 0.2f, 0.06f, 0.8f, 0.16f);

            SavePrefab(root, $"{PrefabRoot}/MainMenu/MainMenuPanel.prefab");
        }

        #endregion

        #region Loading Prefabs

        private static void GenerateLoadingPrefabs()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating Loading prefabs...");

            var root = new GameObject("LoadingScreen");
            var rect = root.AddComponent<RectTransform>();
            StretchFill(rect);

            var bg = root.AddComponent<Image>();
            bg.color = BgDark;

            // Loading text
            var loadingText = CreateTMP("LoadingText", root.transform, "Loading...", 32, TextWhite, TextAlignmentOptions.Center);
            SetAnchoredRect(loadingText.GetComponent<RectTransform>(), 0, 0.45f, 1, 0.55f);

            // Progress bar background
            var progressBg = CreatePanel("ProgressBarBg", root.transform, BgCard);
            SetAnchoredRect(progressBg.GetComponent<RectTransform>(), 0.15f, 0.35f, 0.85f, 0.4f);

            // Progress bar fill
            var progressFill = CreatePanel("ProgressBarFill", progressBg.transform, AccentBlue);
            var fillRect = progressFill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(0, 1);
            fillRect.pivot = new Vector2(0, 0.5f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            // Tip text
            var tipText = CreateTMP("TipText", root.transform, "Tip: Plan your budget carefully!", 18, TextGray, TextAlignmentOptions.Center);
            SetAnchoredRect(tipText.GetComponent<RectTransform>(), 0.1f, 0.15f, 0.9f, 0.25f);

            SavePrefab(root, $"{PrefabRoot}/Loading/LoadingScreen.prefab");
        }

        #endregion

        #region Scene Generation

        private static void GenerateMainMenuScene()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating MainMenu scene...");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            var cameraObj = new GameObject("Main Camera");
            var camera = cameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = BgDark;
            camera.orthographic = true;
            cameraObj.transform.position = new Vector3(0, 0, -10);
            cameraObj.tag = "MainCamera";
            cameraObj.AddComponent<AudioListener>();

            // EventSystem
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            // Canvas
            var canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            // Main Menu Panel placeholder
            var mainMenuPanel = CreatePanel("MainMenuPanel", canvasObj.transform, BgDark);
            StretchFill(mainMenuPanel.GetComponent<RectTransform>());

            var title = CreateTMP("Title", mainMenuPanel.transform, "Event Planner\nSimulator", 48, TextWhite, TextAlignmentOptions.Center);
            SetAnchoredRect(title.GetComponent<RectTransform>(), 0.1f, 0.65f, 0.9f, 0.9f);

            var note = CreateTMP("Note", mainMenuPanel.transform, "(MainMenu scene - instantiate MainMenuPanel prefab here)", 16, TextGray, TextAlignmentOptions.Center);
            SetAnchoredRect(note.GetComponent<RectTransform>(), 0.1f, 0.4f, 0.9f, 0.5f);

            EditorSceneManager.SaveScene(scene, $"{SceneRoot}/MainMenu.unity");
            UnityEngine.Debug.Log($"[UnityAssetGenerator] Created scene: {SceneRoot}/MainMenu.unity");
        }

        private static void GenerateGameplayMainScene()
        {
            UnityEngine.Debug.Log("[UnityAssetGenerator] Generating GameplayMain scene...");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            var cameraObj = new GameObject("Main Camera");
            var camera = cameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = BgDark;
            camera.orthographic = true;
            cameraObj.transform.position = new Vector3(0, 0, -10);
            cameraObj.tag = "MainCamera";
            cameraObj.AddComponent<AudioListener>();

            // EventSystem
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            // Root Canvas with sort order 0
            var rootCanvas = CreateCanvasLayer("GameplayCanvas", 0);

            // Create layer containers per design.md
            CreateCanvasLayer("GameplayLayer", 0, rootCanvas.transform);
            CreateCanvasLayer("PhoneOverlay", 10, rootCanvas.transform).SetActive(false);
            CreateCanvasLayer("MapOverlay", 20, rootCanvas.transform).SetActive(false);
            CreateCanvasLayer("TutorialOverlay", 30, rootCanvas.transform).SetActive(false);
            CreateCanvasLayer("NotificationLayer", 40, rootCanvas.transform);
            CreateCanvasLayer("SettingsOverlay", 50, rootCanvas.transform).SetActive(false);
            CreateCanvasLayer("PauseMenuOverlay", 60, rootCanvas.transform).SetActive(false);
            CreateCanvasLayer("MilestoneOverlay", 70, rootCanvas.transform).SetActive(false);
            CreateCanvasLayer("LoadingOverlay", 80, rootCanvas.transform).SetActive(false);

            // Add HUD to GameplayLayer
            var gameplayLayer = rootCanvas.transform.Find("GameplayLayer");
            var hudPlaceholder = CreatePanel("HUD", gameplayLayer, Color.clear);
            StretchFill(hudPlaceholder.GetComponent<RectTransform>());

            var topBar = CreatePanel("TopBar_Placeholder", hudPlaceholder.transform, BgPanel);
            var topRect = topBar.GetComponent<RectTransform>();
            topRect.anchorMin = new Vector2(0, 1);
            topRect.anchorMax = new Vector2(1, 1);
            topRect.pivot = new Vector2(0.5f, 1);
            topRect.sizeDelta = new Vector2(0, 80);

            var bottomBar = CreatePanel("BottomBar_Placeholder", hudPlaceholder.transform, BgPanel);
            var botRect = bottomBar.GetComponent<RectTransform>();
            botRect.anchorMin = new Vector2(0, 0);
            botRect.anchorMax = new Vector2(1, 0);
            botRect.pivot = new Vector2(0.5f, 0);
            botRect.sizeDelta = new Vector2(0, 100);

            var note = CreateTMP("Note", gameplayLayer, "(GameplayMain scene - instantiate prefabs into appropriate layers)", 14, TextGray, TextAlignmentOptions.Center);
            SetAnchoredRect(note.GetComponent<RectTransform>(), 0.05f, 0.45f, 0.95f, 0.55f);

            EditorSceneManager.SaveScene(scene, $"{SceneRoot}/GameplayMain.unity");
            UnityEngine.Debug.Log($"[UnityAssetGenerator] Created scene: {SceneRoot}/GameplayMain.unity");
        }

        private static GameObject CreateCanvasLayer(string name, int sortOrder, Transform parent = null)
        {
            var obj = new GameObject(name);

            if (parent != null)
            {
                obj.transform.SetParent(parent, false);
                var rect = obj.AddComponent<RectTransform>();
                StretchFill(rect);
            }
            else
            {
                var canvas = obj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = sortOrder;

                var scaler = obj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
                scaler.matchWidthOrHeight = 0.5f;

                obj.AddComponent<GraphicRaycaster>();
            }

            return obj;
        }

        #endregion

        #region Helper Methods

        private static GameObject CreatePanel(string name, Transform parent, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.AddComponent<RectTransform>();
            var img = obj.AddComponent<Image>();
            img.color = color;
            return obj;
        }

        private static GameObject CreateTMP(string name, Transform parent, string text, int fontSize, Color color, TextAlignmentOptions alignment)
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

        private static GameObject CreateButtonObj(string name, Transform parent, string label, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.AddComponent<RectTransform>();

            var img = obj.AddComponent<Image>();
            img.color = color;

            var btn = obj.AddComponent<Button>();
            btn.targetGraphic = img;

            var labelObj = CreateTMP("Label", obj.transform, label, 20, TextWhite, TextAlignmentOptions.Center);
            StretchFill(labelObj.GetComponent<RectTransform>());

            return obj;
        }

        private static void StretchFill(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void SetAnchoredRect(RectTransform rect, float minX, float minY, float maxX, float maxY)
        {
            rect.anchorMin = new Vector2(minX, minY);
            rect.anchorMax = new Vector2(maxX, maxY);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void SavePrefab(GameObject obj, string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            PrefabUtility.SaveAsPrefabAsset(obj, path);
            Object.DestroyImmediate(obj);
            UnityEngine.Debug.Log($"[UnityAssetGenerator] Created: {path}");
        }

        #endregion

        #region Command Line Entry Point

        public static void GenerateFromCommandLine()
        {
            GenerateAll();
            EditorApplication.Exit(0);
        }

        #endregion
    }
}
