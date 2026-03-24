using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	#region Class References
	private static PlayerManager _instance;

    PlayerMovement playerMovement;
    PlayerAnimation playerAnimation;

    #endregion

    #region Private Fields
    [Header("Player Bools")]
    [SerializeField] private bool isStunned;
    [SerializeField] private bool canMove;

    [Header("Stun Fields")]
    [SerializeField] private float stunTimer;
    [SerializeField] private float stunDuration = 1.5f;
    #endregion

    #region Properties
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PlayerManager>();
                if (_instance == null )
                {
                    Debug.LogError("PlayerManager has not been assigned");
                }
            }
            return _instance;
        }
    }

    public bool IsStunned => isStunned;
    public bool CanMove => canMove;

    public PlayerAnimation GetAnim => playerAnimation;
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
        Debug.Log("Start Palayer");
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerMovement.OnStart(Instance);
        playerAnimation.OnStart();

       
        SetVals();
    }

    private void SetVals()
    {
        isStunned = false;
        canMove = true;
    }


    public void SetStartPos(Vector3 startPos)
    {
        transform.position = startPos;
        Vector2 cellPos = TileManager.Instance.WorldToCell(startPos);

        Vector2Int cell = new Vector2Int(Mathf.FloorToInt(cellPos.x), Mathf.FloorToInt(cellPos.y));
        playerMovement.SetCellPos(cell);
    }
    #endregion

    #region Class Methods
    public void OnPlayerHit()
    {
        StunPlayer();   
    }


    private void StunPlayer()
    {
        playerAnimation.ToggleStunSprite(true);
        canMove = false;
        isStunned = true;
        stunTimer = 0;
    }

    private void UnStunPlayer()
    {
        playerAnimation.ToggleStunSprite(false);
        canMove = true;
        isStunned = false;
        stunTimer = 0;  
    }
    #endregion

    #region Update Methods
    public void OnUpdate()
    {
        if (isStunned)
        {
            stunTimer += Time.deltaTime;

            if (stunTimer >= stunDuration)
            {
                UnStunPlayer();
            }
            return;
        }

        if (CanMove)
        {
            playerMovement.OnUpdate();
        }
        
    }

    #endregion

    #region Input Methods
    public void Input_OnMovementInput(Vector2 movementVector)
    {
        if (!canMove) return;
        Vector2Int cell = new Vector2Int(Mathf.FloorToInt(movementVector.x), Mathf.FloorToInt(movementVector.y));
        playerMovement.ApplyMovement(cell);
        playerAnimation.UpdateFace(movementVector);
    }
    #endregion

    #region Helpes
    public void OnDestroy()
    {
        if (_instance == this)
            _instance = null;

    }
    #endregion
}
