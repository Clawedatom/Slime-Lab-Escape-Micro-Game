using UnityEngine;

public class MicroGBase : MonoBehaviour
{
    private static MicroGBase _instance;

    public static MicroGBase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<MicroGBase>();
                if (_instance == null)
                {
                    Debug.LogError("Micro Game as not been set up properly");
                }
            }
            return _instance;
        }
    }
    GameManager gameManager;
    protected TileManager gridManager;

    [Header("Base Microgame Fields")]
    [SerializeField] protected int difficulty;
   
    [SerializeField]protected float roundDuration;//max duration a round is

    public float GetElapsedTime => GameMaster.GetTimeElapsed();

    public int GetDifficulty => difficulty;
    public float GetRoundDuration => roundDuration;
    public bool IsGameActive // shortcut ref to GameManager - protected
    {
        get
        {
            return gameManager.HasStarted; 
        }
    }
    public virtual void OnMGAwake(GameManager gm)
    {
        gameManager = gm;
        gridManager = TileManager.Instance;
    }

    public virtual void OnMGStart(float dura, int diff)
    {
        roundDuration = dura;
        difficulty = diff;

    }

    public virtual void OnMGUpdate()
    {
        if (!IsGameActive) return; // dont run microgame logic
        
    }

    public virtual void OnMGEnd()
    {
        //generic end logic
        
    }

    public virtual void OnMGWin()
    {
        //player win logic
    }
    public virtual void OnMGLose()
    {
        //player lose logic
    }

    public virtual void OnMGCheckWin()
    {

    }

    #region Helpes
    public void OnDestroy()
    {
        _instance = null;
        
    }
    #endregion

}
