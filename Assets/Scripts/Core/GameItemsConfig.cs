using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameItemsConfig", fileName = "GameItemsConfig", order = 0)]
public class GameItemsConfig : ScriptableObject
{
    public List<ItemDataSerializer> ItemDataSerializers;

    
    public ItemData GetItemData(OID oid)
    {
        int objectID = oid.ObjectID;
        int variantID = oid.VariantID;

        foreach (ItemDataSerializer serializer in ItemDataSerializers)
        {
            foreach (ItemData itemData in serializer.ItemData)
            {
                if (objectID == itemData.OID.ObjectID && variantID == itemData.OID.VariantID)
                {
                    return itemData;
                }
            }
        }
        ItemData item = new ItemData();
        return item;
    }
}