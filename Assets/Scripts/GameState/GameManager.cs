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
    public int goblinHealth;
    public float goblinSpeed;
    public int skeletonHealth;
    public float skeletonFireRate;
    public float ghastWaitTime;
    public int wizardHealth;
    public float wizardProjSpeed;
    public int demonHealth;

    // Stats and Progression Tracking
    public int totalDays;
    public int totalCoinsDeposited;
    public int totalCoinsLost;
    public int deaths;
    public int escapes;
    public int goblinKills;
    public int skeletonKills;
    public int ghastKills;
    public int wizardKills;
    public int demonKills;


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
            ghastKills = data.spiderKills;
            wizardKills = data.wizardKills;
            demonKills = data.bossKills;

            dungeonRoomCount = data.dungeonRoomCount;
            goblinHealth = data.goblinHealth;
            goblinSpeed = data.goblinSpeed;
            skeletonHealth = data.skeletonHealth;
            skeletonFireRate = data.skeletonFireRate;
            ghastWaitTime = data.ghastWaitTime;
            wizardHealth = data.wizardHealth;
            wizardProjSpeed = data.wizardProjSpeed;
            demonHealth = data.demonHealth;

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
            spiderKills = ghastKills,
            wizardKills = wizardKills,
            bossKills = demonKills,
            dungeonRoomCount = dungeonRoomCount,
            goblinHealth = goblinHealth,
            goblinSpeed = goblinSpeed,
            skeletonHealth = skeletonHealth,
            skeletonFireRate = skeletonFireRate,
            ghastWaitTime = ghastWaitTime,
            wizardHealth = wizardHealth,
            wizardProjSpeed = wizardProjSpeed,
            demonHealth = demonHealth,
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
        // game state
        isActive = false;
        daysRemaining = 4;
        currentWeek = 1;
        weeklyCoins = 0;
        coinQuota = 100;
        coinsDeposited = 50;
        coinsHolding = 0;

        // run stats
        totalDays = 0;
        totalCoinsDeposited = 0;
        totalCoinsLost = 0;
        deaths = 0;
        escapes = 0;
        goblinKills = 0;
        skeletonKills = 0;
        ghastKills = 0;
        wizardKills = 0;
        demonKills = 0;

        // shop & inventory 
        statList.Clear();
        purchasedItems.Clear();
        equippedItems.Clear();
        activeSpecialEffects.Clear();
        dailyItems.Clear();
        weeklyItem = null;

        // default player stats
        statList.Add(new Stat("playerMaxHealth", 6));
        statList.Add(new Stat("playerMoveSpeed", 6.0f));
        statList.Add(new Stat("playerInvincibilityDuration", 2.5f));
        statList.Add(new Stat("slashDamage", 20));
        statList.Add(new Stat("slashCooldown", 3f));
        statList.Add(new Stat("daggerDamage", 15)); 
        statList.Add(new Stat("daggerSpeed", 10f));
        statList.Add(new Stat("daggerCooldown", 1.5f));
        statList.Add(new Stat("dashSpeed", 10f));
        statList.Add(new Stat("dashDamage", 3));
        statList.Add(new Stat("dashCooldown", 4.5f));
        statList.Add(new Stat("dashDistance", 4.0f));
        statList.Add(new Stat("treasureRoomChance", 0.1f));

        // dungeon properties
        dungeonRoomCount = 10;
        goblinHealth = 20;
        goblinSpeed = 5f;
        skeletonHealth = 15;
        skeletonFireRate = 2f;
        ghastWaitTime = 3f;
        wizardHealth = 30;
        wizardProjSpeed = 2f;
        demonHealth = 150;

        SaveGameState();
    }

    public void scaleDungeon()
    {
        dungeonRoomCount += 5;
        // Scale Goblin properties
        goblinHealth += 5;
        goblinSpeed = Mathf.Min(goblinSpeed + 2.5f, 17.5f); // Cap at 22.5

        // Scale Skeleton properties
        skeletonHealth += 5; // No cap
        skeletonFireRate = Mathf.Min(skeletonFireRate + 1f, 9f); // Cap at 9

        // Scale Ghast properties
        ghastWaitTime = Mathf.Max(ghastWaitTime - 0.5f, 1f); // Cap at 1 (decreasing)

        // Scale Wizard properties
        wizardHealth += 10; // No cap
        wizardProjSpeed = Mathf.Max(wizardProjSpeed + 2f, 16f); // Cap at 16

        // Scale Demon properties
        demonHealth += 100; // No cap

        Debug.Log($"Dungeon scaled: RoomCount={dungeonRoomCount}, GoblinHealth={goblinHealth}, GoblinSpeed={goblinSpeed}, " +
                  $"SkeletonHealth={skeletonHealth}, SkeletonFireRate={skeletonFireRate}, GhastWaitTime={ghastWaitTime}, " +
                  $"WizardHealth={wizardHealth}, WizardProjSpeed={wizardProjSpeed}, DemonHealth={demonHealth}");
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

    /// <summary>
    /// Adds a special effect to the active effects list.
    /// </summary>
    /// <param name="effectName">The name of the special effect to add.</param>
    public void AddSpecialEffect(string effectName)
    {
        if (!activeSpecialEffects.Contains(effectName))
        {
            activeSpecialEffects.Add(effectName);
            Debug.Log($"Special effect '{effectName}' activated.");
        }
        else
        {
            Debug.LogWarning($"Special effect '{effectName}' is already active.");
        }
    }

    /// <summary>
    /// Removes a special effect from the active effects list.
    /// </summary>
    /// <param name="effectName">The name of the special effect to remove.</param>
    public void RemoveSpecialEffect(string effectName)
    {
        if (activeSpecialEffects.Contains(effectName))
        {
            activeSpecialEffects.Remove(effectName);
            Debug.Log($"Special effect '{effectName}' deactivated.");
        }
        else
        {
            Debug.LogWarning($"Special effect '{effectName}' is not currently active.");
        }
    }

    public bool IsSpecialEffectActive(string effectName)
    {
        return activeSpecialEffects.Contains(effectName);
    }

    /// <summary>
    /// Handles the logic for purchasing an item.
    /// </summary>
    /// <param name="item">The item to purchase.</param>
    /// <returns>True if the item was successfully purchased, false otherwise.</returns>
    public bool PurchaseItem(ItemData item)
    {
        if (purchasedItems.Contains(item.itemName))
        {
            Debug.LogWarning($"{item.itemName} is already purchased.");
            return false;
        }

        if (coinsDeposited >= item.itemPrice)
        {
            coinsDeposited -= item.itemPrice;
            purchasedItems.Add(item.itemName);
            SaveGameState();
            Debug.Log($"{item.itemName} purchased for {item.itemPrice} coins.");
            return true;
        }
        else
        {
            Debug.LogWarning($"Not enough coins to purchase {item.itemName}. You need {item.itemPrice - coinsDeposited} more coins.");
            return false;
        }
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
        scaleDungeon();
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