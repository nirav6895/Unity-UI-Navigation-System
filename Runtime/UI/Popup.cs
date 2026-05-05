#region Using
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace NG.UINavigationSystem
{
    /// <summary>
    /// Base class for creating popup UI elements. 
    /// Popups are a type of UI that appears on top of the current screen and can be used to display additional information or 
    /// options without navigating away from the current screen. 
    /// This class provides common functionality for popups, such as handling transitions and raycast blocking. 
    /// You can create your own popup by inheriting from this class and implementing the necessary functionality.
    /// </summary>
    public abstract class Popup : SNP
    {
        #region Inspector Variables
        [Header("Child References - Popup")]
        /// <Summary> 
        /// Black transparent raycast blocker.
        /// </Summary>
        [SerializeField] protected Image raycastBlocker;

        /// <Summary> 
        /// Content of PopUp which can be animated. This should not include black transparent raycast blocker.
        /// </Summary>
        [SerializeField] protected GameObject content;
        #endregion


        #region Private Variables
        /// <Summary> 
        /// Default Alpha Value of Raycast Blocker
        /// </Summary>
        private float defaultAlphaOfRaycastBlocker;
        #endregion


        #region Properties
        /// <summary>
        /// Transition to be played when showing or hiding this Popup. This is used to play Popup open/close animations.
        /// </summary>
        protected override ITransition Transition
        {
            get
            {
                return new PopupTransition();
            }
        }

        /// <summary>
        /// Transition parameters to be passed to the Transition when showing or hiding this Popup. This is used to pass necessary references and values for the PopupTransition to work properly.
        /// </summary>
        protected override ITransitionParameters TransitionParameters
        {
            get
            {
                return new PopupTransitionParameters()
                {
                    monoBehaviour = this,
                    content = content,
                    raycastBlocker = raycastBlocker,
                    defaultAlphaOfRaycastBlocker = defaultAlphaOfRaycastBlocker
                };
            }
        }
        #endregion


        #region Overridden Methods
        /// <summary>
        /// Initialize Variables
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (raycastBlocker != null)
                defaultAlphaOfRaycastBlocker = raycastBlocker.color.a;
        }
        #endregion
    }
}