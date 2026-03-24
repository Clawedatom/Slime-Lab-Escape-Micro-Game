using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class TileManager : MonoBehaviour
{
    #region Class References
    private static TileManager _instance;
    #endregion

    #region Private Fields
    [Header("Game Grid Fields")]
    [SerializeField] private float gridHeight;
    [SerializeField] private float gridWidth;
    
    [SerializeField] private float resolution = 1f;
    [SerializeField]private float cellSize;
    private int width;
    private int height;
    private Vector3 worldOffset;
    private int cellCountX;
    private int cellCountY;
    [Header("Level Fields")]
    [SerializeField] private List<GameTile> gameTiles = new List<GameTile>();
    [SerializeField] private GameObject gameTilePrefab;
    [SerializeField] private GameTile endTile;
    [SerializeField] private GameTile playerSpawn;
    private float minDistForSpawnPoint;

    [SerializeField] private GameObject explosionPrefab;

    private List<GameTile> spawnTiles;

    [SerializeField] private Transform tileFolder;

    #endregion

    #region Properties
    public static TileManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<TileManager>();
                if (_instance == null)
                {
                    Debug.LogError("TileManager has not been assgined");
                }
            }
            return _instance;
        }
    }

    public float GetCellSize => cellSize;

    public Vector3 GetPlayerSpawnPos => playerSpawn.GetWorldPos;

    public Vector2Int GetGridSize
    {
        get
        {
            return new Vector2Int(Mathf.RoundToInt(gridWidth), Mathf.RoundToInt(gridHeight));
        }

    }

    public Vector2 GetCellCounts
    {
        get
        {
            return new Vector2(cellCountX, cellCountY);
        }
    }

    public GameTile GetEndTile => endTile;
    #endregion


    #region Start Up

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            _instance = null;
            return;
        }

        _instance = this;
    }
    public void OnStart()
    {
        SetGridValues();

        CreateFloorTiles();

        SetLevelPoints();
    }
    private void SetGridValues()
    {

        cellSize = 1f / resolution;

        width = Mathf.FloorToInt(gridWidth * resolution);
        height = Mathf.FloorToInt(gridHeight * resolution);

        cellCountX = Mathf.FloorToInt(gridWidth * resolution);
        cellCountY = Mathf.FloorToInt(gridHeight * resolution);

        float maxDist = Mathf.Sqrt(cellCountX * cellCountX + cellCountY * cellCountY);
        minDistForSpawnPoint = Mathf.Min(cellCountX * 0.75f, maxDist * 0.9f);


        float actualWidth = cellCountX * cellSize;
        float actualHeight = cellCountY * cellSize;

        worldOffset = transform.position;

    }
    private void CreateFloorTiles()
    {
        gameTiles = new List<GameTile>();
        for (int y = 0; y<cellCountY; y++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                Vector3 pos = new Vector3(cell.x * cellSize, cell.y * cellSize, 0);
                Vector3 worldPos = pos + worldOffset;
                GameTile tile = Instantiate(gameTilePrefab, worldPos, Quaternion.identity, tileFolder).GetComponent<GameTile>();
                tile.InitTile(cell, cellSize, worldPos);
                gameTiles.Add(tile);
            }

        }
    }

    private void SetLevelPoints()
    {
        SetEndPoint(); // any tile
        

        SetPlayerSpawn();

    }
    

    public void SetPlayerSpawn()
    {
        //get far left center point

        //int y = Mathf.FloorToInt(cellCountY / 2f);
        //int x = 0;
        //int index = GetIndexFromCell(new Vector2Int(x, y));

        //playerSpawn = gameTiles[index];

        playerSpawn = GetRandomEmptyTile();
        SetLevelCell(playerSpawn.GetCellPos, null, GameManager.Instance.GetGameData.GetPlayerSpawnSprite);

    }

   
    private void SetEndPoint()
    {
        ////far right
        //int y = Mathf.FloorToInt(cellCountY / 2f);
        //int x = Mathf.FloorToInt(cellCountX - 1);

        //Debug.Log("Y: " + y + " cell Count: " + cellCountY);
        //Debug.Log("X: " + x + " cell Count: " + cellCountX);

        //int index = GetIndexFromCell(new Vector2Int(x, y));
        //Debug.Log("Index: " + index);
        //endTile = gameTiles[index];
        endTile = GetRandomEmptyTile();
        SetLevelCell(endTile.GetCellPos, OnEndTileReached, GameManager.Instance.GetGameData.GetExitTiles[0]);

    }
    #endregion

    #region Class Methods
    public void HandleOpenExitTile()
    {
        endTile.UpdateTileSprite(GameManager.Instance.GetGameData.GetExitTiles[1]);
    }

    public void OnUpdate()
    {

    }

    public void OnDrawGizmos()
    {
        SetGridValues();
        //grid lines
        Gizmos.color = Color.black;

        // vertical lines
        Vector3 cellOffset = new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0);
        for (int i = 0; i <= width; i++)
        {
            float x = i * cellSize;
            Vector3 from = new Vector3(x, 0, 0);
            Vector3 to = new Vector3(x, gridHeight, 0);

            Gizmos.DrawLine(from + worldOffset - cellOffset, to + worldOffset - cellOffset);
        }

        // horizontal lines
        for (int i = 0; i <= height; i++)
        {
            float y = i * cellSize;
            Vector3 from = new Vector3(0, y, 0);
            Vector3 to = new Vector3(gridWidth, y, 0);

            Gizmos.DrawLine(from + worldOffset - cellOffset, to + worldOffset - cellOffset);
        }

    }
    
    private GameTile GetRandomTile()
    {
        int ran = Random.Range(0, gameTiles.Count - 1);

        return gameTiles[ran];
    }

    public GameTile GetRandomEmptyTile()
    {
        List<GameTile> emptyTiles = GetEmptyTiles();
        int ran = Random.Range(0, emptyTiles.Count - 1);


        return emptyTiles[ran];
    }

    

    public void OnTileInteract(Vector2Int cellPos)
    {
        int index = GetIndexFromCell(cellPos);

        if (index < 0 || index >= gameTiles.Count)
            return; //invalid tile
        gameTiles[index].OnTileInteract();
    }

    private List<GameTile> GetEmptyTiles()
    {
        List<GameTile> emptyTiles = new List<GameTile>();

        foreach (GameTile tile in gameTiles)
        {
            if (tile.IsEmpty) emptyTiles.Add(tile);
        }
        return emptyTiles;
    }
    #endregion



    #region Helpers

    public bool IsCellOccupied(Vector2Int cell)
    {
        int index = GetIndexFromCell(cell);

        return gameTiles[index].IsOccupied;
    }

    private int GetIndexFromCell(Vector2Int cellPos)
    {
        return cellPos.y * cellCountX + cellPos.x;
        
    }
 

    private void OnEndTileReached()
    {

        MicroGBase.Instance.OnMGCheckWin();
    }
    public bool IsInEndTile(Vector3 playerPos)
    {
        Vector2Int cellPos = WorldToCell(playerPos);

        return cellPos == endTile.GetCellPos;
        
           
        
    }
    public Vector3 CellToWorld(Vector2Int cell)
    {


        return new Vector3(cell.x * cellSize, cell.y * cellSize, 0) + worldOffset;
    }

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        Vector3 local = worldPos - worldOffset;

        int x = Mathf.FloorToInt(local.x / cellSize);
        int y = Mathf.FloorToInt(local.y / cellSize);

        x = Mathf.Clamp(x, 0, cellCountX - 1);
        y = Mathf.Clamp(y, 0, cellCountY - 1);

        return new Vector2Int(x, y);
    }

    public bool IsInCellBounds(Vector2Int cellPos)
    {
        int x = cellPos.x;
        int y = cellPos.y;

        if (x >= 0 && x < cellCountX)
            if (y >= 0 && y < cellCountY)
                return true;
        return false;
    }

    public void SetLevelCell(Vector2Int cellPos, Action tileAction, Sprite sprite)
    {
        int index = GetIndexFromCell(cellPos);

        gameTiles[index].SetLevelPoint();
        gameTiles[index].AddTileEvent(tileAction);
        gameTiles[index].UpdateTileSprite(sprite);

    }

    public void CreateExplosionOnTile(Vector2Int targetTile)
    {
        //explode

        Vector3 worldPos = CellToWorld(targetTile);

        GameObject expGO = Instantiate(explosionPrefab, worldPos, Quaternion.identity, transform);

        DealDamageToTile(targetTile);

        // DEAL DAMAGE
    }

    public void ResetGameTile(Vector2Int targetTile)
    {
        int index = GetIndexFromCell(targetTile);

        gameTiles[index].ClearTile();
    }

    public void DealDamageToTile(Vector2Int targetTile)
    {
        //if player is in tile, take damage

        Vector2Int playerPos = WorldToCell(PlayerManager.Instance.transform.position);

        if (playerPos == targetTile)
        {
            Debug.Log("Player taken damage");
            PlayerManager.Instance.OnPlayerHit(); 
        }
    }
    public void OnDestroy()
    {
       
        if (_instance == this)
            _instance = null;
    }
    
    #endregion
}
