using UnityEngine;

public class GameManager : SingletonGameSystem<GameManager>
{
    public bool IsMainActive { get; private set; }
    public GameplayConfig GameplayConfig { get; private set; }
    public int Level { get; private set; } 

    protected override void OnInitialize()
    {
        base.OnInitialize();
        GameplayConfig = Resources.Load<GameplayConfig>("GameplayConfig");
    }
    
    private void StartGameplay()
    {
        Main.Instance.SetInputEnable(true);
        LevelManager.Instance.LoadLevel(); 
    }
    
    public void GameplayCompleted(bool isSuccess)
    {
        Main.Instance.SetInputEnable(false);
        
        if (isSuccess)
        {
            Level++;
        }
        
        LoadMain();
    }
    
    public void LoadMain()
    {
        UIManager.Instance.OpenMainMenuPanel();
        LevelManager.Instance.UnloadLeveL();
        GameUtility.GCCollectDefault();
        IsMainActive = true;
    }
    
    public void LoadGameplay()
    {
        UIManager.Instance.OpenGameplayPanel();
        StartGameplay();
        IsMainActive = false;
    }
}