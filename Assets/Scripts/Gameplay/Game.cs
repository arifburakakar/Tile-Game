using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;

public partial class Game
{
    private LevelManager levelManager;
    private Level level;
    private GameplayConfig gameplayConfig;
    private int boardActionCounter = 0;
    private GameContainerProxy gameContainer;

    public GameContainerProxy GameContainer => gameContainer;
    
    public Game(Level level)
    {
        this.level = level;
    }
    
    public void CreateGame()
    {
        gameplayConfig = GameManager.Instance.GameplayConfig;
        levelManager = LevelManager.Instance;
        gameContainer = Object.Instantiate(Resources.Load<GameContainerProxy>("Game Container"), level.LevelObject.transform, true);
    }

    public void InitializeGame()
    {
        Main.Instance.MainUpdate += UpdateGame;
        Main.Instance.MainFixedUpdate += FixedUpdateGame;
        Main.Instance.MainLateUpdate += LateUpdateGame;
        
        InitializeInput();
        CreateGrid();
        InitializeHolder();
    }

    private void UpdateGame()
    {
        UpdateHolderVisuals();
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
        if (level.IsLevelEnd)
        {
            return;
        }
        
        if (isHolderFull)
        {
            level.IsLevelEnd = true;
            UIManager.Instance.Open<UILevelFailedScreen>();
        }

        if (IsBoardClear() && holderItems.Count == 0)
        { 
            level.IsLevelEnd = true;
            UIManager.Instance.Open<UILevelWinScreen>();
        }
    }

    
    public void ResetGame()
    {
        Main.Instance.MainUpdate -= UpdateGame;
        Main.Instance.MainFixedUpdate -= FixedUpdateGame;
        Main.Instance.MainLateUpdate -= LateUpdateGame;
        
        ResetInput();
    }
}
