using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public partial class UIManager
{
    [SerializeField] 
    private Transform screenContainer;
    [SerializeField] 
    private CanvasGroup background;
    private UIScreen topScreen => activeScreens.Count == 0 ? null : activeScreens.Peek();
    private Stack<UIScreen> activeScreens = new Stack<UIScreen>();
    private List<OpenScreenRequest> openRequests = new List<OpenScreenRequest>();
    private List<OpenScreenRequest> queuedRequests = new List<OpenScreenRequest>();
    private int runningOpenRequestCount;
    private int runningQueueRequestCount;
    private List<UIScreen> loadedScreens = new List<UIScreen>();
    public bool HasActiveScreen => activeScreens.Count > 0;
    private void LateUpdate()
    {
        if (openRequests.Count > 0)
        {
            OpenScreenRequest openAllowedScreenRequest = null;

            for (int i = 0; i < openRequests.Count; i++)
            {
                OpenScreenRequest screenRequest = openRequests[i];
                openRequests.RemoveAt(i);
                openAllowedScreenRequest = screenRequest;
                break;
            }

            if (openAllowedScreenRequest == null)
            {
                return;
            }

            OpenScreen(openAllowedScreenRequest, true);
        }
        else if (queuedRequests.Count > 0 && !HasActiveScreen)
        {
            for (int index = queuedRequests.Count - 1; index >= 0; index--)
            {
                OpenScreenRequest screenRequest = queuedRequests[index];

                queuedRequests.RemoveAt(index);
                OpenScreen(screenRequest, true);
                break;
            }
        }
    }

    private void OpenScreen(OpenScreenRequest openScreenRequest, bool isOpenedFromQueue)
    {
        if (openScreenRequest.OpenType != UIScreen.ScreenOpenType.ADDITIVE)
        {
            while (activeScreens.Count > 0)
            {
                Type topScreenType = topScreen.GetType();
                
                topScreen.Close();
                
                if (openScreenRequest.OpenType == UIScreen.ScreenOpenType.CLOSE_QUEUE)
                {
                    Queue(topScreenType);
                }
            }
        }
        
        UIScreen uiScreen = GetOrLoadScreen(openScreenRequest.ScreenName);
        openScreenRequest.OnLoadingAction?.Invoke(uiScreen);
        activeScreens.Push(uiScreen);
        uiScreen.Initialize(this, isOpenedFromQueue);
        uiScreen.Open();
    }

    public UIScreen PopActiveScreen()
    {
        return activeScreens.Pop();
    }

    public void PushActiveScreen(UIScreen screen)
    {
        activeScreens.Push(screen);
    }
    
    public UIScreen Open(string screenName, UIScreen.ScreenOpenType? openType = null, Action<UIScreen> onLoadingAction = null)
    {
        UIScreen screen = GetOrLoadScreen(screenName);
        OpenScreenRequest openRequest = new OpenScreenRequest(screenName, openType ?? screen.OpenType, screen.ShouldOpenBackground,onLoadingAction);
        openRequests.Add(openRequest);
        return screen == null ? null : screen;
    }

    public UIScreen Open(UIScreen targetScreen, UIScreen.ScreenOpenType? openType = null, Action<UIScreen> onLoadingAction = null)
    {
        Type screenType = targetScreen.GetType();
        UIScreen screen = GetOrLoadScreen(screenType);
        OpenScreenRequest openRequest = new OpenScreenRequest(screenType.ToString(), openType ?? screen.OpenType, screen.ShouldOpenBackground, onLoadingAction);
        openRequests.Add(openRequest);
        return screen == null ? null : screen;
    }

    public TScreen Open<TScreen>(UIScreen.ScreenOpenType? openType = null,Action<UIScreen> onLoadingAction = null) where TScreen : UIScreen
    {
        Type screenType = typeof(TScreen);
        UIScreen screen = GetOrLoadScreen(screenType);
        OpenScreenRequest openRequest = new OpenScreenRequest(screenType.ToString(), openType ?? screen.OpenType, screen.ShouldOpenBackground, onLoadingAction);
        openRequests.Add(openRequest);
        return screen == null ? null : (TScreen)screen;
    }

    public void Queue<TScreen>(UIScreen.ScreenOpenType? openType = null, Action<UIScreen> onLoadingAction = null) where TScreen : UIScreen
    {
        Type screenType = typeof(TScreen);
        UIScreen screen = GetOrLoadScreen(screenType);
        OpenScreenRequest queueRequest = new OpenScreenRequest(screenType.ToString(), openType ?? screen.OpenType, screen.ShouldOpenBackground, onLoadingAction);
        openRequests.Add(queueRequest);
    }
    
    private void Queue(Type screenType, UIScreen.ScreenOpenType? openType = null, Action<UIScreen> onLoadingAction = null)
    {
        UIScreen screen = GetOrLoadScreen(screenType);
        OpenScreenRequest queueRequest = new OpenScreenRequest(screenType.ToString(), openType ?? screen.OpenType, screen.ShouldOpenBackground, onLoadingAction);
        openRequests.Add(queueRequest);
    }

    public void OnScreenOpen(UIScreen uiScreen)
    {
        if (!background.gameObject.activeInHierarchy && uiScreen.ShouldOpenBackground)
        {
            OpenBackground(0.15f, 0.85f, .15f);
        }
        
        int backgroundIndex = uiScreen.transform.GetSiblingIndex() - 1;
        
        background.transform.SetSiblingIndex(backgroundIndex);
    }

    public void OnScreenClose(UIScreen uiScreen)
    {
        activeScreens.Pop();
        
        if (activeScreens.Count == 0)
        {
            if (openRequests.Count == 0 && queuedRequests.Count == 0)
            {
                CloseBackground(0);
            }
         
            if (openRequests.Count > 0 && !openRequests[0].ShouldOpenBackground)
            {
               CloseBackground(0);
            }
            
            if (queuedRequests.Count > 0 &&  !queuedRequests[0].ShouldOpenBackground)
            {
                CloseBackground(0);
            }

        }

        if (topScreen)
        {
            background.transform.SetSiblingIndex(topScreen.transform.GetSiblingIndex());
        }
    }

    public void CloseAllScreen()
    {
        while (topScreen != null)
        {
            topScreen.Close();
        }
    }

    private UIScreen GetOrLoadScreen(string screenName)
    {
        foreach (UIScreen screen in loadedScreens)
        {
            if (screen.name == screenName)
            {
                return screen;
            }
        }
        
        UIScreen uiScreen = Instantiate(Resources.Load<UIScreen>(screenName), screenContainer);
        uiScreen.name = screenName;
        loadedScreens.Add(uiScreen);
        uiScreen.gameObject.SetActive(false);
        return uiScreen;
    }

    private UIScreen GetOrLoadScreen(Type screenType)
    {
        string screenName = screenType.ToString();

        foreach (UIScreen screen in loadedScreens)
        {
            if (screen.GetType() == screenType)
            {
                return screen;
            }
        }

        UIScreen uiScreen = Instantiate(Resources.Load<UIScreen>(screenName), screenContainer);
        uiScreen.name = screenName;
        loadedScreens.Add(uiScreen);
        uiScreen.gameObject.SetActive(false);
        return uiScreen;
    }

    public void RemoveFromLoadedScreen(UIScreen screen)
    {
        loadedScreens.Remove(screen);
    }
    
    public void CloseBackground()
    {
        CloseBackground(0);
    }

    public void OnClickBackground()
    {
        if(topScreen != null && topScreen.ShouldCloseOnBackgroundClick)
        {
            topScreen.Close();
        }
    }

    public void OpenBackground(float initialScale, float targetScale, float duration)
    {
        background.alpha = initialScale;
        background.gameObject.SetActive(true);
        background.DOFade(targetScale, duration);
    }

    private void CloseBackground(float duration)
    {
        background.DOFade(0, duration).OnComplete(()=> background.gameObject.SetActive(false));
    }
}

public class OpenScreenRequest : IEquatable<OpenScreenRequest>
{
    #region VARIABLES
    public readonly string ScreenName;
    public readonly UIScreen.ScreenOpenType OpenType;
    public readonly Action<UIScreen> OnLoadingAction;
    public readonly int RequestFrame;
    public readonly bool ShouldOpenBackground;
    public UIScreen OpenedScreen;
    public bool IsCompleted;
    #endregion

    public OpenScreenRequest(string screenName, UIScreen.ScreenOpenType openType, bool shouldOpenBackground,Action<UIScreen> onLoadingAction)
    {
        ScreenName = screenName;
        OpenType = openType;
        OnLoadingAction = onLoadingAction;
        ShouldOpenBackground = shouldOpenBackground;
        RequestFrame = Time.frameCount;

        OpenedScreen = null;
        IsCompleted = false;
    }

    public void Complete(UIScreen uiScreen)
    {
        IsCompleted = true;
        OpenedScreen = uiScreen;
    }

    public bool Equals(OpenScreenRequest other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ScreenName == other.ScreenName;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((OpenScreenRequest)obj);
    }

    public override int GetHashCode()
    {
        return (ScreenName != null ? ScreenName.GetHashCode() : 0);
    }
}