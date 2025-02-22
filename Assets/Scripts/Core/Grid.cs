using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public Dictionary<Vector3Int, Cell> Cells;
    public List<Vector2Int> BoardSizes;
    public int LayerCount = 0;

    public Grid()
    {
        Cells = new Dictionary<Vector3Int, Cell>();
    }

    public Grid(Dictionary<Vector3Int, Cell> cells)
    {
        Cells = cells;
    }

    public void AddCell(Vector3Int index, Cell cell)
    {
        Cells.Add(index, cell);
    }

    public Cell GetCell(Vector3Int index)
    {
        return Cells.GetValueOrDefault(index);
    }
}
public class Cell
{
    public OID OID;
    public Vector2 WorldPosition;
    public int Layer;
    public Item Item;
    public bool HasItem => Item;

    //public Vector2Int MasterCellIndex; useful for big items

    public Cell(Vector2 worldPosition, int layer)
    {
        WorldPosition = worldPosition;
        Layer = layer;
    }
    
    public void SetCellItem(Item item)
    {
        Item = item;
        item.SetCell(this);
    }
    
    public void RemoveCellItem()
    {
        Item.Cell = null;
        Item = null;
        OID = null;
    }


    public void Blast(BlastType blastType)
    {
        if (Item != null)
        {
            if (Item.TryGetAbility(out BlastAbility blastAbility))
            {
                blastAbility.Blast(blastType);
            }
        }
    }

}