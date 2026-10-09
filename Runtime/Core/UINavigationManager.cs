#region Using
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using Logger = NG.UINavigationSystem.Utilities.Logger;
#endregion

namespace NG.UINavigationSystem
{
    /// <summary>
    /// Manages the navigation of UIs in the game. It uses a stack to manage the showing and hiding of UIs.
    /// </summary>
    /// <typeparam name="TCore">The type of UI to manage. Suggest you to use <see cref="SNP"/> for Screens & Popups and <see cref="Notification"/> for Notifications</typeparam>
    public class UINavigationManager<TCore> where TCore : UI
    {
        #region Variables
        /// <summary>
        /// Private instance of UINavigationManager. It will be created on first access. 
        /// </summary>
        private static UINavigationManager<TCore> instance;

        /// <summary>
        /// Stack of all UIs. UIs could be SNP, Notification, Screen or Pop.
        /// </summary>
        protected readonly Stack<TCore> uiStack;

        /// <summary>
        /// Dictionary of all UIs. It is used to store the reference of all UIs in the game. 
        /// It will help to avoid multiple instantiation of the same UI and also provide easy access to any UI in the game.
        /// </summary>
        protected readonly Dictionary<Type, TCore> allUIs;

        /// <summary>
        /// Event triggered when current showing UI is changed. It will pass the current showing UI as parameter.
        /// </summary>
        public static event Action<TCore> OnChangeCurShowingUI;

        /// <summary>
        /// Current Sorting Order. It will be updated on showing or hiding UI. It will help to manage the sorting order of UIs in the game.
        /// </summary>
        protected int curSortingOrder = -1;

        /// <summary>
        /// Default UI. It will be shown when the stack is cleared. It is recommended to set default UI at the start of the game.
        /// </summary>
        protected TCore defaultUI = null;
        #endregion


        #region Properties
        /// <summary>
        /// Public instance of UINavigationManager. It will be created on first access. 
        /// </summary>
        public static UINavigationManager<TCore> Instance
        {
            get
            {
                instance ??= new UINavigationManager<TCore>();
                return instance;
            }
        }
        #endregion


        #region Constructors
        /// <summary>
        /// Constructor of UINavigationManager. It is private to prevent multiple instances. It will initialize the uiStack and allUIs.
        /// </summary>
        public UINavigationManager()
        {
            uiStack = new();
            allUIs = new();
        }
        #endregion


        #region Public Virtual Methods
        /// <summary>
        /// Set Default UI. On showing default UI, the stack will be cleared. So, it is recommended to set default UI at the start of the game.
        /// You can set any UI as default UI but it is recommended to set Screen as default UI for better user experience.
        /// Default UI is the root of the navigation, so it is never hidden by <see cref="HideCurUI"/> (e.g. close button or device back key)
        /// and never removed by <see cref="ReplaceUI{T}"/>.
        /// </summary>
        /// <typeparam name="T">UI type</typeparam>
        public virtual void SetDefaultUI<T>() where T : TCore
        {
            defaultUI = GetUI<T>();
            Logger.Log("Set Default UI: {0}", typeof(T));
        }

        /// <summary>
        /// Get UI of type T. It will first check in the allUIs dictionary, 
        /// if not found then it will check in the existing UIs in the scene, 
        /// if still not found then it will instantiate from the prefab list. 
        /// If UI is not found in any of these places, it will return null.
        /// </summary>
        /// <typeparam name="T">UI type</typeparam>
        /// <returns>Object of UI</returns>
        public virtual T GetUI<T>() where T : TCore
        {
            T ui;

            // Check if the ui already exist in dictionary
            if (allUIs.ContainsKey(typeof(T)))
            {
                ui = allUIs[typeof(T)] as T;

                if (ui != null)
                {
                    return ui;
                }
                else
                {
                    // Remove the entry if the ui is not found
                    allUIs.Remove(typeof(T));
                }
            }

            // Null Check for UIPrefabListController
            if (UIPrefabListController.Instance == null)
            {
                Logger.LogWarning("UIPrefabListController instance is null. " +
                    "Please make sure you have added UIPrefabListController in the scene.");
                return null;
            }

            // Find the ui in existing UI list
            ui = UIPrefabListController.Instance.GetExistingUI<T>();
            if (ui != null)
            {
                return ui;
            }

            // Find the ui in prefab list and instantiate it
            T uiPrefab = UIPrefabListController.Instance.GetUIPrefab<T>();
            if (uiPrefab != null)
            {
                if (UIPrefabListController.Instance.uiRoot == null)
                {
                    Logger.LogWarning("UIPrefabListController.uiRoot is null. " +
                        "Please make sure you have set the uiRoot in UIPrefabListController to instantiate UIs under it.");
                }
                ui = Object.Instantiate(uiPrefab, UIPrefabListController.Instance.uiRoot);
                allUIs.Add(typeof(T), ui);
                return ui;
            }

            Logger.LogWarning($"UI of type {typeof(T)} not found.");
            return null;
        }

