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
    /// Controls the Clients app view.
    /// Displays client CRM with history and notes.
    /// </summary>
    public class ClientsAppController : UIControllerBase, IPhoneAppController
    {
        #region Serialized Fields

        [Header("Summary")]
        [SerializeField] private TextMeshProUGUI totalClientsText;
        [SerializeField] private TextMeshProUGUI repeatClientsText;
        [SerializeField] private TextMeshProUGUI referralSourcesText;

        [Header("Content")]
        [SerializeField] private Transform clientsContainer;
        [SerializeField] private GameObject clientItemPrefab;
        [SerializeField] private TextMeshProUGUI noClientsText;

        #endregion

        #region State

        private List<GameObject> _clientItems = new List<GameObject>();

        #endregion

        #region UIControllerBase Implementation

        protected override void SubscribeToEvents()
        {
            // Note: Event subscriptions will be added when client tracking events are available
        }

        protected override void UnsubscribeFromEvents()
        {
            // Note: Event unsubscriptions will be added when client tracking events are available
        }

        protected override void RefreshDisplay()
        {
            if (!HasValidGameState()) return;

            UpdateSummary();
            UpdateClientsList();
        }

        #endregion

        #region IPhoneAppController

        public void Refresh()
        {
            RefreshDisplay();
        }

        #endregion

        #region Summary

        private void UpdateSummary()
        {
            // Note: Client records will be added when SaveData includes client CRM
            // For now, show zeros
            if (totalClientsText != null) totalClientsText.text = "0";
            if (repeatClientsText != null) repeatClientsText.text = "0";
            if (referralSourcesText != null) referralSourcesText.text = "0";
        }

        #endregion

        #region Client List

        private void UpdateClientsList()
        {
            if (clientsContainer == null) return;

            // Clear existing items
            foreach (var item in _clientItems)
            {
                if (item != null) Destroy(item);
            }
            _clientItems.Clear();

            // Note: Client records will be added when SaveData includes client CRM
            // For now, show empty state
            if (noClientsText != null)
            {
                noClientsText.gameObject.SetActive(true);
                noClientsText.text = "No clients yet";
            }
        }

        #endregion
    }
}
