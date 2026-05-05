#region Using
#endregion

namespace NG.UINavigationSystem
{
    /// <summary>
    /// Base class for creating notification UI elements.
    /// </summary>
    public abstract class Notification : UI
    {
        #region Inspector Variables
        #endregion


        #region Constant Variables
        #endregion


        #region Overridden Methods
        /// <summary>
        /// Overridden to automatically close the notification after some time defined in Configurations.Notification.ShowTime. 
        /// You can also close the notification by clicking the close button or pressing the device back key.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            // Close notification automatically after some time
            Invoke(nameof(OnClickCloseButton), Configurations.Notification.ShowTime);
        }

        /// <summary>
        /// Overridden to cancel the automatic close if the notification is disabled before the time defined in Configurations.Notification.ShowTime.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            // Cancel automatic close if notification is disabled before time
            CancelInvoke(nameof(OnClickCloseButton));
        }

        /// <summary>
        /// On Click Close Button, Close this Notification
        /// </summary>
        protected override void OnClickCloseButton()
        {
            if (!AllowBackKey)
                return;

            UINavigationManager<Notification>.Instance.HideCurUIOnlyIfMatch(this);
        }

        /// <summary>
        /// Overridden to hide this Notification when pressed back key.
        /// </summary>
        protected override void OnPressedDeviceBackKey()
        {
            if (!AllowBackKey)
                return;

            UINavigationManager<Notification>.Instance.HideCurUIOnlyIfMatch(this);
        }
        #endregion
    }
}