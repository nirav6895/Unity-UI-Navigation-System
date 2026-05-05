namespace NG.UINavigationSystem
{
    /// <summary>
    /// Base class for Screens & Popups Only. 
    /// </summary>
    public abstract class SNP : UI
    {
        #region Overridde Methods
        /// <summary>
        /// Override the default behavior of the close button to hide this SNP when clicked.
        /// </summary>
        protected override void OnClickCloseButton()
        {
            if (!AllowBackKey)
                return;

            UINavigationManager<SNP>.Instance.HideCurUIOnlyIfMatch(this);
        }

        /// <summary>
        /// Override the default behavior of the device back key to hide this SNP when pressed.
        /// </summary>
        protected override void OnPressedDeviceBackKey()
        {
            if (!AllowBackKey)
                return;

            UINavigationManager<SNP>.Instance.HideCurUIOnlyIfMatch(this);
        }
        #endregion
    }
}