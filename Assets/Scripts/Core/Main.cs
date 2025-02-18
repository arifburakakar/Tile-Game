using System;
using UnityEngine;

public partial class Main : MonoBehaviour
{
    public static Main Instance;
    
    public Action MainUpdate;
    public Action MainFixedUpdate;
    public Action MainLateUpdate;

    private void Awake()
    {
        Instance = this;
        SetTargetFrameRate();
        SetGCToManuel();
        CreateInstances();
    }

    private void Start()
    {
        InitializeInput();
        InitializeInstances();
        GameManager.Instance.LoadMain();
    }

    private void Update()
    {
        HandleInput();
        MainUpdate?.Invoke();
    }
    
    private void FixedUpdate()
    {
        MainFixedUpdate?.Invoke();
    }

    private void LateUpdate()
    {
        MainLateUpdate?.Invoke();
    }
    
    private void SetGCToManuel()
    {
        GameUtility.GCCollectManuel();
    }
    
    private void SetTargetFrameRate()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 120;
    }

    private void CreateInstances()
    {
        GameManager.CreateInstance();
        LevelManager.CreateInstance();
        VFXManager.CreateInstance();
        // monosingleton
        UIManager uimanager = Instantiate(Resources.Load<UIManager>("UIManager"));
        uimanager.OnCreate();
    }

    private void InitializeInstances()
    {
        GameManager.Instance.Initialize();
        LevelManager.Instance.Initialize();
        VFXManager.Instance.Initialize();
        UIManager.Instance.Initialize();
    }
}
