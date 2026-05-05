#region Using
using System;
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace NG.UINavigationSystem
{
    /// <summary>
    /// Base class for all UI elements (like Screen, Popup, Notification) in the navigation system. 
    /// This class provides common functionality for showing, hiding, and transitioning UI elements, as well as handling back button behavior. 
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public abstract class UI : MonoBehaviour
    {
        #region Inspector Variables
        [Header("UI Variables")]
        /// <Summary> 
        /// Back or Close Button
        /// </Summary>
        [SerializeField] protected Button closeButton;

        /// <summary>
        /// Gets or sets a value indicating whether the back key is allowed.
        /// </summary>
        public bool AllowBackKey = true;
        #endregion


        #region Public Variables
        /// <summary>
        /// Events triggered when the UI is shown. 
        /// </summary>
        public event Action OnShown;

        /// <summary>
        /// Events triggered when the UI is hidden. 
        /// </summary>
        public event Action OnHidden;
        #endregion


        #region Properties
        /// <summary>
        /// Canvas component of this UI. This is used to control the sorting order when showing the UI.
        /// </summary>
        public Canvas Canvas { get; private set; }

        /// <summary>
        /// Transition to be played when showing or hiding this UI. This is used to play open/close animations.
        /// </summary>
        /// <value></value>
        protected virtual ITransition Transition
        {
            get
            {
                return new DefaultTransition();
            }
        }

        /// <summary>
        /// Parameters for the transition animations. This is used to pass necessary references and values to the transition when playing open/close animations.
        /// </summary>
        protected virtual ITransitionParameters TransitionParameters
        {
            get
            {
                return null;
            }
        }
        #endregion


        #region Unity Methods
        /// <summary>
        /// Initializes Variables
        /// </summary>
        protected virtual void Awake()
        {
            Canvas = GetComponent<Canvas>();
        }

        /// <summary>
        /// Add Listeners on Enable.
        /// </summary>
        protected virtual void OnEnable()
        {
            AddListeners();
        }

        /// <summary>
        /// Remove Listeners on Disable.
        /// </summary>
        protected virtual void OnDisable()
        {
            RemoveListeners();
        }
        #endregion


        #region Virtual & Abstract Methods
        /// <summary>
        /// Add Listeners
        /// </summary>
        protected virtual void AddListeners()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(OnClickCloseButton);
        }

        /// <summary>
        /// Remove Listeners
        /// </summary>
        protected virtual void RemoveListeners()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveListener(OnClickCloseButton);
        }

        /// <summary>
        /// Set parameters for this UI. This method can be overridden by derived classes to accept specific parameters when showing the UI.
        /// </summary>
        /// <param name="parameters">UI Parameters to pass data when showing the UI</param>
        public virtual void SetParameters(IUIParameters parameters = null)
        {
        }

        /// <summary>
        /// Defines the behavior when the close button is clicked. 
        /// </summary>
        protected abstract void OnClickCloseButton();

        /// <summary>
        /// Defines the behavior when the device back key is pressed. 
        /// </summary>
        protected abstract void OnPressedDeviceBackKey();
        #endregion


        #region Internal Methods
        /// <summary>
        /// Shows this UI with the specified parameters and sorting order. This method is called by the navigation manager when showing this UI.
        /// It activates the GameObject, sets the sorting order of the Canvas if specified, applies the parameters, 
        /// plays the open animation, and invokes the OnShown event.
        /// </summary>
        /// <param name="parameters">UI Parameters to pass data when showing the UI</param>
        /// <param name="sortingOrder">Sorting order for this UI. If not specified, it will use the default sorting order defined in Configurations.</param>
        internal void Show(IUIParameters parameters = null, int sortingOrder = -1)
        {
            gameObject.SetActive(true);
            if (sortingOrder >= 0)
            {
                Canvas.overrideSorting = true;
                Canvas.sortingOrder = sortingOrder;
            }
            else
            {
                Canvas.overrideSorting = false;
            }
            SetParameters(parameters);
            Transition?.PlayOpenAnimation(TransitionParameters);
            OnShown?.Invoke();
        }

        /// <summary>
        /// Hides this UI. This method is called by the navigation manager when hiding this UI. 
        /// It plays the close animation and deactivates the GameObject after the animation is completed,
        /// </summary>
        internal void Hide()
        {
            Transition?.PlayCloseAnimation(TransitionParameters, () =>
            {
                gameObject.SetActive(false);
            });
            OnHidden?.Invoke();
        }

        /// <summary>
        /// Calls the OnPressedDeviceBackKey method. 
        /// This is called by the navigation manager when the device back key is pressed to trigger the back key behavior of this UI.
        /// </summary>
        internal void CallOnPressedDeviceBackKey()
        {
            OnPressedDeviceBackKey();
        }
        #endregion
    }
}