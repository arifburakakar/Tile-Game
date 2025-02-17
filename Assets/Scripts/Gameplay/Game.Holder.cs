using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public partial class Game
{
    private List<Item> holderItems;
    private Vector2 leftPosition;
    private int slotCount = 7;

    private void InitializeHolder()
    {
        holderItems = new List<Item>();
        leftPosition = gameContainer.HolderPoints[0].position;
    }

    private async void PlaceItemToHolder(Item item)
    {
        BoardActionStart();
        int targetIndex = 0;

        if (holderItems.Count == 0)
        {
            holderItems.Add(item);
        }
        else
        {
            for (int i = 0; i < holderItems.Count; i++)
            {
                Item holderItem = holderItems[i];
                if (holderItem.OID.Equals(item.OID))
                {
                    targetIndex = i;
                }
            }

            if (targetIndex != 0)
            {
                holderItems.Add(item);
            }
            else
            {
                holderItems.Insert(targetIndex + 1, item);
            }
        }
        
        // fix later
        for (int i = 0; i < holderItems.Count; i++)
        {
            Vector2 targetPosition = leftPosition + i * Vector2.right;
            item.DOKill();
            await item.transform.DOMove(targetPosition, gameplayConfig.ItemCollectMovementDuration)
                .SetEase(gameplayConfig.ItemCollectMovementEase).SetSpeedBased().AsyncWaitForCompletion();
        }
        
      
        BoardActionEnd();
    }


    private bool CanPlaceItemToHolder(Item item)
    {
        return true;
    }
}