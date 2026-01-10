using UnityEngine;

namespace EventPlannerSim.UI.Core
{
    /// <summary>
    /// Adjusts RectTransform to respect device safe areas (notches, home indicators, etc.).
    /// Attach to any UI element that should be constrained to the safe area.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool applyTop = true;
        [SerializeField] private bool applyBottom = true;
        [SerializeField] private bool applyLeft = true;
        [SerializeField] private bool applyRight = true;

        [Header("Debug")]
        [SerializeField] private bool simulateSafeArea = false;
        [SerializeField] private Vector4 simulatedInsets = new Vector4(0, 44, 0, 34); // left, top, right, bottom (iPhone X style)

        private RectTransform _rectTransform;
        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            ApplySafeArea();
        }

        private void Update()
        {
            // Check if safe area or screen size changed (orientation change)
            if (ScreenSizeChanged() || SafeAreaChanged())
            {
                ApplySafeArea();
            }
        }

        private bool ScreenSizeChanged()
        {
            Vector2Int currentSize = new Vector2Int(Screen.width, Screen.height);
            if (currentSize != _lastScreenSize)
            {
                _lastScreenSize = currentSize;
                return true;
            }
            return false;
        }

        private bool SafeAreaChanged()
        {
            Rect currentSafeArea = GetSafeArea();
            if (currentSafeArea != _lastSafeArea)
            {
                _lastSafeArea = currentSafeArea;
                return true;
            }
            return false;
        }

        private Rect GetSafeArea()
        {
            if (simulateSafeArea)
            {
                // Create simulated safe area from insets
                return new Rect(
                    simulatedInsets.x,
                    simulatedInsets.w,
                    Screen.width - simulatedInsets.x - simulatedInsets.z,
                    Screen.height - simulatedInsets.y - simulatedInsets.w
                );
            }

            return Screen.safeArea;
        }

        private void ApplySafeArea()
        {
            Rect safeArea = GetSafeArea();

            // Convert safe area to anchor positions (0-1 range)
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            // Apply only specified edges
            if (!applyLeft) anchorMin.x = 0;
            if (!applyBottom) anchorMin.y = 0;
            if (!applyRight) anchorMax.x = 1;
            if (!applyTop) anchorMax.y = 1;

            // Apply to RectTransform
            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;

            Debug.Log($"[SafeAreaHandler] Applied safe area: {safeArea}");
        }

        /// <summary>
        /// Force refresh the safe area adjustment.
        /// </summary>
        public void Refresh()
        {
            ApplySafeArea();
        }
    }
}
