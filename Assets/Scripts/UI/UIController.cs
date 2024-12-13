using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Manages the game's user interface, including coin and timer updates, end-level screens,
/// and transitions between game states.
/// </summary>
public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [Header("References to Player and UI Elements")]
    public Player player;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text timerText;

    [Header("End Level Screen")]
    [SerializeField] private GameObject endLevelScreen;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text coinsInfoText;
    [SerializeField] private TMP_Text coinsValueText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private GameObject transitionBackground;
    [SerializeField] private GameObject nextFloorButton;
    [SerializeField] private GameObject exitDungeonButton;

    private float gameTime = GameManager.Instance.time;  // 1080 - Start time set to 6:00 PM in minutes 
    private float timeIncrementPerSecond = 0.6f; // In-game minutes per real-time second
    private bool outOfTime = false;
    private bool stopTimer = false;


    /// <summary>
    /// Ensures there is only one instance of the UIController and destroys duplicates.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    /// <summary>
    /// Initializes the UI by updating the coin and timer text at the start of the game.
    /// </summary>
    private void Start()
    {
        UpdateCoinText();
        UpdateTimerText();
    }

    /// <summary>
    /// Updates the in-game time and timer display. Triggers the out-of-time screen if the game reaches 12:00 AM.
    /// </summary>
    private void Update()
    {
        gameTime += Time.deltaTime * timeIncrementPerSecond; 
        
        if (!stopTimer) 
        {
            UpdateTimerText();
        }

        if (gameTime >= 1440 && !outOfTime) // 1440 minutes = 12:00 AM
        {
            outOfTime = true;
            ShowOutOfTimeScreen();
        }
    }

    /// <summary>
    /// Updates the coin display with the current number of coins the player is holding.
    /// </summary>
    public void UpdateCoinText()
    {
        coinText.text = $"x {GameManager.Instance.coinsHolding}";
    }

    /// <summary>
    /// Converts the in-game time into a 12-hour format and updates the timer UI.
    /// </summary>
    private void UpdateTimerText()
    {
        int hours = Mathf.FloorToInt(gameTime / 60) % 12;
        if (hours == 0) hours = 12;
        int minutes = Mathf.FloorToInt(gameTime % 60);
        string period = gameTime >= 720 ? "PM" : "AM";

        timerText.text = $"{hours:D2}:{minutes:D2} {period}";
    }

    /// <summary>
    /// Returns the current in-game time as a formatted string in 12-hour format.
    /// </summary>
    public string GetCurrentTime()
    {
        int hours = Mathf.FloorToInt(gameTime / 60) % 12;
        if (hours == 0) hours = 12;
        int minutes = Mathf.FloorToInt(gameTime % 60);
        string period = gameTime >= 720 ? "PM" : "AM";

        return $"{hours:D2}:{minutes:D2} {period}";
    }

    /// <summary>
    /// Displays the end-level screen for a successful dungeon escape, showing collected coins and the current time.
    /// </summary>
    public void ShowEscapeScreen()
    {
        stopTimer = true;
        endLevelScreen.SetActive(true);
        titleText.text = "You Escaped!";
        coinsInfoText.text = "Coins Holding:";
        coinsValueText.text = $"x {GameManager.Instance.coinsHolding}";
        timeText.text = "Time: " + GetCurrentTime();

        DisplayWinScreen();
    }

    /// <summary>
    /// Displays the end-level screen when the player dies, showing coins lost and the current time.
    /// </summary>
    public void ShowDieScreen()
    {
        stopTimer = true;
        endLevelScreen.SetActive(true);
        titleText.text = "You Died!";
        coinsInfoText.text = "Coins Lost:";
        coinsValueText.text = $"x {GameManager.Instance.coinsHolding}";
        timeText.text = "Time: " + GetCurrentTime();

        DisplayLostScreen();
    }

    /// <summary>
    /// Displays the out-of-time screen when the player fails to escape the dungeon before midnight.
    /// Marks the player as dead, shows coins lost, and updates the time display in red.
    /// </summary>
    public void ShowOutOfTimeScreen()
    {
        stopTimer = true;
        player.Die();
        endLevelScreen.SetActive(true);
        titleText.text = "Out Of Time!";
        coinsInfoText.text = "Coins Lost:";
        coinsValueText.text = $"x {GameManager.Instance.coinsHolding}";
        timeText.text = "Time: 12:00 AM";
        timeText.color = Color.red;

        DisplayLostScreen();
    }

    /// <summary>
    /// Prepares the win screen by transitioning the background and enabling buttons to proceed to the next floor or exit the dungeon.
    /// </summary>
    private void DisplayWinScreen()
    {
        StartCoroutine(TransitionBackgroundEffect());

        nextFloorButton.SetActive(true);
        exitDungeonButton.SetActive(true);
    }

    /// <summary>
    /// Handles the visual and functional setup for the loss screen, applying effects such as coin loss and showing the exit option.
    /// </summary>
    private void DisplayLostScreen()
    {
        StartCoroutine(TransitionBackgroundEffect());
        if (GameManager.Instance.IsSpecialEffectActive("DeathDepositor"))
        {
            GameManager.Instance.coinsHolding = Mathf.FloorToInt(GameManager.Instance.coinsHolding * 0.3f);
        }
        else
        {
            GameManager.Instance.LoseHolding();
        }
        nextFloorButton.SetActive(false);
        exitDungeonButton.SetActive(true);
    }

    /// <summary>
    /// Gradually fades the background to black during the transition to the end-level screen.
    /// </summary>
    private IEnumerator TransitionBackgroundEffect()
    {
        transitionBackground.SetActive(true);
        UnityEngine.UI.Image transitionImage = transitionBackground.GetComponent<UnityEngine.UI.Image>();
        Color color = transitionImage.color;

        float delayBeforeFade = 0.35f; 
        yield return new WaitForSeconds(delayBeforeFade);

        float fadeDuration = 0.4f;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            transitionImage.color = color;
            yield return null;
        }

        color.a = 1;
        transitionImage.color = color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color = Color.Lerp(Color.white, Color.black, t / fadeDuration);
            transitionImage.color = color;
            yield return null;
        }

        color = Color.black;
        transitionImage.color = color;

        float transparencyTarget = 0.9f; 
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(1, transparencyTarget, t / fadeDuration);
            transitionImage.color = color;
            yield return null;
        }

        color.a = transparencyTarget;
        transitionImage.color = color;

        Time.timeScale = 0f;
        ActivateEndScreenElements();
    }

    /// <summary>
    /// Activates all UI elements on the end-level screen to display the appropriate information.
    /// </summary>
    private void ActivateEndScreenElements()
    {
        titleText.gameObject.SetActive(true);
        coinsInfoText.gameObject.SetActive(true);
        coinsValueText.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Loads the next floor of the dungeon, resuming time and preserving the current in-game time.
    /// </summary>
    public void OnNextFloorButtonClicked()
    {
        Time.timeScale = 1f;
        GameManager.Instance.time = gameTime;
        SceneManager.LoadScene("Dungeon"); 
    }

    /// <summary>
    /// Exits the dungeon, deposits the player's coins, and returns to the game menu.
    /// </summary>
    public void OnExitDungeonButtonClicked()
    {
        Time.timeScale = 1f;
        GameManager.Instance.DepositHolding();
        SceneManager.LoadScene("GameMenu");
    }
}
