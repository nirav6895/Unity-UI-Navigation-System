using UnityEngine;
using UnityEngine.UI;

namespace NG.UINavigationSystem.Samples.BasicNavigationDemo
{
    public class SettingPopup : Popup
    {
        [Header("Child References - Setting Popup")]
        [SerializeField] private Button languageButton;
        [SerializeField] private Button storeButton;

        protected override void AddListeners()
        {
            base.AddListeners();

            languageButton.onClick.AddListener(OnClickLanguageButton);
            storeButton.onClick.AddListener(OnClickStoreButton);
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();

            languageButton.onClick.RemoveListener(OnClickLanguageButton);
            storeButton.onClick.RemoveListener(OnClickStoreButton);
        }

        private void OnClickLanguageButton()
        {
            UINavigationManager<SNP>.Instance.ShowUI<LanguagePopup>();
        }

        private void OnClickStoreButton()
        {
            UINavigationManager<SNP>.Instance.ShowUI<StoreScreen>();
        }
    }
}