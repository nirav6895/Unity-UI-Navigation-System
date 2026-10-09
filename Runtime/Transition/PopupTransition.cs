#region Using
using System;
using System.Collections;
using UnityEngine;
#endregion

namespace NG.UINavigationSystem
{
    /// <summary>
    /// Represents a transition effect for popup screens.
    /// Use one instance per popup for both open and close animations, as it stops its own previous animation when a new one starts.
    /// </summary>
    public class PopupTransition : ITransition
    {
        #region Variables
        /// <summary>
        /// Running animation of the content.
        /// Only the animations of this transition are stopped, so other coroutines of the popup keep running.
        /// </summary>
        private Coroutine contentRoutine;

        /// <summary>
        /// Running animation of the raycast blocker.
        /// </summary>
        private Coroutine raycastBlockerRoutine;
        #endregion


        #region Methods
        /// <Summary> 
        /// Plays PopUp Open Animation
        /// </Summary>
        public void PlayOpenAnimation(ITransitionParameters transitionParameters)
        {
            if (transitionParameters is not PopupTransitionParameters popupTransitionParameters || popupTransitionParameters.monoBehaviour == null)
            {
                return;
            }

            // Stop Previous Animation
            StopAnimations(popupTransitionParameters.monoBehaviour);

            // Show PopUp Animation of Content by scaling
            if (popupTransitionParameters.content != null)
            {
                contentRoutine = popupTransitionParameters.monoBehaviour.StartCoroutine(TweenRoutine(Configurations.Popup.OpenCloseAnimationTime, (value) =>
                {
                    popupTransitionParameters.content.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, value);
                }));
            }

            // Show Fadeout Animation of Black Transparent Raycast Blocker
            if (popupTransitionParameters.raycastBlocker != null)
            {
                raycastBlockerRoutine = popupTransitionParameters.monoBehaviour.StartCoroutine(TweenRoutine(Configurations.Popup.RaycastBlockerFadeTime, (value) =>
                {
                    Color tempColor = popupTransitionParameters.raycastBlocker.color;
                    tempColor.a = Mathf.Lerp(0, popupTransitionParameters.defaultAlphaOfRaycastBlocker, value);
                    popupTransitionParameters.raycastBlocker.color = tempColor;
                }));
            }
        }

        /// <Summary> 
        /// Plays PopUp Close Animation
        /// </Summary>
        public void PlayCloseAnimation(ITransitionParameters transitionParameters, Action callback)
        {

            if (transitionParameters is not PopupTransitionParameters popupTransitionParameters || popupTransitionParameters.monoBehaviour == null)
            {
                callback?.Invoke();
                return;
            }

            // Stop Previous Animation
            StopAnimations(popupTransitionParameters.monoBehaviour);

            // Show Fadeout Animation of Black Transparent Raycast Blocker
            // It's started before the content animation, as callback can deactivate the popup and coroutine can't be started after that.
            if (popupTransitionParameters.raycastBlocker != null)
            {
                raycastBlockerRoutine = popupTransitionParameters.monoBehaviour.StartCoroutine(TweenRoutine(Configurations.Popup.RaycastBlockerFadeTime, (value) =>
                {
                    Color tempColor = popupTransitionParameters.raycastBlocker.color;
                    tempColor.a = Mathf.Lerp(popupTransitionParameters.defaultAlphaOfRaycastBlocker, 0, value);
                    popupTransitionParameters.raycastBlocker.color = tempColor;
                }));
            }

            // Show PopUp Animation of Content by scaling
            if (popupTransitionParameters.content != null)
            {
                contentRoutine = popupTransitionParameters.monoBehaviour.StartCoroutine(TweenRoutine(Configurations.Popup.OpenCloseAnimationTime,
                    onUpdate: (value) =>
                    {
                        popupTransitionParameters.content.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, value);
                    },
                    onComplete: () =>
                    {
                        popupTransitionParameters.content.transform.localScale = Vector3.one;
                        popupTransitionParameters.monoBehaviour.gameObject.SetActive(false);
                        callback?.Invoke();
                    }));
            }
            else
            {
                callback?.Invoke();
            }
        }

        /// <summary>
        /// Stops the running animations of this transition.
        /// </summary>
        /// <param name="monoBehaviour">MonoBehaviour on which the animations were started.</param>
        private void StopAnimations(MonoBehaviour monoBehaviour)
        {
            if (contentRoutine != null)
                monoBehaviour.StopCoroutine(contentRoutine);

            if (raycastBlockerRoutine != null)
                monoBehaviour.StopCoroutine(raycastBlockerRoutine);

            contentRoutine = null;
            raycastBlockerRoutine = null;
        }

        /// <summary>
        /// A helper method to perform tweening over a specified duration, invoking update and completion callbacks.
        /// </summary>
        /// <param name="duration">The duration of the tween.</param>
        /// <param name="onUpdate">The callback to invoke during the tween.</param>
        /// <param name="onComplete">The callback to invoke when the tween completes.</param>
        /// <returns></returns>
        private IEnumerator TweenRoutine(float duration, Action<float> onUpdate = null, Action onComplete = null)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                float smoothedT = Mathf.SmoothStep(0f, 1f, t);

                onUpdate?.Invoke(smoothedT);
                yield return null;
            }

            onUpdate?.Invoke(1f);
            onComplete?.Invoke();
        }
        #endregion
    }
}