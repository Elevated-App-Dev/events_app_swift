using System;
using System.Collections;
using UnityEngine;

namespace EventPlannerSim.UI.Core
{
    /// <summary>
    /// Static utility class for UI animations.
    /// Follows Mini Metro-inspired minimalist animation principles.
    /// </summary>
    public static class UIAnimations
    {
        #region Timing Constants

        /// <summary>
        /// Instant - state changes, no visible animation.
        /// </summary>
        public const float Instant = 0f;

        /// <summary>
        /// Fast - button feedback, micro-interactions.
        /// </summary>
        public const float Fast = 0.1f;

        /// <summary>
        /// Normal - panel transitions, reveals.
        /// </summary>
        public const float Normal = 0.2f;

        /// <summary>
        /// Slow - full-screen transitions.
        /// </summary>
        public const float Slow = 0.3f;

        /// <summary>
        /// Emphasis - celebration moments, important reveals.
        /// </summary>
        public const float Emphasis = 0.4f;

        #endregion

        #region Settings

        /// <summary>
        /// Whether reduced motion mode is enabled.
        /// When true, animations are instant.
        /// </summary>
        public static bool ReducedMotion { get; set; } = false;

        /// <summary>
        /// Get animation duration, respecting reduced motion setting.
        /// </summary>
        public static float GetDuration(float normalDuration)
        {
            return ReducedMotion ? 0f : normalDuration;
        }

        #endregion

        #region Scale Animations

        /// <summary>
        /// Animate scale with easing.
        /// </summary>
        public static IEnumerator AnimateScale(Transform target, Vector3 toScale, float duration, Action onComplete = null)
        {
            if (target == null) yield break;

            duration = GetDuration(duration);

            if (duration <= 0)
            {
                target.localScale = toScale;
                onComplete?.Invoke();
                yield break;
            }

            Vector3 fromScale = target.localScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = EaseOutCubic(Mathf.Clamp01(elapsed / duration));
                target.localScale = Vector3.LerpUnclamped(fromScale, toScale, t);
                yield return null;
            }

            target.localScale = toScale;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Button press scale animation (down then up).
        /// </summary>
        public static IEnumerator AnimateButtonPress(Transform target, float pressScale = 0.97f)
        {
            if (target == null) yield break;

            // Press down
            yield return AnimateScale(target, Vector3.one * pressScale, Fast);

            // Release up
            yield return AnimateScale(target, Vector3.one, Fast * 1.5f);
        }

        /// <summary>
        /// Scale pop animation for emphasis.
        /// </summary>
        public static IEnumerator AnimateScalePop(Transform target, float overshoot = 1.05f)
        {
            if (target == null) yield break;

            var duration = GetDuration(Normal);
            if (duration <= 0)
            {
                target.localScale = Vector3.one;
                yield break;
            }

            // Pop up with overshoot
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float scale = EaseOutBack(t, overshoot);
                target.localScale = Vector3.one * scale;
                yield return null;
            }

            target.localScale = Vector3.one;
        }

        #endregion

        #region Fade Animations

        /// <summary>
        /// Animate canvas group alpha.
        /// </summary>
        public static IEnumerator AnimateFade(CanvasGroup canvasGroup, float toAlpha, float duration, Action onComplete = null)
        {
            if (canvasGroup == null) yield break;

            duration = GetDuration(duration);

            if (duration <= 0)
            {
                canvasGroup.alpha = toAlpha;
                onComplete?.Invoke();
                yield break;
            }

            float fromAlpha = canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = EaseOutCubic(Mathf.Clamp01(elapsed / duration));
                canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
                yield return null;
            }

