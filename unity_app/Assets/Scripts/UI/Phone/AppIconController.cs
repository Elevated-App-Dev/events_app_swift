using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;

namespace EventPlannerSim.UI.Phone
{
    /// <summary>
    /// Controls a single app icon on the phone home screen.
    /// Displays icon, label, and notification badge.
    /// </summary>
    public class AppIconController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField] private Button button;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI labelText;

        [Header("Badge")]
        [SerializeField] private GameObject badgeContainer;
        [SerializeField] private TextMeshProUGUI badgeText;

        [Header("Settings")]
        [SerializeField] private PhoneApp appType;

        #endregion

        #region Properties

        /// <summary>
        /// The app type this icon represents.
        /// </summary>
        public PhoneApp App => appType;

        #endregion

        #region Events

        /// <summary>
        /// Fired when the icon is tapped.
        /// </summary>
        public event Action OnTapped;

        #endregion

        #region Lifecycle

        private void Awake()
        {
            if (button != null)
            {
                button.onClick.AddListener(HandleTap);
            }

            // Hide badge by default
            if (badgeContainer != null)
            {
                badgeContainer.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(HandleTap);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the badge count. Hides badge if count is 0.
        /// </summary>
        public void SetBadgeCount(int count)
        {
            if (badgeContainer == null) return;

            if (count <= 0)
            {
                badgeContainer.SetActive(false);
            }
            else
            {
                badgeContainer.SetActive(true);

                if (badgeText != null)
                {
                    // Cap display at 99+
                    badgeText.text = count > 99 ? "99+" : count.ToString();
                }

                // Animate badge pop
                StartCoroutine(UIAnimations.AnimateScalePop(badgeContainer.transform, 1.1f));
            }
        }

        /// <summary>
        /// Set the icon sprite.
        /// </summary>
        public void SetIcon(Sprite sprite)
        {
            if (iconImage != null)
            {
                iconImage.sprite = sprite;
            }
        }

        /// <summary>
        /// Set the label text.
        /// </summary>
        public void SetLabel(string label)
        {
            if (labelText != null)
            {
                labelText.text = label;
            }
        }

        /// <summary>
        /// Enable or disable the icon.
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }

            // Dim the icon when disabled
            if (iconImage != null)
            {
                iconImage.color = interactable
                    ? Color.white
                    : DesignTokens.TextMuted;
            }
        }

        #endregion

        #region Private Methods

        private void HandleTap()
        {
            OnTapped?.Invoke();
        }

        #endregion
    }
}
