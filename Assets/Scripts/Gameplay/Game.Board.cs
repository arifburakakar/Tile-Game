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
        SetBoardItemsAvailable(inputPosition, selectedCell.Layer, true);
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

            sideCell.Blast(BlastType.SIDE);
        }
    }

    private void SetBoardItemsAvailable(Vector2 worldPosition, int layer, bool toggle)
    {
        for (int j = 0; j < bottomDirections.Length; j++)
        {
            Vector2 targetPosition = worldPosition + bottomDirections[j] * .5f;
            Vector3Int targetIndex = WorldPosToGridIndex(targetPosition, layer - 1);

            if (grid.Cells.TryGetValue(targetIndex, out Cell bottomCell) && bottomCell.HasItem)
            {
                bottomCell.Item.ToggleAvailability(toggle);
            }
        }
    }
    
    private Vector3Int WorldPosToGridIndex(Vector2 worldPos, int layer)
    {
        float cellSize = 1f;
        float offset = .5f;
        
        int x = Mathf.FloorToInt((worldPos.x - (offset - .5f * (layer % 2))) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - (offset - .5f * (layer % 2))) / cellSize);
        
        return new Vector3Int(x, y, layer);
    }

    public bool IsBoardClear()
    {
        bool isBoardClear = true;

        foreach (Cell cell in grid.Cells.Values)
        {
            if (cell.HasItem)
            {
                isBoardClear = false;
                break;
            }
        }
        
        return isBoardClear;
    }
}

/*
 * level editor fixes
 * new levels and tiles
 */