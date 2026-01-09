using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using System;

namespace EventPlannerSim.UI.Settings
{
    /// <summary>
    /// Controls the settings panel.
    /// </summary>
    public class SettingsController : UIControllerBase
    {
        [Header("Audio")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private TextMeshProUGUI sfxVolumeText;

        [Header("Notifications")]
        [SerializeField] private Toggle notificationsToggle;

        [Header("Accessibility")]
        [SerializeField] private TMP_Dropdown textSizeDropdown;

        [Header("Account")]
        [SerializeField] private Button restorePurchasesButton;
        [SerializeField] private Button resetGameButton;
        [SerializeField] private Button privacyButton;

        [Header("Info")]
        [SerializeField] private TextMeshProUGUI versionText;

        [Header("Navigation")]
        [SerializeField] private Button backButton;

        public event Action OnBackRequested;
        public event Action OnRestorePurchases;
        public event Action OnResetGame;
        public event Action OnPrivacySettings;

        protected override void Awake()
        {
            base.Awake();

            // Audio sliders
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            }

            // Notifications
            if (notificationsToggle != null)
            {
                notificationsToggle.onValueChanged.AddListener(OnNotificationsChanged);
            }

            // Text size
            if (textSizeDropdown != null)
            {
                textSizeDropdown.onValueChanged.AddListener(OnTextSizeChanged);
            }

            // Buttons
            if (restorePurchasesButton != null)
            {
                restorePurchasesButton.onClick.AddListener(HandleRestorePurchases);
            }
            if (resetGameButton != null)
            {
                resetGameButton.onClick.AddListener(HandleResetGame);
            }
            if (privacyButton != null)
            {
                privacyButton.onClick.AddListener(HandlePrivacy);
            }
            if (backButton != null)
            {
                backButton.onClick.AddListener(HandleBack);
            }
        }

        protected override void SubscribeToEvents() { }
        protected override void UnsubscribeFromEvents() { }

        protected override void RefreshDisplay()
        {
            LoadCurrentSettings();
            UpdateVersionDisplay();
        }

        private void LoadCurrentSettings()
        {
            // Load from PlayerPrefs or settings system
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
            }
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
            }
            if (notificationsToggle != null)
            {
                notificationsToggle.isOn = PlayerPrefs.GetInt("Notifications", 1) == 1;
            }
            if (textSizeDropdown != null)
            {
                textSizeDropdown.value = PlayerPrefs.GetInt("TextSize", 1); // 0=Small, 1=Medium, 2=Large
            }

            UpdateVolumeLabels();
        }

        private void UpdateVersionDisplay()
        {
            if (versionText != null)
            {
                versionText.text = $"Version {Application.version}";
            }
        }

        private void OnMusicVolumeChanged(float value)
        {
            PlayerPrefs.SetFloat("MusicVolume", value);
            UpdateVolumeLabels();
            // Apply to audio system
        }

        private void OnSfxVolumeChanged(float value)
        {
            PlayerPrefs.SetFloat("SFXVolume", value);
            UpdateVolumeLabels();
            // Apply to audio system
        }

        private void UpdateVolumeLabels()
        {
            if (musicVolumeText != null && musicVolumeSlider != null)
            {
                musicVolumeText.text = $"{musicVolumeSlider.value * 100:F0}%";
            }
            if (sfxVolumeText != null && sfxVolumeSlider != null)
            {
                sfxVolumeText.text = $"{sfxVolumeSlider.value * 100:F0}%";
            }
        }

        private void OnNotificationsChanged(bool enabled)
        {
            PlayerPrefs.SetInt("Notifications", enabled ? 1 : 0);
        }

        private void OnTextSizeChanged(int index)
        {
            PlayerPrefs.SetInt("TextSize", index);
            // Apply to accessibility system
        }

        private void HandleRestorePurchases()
        {
            OnRestorePurchases?.Invoke();
        }

        private void HandleResetGame()
        {
            // Should show confirmation dialog first
            OnResetGame?.Invoke();
        }

        private void HandlePrivacy()
        {
            OnPrivacySettings?.Invoke();
        }

        private void HandleBack()
        {
            PlayerPrefs.Save();
            OnBackRequested?.Invoke();
            Hide();
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
