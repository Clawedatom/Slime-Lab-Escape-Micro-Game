using System.Collections.Generic;
using UnityEngine;

public class MG_LabEscape : MicroGBase
{
    TileManager tileManager;
    FallingBombsManager fallingObjects;
    LaserDodge laserDodge;

    [Header("Minigames  Key fields")]
    
    [SerializeField] private List<LabKey> labKeys;
    [SerializeField] private bool isExitLocked;

    private int keysRemaining;

    public GameTile GetEndTile => tileManager.GetEndTile;
    public override void OnMGAwake(GameManager gm)
    {
        base.OnMGAwake(gm);

        tileManager = TileManager.Instance;
        fallingObjects = GetComponentInChildren<FallingBombsManager>();
        laserDodge = GetComponentInChildren<LaserDodge>();

        isExitLocked = true;



    }

    public override void OnMGStart(float roundDuration, int difficulty)
    {
        base.OnMGStart(roundDuration, difficulty);
        fallingObjects.OnInit(this);

        laserDodge.OnLDInit(this);

        //place keys
        PlaceGameKeys();
        keysRemaining = labKeys.Count;


        
    }

    private void PlaceGameKeys()
    {
        foreach (LabKey key in labKeys)
        {
            //occupy random tile
            GameTile tile = tileManager.GetRandomEmptyTile();
            tileManager.SetLevelCell(tile.GetCellPos, () => OnKeyCollect(key), GameManager.Instance.GetGameData.GetDefaultSprite);

            tile.UpdateTileColour(Color.yellow);


            //move key
            Vector3 worldPos = tileManager.CellToWorld(tile.GetCellPos);
            key.transform.position = worldPos;

            key.KeyInit(tile.GetCellPos);

        }
    }

    public override void OnMGUpdate()
    {
        base.OnMGUpdate();
        if (!IsGameActive) return;
        //drop fallings

        fallingObjects.OnFBMUpdate();
        laserDodge.OnLDUpdate();

    }


    public override void OnMGEnd()
    {
        base.OnMGEnd();
        //
    }

    public void OnKeyCollect(LabKey key)
    {
        //
        keysRemaining--;
        tileManager.ResetGameTile(key.GetPos);
        key.DestroySelf();

        if (keysRemaining == 0)
        {
            UnlockExit();
        }
    }

    private void UnlockExit()
    {
        
        isExitLocked = false; // unlcok
        GetEndTile.UpdateTileSprite(GameManager.Instance.GetGameData.GetExitTiles[1]); // swap asset
    }

    public override void OnMGCheckWin()
    {
        base.OnMGCheckWin();

        if (!isExitLocked)
        {
            //win
            Debug.Log("Win");
            GameMaster.GameSucceeded();
        }
        else
        {
            Debug.Log("YOu need the keys!");
        }
    }
}