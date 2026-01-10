using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using System.IO;

namespace EventPlannerSim.Editor
{
    /// <summary>
    /// Generates and configures scenes with proper canvas hierarchy.
    /// Run via: Tools > Setup Scenes
    /// </summary>
    public static class SceneSetupGenerator
    {
        private const string ScenesPath = "Assets/Scenes";

        [MenuItem("Tools/Setup Scenes")]
        public static void SetupAllScenes()
        {
            EnsureDirectory(ScenesPath);

            SetupMainMenuScene();
            SetupGameplayScene();

            UnityEngine.Debug.Log("[SceneSetupGenerator] Scene setup complete!");
        }

        [MenuItem("Tools/Setup Scenes/Main Menu Only")]
        public static void SetupMainMenuScene()
        {
            string scenePath = $"{ScenesPath}/MainMenu.unity";

            // Create or open scene
            Scene scene;
            if (File.Exists(scenePath))
            {
                scene = EditorSceneManager.OpenScene(scenePath);
                UnityEngine.Debug.Log("[SceneSetupGenerator] Opened existing MainMenu scene");
            }
            else
            {
                scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                UnityEngine.Debug.Log("[SceneSetupGenerator] Created new MainMenu scene");
            }

            // Set up scene contents
            SetupMainMenuContents();

            // Save scene
            EditorSceneManager.SaveScene(scene, scenePath);
            UnityEngine.Debug.Log($"[SceneSetupGenerator] Saved MainMenu scene to {scenePath}");
        }

        [MenuItem("Tools/Setup Scenes/Gameplay Only")]
        public static void SetupGameplayScene()
        {
            string scenePath = $"{ScenesPath}/GameplayMain.unity";

            // Create or open scene
            Scene scene;
            if (File.Exists(scenePath))
            {
                scene = EditorSceneManager.OpenScene(scenePath);
                UnityEngine.Debug.Log("[SceneSetupGenerator] Opened existing GameplayMain scene");
            }
            else
            {
                scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                UnityEngine.Debug.Log("[SceneSetupGenerator] Created new GameplayMain scene");
            }

            // Set up scene contents
            SetupGameplayContents();

            // Save scene
            EditorSceneManager.SaveScene(scene, scenePath);
            UnityEngine.Debug.Log($"[SceneSetupGenerator] Saved GameplayMain scene to {scenePath}");
        }

        private static void SetupMainMenuContents()
        {
            // Clean up existing canvas if present
            var existingCanvas = GameObject.Find("MainMenuCanvas");
            if (existingCanvas != null)
            {
                UnityEngine.Debug.Log("[SceneSetupGenerator] MainMenu canvas already exists, skipping setup");
                return;
            }

            // Create main camera
            var camera = CreateCamera("MainCamera");

            // Create EventSystem
            CreateEventSystem();

            // Create Canvas
            var canvasObj = CreateCanvas("MainMenuCanvas", RenderMode.ScreenSpaceOverlay);
            var canvas = canvasObj.GetComponent<Canvas>();

            // Configure Canvas Scaler
            var scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            // Add SafeAreaHandler to canvas
            canvasObj.AddComponent<UI.Core.SafeAreaHandler>();

            // Create main menu structure
            var menuPanel = CreatePanel("MainMenuPanel", canvasObj.transform);
            StretchToParent(menuPanel);

            // Add title
            var title = CreateText("Title", menuPanel.transform, "Event Planner Sim");
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.1f, 0.7f);
            titleRect.anchorMax = new Vector2(0.9f, 0.85f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            var titleText = title.GetComponent<TextMeshProUGUI>();
            titleText.fontSize = 72;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.fontStyle = FontStyles.Bold;

            // Add buttons container
            var buttonsContainer = CreatePanel("ButtonsContainer", menuPanel.transform);
            var buttonsRect = buttonsContainer.GetComponent<RectTransform>();
            buttonsRect.anchorMin = new Vector2(0.2f, 0.3f);
            buttonsRect.anchorMax = new Vector2(0.8f, 0.6f);
            buttonsRect.offsetMin = Vector2.zero;
            buttonsRect.offsetMax = Vector2.zero;

            // Add vertical layout
            var layout = buttonsContainer.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            // Create buttons
            CreateButton("NewGameButton", buttonsContainer.transform, "New Game");
            CreateButton("ContinueButton", buttonsContainer.transform, "Continue");
            CreateButton("SettingsButton", buttonsContainer.transform, "Settings");

            // Add version text
            var version = CreateText("VersionText", menuPanel.transform, "v1.0.0");
            var versionRect = version.GetComponent<RectTransform>();
            versionRect.anchorMin = new Vector2(0, 0);
            versionRect.anchorMax = new Vector2(1, 0.05f);
            versionRect.offsetMin = Vector2.zero;
            versionRect.offsetMax = Vector2.zero;
            var versionText = version.GetComponent<TextMeshProUGUI>();
            versionText.fontSize = 24;
            versionText.alignment = TextAlignmentOptions.Center;
            versionText.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            UnityEngine.Debug.Log("[SceneSetupGenerator] MainMenu contents created");
        }

