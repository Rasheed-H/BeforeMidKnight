using UnityEngine;

/// <summary>
/// Handles saving, loading, and deleting game data for WebGL and other platforms.
/// </summary>
public class SaveSystem
{
    private const string saveKey = "GameSaveData"; 

    /// <summary>
    /// Loads the saved game data from PlayerPrefs.
    /// </summary>
    public static SaveData LoadGame()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string json = PlayerPrefs.GetString(saveKey);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Saves the provided game data as a JSON string in PlayerPrefs.
    /// </summary>
    /// <param name="data">The game state to save.</param>
    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save(); 
    }

    /// <summary>
    /// Deletes the saved game data from PlayerPrefs.
    /// </summary>
    public static void DeleteSaveData()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            PlayerPrefs.DeleteKey(saveKey);
        }
    }
}
