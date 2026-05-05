#region Using
using NG.UINavigationSystem.Utilities;
#endregion

namespace NG.UINavigationSystem
{
    /// <summary>
    /// Provides configuration options for the navigation system.
    /// </summary>
    public static class Configurations
    {
        /// <summary>
        /// Allow device back key functionality globally.
        /// </summary>
        public static bool AllowDeviceBackKey = true;

        /// <summary>
        /// Default sorting order for Screens and Popups only. 
        /// You can set it to a higher value if you want them to appear above other UI elements in your scene by default.
        /// </summary>
        public static int DefaultSortingOrderOfSNP = -1;

        /// <summary>
        /// Default sorting order for Notifications only. 
        /// You can set it to a higher value if you want them to appear above other UI elements in your scene by default.
        /// </summary>
        public static int DefaultSortingOrderOfNotification = 100;

        /// <summary>
        /// Log level for the navigation system. Adjust this to control the verbosity of logs for debugging purposes.
        /// </summary>
        /// <value></value>
        public static LogLevel LogLevel
        {
            get => Logger.LogLevel;
            set => Logger.LogLevel = value;
        }

        /// <summary>
        /// Configurations related to Popups.
        /// </summary>
        public static class Popup
        {
            /// <Summary> 
            /// Animation time of open/close for Popup
            /// </Summary>
            public static float OpenCloseAnimationTime = 0.25f;

            /// <Summary> 
            /// Animation time of Raycast Blocker fade in/out for Popup
            /// </Summary>
            public static float RaycastBlockerFadeTime = 0.25f;
        }

        /// <summary>
        /// Configurations related to Notifications.
        /// </summary>
        public static class Notification
        {
            /// <Summary> 
            /// Show time duration of Notification before auto close
            /// </Summary>
            public static float ShowTime { get; set; } = 2f;

            /// <summary>
            /// Allow device back key functionality for Notification.
            /// </summary>
            public static bool AllowDeviceBackKey { get; set; } = false;
        }
    }
}