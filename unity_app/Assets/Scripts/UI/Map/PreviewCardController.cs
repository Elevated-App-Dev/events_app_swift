using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.Data;
using EventPlannerSim.Core;
using System;

namespace EventPlannerSim.UI.Map
{
    /// <summary>
    /// Controls the location preview card shown when a pin is tapped.
    /// </summary>
    public class PreviewCardController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI attributesText;
        [SerializeField] private Button visitButton;
        [SerializeField] private Button closeButton;

        public event Action<LocationData> OnVisitRequested;
        public event Action OnCloseRequested;

        private LocationData _currentLocation;

        private void Awake()
        {
            if (visitButton != null)
            {
                visitButton.onClick.AddListener(HandleVisit);
            }
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(HandleClose);
            }
        }

        private void OnDestroy()
        {
            if (visitButton != null)
            {
                visitButton.onClick.RemoveListener(HandleVisit);
            }
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(HandleClose);
            }
        }

        public void SetData(LocationPreviewData preview)
        {
            _currentLocation = new LocationData
            {
                locationId = preview.locationId,
                displayName = preview.displayName,
                locationType = preview.locationType
            };

            if (nameText != null)
            {
                nameText.text = preview.displayName;
            }

            if (descriptionText != null)
            {
                descriptionText.text = preview.description;
            }

            if (attributesText != null)
            {
                attributesText.text = FormatAttributes(preview);
            }
        }

        private string FormatAttributes(LocationPreviewData preview)
        {
            return preview.locationType switch
            {
                LocationType.Venue => $"Capacity: {preview.capacity} | ${preview.pricePerEvent:N0}",
                LocationType.Vendor => $"Quality: {preview.rating:F1} | ${preview.vendorBasePrice:N0}",
                _ => ""
            };
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void HandleVisit()
        {
            OnVisitRequested?.Invoke(_currentLocation);
        }

        private void HandleClose()
        {
            OnCloseRequested?.Invoke();
            Hide();
        }
    }
}
