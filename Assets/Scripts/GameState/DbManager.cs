using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// Manages all database-related interactions, such as creating users, saving runs, and fetching or updating stats.
/// This class ensures seamless integration with the backend API.
/// </summary>
public class DbManager : MonoBehaviour
{
    public static DbManager Instance;
    private string guestId;
    private const string BaseUrl = "https://before-midknight.rf.gd/api/api.php";
    private int userId;


    /// <summary>
    /// Initializes the DbManager instance and ensures only one instance exists throughout the game.
    /// Automatically gets or creates a user on startup.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GetOrCreateUser(); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Retrieves the existing user or creates a new one if no user exists.
    /// Uses PlayerPrefs to store and retrieve the guest ID.
    /// </summary>
    private void GetOrCreateUser()
    {
        if (!PlayerPrefs.HasKey("GuestId"))
        {
            guestId = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("GuestId", guestId);
            StartCoroutine(InsertUserIntoDatabase(guestId)); 
            Debug.Log($"Guest Created: {guestId}");
        }
        else
        {
            guestId = PlayerPrefs.GetString("GuestId");
            Debug.Log($"Guest Exists: {guestId}");
            StartCoroutine(VerifyUser(guestId));
        }
    }

    /// <summary>
    /// Sends a request to insert a new user into the database.
    /// </summary>
    /// <param name="guestId">The unique guest ID to associate with the user.</param>
    private IEnumerator InsertUserIntoDatabase(string guestId)
    {
        WWWForm form = new WWWForm();
        form.AddField("guest_id", guestId);

        using (UnityWebRequest request = UnityWebRequest.Post($"{BaseUrl}?action=create-user", form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"InsertUserIntoDatabase Response: {request.downloadHandler.text}");
                UserInsertResponse response = JsonUtility.FromJson<UserInsertResponse>(request.downloadHandler.text);

                if (response.success)
                {
                    userId = response.user_id; 
                    Debug.Log($"User created with UserId: {userId}");

                    StartCoroutine(InitializeStats(userId));
                    StartCoroutine(CreateRandomRuns(userId));
                }
                else
                {
                    Debug.LogError($"InsertUserIntoDatabase failed: {response.error}");
                }
            }
            else
            {
                Debug.LogError($"InsertUserIntoDatabase Error: {request.error}");
            }
        }
    }

    /// <summary>
    /// Verifies if the user exists in the database. If not, creates a new user.
    /// </summary>
    /// <param name="guestId">The unique guest ID to verify.</param>
    private IEnumerator VerifyUser(string guestId)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{BaseUrl}?action=get-user&guest_id={guestId}"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"VerifyUser Response: {request.downloadHandler.text}");
                UserResponse response = JsonUtility.FromJson<UserResponse>(request.downloadHandler.text);

                if (response.success)
                {
                    userId = response.user.id; 
                    Debug.Log($"User verified with UserId: {userId}");
                }
                else
                {
                    Debug.Log($"User not in database. Creating a new user.");
                    StartCoroutine(InsertUserIntoDatabase(guestId));
                }
            }
            else
            {
                Debug.LogError($"VerifyUser Error: {request.error}");
            }
        }
    }

    /// <summary>
    /// Creates 10 random runs for the user in the database for testing purposes.
    /// </summary>
    /// <param name="userId">The user ID to associate the runs with.</param>
    private IEnumerator CreateRandomRuns(int userId)
    {
        for (int i = 0; i < 10; i++)
        {
            WWWForm form = new WWWForm();
            var week = UnityEngine.Random.Range(1, 10);
            var tcd = UnityEngine.Random.Range(50, 500);
            var score = week * tcd;
            form.AddField("user_id", userId);
            form.AddField("score", score);
            form.AddField("week", week);
            form.AddField("deaths", UnityEngine.Random.Range(0, 50));
            form.AddField("escapes", UnityEngine.Random.Range(0, 50));
            form.AddField("total_coins_deposited", tcd);
            form.AddField("kills", UnityEngine.Random.Range(10, 250));

            using (UnityWebRequest request = UnityWebRequest.Post($"{BaseUrl}?action=create-run", form))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"Run {i + 1} insert response: {request.downloadHandler.text}");
                }
                else
                {
                    Debug.LogError($"Error creating run {i + 1}: {request.error}");
                }
            }
        }

        Debug.Log("All random runs have been created.");
    }

    /// <summary>
    /// Retrieves all runs for the current user from the database.
    /// </summary>
    /// <param name="callback">Callback function to handle the list of runs.</param>
    public void GetRuns(System.Action<List<RunData>> callback)
    {
        StartCoroutine(GetRunsCoroutine(callback));
    }

    /// <summary>
    /// Coroutine to fetch runs data from the database.
    /// </summary>
    /// <param name="callback">Callback function to handle the list of runs.</param>
    private IEnumerator GetRunsCoroutine(System.Action<List<RunData>> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{BaseUrl}?action=get-runs&user_id={userId}"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"GetRuns Response: {request.downloadHandler.text}");
                RunListResponse response = JsonUtility.FromJson<RunListResponse>(request.downloadHandler.text);

                if (response.success)
                {
                    Debug.Log($"Received {response.runs.Count} runs.");
                    callback?.Invoke(response.runs);
                }
                else
                {
                    Debug.LogError($"GetRuns failed: {response.error}");
                    callback?.Invoke(new List<RunData>());
                }
            }
            else
            {
                Debug.LogError($"GetRuns Error: {request.error}");
                callback?.Invoke(new List<RunData>()); 
            }
        }
    }

    /// <summary>
    /// Initializes stats for the user in the database.
    /// </summary>
    /// <param name="userId">The user ID to associate the stats with.</param>
    private IEnumerator InitializeStats(int userId)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", userId);

        using (UnityWebRequest request = UnityWebRequest.Post($"{BaseUrl}?action=create-stats", form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"InitializeStats Response: {request.downloadHandler.text}");
                StatsResponse response = JsonUtility.FromJson<StatsResponse>(request.downloadHandler.text);

                if (response.success)
                {
                    Debug.Log("Stats successfully initialized.");
                }
                else
                {
                    Debug.LogError($"InitializeStats failed: {response.error}");
                }
            }
            else
            {
                Debug.LogError($"InitializeStats Error: {request.error}");
            }
        }
    }

    /// <summary>
    /// Fetches the stats for the current user from the database.
    /// </summary>
    /// <param name="callback">Callback function to handle the stats data.</param>
    public void GetStats(System.Action<StatsData> callback)
    {
        StartCoroutine(GetStatsCoroutine(callback));
    }

    /// <summary>
    /// Coroutine to fetch stats data from the database.
    /// </summary>
    /// <param name="callback">Callback function to handle the stats data.</param>
    private IEnumerator GetStatsCoroutine(System.Action<StatsData> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{BaseUrl}?action=get-stats&user_id={userId}"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"GetStats Response: {request.downloadHandler.text}");

                try
                {
                    StatsResponse response = JsonUtility.FromJson<StatsResponse>(request.downloadHandler.text);

                    if (response != null && response.success)
                    {
                        StatsData stats = response.stats;
                        callback?.Invoke(stats);
                    }
                    else
                    {
                        Debug.LogError("Invalid stats data in response.");
                        callback?.Invoke(null);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing stats: {e.Message}");
                    callback?.Invoke(null);
                }
            }
            else
            {
                Debug.LogError($"GetStats Error: {request.error}");
                callback?.Invoke(null);
            }
        }
    }

    /// <summary>
    /// Saves a run to the database with details such as score, week, deaths, escapes, and kills.
    /// </summary>
    /// <param name="score">The score for the run.</param>
    /// <param name="week">The week in which the run occurred.</param>
    /// <param name="deaths">The number of deaths during the run.</param>
    /// <param name="escapes">The number of escapes during the run.</param>
    /// <param name="totalCoinsDeposited">The total coins deposited during the run.</param>
    /// <param name="totalKills">The total kills during the run.</param>
    public void SaveRun(int score, int week, int deaths, int escapes, int totalCoinsDeposited, int totalKills)
    {
        StartCoroutine(SaveRunCoroutine(score, week, deaths, escapes, totalCoinsDeposited, totalKills));
    }

    /// <summary>
    /// Coroutine to handle saving a run to the database.
    /// </summary>
    private IEnumerator SaveRunCoroutine(int score, int week, int deaths, int escapes, int totalCoinsDeposited, int totalKills)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", userId);
        form.AddField("score", score);
        form.AddField("week", week);
        form.AddField("deaths", deaths);
        form.AddField("escapes", escapes);
        form.AddField("total_coins_deposited", totalCoinsDeposited);
        form.AddField("kills", totalKills);

        using (UnityWebRequest request = UnityWebRequest.Post($"{BaseUrl}?action=create-run", form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Run saved successfully: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"Error saving run: {request.error}");
            }
        }
    }

    /// <summary>
    /// Saves the stats to the database.
    /// </summary>
    public void SaveStats(int highestScore, int highestWeek, int totalDays, int totalCoinsDeposited, int totalCoinsLost, int totalDeaths, int totalEscapes, int totalKills, int totalGoblinKills, int totalSkeletonKills, int totalGhastKills, int totalWizardKills, int totalDemonKills, System.Action<bool> callback = null)
    {
        StartCoroutine(SaveStatsCoroutine(highestScore, highestWeek, totalDays, totalCoinsDeposited, totalCoinsLost, totalDeaths, totalEscapes, totalKills, totalGoblinKills, totalSkeletonKills, totalGhastKills, totalWizardKills, totalDemonKills, callback));
    }

    /// <summary>
    /// Coroutine to save stats to the database.
    /// </summary>
    private IEnumerator SaveStatsCoroutine(int highestScore, int highestWeek, int totalDays, int totalCoinsDeposited, int totalCoinsLost, int totalDeaths, int totalEscapes, int totalKills, int totalGoblinKills, int totalSkeletonKills, int totalGhastKills, int totalWizardKills, int totalDemonKills, System.Action<bool> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", userId);
        form.AddField("highest_score", highestScore);
        form.AddField("highest_week", highestWeek);
        form.AddField("total_days", totalDays);
        form.AddField("total_coins_deposited", totalCoinsDeposited);
        form.AddField("total_coins_lost", totalCoinsLost);
        form.AddField("total_deaths", totalDeaths);
        form.AddField("total_escapes", totalEscapes);
        form.AddField("total_kills", totalKills);
        form.AddField("total_goblin_kills", totalGoblinKills);
        form.AddField("total_skeleton_kills", totalSkeletonKills);
        form.AddField("total_ghast_kills", totalGhastKills);
        form.AddField("total_wizard_kills", totalWizardKills);
        form.AddField("total_demon_kills", totalDemonKills);

        using (UnityWebRequest request = UnityWebRequest.Put($"{BaseUrl}?action=update-stats", form.data))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Stats updated successfully: {request.downloadHandler.text}");
                StatsResponse response = JsonUtility.FromJson<StatsResponse>(request.downloadHandler.text);

                if (response.success)
                {
                    callback?.Invoke(true);
                }
                else
                {
                    Debug.LogError($"API error: {response.error}");
                    callback?.Invoke(false);
                }
            }
            else
            {
                Debug.LogError($"SaveStats Error: {request.error}");
                callback?.Invoke(false);
            }
        }
    }
}

