using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using System;

namespace EventPlannerSim.UI.MainMenu
{
    /// <summary>
    /// Controls the main menu screen.
    /// </summary>
    public class MainMenuController : UIControllerBase
    {
        [Header("Buttons")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button creditsButton;

        [Header("Title")]
        [SerializeField] private TextMeshProUGUI titleText;

        public event Action OnNewGame;
        public event Action OnContinue;
        public event Action OnSettings;
        public event Action OnCredits;

        private bool _hasSaveFile;

        protected override void Awake()
        {
            base.Awake();

            if (newGameButton != null)
            {
                newGameButton.onClick.AddListener(HandleNewGame);
            }
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(HandleContinue);
            }
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(HandleSettings);
            }
            if (creditsButton != null)
            {
                creditsButton.onClick.AddListener(HandleCredits);
            }
        }

        protected override void SubscribeToEvents() { }
        protected override void UnsubscribeFromEvents() { }

        protected override void RefreshDisplay()
        {
            CheckForSaveFile();
            UpdateContinueButton();
        }

        private void CheckForSaveFile()
        {
            // Check if a save file exists
            _hasSaveFile = PlayerPrefs.HasKey("SaveExists") && PlayerPrefs.GetInt("SaveExists") == 1;

            // Or check for actual save file
            // _hasSaveFile = System.IO.File.Exists(Application.persistentDataPath + "/save.json");
        }

        private void UpdateContinueButton()
        {
            if (continueButton != null)
            {
                continueButton.interactable = _hasSaveFile;

                // Optionally change visual appearance
                var colors = continueButton.colors;
                colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                continueButton.colors = colors;
            }
        }

        private void HandleNewGame()
        {
            if (_hasSaveFile)
            {
                // Should show confirmation dialog
                // "Starting a new game will overwrite your existing save. Continue?"
                ShowNewGameConfirmation();
            }
            else
            {
                OnNewGame?.Invoke();
            }
        }

        private void ShowNewGameConfirmation()
        {
            // For now, just start new game
            // In full implementation, show a dialog
            OnNewGame?.Invoke();
        }

        private void HandleContinue()
        {
            if (_hasSaveFile)
            {
                OnContinue?.Invoke();
            }
        }

        private void HandleSettings()
        {
            OnSettings?.Invoke();
        }

        private void HandleCredits()
        {
            OnCredits?.Invoke();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            RefreshDisplay();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
