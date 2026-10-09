using UnityEngine;
using UnityEngine.UI;

namespace NG.UINavigationSystem.Samples.BasicNavigationDemo
{
    public class LevelsScreen : Screen
    {

        [SerializeField] private Button[] allLevelButtons;

        protected override void Awake()
        {
            base.Awake();

            foreach (Button button in allLevelButtons)
            {
                button.onClick.AddListener(() => UINavigationManager<SNP>.Instance.ShowUI<SubLevelScreen>());
            }
        }

        protected override void OnPressedDeviceBackKey()
        {
            UINavigationManager<SNP>.Instance.ShowUI<HomeScreen>();
        }
    }
}