#region Using
using System.Collections.Generic;
using UnityEngine;
#endregion

namespace NG.UINavigationSystem
{
    [CreateAssetMenu(
        fileName = "UIPrefabListSO",
        menuName = "UI Navigation System/Create UIPrefabListSO",
        order = 1
    )]
    public class UIPrefabListSO : ScriptableObject
    {
        #region Inspector Variables
        /// <summary>
        /// UI Prefabs to instantiate.
        /// </summary>
        [SerializeField] protected List<UI> uiPrefabs;
        #endregion


        #region Public Methods
        /// <summary>
        /// Gets a UI prefab of the specified type from the list of UIPrefabListSO.
        /// </summary>
        /// <typeparam name="T">The type of UI prefab to get.</typeparam>
        /// <returns>The found UI prefab or null if not found.</returns>
        public virtual T GetUIPrefab<T>() where T : UI
        {
            return uiPrefabs.Find(x => x is T) as T;
        }
        #endregion
    }
}