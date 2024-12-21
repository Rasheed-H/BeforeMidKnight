using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameOverMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text scoreText;
    public TMP_Text weeksText;
    public TMP_Text escapesText;
    public TMP_Text deathsText;
    public TMP_Text coinDepositedText;
    public TMP_Text totalKillsText;

    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip gameOverSound;

    private int score;
    private int totalKills;

    private void Start()
    {
        MusicManager.Instance.StopMusic();
        SoundEffects.Instance.PlaySound(gameOverSound);

        score = GameManager.Instance.totalCoinsDeposited * GameManager.Instance.currentWeek;

        totalKills = GameManager.Instance.goblinKills + GameManager.Instance.skeletonKills +
                     GameManager.Instance.ghastKills + GameManager.Instance.wizardKills +
                     GameManager.Instance.demonKills;

        UpdateText();
    }

    private void UpdateText()
    {
        weeksText.text = $"Weeks Survived: {GameManager.Instance.currentWeek}";
        escapesText.text = $"Escapes: {GameManager.Instance.escapes}";
        totalKillsText.text = $"Kills: {totalKills}";
        deathsText.text = $"Deaths: {GameManager.Instance.deaths}";
        coinDepositedText.text = $"x {GameManager.Instance.totalCoinsDeposited}";
        scoreText.text = $"{score}";
    }

    public void OnMainMenuClicked()
    {
        
        DbManager.Instance.SaveRun(
            score,
            GameManager.Instance.currentWeek,
            GameManager.Instance.deaths,
            GameManager.Instance.escapes,
            GameManager.Instance.totalCoinsDeposited,
            totalKills
        );

        
        DbManager.Instance.SaveStats(
            highestScore: score,
            highestWeek: GameManager.Instance.currentWeek,
            totalDays: GameManager.Instance.totalDays,
            totalCoinsDeposited: GameManager.Instance.totalCoinsDeposited,
            totalCoinsLost: GameManager.Instance.totalCoinsLost,
            totalDeaths: GameManager.Instance.deaths,
            totalEscapes: GameManager.Instance.escapes,
            totalKills: totalKills,
            totalGoblinKills: GameManager.Instance.goblinKills,
            totalSkeletonKills: GameManager.Instance.skeletonKills,
            totalGhastKills: GameManager.Instance.ghastKills,
            totalWizardKills: GameManager.Instance.wizardKills,
            totalDemonKills: GameManager.Instance.demonKills,
            callback: success =>
            {
                if (success)
                {
                    Debug.Log("Stats updated successfully.");
                }
                else
                {
                    Debug.LogError("Failed to update stats.");
                }
            }
        );

        
        GameManager.Instance.ResetGameState();

        SceneManager.LoadScene("MainMenu");
    }

    public void PlayButtonClickSound()
    {
        SoundEffects.Instance.PlaySound(buttonClickSound);
    }
}
