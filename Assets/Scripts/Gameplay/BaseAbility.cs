using UnityEngine;
public abstract class BaseAbility : IAbility
{
    protected Item item;
    
    protected BaseAbility(Item item)
    {
        this.item = item;
    }
    
    public void Initialize()
    {
        OnInitialize();
    }

    protected virtual void OnInitialize()
    {
        
    }

    public virtual void Execute()
    {
        
    }

    public virtual void OnDespawn()
    {
        
    }
}