using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace EventPlannerSim.Editor
{
    /// <summary>
    /// Verifies that the UI foundation is correctly set up.
    /// Run via: Tools > Verify Foundation
    /// This corresponds to Task 3: Checkpoint - Foundation Complete
    /// </summary>
    public static class FoundationVerifier
    {
        private static List<string> _errors = new List<string>();
        private static List<string> _warnings = new List<string>();
        private static List<string> _successes = new List<string>();

        [MenuItem("Tools/Verify Foundation")]
        public static void VerifyAll()
        {
            _errors.Clear();
            _warnings.Clear();
            _successes.Clear();

            UnityEngine.Debug.Log("=== Foundation Verification Starting ===");

            // Verify scenes exist and load correctly
            VerifyScenes();

            // Verify core UI components exist
            VerifyUIComponents();

            // Verify prefabs exist
            VerifyPrefabs();

            // Print summary
            PrintSummary();
        }

        private static void VerifyScenes()
        {
            UnityEngine.Debug.Log("\n--- Verifying Scenes ---");

            // Check MainMenu scene
            string mainMenuPath = "Assets/Scenes/MainMenu.unity";
            if (System.IO.File.Exists(mainMenuPath))
            {
                Success("MainMenu.unity exists");
                VerifyMainMenuScene(mainMenuPath);
            }
            else
            {
                Error("MainMenu.unity not found - run Tools > Setup Scenes");
            }

            // Check GameplayMain scene
            string gameplayPath = "Assets/Scenes/GameplayMain.unity";
            if (System.IO.File.Exists(gameplayPath))
            {
                Success("GameplayMain.unity exists");
                VerifyGameplayScene(gameplayPath);
            }
            else
            {
                Error("GameplayMain.unity not found - run Tools > Setup Scenes");
            }
        }

        private static void VerifyMainMenuScene(string scenePath)
        {
            // Remember the current scene so we can return to it
            string originalScenePath = SceneManager.GetActiveScene().path;
            bool wasAdditive = !string.IsNullOrEmpty(originalScenePath) && originalScenePath != scenePath;

            Scene scene;
            if (wasAdditive)
            {
                scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
            else
            {
                scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }

            try
            {
                // Check for canvas
                var canvas = GameObject.FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    Success("MainMenu has Canvas");

                    // Check canvas scaler
                    var scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
                    if (scaler != null)
                    {
                        if (scaler.uiScaleMode == UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize)
                        {
                            Success("Canvas Scaler configured correctly");
                        }
                        else
                        {
                            Warning("Canvas Scaler should use Scale With Screen Size mode");
                        }
                    }
                    else
                    {
                        Error("MainMenu Canvas missing CanvasScaler");
                    }
                }
                else
                {
                    Error("MainMenu missing Canvas");
                }

                // Check for EventSystem
                var eventSystem = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
                if (eventSystem != null)
                {
                    Success("MainMenu has EventSystem");
                }
                else
                {
                    Error("MainMenu missing EventSystem");
                }
            }
            finally
            {
                // Only close if we opened additively
                if (wasAdditive)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        private static void VerifyGameplayScene(string scenePath)
        {
            // Remember the current scene so we can return to it
            string originalScenePath = SceneManager.GetActiveScene().path;
            bool wasAdditive = !string.IsNullOrEmpty(originalScenePath) && originalScenePath != scenePath;

            Scene scene;
            if (wasAdditive)
            {
                scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
            else
            {
                scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }

            try
            {
                // Check for canvas
                var canvas = GameObject.FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    Success("GameplayMain has Canvas");

                    // Check for layer hierarchy
                    string[] expectedLayers = {
                        "Layer_Background",
                        "Layer_Map",
                        "Layer_HUD",
                        "Layer_Panels",
                        "Layer_Overlays",
                        "Layer_Phone",
                        "Layer_Modal",
                        "Layer_Tutorial",
                        "Layer_Loading"
                    };

                    int foundLayers = 0;
                    foreach (var layerName in expectedLayers)
                    {
                        var layer = canvas.transform.Find(layerName);
                        if (layer != null)
                        {
                            foundLayers++;
                        }
                        else
                        {
                            Warning($"Missing layer: {layerName}");
                        }
                    }

                    if (foundLayers == expectedLayers.Length)
                    {
                        Success($"All {expectedLayers.Length} canvas layers present");
                    }
                    else
                    {
                        Warning($"Only {foundLayers}/{expectedLayers.Length} layers found");
                    }

                    // Check for UIManager
                    var uiManager = canvas.GetComponent<UI.Core.UIManager>();
                    if (uiManager != null)
                    {
                        Success("UIManager component attached to canvas");
                    }
                    else
                    {
                        Warning("UIManager not attached to canvas - add it manually or re-run setup");
                    }

                    // Check for SafeAreaHandler
                    var safeArea = canvas.GetComponentInChildren<UI.Core.SafeAreaHandler>();
                    if (safeArea != null)
                    {
                        Success("SafeAreaHandler present in scene");
                    }
                    else
                    {
                        Warning("SafeAreaHandler not found - add to HUD layer");
                    }
                }
                else
                {
                    Error("GameplayMain missing Canvas");
                }

                // Check for EventSystem
                var eventSystem = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
                if (eventSystem != null)
                {
                    Success("GameplayMain has EventSystem");
                }
                else
                {
                    Error("GameplayMain missing EventSystem");
                }
            }
            finally
            {
                // Only close if we opened additively
                if (wasAdditive)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        private static void VerifyUIComponents()
        {
            UnityEngine.Debug.Log("\n--- Verifying UI Components ---");

            // Check core scripts exist
            var componentChecks = new Dictionary<string, string>
            {
                { "UIControllerBase", "Assets/Scripts/UI/Core/UIControllerBase.cs" },
                { "UIManager", "Assets/Scripts/UI/Core/UIManager.cs" },
                { "UIAnimations", "Assets/Scripts/UI/Core/UIAnimations.cs" },
                { "ButtonFeedback", "Assets/Scripts/UI/Core/ButtonFeedback.cs" },
                { "AccessibleText", "Assets/Scripts/UI/Core/AccessibleText.cs" },
                { "SafeAreaHandler", "Assets/Scripts/UI/Core/SafeAreaHandler.cs" },
                { "SceneLoader", "Assets/Scripts/UI/Core/SceneLoader.cs" },
                { "DesignTokens", "Assets/Scripts/UI/Core/DesignTokens.cs" }
            };

            foreach (var check in componentChecks)
            {
                if (System.IO.File.Exists(check.Value))
                {
                    Success($"{check.Key} script exists");
                }
                else
                {
                    Error($"{check.Key} script missing at {check.Value}");
                }
            }

            // Verify UIAnimations has reduced motion support
            string animationsPath = "Assets/Scripts/UI/Core/UIAnimations.cs";
            if (System.IO.File.Exists(animationsPath))
            {
                string content = System.IO.File.ReadAllText(animationsPath);
                if (content.Contains("ReducedMotion"))
                {
                    Success("UIAnimations has ReducedMotion support");
                }
                else
                {
                    Warning("UIAnimations should have ReducedMotion support");
                }
            }
        }

        private static void VerifyPrefabs()
        {
            UnityEngine.Debug.Log("\n--- Verifying Prefabs ---");

            var prefabChecks = new Dictionary<string, string>
            {
                { "HUD/TopBar", "Assets/Prefabs/UI/HUD/TopBar.prefab" },
                { "HUD/BottomBar", "Assets/Prefabs/UI/HUD/BottomBar.prefab" },
                { "Phone/PhoneOverlay", "Assets/Prefabs/UI/Phone/PhoneOverlay.prefab" },
                { "Map/MapOverlay", "Assets/Prefabs/UI/Map/MapOverlay.prefab" },
                { "Common/Button", "Assets/Prefabs/UI/Common/Button.prefab" }
            };

            foreach (var check in prefabChecks)
            {
                if (System.IO.File.Exists(check.Value))
                {
                    Success($"{check.Key} prefab exists");

                    // Load and check for ButtonFeedback on buttons
                    if (check.Key.Contains("Button"))
                    {
                        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(check.Value);
                        if (prefab != null)
                        {
                            var feedback = prefab.GetComponent<UI.Core.ButtonFeedback>();
                            if (feedback != null)
                            {
                                Success($"{check.Key} has ButtonFeedback component");
                            }
                            else
                            {
                                Warning($"{check.Key} should have ButtonFeedback component");
                            }
                        }
                    }
                }
                else
                {
                    Warning($"{check.Key} prefab not found at {check.Value}");
                }
            }
        }

        private static void Success(string message)
        {
            _successes.Add(message);
            UnityEngine.Debug.Log($"<color=green>✓</color> {message}");
        }

        private static void Warning(string message)
        {
            _warnings.Add(message);
            UnityEngine.Debug.LogWarning($"<color=yellow>!</color> {message}");
        }

        private static void Error(string message)
        {
            _errors.Add(message);
            UnityEngine.Debug.LogError($"<color=red>✗</color> {message}");
        }

        private static void PrintSummary()
        {
            UnityEngine.Debug.Log("\n=== Foundation Verification Summary ===");
            UnityEngine.Debug.Log($"<color=green>Passed: {_successes.Count}</color>");
            UnityEngine.Debug.Log($"<color=yellow>Warnings: {_warnings.Count}</color>");
            UnityEngine.Debug.Log($"<color=red>Errors: {_errors.Count}</color>");

            if (_errors.Count == 0 && _warnings.Count == 0)
            {
                UnityEngine.Debug.Log("\n<color=green>*** FOUNDATION VERIFICATION PASSED ***</color>");
                UnityEngine.Debug.Log("Task 3 Checkpoint Complete - Foundation is ready!");
            }
            else if (_errors.Count == 0)
            {
                UnityEngine.Debug.Log("\n<color=yellow>*** FOUNDATION MOSTLY COMPLETE ***</color>");
                UnityEngine.Debug.Log("No critical errors. Review warnings above.");
            }
            else
            {
                UnityEngine.Debug.Log("\n<color=red>*** FOUNDATION INCOMPLETE ***</color>");
                UnityEngine.Debug.Log("Fix errors above before proceeding.");
                UnityEngine.Debug.Log("Run 'Tools > Setup Scenes' to generate missing scene content.");
            }
        }

        [MenuItem("Tools/Quick Fix/Add ButtonFeedback to All Buttons")]
        public static void AddButtonFeedbackToAllButtons()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs/UI" });
            int added = 0;

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab == null) continue;

                var prefabRoot = PrefabUtility.LoadPrefabContents(path);
                bool modified = false;

                try
                {
                    var buttons = prefabRoot.GetComponentsInChildren<UnityEngine.UI.Button>(true);
                    foreach (var button in buttons)
                    {
                        if (button.GetComponent<UI.Core.ButtonFeedback>() == null)
                        {
                            button.gameObject.AddComponent<UI.Core.ButtonFeedback>();
                            modified = true;
                            added++;
                        }
                    }

                    if (modified)
                    {
                        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefabRoot);
                }
            }

            UnityEngine.Debug.Log($"[Quick Fix] Added ButtonFeedback to {added} buttons");
            AssetDatabase.Refresh();
        }
    }
}
