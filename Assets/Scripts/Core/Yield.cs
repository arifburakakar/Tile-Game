using System;
using Cysharp.Threading.Tasks;

public static class Yield
{
    public static UniTask WaitForSeconds(float seconds)
    {
        return UniTask.Delay(TimeSpan.FromSeconds(seconds));
    }
    public static YieldAwaitable WaitForUpdate()
    {
        return UniTask.Yield(PlayerLoopTiming.Update);
    }
    
    public static YieldAwaitable WaitForLateUpdate()
    {
        return UniTask.Yield(PlayerLoopTiming.PreLateUpdate);
    }

    public static YieldAwaitable WaitForFixedUpdate()
    {
        return UniTask.WaitForFixedUpdate();
    }

    public static YieldAwaitable WaitForFrame()
    {
        return UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
    }
}