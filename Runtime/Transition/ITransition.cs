#region Using
using System;
#endregion

namespace NG.UINavigationSystem
{
    /// <summary>
    /// Defines the interface for a transition effect.
    /// </summary>
    public interface ITransition
    {
        #region Methods
        /// <summary>
        /// Plays the open animation for the specified transition parameters.
        /// </summary>
        /// <param name="transitionParameters">The transition parameters.</param>
        void PlayOpenAnimation(ITransitionParameters transitionParameters);

        /// <summary>
        /// Plays the close animation for the specified transition parameters.
        /// </summary>
        /// <param name="transitionParameters">The transition parameters.</param>
        /// <param name="callback">The callback to invoke when the animation completes.</param>
        void PlayCloseAnimation(ITransitionParameters transitionParameters, Action callback = null);
        #endregion
    }
}