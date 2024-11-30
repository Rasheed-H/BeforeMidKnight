using UnityEngine;
using TMPro;
using System.Collections.Generic;
using static DbManager;

public class ScoreListController : MonoBehaviour
{
    public GameObject runEntryPrefab;
    public Transform contentPanel;

    public void DisplayScores()
    {
        // Clear existing entries
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        List<RunData> runs = DbManager.Instance.GetAllRuns();

        foreach (RunData run in runs)
        {
            GameObject newEntry = Instantiate(runEntryPrefab, contentPanel);

            // Set up references for each field
            TMP_Text idText = newEntry.transform.Find("IDContainer/IDText").GetComponent<TMP_Text>();
            TMP_Text scoreText = newEntry.transform.Find("RunContainer/ScoreText").GetComponent<TMP_Text>();
            TMP_Text statsText = newEntry.transform.Find("RunContainer/StatsText").GetComponent<TMP_Text>();
            TMP_Text coinsText = newEntry.transform.Find("RunContainer/CoinsText").GetComponent<TMP_Text>();
            TMP_Text dateText = newEntry.transform.Find("RunContainer/DateText").GetComponent<TMP_Text>();

            // Assign values from database to text fields
            idText.text = $"{run.Week}";
            scoreText.text = $"{run.Score}";
            statsText.text = $"Escapes: {run.Escapes}\nDeaths: {run.Deaths}\nKills: {run.Kills}";
            coinsText.text = $"x {run.TotalCoinsDeposited}";
            dateText.text = System.DateTime.Parse(run.Date).ToString("MM/dd/yyyy");
        }
    }
}
