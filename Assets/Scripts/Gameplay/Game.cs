using System.Collections.Generic;using UnityEngine;

public partial class Game
{
    private LevelManager levelManager;
    private Level level;
    private GameplayConfig gameplayConfig;
    private int boardActionCounter = 0;
    private GameObject gameContainer;

    public GameObject GameContainer => gameContainer;
    
    public Game(Level level)
    {
        this.level = level;
    }
    
    public void CreateGame()
    {
        gameplayConfig = GameManager.Instance.GameplayConfig;
        levelManager = LevelManager.Instance;
        gameContainer = new GameObject("Game Container");
        gameContainer.transform.SetParent(level.LevelObject.transform);
    }

    public void InitializeGame()
    {
        Main.Instance.MainUpdate += UpdateGame;
        Main.Instance.MainFixedUpdate += FixedUpdateGame;
        Main.Instance.MainLateUpdate += LateUpdateGame;
        
        InitializeInput();   
        CreateGrid();
    }

    private void UpdateGame()
    {
        //Debug.Log(boardActionCounter);
    }

    private void FixedUpdateGame()
    {

    }

    private void LateUpdateGame()
    {

    }
    
    public void BoardActionStart()
    {
        boardActionCounter++;
    }
    
    public void BoardActionEnd()
    {
        int previousActionCount = boardActionCounter;
        boardActionCounter = Mathf.Clamp(boardActionCounter - 1, 0, int.MaxValue);
        if (boardActionCounter == 0 && previousActionCount - boardActionCounter > 0)
        {
            AllActionsEnd();
        }   
    }
    
    private void AllActionsEnd()
    {
        level.TryLevelEnd();
    }

    
    public void ResetGame()
    {
        Main.Instance.MainUpdate -= UpdateGame;
        Main.Instance.MainFixedUpdate -= FixedUpdateGame;
        Main.Instance.MainLateUpdate -= LateUpdateGame;
        
        ResetInput();
    }
}
