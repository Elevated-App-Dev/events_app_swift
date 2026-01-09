using System;
using UnityEngine;
using UnityEngine.UI;
using EventPlannerSim.UI.Core;
using EventPlannerSim.Data;

namespace EventPlannerSim.UI.Phone
{
    /// <summary>
    /// Controls the phone overlay and app navigation.
    /// Slides up from bottom, displays app grid, manages app content area.
    /// </summary>
    public class PhoneOverlayController : UIControllerBase
    {
        #region Serialized Fields

        [Header("Structure")]
        [SerializeField] private RectTransform phoneContainer;
        [SerializeField] private GameObject homeScreen;
        [SerializeField] private GameObject appContentArea;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Navigation")]
        [SerializeField] private Button backButton;
        [SerializeField] private Button closeButton;

        [Header("App Icons")]
        [SerializeField] private AppIconController calendarIcon;
        [SerializeField] private AppIconController messagesIcon;
        [SerializeField] private AppIconController bankIcon;
        [SerializeField] private AppIconController contactsIcon;
        [SerializeField] private AppIconController reviewsIcon;
        [SerializeField] private AppIconController tasksIcon;
        [SerializeField] private AppIconController clientsIcon;

        [Header("App Views")]
        [SerializeField] private GameObject calendarAppView;
        [SerializeField] private GameObject messagesAppView;
        [SerializeField] private GameObject bankAppView;
        [SerializeField] private GameObject contactsAppView;
        [SerializeField] private GameObject reviewsAppView;
        [SerializeField] private GameObject tasksAppView;
        [SerializeField] private GameObject clientsAppView;

        [Header("Animation")]
        [SerializeField] private float slideAnimationDuration = 0.25f;

        #endregion

        #region State

        private PhoneApp? _currentApp;
        private bool _isAnimating;

        #endregion

        #region Events

        /// <summary>
        /// Fired when an app is opened.
        /// </summary>
        public event Action<PhoneApp> OnAppOpened;

        /// <summary>
        /// Fired when returning to home screen.
        /// </summary>
        public event Action OnReturnedToHome;

        #endregion

        #region Lifecycle

        protected override void Awake()
        {
            base.Awake();

            // Set up button listeners
            if (backButton != null) backButton.onClick.AddListener(OnBackPressed);
            if (closeButton != null) closeButton.onClick.AddListener(OnClosePressed);

            // Set up app icon listeners
            SetupAppIcons();

            // Start hidden
            gameObject.SetActive(false);
        }

        private void SetupAppIcons()
        {
            if (calendarIcon != null) calendarIcon.OnTapped += () => OpenApp(PhoneApp.Calendar);
            if (messagesIcon != null) messagesIcon.OnTapped += () => OpenApp(PhoneApp.Messages);
            if (bankIcon != null) bankIcon.OnTapped += () => OpenApp(PhoneApp.Bank);
            if (contactsIcon != null) contactsIcon.OnTapped += () => OpenApp(PhoneApp.Contacts);
            if (reviewsIcon != null) reviewsIcon.OnTapped += () => OpenApp(PhoneApp.Reviews);
            if (tasksIcon != null) tasksIcon.OnTapped += () => OpenApp(PhoneApp.Tasks);
            if (clientsIcon != null) clientsIcon.OnTapped += () => OpenApp(PhoneApp.Clients);
        }

        #endregion

        #region UIControllerBase Implementation

        protected override void SubscribeToEvents()
        {
            // Note: Phone system badge updates will be added when IGameContext exposes PhoneSystem
        }

        protected override void UnsubscribeFromEvents()
        {
            // Note: Phone system event unsubscriptions will be added when IGameContext exposes PhoneSystem
        }

        protected override void RefreshDisplay()
        {
            RefreshBadges();
        }

        #endregion

        #region Show/Hide

        /// <summary>
        /// Show the phone overlay with animation.
        /// </summary>
        public void Show()
        {
            if (_isAnimating) return;

            gameObject.SetActive(true);
            ShowHomeScreen();
            RefreshBadges();

            // Animate slide from bottom
            if (phoneContainer != null)
            {
                _isAnimating = true;
                StartCoroutine(UIAnimations.SlideInFromBottom(phoneContainer, slideAnimationDuration, () =>
                {
                    _isAnimating = false;
                }));
            }
        }

