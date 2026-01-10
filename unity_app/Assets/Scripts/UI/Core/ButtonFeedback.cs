using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace EventPlannerSim.UI.Core
{
    /// <summary>
    /// Provides visual and audio feedback for button interactions.
    /// Attach to any Button component for press/release animations.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Scale Animation")]
        [SerializeField] private float pressScale = 0.97f;
        [SerializeField] private float pressDuration = 0.08f;
        [SerializeField] private float releaseDuration = 0.12f;

        [Header("Audio")]
        [SerializeField] private AudioClip pressSound;
        [SerializeField] private AudioClip releaseSound;

        private Button _button;
        private Transform _targetTransform;
        private Vector3 _originalScale;
        private Coroutine _currentAnimation;
        private AudioSource _audioSource;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _targetTransform = transform;
            _originalScale = _targetTransform.localScale;

            // Try to find or create audio source
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null && (pressSound != null || releaseSound != null))
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_button.interactable) return;

            // Play press sound
            PlaySound(pressSound);

            // Animate scale down
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
            }

            if (UIAnimations.ReducedMotion)
            {
                _targetTransform.localScale = _originalScale * pressScale;
            }
            else
            {
                _currentAnimation = StartCoroutine(AnimateScale(_originalScale * pressScale, pressDuration));
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_button.interactable) return;

            // Play release sound
            PlaySound(releaseSound);

            // Animate scale back
            if (_currentAnimation != null)
            {
                StopCoroutine(_currentAnimation);
            }

            if (UIAnimations.ReducedMotion)
            {
                _targetTransform.localScale = _originalScale;
            }
            else
            {
                _currentAnimation = StartCoroutine(AnimateScale(_originalScale, releaseDuration));
            }
        }

        private IEnumerator AnimateScale(Vector3 targetScale, float duration)
        {
            Vector3 startScale = _targetTransform.localScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = UIAnimations.EaseOutCubic(Mathf.Clamp01(elapsed / duration));
                _targetTransform.localScale = Vector3.LerpUnclamped(startScale, targetScale, t);
                yield return null;
            }

            _targetTransform.localScale = targetScale;
            _currentAnimation = null;
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(clip);
            }
        }

        private void OnDisable()
        {
            // Reset scale when disabled
            if (_targetTransform != null)
            {
                _targetTransform.localScale = _originalScale;
            }
            _currentAnimation = null;
        }
    }
}
