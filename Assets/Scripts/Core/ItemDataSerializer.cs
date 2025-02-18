using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "Item Data", order = 0)]
public class ItemDataSerializer : ScriptableObject
{
    public List<ItemData> ItemData;
}

[Serializable]
public class ItemData
{
    public string ItemName;
    public OID OID;
    public Item Item;
    public int ItemLife;
    public string DestroyParticle;
    public Sprite ItemImage;
    public int WarmCount;
}
