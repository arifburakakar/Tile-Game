using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIScreen : MonoBehaviour
{
    public enum ScreenOpenType
    { 
        ADDITIVE         = 0,
        CLOSE_COMPLETELY = 1,
        CLOSE_QUEUE      = 2
    }

    protected UIManager uiManager;
    [SerializeField]
    protected ScreenOpenType openType = ScreenOpenType.ADDITIVE;
    [SerializeField]
    protected bool shouldOpenBackground = true;
    [SerializeField]
    protected bool shouldCloseOnBackgroundClick = true;

    protected bool isOpenedFromQueue;
    [SerializeField]
    private Animation animation;
    [SerializeField] 
    private AnimationClip openAnimation;
    [SerializeField] 
    private AnimationClip closeAnimation;
    public ScreenOpenType OpenType => openType;
    public bool ShouldOpenBackground => shouldOpenBackground;
    public bool ShouldCloseOnBackgroundClick => shouldCloseOnBackgroundClick;

    public Animation Animation => animation;
    
    public virtual void Initialize(UIManager manager, bool isOpenedFromQueue)
    {
        uiManager = manager;
        this.isOpenedFromQueue = isOpenedFromQueue;
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        OnOpen();
    }

    public void Close()
    {
        OnClose();
        gameObject.SetActive(false);
    }
    
    protected virtual void OnOpen()
    {
        transform.SetAsLastSibling();
        uiManager.OnScreenOpen(this);
        
        animation.Play(openAnimation.name);
        animation[openAnimation.name].time = 0;
        animation.Sample();
        animation.Stop();
        animation.Play(openAnimation.name);
    }

    protected virtual void OnClose()
    {
        uiManager.OnScreenClose(this);
        animation.Play(closeAnimation.name);
    }
}