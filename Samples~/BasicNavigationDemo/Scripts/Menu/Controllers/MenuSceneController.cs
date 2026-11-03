#region Using
using NG.UINavigationSystem.Utilities;
using UnityEngine;
#endregion

namespace NG.UINavigationSystem.Samples.BasicNavigationDemo
{
    internal class MenuSceneController : MonoBehaviour
    {
        private void Start()
        {
            // Configure the navigation system
            Configurations.AllowDeviceBackKey = true;
            Configurations.LogLevel = LogLevel.Full;

            // Edge swipe to go back, on iOS only
            Configurations.SwipeBack.AllowLeftEdge = true;
            Configurations.SwipeBack.AllowRightEdge = true;

            // Show Home Screen by default on app start
            UINavigationManager<SNP>.Instance.SetDefaultUI<HomeScreen>();
            UINavigationManager<SNP>.Instance.ShowUI<HomeScreen>();
        }
    }
}