using UnityEngine;

namespace EventPlannerSim.UI.Common
{
    /// <summary>
    /// Adjusts RectTransform to respect device safe areas (notches, home indicators).
    /// Attach to any UI container that should respect safe areas.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool applyTop = true;
        [SerializeField] private bool applyBottom = true;
        [SerializeField] private bool applyLeft = true;
        [SerializeField] private bool applyRight = true;

        private RectTransform _rectTransform;
        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;
        private ScreenOrientation _lastOrientation;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        private void Update()
        {
            // Check if safe area or screen changed
            if (_lastSafeArea != Screen.safeArea ||
                _lastScreenSize.x != Screen.width ||
                _lastScreenSize.y != Screen.height ||
                _lastOrientation != Screen.orientation)
            {
                ApplySafeArea();
            }
        }

        /// <summary>
        /// Apply the current device safe area to the RectTransform.
        /// </summary>
        private void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;

            // Store current values
            _lastSafeArea = safeArea;
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
            _lastOrientation = Screen.orientation;

            // Early exit if no valid screen size
            if (Screen.width <= 0 || Screen.height <= 0) return;

            // Convert safe area to anchors
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            // Apply only to selected edges
            if (!applyLeft) anchorMin.x = 0;
            if (!applyBottom) anchorMin.y = 0;
            if (!applyRight) anchorMax.x = 1;
            if (!applyTop) anchorMax.y = 1;

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// Force refresh the safe area.
        /// </summary>
        public void Refresh()
        {
            ApplySafeArea();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Simulate different safe areas in editor for testing.
        /// </summary>
        [Header("Editor Testing")]
        [SerializeField] private bool simulateSafeArea = false;
        [SerializeField] private float simulatedTopInset = 44f;
        [SerializeField] private float simulatedBottomInset = 34f;

        private void OnValidate()
        {
            if (simulateSafeArea && Application.isPlaying)
            {
                ApplySafeArea();
            }
        }
#endif
    }
}
