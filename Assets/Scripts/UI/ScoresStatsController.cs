using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

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
    /// Populates the scores list with entries from the server.
    /// </summary>
    public void DisplayScores()
    {
        
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        DbManager.Instance.GetRuns(runs =>
        {
            var i = runs.Count+1;
            foreach (var run in runs)
            {
                
                GameObject newEntry = Instantiate(runEntryPrefab, contentPanel);

                TMP_Text idText = newEntry.transform.Find("IDContainer/IDText").GetComponent<TMP_Text>();
                TMP_Text weekText = newEntry.transform.Find("RunContainer/WeekText").GetComponent<TMP_Text>();
                TMP_Text scoreText = newEntry.transform.Find("RunContainer/ScoreText").GetComponent<TMP_Text>();
                TMP_Text statsText = newEntry.transform.Find("RunContainer/StatsText").GetComponent<TMP_Text>();
                TMP_Text coinsText = newEntry.transform.Find("RunContainer/CoinsText").GetComponent<TMP_Text>();
                TMP_Text dateText = newEntry.transform.Find("RunContainer/DateText").GetComponent<TMP_Text>();
                
                i--;
                idText.text = $"{i}";
                weekText.text = $"Week: {run.week}";
                scoreText.text = $"{run.score}";
                statsText.text = $"Escapes: {run.escapes}\nDeaths: {run.deaths}\nKills: {run.kills}";
                coinsText.text = $"x {run.total_coins_deposited}";
                dateText.text = System.DateTime.Parse(run.date).ToString("MM/dd/yyyy");
            }
        });
    }

    /// <summary>
    /// Populates the stats display with aggregated statistics from the server.
    /// </summary>
    public void DisplayStats()
    {
        DbManager.Instance.GetStats(stats =>
        {
            if (stats == null)
            {
                Debug.LogError("Invalid stats data received.");
                return;
            }

            highscoreText.text = $"{stats.highest_score}";
            recordsText.text = $"Highest Week: {stats.highest_week}\n" +
                               $"Total Days: {stats.total_days}\n" +
                               $"Escapes: {stats.total_escapes}\n" +
                               $"Deaths: {stats.total_deaths}\n" +
                               $"Coins Deposited: {stats.total_coins_deposited}\n" +
                               $"Coins Lost: {stats.total_coins_lost}";

            totalKillsText.text = $"Total: {stats.total_kills}";
            goblinKillsText.text = $"x{stats.total_goblin_kills}";
            skeletonKillsText.text = $"x{stats.total_skeleton_kills}";
            ghastKillsText.text = $"x{stats.total_ghast_kills}";
            wizardKillsText.text = $"x{stats.total_wizard_kills}";
            demonKillsText.text = $"x{stats.total_demon_kills}";
        });
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
