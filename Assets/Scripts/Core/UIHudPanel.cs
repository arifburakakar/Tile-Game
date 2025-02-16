using UnityEngine;

public class UIHudPanel : MonoBehaviour, IPanel
{
    public void Initialize()
    {
        OnInitialize();
    }
    public void Open()
    {
        OnOpen();
    }

    public void Close()
    {
        OnClose();
    }

    protected virtual void OnInitialize()
    {
        
    }

    protected virtual void OnOpen()
    {
        gameObject.SetActive(true);
    }

    public virtual void OnLevelStart()
    {
        
    }

    public virtual void OnLevelWin()
    {
        
    }

    protected virtual void OnClose()
    {
        gameObject.SetActive(false);
    }
}