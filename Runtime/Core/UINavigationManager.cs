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
        /// </summary>
        /// <typeparam name="T">UI type</typeparam>
        public virtual void SetDefaultUI<T>() where T : TCore
        {
            defaultUI = GetUI<T>();
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
                Logger.LogWarning("UINavigationManager : UIPrefabListController instance is null. " +
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
                    Logger.LogWarning("UINavigationManager : UIPrefabListController.uiRoot is null. " +
                        "Please make sure you have set the uiRoot in UIPrefabListController to instantiate UIs under it.");
                }
                ui = Object.Instantiate(uiPrefab, UIPrefabListController.Instance.uiRoot);
                allUIs.Add(typeof(T), ui);
                return ui;
            }

            Logger.LogWarning($"UINavigationManager : UI of type {typeof(T)} not found.");
            return null;
        }

        /// <summary>
        /// Show UI of type T. 
        /// It will first get the UI object using <see cref="GetUI{T}"/> method, then show if.
        /// This will hide the current showing UI if the new UI is Screen,
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
                return null;
            }

            // If upcoming ui is Screen, then hide current ui
            if (upcomingUi is Screen)
            {
                // Close current ui if available
                if (uiStack.TryPeek(out TCore curUI) && curUI != null)
                {
                    curUI.Hide();
                    Logger.Log($"Hide UI: {curUI.GetType()}");
                }
            }

            // Reset Stack if needed
            if (resetStack || upcomingUi == defaultUI)
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
            Logger.Log($"Show UI: {typeof(T)} with Sorting Order: {curSortingOrder}");

            // Notify listeners about the change
            OnChangeCurShowingUI?.Invoke(upcomingUi);

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
        /// </summary>
        public virtual void HideCurUI()
        {
            if (uiStack.TryPop(out TCore curUI) && curUI != null)
            {
                curUI.Hide();
                Logger.Log($"Hide UI: {curUI.GetType()}");

                if (uiStack.TryPeek(out TCore previousUI) && previousUI != null)
                {
                    // Set Current Sorting Order as per previous UI
                    curSortingOrder = previousUI.Canvas.sortingOrder;

                    // If current closing ui is Screen, then show previous UI if any
                    if (curUI is Screen)
                    {
                        previousUI.Show();
                        Logger.Log($"Show Previous UI: {previousUI.GetType()} with Sorting Order: {curSortingOrder}");
                    }
                    else
                    {
                        Logger.Log($"Top UI: {previousUI.GetType()} at Sorting Order: {curSortingOrder}");
                    }
                }
                else
                {
                    SetDefaultSortingOrder(curUI);
                }
            }

            // Notify listeners about the change
            OnChangeCurShowingUI?.Invoke(uiStack.Peek());
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
            int i = 0;
            while (GetCurrentShowingUI<T>() != null && i < maxIteration)
            {
                HideCurUI();
                i++;
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