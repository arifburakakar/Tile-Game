using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public partial class Game
{
    private List<Item> holderItems;
    private List<Item> movingItems = new List<Item>();
    private List<Item> arrivedItems = new List<Item>();
    private List<Item> despawnItems = new List<Item>();
    private Dictionary<Item, Vector2> itemTargets = new Dictionary<Item, Vector2>();
    private Vector2 leftPosition;
    private int slotCount = 7;
    
    private void InitializeHolder()
    {
        holderItems = new List<Item>();
        leftPosition = gameContainer.HolderPoints[0].position;
    }

    private void SetItemToHolder(Item item)
    {
        BoardActionStart();
        
        int targetIndex = -1;
        int count = 0;
        
        if (holderItems.Count == 0) holderItems.Add(item);
        else
        {
            for (int i = 0; i < holderItems.Count; i++)
            {
                if (holderItems[i].OID.Equals(item.OID))
                {
                    targetIndex = i;
                    count++;
                }
            }

            if (targetIndex == -1 || count == 3)
            {
                holderItems.Add(item);
            }
            else
            {
                holderItems.Insert(targetIndex + 1, item);
            }
        }

        UpdateItemTargets();
        BoardActionEnd();
    }

    private void UpdateHolderVisuals()
    {
        if (movingItems.Count == 0) return;

        List<Item> toRemove = new List<Item>();
        float delta = gameplayConfig.ItemCollectMovementSpeed * Time.deltaTime;

        foreach (Item item in movingItems)
        {
            if (!itemTargets.TryGetValue(item, out Vector2 target))
            {
                continue;
            }

            item.transform.position = Vector2.MoveTowards(
                item.transform.position, 
                target, 
                delta
            );

            if (Vector2.Distance(item.transform.position, target) < 0.05f)
            {
                item.transform.position = target;
                toRemove.Add(item);
            }
        }

        foreach (Item arrivedItem in toRemove)
        {
            movingItems.Remove(arrivedItem);
            arrivedItems.Add(arrivedItem);
            OnItemArrived();
        }
    }

    private void UpdateItemTargets()
    {
        itemTargets.Clear();
        movingItems.Clear();
        arrivedItems.Clear();
        
        for (int i = 0; i < holderItems.Count; i++)
        {
            Vector2 targetPos = leftPosition + Vector2.right * i;
            Item item = holderItems[i];

            if (despawnItems.Contains(item))
            {
                continue;
            }
            
            itemTargets.Add(holderItems[i], targetPos);
            movingItems.Add(item);
        }
    }

    private void OnItemArrived()
    {
         List<List<Item>> groups = FindGroups();
         if (groups.Count > 0)
         {
             RemoveGroups(groups);
             UpdateItemTargets();
         }
    }

    private List<List<Item>> FindGroups()
    {
        List<List<Item>> groups = new List<List<Item>>();
        List<Item> currentGroup = new List<Item>();

        foreach (Item item in holderItems)
        {
            if (!arrivedItems.Contains(item))
            {
                currentGroup.Clear();
                continue;
            }
            
            if (currentGroup.Count == 0 || item.OID.Equals(currentGroup[0].OID))
            {
                currentGroup.Add(item);
            }
            else
            {
                currentGroup = new List<Item> { item };
            }

            if (currentGroup.Count == 3)
            {
                groups.Add(new List<Item>(currentGroup));
                currentGroup.Clear();
            }
        }
        return groups;
    }

    private void RemoveGroups(List<List<Item>> groups)
    {
        foreach (List<Item> group in groups)
        {
            Vector3 centerPoint = group[1].transform.position;

            foreach (Item item in group)
            {
                despawnItems.Add(item);
                item.transform.DOMove(centerPoint, gameplayConfig.ItemMergeDuration).SetEase(gameplayConfig.ItemMergeEase).OnComplete(
                    () =>
                    {
                        holderItems.Remove(item);
                        despawnItems.Remove(item);
                        item.Despawn();
                        UpdateItemTargets();
                    });
            }
        }
    }
}