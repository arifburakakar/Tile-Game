using System.Collections.Generic;
using UnityEngine;

public partial class Game
{
    private Grid grid;
    
    private void CreateGrid()
    {
        LevelData levelData = level.LevelData;
        grid = new Grid(3); // temp

        for (int i = 0; i < levelData.GridData.Count; i++)
        {
            CellData cellData = levelData.GridData[i];
            
            Vector3Int index = cellData.Index;
            Vector2 worldPosition = cellData.WorldPosition;
            Cell cell = new Cell(worldPosition, index.z);
            Item gridItem = GetItem(cellData.OID);
            gridItem.transform.position = worldPosition;
            gridItem.UpdateSortingGroup(index.z * 10);
            gridItem.transform.SetParent(gameContainer.transform);
            cell.SetCellItem(gridItem);  
            grid.Cells.Add(index, cell);
        }
    }
    

    private Item GetItem(OID oid)
    {
        return levelManager.PoolHandler.GetItem(oid);
    }
}