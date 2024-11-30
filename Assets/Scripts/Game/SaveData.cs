using System;
using System.Collections.Generic;

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
    public int spiderKills;
    public int wizardKills;
    public int bossKills;

    public int dungeonRoomCount;
    public int dungeonCrawlerCount;

    // Stats
    public List<Stat> statList = new List<Stat>();

    // Special Effects
    public List<string> activeSpecialEffects = new List<string>();

    // Item Management
    public List<string> purchasedItems = new List<string>();
    public List<string> equippedItems = new List<string>();
    public List<string> dailyItems = new List<string>();
    public string weeklyItem;
}

