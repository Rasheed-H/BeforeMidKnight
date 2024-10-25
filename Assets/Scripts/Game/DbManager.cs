using UnityEngine;
using System.Data;
using System.IO;
using System.Data.SQLite;

/// <summary>
/// Manages database operations for saving and retrieving game scores.
/// This singleton class ensures a single instance of DbManager persists across scenes
/// and handles the creation, insertion, and management of the SQLite database.
/// </summary>
public class DbManager : MonoBehaviour
{
    private static DbManager _instance;
    private string dbPath;

    /// <summary>
    /// Provides access to the single DbManager instance, creating it if it doesn't exist.
    /// </summary>
    public static DbManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject dbManager = new GameObject("DatabaseManager");
                _instance = dbManager.AddComponent<DbManager>();
                DontDestroyOnLoad(dbManager);
            }
            return _instance;
        }
    }

    /// <summary>
    /// Initializes the database path and creates the scores database if it does not exist.
    /// </summary>
    private void Awake()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/gameScores.db";
        CreateDatabase();
    }

    /// <summary>
    /// Creates the scores database with a table for storing player scores if it does not already exist.
    /// </summary>
    private void CreateDatabase()
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"CREATE TABLE IF NOT EXISTS Scores (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Score INTEGER,
                                        Coins_Deposited INTEGER,
                                        Week INTEGER,
                                        Date TEXT)";
                command.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Inserts a new score entry into the database with details about coins deposited, week, score, and date.
    /// </summary>
    /// <param name="coinsDeposited">The number of coins deposited by the player.</param>
    /// <param name="week">The week during which the score was achieved.</param>
    /// <param name="score">The player's score for the current run.</param>
    public void InsertScore(int coinsDeposited, int week, int score)
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Scores (Score, Coins_Deposited, Week, Date) VALUES (@score, @coins, @week, @date)";
                command.Parameters.Add(new SQLiteParameter("@score", score));
                command.Parameters.Add(new SQLiteParameter("@coins", coinsDeposited));
                command.Parameters.Add(new SQLiteParameter("@week", week));
                command.Parameters.Add(new SQLiteParameter("@date", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                command.ExecuteNonQuery();
            }
        }
    }
}
