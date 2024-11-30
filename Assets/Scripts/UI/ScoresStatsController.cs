using UnityEngine;
using TMPro;
using System.Collections.Generic;
using static DbManager;

public class ScoresStatsController : MonoBehaviour
{
    // UI References
    public GameObject runEntryPrefab;
    public Transform contentPanel;
    public GameObject ScoreUI;
    public GameObject StatsUI;
    public TMP_Text statsText; // Reference to the text box in StatsUI for displaying stats
    public GameObject scoreStatsUI; // Reference to the parent Score/Stats UI

    // Text fields for individual stats in StatsUI
    public TMP_Text highscoreText;
    public TMP_Text recordsText;
    public TMP_Text totalKillsText;
    public TMP_Text goblinKillsText;
    public TMP_Text skeletonKillsText;
    public TMP_Text spiderKillsText;
    public TMP_Text wizardKillsText;
    public TMP_Text bossKillsText;

    /// <summary>
    /// Handles the button click to open the Scores/Stats UI, and displays the scores by default.
    /// </summary>
    public void OnScoresStatsButtonClicked()
    {
        scoreStatsUI.SetActive(true); // Enable the ScoreStats UI
        DisplayScores(); // Show scores by default
        DisplayStats();
        OnScoresTabClicked(); // Set Scores tab as active
    }

    /// <summary>
    /// Populates the scores list with entries from the database.
    /// </summary>
    public void DisplayScores()
    {
        // Clear existing entries in the content area
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // Retrieve run data from the database
        List<RunData> runs = DbManager.Instance.GetAllRuns();

        foreach (RunData run in runs)
        {
            GameObject newEntry = Instantiate(runEntryPrefab, contentPanel);

            // Find and assign text components for each field in the entry
            TMP_Text idText = newEntry.transform.Find("IDContainer/IDText").GetComponent<TMP_Text>();
            TMP_Text weekText = newEntry.transform.Find("RunContainer/WeekText").GetComponent<TMP_Text>();
            TMP_Text scoreText = newEntry.transform.Find("RunContainer/ScoreText").GetComponent<TMP_Text>();
            TMP_Text statsText = newEntry.transform.Find("RunContainer/StatsText").GetComponent<TMP_Text>();
            TMP_Text coinsText = newEntry.transform.Find("RunContainer/CoinsText").GetComponent<TMP_Text>();
            TMP_Text dateText = newEntry.transform.Find("RunContainer/DateText").GetComponent<TMP_Text>();

            // Populate each field with data from the current run record
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
        // Retrieve stats data from the database
        var stats = DbManager.Instance.GetStats();

        // Assign values to individual text fields
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
        spiderKillsText.text = $"x{stats.TotalSpiderKills}";
        wizardKillsText.text = $"x{stats.TotalWizardKills}";
        bossKillsText.text = $"x{stats.TotalBossKills}";
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
