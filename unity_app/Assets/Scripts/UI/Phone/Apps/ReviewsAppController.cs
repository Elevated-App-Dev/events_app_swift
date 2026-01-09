using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.UI.Phone.Apps
{
    /// <summary>
    /// Controls the Reviews app view.
    /// Displays reputation overview and client reviews/testimonials.
    /// </summary>
    public class ReviewsAppController : UIControllerBase, IPhoneAppController
    {
        #region Serialized Fields

        [Header("Reputation Summary")]
        [SerializeField] private TextMeshProUGUI reputationScoreText;
        [SerializeField] private TextMeshProUGUI totalEventsText;
        [SerializeField] private TextMeshProUGUI averageSatisfactionText;
        [SerializeField] private Image satisfactionBar;

        [Header("Streak")]
        [SerializeField] private GameObject streakSection;
        [SerializeField] private TextMeshProUGUI currentStreakText;
        [SerializeField] private TextMeshProUGUI bestStreakText;

        [Header("Reviews List")]
        [SerializeField] private Transform reviewsContainer;
        [SerializeField] private GameObject reviewItemPrefab;
        [SerializeField] private TextMeshProUGUI noReviewsText;

        #endregion

        #region State

        private List<GameObject> _reviewItems = new List<GameObject>();

        #endregion

        #region UIControllerBase Implementation

        protected override void SubscribeToEvents()
        {
            // Note: Event subscriptions will be added when review events are available
        }

        protected override void UnsubscribeFromEvents()
        {
            // Note: Event unsubscriptions will be added when review events are available
        }

        protected override void RefreshDisplay()
        {
            if (!HasValidGameState()) return;

            UpdateReputationSummary();
            UpdateReviewsList();
        }

        #endregion

        #region IPhoneAppController

        public void Refresh()
        {
            RefreshDisplay();
        }

        #endregion

        #region Reputation Summary

        private void UpdateReputationSummary()
        {
            var player = GameManager?.CurrentPlayer;
            var saveData = GameManager?.CurrentSaveData;

            if (player == null)
            {
                if (reputationScoreText != null) reputationScoreText.text = "0";
                if (totalEventsText != null) totalEventsText.text = "No events completed";
                return;
            }

            // Main reputation score
            if (reputationScoreText != null)
            {
                reputationScoreText.text = player.reputation.ToString();
            }

            // Total events from history
            int totalEvents = saveData?.eventHistory?.Count ?? 0;
            if (totalEventsText != null)
            {
                totalEventsText.text = totalEvents == 1
                    ? "1 event"
                    : $"{totalEvents} events";
            }

            // Average satisfaction - placeholder until we track this
            if (averageSatisfactionText != null)
            {
                averageSatisfactionText.text = "---";
                averageSatisfactionText.color = DesignTokens.TextMuted;
            }

            if (satisfactionBar != null)
            {
                satisfactionBar.fillAmount = 0f;
            }

            // Streak
            if (streakSection != null)
            {
                int streak = saveData?.excellenceStreak ?? 0;
                streakSection.SetActive(streak > 0);
            }

            if (currentStreakText != null)
            {
                currentStreakText.text = $"{saveData?.excellenceStreak ?? 0}";
            }

            if (bestStreakText != null)
            {
                bestStreakText.text = "Best: ---";
            }
        }

        #endregion

        #region Reviews List

        private void UpdateReviewsList()
        {
            if (reviewsContainer == null) return;

            // Clear existing items
            foreach (var item in _reviewItems)
            {
                if (item != null) Destroy(item);
            }
            _reviewItems.Clear();

            // Note: Reviews will be added when SaveData includes review tracking
            // For now, show empty state
            if (noReviewsText != null)
            {
                noReviewsText.gameObject.SetActive(true);
                noReviewsText.text = "No reviews yet";
            }
        }

        #endregion
    }
}
