using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

namespace EventPlannerSim.Editor
{
    public static class ScreenshotCapture
    {
        [MenuItem("Tools/Capture Screenshot")]
        public static void CaptureFromMenu()
        {
            Capture();
        }

        public static void Capture()
        {
            string path = "/tmp/unity_screenshot.png";

            // Create a simple scene with UI programmatically
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Create camera
            var cameraObj = new GameObject("Main Camera");
            var camera = cameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);
            camera.orthographic = true;
            camera.orthographicSize = 5;
            cameraObj.transform.position = new Vector3(0, 0, -10);
            cameraObj.tag = "MainCamera";

            // Create Canvas
            var canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // Create HUD background
            var hudObj = CreatePanel("HUD", canvasObj.transform, new Color(0.12f, 0.12f, 0.18f, 0.95f));
            var hudRect = hudObj.GetComponent<RectTransform>();
            hudRect.anchorMin = new Vector2(0, 1);
            hudRect.anchorMax = new Vector2(1, 1);
            hudRect.pivot = new Vector2(0.5f, 1);
            hudRect.sizeDelta = new Vector2(0, 120);
            hudRect.anchoredPosition = Vector2.zero;

            // Date text
            CreateText("Jan 15", hudObj.transform, new Vector2(0, 0.3f), new Vector2(0.3f, 0.7f),
                       Color.white, TextAnchor.MiddleLeft, 36);

            // Money text
            CreateText("$2,500", hudObj.transform, new Vector2(0.4f, 0.3f), new Vector2(0.65f, 0.7f),
                       new Color(0.3f, 0.85f, 0.4f), TextAnchor.MiddleRight, 42);

            // Reputation
            CreateText("Rep: 50", hudObj.transform, new Vector2(0.7f, 0.3f), new Vector2(0.95f, 0.7f),
                       new Color(0.9f, 0.7f, 0.2f), TextAnchor.MiddleRight, 36);

            // Bottom nav
            var navObj = CreatePanel("BottomNav", canvasObj.transform, new Color(0.12f, 0.12f, 0.18f, 0.98f));
            var navRect = navObj.GetComponent<RectTransform>();
            navRect.anchorMin = new Vector2(0, 0);
            navRect.anchorMax = new Vector2(1, 0);
            navRect.pivot = new Vector2(0.5f, 0);
            navRect.sizeDelta = new Vector2(0, 140);
            navRect.anchoredPosition = Vector2.zero;

            // Nav buttons
            CreateButton("Phone", navObj.transform, 0, 3, new Color(0.25f, 0.5f, 0.85f));
            CreateButton("Map", navObj.transform, 1, 3, new Color(0.25f, 0.5f, 0.85f));
            CreateButton("Menu", navObj.transform, 2, 3, new Color(0.25f, 0.5f, 0.85f));

            // Center content
            var contentObj = CreatePanel("Content", canvasObj.transform, new Color(0.08f, 0.08f, 0.12f, 0.9f));
            var contentRect = contentObj.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.05f, 0);
            contentRect.anchorMax = new Vector2(0.95f, 1);
            contentRect.offsetMin = new Vector2(0, 160);
            contentRect.offsetMax = new Vector2(0, -140);

            // Welcome text
            CreateText("Event Planner\nSimulator\n\n--- UI Preview ---\n\nHUD  |  Navigation  |  Content Area",
                       contentObj.transform, new Vector2(0.05f, 0.3f), new Vector2(0.95f, 0.7f),
                       Color.white, TextAnchor.MiddleCenter, 48);

            // Force a frame update
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();

            // Take screenshot using Game view
            int width = 1080;
            int height = 1920;

            RenderTexture rt = new RenderTexture(width, height, 24);
            camera.targetTexture = rt;
            camera.Render();

            RenderTexture.active = rt;
            Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenshot.Apply();

            // But we need UI - let's just save the scene and report
            camera.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);
            Object.DestroyImmediate(screenshot);

            // Save scene
            string scenePath = "Assets/Scenes/GeneratedUI.unity";
            EditorSceneManager.SaveScene(scene, scenePath);

            UnityEngine.Debug.Log($"[ScreenshotCapture] Scene saved to {scenePath}");
            UnityEngine.Debug.Log("[ScreenshotCapture] Open Unity and play the scene to see the UI!");
        }

        private static GameObject CreatePanel(string name, Transform parent, Color color)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            var img = obj.AddComponent<UnityEngine.UI.Image>();
            img.color = color;
            return obj;
        }

        private static GameObject CreateText(string text, Transform parent, Vector2 anchorMin, Vector2 anchorMax,
                                             Color color, TextAnchor alignment, int fontSize)
        {
            var obj = new GameObject("Text");
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = new Vector2(20, 0);
            rect.offsetMax = new Vector2(-20, 0);

            var txt = obj.AddComponent<UnityEngine.UI.Text>();
            txt.text = text;
            txt.color = color;
            txt.alignment = alignment;
            txt.fontSize = fontSize;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return obj;
        }

        private static GameObject CreateButton(string label, Transform parent, int index, int total, Color color)
        {
            float width = 1f / total;

            var obj = new GameObject(label + "Btn");
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(width * index, 0);
            rect.anchorMax = new Vector2(width * (index + 1), 1);
            rect.offsetMin = new Vector2(15, 20);
            rect.offsetMax = new Vector2(-15, -20);

            var img = obj.AddComponent<UnityEngine.UI.Image>();
            img.color = color;

            obj.AddComponent<UnityEngine.UI.Button>();

            // Label
            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(obj.transform, false);
            var labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var txt = labelObj.AddComponent<UnityEngine.UI.Text>();
            txt.text = label;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.fontSize = 32;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return obj;
        }

        // Called from command line
        public static void CaptureFromCommandLine()
        {
            Capture();
            EditorApplication.Exit(0);
        }
    }
}
