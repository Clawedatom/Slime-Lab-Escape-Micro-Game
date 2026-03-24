using System;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    Action tileInteract;
  
    [SerializeField] SpriteRenderer tileRenderer;
    [SerializeField] private Vector2Int cellPos;
    private Vector3 worldPos;
    [SerializeField] private bool isLevelPoint;
    [SerializeField] private bool isOccupied;
    public Vector2Int GetCellPos => cellPos;
    public bool IsOccupied => isOccupied;

    public bool IsLevelPoint => isLevelPoint;
    public Vector3 GetWorldPos => worldPos;

    public bool IsEmpty
    {
        get
        {
            return !isLevelPoint && !isOccupied;
        }
    }

    public void InitTile(Vector2Int cell, float cellSize, Vector3 world)
    {

        transform.localScale = new Vector3(cellSize , cellSize , 1f);
        worldPos = world;
        cellPos = cell;
        isOccupied = false;
        isLevelPoint = false;

    }

    
    public void SetLevelPoint() // spawnpoint/endpoint
    {
        isLevelPoint = true;


        
    }

    public void UpdateTileSprite(Sprite newSprite)
    {
        tileRenderer.sprite = newSprite;
    }
    public void UpdateTileColour(Color colour)
    {
        tileRenderer.color = colour;
    }

    public void AddTileEvent(Action eventAction)
    {
        tileInteract += eventAction;
        
    }

    public void OnTileInteract()
    {
        tileInteract?.Invoke();
    }

    public void ClearTile()
    {
        UpdateTileColour(Color.white);
        isLevelPoint = false;
        isOccupied = false;

        tileInteract = null;
        
    }

    public void OnDestroy()
    {
        tileInteract = null;
    }
    //manage colours here
}
