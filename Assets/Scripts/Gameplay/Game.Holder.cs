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
    private string MergeParticle = "Merge Particle";
    private bool isHolderFull;
    
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

        if (holderItems.Count == 0)
        {
            holderItems.Add(item);
        }
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
        
        if (holderItems.Count == slotCount && FindGroupData().Count == 0)
        {
            Main.Instance.SetInputEnable(false);
            isHolderFull = true;
        }
        
        UpdateItemTargets();
        BoardActionEnd();
    }

    private void UpdateHolderVisuals()
    {
        if (movingItems.Count == 0) return;

        List<Item> toRemove = new List<Item>();
        float delta = gameplayConfig.ItemCollectMovementSpeed * Time.deltaTime;

        for (var i = 0; i < movingItems.Count; i++)
        {
            var item = movingItems[i];
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

        for (var i = 0; i < toRemove.Count; i++)
        {
            var arrivedItem = toRemove[i];
            movingItems.Remove(arrivedItem);
            arrivedItems.Add(arrivedItem);
            OnItemArrived();
            BoardActionEnd();
        }
    }

    private void UpdateItemTargets()
    {
        int movingCount = movingItems.Count;

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
            
            BoardActionStart();
            itemTargets.Add(holderItems[i], targetPos);
            movingItems.Add(item);
        }

        for (int i = 0; i < movingCount; i++)
        {
            BoardActionEnd();
        }
    }

    private void OnItemArrived()
    {
         List<List<Item>> groups = FindGroupsVisual();
         if (groups.Count > 0)
         {
             RemoveGroups(groups);
             UpdateItemTargets();
         }
    }

    private List<List<Item>> FindGroupsVisual()
    {
        List<List<Item>> groups = new List<List<Item>>();
        List<Item> currentGroup = new List<Item>();

        for (var i = 0; i < holderItems.Count; i++)
        {
            var item = holderItems[i];
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

    private List<List<Item>> FindGroupData()
    {
        List<List<Item>> groups = new List<List<Item>>();
        List<Item> currentGroup = new List<Item>();

        for (var i = 0; i < holderItems.Count; i++)
        {
            var item = holderItems[i];
            
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
        for (var i = 0; i < groups.Count; i++)
        {
            MergeAnimation(groups, i);
        }
    }

    private async void MergeAnimation(List<List<Item>> groups, int i)
    {
        List<Item> group = groups[i];
        Vector3 centerPoint = group[1].transform.position;
        
        BoardActionStart();
        
        for (var j = 0; j < group.Count; j++)
        {
            var item = group[j];
            despawnItems.Add(item);
            item.transform.DOMove(centerPoint, gameplayConfig.ItemMergeDuration)
                .SetEase(gameplayConfig.ItemMergeEase).OnComplete(
                    () =>
                    {
                        holderItems.Remove(item);
                        despawnItems.Remove(item);
                        item.Despawn();
                        UpdateItemTargets();
                    });
        }

        await Yield.WaitForSeconds(gameplayConfig.ItemMergeDuration);
        
        VFXManager.Instance.Play(MergeParticle, centerPoint);

        await Yield.WaitForLateUpdate();
        BoardActionEnd();
    }
}