/// <summary>
/// Response for user-related requests.
/// Contains information about success, the user object, and any error messages.
/// </summary>
[System.Serializable]
public class UserResponse
{
    public bool success;
    public User user;
    public string error;
}

/// <summary>
/// Represents a user in the database.
/// </summary>
[System.Serializable]
public class User
{
    public int id;
    public string guest_id;
    public string created_at;
}

/// <summary>
/// Response for user creation requests.
/// Contains success status, the user ID, and any error messages.
/// </summary>
[System.Serializable]
public class UserInsertResponse
{
    public bool success;
    public int user_id;
    public string error;
}

/// <summary>
/// Response for stats-related requests.
/// Contains information about success, the stats object, and any error messages.
/// </summary>
[System.Serializable]
public class StatsResponse
{
    public bool success;
    public StatsData stats;
    public string error;
}

/// <summary>
/// Response for fetching runs data.
/// Contains information about success, a list of runs, and any error messages.
/// </summary>
[System.Serializable]
public class RunListResponse
{
    public bool success;
    public List<RunData> runs;
    public string error;
}

/// <summary>
/// Represents a single run in the database.
/// </summary>
[System.Serializable]
public class RunData
{
    public int id;
    public int user_id;
    public int score;
    public int week;
    public int deaths;
    public int escapes;
    public int total_coins_deposited;
    public int kills;
    public string date;
}

/// <summary>
/// Represents stats data for a user.
/// </summary>
[System.Serializable]
public class StatsData
{
    public int highest_score;
    public int highest_week;
    public int total_days;
    public int total_coins_deposited;
    public int total_coins_lost;
    public int total_deaths;
    public int total_escapes;
    public int total_kills;
    public int total_goblin_kills;
    public int total_skeleton_kills;
    public int total_ghast_kills;
    public int total_wizard_kills;
    public int total_demon_kills;
}


