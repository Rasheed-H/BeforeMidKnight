using UnityEngine;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Manages database operations for saving and retrieving game data.
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
    /// Initializes the database path and creates the database with necessary tables if it does not exist.
    /// </summary>
    private void Awake()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/gameData.db";
        CreateDatabase();
    }

    /// <summary>
    /// Creates the database tables for runs and stats if they do not already exist.
    /// </summary>
    private void CreateDatabase()
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Create the Runs table
                command.CommandText = @"CREATE TABLE IF NOT EXISTS Runs (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Score INTEGER,
                                    Week INTEGER,
                                    Deaths INTEGER,
                                    Escapes INTEGER,
                                    TotalCoinsDeposited INTEGER,
                                    Kills INTEGER,
                                    Date TEXT)";
                command.ExecuteNonQuery();

                // Create the Stats table
                command.CommandText = @"CREATE TABLE IF NOT EXISTS Stats (
                                    Id INTEGER PRIMARY KEY,
                                    HighestScore INTEGER DEFAULT 0,
                                    HighestWeek INTEGER DEFAULT 0,
                                    TotalDays INTEGER DEFAULT 0,
                                    TotalCoinsDeposited INTEGER DEFAULT 0,
                                    TotalCoinsLost INTEGER DEFAULT 0,
                                    TotalDeaths INTEGER DEFAULT 0,
                                    TotalEscapes INTEGER DEFAULT 0,
                                    TotalKills INTEGER DEFAULT 0,
                                    TotalGoblinKills INTEGER DEFAULT 0,
                                    TotalSkeletonKills INTEGER DEFAULT 0,
                                    TotalGhastKills INTEGER DEFAULT 0,
                                    TotalWizardKills INTEGER DEFAULT 0,
                                    TotalDemonKills INTEGER DEFAULT 0)";
                command.ExecuteNonQuery();

                // Insert initial row with all 0s if it doesn't already exist
                command.CommandText = @"INSERT OR IGNORE INTO Stats 
                                    (Id, HighestScore, HighestWeek, TotalDays, TotalCoinsDeposited, TotalCoinsLost, TotalDeaths, 
                                     TotalEscapes, TotalKills, TotalGoblinKills, TotalSkeletonKills, TotalGhastKills, 
                                     TotalWizardKills, TotalDemonKills) 
                                    VALUES 
                                    (1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)";
                command.ExecuteNonQuery();

            }
        }
    }

    /// <summary>
    /// Inserts a new run entry into the Runs table with details from the player's current run.
    /// </summary>
    public void InsertRun(int score, int week, int deaths, int escapes, int totalCoinsDeposited, int kills)
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Runs (Score, Week, Deaths, Escapes, TotalCoinsDeposited, Kills, Date) VALUES (@score, @week, @deaths, @escapes, @totalCoinsDeposited, @kills, @date)";
                command.Parameters.Add(new SQLiteParameter("@score", score));
                command.Parameters.Add(new SQLiteParameter("@week", week));
                command.Parameters.Add(new SQLiteParameter("@deaths", deaths));
                command.Parameters.Add(new SQLiteParameter("@escapes", escapes));
                command.Parameters.Add(new SQLiteParameter("@totalCoinsDeposited", totalCoinsDeposited));
                command.Parameters.Add(new SQLiteParameter("@kills", kills));
                command.Parameters.Add(new SQLiteParameter("@date", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                command.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Updates the Stats table, comparing incoming values for highestScore and highestWeek, 
    /// and adding all other totals to the existing values.
    /// </summary>
    public void UpdateStats(int highestScore, int highestWeek, int totalDays, int totalCoinsDeposited, int totalCoinsLost, int totalDeaths, int totalEscapes, int totalKills, int totalGoblinKills, int totalSkeletonKills, int totalSpiderKills, int totalWizardKills, int totalBossKills)
    {
        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.Open();

            int currentHighestScore = 0, currentHighestWeek = 0, currentTotalDays = 0, currentTotalCoinsDeposited = 0;
            int currentTotalCoinsLost = 0, currentTotalDeaths = 0, currentTotalEscapes = 0, currentTotalKills = 0;
            int currentTotalGoblinKills = 0, currentTotalSkeletonKills = 0, currentTotalSpiderKills = 0;
            int currentTotalWizardKills = 0, currentTotalBossKills = 0;

            // Retrieve current values
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Stats WHERE Id = 1";
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        currentHighestScore = reader.GetInt32(reader.GetOrdinal("HighestScore"));
                        currentHighestWeek = reader.GetInt32(reader.GetOrdinal("HighestWeek"));
                        currentTotalDays = reader.GetInt32(reader.GetOrdinal("TotalDays"));
                        currentTotalCoinsDeposited = reader.GetInt32(reader.GetOrdinal("TotalCoinsDeposited"));
                        currentTotalCoinsLost = reader.GetInt32(reader.GetOrdinal("TotalCoinsLost"));
                        currentTotalDeaths = reader.GetInt32(reader.GetOrdinal("TotalDeaths"));
                        currentTotalEscapes = reader.GetInt32(reader.GetOrdinal("TotalEscapes"));
                        currentTotalKills = reader.GetInt32(reader.GetOrdinal("TotalKills"));
                        currentTotalGoblinKills = reader.GetInt32(reader.GetOrdinal("TotalGoblinKills"));
                        currentTotalSkeletonKills = reader.GetInt32(reader.GetOrdinal("TotalSkeletonKills"));
                        currentTotalSpiderKills = reader.GetInt32(reader.GetOrdinal("TotalGhastKills"));
                        currentTotalWizardKills = reader.GetInt32(reader.GetOrdinal("TotalWizardKills"));
                        currentTotalBossKills = reader.GetInt32(reader.GetOrdinal("TotalDemonKills"));
                    }
                }
            }

            // Update only if incoming values are greater or add to totals
            highestScore = Mathf.Max(currentHighestScore, highestScore);
            highestWeek = Mathf.Max(currentHighestWeek, highestWeek);
            totalDays += currentTotalDays;
            totalCoinsDeposited += currentTotalCoinsDeposited;
            totalCoinsLost += currentTotalCoinsLost;
            totalDeaths += currentTotalDeaths;
            totalEscapes += currentTotalEscapes;
            totalKills += currentTotalKills;
            totalGoblinKills += currentTotalGoblinKills;
            totalSkeletonKills += currentTotalSkeletonKills;
            totalSpiderKills += currentTotalSpiderKills;
            totalWizardKills += currentTotalWizardKills;
            totalBossKills += currentTotalBossKills;

            // Update the Stats table
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"UPDATE Stats SET HighestScore = @highestScore, HighestWeek = @highestWeek, 
                                        TotalDays = @totalDays, TotalCoinsDeposited = @totalCoinsDeposited, TotalCoinsLost = @totalCoinsLost, 
                                        TotalDeaths = @totalDeaths, TotalEscapes = @totalEscapes, TotalKills = @totalKills,
                                        TotalGoblinKills = @totalGoblinKills, TotalSkeletonKills = @totalSkeletonKills, 
                                        TotalSpiderKills = @totalSpiderKills, TotalWizardKills = @totalWizardKills, TotalBossKills = @totalBossKills
                                        WHERE Id = 1";
                command.Parameters.Add(new SQLiteParameter("@highestScore", highestScore));
                command.Parameters.Add(new SQLiteParameter("@highestWeek", highestWeek));
                command.Parameters.Add(new SQLiteParameter("@totalDays", totalDays));
                command.Parameters.Add(new SQLiteParameter("@totalCoinsDeposited", totalCoinsDeposited));
                command.Parameters.Add(new SQLiteParameter("@totalCoinsLost", totalCoinsLost));
                command.Parameters.Add(new SQLiteParameter("@totalDeaths", totalDeaths));
                command.Parameters.Add(new SQLiteParameter("@totalEscapes", totalEscapes));
                command.Parameters.Add(new SQLiteParameter("@totalKills", totalKills));
                command.Parameters.Add(new SQLiteParameter("@totalGoblinKills", totalGoblinKills));
                command.Parameters.Add(new SQLiteParameter("@totalSkeletonKills", totalSkeletonKills));
                command.Parameters.Add(new SQLiteParameter("@totalGhastKills", totalSpiderKills));
                command.Parameters.Add(new SQLiteParameter("@totalWizardKills", totalWizardKills));
                command.Parameters.Add(new SQLiteParameter("@totalDemonKills", totalBossKills));
                command.ExecuteNonQuery();
            }
        }
    }
    public class RunData
    {
        public int Id;
        public int Score;
        public int Week;
        public int Deaths;
        public int Escapes;
        public int TotalCoinsDeposited;
        public int Kills;
        public string Date;
    }

    public List<RunData> GetAllRuns()
    {
        List<RunData> runs = new List<RunData>();

        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Score, Week, Deaths, Escapes, TotalCoinsDeposited, Kills, Date FROM Runs ORDER BY Score DESC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RunData run = new RunData
                        {
                            Id = reader.GetInt32(0),
                            Score = reader.GetInt32(1),
                            Week = reader.GetInt32(2),
                            Deaths = reader.GetInt32(3),
                            Escapes = reader.GetInt32(4),
                            TotalCoinsDeposited = reader.GetInt32(5),
                            Kills = reader.GetInt32(6),
                            Date = reader.GetString(7)
                        };
                        runs.Add(run);
                    }
                }
            }
        }
        return runs;
    }

    public class StatsData
    {
        public int HighestScore;
        public int HighestWeek;
        public int TotalDays;
        public int TotalCoinsDeposited;
        public int TotalCoinsLost;
        public int TotalDeaths;
        public int TotalEscapes;
        public int TotalKills;
        public int TotalGoblinKills;
        public int TotalSkeletonKills;
        public int TotalSpiderKills;
        public int TotalWizardKills;
        public int TotalBossKills;
    }

    public StatsData GetStats()
    {
        StatsData stats = new StatsData();

        using (var connection = new SQLiteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Stats WHERE Id = 1";
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stats.HighestScore = reader.GetInt32(reader.GetOrdinal("HighestScore"));
                        stats.HighestWeek = reader.GetInt32(reader.GetOrdinal("HighestWeek"));
                        stats.TotalDays = reader.GetInt32(reader.GetOrdinal("TotalDays"));
                        stats.TotalCoinsDeposited = reader.GetInt32(reader.GetOrdinal("TotalCoinsDeposited"));
                        stats.TotalCoinsLost = reader.GetInt32(reader.GetOrdinal("TotalCoinsLost"));
                        stats.TotalDeaths = reader.GetInt32(reader.GetOrdinal("TotalDeaths"));
                        stats.TotalEscapes = reader.GetInt32(reader.GetOrdinal("TotalEscapes"));
                        stats.TotalKills = reader.GetInt32(reader.GetOrdinal("TotalKills"));
                        stats.TotalGoblinKills = reader.GetInt32(reader.GetOrdinal("TotalGoblinKills"));
                        stats.TotalSkeletonKills = reader.GetInt32(reader.GetOrdinal("TotalSkeletonKills"));
                        stats.TotalSpiderKills = reader.GetInt32(reader.GetOrdinal("TotalSpiderKills"));
                        stats.TotalWizardKills = reader.GetInt32(reader.GetOrdinal("TotalWizardKills"));
                        stats.TotalBossKills = reader.GetInt32(reader.GetOrdinal("TotalBossKills"));
                    }
                }
            }
        }

        return stats;
    }

}
