using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Item : MonoBehaviour, IPoolable, IEquatable<Item>
{
    public OID OID;
    public SpriteRenderer GFX;
    public SpriteRenderer DisableRenderer;
    public Cell Cell;
    public BlastType MinBlastCap;
    public int Lives = 1;
    private int lives;
    public event Action OnLifeDecreaseEvent;
    [SerializeField]
    private SortingGroup sortingGroup;
    [SerializeField]
    private Animation animation;
    [SerializeField]
    private List<Abilities> selectedAbilities = new List<Abilities>();
    private int busyCounter = 0;
    private List<IAbility> abilities;
    private readonly string uniqueID = Guid.NewGuid().ToString();
    public bool IsAvailable => busyCounter == 0;
    public bool HasLives => Lives != 0;
    public int ObjectID => OID.ObjectID;
    public int VariantID => OID.VariantID;

    public void Create()
    {
        CreateAbilities();
    }

    public void Spawn()
    {
        InitializeAbilities();
    }

    
    // abilityleri scriptable objectile ekle 
    private void CreateAbilities()
    {
        abilities = new List<IAbility>();
        foreach (Abilities ability in selectedAbilities)
        {
            switch (ability)
            {
                case Abilities.SELECT_ABILITY:
                    abilities.Add(new SelectAbility(this));
                    break;
                case Abilities.BLAST_ABILITY:
                    abilities.Add(new BlastAbility(this));
                    break;
            }
        }
    }

    private void InitializeAbilities()
    {
        foreach (IAbility ability in abilities)
        {
            ability.Initialize();
        }
    }
    
    public bool TryGetAbility<T>(out T ability) where T : IAbility
    {
        foreach (IAbility a in abilities)
        {
            if (a is T matchedAbility)
            {
                ability = matchedAbility;
                return true;
            }
        }

        ability = default;
        return false;
    }


    private void ResetAbilities()
    {
        
    }

    public void ToggleAvailability(bool toggle)
    {
        busyCounter += toggle ? -1 : 1;
        DisableRenderer.gameObject.SetActive(busyCounter != 0);
    }

    public void UpdateSortingGroup(int targetSortingOrder)
    {
        sortingGroup.sortingOrder = targetSortingOrder;
    }

    public void SetCell(Cell cell)
    {
        Cell = cell;
        transform.position = cell.WorldPosition;
        UpdateSortingGroup((cell.Layer + 1) * 10);
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

        // if (DestroyPartilce != "")
        // {
        //     VFXManager.Instance.Play(DestroyPartilce, transform.position);
        // }
        //
        // if (ExplodeSound != "")
        // {
        //     SFXManager.Instance.Play(ExplodeSound);
        // }
        
        Despawn();
        
        activeLevel.Game.BoardActionEnd();
    }
    
    protected virtual void OnLifeDecrease(BlastType blastType)
    {
        OnLifeDecreaseEvent?.Invoke();
    }



    public void Despawn()
    {
        ResetAbilities();
        Cell.RemoveCellItem();
        LevelManager.Instance.PoolHandler.ReleaseItem(this);
        busyCounter = 0;
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
