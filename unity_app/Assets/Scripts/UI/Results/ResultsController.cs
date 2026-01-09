using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Data;
using System;
using System.Collections;

namespace EventPlannerSim.UI.Results
{
    /// <summary>
    /// Controls the results screen shown after event completion.
    /// </summary>
    public class ResultsController : UIControllerBase
    {
        [Header("Satisfaction Score")]
        [SerializeField] private TextMeshProUGUI satisfactionScoreText;
        [SerializeField] private TextMeshProUGUI satisfactionLabelText;

        [Header("Financial")]
        [SerializeField] private TextMeshProUGUI profitText;
        [SerializeField] private TextMeshProUGUI revenueText;
        [SerializeField] private TextMeshProUGUI expensesText;

        [Header("Reputation")]
        [SerializeField] private TextMeshProUGUI reputationChangeText;

        [Header("Client Feedback")]
        [SerializeField] private TextMeshProUGUI feedbackText;

        [Header("Referral")]
        [SerializeField] private GameObject referralPanel;
        [SerializeField] private TextMeshProUGUI referralText;

        [Header("Animation")]
        [SerializeField] private float scoreAnimationDuration = 1.5f;
        [SerializeField] private float delayBetweenElements = 0.3f;

        [Header("Buttons")]
        [SerializeField] private Button continueButton;

        public event Action OnContinue;

        private EventResults _currentResult;

        protected override void Awake()
        {
            base.Awake();

            if (continueButton != null)
            {
                continueButton.onClick.AddListener(HandleContinue);
            }
        }

        protected override void SubscribeToEvents() { }
        protected override void UnsubscribeFromEvents() { }

        protected override void RefreshDisplay()
        {
            // Initial display handled by SetResult with animation
        }

        public void SetResult(EventResults result)
        {
            _currentResult = result;

            // Hide referral panel initially
            if (referralPanel != null)
            {
                referralPanel.SetActive(false);
            }

            // Start animated reveal
            StartCoroutine(AnimatedReveal());
        }

        private IEnumerator AnimatedReveal()
        {
            // Disable continue button during animation
            if (continueButton != null)
            {
                continueButton.interactable = false;
            }

            // Animate satisfaction score from 0
            yield return StartCoroutine(AnimateSatisfactionScore());

            yield return new WaitForSeconds(delayBetweenElements);

            // Show profit/loss
            ShowFinancials();

            yield return new WaitForSeconds(delayBetweenElements);

            // Show reputation change
            ShowReputationChange();

            yield return new WaitForSeconds(delayBetweenElements);

            // Show feedback
            ShowFeedback();

            yield return new WaitForSeconds(delayBetweenElements);

            // Show referral if triggered
            if (_currentResult.triggeredReferral)
            {
                ShowReferral();
            }

            // Enable continue button
            if (continueButton != null)
            {
                continueButton.interactable = true;
            }
        }

        private IEnumerator AnimateSatisfactionScore()
        {
            if (satisfactionScoreText == null) yield break;

            float elapsed = 0f;
            int targetScore = Mathf.RoundToInt(_currentResult.finalSatisfaction);

            while (elapsed < scoreAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / scoreAnimationDuration;
                int currentScore = Mathf.RoundToInt(Mathf.Lerp(0, targetScore, t));

                satisfactionScoreText.text = $"{currentScore}%";
                satisfactionScoreText.color = GetScoreColor(currentScore);

                yield return null;
            }

            satisfactionScoreText.text = $"{targetScore}%";
            satisfactionScoreText.color = GetScoreColor(targetScore);
        }

        private Color GetScoreColor(int score)
        {
            if (score >= 80) return DesignTokens.Success;
            if (score >= 60) return DesignTokens.Warning;
            return DesignTokens.Error;
        }

        private void ShowFinancials()
        {
            // EventResults stores profit directly
            float profit = _currentResult.profit;

            // Hide revenue/expenses since EventResults only has profit
            if (revenueText != null)
            {
                revenueText.gameObject.SetActive(false);
            }

            if (expensesText != null)
            {
                expensesText.gameObject.SetActive(false);
            }

            if (profitText != null)
            {
                profitText.text = profit >= 0
                    ? $"Profit: +${profit:N0}"
                    : $"Loss: -${Mathf.Abs(profit):N0}";
                profitText.color = profit >= 0 ? DesignTokens.Success : DesignTokens.Error;
            }
        }

        private void ShowReputationChange()
        {
            if (reputationChangeText == null) return;

            int change = _currentResult.reputationChange;
            reputationChangeText.text = change >= 0
                ? $"Reputation: +{change}"
                : $"Reputation: {change}";
            reputationChangeText.color = change >= 0 ? DesignTokens.Reputation : DesignTokens.Error;
        }

        private void ShowFeedback()
        {
            if (feedbackText == null) return;

            feedbackText.text = $"\"{_currentResult.clientFeedback}\"";
        }

        private void ShowReferral()
        {
            if (referralPanel != null)
            {
                referralPanel.SetActive(true);
            }

            if (referralText != null)
            {
                referralText.text = "Client loved the event so much they referred you to a friend!";
            }
        }

        private void HandleContinue()
        {
            OnContinue?.Invoke();
            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
