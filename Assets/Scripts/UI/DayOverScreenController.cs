using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class DayOverScreenController : MonoBehaviour
{
    [Header("References to UI Elements")]
    public GameObject dayOverScreen;      
    public TMP_Text titleText;             
    public TMP_Text infoText;              
    public TMP_Text coinValueText;        
    public TMP_Text livesRemainingText;    

    /// <summary>
    /// Displays the "You Escaped!" screen with information about coins deposited.
    /// </summary>
    public void ShowEscapeScreen()
    {

        titleText.text = "You Escaped!";
        infoText.text = "Coins Deposited:";
        coinValueText.text = $"x {GameManager.Instance.currentCoins}";
        livesRemainingText.text = ""; 


        DisplayScreen();
    }

    /// <summary>
    /// Displays the "You Died!" screen with information about lost coins and lives remaining.
    /// </summary>
    public void ShowDieScreen()
    {
        titleText.text = "You Died!";
        infoText.text = "Coins Lost:";
        coinValueText.text = $"x {GameManager.Instance.currentCoins}"; 
        livesRemainingText.text = $"Lives Remaining: {GameManager.Instance.lives}"; 


        DisplayScreen();
    }

    private void DisplayScreen()
    {
        StartCoroutine(WaitAndDisplayScreen());
    }

    private IEnumerator WaitAndDisplayScreen()
    {
        yield return new WaitForSeconds(0.5f);  
        dayOverScreen.SetActive(true);        
        Time.timeScale = 0f;                
    }

    /// <summary>
    /// Called when the "Continue" button is pressed. Takes the player back to the Game Menu.
    /// </summary>
    public void OnContinueButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameMenu");
    }

    /// <summary>
    /// Called when the "Main Menu" button is pressed. Takes the player to the Main Menu.
    /// </summary>
    public void OnMainMenuButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
