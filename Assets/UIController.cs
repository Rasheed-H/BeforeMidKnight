using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

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

    private float gameTime = 1080;  // 1080 - Start time set to 6:00 PM in minutes 
    private float timeIncrementPerSecond = 0.6f; // In-game minutes per real-time second
    private bool outOfTime = false; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent multiple instances
        }
    }

    private void Start()
    {
        UpdateCoinText();
        UpdateTimerText();
    }

    private void Update()
    {
        gameTime += Time.deltaTime * timeIncrementPerSecond; // Increment game time by 0.6 minutes per real-time second
        UpdateTimerText();

        if (gameTime >= 1440 && !outOfTime) // 1440 minutes = 12:00 AM
        {
            outOfTime = true;
            ShowOutOfTimeScreen();
        }
    }

    public void UpdateCoinText()
    {
        coinText.text = $"x {GameManager.Instance.coinsHolding}";
    }

    private void UpdateTimerText()
    {
        int hours = Mathf.FloorToInt(gameTime / 60) % 12;
        if (hours == 0) hours = 12;
        int minutes = Mathf.FloorToInt(gameTime % 60);
        string period = gameTime >= 720 ? "PM" : "AM";

        timerText.text = $"{hours:D2}:{minutes:D2} {period}";
    }

    public string GetCurrentTime()
    {
        int hours = Mathf.FloorToInt(gameTime / 60) % 12;
        if (hours == 0) hours = 12;
        int minutes = Mathf.FloorToInt(gameTime % 60);
        string period = gameTime >= 720 ? "PM" : "AM";

        return $"{hours:D2}:{minutes:D2} {period}";
    }

    public void ShowEscapeScreen()
    {
        endLevelScreen.SetActive(true);
        titleText.text = "You Escaped!";
        coinsInfoText.text = "Coins Holding:";
        coinsValueText.text = $"x {GameManager.Instance.coinsHolding}";
        timeText.text = GetCurrentTime();

        DisplayWinScreen();
    }

    public void ShowDieScreen()
    {
        endLevelScreen.SetActive(true);
        titleText.text = "You Died!";
        coinsInfoText.text = "Coins Lost:";
        coinsValueText.text = $"x {GameManager.Instance.coinsHolding}";
        timeText.text = GetCurrentTime();

        DisplayLostScreen();
    }

    public void ShowOutOfTimeScreen()
    {
        player.Die();
        endLevelScreen.SetActive(true);
        titleText.text = "Out Of Time!";
        coinsInfoText.text = "Coins Lost:";
        coinsValueText.text = $"x {GameManager.Instance.coinsHolding}";
        timeText.text = "12:00 AM";
        timeText.color = Color.red;

        DisplayLostScreen();
    }

    private void DisplayWinScreen()
    {
        StartCoroutine(TransitionBackgroundEffect());

        nextFloorButton.SetActive(true);
        exitDungeonButton.SetActive(true);
    }

    private void DisplayLostScreen()
    {
        StartCoroutine(TransitionBackgroundEffect());
        GameManager.Instance.coinsHolding = 0;
        nextFloorButton.SetActive(false);
        exitDungeonButton.SetActive(true);
    }

    private IEnumerator TransitionBackgroundEffect()
    {
        transitionBackground.SetActive(true);
        UnityEngine.UI.Image transitionImage = transitionBackground.GetComponent<UnityEngine.UI.Image>();
        Color color = transitionImage.color;

        float delayBeforeFade = 0.35f; 
        yield return new WaitForSeconds(delayBeforeFade);

        // Fade to white
        float fadeDuration = 0.4f;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            transitionImage.color = color;
            yield return null;
        }

        color.a = 1;
        transitionImage.color = color;

        // Fade to black
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color = Color.Lerp(Color.white, Color.black, t / fadeDuration);
            transitionImage.color = color;
            yield return null;
        }

        color = Color.black;
        transitionImage.color = color;

        // Fade to 70% transparency
        float transparencyTarget = 0.9f; // 1 - 70% transparency = 0.3 alpha
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

    private void ActivateEndScreenElements()
    {
        titleText.gameObject.SetActive(true);
        coinsInfoText.gameObject.SetActive(true);
        coinsValueText.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
    }

    public void OnNextFloorButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("DungeonMainTest"); // Replace with actual next floor scene
    }

    public void OnExitDungeonButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameMenu");
    }
}
