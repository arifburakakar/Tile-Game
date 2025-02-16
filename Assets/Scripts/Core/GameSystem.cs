public class GameSystem
{
    protected GameSystem()
    {
        OnCreate();
    }

    protected virtual void OnCreate()
    {
        
    }

    public void Initialize()
    {
        OnInitialize();
    }
    
    protected virtual void OnInitialize()
    {
        
    }
}

public abstract class SingletonGameSystem<T> : GameSystem where T : GameSystem, new()
{
    public static T Instance { get; private set; }

    public static T CreateInstance()
    {
        Instance = new T();
        return Instance;
    }

    public static void InitializeInstance()
    {
        Instance.Initialize();
    }
}