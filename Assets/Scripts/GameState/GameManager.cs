using UnityEngine;
using System.Collections.Generic;


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
    public float time;

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

    /// <summary>
    /// Initializes the singleton instance and loads the game state, ensuring the object persists across scenes.
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
    /// Loads the game state from the save system. If no save data exists, resets the game state to defaults.
    /// </summary>
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
            ghastKills = data.ghastKills;
            wizardKills = data.wizardKills;
            demonKills = data.demonKills;
            time = data.time;

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

    /// <summary>
    /// Saves the current game state to the save system, preserving player progress and stats.
    /// </summary>
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
            ghastKills = ghastKills,
            wizardKills = wizardKills,
            demonKills = demonKills,
            dungeonRoomCount = dungeonRoomCount,
            goblinHealth = goblinHealth,
            goblinSpeed = goblinSpeed,
            skeletonHealth = skeletonHealth,
            skeletonFireRate = skeletonFireRate,
            ghastWaitTime = ghastWaitTime,
            wizardHealth = wizardHealth,
            wizardProjSpeed = wizardProjSpeed,
            demonHealth = demonHealth,
            time = time,
            statList = statList,
            activeSpecialEffects = new List<string>(activeSpecialEffects),
            purchasedItems = new List<string>(purchasedItems),
            equippedItems = new List<string>(equippedItems),
            dailyItems = new List<string>(dailyItems),
            weeklyItem = weeklyItem
        };

        SaveSystem.SaveGame(data);
    }

    /// <summary>
    /// Resets all game-related data to default values, including player stats, progression, and dungeon properties.
    /// </summary>
    public void ResetGameState()
    {
        // game state
        isActive = false;
        daysRemaining = 4;
        currentWeek = 1;
        weeklyCoins = 0;
        coinQuota = 100;
        coinsDeposited = 0;
        coinsHolding = 0;
        time = 1080;

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
        statList.Add(new Stat("dashDamage", 10));
        statList.Add(new Stat("dashCooldown", 4.5f));
        statList.Add(new Stat("dashDistance", 4.0f));
        statList.Add(new Stat("treasureRoomChance", 0.1f));

        // dungeon properties
        dungeonRoomCount = 8; //8
        goblinHealth = 20; //20
        goblinSpeed = 4f; //4
        skeletonHealth = 15; //15
        skeletonFireRate = 1f; //1
        ghastWaitTime = 3.5f; //3.5
        wizardHealth = 20; //20
        wizardProjSpeed = 1f; //1
        demonHealth = 160; //160

        SaveGameState();
    }

    /// <summary>
    /// Scales the dungeon difficulty by increasing enemy stats and room count as the player progresses.
    /// </summary>
    public void scaleDungeon()
    {
        dungeonRoomCount += 4;
        
        // Scale Goblin properties
        goblinHealth += 10;
        goblinSpeed = Mathf.Min(goblinSpeed + 1.5f, 10.0f); // Cap at 10

        // Scale Skeleton properties
        skeletonHealth += 10; // No cap
        skeletonFireRate = Mathf.Min(skeletonFireRate + 0.5f, 4f); // Cap at 4

        // Scale Ghast properties
        ghastWaitTime = Mathf.Max(ghastWaitTime - 0.5f, 1f); // Cap at 1 (decreasing)

        // Scale Wizard properties
        wizardHealth += 10; // No cap
        wizardProjSpeed = Mathf.Max(wizardProjSpeed + 2f, 16f); // Cap at 16

        // Scale Demon properties
        demonHealth += 75; // No cap
    }

    /// <summary>
    /// Modifies the value of a specified stat by a given amount. Logs an error if the stat does not exist.
    /// </summary>
    public void ModifyStat(string statName, float amount)
    {
        Stat stat = statList.Find(s => s.statName == statName);
        if (stat != null)
        {
            stat.statValue += amount;
        }
    }

    /// <summary>
    /// Retrieves the current value of a specified stat. Returns 0 if the stat does not exist.
    /// </summary>
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
        }
    }

    /// <summary>
    /// Checks if a specific special effect is currently active.
    /// </summary>
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
            return false;
        }

        if (coinsDeposited >= item.itemPrice)
        {
            coinsDeposited -= item.itemPrice;
            purchasedItems.Add(item.itemName);
            SaveGameState();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Increments the kill counter for the specified enemy type.
    /// </summary>
    /// <param name="enemyType">The type of the enemy (e.g., "goblin", "skeleton").</param>
    public void IncrementKillCounter(string enemyType)
    {
        switch (enemyType.ToLower())
        {
            case "goblin":
                goblinKills++;
                break;

            case "skeleton":
                skeletonKills++;
                break;

            case "ghast":
                ghastKills++;
                break;

            case "wizard":
                wizardKills++;
                break;

            case "demon":
                demonKills++;
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Starts a new day by decrementing days remaining and resetting the daily coin count.
    /// </summary>
    public void StartDay()
    {
        isActive = true;
        daysRemaining--;
        totalDays++;
        coinsHolding = 0;
        if (IsSpecialEffectActive("EarlyBird"))
        {
            time = 900;
        }
        else
        {
            time = 1080;
        }
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
        coinQuota += 100;
        weeklyCoins = 0;
        weeklyItem = null;
        scaleDungeon();
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
    /// Handles the logic for losing all coins currently held by the player. Applies special effects if active.
    /// </summary>
    public void LoseHolding()
    {
        deaths++;
        if (IsSpecialEffectActive("DeathDepositor"))
        {
            int coinsLostCalculate = coinsHolding;
            coinsHolding = Mathf.FloorToInt(GameManager.Instance.coinsHolding * 0.3f);
            coinsLostCalculate -= coinsHolding;
            totalCoinsLost += coinsLostCalculate;
        }
        else
        {
            totalCoinsLost += coinsHolding;
            coinsHolding = 0;
        }
        
        SaveGameState();
    }

    /// <summary>
    /// Deposits all coins currently held by the player, adding them to the total and weekly coin counts.
    /// </summary>
    public void DepositHolding()
    {
        escapes++;
        coinsDeposited += coinsHolding;
        weeklyCoins += coinsHolding;
        totalCoinsDeposited += coinsHolding;
        coinsHolding = 0;
        SaveGameState();
    }


}