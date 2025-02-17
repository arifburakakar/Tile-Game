using System;
using UnityEngine;

public class BlastAbility : BaseAbility
{
    public int BlastBlockLevel = 0;
    public BlastType MinBlastCap;
    public Action OnBlastAction;
    
    public BlastAbility(Item item) : base(item)
    {
        
    }
    
    public void Blast(BlastType blastType)
    {
        if ((int)blastType < (int)item.MinBlastCap)
        {
            return;
        }
        
        if (BlastBlockLevel > 0)
        {
            return;
        }
        
        item.DecreaseLife(1, blastType);
        OnBlastAction?.Invoke();
        
        Debug.Log(blastType);
    }
}

public enum BlastType
{
    NONE,
    SIDE,
    TAP,
}