        /// <summary>
        /// Hide the phone overlay with animation.
        /// </summary>
        public void Hide()
        {
            if (_isAnimating) return;

            if (phoneContainer != null)
            {
                _isAnimating = true;
                StartCoroutine(UIAnimations.SlideOutToBottom(phoneContainer, slideAnimationDuration, () =>
                {
                    _isAnimating = false;
                    gameObject.SetActive(false);
                }));
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        #endregion

        #region Navigation

        /// <summary>
        /// Show the home screen with app grid.
        /// </summary>
        private void ShowHomeScreen()
        {
            _currentApp = null;

            if (homeScreen != null) homeScreen.SetActive(true);
            if (appContentArea != null) appContentArea.SetActive(false);
            if (backButton != null) backButton.gameObject.SetActive(false);

            HideAllApps();
            OnReturnedToHome?.Invoke();
        }

        /// <summary>
        /// Open a specific app.
        /// </summary>
        public void OpenApp(PhoneApp app)
        {
            if (_isAnimating) return;

            _currentApp = app;

            if (homeScreen != null) homeScreen.SetActive(false);
            if (appContentArea != null) appContentArea.SetActive(true);
            if (backButton != null) backButton.gameObject.SetActive(true);

            HideAllApps();
            ShowAppView(app);

            OnAppOpened?.Invoke(app);
            Log($"Opened app: {app}");
        }

        /// <summary>
        /// Show the view for a specific app.
        /// </summary>
        private void ShowAppView(PhoneApp app)
        {
            var view = GetAppView(app);
            if (view != null)
            {
                view.SetActive(true);

                // Refresh the app controller
                var controller = view.GetComponent<IPhoneAppController>();
                controller?.Refresh();
            }
        }

        /// <summary>
        /// Get the GameObject for an app view.
        /// </summary>
        private GameObject GetAppView(PhoneApp app)
        {
            return app switch
            {
                PhoneApp.Calendar => calendarAppView,
                PhoneApp.Messages => messagesAppView,
                PhoneApp.Bank => bankAppView,
                PhoneApp.Contacts => contactsAppView,
                PhoneApp.Reviews => reviewsAppView,
                PhoneApp.Tasks => tasksAppView,
                PhoneApp.Clients => clientsAppView,
                _ => null
            };
        }

        /// <summary>
        /// Hide all app views.
        /// </summary>
        private void HideAllApps()
        {
            if (calendarAppView != null) calendarAppView.SetActive(false);
            if (messagesAppView != null) messagesAppView.SetActive(false);
            if (bankAppView != null) bankAppView.SetActive(false);
            if (contactsAppView != null) contactsAppView.SetActive(false);
            if (reviewsAppView != null) reviewsAppView.SetActive(false);
            if (tasksAppView != null) tasksAppView.SetActive(false);
            if (clientsAppView != null) clientsAppView.SetActive(false);
        }

        #endregion

        #region Badges

        /// <summary>
        /// Refresh all app icon badges.
        /// Note: Badge counts will be fetched when IGameContext exposes PhoneSystem.
        /// </summary>
        private void RefreshBadges()
        {
            // Clear all badges for now - will be updated when PhoneSystem is available
            SetBadge(calendarIcon, 0);
            SetBadge(messagesIcon, 0);
            SetBadge(bankIcon, 0);
            SetBadge(contactsIcon, 0);
            SetBadge(reviewsIcon, 0);
            SetBadge(tasksIcon, 0);
            SetBadge(clientsIcon, 0);
        }

        private void SetBadge(AppIconController icon, int count)
        {
            if (icon != null)
            {
                icon.SetBadgeCount(count);
            }
        }

        #endregion

        #region Button Handlers

        private void OnBackPressed()
        {
            if (_currentApp.HasValue)
            {
                ShowHomeScreen();
            }
        }

        private void OnClosePressed()
        {
            Hide();
            UIManager.Instance?.ClosePhone();
        }

        #endregion
    }

    #region Enums

    /// <summary>
    /// Phone app types.
    /// </summary>
    public enum PhoneApp
    {
        Calendar,
        Messages,
        Bank,
        Contacts,
        Reviews,
        Tasks,
        Clients
    }

    #endregion

    #region Interfaces

    /// <summary>
    /// Interface for phone app controllers.
    /// </summary>
    public interface IPhoneAppController
    {
        void Refresh();
    }

    #endregion
}
