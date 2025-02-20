using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class Item : MonoBehaviour, IPoolable, IEquatable<Item>
{
    public OID OID;
    public int Lives = 1;
    public SpriteRenderer GFX;
    public SpriteRenderer DisableRenderer;
    public Cell Cell;
    private int lives;
    [SerializeField] 
    private string DestroyPartilce;
    [SerializeField] 
    private SortingGroup sortingGroup;
    [SerializeField] 
    private float fadeAnimationDuration;
    [SerializeField] 
    private float targetFadeAmount;
    [SerializeField] 
    private List<AbilityConfig> abilityConfigs;
    private List<IAbility> abilities;
    private int busyCounter = 0;
    private readonly string uniqueID = Guid.NewGuid().ToString();
    public bool IsAvailable => busyCounter == 0;
    public bool HasLives => Lives != 0;
    public int ObjectID => OID.ObjectID;
    public int VariantID => OID.VariantID;

    public void Create()
    {
        lives = Lives;
        CreateAbilities();
    }

    public void Spawn()
    {
        Lives = lives;
        InitializeAbilities();
    }
    
    private void CreateAbilities()
    {
        abilities = new List<IAbility>();
        foreach (AbilityConfig config in abilityConfigs)
        {
            abilities.Add(config.CreateAbility(this));
        }
    }

    private void InitializeAbilities()
    {
        foreach (IAbility ability in abilities)
        {
            ability.Initialize();
        }
    }

    public bool TryGetAbility<T>(out T  targetAbility) where T : IAbility
    {
        for (var i = 0; i < abilities.Count; i++)
        {
            var ability = abilities[i];
            if (ability is T matchedAbility)
            {
                targetAbility = matchedAbility;
                return true;
            }
        }

        targetAbility = default;
        return false;
    }

    private void ResetAbilities()
    {
        foreach (IAbility ability in abilities)
        {
            ability.OnDespawn();
        }
    }

    public void ToggleAvailability(bool toggle)
    {
        int prevCount = busyCounter;
        busyCounter += toggle ? -1 : 1;

        if (busyCounter != 0 && prevCount == 0)
        {
            DisableRenderer.DOFade(targetFadeAmount, fadeAnimationDuration);
        }
        else if (busyCounter == 0 && prevCount != 0)
        {
            DisableRenderer.DOFade(0, fadeAnimationDuration);
        }
    }

    protected void SetSortingGroup(int targetSortingOrder)
    {
        sortingGroup.sortingOrder = targetSortingOrder;
    }

    public void UpdateSortingGroup(int amount)
    {
        sortingGroup.sortingOrder += amount;
    }

    public void SetCell(Cell cell)
    {
        Cell = cell;
        transform.position = cell.WorldPosition;
        SetSortingGroup((cell.Layer + 1) * 10);
    }

    public void DecreaseLife(int decreaseAmount, BlastType blastType)
    {
        Lives -= decreaseAmount;
        OnLifeDecrease(blastType);
        if (!HasLives)
        {
            OnLifeFinished(blastType);
        }
    }

    protected virtual void OnLifeFinished(BlastType blastType)
    {
        LevelManager levelManager = LevelManager.Instance;

        Level activeLevel = levelManager.ActiveLevel;
        activeLevel.Game.BoardActionStart();
        
        if (DestroyPartilce != "")
        {
            VFXManager.Instance.Play(DestroyPartilce, transform.position);
        }

        Despawn();

        activeLevel.Game.BoardActionEnd();
    }

    protected virtual void OnLifeDecrease(BlastType blastType)
    {
        
    }

    public void Despawn()
    {
        ResetAbilities();

        if (Cell != null)
        {
            Cell.RemoveCellItem();
        }

        LevelManager.Instance.GameItemPoolFactory.ReleaseItem(this);
        busyCounter = 0;
        DisableRenderer.DOFade(0, 0);
    }

    public virtual void OnDespawn()
    {
    }

    public bool Equals(Item other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && uniqueID == other.uniqueID;
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Item)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), uniqueID);
    }
}