        private static void SetupGameplayContents()
        {
            // Clean up existing canvas if present
            var existingCanvas = GameObject.Find("GameplayCanvas");
            if (existingCanvas != null)
            {
                UnityEngine.Debug.Log("[SceneSetupGenerator] Gameplay canvas already exists, skipping setup");
                return;
            }

            // Create main camera
            var camera = CreateCamera("MainCamera");

            // Create EventSystem
            CreateEventSystem();

            // Create Canvas
            var canvasObj = CreateCanvas("GameplayCanvas", RenderMode.ScreenSpaceOverlay);
            var canvas = canvasObj.GetComponent<Canvas>();

            // Configure Canvas Scaler
            var scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            // Create layer containers with sort orders per design spec
            var layers = new (string name, int sortOrder)[]
            {
                ("Layer_Background", 0),
                ("Layer_Map", 10),
                ("Layer_HUD", 20),
                ("Layer_Panels", 30),
                ("Layer_Overlays", 40),
                ("Layer_Phone", 50),
                ("Layer_Modal", 60),
                ("Layer_Tutorial", 70),
                ("Layer_Loading", 80)
            };

            foreach (var (name, sortOrder) in layers)
            {
                var layerObj = new GameObject(name);
                layerObj.transform.SetParent(canvasObj.transform, false);

                var rect = layerObj.AddComponent<RectTransform>();
                StretchToParent(layerObj);

                // Add canvas for sort order
                var layerCanvas = layerObj.AddComponent<Canvas>();
                layerCanvas.overrideSorting = true;
                layerCanvas.sortingOrder = sortOrder;

                // Add raycaster for interactivity
                layerObj.AddComponent<GraphicRaycaster>();
            }

            // Add SafeAreaHandler to HUD layer
            var hudLayer = canvasObj.transform.Find("Layer_HUD")?.gameObject;
            if (hudLayer != null)
            {
                hudLayer.AddComponent<UI.Core.SafeAreaHandler>();
            }

            // Create placeholder HUD
            CreateHUDPlaceholder(hudLayer?.transform ?? canvasObj.transform);

            // Add UIManager to canvas
            canvasObj.AddComponent<UI.Core.UIManager>();

            UnityEngine.Debug.Log("[SceneSetupGenerator] Gameplay contents created with layered canvas hierarchy");
        }

        private static void CreateHUDPlaceholder(Transform parent)
        {
            // Top bar
            var topBar = CreatePanel("TopBar", parent);
            var topBarRect = topBar.GetComponent<RectTransform>();
            topBarRect.anchorMin = new Vector2(0, 0.92f);
            topBarRect.anchorMax = new Vector2(1, 1);
            topBarRect.offsetMin = Vector2.zero;
            topBarRect.offsetMax = Vector2.zero;

            var topBarImage = topBar.GetComponent<Image>();
            topBarImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            // Date text
            var date = CreateText("DateText", topBar.transform, "Mar 15");
            var dateRect = date.GetComponent<RectTransform>();
            dateRect.anchorMin = new Vector2(0.02f, 0.2f);
            dateRect.anchorMax = new Vector2(0.3f, 0.8f);
            dateRect.offsetMin = Vector2.zero;
            dateRect.offsetMax = Vector2.zero;

            // Money text
            var money = CreateText("MoneyText", topBar.transform, "$5,000");
            var moneyRect = money.GetComponent<RectTransform>();
            moneyRect.anchorMin = new Vector2(0.4f, 0.2f);
            moneyRect.anchorMax = new Vector2(0.6f, 0.8f);
            moneyRect.offsetMin = Vector2.zero;
            moneyRect.offsetMax = Vector2.zero;
            var moneyTmp = money.GetComponent<TextMeshProUGUI>();
            moneyTmp.alignment = TextAlignmentOptions.Center;
            moneyTmp.color = new Color(0.2f, 0.8f, 0.2f, 1f);

            // Reputation text
            var rep = CreateText("ReputationText", topBar.transform, "50");
            var repRect = rep.GetComponent<RectTransform>();
            repRect.anchorMin = new Vector2(0.7f, 0.2f);
            repRect.anchorMax = new Vector2(0.98f, 0.8f);
            repRect.offsetMin = Vector2.zero;
            repRect.offsetMax = Vector2.zero;
            var repTmp = rep.GetComponent<TextMeshProUGUI>();
            repTmp.alignment = TextAlignmentOptions.Right;
            repTmp.color = new Color(0.9f, 0.7f, 0.2f, 1f);

            // Bottom bar
            var bottomBar = CreatePanel("BottomBar", parent);
            var bottomBarRect = bottomBar.GetComponent<RectTransform>();
            bottomBarRect.anchorMin = new Vector2(0, 0);
            bottomBarRect.anchorMax = new Vector2(1, 0.08f);
            bottomBarRect.offsetMin = Vector2.zero;
            bottomBarRect.offsetMax = Vector2.zero;

            var bottomBarImage = bottomBar.GetComponent<Image>();
            bottomBarImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            // Add horizontal layout for buttons
            var bottomLayout = bottomBar.AddComponent<HorizontalLayoutGroup>();
            bottomLayout.childAlignment = TextAnchor.MiddleCenter;
            bottomLayout.childControlWidth = true;
            bottomLayout.childControlHeight = true;
            bottomLayout.childForceExpandWidth = true;

            // Bottom bar buttons
            CreateButton("PhoneButton", bottomBar.transform, "Phone");
            CreateButton("MapButton", bottomBar.transform, "Map");
            CreateButton("SettingsButton", bottomBar.transform, "Settings");
        }

