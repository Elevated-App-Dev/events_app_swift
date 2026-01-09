using UnityEngine;
using UnityEngine.EventSystems;
using EventPlannerSim.UI.Core;

namespace EventPlannerSim.UI.Common
{
    /// <summary>
    /// Provides tactile press feedback for buttons.
    /// Attaches to any button to add scale animation on press.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Header("Animation Settings")]
        [SerializeField] private float pressScale = 0.97f;
        [SerializeField] private float pressDuration = 0.05f;
        [SerializeField] private float releaseDuration = 0.1f;

        [Header("Audio")]
        [SerializeField] private bool playSound = true;

        private RectTransform _rectTransform;
        private Coroutine _currentAnimation;
        private bool _isPressed;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!enabled) return;

            _isPressed = true;

            // Cancel any running animation
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
            }

            // Animate to pressed scale
            _currentAnimation = StartCoroutine(
                UIAnimations.AnimateScale(
                    transform,
                    Vector3.one * pressScale,
                    UIAnimations.GetDuration(pressDuration)
                )
            );

            // Play sound
            if (playSound)
            {
                PlayClickSound();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!enabled || !_isPressed) return;

            _isPressed = false;

            // Cancel any running animation
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
            }

            // Animate back to normal scale
            _currentAnimation = StartCoroutine(
                UIAnimations.AnimateScale(
                    transform,
                    Vector3.one,
                    UIAnimations.GetDuration(releaseDuration)
                )
            );
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!enabled || !_isPressed) return;

            _isPressed = false;

            // Cancel any running animation
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
            }

            // Animate back to normal scale
            _currentAnimation = StartCoroutine(
                UIAnimations.AnimateScale(
                    transform,
                    Vector3.one,
                    UIAnimations.GetDuration(releaseDuration)
                )
            );
        }

        private void OnDisable()
        {
            // Reset scale when disabled
            transform.localScale = Vector3.one;
            _isPressed = false;

            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
                _currentAnimation = null;
            }
        }

        private void PlayClickSound()
        {
            // Will integrate with AudioManager
            // GameManager.Instance?.AudioManager?.PlaySFX(SoundEffect.ButtonClick);
        }
    }
}
