#region Using
using System;
#endregion

namespace NG.UINavigationSystem
{
    /// <summary>
    /// Default Transition that does not play any animation when opening and closing a screen.
    /// </summary>
    public class DefaultTransition : ITransition
    {
        /// <summary>
        /// Plays the open animation for the specified transition parameters.
        /// </summary>
        /// <param name="transitionParameters">The transition parameters.</param>
        public void PlayOpenAnimation(ITransitionParameters transitionParameters)
        {
        }

        /// <summary>
        /// Plays the close animation for the specified transition parameters.
        /// </summary>
        /// <param name="transitionParameters">The transition parameters.</param>
        /// <param name="callback">The callback to invoke when the animation completes.</param>
        public void PlayCloseAnimation(ITransitionParameters transitionParameters, Action callback = null)
        {
            callback?.Invoke();
        }
    }
}
