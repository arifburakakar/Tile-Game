using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Item : MonoBehaviour, IPoolable, IEquatable<Item>
{
    public OID OID;
    public SpriteRenderer GFX;
    public string OnSetCellAnimation;
    public int ObjectID => OID.ObjectID;
    public int VariantID => OID.VariantID;
    public Cell Cell;
    
    [SerializeField]
    private SortingGroup sortingGroup;
    private Animation animation;
    private List<IAbility> abilities;
    private readonly string uniqueID = Guid.NewGuid().ToString();    

    public void Create()
    {
        CreateAbilities();
    }

    public void Spawn()
    {
        InitializeAbilities();
    }

    private void CreateAbilities()
    {
        
    }

    private void InitializeAbilities()
    {
        
    }

    private void ResetAbilities()
    {
        
    }

    public void UpdateSortingGroup(int targetSortingOrder)
    {
        sortingGroup.sortingOrder = targetSortingOrder;
    }

    public void SetCell(Cell cell)
    {
        Cell = cell;
    }

    public void Despawn()
    {
        ResetAbilities();
        Cell.RemoveCellItem();
        Cell = null;
        LevelManager.Instance.PoolHandler.ReleaseItem(this);
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