        /// <summary>
        /// Show UI of type T. 
        /// It will first get the UI object using <see cref="GetUI{T}"/> method, then show if.
        /// If the new UI is Screen, it will hide the current showing Screen along with all UIs above it.
        /// If the UI is already the current showing UI, it will only set the parameters instead of adding it to the stack again.
        /// </summary>
        /// <param name="parameters">UI parameters to pass along with showing screen</param>
        /// <param name="resetStack">Whether to reset the UI stack</param>
        /// <param name="sortingOrder">Sorting order for the UI</param>
        /// <typeparam name="T">UI type</typeparam>
        /// <returns>Current Showing UI</returns>
        public virtual T ShowUI<T>(IUIParameters parameters = null, bool resetStack = false, int sortingOrder = -1) where T : TCore
        {
            // Get the UI object
            T upcomingUi = GetUI<T>();

            // Null Check
            if (upcomingUi == null)
            {
                Logger.LogWarning($"UI of type {typeof(T)} not found.");
                return null;
            }

            ShowUI(upcomingUi, parameters, resetStack, sortingOrder);

            // Return the ui object
            return upcomingUi;
        }

        /// <summary>
        /// Replace Current Showing UI with UI of type T, without adding a new entry in the stack.
        /// Going back from UI of type T will show the UI that was below the replaced UI.
        /// Default UI is never replaced, UI of type T will be shown above it instead.
        /// </summary>
        /// <param name="parameters">UI parameters to pass along with showing screen</param>
        /// <param name="sortingOrder">Sorting order for the UI</param>
        /// <typeparam name="T">UI type</typeparam>
        /// <returns>Current Showing UI</returns>
        public virtual T ReplaceUI<T>(IUIParameters parameters = null, int sortingOrder = -1) where T : TCore
        {
            // Get the UI object
            T upcomingUi = GetUI<T>();

            // Null Check
            if (upcomingUi == null)
            {
                Logger.LogWarning($"UI of type {typeof(T)} not found.");
                return null;
            }

            // Remove current ui from the stack, without showing the UIs below it
            if (uiStack.TryPeek(out TCore curUI) && curUI != upcomingUi && curUI != defaultUI)
            {
                PopUI();

                // If upcoming ui was just below the replaced ui, then it's same as going back to it
                bool isUpcomingUiOnTop = GetCurrentShowingUI<TCore>() == upcomingUi;

                // Replaced Screen was covering the UIs below it. Show them again if upcoming ui is one of them or is not going to cover them.
                if (curUI is Screen && (isUpcomingUiOnTop || upcomingUi is not Screen))
                    ShowTopLayer();

                if (isUpcomingUiOnTop)
                {
                    upcomingUi.SetParameters(parameters);
                    OnChangeCurShowingUI?.Invoke(upcomingUi);
                    return upcomingUi;
                }
            }

            ShowUI(upcomingUi, parameters, false, sortingOrder);

            // Return the ui object
            return upcomingUi;
        }

        /// <Summary> 
        /// Get Current Showing UI
        /// </Summary>
        /// <typeparam name="T">UI type</typeparam>
        /// <returns>Current Showing UI</returns>
        public virtual T GetCurrentShowingUI<T>() where T : TCore
        {
            return uiStack.TryPeek(out TCore ui) ? ui as T : null;
        }

