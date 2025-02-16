public class UIMainHudPanel : UIHudPanel
{
    public void OnClickPlay()
    {
        GameManager.Instance.LoadGameplay();
    }
}