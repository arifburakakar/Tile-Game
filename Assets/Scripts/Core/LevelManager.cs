using UnityEngine;

public class LevelManager : SingletonGameSystem<LevelManager>
{
    private Level activeLevel;
    private Level previousLevel;
    private GameObject levelObject;
    public GameItemsConfig GameItemsConfig { get; private set; }
    public LevelsDataConfig LevelsDataConfig { get; private set; }
    
    public PoolHandler PoolHandler { get; private set; }
    public Level ActiveLevel => activeLevel;

    protected override void OnCreate()
    {
        base.OnCreate();
        LevelsDataConfig = Resources.Load<LevelsDataConfig>("LevelsDataConfig");
        GameItemsConfig = Resources.Load<GameItemsConfig>("GameItemsConfig");
        PoolHandler = new PoolHandler();
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        PoolHandler.Initialize(GameItemsConfig);
    }

    public void LoadLevel()
    {
        int level = GameManager.Instance.Level;
        TextAsset levelText = LevelsDataConfig.Levels[level % LevelsDataConfig.Levels.Count];
        LevelData levelData = JsonUtility.FromJson<LevelData>(levelText.ToString());
        activeLevel = new Level(level, levelData);
        activeLevel.InitializeLevel();
    }

    public void UnloadLeveL()
    {
        PoolHandler.Clear();
        activeLevel?.ClearLevel();
    }
}