        #region Helper Methods

        private static void EnsureDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parentPath = Path.GetDirectoryName(path);
                string folderName = Path.GetFileName(path);
                AssetDatabase.CreateFolder(parentPath, folderName);
            }
        }

        private static GameObject CreateCamera(string name)
        {
            var existing = GameObject.Find(name);
            if (existing != null) return existing;

            // Also check if there's already a camera with AudioListener
            var existingListener = GameObject.FindObjectOfType<AudioListener>();
            if (existingListener != null)
            {
                UnityEngine.Debug.Log("[SceneSetupGenerator] AudioListener already exists, skipping camera creation");
                return existingListener.gameObject;
            }

            var cameraObj = new GameObject(name);
            var camera = cameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.05f, 0.05f, 0.1f, 1f);
            camera.orthographic = true;
            cameraObj.AddComponent<AudioListener>();
            cameraObj.tag = "MainCamera";
            return cameraObj;
        }

        private static void CreateEventSystem()
        {
            if (GameObject.FindObjectOfType<EventSystem>() != null) return;

            var eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();

            // Try to add Input System UI module dynamically, fall back to legacy if not available
            var inputSystemType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemType != null)
            {
                eventSystemObj.AddComponent(inputSystemType);
                UnityEngine.Debug.Log("[SceneSetupGenerator] Added InputSystemUIInputModule");
            }
            else
            {
                // Fall back to legacy input module
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                UnityEngine.Debug.Log("[SceneSetupGenerator] Added StandaloneInputModule (Input System not found)");
            }
        }

        private static GameObject CreateCanvas(string name, RenderMode renderMode)
        {
            var canvasObj = new GameObject(name);
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = renderMode;

            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            return canvasObj;
        }

        private static GameObject CreatePanel(string name, Transform parent)
        {
            var panelObj = new GameObject(name);
            panelObj.transform.SetParent(parent, false);

            var rect = panelObj.AddComponent<RectTransform>();
            var image = panelObj.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0); // Transparent by default

            return panelObj;
        }

        private static GameObject CreateText(string name, Transform parent, string text)
        {
            var textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);

            var rect = textObj.AddComponent<RectTransform>();
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 36;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Left;

            return textObj;
        }

        private static GameObject CreateButton(string name, Transform parent, string label)
        {
            var buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);

            var rect = buttonObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 60);

            var image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.3f, 1f);

            var button = buttonObj.AddComponent<Button>();
            button.targetGraphic = image;

            // Add button feedback
            buttonObj.AddComponent<UI.Core.ButtonFeedback>();

            // Add layout element
            var layoutElement = buttonObj.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 60;

            // Add label
            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(buttonObj.transform, false);

            var labelRect = labelObj.AddComponent<RectTransform>();
            StretchToParent(labelObj);

            var tmp = labelObj.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 32;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;

            return buttonObj;
        }

        private static void StretchToParent(GameObject obj)
        {
            var rect = obj.GetComponent<RectTransform>();
            if (rect == null) rect = obj.AddComponent<RectTransform>();

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        #endregion
    }
}
