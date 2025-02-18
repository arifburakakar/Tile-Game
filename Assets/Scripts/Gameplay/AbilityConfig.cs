using UnityEngine;

public abstract class AbilityConfig : ScriptableObject
{
    public abstract IAbility CreateAbility(Item item);
}


