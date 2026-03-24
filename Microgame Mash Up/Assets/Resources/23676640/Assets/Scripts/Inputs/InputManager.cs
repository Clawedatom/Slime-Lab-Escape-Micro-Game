using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Class References
    private static InputManager _instance;

    PlayerControls playerControls;
    #endregion

    #region Private Fields
    [Header("Input Fields")]
    [SerializeField] private Vector2 movementVector;
    #endregion

    #region Properties
    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<InputManager>();
                if (_instance == null)
                {
                    Debug.LogError("InputManager has not been assigned");
                }
            }
            return _instance;
        }
    }

    public float Horizontal => movementVector.x;
    public float Vertical => movementVector.y;
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

    }
    #endregion

    #region Class Methods
    public void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.started += ctx => OnMovementInput(ctx);
        }
        playerControls.Enable();
    }
    public void OnDisable()
    {
        playerControls.Disable();
    }
    private void OnMovementInput(InputAction.CallbackContext ctx)
    {
        movementVector = ctx.ReadValue<Vector2>();
        PlayerManager.Instance.Input_OnMovementInput(movementVector);
    }
    #endregion

    #region Update Methods
    public void OnUpdate()
    {

    }

    public void OnLateUpdate()
    {
        ResetInputs();
    }


    private void ResetInputs()
    {
        if (movementVector != Vector2.zero)
        {
            movementVector = Vector2.zero;
        }
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
