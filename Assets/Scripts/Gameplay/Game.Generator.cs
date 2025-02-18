using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class Game
{
    private Grid grid;

    private void CreateGrid()
    {
        LevelData levelData = level.LevelData;
        grid = new Grid();
        List<CellData> gridData = levelData.GridData;
        List<CellData> sortedGridData = gridData
            .OrderBy(cellData => cellData.Index.z)
            .ThenBy(cellData => cellData.Index.x)
            .ThenBy(cellData => cellData.Index.y)
            .ToList();

        for (int i = 0; i < sortedGridData.Count; i++)
        {
            CellData cellData = sortedGridData[i];
            Vector3Int index = cellData.Index;
            Vector2 worldPosition = cellData.WorldPosition;
            Cell cell = new Cell(worldPosition, index.z);
            Item gridItem = GetItem(cellData.OID);
            gridItem.transform.SetParent(gameContainer.transform);
            cell.SetCellItem(gridItem);
            grid.Cells.Add(index, cell);


            if (index.z + 1 > grid.LayerCount)
            {
                grid.LayerCount = index.z + 1;
            }

            SetBoardItemsAvailable(worldPosition, index.z, false);
        }
    }

    private Item GetItem(OID oid)
    {
        return levelManager.GameItemPoolFactory.GetItem(oid);
    }
}