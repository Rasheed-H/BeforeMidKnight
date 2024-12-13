using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Handles user input using Unity's new input system, providing movement, attack, weapon switch,
/// dash, and pause actions as properties for other scripts to access.
/// </summary>
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

    private bool _isInputEnabled = true;

    // Input Actions
    private InputAction _moveAction;
    private InputAction _attackUpAction;
    private InputAction _attackDownAction;
    private InputAction _attackLeftAction;
    private InputAction _attackRightAction;
    private InputAction _switchWeaponAction;
    private InputAction _dashAction;
    private InputAction _pauseAction;

    /// <summary>
    /// Ensures a single instance of the UserInput class and initializes the input system by setting up input actions.
    /// </summary>
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

    /// <summary>
    /// Configures the input actions by mapping them to the player's input action asset.
    /// </summary>
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

    /// <summary>
    /// Updates the input values every frame if input is enabled; otherwise, resets all inputs.
    /// </summary>
    private void Update()
    {
        if (_isInputEnabled)
        {
            UpdateInputs();
        }
        else
        {
            ResetInputs(); 
        }
    }

    /// <summary>
    /// Resets all input values to their default states, effectively disabling all player actions.
    /// </summary>
    private void ResetInputs()
    {
        MoveInput = Vector2.zero;
        AttackUpInput = false;
        AttackDownInput = false;
        AttackLeftInput = false;
        AttackRightInput = false;
        SwitchWeaponInput = false;
        DashInput = false;
        PauseInput = false;
    }

    /// <summary>
    /// Reads and updates input values for movement, attack directions, weapon switching, dashing, and pausing.
    /// </summary>
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

    /// <summary>
    /// Enables or disables user inputs based on the provided boolean value.
    /// </summary>
    public void EnableInputs(bool isEnabled)
    {
        _isInputEnabled = isEnabled;
    }
}
