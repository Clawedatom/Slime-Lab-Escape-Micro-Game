using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class FallingBombsManager : MonoBehaviour // every x seconds y amount of bombs are dropped on tiles
{
    TileManager tileManager;
    MicroGBase manager;

    [Header("Falling Bomb Fields")]
    
    [SerializeField] int baseFallingBombs = 2;
    [SerializeField] private int bombCount; // Bombss per wave
    [SerializeField] private float fallWaveInterval; // time between wave
    [SerializeField] private float nextObjWave;
    [SerializeField] private int waveCount;
    
    
    [Header("   ---Falling Bomb Fields---   ")]
    [SerializeField] private FallingBomb fallingBombPrefab;

    [SerializeField] private float fallDuration; // higher difficulty means shorter fall time

    private List<FallingBomb> activeFallings = new List<FallingBomb>();

    [Header("Falling Bomb Multipliers")]
    [SerializeField] private float countMultiplier = 1f;
    [SerializeField] private float intervalMultiplier = 1f;
    [SerializeField] private float fallDurationMultiplier = 1f;
    [SerializeField] private float growthMultiplier = 1f;

    [Header("Other Fields")]
    [SerializeField] private Transform bombFolder;
    

    //ROUND START

    //wait 

    public void OnInit(MicroGBase m)
    {
        tileManager = TileManager.Instance;

        manager = m;
        
        SetInitialVals();
    }
    

    private void SetInitialVals()
    {

        int difficulty = manager.GetDifficulty;
        float roundDuration = manager.GetRoundDuration;
        //falling object count - increased by difficulty
        bombCount = baseFallingBombs + Mathf.RoundToInt(Mathf.Log(difficulty + 1, 1.5f) * 4);

        bombCount = Mathf.Max(1, Mathf.RoundToInt(bombCount * countMultiplier));

        // wave interal  - decrease by diff
        fallWaveInterval = Mathf.Lerp(3.5f, 1f, difficulty / 10f);
        fallWaveInterval *= intervalMultiplier;

        // first wave 
        nextObjWave = fallWaveInterval;

        // Fall Duration - decrased by difficulty
        fallDuration = Mathf.Lerp(2.5f, 0.4f, difficulty / 10f);
        fallDuration *= fallDurationMultiplier;

        // Wave Count -  useless?
        waveCount = Mathf.FloorToInt(roundDuration / fallWaveInterval); 
    }

    public void OnFBMUpdate()
    {

        if (manager.GetElapsedTime >= nextObjWave)
        {
            //spawn falling wave


            //set neew values
            OnFallingWave();
        }


        if (activeFallings.Count > 0)
        {
            List<FallingBomb> landed = new List<FallingBomb>();
            foreach (FallingBomb obj in activeFallings)
            {
                bool hasLanded = obj.OnFBUpdate();
                if (hasLanded) landed.Add(obj);
            }

            foreach (FallingBomb obj in landed)
            {
                activeFallings.Remove(obj); // stop updating 
                obj.OnFBExplode(); // blow up bomb
            }


            landed.Clear();
        }

    }

    private void OnFallingWave()
    {
        //spawn x bombs
        for (int i = 0; i < bombCount; i++)
        {
            Vector2Int cellPos = tileManager.GetRandomEmptyTile().GetCellPos;

            CreateFallingObj(cellPos);
        }
        //resest vals

        nextObjWave += fallWaveInterval;
        bombCount = Mathf.RoundToInt(bombCount * growthMultiplier);
    }

    private void CreateFallingObj(Vector2Int cellPos)
    {
        Vector3 worldPos = tileManager.CellToWorld(cellPos);

        FallingBomb obj = Instantiate(fallingBombPrefab, worldPos, Quaternion.identity, bombFolder);
        float cell = tileManager.GetCellSize;
        //Vector3 scale = new Vector3(cell - (cell * 0.5f), cell - (cell * 0.5f), 1);
        Vector3 scale = cell * Vector3.one;
        scale.z = 1;
        obj.InitFallingBomb(cellPos, worldPos, fallDuration, scale);

        activeFallings.Add(obj);
    }

    
}
