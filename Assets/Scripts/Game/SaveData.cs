using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData
{
    public bool isActive;
    public int lives;
    public int daysRemaining;
    public int totalDays;
    public int weekCoins;
    public int coinQuota;
    public int totalCoins;
    public int totalCoinsAllTime;
    public int currentWeek;
}
