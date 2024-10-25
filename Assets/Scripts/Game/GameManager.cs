using UnityEngine;

/// <summary>
/// Manages game state, progression, and data persistence for the player's lives, coins, and weekly targets.
/// This singleton ensures data is retained across scenes and manages saving/loading of game data.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isActive;             // Tracks if there is an ongoing run
    public int lives;                 // Player's remaining lives
    public int daysRemaining;         // Days left to meet the current coin quota
    public int totalDays;             // Total days per week
    public int weekCoins;             // Coins collected in the current week
    public int coinQuota;             // Coin quota to meet each week
    public int totalCoins;            // Player's total coins for the run
    public int totalCoinsAllTime;     // Player's cumulative coin total across all runs
    public int currentWeek;           // Current week of the run
    public int currentCoins;          // Coins collected in the current day

    /// <summary>
    /// Sets up the singleton instance, ensures the game manager persists across scenes, and loads saved game state.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Loads the saved game state from storage, if available. Initializes a new game state if no saved data is found.
    /// </summary>
    public void LoadGameState()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data != null)
        {
            isActive = data.isActive;
            lives = data.lives;
            daysRemaining = data.daysRemaining;
            totalDays = data.totalDays;
            weekCoins = data.weekCoins;
            coinQuota = data.coinQuota;
            totalCoins = data.totalCoins;
            currentWeek = data.currentWeek;
            totalCoinsAllTime = data.totalCoinsAllTime;
        }
        else
        {
            ResetGameState();
        }
    }

    /// <summary>
    /// Saves the current game state to storage.
    /// </summary>
    public void SaveGameState()
    {
        SaveData data = new SaveData
        {
            isActive = isActive,
            lives = lives,
            daysRemaining = daysRemaining,
            totalDays = totalDays,
            weekCoins = weekCoins,
            coinQuota = coinQuota,
            totalCoins = totalCoins,
            currentWeek = currentWeek,
            totalCoinsAllTime = totalCoinsAllTime
        };
        SaveSystem.SaveGame(data);
    }

    /// <summary>
    /// Resets the game state to default values, suitable for starting a new game.
    /// </summary>
    public void ResetGameState()
    {
        isActive = false;
        lives = 3;
        daysRemaining = 5;
        totalDays = 5;
        weekCoins = 0;
        coinQuota = 100;
        totalCoins = 0;
        currentWeek = 1;
        totalCoinsAllTime = 0;
        currentCoins = 0;

        SaveGameState();
    }

    /// <summary>
    /// Handles game over conditions by resetting the game state and disabling active run status.
    /// </summary>
    public void GameOver()
    {
        Debug.Log("Game Over: Player has no lives left");
        ResetGameState();
    }

    /// <summary>
    /// Starts a new day by decrementing days remaining and resetting the daily coin count.
    /// </summary>
    public void StartDay()
    {
        isActive = true;
        daysRemaining--;
        lives--;
        currentCoins = 0;
        SaveGameState();
    }

    /// <summary>
    /// Initiates a new week when the coin quota is met, resetting the days and increasing the quota.
    /// </summary>
    public void NewWeek()
    {
        currentWeek++;
        daysRemaining = totalDays;
        coinQuota += 50;
        weekCoins = 0;
        SaveGameState();
    }

    /// <summary>
    /// Adds a specified amount of coins to the player's current daily total.
    /// </summary>
    /// <param name="amount">The number of coins to add.</param>
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        SaveGameState();
    }

    /// <summary>
    /// Deposits the player's current coins into the weekly total after a successful dungeon escape.
    /// </summary>
    public void Escaped()
    {
        lives++;
        weekCoins += currentCoins;
        totalCoins += currentCoins;
        totalCoinsAllTime += currentCoins;
        SaveGameState();
    }
}
