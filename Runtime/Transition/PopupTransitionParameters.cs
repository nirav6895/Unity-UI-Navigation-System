#region Using
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace NG.UINavigationSystem
{
    /// <summary>
    /// Parameters for configuring the popup transition animation.
    /// </summary>
    public class PopupTransitionParameters : ITransitionParameters
    {
        #region Variables
        /// <summary>
        /// MonoBehaviour reference to start coroutines for playing animations. 
        /// This should be a reference to the MonoBehaviour of the screen that is being opened or closed.
        /// </summary>
        public MonoBehaviour monoBehaviour;

        /// <summary>
        /// The content GameObject of the popup screen that will be animated during the transition.
        /// </summary>
        public GameObject content;

        /// <summary>
        /// The Image component of the raycast blocker that will be animated during the transition.
        /// </summary>
        public Image raycastBlocker;

        /// <summary>
        /// The default alpha value of the raycast blocker.
        /// </summary>
        public float defaultAlphaOfRaycastBlocker;
        #endregion
    }
}