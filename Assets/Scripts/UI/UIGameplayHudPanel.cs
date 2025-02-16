public class UIGameplayHudPanel : UIHudPanel
{
    public void OnClickPlay()
    {
        GameManager.Instance.LoadGameplay();
    }
}