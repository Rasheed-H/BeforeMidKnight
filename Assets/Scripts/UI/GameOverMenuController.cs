using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the game over menu, displaying the player's score, weeks survived, and a loss message.
/// Allows navigation back to the main menu and saves the score in the database.
/// </summary>
public class GameOverMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text scoreText;
    public TMP_Text weeksText;
    public TMP_Text loseMessageText;

    private int score;

    /// <summary>
    /// Initializes the game over menu by calculating the score and updating the UI elements.
    /// </summary>
    private void Start()
    {
        score = GameManager.Instance.totalCoinsAllTime * GameManager.Instance.currentWeek;
        UpdateLoseMessage();
        UpdateText();
    }

    /// <summary>
    /// Sets the loss message based on the game over condition (e.g., lives or weekly quota).
    /// </summary>
    private void UpdateLoseMessage()
    {
        if (GameManager.Instance.lives <= 0)
        {
            loseMessageText.text = "You ran out of lives!";
        }
        else
        {
            loseMessageText.text = "You failed to meet the weekly quota!";
        }
    }

    /// <summary>
    /// Updates the UI with the calculated score and weeks survived.
    /// </summary>
    private void UpdateText()
    {
        weeksText.text = "Weeks Survived: " + GameManager.Instance.currentWeek;
        scoreText.text = "Score: " + score;
    }

    /// <summary>
    /// Handles the main menu button click, saving the score to the database, resetting the game state,
    /// and loading the main menu scene.
    /// </summary>
    public void OnMainMenuClicked()
    {
        DbManager.Instance.InsertScore(GameManager.Instance.totalCoinsAllTime, GameManager.Instance.currentWeek, score);
        GameManager.Instance.ResetGameState();
        SceneManager.LoadScene("MainMenu");
    }
}
