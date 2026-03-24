using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Class References
    PlayerManager manager;
    TileManager tileManager;
    #endregion

    #region Private Fields
    [Header("Movement Fields")]
    [SerializeField] private Vector2Int currentGridPos;

    [SerializeField] private bool moveActive;

    [SerializeField] private float moveSpeed = 1.0f;

    [SerializeField] private bool isMoving;

    Vector2Int moveDirection;
    Vector3 startPos;
    Vector3 targetPos;
    float t;
    #endregion

    #region Properties
    
    #endregion

    #region Start Up
    public void OnStart(PlayerManager m)
    {
        manager = m;
        tileManager = TileManager.Instance;
        
        moveActive = false;
    }
    #endregion

    #region Class Methods
    public void SetCellPos(Vector2Int cellPos)
    {
        currentGridPos = cellPos;
    }
    public void ApplyMovement(Vector2Int movementVector)
    {
        if (!tileManager.IsInCellBounds(currentGridPos + movementVector) || moveActive || tileManager.IsCellOccupied(currentGridPos + movementVector)) return;
        currentGridPos += movementVector;
        moveDirection = movementVector;
        MovePlayer();
    }

    private void MovePlayer()
    {
        startPos = transform.position;
        targetPos = tileManager.CellToWorld(currentGridPos);
        moveActive = true;
        t = 0;
    }
    #endregion

    #region Update Methods
    public void OnUpdate()
    {
        if (moveActive)
        {
            t += Time.deltaTime * moveSpeed;
            t = Mathf.Clamp01(t);

            float smoothT = Mathf.SmoothStep(0, 1, t);
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, smoothT);
            transform.position = newPos;

            manager.GetAnim.Anim_OnMovement(smoothT, moveDirection);
            if (t >= 1 && moveActive)
            {
                StopMovement();
            }
        }
    }

    private void StopMovement()
    {
        moveDirection = Vector2Int.zero;

        //transform.position = targetPos;
        moveActive = false;
        t = 0;

        manager.GetAnim.ResetScale();
        transform.position = targetPos;
        tileManager.OnTileInteract(currentGridPos);
    }
        #endregion
    }
