using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorTileHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer placedObject;
    [SerializeField] private SpriteRenderer targetSprite;
    [SerializeField] private SpriteRenderer background;
    
    public void  UpdateSprite(bool toggle, Sprite sprite)
    {
        if (!toggle)
        {
            background.gameObject.SetActive(true);
            targetSprite.sprite = null;
            targetSprite.gameObject.SetActive(false);
            placedObject.gameObject.SetActive(false);
        }
        else
        {
            background.gameObject.SetActive(false);
            placedObject.gameObject.SetActive(true);
            if (targetSprite)
            {
                targetSprite.gameObject.SetActive(true);
                targetSprite.sprite = sprite;
            }
        }
    }

    public void UpdateSortingLayer(int layer, int y)
    {
        placedObject.sortingOrder = y + layer * 10;
        targetSprite.sortingOrder = y + 2 + layer * 10;
        background.sortingOrder = y + 1 + layer * 10;

    }
}
