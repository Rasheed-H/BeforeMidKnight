using System.IO;
using UnityEngine;

/// <summary>
/// Handles saving, loading, and deleting game data using a JSON file stored in persistent storage.
/// </summary>
public class SaveSystem
{
    private static string saveFilePath = Application.persistentDataPath + "/saveData.json";

    /// <summary>
    /// Loads the saved game data from the JSON file. 
    /// Returns a `SaveData` object if the file exists; otherwise, returns null.
    /// </summary>
    public static SaveData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            return null; 
        }
    }

    /// <summary>
    /// Saves the provided game data to a JSON file, overwriting any existing save file.
    /// </summary>
    /// <param name="data">The `SaveData` object containing the game state to be saved.</param>
    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }

    /// <summary>
    /// Deletes the existing save file, effectively resetting the saved game state.
    /// </summary>
    public static void DeleteSaveData()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }
}
