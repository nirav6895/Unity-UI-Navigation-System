namespace NG.UINavigationSystem.Samples.BasicNavigationDemo
{
    public class StoreScreen : Screen
    {
        protected override void OnPressedDeviceBackKey()
        {
            UINavigationManager<SNP>.Instance.ShowUI<HomeScreen>();
        }
    }
}