        /// <summary>
        /// Hide Current Showing UI. It will show the previous showing UI if any exist.
        /// Default UI is never hidden, as nothing would be left to show.
        /// </summary>
        public virtual void HideCurUI()
        {
            // Default UI is the root of the navigation, so don't hide it
            if (defaultUI != null && uiStack.TryPeek(out TCore topUI) && topUI == defaultUI)
            {
                Logger.Log("Default UI can't be hidden: {0}", defaultUI.GetType());
                return;
            }

            TCore curUI = PopUI();
            if (curUI != null && uiStack.TryPeek(out TCore previousUI) && previousUI != null)
            {
                // If current closing ui is Screen, then show the UIs it was covering
                if (curUI is Screen)
                {
                    ShowTopLayer();
                }
                else
                {
                    Logger.Log("Top UI: {0} at Sorting Order: {1}", previousUI.GetType(), curSortingOrder);
                }
            }

            // Notify listeners about the change
            if (uiStack.TryPeek(out TCore latestCurUI) && latestCurUI != null)
            {
                OnChangeCurShowingUI?.Invoke(latestCurUI);
            }
        }

        /// <summary>
        /// Hide Current Showing UI only if it match with the given UI. It will show the previous showing UI if any exist.
        /// </summary>
        /// <param name="ui">UI to match if it's current showing UI</param>
        public virtual void HideCurUIOnlyIfMatch(TCore ui)
        {
            if (GetCurrentShowingUI<TCore>() == ui)
                HideCurUI();
        }

        /// <summary>
        /// Hide Current Showing UI only if it match with the given UI. It will show the previous showing UI if any exist.
        /// </summary>
        /// <typeparam name="T">UI type to match if it's current showing UI</typeparam>
        public virtual void HideCurUIOnlyIfMatch<T>() where T : TCore
        {
            if (GetCurrentShowingUI<T>() != null)
                HideCurUI();
        }

        /// <summary>
        /// Hide all UIs till the given UI. It will show the given UI if found in the stack. 
        /// If not found, it will hide all UIs and show default UI if set.
        /// </summary>
        /// <typeparam name="T">UI type to match if it's current showing UI</typeparam>
        /// <param name="maxIteration">Maximum number of iterations to hide UIs</param>
        public virtual void HideAllUITill<T>(int maxIteration = 5) where T : TCore
        {
            // If the given UI is not in the stack and default UI is also not in the stack, then show default UI.
            // If default UI is in the stack, the loop below will hide all UIs till the default UI.
            if (!IsUIInStack<T>() && defaultUI != null && !uiStack.Contains(defaultUI))
            {
                ShowUI(defaultUI, null, true, -1);
                return;
            }

            for (int i = 0; i < maxIteration && GetCurrentShowingUI<T>() == null; i++)
            {
                int stackCount = uiStack.Count;
                HideCurUI();

                // Stop if nothing was hidden, i.e. stack is empty or default UI is on top
                if (uiStack.Count == stackCount)
                    break;
            }
        }

        /// <summary>
        /// Check if UI of type T exist in the stack. It will return true if found, otherwise false.
        /// </summary>
        /// <typeparam name="T">UI type to check</typeparam>
        /// <returns>Whether the UI type exists in the stack</returns>
        public virtual bool IsUIInStack<T>() where T : TCore
        {
            foreach (TCore ui in uiStack)
            {
                if (ui is T)
                    return true;
            }
            return false;
        }
        #endregion


