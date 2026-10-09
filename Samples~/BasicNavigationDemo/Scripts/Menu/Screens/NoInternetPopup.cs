using UnityEngine;
using UnityEngine.UI;

namespace NG.UINavigationSystem.Samples.BasicNavigationDemo
{
    public class NoInternetPopup : Popup
    {
        [Header("Child References - No Internet Popup")]
        [SerializeField] private Text backKeyNotAllowedText;

        protected override void OnEnable()
        {
            base.OnEnable();

            backKeyNotAllowedText.gameObject.SetActive(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            UINavigationManager<Notification>.Instance.HideCurUIOnlyIfMatch<NoInternetNotification>();
            UINavigationManager<Notification>.Instance.ShowUI<NoInternetNotification>(new NoInternetNotificationParameters { isInternetAvailable = true });
        }


        protected override void OnPressedDeviceBackKey()
        {
            backKeyNotAllowedText.gameObject.SetActive(true);
        }
    }
}