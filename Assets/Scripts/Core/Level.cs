using UnityEngine;

public class Level
{
    public LevelData LevelData { get; private set; }
    public LevelType LevelType { get; private set; }
    public GameObject LevelObject { get; private set; }
    public Game Game { get; private set; }
    
    public Level(int level, LevelData levelData)
    {
        LevelObject = new GameObject($"Level {level + 1}");
        LevelData = levelData;
        LevelType = levelData.LevelType;
        Game = new Game(this);
        Game.CreateGame();
    }    
    
    public void InitializeLevel()
    {
        Game.InitializeGame();
    }
    
    public void TryLevelEnd()
    {
        // try open win screen
        // try open lose screen
    }
    
    public void ClearLevel()
    {
        Game.ResetGame();
        Object.Destroy(LevelObject);
    }
}