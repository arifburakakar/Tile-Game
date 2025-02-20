using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Create LevelEditorWindow", fileName = "LevelEditorWindow", order = 0)]
public class LevelEditorWindow : ScriptableObject
{
    [Button]
    public void OpenGameScene()
    {
        if (EditorApplication.isPlaying)
        {
            SceneManager.LoadSceneAsync(0);
        }
        else
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Game Scene.unity", OpenSceneMode.Single);
        }
    }

    [Button]
    public void OpenLevelEditor()
    {
        string currentScenePath = EditorSceneManager.GetActiveScene().path;

        if (currentScenePath != "Assets/Scenes/Level Editor Scene.unity")
        {
            if (EditorApplication.isPlaying)
            {
                SceneManager.LoadSceneAsync(1);
            }
            else
            {
                EditorSceneManager.OpenScene("Assets/Scenes/Level Editor Scene.unity", OpenSceneMode.Single);
            }
        }
        else
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }
        }

        EditorApplication.EnterPlaymode();
    }

    public string LevelName;
    public LevelType LevelType;
    public Layer TargetLayer;
    public List<Vector2Int> BoardSizes;
    private LevelData activeLevelData;

    public enum Layer
    {
        LAYER_0 = 0,
        LAYER_1 = 1,
        LAYER_2 = 2,
        LAYER_3 = 3,
        LAYER_4 = 4,
        LAYER_5 = 5,
        LAYER_6 = 6,
        LAYER_7 = 7,
    }

    [Button]
    public void ChangeLayer()
    {
        LevelEditor levelEditor = LevelEditor.Instance;
        levelEditor.ChangeLayer((int)TargetLayer);
    }


    [Button]
    public void NewLevel()
    {
        string currentScenePath = EditorSceneManager.GetActiveScene().path;

        if (currentScenePath == "Assets/Scenes/LevelScene.unity" && !EditorApplication.isPlaying)
        {
            return;
        }

        LevelEditor levelEditor = LevelEditor.Instance;
        activeLevelData = new LevelData();
        LevelName = "New Level";
        LevelType = LevelType.NORMAL;
        levelEditor.GenerateGrid(BoardSizes);

        TargetLayer = Layer.LAYER_0;
        ChangeLayer();
    }

    [Button]
    public void LoadLevel()
    {
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
    
        if (currentScenePath == "Assets/Scenes/Level Editor Scene.unity" && !EditorApplication.isPlaying)
        {
            return;
        }
    
        if (LevelName == string.Empty)
        {
            return;
        }
        
        LevelEditor levelEditor = LevelEditor.Instance;
        TextAsset levelText = Resources.Load<TextAsset>(LevelName);
        
        if (levelText == null)
        {
            Debug.Log("Level yok");
            return;
        }
        
        activeLevelData = JsonUtility.FromJson<LevelData>(levelText.ToString());
        LevelType = activeLevelData.LevelType;
        BoardSizes = activeLevelData.BoardSizes;
        levelEditor.FillGrid(activeLevelData.GridData, BoardSizes);
        
        TargetLayer = Layer.LAYER_0;
        ChangeLayer();
    }
    

    [Button]
    public void SaveLevel()
    {
        string currentScenePath = EditorSceneManager.GetActiveScene().path;

        if (currentScenePath == "Assets/Scenes/LevelScene.unity" && !EditorApplication.isPlaying)
        {
            return;
        }

        if (LevelName == string.Empty)
        {
            return;
        }

        LevelEditor levelEditor = LevelEditor.Instance;
        activeLevelData = new LevelData();
        activeLevelData.LevelType = LevelType;
        activeLevelData.BoardSizes = BoardSizes;
        activeLevelData.GridData = new List<CellData>();

        foreach (KeyValuePair<Vector3Int, Cell> cell in levelEditor.Grid.Cells)
        {
            CellData cellData = new CellData();
            cellData.Index = cell.Key;
            cellData.OID = cell.Value.OID;
            cellData.WorldPosition = cell.Value.WorldPosition;
            activeLevelData.GridData.Add(cellData);
        }

        string text = JsonUtility.ToJson(activeLevelData, true);
        TextAsset textAsset = new TextAsset(text);

        if (Resources.Load<TextAsset>(LevelName))
        {
            AssetDatabase.DeleteAsset($"Assets/Data/Levels/Resources/{LevelName}");
        }

        AssetDatabase.CreateAsset(textAsset, $"Assets/Data/Levels/Resources/{LevelName}.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    // [Button]
    // public async void Play()
    // {
    //     await SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    //
    //     await Yield.WaitForFixedUpdate();
    //
    //     Main.Instance.SetInputEnable(true);
    //
    //     activeLevelData.LevelMove = LevelMove;
    //     activeLevelData.LevelType = LevelType;
    //
    //     activeLevelData.Objectives = new List<ObjectiveItemData>(this.ObjectiveItems);
    //     activeLevelData.Fillers = new List<GeneratorsItemData>(this.GeneratorsItems);
    //     activeLevelData.GridData = new List<CellData>();
    //     foreach (KeyValuePair<Vector2Int, Cell> cell in LevelEditor.Instance.Grid.Cells)
    //     {
    //         CellData cellData = new CellData();
    //         cellData.Index = cell.Key;
    //         cellData.OID = cell.Value.OID;
    //         activeLevelData.GridData.Add(cellData);
    //     }
    //
    //     LevelManager.Instance.OpenTargetLevelData(activeLevelData);
    //
    //     UIManager.Instance.OnLevelStart();
    //     GameManager.Instance.LoadGameplayForEditor();
    // }
}