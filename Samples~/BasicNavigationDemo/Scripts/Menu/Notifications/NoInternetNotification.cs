using UnityEngine;
using UnityEngine.UI;

namespace NG.UINavigationSystem.Samples.BasicNavigationDemo
{
    public class NoInternetNotification : Notification
    {
        [Header("Child References - No Internet Notification")]
        [SerializeField] private Text internetStatusText;

        public override void SetParameters(IUIParameters parameters = null)
        {
            base.SetParameters(parameters);

            if (parameters is NoInternetNotificationParameters noInternetParams)
            {
                internetStatusText.text = noInternetParams.isInternetAvailable ? "Internet is available!" : "No Internet!";
            }
        }
    }

    public class NoInternetNotificationParameters : IUIParameters
    {
        public bool isInternetAvailable;
    }
}