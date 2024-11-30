using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatContainer
{
    private Dictionary<string, float> stats = new Dictionary<string, float>();

    public void AddStat(string name, float baseValue)
    {
        if (!stats.ContainsKey(name))
        {
            stats[name] = baseValue;
        }
    }

    public void ModifyStat(string name, float amount)
    {
        if (stats.ContainsKey(name))
        {
            stats[name] += amount;
        }
    }

    public float GetStat(string name)
    {
        return stats.TryGetValue(name, out float value) ? value : 0f;
    }
}