        #region Internal & Protected Virtual Methods
        /// <summary>
        /// Called when the device back key is pressed.
        /// </summary>
        /// <returns>Whether the back key was handled</returns>
        internal virtual bool OnPressedDeviceBackKey()
        {
            if (uiStack.TryPeek(out TCore curUI) && curUI != null)
            {
                curUI.CallOnPressedDeviceBackKey();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Show the given UI. See <see cref="ShowUI{T}"/>.
        /// </summary>
        /// <param name="upcomingUi">UI to show</param>
        /// <param name="parameters">UI parameters to pass along with showing screen</param>
        /// <param name="resetStack">Whether to reset the UI stack</param>
        /// <param name="sortingOrder">Sorting order for the UI</param>
        protected virtual void ShowUI(TCore upcomingUi, IUIParameters parameters, bool resetStack, int sortingOrder)
        {
            resetStack |= upcomingUi == defaultUI;

            // If ui is already on top, only set the parameters. Otherwise it would be in the stack twice and need two backs to close.
            if (!resetStack && uiStack.TryPeek(out TCore topUI) && topUI == upcomingUi)
            {
                upcomingUi.SetParameters(parameters);
                Logger.Log("UI is already on top: {0}", upcomingUi.GetType());
                return;
            }

            // If upcoming ui is Screen, then hide current Screen and all UIs above it.
            // On resetting the stack, hide them too, as they won't be in the stack anymore.
            if (resetStack || upcomingUi is Screen)
            {
                HideTopLayer(upcomingUi);
            }

            // Reset Stack if needed
            if (resetStack)
            {
                uiStack.Clear();
                if (sortingOrder < 0)
                    SetDefaultSortingOrder(upcomingUi);
            }
            else if (curSortingOrder == -1)
            {
                SetDefaultSortingOrder(upcomingUi);
            }

            // Set Current Sorting Order
            curSortingOrder = sortingOrder >= 0 ? sortingOrder : ++curSortingOrder;

            // Show the new ui
            uiStack.Push(upcomingUi);
            upcomingUi.Show(parameters, curSortingOrder);
            Logger.Log("Show UI: {0} with Sorting Order: {1}", upcomingUi.GetType(), curSortingOrder);

            // Notify listeners about the change
            OnChangeCurShowingUI?.Invoke(upcomingUi);
        }

        /// <summary>
        /// Remove the top UI from the stack and hide it. Current sorting order will be set as per the UI below it.
        /// It doesn't show the UIs below it.
        /// </summary>
        /// <returns>Removed UI, or null if stack is empty</returns>
        protected virtual TCore PopUI()
        {
            if (!uiStack.TryPop(out TCore curUI))
                return null;

            HideUI(curUI);

            // Set Current Sorting Order as per previous UI
            if (uiStack.TryPeek(out TCore previousUI) && previousUI != null)
                curSortingOrder = previousUI.Canvas.sortingOrder;
            else
                SetDefaultSortingOrder(curUI);

            return curUI;
        }

        /// <summary>
        /// Get the top layer of the stack, from top to bottom. It's the top UI and all UIs below it till the top most Screen (included).
        /// These are the UIs visible on the screen, as the top most Screen hides all UIs below it.
        /// </summary>
        /// <returns>UIs of the top layer, from top to bottom</returns>
        protected virtual List<TCore> GetTopLayer()
        {
            // Copy is used, as showing or hiding UI can trigger navigation (e.g. from OnEnable/OnDisable) which modifies the stack
            List<TCore> topLayer = new();
            foreach (TCore ui in uiStack)
            {
                topLayer.Add(ui);
                if (ui is Screen)
                    break;
            }
            return topLayer;
        }

        /// <summary>
        /// Hide all UIs of the top layer. See <see cref="GetTopLayer"/>.
        /// </summary>
        /// <param name="exceptUI">UI to keep showing, e.g. UI which is going to be shown</param>
        protected virtual void HideTopLayer(TCore exceptUI = null)
        {
            foreach (TCore ui in GetTopLayer())
            {
                if (ui != exceptUI)
                    HideUI(ui);
            }
        }

        /// <summary>
        /// Show all UIs of the top layer again, from bottom to top. See <see cref="GetTopLayer"/>.
        /// It is used on going back from a Screen. UIs will keep their sorting order and parameters.
        /// </summary>
        protected virtual void ShowTopLayer()
        {
            List<TCore> topLayer = GetTopLayer();
            for (int i = topLayer.Count - 1; i >= 0; i--)
            {
                TCore ui = topLayer[i];
                if (ui == null)
                    continue;

                ui.Reshow();
                Logger.Log("Show Previous UI: {0} with Sorting Order: {1}", ui.GetType(), ui.Canvas.sortingOrder);
            }
        }

        /// <summary>
        /// Hide the given UI if it's showing.
        /// </summary>
        /// <param name="ui">UI to hide</param>
        protected virtual void HideUI(TCore ui)
        {
            if (ui == null || !ui.gameObject.activeSelf)
                return;

            ui.Hide();
            Logger.Log("Hide UI: {0}", ui.GetType());
        }

        /// <summary>
        /// Set default sorting order based on the UI type. By default, it will set -1 for Screen and 100 for other UIs.
        /// </summary>
        /// <param name="uiType">The UI type for which to set the default sorting order</param>
        protected virtual void SetDefaultSortingOrder(TCore uiType)
        {
            curSortingOrder = uiType is SNP ? Configurations.DefaultSortingOrderOfSNP : Configurations.DefaultSortingOrderOfNotification;
        }
        #endregion
    }
}