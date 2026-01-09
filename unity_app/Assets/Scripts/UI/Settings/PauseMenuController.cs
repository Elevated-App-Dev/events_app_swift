using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using System;

namespace EventPlannerSim.UI.Settings
{
    /// <summary>
    /// Controls the pause menu overlay.
    /// </summary>
    public class PauseMenuController : UIControllerBase
    {
        [Header("Background")]
        [SerializeField] private Image dimBackground;

        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        public event Action OnResumeRequested;
        public event Action OnSettingsRequested;
        public event Action OnQuitRequested;

        private bool _wasPaused;

        protected override void Awake()
        {
            base.Awake();

            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(HandleResume);
            }
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(HandleSettings);
            }
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(HandleQuit);
            }
        }

        protected override void SubscribeToEvents() { }
        protected override void UnsubscribeFromEvents() { }

        protected override void RefreshDisplay() { }

        public void Show()
        {
            gameObject.SetActive(true);

            // Pause time
            _wasPaused = Time.timeScale == 0f;
            Time.timeScale = 0f;
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            // Resume time (unless it was already paused)
            if (!_wasPaused)
            {
                Time.timeScale = 1f;
            }
        }

        private void HandleResume()
        {
            OnResumeRequested?.Invoke();
            Hide();
        }

        private void HandleSettings()
        {
            OnSettingsRequested?.Invoke();
            // Don't hide - settings opens on top
        }

        private void HandleQuit()
        {
            OnQuitRequested?.Invoke();
            // Will likely trigger scene load
        }
    }
}
