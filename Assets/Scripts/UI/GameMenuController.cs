using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Controls the game menu UI and navigation between scenes based on game state.
/// Updates display elements for remaining days, current week, weekly quota, and total coins.
/// </summary>
public class GameMenuController : MonoBehaviour
{
    [Header("UI Text Elements")]
    public TMP_Text daysLeftText;
    public TMP_Text currentWeekText;
    public TMP_Text weeklyQuotaText;
    public TMP_Text totalCoinsText;
    [SerializeField] private AudioClip buttonClickSound;

    /// <summary>
    /// Initializes the game menu, checks game over conditions, and updates the UI.
    /// </summary>
    private void Awake()
    {
        if (GameManager.Instance.daysRemaining <= 0)
        {
            if (GameManager.Instance.weeklyCoins < GameManager.Instance.coinQuota)
            {
                SceneManager.LoadScene("GameOverMenu");
            }
            else
            {
                GameManager.Instance.NewWeek();
            }
        }
        UpdateGameMenuUI();
    }

    /// <summary>
    /// Updates the game menu UI elements with the latest values from GameManager.
    /// </summary>
    public void UpdateGameMenuUI()
    {
        daysLeftText.text = $"Days Left: {GameManager.Instance.daysRemaining}";
        currentWeekText.text = $"Week: {GameManager.Instance.currentWeek}";
        weeklyQuotaText.text = $"{GameManager.Instance.weeklyCoins} / {GameManager.Instance.coinQuota}";
        totalCoinsText.text = $"{GameManager.Instance.coinsDeposited}";
    }

    /// <summary>
    /// Starts a new day and loads the dungeon scene.
    /// </summary>
    public void OnStartGameClicked()
    {
        GameManager.Instance.StartDay();
        SceneManager.LoadScene("Dungeon");
    }

    /// <summary>
    /// Navigates to the shop scene.
    /// </summary>
    public void OnShopClicked()
    {
        SceneManager.LoadScene("Shop");
    }

    /// <summary>
    /// Returns to the main menu scene.
    /// </summary>
    public void OnMainMenuClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayButtonClickSound()
    {
        SoundEffects.Instance.PlaySound(buttonClickSound);
    }

}
