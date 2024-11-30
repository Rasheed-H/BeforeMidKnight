using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;


/// <summary>
/// Manages game state, progression, and data persistence for the player's lives, coins, and weekly targets.
/// This singleton ensures data is retained across scenes and manages saving/loading of game data.
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Game Management
    public bool isActive;
    public int daysRemaining;
    public int currentWeek;
    public int weeklyCoins;
    public int coinQuota;
    public int coinsDeposited;
    public int coinsHolding;

    // Dungeon Properties
    public int dungeonRoomCount;
    public int dungeonCrawlerCount;

    // Stats and Progression Tracking
    public int totalDays;
    public int totalCoinsDeposited;
    public int totalCoinsLost;
    public int deaths;
    public int escapes;
    public int goblinKills;
    public int skeletonKills;
    public int spiderKills;
    public int wizardKills;
    public int bossKills;


    // Item Management
    public List<string> purchasedItems = new List<string>();
    public List<string> equippedItems = new List<string>();
    public List<string> dailyItems = new List<string>();
    public string weeklyItem;

    public List<Stat> statList = new List<Stat>();
    private HashSet<string> activeSpecialEffects = new HashSet<string>();

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

    

    public void LoadGameState()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data != null)
        {
            isActive = data.isActive;
            daysRemaining = data.daysRemaining;
            currentWeek = data.currentWeek;
            weeklyCoins = data.weeklyCoins;
            coinQuota = data.coinQuota;
            coinsDeposited = data.coinsDeposited;
            coinsHolding = data.coinsHolding;
            totalDays = data.totalDays;
            totalCoinsDeposited = data.totalCoinsDeposited;
            totalCoinsLost = data.totalCoinsLost;
            deaths = data.deaths;
            escapes = data.escapes;
            goblinKills = data.goblinKills;
            skeletonKills = data.skeletonKills;
            spiderKills = data.spiderKills;
            wizardKills = data.wizardKills;
            bossKills = data.bossKills;

            dungeonRoomCount = data.dungeonRoomCount;
            dungeonCrawlerCount = data.dungeonCrawlerCount;

            statList = data.statList ?? new List<Stat>();

            activeSpecialEffects = new HashSet<string>(data.activeSpecialEffects);

            purchasedItems = data.purchasedItems ?? new List<string>();
            equippedItems = data.equippedItems ?? new List<string>();
            dailyItems = data.dailyItems ?? new List<string>();
            weeklyItem = data.weeklyItem;
        }
        else
        {
            ResetGameState();
        }
    }

    public void SaveGameState()
    {
        SaveData data = new SaveData
        {
            isActive = isActive,
            daysRemaining = daysRemaining,
            currentWeek = currentWeek,
            weeklyCoins = weeklyCoins,
            coinQuota = coinQuota,
            coinsDeposited = coinsDeposited,
            coinsHolding = coinsHolding,
            totalDays = totalDays,
            totalCoinsDeposited = totalCoinsDeposited,
            totalCoinsLost = totalCoinsLost,
            deaths = deaths,
            escapes = escapes,
            goblinKills = goblinKills,
            skeletonKills = skeletonKills,
            spiderKills = spiderKills,
            wizardKills = wizardKills,
            bossKills = bossKills,
            dungeonRoomCount = dungeonRoomCount,
            dungeonCrawlerCount = dungeonCrawlerCount,
            statList = statList,
            activeSpecialEffects = new List<string>(activeSpecialEffects),
            purchasedItems = new List<string>(purchasedItems),
            equippedItems = new List<string>(equippedItems),
            dailyItems = new List<string>(dailyItems),
            weeklyItem = weeklyItem
        };

        SaveSystem.SaveGame(data);
    }

    public void ResetGameState()
    {
        isActive = false;
        daysRemaining = 4;
        currentWeek = 1;
        weeklyCoins = 0;
        coinQuota = 100;
        coinsDeposited = 0;
        coinsHolding = 0;
        totalDays = 0;
        totalCoinsDeposited = 0;
        totalCoinsLost = 0;
        deaths = 0;
        escapes = 0;
        goblinKills = 0;
        skeletonKills = 0;
        spiderKills = 0;
        wizardKills = 0;
        bossKills = 0;
        dungeonRoomCount = 10;
        dungeonCrawlerCount = 3;

        statList.Clear();
        purchasedItems.Clear();
        equippedItems.Clear();
        activeSpecialEffects.Clear();

        dailyItems.Clear();
        weeklyItem = null;

        // Initialize default stats
        statList.Add(new Stat("playerMaxHealth", 6));
        statList.Add(new Stat("playerMoveSpeed", 6.0f));
        statList.Add(new Stat("playerInvincibilityDuration", 2.5f));

        statList.Add(new Stat("slashDamage", 3));
        statList.Add(new Stat("daggerDamage", 2));
        statList.Add(new Stat("daggerSpeed", 10f));
        statList.Add(new Stat("slashCooldown", 1f));
        statList.Add(new Stat("daggerCooldown", 0.5f));

        statList.Add(new Stat("dashSpeed", 10f));
        statList.Add(new Stat("dashDamage", 5));
        statList.Add(new Stat("dashCooldown", 0.0f));
        statList.Add(new Stat("dashDistance", 4.0f));

        statList.Add(new Stat("goblinHealth", 20f));
        statList.Add(new Stat("goblinSpeed", 2.0f));
        statList.Add(new Stat("skeletonHealth", 15));
        statList.Add(new Stat("skeletonSpeed", 2.0f));

        SaveGameState();
    }

    public void ModifyStat(string statName, float amount)
    {
        Stat stat = statList.Find(s => s.statName == statName);
        if (stat != null)
        {
            stat.statValue += amount;
        }
        else
        {
            Debug.LogError($"Stat '{statName}' does not exist in GameManager.");
        }
    }

    public float GetStat(string statName)
    {
        Stat stat = statList.Find(s => s.statName == statName);
        return stat != null ? stat.statValue : 0f;
    }

    /**
    public void EquipItem(ItemData item)
    {
        if (!purchasedItems.Contains(item))
        {
            Debug.LogWarning($"{item.itemName} must be purchased before it can be equipped.");
            return;
        }

        if (!equippedItems.Contains(item))
        {
            equippedItems.Add(item);

            foreach (var modifier in item.statModifiers)
            {
                ModifyStat(modifier.statName, modifier.value);
            }

            foreach (var specialEffect in item.specialEffects)
            {
                activeSpecialEffects.Add(specialEffect);
            }

            Debug.Log($"{item.itemName} equipped, stats updated.");
            SaveGameState();
        }
    }

    public void UnequipItem(ItemData item)
    {
        if (equippedItems.Contains(item))
        {
            equippedItems.Remove(item);

            foreach (var modifier in item.statModifiers)
            {
                ModifyStat(modifier.statName, -modifier.value);
            }

            foreach (var specialEffect in item.specialEffects)
            {
                activeSpecialEffects.Remove(specialEffect);
            }

            Debug.Log($"{item.itemName} unequipped, stats reverted.");
            SaveGameState();
        }
    }

    public void PurchaseItem(ItemData item)
    {
        if (purchasedItems.Contains(item))
        {
            Debug.LogWarning($"{item.itemName} is already purchased.");
            return;
        }

        if (coinsDeposited >= item.itemPrice)
        {
            coinsDeposited -= item.itemPrice;
            purchasedItems.Add(item);
            SaveGameState();
            Debug.Log($"{item.itemName} purchased for {item.itemPrice} coins.");
        }
        else
        {
            Debug.LogWarning($"Not enough coins to purchase {item.itemName}. You need {item.itemPrice - coinsDeposited} more coins.");
        }
    }
    **/

    public bool IsSpecialEffectActive(string effectName)
    {
        return activeSpecialEffects.Contains(effectName);
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
        coinsHolding = 0;
        dailyItems.Clear();
        SaveGameState();
    }

    /// <summary>
    /// Initiates a new week when the coin quota is met, resetting the days and increasing the quota.
    /// </summary>
    public void NewWeek()
    {
        currentWeek++;
        daysRemaining = 4;
        coinQuota += 50;
        weeklyCoins = 0;
        weeklyItem = null;
        SaveGameState();
    }

    /// <summary>
    /// Adds a specified amount of coins to the player's current daily total.
    /// </summary>
    /// <param name="amount">The number of coins to add.</param>
    public void AddCoins(int amount)
    {
        coinsHolding += amount;
    }

    /// <summary>
    /// Deposits the player's current coins into the weekly total after a successful dungeon escape.
    /// </summary>
    public void Escaped()
    {
        weeklyCoins += coinsHolding;
        coinsDeposited += coinsHolding;
        totalCoinsDeposited += coinsHolding;
        coinsHolding = 0;
        SaveGameState();
    }
}