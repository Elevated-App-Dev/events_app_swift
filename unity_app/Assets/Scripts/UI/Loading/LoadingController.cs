using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace EventPlannerSim.UI.Loading
{
    /// <summary>
    /// Controls the loading screen.
    /// </summary>
    public class LoadingController : MonoBehaviour
    {
        [Header("Progress")]
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private TextMeshProUGUI progressText;

        [Header("Tips")]
        [SerializeField] private TextMeshProUGUI tipText;
        [SerializeField] private string[] loadingTips;

        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.3f;
        [SerializeField] private float tipRotationInterval = 3f;

        [Header("Visual")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject spinnerObject;

        private float _targetProgress;
        private float _displayedProgress;
        private Coroutine _tipRotationCoroutine;

        private void Awake()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _displayedProgress = 0f;
            _targetProgress = 0f;

            UpdateProgressDisplay();
            ShowRandomTip();

            if (_tipRotationCoroutine != null)
            {
                StopCoroutine(_tipRotationCoroutine);
            }
            _tipRotationCoroutine = StartCoroutine(RotateTips());

            StartCoroutine(FadeIn());
        }

        public void Hide()
        {
            if (_tipRotationCoroutine != null)
            {
                StopCoroutine(_tipRotationCoroutine);
                _tipRotationCoroutine = null;
            }

            StartCoroutine(FadeOut());
        }

        public void SetProgress(float progress)
        {
            _targetProgress = Mathf.Clamp01(progress);
        }

        public void SetLoadingText(string text)
        {
            if (loadingText != null)
            {
                loadingText.text = text;
            }
        }

        private void Update()
        {
            // Smoothly interpolate progress
            if (!Mathf.Approximately(_displayedProgress, _targetProgress))
            {
                _displayedProgress = Mathf.MoveTowards(_displayedProgress, _targetProgress, Time.deltaTime * 2f);
                UpdateProgressDisplay();
            }

            // Rotate spinner if present
            if (spinnerObject != null)
            {
                spinnerObject.transform.Rotate(0, 0, -180f * Time.deltaTime);
            }
        }

        private void UpdateProgressDisplay()
        {
            if (progressBar != null)
            {
                progressBar.value = _displayedProgress;
            }

            if (progressText != null)
            {
                progressText.text = $"{_displayedProgress * 100:F0}%";
            }
        }

        private void ShowRandomTip()
        {
            if (tipText == null || loadingTips == null || loadingTips.Length == 0)
                return;

            int index = Random.Range(0, loadingTips.Length);
            tipText.text = loadingTips[index];
        }

        private IEnumerator RotateTips()
        {
            while (true)
            {
                yield return new WaitForSeconds(tipRotationInterval);
                ShowRandomTip();
            }
        }

        private IEnumerator FadeIn()
        {
            if (canvasGroup == null) yield break;

            canvasGroup.alpha = 0f;
            float elapsed = 0f;

            while (elapsed < fadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime; // Use unscaled for loading screens
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        private IEnumerator FadeOut()
        {
            if (canvasGroup == null)
            {
                gameObject.SetActive(false);
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Wait for async operation and update progress.
        /// </summary>
        public IEnumerator TrackAsyncOperation(AsyncOperation operation)
        {
            while (!operation.isDone)
            {
                SetProgress(operation.progress);
                yield return null;
            }

            SetProgress(1f);
        }
    }
}
