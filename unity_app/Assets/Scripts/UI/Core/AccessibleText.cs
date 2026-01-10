using UnityEngine;
using TMPro;

namespace EventPlannerSim.UI.Core
{
    /// <summary>
    /// Applies accessibility text scaling to TextMeshPro components.
    /// Scales font size based on global accessibility settings.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class AccessibleText : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool scaleWithAccessibility = true;
        [SerializeField] private float minScale = 0.8f;
        [SerializeField] private float maxScale = 1.5f;

        private TextMeshProUGUI _textComponent;
        private float _baseFontSize;

        /// <summary>
        /// Global text scale multiplier (1.0 = normal).
        /// Set this from accessibility settings.
        /// </summary>
        public static float GlobalTextScale { get; set; } = 1.0f;

        /// <summary>
        /// Event fired when global text scale changes.
        /// </summary>
        public static event System.Action OnTextScaleChanged;

        private void Awake()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();
            _baseFontSize = _textComponent.fontSize;
        }

        private void OnEnable()
        {
            OnTextScaleChanged += ApplyScale;
            ApplyScale();
        }

        private void OnDisable()
        {
            OnTextScaleChanged -= ApplyScale;
        }

        /// <summary>
        /// Apply the current accessibility scale.
        /// </summary>
        public void ApplyScale()
        {
            if (_textComponent == null || !scaleWithAccessibility) return;

            float clampedScale = Mathf.Clamp(GlobalTextScale, minScale, maxScale);
            _textComponent.fontSize = _baseFontSize * clampedScale;
        }

        /// <summary>
        /// Set the global text scale and notify all AccessibleText components.
        /// </summary>
        public static void SetGlobalScale(float scale)
        {
            GlobalTextScale = scale;
            OnTextScaleChanged?.Invoke();
        }

        /// <summary>
        /// Reset the font size to base value.
        /// </summary>
        public void ResetScale()
        {
            if (_textComponent != null)
            {
                _textComponent.fontSize = _baseFontSize;
            }
        }
    }
}
