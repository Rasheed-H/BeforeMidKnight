using UnityEngine;
using TMPro;

/// <summary>
/// Controls the Heads-Up Display (HUD), showing the player's current coin count and in-game timer.
/// Manages the in-game time progression and triggers the player's death if the time reaches midnight.
/// </summary>
public class HUDController : MonoBehaviour
{
    public Player player;

    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text timerText;

    private float gameTime = 1080;  // Start time set to 6:00 PM in minutes
    private float timeIncrementPerSecond = 0.6f; // In-game minutes per real-time second

    /// <summary>
    /// Initializes the HUD by setting the initial coin count and timer text.
    /// </summary>
    private void Start()
    {
        UpdateCoinText();
        UpdateTimerText();
    }

    /// <summary>
    /// Updates the game time each frame and refreshes the timer display.
    /// Ends the game if time reaches midnight.
    /// </summary>
    private void Update()
    {
        gameTime += Time.deltaTime * timeIncrementPerSecond; // Increment game time by 0.6 minutes per real-time second
        UpdateTimerText();

        if (gameTime >= 1440) // 1440 minutes = 12:00 AM
        {
            player.Die();
        }
    }

    /// <summary>
    /// Updates the coin display with the player's current coin count.
    /// </summary>
    public void UpdateCoinText()
    {
        coinText.text = $"x {GameManager.Instance.coinsHolding}";
    }

    /// <summary>
    /// Updates the timer display, converting the game time in minutes to a 12-hour format with AM/PM.
    /// </summary>
    private void UpdateTimerText()
    {
        int hours = Mathf.FloorToInt(gameTime / 60) % 12;
        if (hours == 0) hours = 12;
        int minutes = Mathf.FloorToInt(gameTime % 60);
        string period = gameTime >= 720 ? "PM" : "AM";

        timerText.text = $"{hours:D2}:{minutes:D2} {period}";
    }
}
