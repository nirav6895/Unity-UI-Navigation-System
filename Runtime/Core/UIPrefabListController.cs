#region Using
using System;
using System.Collections.Generic;
using UnityEngine;
#endregion

namespace NG.UINavigationSystem
{
    /// <summary>
    /// Manages the list of UI prefabs and their instantiation.
    /// </summary>
    public class UIPrefabListController : MonoBehaviour
    {
        #region Inspector Variables
        [Header("UIPrefabListController Variables")]
        /// <summary>
        /// All UI Prefab List Scriptable Objects.
        /// </summary>
        [SerializeField] protected List<UIPrefabListSO> allUIPrefabListSO;

        /// <summary>
        /// Root of all existing UIs in the scene and where all new UIs will be instantiated.
        /// </summary>
        public Transform uiRoot;
        #endregion


        #region Variables
        /// <summary>
        /// All Existing UIs in the scene.
        /// </summary>
        protected UI[] allExistingUIs;
        #endregion


        #region Properties
        /// <summary>
        /// Public Instance of UIPrefabListController.
        /// </summary>
        public static UIPrefabListController Instance { get; private set; }
        #endregion


        #region Unity Methods
        /// <summary>
        /// On Validate, get all existing UIs in the scene under uiRoot and store them in allExistingUIs array.
        /// </summary>
        protected virtual void OnValidate()
        {
            if (uiRoot != null)
                allExistingUIs = uiRoot.GetComponentsInChildren<Screen>(true);
        }

        /// <summary>
        /// Initialize the Instance of UIPrefabListController.
        /// </summary>
        protected virtual void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// If AllowDeviceBackKey is true in Configurations, check if the device back key is pressed and call OnPressedDeviceBackKey() method.
        /// </summary>
        protected virtual void Update()
        {
            if (Configurations.AllowDeviceBackKey)
            {
#if ENABLE_INPUT_SYSTEM
                if (UnityEngine.InputSystem.Keyboard.current != null &&
                    UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    OnPressedDeviceBackKey();
                }
#else
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    OnPressedDeviceBackKey();
                }
#endif
            }
        }
        #endregion


        #region Virtual Methods
        /// <summary>
        /// Gets an existing UI of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of UI to find.</typeparam>
        /// <returns>The found UI or null if not found.</returns>
        public virtual T GetExistingUI<T>() where T : UI
        {
            // Find in existing UIs first
            if (allExistingUIs != null)
            {
                var existingUI = Array.Find(allExistingUIs, x => x is T);
                if (existingUI != null)
                {
                    return existingUI as T;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a UI prefab of the specified type from the list of UIPrefabListSO.
        /// </summary>
        /// <typeparam name="T">The type of UI prefab to find.</typeparam>
        /// <returns>The found UI prefab or null if not found.</returns>
        public virtual T GetUIPrefab<T>() where T : UI
        {
            // Find in all UIPrefabListSO
            if (allUIPrefabListSO != null)
            {
                foreach (var uiPrefabListSO in allUIPrefabListSO)
                {
                    if (uiPrefabListSO != null)
                    {
                        T uiPrefab = uiPrefabListSO.GetUIPrefab<T>();
                        if (uiPrefab != null)
                        {
                            return uiPrefab;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Handles the device back key press event. It first checks if there are any notifications that should handle the back key press, 
        /// and if not, it checks for any Screens or Popups that should handle it.
        /// </summary>
        protected virtual void OnPressedDeviceBackKey()
        {
            bool foundNotificationForBackKey = false;
            if (Configurations.Notification.AllowDeviceBackKey)
            {
                // Handle Device Back Key Pressed for Notification
                foundNotificationForBackKey = UINavigationManager<Notification>.Instance.OnPressedDeviceBackKey();
            }

            if (!foundNotificationForBackKey)
            {
                // Handle Device Back Key Pressed
                UINavigationManager<SNP>.Instance.OnPressedDeviceBackKey();
            }
        }
        #endregion
    }
}