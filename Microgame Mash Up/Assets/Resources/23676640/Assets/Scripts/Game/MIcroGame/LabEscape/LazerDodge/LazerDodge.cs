using System.Collections.Generic;
using UnityEngine;

public class LaserDodge : MonoBehaviour
{

    [System.Serializable]
    public struct ShootPattern
    {
        public List<int> turretSequence; // indices of turrets to fire order
        public float timeBetweenShots;   // delay between each turret firing
    }


    TileManager tileManager;
    MicroGBase manager;

    [Header("Laser Turret Settings")]
    [SerializeField] private GameObject turretPrefab;
    [SerializeField] private List<LaserTurret> turrets = new List<LaserTurret>();
    [SerializeField] private Vector2 turretOffset;

    [Header("Pattern Settings")]
    [SerializeField] private List<ShootPattern> patterns = new List<ShootPattern>();
  
    private int sequenceIndex = 0;

    [SerializeField] private float timeBetweenShots;
    private float shotTimer = 0f;

    [SerializeField] private float timeBetweenPatterns = 1f;
    
    private float patternPauseTimer;

    [SerializeField] private float minTimeBetweenShots = 0.5f;
    [SerializeField] private float maxTimeBetweenShots = 3.0f;

    [SerializeField] private float minTimeBetweenPatterns = 0.5f;
    [SerializeField] private float maxTimeBetweenPatterns = 3.0f;

    public void OnLDInit(MicroGBase m)
    {
        tileManager = TileManager.Instance;
        manager = m;

        //create Lazers
        CreateLasers();

        SetInitVals();

    }

    private void SetInitVals()
    {
        patternPauseTimer = 0f;

        float diff = manager.GetDifficulty; //(1-5) 5 hardest
        float roundTime = manager.GetRoundDuration; // very hard -> 10s very easy -> 20s

        float t = diff / 5;
        timeBetweenShots = Mathf.Lerp(minTimeBetweenShots, maxTimeBetweenShots, t);
        timeBetweenPatterns = Mathf.Lerp(minTimeBetweenPatterns, maxTimeBetweenPatterns, t);
    }

    private void CreateLasers()
    {
        float cellY = tileManager.GetCellCounts.y;
        float cellX = tileManager.GetCellCounts.x;
        for (int i = 0; i < cellY ; i++)
        {
            Vector3 pos = tileManager.CellToWorld(new Vector2Int((int)cellX, i));
            pos.x -= tileManager.GetCellSize;
            LaserTurret turret = Instantiate(turretPrefab, pos + new Vector3(turretOffset.x, turretOffset.y,0), Quaternion.identity, transform).GetComponent<LaserTurret>();
            turret.OnTurretInit((int)GameMaster.GetDifficulty());
            turrets.Add(turret);
        }
    }


    public void OnLDUpdate()
    {
        float t = manager.GetElapsedTime;

        if (turrets.Count == 0 || patterns.Count == 0)
        {
            Debug.Log("No turrets or no patterns");
        }

        patternPauseTimer += Time.deltaTime;
        if (patternPauseTimer >= timeBetweenPatterns) // break between
        {
            ShootPattern pattern = GetRandomPattern();

            shotTimer += Time.deltaTime;

            if (shotTimer >= timeBetweenShots)
            {
                shotTimer = 0f;

                // get turret index from sequence
                int turretIndex = pattern.turretSequence[sequenceIndex];


                if (turretIndex >= 0 && turretIndex < turrets.Count)
                    turrets[turretIndex].ManualShoot();  //Shoot 
                else
                {
                    Debug.Log("Tried to shoot turret outside of range");
                }

                sequenceIndex++; //next turret

                //pattern finish chefck
                if (sequenceIndex >= pattern.turretSequence.Count -1 )
                {
                    sequenceIndex = 0;
                    
                    patternPauseTimer = 0.0f; // wait between patterns

                    
                }
            }
        }
        

        



        foreach (LaserTurret turret in turrets)
        {
            turret.OnTurretUpdate();
        }

    }

    private ShootPattern GetRandomPattern()
    {
        int ran = Random.Range(0, patterns.Count);

        return patterns[ran];
    }

    
}
