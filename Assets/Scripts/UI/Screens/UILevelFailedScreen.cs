public class UILevelFailedScreen : UIScreen
{
    public void OnReturnMetaClick()
    {
        GameManager.Instance.GameplayCompleted(false);
        Close();
    }
}
