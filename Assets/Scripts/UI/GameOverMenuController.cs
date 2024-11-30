using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the game over menu, displaying the player's score, weeks survived, kills, escapes, deaths,
/// and coins deposited. Allows navigation back to the main menu and saves the run data in the database.
/// </summary>
public class GameOverMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text scoreText;
    public TMP_Text weeksText;
    public TMP_Text escapesText;
    public TMP_Text deathsText;
    public TMP_Text coinDepositedText;

    private int score;
    private int totalKills;

    /// <summary>
    /// Initializes the game over menu by calculating the score and total kills, then updating the UI elements.
    /// </summary>
    private void Start()
    {
        // Calculate score based on total coins deposited and current week
        score = GameManager.Instance.totalCoinsDeposited * GameManager.Instance.currentWeek;

        // Calculate total kills from various enemies
        totalKills = GameManager.Instance.goblinKills + GameManager.Instance.skeletonKills +
                     GameManager.Instance.spiderKills + GameManager.Instance.wizardKills + GameManager.Instance.bossKills;

        UpdateText();
    }

    /// <summary>
    /// Updates the UI with values from GameManager and calculated score and kills.
    /// </summary>
    private void UpdateText()
    {
        weeksText.text = "Weeks Survived: " + GameManager.Instance.currentWeek;
        scoreText.text = ""+score;
        escapesText.text = "Escapes: " + GameManager.Instance.escapes;
        deathsText.text = "Deaths: " + GameManager.Instance.deaths;
        coinDepositedText.text = "x " + GameManager.Instance.totalCoinsDeposited;
    }

    /// <summary>
    /// Handles the main menu button click, saving the run and stats to the database,
    /// resetting the game state, and loading the main menu scene.
    /// </summary>
    public void OnMainMenuClicked()
    {
        // Insert the run data into the Runs table
        DbManager.Instance.InsertRun(
            score,
            GameManager.Instance.currentWeek,
            GameManager.Instance.deaths,
            GameManager.Instance.escapes,
            GameManager.Instance.totalCoinsDeposited,
            totalKills
        );

        // Update cumulative and highest stats in the Stats table
        DbManager.Instance.UpdateStats(
            score,
            GameManager.Instance.currentWeek,
            GameManager.Instance.totalDays,
            GameManager.Instance.totalCoinsDeposited,
            GameManager.Instance.totalCoinsLost,
            GameManager.Instance.deaths,
            GameManager.Instance.escapes,
            totalKills,
            GameManager.Instance.goblinKills,
            GameManager.Instance.skeletonKills,
            GameManager.Instance.spiderKills,
            GameManager.Instance.wizardKills,
            GameManager.Instance.bossKills
        );

        // Reset the game state and return to the main menu
        GameManager.Instance.ResetGameState();
        SceneManager.LoadScene("MainMenu");
    }
}
