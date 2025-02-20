using UnityEngine;

public partial class Game
{
    private Vector2[] bottomDirections = new Vector2[] { Vector2.up + Vector2.right, Vector2.down + Vector2.right, Vector2.up + Vector2.left, Vector2.down + Vector2.left};
    private Vector2[] sideDirections = new Vector2[] { Vector2.left, Vector2.right, Vector2.up, Vector2.down};

    private Cell selectedCell;
    
    private void TrySelectItem(Vector2 inputPosition)
    {
        selectedCell = null;
        Cell targetCell = null;
        for (int i = grid.LayerCount; i >= 0; i--)
        {
            Vector3Int targetIndex = WorldPosToGridIndex(inputPosition, i);
            if (grid.Cells.TryGetValue(targetIndex, out Cell cell) && cell.HasItem)
            {
                targetCell = cell;
                break;
            }
        }
        
        if (targetCell == null)
        {
            return;
        }
        
        Item selectedItem = targetCell.Item;
        if (!selectedItem.IsAvailable)
        {
            return;
        }
        
        BoardActionStart();
        
        if(selectedItem.TryGetAbility(out SelectAbility selectAbility) && selectAbility.Selectable)
        {
            selectedCell = targetCell;
            selectAbility.Execute();
        }
        
        BoardActionEnd();
    }
    
    private void TryCollectItem(Vector2 inputPosition)
    {
        if (selectedCell == null)
        {
            return;
        }
        
        Cell targetCell = null;
        for (int i = grid.LayerCount; i >= 0; i--)
        {
            Vector3Int targetIndex = WorldPosToGridIndex(inputPosition, i);
            if (grid.Cells.TryGetValue(targetIndex, out Cell cell) && cell.HasItem)
            {
                targetCell = cell;
                break;
            }
        }
        
        
        Item selectedItem = selectedCell.Item;

        if (targetCell == null || targetCell != selectedCell)
        {
            if(selectedItem.TryGetAbility(out SelectAbility selectAbility) && selectAbility.Selectable)
            {
                selectAbility.Deselect();
            }
            
            selectedCell = null;
            return;
        }
        
        BoardActionStart();
        
        if(selectedItem.TryGetAbility(out SelectAbility targetAbility) && targetAbility.Selectable)
        {
            targetAbility.Collect();
        }

        selectedCell.RemoveCellItem();
        SetItemToHolder(selectedItem);
        SetBoardItemsAvailable(inputPosition, selectedCell.Layer - 1, true);
        TrySideBlast(targetCell, selectedCell.Layer);
        
        BoardActionEnd();
    }
    
    private void TrySideBlast(Cell targetCell, int currentLayer)
    {
        for (int i = 0; i < sideDirections.Length; i++)
        {
            Vector2 targetPosition = targetCell.WorldPosition + sideDirections[i];
            Vector3Int targetIndex = WorldPosToGridIndex(targetPosition, currentLayer);
            Cell sideCell = grid.GetCell(targetIndex);
            
            if (sideCell == null)
            {
                return;
            }

            bool previousSideCellHasItem = sideCell.HasItem;
            sideCell.Blast(BlastType.SIDE);
            if (previousSideCellHasItem != sideCell.HasItem)
            {
                SetBoardItemsAvailable(sideCell.WorldPosition, sideCell.Layer - 1, true);
            }
        }
    }

    private void SetBoardItemsAvailable(Vector2 worldPosition, int layer, bool toggle)
    {
        for (int j = 0; j < bottomDirections.Length; j++)
        {
            if (layer == -1)
            {
                continue;
            }
            
            Vector2 targetPosition = worldPosition + bottomDirections[j] * .5f;
            Vector3Int targetIndex = WorldPosToGridIndex(targetPosition, layer);

            if (grid.Cells.TryGetValue(targetIndex, out Cell bottomCell) && bottomCell.HasItem)
            {
                bottomCell.Item.ToggleAvailability(toggle);
            }
        }
    }
    
    private Vector3Int WorldPosToGridIndex(Vector2 worldPos, int layer)
    {
        Vector2Int gridSize = grid.BoardSizes[layer];
        
        float offsetX = (gridSize.x - 1) * 0.5f;
        float offsetY = (gridSize.y - 1) * 0.5f;
        float adjustedX = (worldPos.x + offsetX);
        float adjustedY = (worldPos.y + offsetY);
        int x = Mathf.RoundToInt(adjustedX);
        int y = Mathf.RoundToInt(adjustedY);

        return new Vector3Int(x, y, layer);
    }

    public bool IsBoardClear()
    {
        foreach (Cell cell in grid.Cells.Values)
        {
            if (cell.HasItem)
            {
                return false;
            }
        }

        return true;
    }
}
