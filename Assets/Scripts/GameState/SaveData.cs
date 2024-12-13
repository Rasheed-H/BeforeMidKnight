using System;
using System.Collections.Generic;

/// <summary>
/// Represents the structure for saving and loading game data. Includes information about
/// the player's progress, stats, inventory, and dungeon properties.
/// </summary>
[Serializable]
public class SaveData
{
    public bool isActive;
    public int daysRemaining;
    public int currentWeek;
    public int weeklyCoins;
    public int coinQuota;
    public int coinsDeposited;
    public int coinsHolding;

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

    public List<Stat> statList = new List<Stat>();
    public List<string> activeSpecialEffects = new List<string>();
    public List<string> purchasedItems = new List<string>();
    public List<string> equippedItems = new List<string>();
    public List<string> dailyItems = new List<string>();
    public string weeklyItem;
}

