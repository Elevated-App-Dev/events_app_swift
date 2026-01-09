using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.UI.Phone.Apps
{
    /// <summary>
    /// Controls the Contacts app view (vendor rolodex).
    /// Displays contacts organized by type with filtering and detail views.
    /// </summary>
    public class ContactsAppController : UIControllerBase, IPhoneAppController
    {
        #region Serialized Fields

        [Header("Content")]
        [SerializeField] private Transform contactsContainer;
        [SerializeField] private GameObject contactItemPrefab;
        [SerializeField] private TextMeshProUGUI noContactsText;

        [Header("Views")]
        [SerializeField] private GameObject contactListView;
        [SerializeField] private GameObject contactDetailView;

        [Header("Filters")]
        [SerializeField] private Button allFilterButton;
        [SerializeField] private Button vendorsFilterButton;
        [SerializeField] private Button venuesFilterButton;
        [SerializeField] private Button favoritesFilterButton;

        [Header("Navigation")]
        [SerializeField] private Button backButton;

        #endregion

        #region State

        private List<GameObject> _contactItems = new List<GameObject>();
        private ContactFilter _currentFilter = ContactFilter.All;

        private enum ContactFilter
        {
            All,
            Vendors,
            Venues,
            Favorites
        }

        #endregion

        #region UIControllerBase Implementation

        protected override void SubscribeToEvents()
        {
            if (backButton != null)
                backButton.onClick.AddListener(ShowContactList);

            if (allFilterButton != null)
                allFilterButton.onClick.AddListener(() => SetFilter(ContactFilter.All));
            if (vendorsFilterButton != null)
                vendorsFilterButton.onClick.AddListener(() => SetFilter(ContactFilter.Vendors));
            if (venuesFilterButton != null)
                venuesFilterButton.onClick.AddListener(() => SetFilter(ContactFilter.Venues));
            if (favoritesFilterButton != null)
                favoritesFilterButton.onClick.AddListener(() => SetFilter(ContactFilter.Favorites));
        }

        protected override void UnsubscribeFromEvents()
        {
            if (backButton != null)
                backButton.onClick.RemoveAllListeners();
            if (allFilterButton != null)
                allFilterButton.onClick.RemoveAllListeners();
            if (vendorsFilterButton != null)
                vendorsFilterButton.onClick.RemoveAllListeners();
            if (venuesFilterButton != null)
                venuesFilterButton.onClick.RemoveAllListeners();
            if (favoritesFilterButton != null)
                favoritesFilterButton.onClick.RemoveAllListeners();
        }

        protected override void RefreshDisplay()
        {
            if (!HasValidGameState()) return;

            UpdateContactsList();
        }

        #endregion

        #region IPhoneAppController

        public void Refresh()
        {
            RefreshDisplay();
        }

        #endregion

        #region Filter

        private void SetFilter(ContactFilter filter)
        {
            _currentFilter = filter;
            UpdateFilterButtons();
            UpdateContactsList();
        }

        private void UpdateFilterButtons()
        {
            SetFilterButtonActive(allFilterButton, _currentFilter == ContactFilter.All);
            SetFilterButtonActive(vendorsFilterButton, _currentFilter == ContactFilter.Vendors);
            SetFilterButtonActive(venuesFilterButton, _currentFilter == ContactFilter.Venues);
            SetFilterButtonActive(favoritesFilterButton, _currentFilter == ContactFilter.Favorites);
        }

        private void SetFilterButtonActive(Button button, bool isActive)
        {
            if (button == null) return;
            var img = button.GetComponent<Image>();
            if (img != null) img.color = isActive ? DesignTokens.Accent : DesignTokens.Surface;
        }

        #endregion

        #region Contact List

        private void ShowContactList()
        {
            if (contactListView != null) contactListView.SetActive(true);
            if (contactDetailView != null) contactDetailView.SetActive(false);

            UpdateContactsList();
        }

        private void UpdateContactsList()
        {
            if (contactsContainer == null) return;

            // Clear existing items
            foreach (var item in _contactItems)
            {
                if (item != null) Destroy(item);
            }
            _contactItems.Clear();

            // Note: Contacts will be added when SaveData includes contact tracking
            // For now, show empty state
            if (noContactsText != null)
            {
                noContactsText.gameObject.SetActive(true);
                noContactsText.text = _currentFilter == ContactFilter.Favorites
                    ? "No favorites yet"
                    : "No contacts";
            }
        }

        #endregion
    }
}
