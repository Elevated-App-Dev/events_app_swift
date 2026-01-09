using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EventPlannerSim.UI.Notifications
{
    /// <summary>
    /// Manages notification popups with queue and auto-dismiss functionality.
    /// </summary>
    public class NotificationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private Transform notificationContainer;

        [Header("Settings")]
        [SerializeField] private float displayDuration = 4f;
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private int maxVisibleNotifications = 3;

        [Header("Notification Elements (if using single popup)")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button dismissButton;

        private Queue<NotificationData> _notificationQueue = new Queue<NotificationData>();
        private List<NotificationPopup> _activeNotifications = new List<NotificationPopup>();
        private bool _isProcessing;

        private void Awake()
        {
            if (dismissButton != null)
            {
                dismissButton.onClick.AddListener(DismissCurrent);
            }
        }

        public void QueueNotification(string title, string message, NotificationType type = NotificationType.Info)
        {
            var data = new NotificationData
            {
                title = title,
                message = message,
                type = type
            };

            _notificationQueue.Enqueue(data);

            if (!_isProcessing)
            {
                StartCoroutine(ProcessQueue());
            }
        }

        public void QueueNotification(NotificationData data)
        {
            _notificationQueue.Enqueue(data);

            if (!_isProcessing)
            {
                StartCoroutine(ProcessQueue());
            }
        }

        private IEnumerator ProcessQueue()
        {
            _isProcessing = true;

            while (_notificationQueue.Count > 0)
            {
                // Wait if we have max visible notifications
                while (_activeNotifications.Count >= maxVisibleNotifications)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                var data = _notificationQueue.Dequeue();
                ShowNotification(data);

                // Small delay between notifications
                yield return new WaitForSeconds(0.2f);
            }

            _isProcessing = false;
        }

        private void ShowNotification(NotificationData data)
        {
            if (notificationPrefab != null && notificationContainer != null)
            {
                // Spawn from prefab
                var popupObj = Instantiate(notificationPrefab, notificationContainer);
                var popup = popupObj.GetComponent<NotificationPopup>();

                if (popup != null)
                {
                    popup.SetData(data);
                    popup.OnDismissed += () => HandlePopupDismissed(popup);
                    popup.Show(animationDuration);
                    _activeNotifications.Add(popup);

                    // Auto-dismiss after duration
                    StartCoroutine(AutoDismiss(popup, displayDuration));
                }
            }
            else
            {
                // Use single popup mode
                ShowSingleNotification(data);
            }
        }

        private void ShowSingleNotification(NotificationData data)
        {
            gameObject.SetActive(true);

            if (titleText != null)
                titleText.text = data.title;

            if (messageText != null)
                messageText.text = data.message;

            if (iconImage != null)
                iconImage.color = GetColorForType(data.type);

            // Auto-dismiss
            StartCoroutine(AutoDismissSingle(displayDuration));
        }

        private IEnumerator AutoDismiss(NotificationPopup popup, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (popup != null && _activeNotifications.Contains(popup))
            {
                popup.Dismiss(animationDuration);
            }
        }

        private IEnumerator AutoDismissSingle(float delay)
        {
            yield return new WaitForSeconds(delay);
            DismissCurrent();
        }

        private void HandlePopupDismissed(NotificationPopup popup)
        {
            _activeNotifications.Remove(popup);
            if (popup != null)
            {
                Destroy(popup.gameObject);
            }
        }

        private void DismissCurrent()
        {
            gameObject.SetActive(false);
        }

        private Color GetColorForType(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => DesignTokens.Success,
                NotificationType.Warning => DesignTokens.Warning,
                NotificationType.Error => DesignTokens.Error,
                NotificationType.Info => DesignTokens.Accent,
                _ => Color.white
            };
        }

        public void ClearAll()
        {
            _notificationQueue.Clear();

            foreach (var popup in _activeNotifications)
            {
                if (popup != null)
                {
                    Destroy(popup.gameObject);
                }
            }
            _activeNotifications.Clear();
        }
    }

    /// <summary>
    /// Individual notification popup component.
    /// </summary>
    public class NotificationPopup : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button dismissButton;
        [SerializeField] private CanvasGroup canvasGroup;

        public event Action OnDismissed;

        private void Awake()
        {
            if (dismissButton != null)
            {
                dismissButton.onClick.AddListener(() => Dismiss(0.2f));
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void SetData(NotificationData data)
        {
            if (titleText != null)
                titleText.text = data.title;

            if (messageText != null)
                messageText.text = data.message;

            if (iconImage != null)
                iconImage.color = GetColorForType(data.type);
        }

        public void Show(float duration)
        {
            StartCoroutine(AnimateIn(duration));
        }

        public void Dismiss(float duration)
        {
            StartCoroutine(AnimateOut(duration));
        }

        private IEnumerator AnimateIn(float duration)
        {
            if (canvasGroup == null) yield break;

            canvasGroup.alpha = 0f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        private IEnumerator AnimateOut(float duration)
        {
            if (canvasGroup != null)
            {
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                    yield return null;
                }
            }

            OnDismissed?.Invoke();
        }

        private Color GetColorForType(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => DesignTokens.Success,
                NotificationType.Warning => DesignTokens.Warning,
                NotificationType.Error => DesignTokens.Error,
                NotificationType.Info => DesignTokens.Accent,
                _ => Color.white
            };
        }
    }

    public struct NotificationData
    {
        public string title;
        public string message;
        public NotificationType type;
    }

    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }
}
