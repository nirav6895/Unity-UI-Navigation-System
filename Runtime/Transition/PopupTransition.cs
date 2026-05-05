#region Using
using System;
using System.Collections;
using UnityEngine;
#endregion

namespace NG.UINavigationSystem
{
    /// <summary>
    /// Represents a transition effect for popup screens.
    /// </summary>
    public class PopupTransition : ITransition
    {
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

            // Stop All Previous Animation
            popupTransitionParameters.monoBehaviour.StopAllCoroutines();

            // Show PopUp Animation of Content by scaling
            if (popupTransitionParameters.content != null)
            {
                popupTransitionParameters.monoBehaviour.StartCoroutine(TweenRoutine(Configurations.Popup.OpenCloseAnimationTime, (value) =>
                {
                    popupTransitionParameters.content.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, value);
                }));
            }

            // Show Fadeout Animation of Black Transparent Raycast Blocker
            if (popupTransitionParameters.raycastBlocker != null)
            {
                popupTransitionParameters.monoBehaviour.StartCoroutine(TweenRoutine(Configurations.Popup.RaycastBlockerFadeTime, (value) =>
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

            // Stop All Previous Animation
            popupTransitionParameters.monoBehaviour.StopAllCoroutines();

            // Show PopUp Animation of Content by scaling
            if (popupTransitionParameters.content != null)
            {
                popupTransitionParameters.monoBehaviour.StartCoroutine(TweenRoutine(Configurations.Popup.OpenCloseAnimationTime,
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

            // Show Fadeout Animation of Black Transparent Raycast Blocker
            if (popupTransitionParameters.raycastBlocker != null)
            {
                popupTransitionParameters.monoBehaviour.StartCoroutine(TweenRoutine(Configurations.Popup.RaycastBlockerFadeTime, (value) =>
                {
                    Color tempColor = popupTransitionParameters.raycastBlocker.color;
                    tempColor.a = Mathf.Lerp(popupTransitionParameters.defaultAlphaOfRaycastBlocker, 0, value);
                    popupTransitionParameters.raycastBlocker.color = tempColor;
                }));
            }
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