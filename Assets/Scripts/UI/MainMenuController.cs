using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the main menu options, handling new game, continue, and quit functionality.
/// Manages the active game state for the continue option and displays a warning for new game resets.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text continueButtonText;
    [SerializeField] private Color disabledButtonTextColor;
    [SerializeField] private GameObject newGameWarningPopup;

    /// <summary>
    /// Initializes the main menu, checking the game state to set up the Continue button accordingly.
    /// </summary>
    private void Start()
    {
        GameManager.Instance.LoadGameState();

        if (GameManager.Instance.isActive)
        {
            continueButton.interactable = true;
            continueButtonText.color = Color.white;
        }
        else
        {
            continueButton.interactable = false;
            continueButtonText.color = disabledButtonTextColor;
        }
    }

    /// <summary>
    /// Handles the New Game button click, showing a warning popup if there is an active game.
    /// </summary>
    public void OnNewGameButtonClicked()
    {
        if (GameManager.Instance.isActive)
        {
            newGameWarningPopup.SetActive(true);
        }
        else
        {
            StartNewGame();
        }
    }

    /// <summary>
    /// Starts a new game by resetting the game state and loading the game menu scene.
    /// </summary>
    public void StartNewGame()
    {
        GameManager.Instance.ResetGameState();
        SceneManager.LoadScene("GameMenu");
    }

    /// <summary>
    /// Confirms starting a new game, hides the warning popup, and resets the game.
    /// </summary>
    public void OnConfirmNewGame()
    {
        newGameWarningPopup.SetActive(false);
        StartNewGame();
    }

    /// <summary>
    /// Cancels starting a new game, hiding the warning popup.
    /// </summary>
    public void OnCancelNewGame()
    {
        newGameWarningPopup.SetActive(false);
    }

    /// <summary>
    /// Handles the Continue button click, loading the game menu if an active game exists.
    /// </summary>
    public void OnContinueButtonClicked()
    {
        if (GameManager.Instance.isActive)
        {
            SceneManager.LoadScene("GameMenu");
        }
    }

    /// <summary>
    /// Exits the game when the Quit button is clicked.
    /// </summary>
    public void OnQuitButtonClicked()
    {
        Application.Quit();
        Debug.Log("Game Quit!");
    }
}
