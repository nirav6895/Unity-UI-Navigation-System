using UnityEngine;
using UnityEngine.UI;

namespace NG.UINavigationSystem.Samples.BasicNavigationDemo
{
    public class HomeScreen : Screen
    {
        [Header("Child References - Home Screen")]
        [SerializeField] private Button settingButton;
        [SerializeField] private Button noInternetButton;

        protected override void AddListeners()
        {
            base.AddListeners();

            settingButton.onClick.AddListener(OnClickSettingButton);
            noInternetButton.onClick.AddListener(OnClickNoInternetButton);
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();

            settingButton.onClick.RemoveListener(OnClickSettingButton);
            noInternetButton.onClick.RemoveListener(OnClickNoInternetButton);
        }

        protected override void OnPressedDeviceBackKey()
        {
            UINavigationManager<SNP>.Instance.ShowUI<ExitPopup>();
        }

        private void OnClickSettingButton()
        {
            UINavigationManager<SNP>.Instance.ShowUI<SettingPopup>();
        }

        private void OnClickNoInternetButton()
        {
            UINavigationManager<Notification>.Instance.HideCurUIOnlyIfMatch<NoInternetNotification>();
            UINavigationManager<Notification>.Instance.ShowUI<NoInternetNotification>(new NoInternetNotificationParameters { isInternetAvailable = false });

            UINavigationManager<SNP>.Instance.ShowUI<NoInternetPopup>();
        }
    }
}