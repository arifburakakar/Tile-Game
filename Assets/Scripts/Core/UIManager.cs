using UnityEngine;

public partial class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] 
    private Canvas canvas;
    [SerializeField]
    private UIHudPanel uiMainHudPanel;
    [SerializeField] 
    private UIHudPanel uiGameplayHudPanel;
    
    public void OnCreate()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void Initialize()
    {
        uiMainHudPanel.Initialize();
        uiGameplayHudPanel.Initialize();
        canvas.worldCamera = Camera.main;
    }
    
    public void OpenGameplayPanel()
    {
        uiGameplayHudPanel.Open();
        CloseMainMenuPanel();
    }

    public void CloseGameplayPanel()
    {
        uiGameplayHudPanel.Close();
    }

    public void OpenMainMenuPanel()
    {
        uiMainHudPanel.Open();
        CloseGameplayPanel();
    }

    public void CloseMainMenuPanel()
    {
        uiMainHudPanel.Close();
    }
    
    public void OnLevelStart()
    {
        uiGameplayHudPanel.OnLevelStart();
        uiMainHudPanel.OnLevelStart(); // neden duzelt
    }

    public void OnLevelWin()
    {
        uiGameplayHudPanel.OnLevelWin();
        uiMainHudPanel.OnLevelWin();
    }
}