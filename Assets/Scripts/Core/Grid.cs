using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public Dictionary<Vector3Int, Cell> Cells;
    public int LayerCount = 0;

    public Grid(int layerCount)
    {
        Cells = new Dictionary<Vector3Int, Cell>();
        LayerCount = layerCount;
    }

    public Grid(Dictionary<Vector3Int, Cell> cells, int layerCount)
    {
        Cells = cells;
        LayerCount = layerCount;
    }

    public void AddCell(Vector3Int index, Cell cell)
    {
        Cells.Add(index, cell);
    }

    public Cell GetCell(int layer, Vector2Int index)
    {
        
        // en ustte varsa onu returnler her layerda get cell index degisebilir cunku her layerda center degisecek .5 offset degisecek 
        
        // en yukari layerdan asagiya dogru bakarak in her layer degisiminde 
        // if (Cells[layer].TryGetValue(index, out Cell cell))
        // {
        //     return cell;
        // }
        //
        return null;
    }

    public Vector2Int GetCellIndex(Vector2 position, float cellSize)
    {
        int x = Mathf.FloorToInt((position.x - cellSize * .5f) / cellSize);
        int y = Mathf.FloorToInt((position.y - cellSize * .5f) / cellSize);
        
        Vector2Int index = new Vector2Int(x, y);

        return index;
    }
}
public class Cell
{
    public OID OID;
    public Vector2 WorldPosition;
    public int layer;
    public Item Item;
    public bool HasItem => Item;

    //public Vector2Int MasterCellIndex; useful for big items

    public Cell(Vector2 worldPosition, int layer)
    {
        WorldPosition = worldPosition;
        this.layer = layer;
    }
    
    public void SetCellItem(Item item)
    {
        item.SetCell(this);
    }
    
    public void RemoveCellItem()
    {
        Item = null;
        OID = null;
    }

}