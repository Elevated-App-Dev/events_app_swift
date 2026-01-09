using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Data;
using EventPlannerSim.Core;

namespace EventPlannerSim.UI.Phone.Apps
{
    /// <summary>
    /// Controls the Messages app view.
    /// Displays message threads from clients, vendors, and system notifications.
    /// </summary>
    public class MessagesAppController : UIControllerBase, IPhoneAppController
    {
        #region Serialized Fields

        [Header("Content")]
        [SerializeField] private Transform threadsContainer;
        [SerializeField] private GameObject threadItemPrefab;
        [SerializeField] private TextMeshProUGUI noMessagesText;

        [Header("Thread View")]
        [SerializeField] private GameObject threadListView;
        [SerializeField] private GameObject threadDetailView;
        [SerializeField] private TextMeshProUGUI threadTitleText;
        [SerializeField] private Button backButton;

        #endregion

        #region State

        private List<GameObject> _threadItems = new List<GameObject>();

        #endregion

        #region UIControllerBase Implementation

        protected override void SubscribeToEvents()
        {
            if (backButton != null)
            {
                backButton.onClick.AddListener(ShowThreadList);
            }
        }

        protected override void UnsubscribeFromEvents()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
            }
        }

        protected override void RefreshDisplay()
        {
            if (!HasValidGameState()) return;

            UpdateThreadsList();
        }

        #endregion

        #region IPhoneAppController

        public void Refresh()
        {
            RefreshDisplay();
        }

        #endregion

        #region Thread List

        private void ShowThreadList()
        {
            if (threadListView != null) threadListView.SetActive(true);
            if (threadDetailView != null) threadDetailView.SetActive(false);

            UpdateThreadsList();
        }

        private void UpdateThreadsList()
        {
            if (threadsContainer == null) return;

            // Clear existing items
            foreach (var item in _threadItems)
            {
                if (item != null) Destroy(item);
            }
            _threadItems.Clear();

            // Note: Message threads will be added when SaveData includes messaging
            // For now, show empty state
            if (noMessagesText != null)
            {
                noMessagesText.gameObject.SetActive(true);
                noMessagesText.text = "No messages";
            }
        }

        #endregion
    }
}
