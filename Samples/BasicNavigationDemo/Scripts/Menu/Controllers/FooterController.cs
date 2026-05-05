#region Using
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace NG.UINavigationSystem.Samples.BasicNavigationDemo
{
    public class FooterController : MonoBehaviour
    {
        [SerializeField] private GameObject footerPanel;
        [SerializeField] private Button homeButton;
        [SerializeField] private Button levelsButton;
        [SerializeField] private Button storeButton;

        private void Awake()
        {
            homeButton.onClick.AddListener(() => OpenScreen<HomeScreen>(true));
            levelsButton.onClick.AddListener(() => OpenScreen<LevelsScreen>());
            storeButton.onClick.AddListener(() => OpenScreen<StoreScreen>());
        }

        private void OpenScreen<T>(bool resetStack = false) where T : SNP
        {
            UINavigationManager<SNP>.Instance.ShowUI<T>(resetStack: resetStack);
        }

        private void OnEnable()
        {
            UINavigationManager<SNP>.OnChangeCurShowingUI += HandleOnChangeCurShowingUI;
        }

        private void OnDisable()
        {
            UINavigationManager<SNP>.OnChangeCurShowingUI -= HandleOnChangeCurShowingUI;
        }

        private void HandleOnChangeCurShowingUI(SNP newScreen)
        {
            // Update UI elements based on the currently showing screen
            homeButton.interactable = newScreen is not HomeScreen;
            levelsButton.interactable = newScreen is not LevelsScreen;
            storeButton.interactable = newScreen is not StoreScreen;

            switch (newScreen)
            {
                case HomeScreen:
                case LevelsScreen:
                case StoreScreen:
                    footerPanel.SetActive(true);
                    break;
                default:
                    footerPanel.SetActive(false);
                    break;
            }
        }
    }
}