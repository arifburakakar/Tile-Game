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
    [SerializeField] private EditorTileHandler editorTilePrefab;
    [SerializeField] private TMP_Dropdown Dropdown;
    [NonSerialized] private ItemData itemData;
    private GameItemsConfig gameItemsConfig;
    private Camera camera;
    private Grid grid;
    private List<Vector2Int> boardSizes;
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
                Vector2Int gridSize = boardSizes[targetIndex.z];
                float offsetX = (gridSize.x - 1) * cellSize * 0.5f;
                float offsetY = (gridSize.y - 1) * cellSize * 0.5f;

                float posX = targetIndex.x * cellSize - offsetX;
                float posY = targetIndex.y * cellSize - offsetY;
                Vector3 position = new Vector3(posX, posY, 0);
                
                Debug.Log($"{position} -- {tile.transform.position}");
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
        ResetGrid();

        for (int i = 0; i < boardSizes.Count; i++)
        {
            GameObject gridObject = new GameObject($"Grid {i}");
            layers.Add(i, gridObject);
            Vector2Int gridSize = boardSizes[i];

            float offsetX = (gridSize.x - 1) * cellSize * 0.5f;
            float offsetY = (gridSize.y - 1) * cellSize * 0.5f;

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3Int index = new Vector3Int(x, y, i);

                    float posX = x * cellSize - offsetX;
                    float posY = y * cellSize - offsetY;
                    Vector3 position = new Vector3(posX, posY, 0);

                    EditorTileHandler tile = Instantiate(editorTilePrefab, position, Quaternion.identity,
                        gridObject.transform);
                    tile.UpdateSortingLayer(i, gridSize.y - y);
                    tileBoard.Add(index, tile);
                }
            }
        }


        this.boardSizes = boardSizes;
        SetCameraToCenter();
    }

    public void FillGrid(List<CellData> gridData, List<Vector2Int> boardSizes)
    {
        ResetGrid();

        this.boardSizes = boardSizes;

        for (int i = 0; i < boardSizes.Count; i++)
        {
            GameObject gridObject = new GameObject($"Grid {i}");
            layers.Add(i, gridObject);
            Vector2Int gridSize = boardSizes[i];
            
            float offsetX = (gridSize.x - 1) * cellSize * 0.5f;
            float offsetY = (gridSize.y - 1) * cellSize * 0.5f;

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    Vector3Int index = new Vector3Int(x, y, i);
                    float posX = x * cellSize - offsetX;
                    float posY = y * cellSize - offsetY;
                    Vector3 position = new Vector3(posX, posY, 0);

                    EditorTileHandler tile = Instantiate(editorTilePrefab, position, Quaternion.identity,
                        gridObject.transform);
                    tile.UpdateSortingLayer(i, gridSize.y - y);
                    tileBoard.Add(index, tile);
                }
            }
        }

        foreach (CellData cellData in gridData)
        {
            OID oid = cellData.OID;
            Vector3Int index = cellData.Index;
            itemData = gameItemsConfig.GetItemData(oid);

            tileBoard[index].UpdateSprite(true, itemData.ItemImage);

            Vector3 position = cellData.WorldPosition;
            Cell cell = new Cell(position, currentLayer);
            grid.AddCell(index, cell);
            grid.Cells[index].OID = itemData.OID;
        }
    }

    private void ResetGrid()
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
        Vector2Int gridSize = boardSizes[currentLayer];
        
        float offsetX = (gridSize.x - 1) * cellSize * 0.5f;
        float offsetY = (gridSize.y - 1) * cellSize * 0.5f;
        float adjustedX = (worldPos.x + offsetX) / cellSize;
        float adjustedY = (worldPos.y + offsetY) / cellSize;
        int x = Mathf.RoundToInt(adjustedX);
        int y = Mathf.RoundToInt(adjustedY);

        return new Vector3Int(x, y, currentLayer);
    }

    private void SetCameraToCenter()
    {
        //camera.transform.position = new Vector3(8 * 0.5f, 8 * 0.5f, -10) + Vector3.up * .5f + Vector3.right * .5f;
    }
}