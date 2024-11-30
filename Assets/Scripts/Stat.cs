[System.Serializable]
public class Stat
{
    public string statName;
    public float statValue;

    public Stat(string statName, float statValue)
    {
        this.statName = statName;
        this.statValue = statValue;
    }
}
