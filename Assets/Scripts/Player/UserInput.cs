using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static UserInput Instance { get; private set; }

    public Vector2 MoveInput { get; private set; }
    public bool AttackUpInput { get; private set; }
    public bool AttackDownInput { get; private set; }
    public bool AttackLeftInput { get; private set; }
    public bool AttackRightInput { get; private set; }
    public bool SwitchWeaponInput { get; private set; }
    public bool DashInput { get; private set; }
    public bool PauseInput { get; private set; }

    private PlayerInput _playerInput;

    // Input Actions
    private InputAction _moveAction;
    private InputAction _attackUpAction;
    private InputAction _attackDownAction;
    private InputAction _attackLeftAction;
    private InputAction _attackRightAction;
    private InputAction _switchWeaponAction;
    private InputAction _dashAction;
    private InputAction _pauseAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _playerInput = GetComponent<PlayerInput>();
        SetupInputActions();
    }

    private void SetupInputActions()
    {
        _moveAction = _playerInput.actions["Move"];
        _attackUpAction = _playerInput.actions["AttackUp"];
        _attackDownAction = _playerInput.actions["AttackDown"];
        _attackLeftAction = _playerInput.actions["AttackLeft"];
        _attackRightAction = _playerInput.actions["AttackRight"];
        _switchWeaponAction = _playerInput.actions["SwitchWeapon"];
        _dashAction = _playerInput.actions["Dash"];
        _pauseAction = _playerInput.actions["Pause"];
    }

    private void Update()
    {
        UpdateInputs();
    }

    private void UpdateInputs()
    {
        // Movement
        MoveInput = _moveAction.ReadValue<Vector2>();

        // Attack Inputs
        AttackUpInput = _attackUpAction.WasPressedThisFrame();
        AttackDownInput = _attackDownAction.WasPressedThisFrame();
        AttackLeftInput = _attackLeftAction.WasPressedThisFrame();
        AttackRightInput = _attackRightAction.WasPressedThisFrame();

        // Switch Weapon, Dash, and Pause
        SwitchWeaponInput = _switchWeaponAction.WasPressedThisFrame();
        DashInput = _dashAction.WasPressedThisFrame();
        PauseInput = _pauseAction.WasPressedThisFrame();
    }
}
