using System.Collections.Generic;
using UnityEngine;

public class PoolHandler
{
    public Dictionary<OID, GenericObjectPool<Item>> itemPools;

    private Transform poolContainer;
    private GameItemsConfig gameItemsConfig;
    private GameplayConfig gameplayConfig;

    public PoolHandler()
    {
        itemPools = new Dictionary<OID, GenericObjectPool<Item>>();
    }
    
    // reduce every 5 level end
    public void Initialize(GameItemsConfig gameItemsConfig)
    {
        gameplayConfig = GameManager.Instance.GameplayConfig;
        this.gameItemsConfig = gameItemsConfig;
        poolContainer = new GameObject("Pool Container").transform;

        foreach (ItemDataSerializer itemDataSerializer in gameItemsConfig.ItemDataSerializers)
        {
            foreach (ItemData itemData in itemDataSerializer.ItemData)
            {
                GenericObjectPool<Item> pool = new GenericObjectPool<Item>
                (
                    itemData.Item,
                    itemData.WarmCount,
                    poolContainer
                );

                itemPools.Add(itemData.OID, pool);
                pool.CreateInitialPoolObjects();
            }
        }
    }

    private GenericObjectPool<Item> GetItemPool(OID oid)
    {
        return itemPools[oid];
    }

    public Item GetItem(OID oid)
    {
        return GetItemPool(oid).Get();
    }

    public void ReleaseItem(Item item)
    {
        itemPools[item.OID].Release(item);
    }
    
    public void Clear()
    {
        foreach (var pools in itemPools.Values)
        {
            pools.ReleaseAll();
        }
    }
}