            canvasGroup.alpha = toAlpha;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Fade in a canvas group.
        /// </summary>
        public static IEnumerator FadeIn(CanvasGroup canvasGroup, float duration = Normal, Action onComplete = null)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.gameObject.SetActive(true);
            }
            yield return AnimateFade(canvasGroup, 1f, duration, onComplete);
        }

        /// <summary>
        /// Fade out a canvas group.
        /// </summary>
        public static IEnumerator FadeOut(CanvasGroup canvasGroup, float duration = Normal, Action onComplete = null)
        {
            yield return AnimateFade(canvasGroup, 0f, duration, () =>
            {
                if (canvasGroup != null)
                {
                    canvasGroup.gameObject.SetActive(false);
                }
                onComplete?.Invoke();
            });
        }

        #endregion

        #region Slide Animations

        /// <summary>
        /// Slide a RectTransform from one position to another.
        /// </summary>
        public static IEnumerator AnimateSlide(RectTransform target, Vector2 toPosition, float duration, Action onComplete = null)
        {
            if (target == null) yield break;

            duration = GetDuration(duration);

            if (duration <= 0)
            {
                target.anchoredPosition = toPosition;
                onComplete?.Invoke();
                yield break;
            }

            Vector2 fromPosition = target.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = EaseOutCubic(Mathf.Clamp01(elapsed / duration));
                target.anchoredPosition = Vector2.LerpUnclamped(fromPosition, toPosition, t);
                yield return null;
            }

            target.anchoredPosition = toPosition;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Slide in from bottom (for phone overlay).
        /// </summary>
        public static IEnumerator SlideInFromBottom(RectTransform target, float duration = Slow, Action onComplete = null)
        {
            if (target == null) yield break;

            target.gameObject.SetActive(true);
            float height = target.rect.height;
            target.anchoredPosition = new Vector2(target.anchoredPosition.x, -height);

            yield return AnimateSlide(target, new Vector2(target.anchoredPosition.x, 0), duration, onComplete);
        }

        /// <summary>
        /// Slide out to bottom.
        /// </summary>
        public static IEnumerator SlideOutToBottom(RectTransform target, float duration = Slow, Action onComplete = null)
        {
            if (target == null) yield break;

            float height = target.rect.height;

            yield return AnimateSlide(target, new Vector2(target.anchoredPosition.x, -height), duration, () =>
            {
                target.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }

        /// <summary>
        /// Slide in from top (for notifications).
        /// </summary>
        public static IEnumerator SlideInFromTop(RectTransform target, float duration = Normal, Action onComplete = null)
        {
            if (target == null) yield break;

            target.gameObject.SetActive(true);
            float height = target.rect.height;
            target.anchoredPosition = new Vector2(target.anchoredPosition.x, height);

            yield return AnimateSlide(target, new Vector2(target.anchoredPosition.x, 0), duration, onComplete);
        }

        /// <summary>
        /// Slide out to top.
        /// </summary>
        public static IEnumerator SlideOutToTop(RectTransform target, float duration = Normal, Action onComplete = null)
        {
            if (target == null) yield break;

            float height = target.rect.height;

            yield return AnimateSlide(target, new Vector2(target.anchoredPosition.x, height), duration, () =>
            {
                target.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }

        #endregion

        #region Value Animations

        /// <summary>
        /// Animate a float value.
        /// </summary>
        public static IEnumerator AnimateValue(float from, float to, float duration, Action<float> onUpdate, Action onComplete = null)
        {
            duration = GetDuration(duration);

            if (duration <= 0)
            {
                onUpdate?.Invoke(to);
                onComplete?.Invoke();
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = EaseOutCubic(Mathf.Clamp01(elapsed / duration));
                float value = Mathf.Lerp(from, to, t);
                onUpdate?.Invoke(value);
                yield return null;
            }

            onUpdate?.Invoke(to);
            onComplete?.Invoke();
        }

        /// <summary>
        /// Animate an integer value (for counting up/down displays).
        /// </summary>
        public static IEnumerator AnimateIntValue(int from, int to, float duration, Action<int> onUpdate, Action onComplete = null)
        {
            yield return AnimateValue(from, to, duration,
                value => onUpdate?.Invoke(Mathf.RoundToInt(value)),
                onComplete);
        }

        #endregion

        #region Easing Functions

        /// <summary>
        /// Ease out cubic - decelerates.
        /// </summary>
        public static float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        /// <summary>
        /// Ease in cubic - accelerates.
        /// </summary>
        public static float EaseInCubic(float t)
        {
            return t * t * t;
        }

        /// <summary>
        /// Ease in-out cubic - accelerates then decelerates.
        /// </summary>
        public static float EaseInOutCubic(float t)
        {
            return t < 0.5f
                ? 4f * t * t * t
                : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
        }

        /// <summary>
        /// Ease out back - overshoots then settles.
        /// </summary>
        public static float EaseOutBack(float t, float overshoot = 1.70158f)
        {
            float c1 = overshoot;
            float c3 = c1 + 1f;

            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }

        #endregion

        #region Utility

        /// <summary>
        /// Shake animation for error feedback.
        /// </summary>
        public static IEnumerator Shake(Transform target, float intensity = 10f, float duration = 0.3f)
        {
            if (target == null) yield break;

            duration = GetDuration(duration);
            if (duration <= 0) yield break;

            Vector3 originalPosition = target.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = 1f - (elapsed / duration);
                float x = UnityEngine.Random.Range(-intensity, intensity) * t;
                target.localPosition = originalPosition + new Vector3(x, 0, 0);
                yield return null;
            }

            target.localPosition = originalPosition;
        }

        #endregion
    }
}
