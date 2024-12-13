using UnityEngine;
using TMPro;
using System.Collections.Generic;
using static DbManager;

/// <summary>
/// Manages the Scores/Stats UI, including displaying score entries, aggregated statistics,
/// and handling tab navigation between Scores and Stats views.
/// </summary>
public class ScoresStatsController : MonoBehaviour
{
    public GameObject runEntryPrefab;
    public Transform contentPanel;
    public GameObject ScoreUI;
    public GameObject StatsUI;
    public GameObject scoreStatsUI; 

    public TMP_Text highscoreText;
    public TMP_Text recordsText;
    public TMP_Text totalKillsText;
    public TMP_Text goblinKillsText;
    public TMP_Text skeletonKillsText;
    public TMP_Text ghastKillsText;
    public TMP_Text wizardKillsText;
    public TMP_Text demonKillsText;

    /// <summary>
    /// Handles the button click to open the Scores/Stats UI, and displays the scores by default.
    /// </summary>
    public void OnScoresStatsButtonClicked()
    {
        scoreStatsUI.SetActive(true); 
        DisplayScores(); 
        DisplayStats();
        OnScoresTabClicked(); 
    }

    /// <summary>
    /// Populates the scores list with entries from the database.
    /// </summary>
    public void DisplayScores()
    {

        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }


        List<RunData> runs = DbManager.Instance.GetAllRuns();

        foreach (RunData run in runs)
        {
            GameObject newEntry = Instantiate(runEntryPrefab, contentPanel);

            TMP_Text idText = newEntry.transform.Find("IDContainer/IDText").GetComponent<TMP_Text>();
            TMP_Text weekText = newEntry.transform.Find("RunContainer/WeekText").GetComponent<TMP_Text>();
            TMP_Text scoreText = newEntry.transform.Find("RunContainer/ScoreText").GetComponent<TMP_Text>();
            TMP_Text statsText = newEntry.transform.Find("RunContainer/StatsText").GetComponent<TMP_Text>();
            TMP_Text coinsText = newEntry.transform.Find("RunContainer/CoinsText").GetComponent<TMP_Text>();
            TMP_Text dateText = newEntry.transform.Find("RunContainer/DateText").GetComponent<TMP_Text>();

            idText.text = $"{run.Id}";
            weekText.text = $"Week: {run.Week}";
            scoreText.text = $"{run.Score}";
            statsText.text = $"Escapes: {run.Escapes}\nDeaths: {run.Deaths}\nKills: {run.Kills}";
            coinsText.text = $"x {run.TotalCoinsDeposited}";
            dateText.text = System.DateTime.Parse(run.Date).ToString("MM/dd/yyyy");
        }
    }

    /// <summary>
    /// Populates the stats display with aggregated statistics from the database.
    /// </summary>
    public void DisplayStats()
    {

        var stats = DbManager.Instance.GetStats();

        highscoreText.text = $"{stats.HighestScore}";
        recordsText.text = $"Highest Week: {stats.HighestWeek}\n" +
                           $"Total Days: {stats.TotalDays}\n" +
                           $"Escapes: {stats.TotalEscapes}\n" +
                           $"Deaths: {stats.TotalDeaths}\n" +
                           $"Coins Deposited: {stats.TotalCoinsDeposited}\n" +
                           $"Coins Lost: {stats.TotalCoinsLost}";

        totalKillsText.text = $"Total: {stats.TotalKills}";
        goblinKillsText.text = $"x{stats.TotalGoblinKills}";
        skeletonKillsText.text = $"x{stats.TotalSkeletonKills}";
        ghastKillsText.text = $"x{stats.TotalGhastKills}";
        wizardKillsText.text = $"x{stats.TotalWizardKills}";
        demonKillsText.text = $"x{stats.TotalDemonKills}";
    }

    /// <summary>
    /// Shows the Score UI and hides the Stats UI.
    /// </summary>
    public void OnScoresTabClicked()
    {
        ScoreUI.SetActive(true);
        StatsUI.SetActive(false);
    }

    /// <summary>
    /// Shows the Stats UI and hides the Score UI.
    /// </summary>
    public void OnStatsTabClicked()
    {
        StatsUI.SetActive(true);
        ScoreUI.SetActive(false);
    }

    /// <summary>
    /// Closes the UI.
    /// </summary>
    public void OnCloseButtonClicked()
    {
        scoreStatsUI.SetActive(false);
    }
}
