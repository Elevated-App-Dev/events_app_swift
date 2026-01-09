using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Data;
using System;

namespace EventPlannerSim.UI.EventPlanning
{
    /// <summary>
    /// Controls the client inquiry panel displayed when a new client contacts the player.
    /// </summary>
    public class ClientInquiryController : UIControllerBase
    {
        [Header("Client Info")]
        [SerializeField] private TextMeshProUGUI clientNameText;
        [SerializeField] private TextMeshProUGUI eventTypeText;
        [SerializeField] private TextMeshProUGUI budgetText;
        [SerializeField] private TextMeshProUGUI guestCountText;
        [SerializeField] private TextMeshProUGUI eventDateText;
        [SerializeField] private TextMeshProUGUI personalityText;
        [SerializeField] private TextMeshProUGUI specialRequirementsText;

        [Header("Expiration")]
        [SerializeField] private TextMeshProUGUI expirationText;

        [Header("Buttons")]
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button declineButton;

        public event Action<ClientInquiry> OnAccepted;
        public event Action<ClientInquiry> OnDeclined;

        private ClientInquiry _currentInquiry;

        protected override void Awake()
        {
            base.Awake();

            if (acceptButton != null)
            {
                acceptButton.onClick.AddListener(HandleAccept);
            }
            if (declineButton != null)
            {
                declineButton.onClick.AddListener(HandleDecline);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (acceptButton != null)
            {
                acceptButton.onClick.RemoveListener(HandleAccept);
            }
            if (declineButton != null)
            {
                declineButton.onClick.RemoveListener(HandleDecline);
            }
        }

        protected override void SubscribeToEvents() { }
        protected override void UnsubscribeFromEvents() { }

        protected override void RefreshDisplay()
        {
            if (_currentInquiry == null) return;

            if (clientNameText != null)
                clientNameText.text = _currentInquiry.clientName;

            if (eventTypeText != null)
                eventTypeText.text = _currentInquiry.eventDisplayName;

            if (budgetText != null)
                budgetText.text = $"Budget: ${_currentInquiry.budget:N0}";

            if (guestCountText != null)
                guestCountText.text = $"{_currentInquiry.guestCount} guests";

            if (eventDateText != null)
                eventDateText.text = $"Event Date: {_currentInquiry.eventDate.ToShortDisplayString()}";

            if (personalityText != null)
                personalityText.text = _currentInquiry.personality.ToString();

            if (expirationText != null)
                expirationText.text = $"Expires in: {ClientInquiry.ExpirationMinutes} minutes";

            if (specialRequirementsText != null)
            {
                specialRequirementsText.text = _currentInquiry.specialRequirements.Count > 0
                    ? string.Join(", ", _currentInquiry.specialRequirements)
                    : "None";
            }
        }

        public void SetInquiry(ClientInquiry inquiry)
        {
            _currentInquiry = inquiry;
            RefreshDisplay();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void HandleAccept()
        {
            if (_currentInquiry != null)
            {
                OnAccepted?.Invoke(_currentInquiry);
            }
            Hide();
        }

        private void HandleDecline()
        {
            if (_currentInquiry != null)
            {
                OnDeclined?.Invoke(_currentInquiry);
            }
            Hide();
        }
    }
}
