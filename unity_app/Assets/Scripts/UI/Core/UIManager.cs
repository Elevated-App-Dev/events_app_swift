using System;
using System.Collections.Generic;
using UnityEngine;
using EventPlannerSim.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.UI.Core
{
    /// <summary>
    /// Central coordinator for UI overlays, panels, and navigation.
    /// Manages overlay stack and screen transitions.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region Singleton

        private static UIManager _instance;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogWarning("[UIManager] Instance is null. Make sure UIManager exists in the scene.");
                }
                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        #endregion

        #region Serialized References

        [Header("Overlays")]
        [SerializeField] private GameObject phoneOverlay;
        [SerializeField] private GameObject mapOverlay;
        [SerializeField] private GameObject tutorialOverlay;
        [SerializeField] private GameObject pauseMenuOverlay;
        [SerializeField] private GameObject settingsOverlay;
        [SerializeField] private GameObject milestoneOverlay;
        [SerializeField] private GameObject loadingOverlay;

        [Header("Panels")]
        [SerializeField] private GameObject clientInquiryPanel;
        [SerializeField] private GameObject budgetAllocationPanel;
        [SerializeField] private GameObject venueSelectionPanel;
        [SerializeField] private GameObject vendorSelectionPanel;
        [SerializeField] private GameObject eventExecutionPanel;
        [SerializeField] private GameObject resultsPanel;

        [Header("Notification")]
        [SerializeField] private Transform notificationContainer;

        [Header("Animation Settings")]
        [SerializeField] private float overlayAnimationDuration = 0.25f;
        [SerializeField] private float panelTransitionDuration = 0.2f;

        #endregion

        #region State

        private Stack<GameObject> _overlayStack = new Stack<GameObject>();
        private GameObject _activePanel;
        private bool _isTransitioning;

        #endregion

        #region Events

        /// <summary>
        /// Fired when an overlay is opened.
        /// </summary>
        public event Action<GameObject> OnOverlayOpened;

        /// <summary>
        /// Fired when an overlay is closed.
        /// </summary>
        public event Action<GameObject> OnOverlayClosed;

        /// <summary>
        /// Fired when a panel transition occurs.
        /// </summary>
        public event Action<GameObject, GameObject> OnPanelTransition;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            // Hide all overlays and panels on start
            HideAllOverlays();
            HideAllPanels();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        #endregion

        #region Overlay Management

        /// <summary>
        /// Open an overlay by reference.
        /// </summary>
        public void OpenOverlay(GameObject overlay)
        {
            if (overlay == null || _isTransitioning) return;

            // Push to stack if not already open
            if (!_overlayStack.Contains(overlay))
            {
                _overlayStack.Push(overlay);
            }

            overlay.SetActive(true);
            OnOverlayOpened?.Invoke(overlay);

            Debug.Log($"[UIManager] Opened overlay: {overlay.name}");
        }

        /// <summary>
        /// Close the top overlay on the stack.
        /// </summary>
        public void CloseTopOverlay()
        {
            if (_overlayStack.Count == 0 || _isTransitioning) return;

            var overlay = _overlayStack.Pop();
            overlay.SetActive(false);
            OnOverlayClosed?.Invoke(overlay);

            Debug.Log($"[UIManager] Closed overlay: {overlay.name}");
        }

        /// <summary>
        /// Close a specific overlay.
        /// </summary>
        public void CloseOverlay(GameObject overlay)
        {
            if (overlay == null) return;

            overlay.SetActive(false);

            // Rebuild stack without this overlay
            var tempList = new List<GameObject>(_overlayStack);
            tempList.Remove(overlay);
            _overlayStack.Clear();
            tempList.Reverse();
            foreach (var item in tempList)
            {
                _overlayStack.Push(item);
            }

            OnOverlayClosed?.Invoke(overlay);
            Debug.Log($"[UIManager] Closed overlay: {overlay.name}");
        }

        /// <summary>
        /// Close all open overlays.
        /// </summary>
        public void CloseAllOverlays()
        {
            while (_overlayStack.Count > 0)
            {
                var overlay = _overlayStack.Pop();
                overlay.SetActive(false);
                OnOverlayClosed?.Invoke(overlay);
            }
        }

        /// <summary>
        /// Check if any overlay is currently open.
        /// </summary>
        public bool HasOpenOverlay => _overlayStack.Count > 0;

        /// <summary>
        /// Hide all overlays without clearing stack state.
        /// </summary>
        private void HideAllOverlays()
        {
            if (phoneOverlay != null) phoneOverlay.SetActive(false);
            if (mapOverlay != null) mapOverlay.SetActive(false);
            if (tutorialOverlay != null) tutorialOverlay.SetActive(false);
            if (pauseMenuOverlay != null) pauseMenuOverlay.SetActive(false);
            if (settingsOverlay != null) settingsOverlay.SetActive(false);
            if (milestoneOverlay != null) milestoneOverlay.SetActive(false);
            if (loadingOverlay != null) loadingOverlay.SetActive(false);
        }

        #endregion

        #region Convenience Overlay Methods

        public void OpenPhone() => OpenOverlay(phoneOverlay);
        public void ClosePhone() => CloseOverlay(phoneOverlay);

        public void OpenMap() => OpenOverlay(mapOverlay);
        public void CloseMap() => CloseOverlay(mapOverlay);

        public void OpenPauseMenu() => OpenOverlay(pauseMenuOverlay);
        public void ClosePauseMenu() => CloseOverlay(pauseMenuOverlay);

        public void OpenSettings() => OpenOverlay(settingsOverlay);
        public void CloseSettings() => CloseOverlay(settingsOverlay);

        public void OpenTutorial() => OpenOverlay(tutorialOverlay);
        public void CloseTutorial() => CloseOverlay(tutorialOverlay);

        public void OpenMilestone() => OpenOverlay(milestoneOverlay);
        public void CloseMilestone() => CloseOverlay(milestoneOverlay);

        public void ShowLoading() => OpenOverlay(loadingOverlay);
        public void HideLoading() => CloseOverlay(loadingOverlay);

        #endregion

        #region Panel Management

        /// <summary>
        /// Show a panel, hiding the currently active one.
        /// </summary>
        public void ShowPanel(GameObject panel)
        {
            if (panel == null || _isTransitioning) return;

            var previousPanel = _activePanel;

            if (_activePanel != null)
            {
                _activePanel.SetActive(false);
            }

            panel.SetActive(true);
            _activePanel = panel;

            OnPanelTransition?.Invoke(previousPanel, panel);
            Debug.Log($"[UIManager] Showing panel: {panel.name}");
        }

        /// <summary>
        /// Hide the currently active panel.
        /// </summary>
        public void HideActivePanel()
        {
            if (_activePanel != null)
            {
                _activePanel.SetActive(false);
                OnPanelTransition?.Invoke(_activePanel, null);
                _activePanel = null;
            }
        }

        /// <summary>
        /// Hide all panels.
        /// </summary>
        private void HideAllPanels()
        {
            if (clientInquiryPanel != null) clientInquiryPanel.SetActive(false);
            if (budgetAllocationPanel != null) budgetAllocationPanel.SetActive(false);
            if (venueSelectionPanel != null) venueSelectionPanel.SetActive(false);
            if (vendorSelectionPanel != null) vendorSelectionPanel.SetActive(false);
            if (eventExecutionPanel != null) eventExecutionPanel.SetActive(false);
            if (resultsPanel != null) resultsPanel.SetActive(false);

            _activePanel = null;
        }

        #endregion

        #region Event Planning Flow

        /// <summary>
        /// Show client inquiry panel with the given inquiry data.
        /// </summary>
        public void ShowClientInquiry(ClientInquiry inquiry)
        {
            // Get controller and set data
            var controller = clientInquiryPanel?.GetComponent<IClientInquiryController>();
            controller?.SetInquiry(inquiry);

            ShowPanel(clientInquiryPanel);
        }

        /// <summary>
        /// Transition from inquiry to budget allocation.
        /// </summary>
        public void TransitionToBudgetAllocation(EventData eventData)
        {
            var controller = budgetAllocationPanel?.GetComponent<IBudgetAllocationController>();
            controller?.SetEvent(eventData);

            ShowPanel(budgetAllocationPanel);
        }

        /// <summary>
        /// Transition from budget to venue selection.
        /// </summary>
        public void TransitionToVenueSelection(EventData eventData)
        {
            var controller = venueSelectionPanel?.GetComponent<IVenueSelectionController>();
            controller?.SetEvent(eventData);

            ShowPanel(venueSelectionPanel);
        }

        /// <summary>
        /// Transition from venue to vendor selection.
        /// </summary>
        public void TransitionToVendorSelection(EventData eventData)
        {
            var controller = vendorSelectionPanel?.GetComponent<IVendorSelectionController>();
            controller?.SetEvent(eventData);

            ShowPanel(vendorSelectionPanel);
        }

        /// <summary>
        /// Show event execution panel.
        /// </summary>
        public void ShowEventExecution(EventData eventData)
        {
            var controller = eventExecutionPanel?.GetComponent<IEventExecutionController>();
            controller?.SetEvent(eventData);

            ShowPanel(eventExecutionPanel);
        }

        /// <summary>
        /// Show results panel with event results.
        /// </summary>
        public void ShowResults(EventResults results)
        {
            var controller = resultsPanel?.GetComponent<IResultsController>();
            controller?.SetResults(results);

            ShowPanel(resultsPanel);
        }

        /// <summary>
        /// Return to main gameplay view.
        /// </summary>
        public void ReturnToGameplay()
        {
            HideActivePanel();
            CloseAllOverlays();
        }

        #endregion

        #region Notifications

        /// <summary>
        /// Show a notification popup.
        /// </summary>
        public void ShowNotification(string title, string message, NotificationType type = NotificationType.NewInquiry)
        {
            // This will be implemented with the NotificationController
            Debug.Log($"[UIManager] Notification: {title} - {message}");
        }

        #endregion
    }

    #region Controller Interfaces

    /// <summary>
    /// Interface for client inquiry panel controller.
    /// </summary>
    public interface IClientInquiryController
    {
        void SetInquiry(ClientInquiry inquiry);
    }

    /// <summary>
    /// Interface for budget allocation panel controller.
    /// </summary>
    public interface IBudgetAllocationController
    {
        void SetEvent(EventData eventData);
    }

    /// <summary>
    /// Interface for venue selection panel controller.
    /// </summary>
    public interface IVenueSelectionController
    {
        void SetEvent(EventData eventData);
    }

    /// <summary>
    /// Interface for vendor selection panel controller.
    /// </summary>
    public interface IVendorSelectionController
    {
        void SetEvent(EventData eventData);
    }

    /// <summary>
    /// Interface for event execution panel controller.
    /// </summary>
    public interface IEventExecutionController
    {
        void SetEvent(EventData eventData);
    }

    /// <summary>
    /// Interface for results panel controller.
    /// </summary>
    public interface IResultsController
    {
        void SetResults(EventResults results);
    }

    #endregion
}
