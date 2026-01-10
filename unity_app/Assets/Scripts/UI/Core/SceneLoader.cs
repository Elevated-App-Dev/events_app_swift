using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EventPlannerSim.UI.Core
{
    /// <summary>
    /// Utility class for loading scenes with transition effects.
    /// Supports loading screen display and progress reporting.
    /// </summary>
    public static class SceneLoader
    {
        /// <summary>
        /// Event fired when scene loading begins.
        /// </summary>
        public static event Action<string> OnLoadStarted;

        /// <summary>
        /// Event fired during loading with progress (0-1).
        /// </summary>
        public static event Action<float> OnLoadProgress;

        /// <summary>
        /// Event fired when scene loading completes.
        /// </summary>
        public static event Action<string> OnLoadComplete;

        /// <summary>
        /// Whether a scene is currently loading.
        /// </summary>
        public static bool IsLoading { get; private set; }

        /// <summary>
        /// The currently loading scene name.
        /// </summary>
        public static string CurrentLoadingScene { get; private set; }

        /// <summary>
        /// Load a scene by name with optional loading screen.
        /// </summary>
        public static void LoadScene(string sceneName, bool showLoadingScreen = true)
        {
            if (IsLoading)
            {
                Debug.LogWarning($"[SceneLoader] Already loading scene '{CurrentLoadingScene}', ignoring request for '{sceneName}'");
                return;
            }

            // Start loading coroutine on a persistent object
            var loader = GetOrCreateLoader();
            loader.StartCoroutine(LoadSceneAsync(sceneName, showLoadingScreen));
        }

        /// <summary>
        /// Load the main menu scene.
        /// </summary>
        public static void LoadMainMenu()
        {
            LoadScene("MainMenu", true);
        }

        /// <summary>
        /// Load the gameplay scene.
        /// </summary>
        public static void LoadGameplay()
        {
            LoadScene("GameplayMain", true);
        }

        private static IEnumerator LoadSceneAsync(string sceneName, bool showLoadingScreen)
        {
            IsLoading = true;
            CurrentLoadingScene = sceneName;
            OnLoadStarted?.Invoke(sceneName);

            // Show loading screen if available
            if (showLoadingScreen && UIManager.HasInstance)
            {
                UIManager.Instance.ShowLoading();
            }

            // Small delay to ensure loading screen is visible
            yield return new WaitForSecondsRealtime(0.1f);

            // Start async load
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            // Report progress
            while (asyncLoad.progress < 0.9f)
            {
                // Unity's async load progress goes 0-0.9 before activation
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                OnLoadProgress?.Invoke(progress);
                yield return null;
            }

            // Scene is ready, report full progress
            OnLoadProgress?.Invoke(1f);

            // Small delay at 100% for polish
            yield return new WaitForSecondsRealtime(0.2f);

            // Activate the scene
            asyncLoad.allowSceneActivation = true;

            // Wait for activation
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Hide loading screen
            if (showLoadingScreen && UIManager.HasInstance)
            {
                UIManager.Instance.HideLoading();
            }

            IsLoading = false;
            CurrentLoadingScene = null;
            OnLoadComplete?.Invoke(sceneName);

            Debug.Log($"[SceneLoader] Loaded scene: {sceneName}");
        }

        /// <summary>
        /// Reload the current scene.
        /// </summary>
        public static void ReloadCurrentScene()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            LoadScene(currentScene, true);
        }

        /// <summary>
        /// Get or create the persistent loader object.
        /// </summary>
        private static MonoBehaviour GetOrCreateLoader()
        {
            var loaderObj = GameObject.Find("SceneLoaderRunner");
            if (loaderObj == null)
            {
                loaderObj = new GameObject("SceneLoaderRunner");
                loaderObj.AddComponent<SceneLoaderRunner>();
                UnityEngine.Object.DontDestroyOnLoad(loaderObj);
            }
            return loaderObj.GetComponent<SceneLoaderRunner>();
        }

        /// <summary>
        /// Helper MonoBehaviour for running coroutines.
        /// </summary>
        private class SceneLoaderRunner : MonoBehaviour { }
    }
}
