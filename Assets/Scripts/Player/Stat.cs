/// <summary>
/// Represents a game stat with a name and a value, allowing for dynamic adjustments during gameplay.
/// </summary>
[System.Serializable]
public class Stat
{
    public string statName;
    public float statValue;

    /// <summary>
    /// Initializes a new instance of the `Stat` class with the specified name and value.
    /// </summary>
    /// <param name="statName">The name of the stat.</param>
    /// <param name="statValue">The initial value of the stat.</param>
    public Stat(string statName, float statValue)
    {
        this.statName = statName;
        this.statValue = statValue;
    }
}
