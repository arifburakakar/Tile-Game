using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class LevelEditor : MonoBehaviour
{
    public static LevelEditor Instance;
    public List<OID> OIDs;
    [SerializeField]
    private EditorTileHandler editorTilePrefab;
    [SerializeField]
    private TMP_Dropdown Dropdown;
    [NonSerialized]
    private ItemData itemData;
    private GameItemsConfig gameItemsConfig;
    private Camera camera;
    private Grid grid;
    private Dictionary<Vector3Int, EditorTileHandler> tileBoard;
    private Dictionary<int, GameObject> layers;
    private Vector3 mousePosition;
    private float cellSize = 1;
    private int currentLayer;
    
    public Grid Grid => grid;

    private void Awake()
    {
        Instance = this;
        gameItemsConfig = Resources.Load<GameItemsConfig>("GameItemsConfig");
    }

    private void Start()
    {
        camera = Camera.main;
        Dropdown.options.Clear();
        Dropdown.options = new List<TMP_Dropdown.OptionData>();
        OIDs = new List<OID>();
        
        List<ItemDataSerializer> itemSerializer = gameItemsConfig.ItemDataSerializers;

        foreach (ItemDataSerializer serializer in itemSerializer)
        {
            foreach (ItemData itemData in serializer.ItemData)
            {
                Dropdown.options.Add(new TMP_Dropdown.OptionData(itemData.ItemName, itemData.ItemImage));
                OIDs.Add(itemData.OID);
            }
        }
        
        Dropdown.options.Add(new TMP_Dropdown.OptionData("Null"));
        OIDs.Add(null);
        
        Dropdown.value = 0;
        OnItemValueChange();
        Dropdown.RefreshShownValue();
    }

    private void Update()
    {
        mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
        
        if (Input.GetMouseButton(0))
        {
            OnMouseLeftClick();
        }
        if (Input.GetMouseButton(1))
        {
            OnMouseRightClick();
        }
    }

    private void OnMouseLeftClick()
    {
        Vector3Int targetIndex = WorldPosToGridIndex(mousePosition);
        
        if (layers == null)
        {
            return;
        }
        
        if (tileBoard.TryGetValue(targetIndex, out EditorTileHandler tile))
        {
            tile.UpdateSprite(true, itemData?.ItemImage);
        
            if (!grid.Cells.ContainsKey(targetIndex))
            {
                Vector3 position = new Vector3(targetIndex.x + cellSize + (-.5f * (currentLayer % 2)), targetIndex.y + cellSize + (- .5f * (currentLayer % 2)));
                Cell cell = new Cell(position, currentLayer);
                grid.AddCell(targetIndex, cell);
            }

            grid.Cells[targetIndex].OID = itemData?.OID;
        }
    }

    private void OnMouseRightClick()
    {
        Vector3Int targetIndex = WorldPosToGridIndex(mousePosition);
        
        if (layers == null)
        {
            return;
        }
        
        if (tileBoard.TryGetValue(targetIndex, out EditorTileHandler tile))
        {
            tile.UpdateSprite(false, null);
        
            if (grid.Cells.ContainsKey(targetIndex))
            {
                grid.Cells[targetIndex].OID = null;
                grid.Cells.Remove(targetIndex);
            }

        }
    }
    
    public void OnItemValueChange()
    {
        OID oid = OIDs[Dropdown.value];
        if (oid == null)
        {
            itemData = null;
        }
        else
        {
            itemData = gameItemsConfig.GetItemData(oid);
        }
    }

    public void GenerateGrid(List<Vector2Int> boardSizes)
    {
        if (layers != null)
        {
            foreach (var layer in layers)
            {
                Destroy(layer.Value);
            }
        }


        layers = new Dictionary<int, GameObject>();
        tileBoard = new Dictionary<Vector3Int, EditorTileHandler>();
        grid = new Grid();
        
        for (int i = 0; i < boardSizes.Count; i++)
        {
            GameObject gridObject = new GameObject($"Grid {i}");
            layers.Add(i, gridObject);
            Vector2Int gridSize = boardSizes[i];
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3Int index = new Vector3Int(x, y, i);
                    Vector3 position = new Vector3(x + cellSize + (-.5f * (i % 2)), y + cellSize + (- .5f * (i % 2)));
                    EditorTileHandler tile = Instantiate(editorTilePrefab, position, Quaternion.identity, gridObject.transform);
                    tile.UpdateSortingLayer(i, gridSize.y - y);
                    tileBoard.Add(index, tile);
                }
            }
        }
       
        SetCameraToCenter();
    }

    public void RandomizeBoardWithGenerator(List<GeneratorsItemData> generatorsItemData, Vector2Int gridSize)
    {
        // if (generatorsItemData.Count == 0)
        // {
        //     Debug.Log("Generator Items Empty");
        //     return;
        // }
        // ResetGrid();
        // this.gridSize = gridSize;
        //
        // for (int x = 0; x < gridSize.x; x++)
        // {
        //     for (int y = 0; y < gridSize.y; y++)
        //     {
        //         Vector2Int index = new Vector2Int(x, y);
        //         Vector3 position = new Vector3(x + cellSize, y + cellSize);
        //         Cell cell = new Cell(position);
        //         cell.OID = generatorsItemData[Random.Range(0, generatorsItemData.Count)].OID;
        //         grid.AddCell(index, cell);
        //         EditorTileHandler tile = Instantiate(editorTilePrefab, position, Quaternion.identity, gridObject.transform);
        //         tile.UpdateSprite(gameItemsConfig.GetItemData(cell.OID).ItemImage);
        //         tiles.Add(index, tile);
        //     }
        // }
        //
        // SetCameraToCenter();
    }

    public void FillGrid(List<CellData> gridData)
    {
        // ResetGrid();
        //
        // foreach (CellData cellData in gridData)
        // {
        //     Vector2Int index = cellData.Index;
        //     Vector3 position = new Vector3(index.x + cellSize, index.y + cellSize);
        //     Cell cell = new Cell(position);
        //     cell.OID = cellData.OID;
        //     grid.AddCell(index, cell);
        //     EditorTileHandler tile = Instantiate(editorTilePrefab, position, Quaternion.identity, gridObject.transform);
        //     tile.UpdateSprite(gameItemsConfig.GetItemData(cell.OID).ItemImage);
        //     tiles.Add(index, tile);
        // }
        //
        // gridSize = gridData[^1].Index + Vector2Int.one;
        //
        // SetCameraToCenter();
    }

    private void ResetGrid()
    {

    }

    public void ChangeLayer(int layer)
    {
        foreach (KeyValuePair<int, GameObject> keys in layers)
        {
            if (keys.Key <= layer)
            {
                layers[keys.Key].SetActive(true);
            }
            else
            {
                layers[keys.Key].SetActive(false);
            }
        }

        currentLayer = layer;
    }
    
    private Vector3Int WorldPosToGridIndex(Vector3 worldPos)
    {
        float cellSize = 1f;
        float offset = .5f;
        
        int x = Mathf.FloorToInt((worldPos.x - (offset - .5f * (currentLayer % 2))) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - (offset - .5f * (currentLayer % 2))) / cellSize);
        
        return new Vector3Int(x, y, currentLayer);
    }

    private void SetCameraToCenter()
    {
        camera.transform.position = new Vector3(8 * 0.5f, 8 * 0.5f, -10) + Vector3.up * .5f + Vector3.right * .5f;
    }
}