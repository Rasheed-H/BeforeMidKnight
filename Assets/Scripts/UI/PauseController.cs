using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// Manages the pause functionality, including pausing and resuming the game,
/// displaying the pause menu, and controlling player input during pause.
/// </summary>
public class PauseController : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Player player;
    private bool isPaused = false;

    private PlayerInputActions playerInputActions;
    [SerializeField] private AudioClip buttonClickSound;

    /// <summary>
    /// Initializes input actions for detecting pause inputs.
    /// </summary>
    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    /// <summary>
    /// Enables the pause input action.
    /// </summary>
    private void OnEnable()
    {
        playerInputActions.UI.Pause.performed += OnPausePressed;
        playerInputActions.UI.Enable();
    }

    /// <summary>
    /// Disables the pause input action.
    /// </summary>
    private void OnDisable()
    {
        playerInputActions.UI.Pause.performed -= OnPausePressed;
        playerInputActions.UI.Disable();
    }

    /// <summary>
    /// Toggles the game's pause state when the pause action is performed.
    /// </summary>
    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    /// <summary>
    /// Pauses the game, displaying the pause menu and disabling player input.
    /// </summary>
    private void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        UserInput.Instance?.EnableInputs(false);
    }

    /// <summary>
    /// Resumes the game, hiding the pause menu and re-enabling player input.
    /// </summary>
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        UserInput.Instance?.EnableInputs(true);
    }

    /// <summary>
    /// Loads the main menu scene, resetting the time scale to normal.
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }


    public void PlayButtonClickSound()
    {
        SoundEffects.Instance.PlaySound(buttonClickSound);
    }
}
