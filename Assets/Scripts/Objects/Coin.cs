using UnityEngine;

/// <summary>
/// Represents a collectible coin in the game that adds value to the player's total coins upon collision.
/// </summary>
public class Coin : MonoBehaviour
{
    public int value;
    [SerializeField] private AudioClip coinSound;

    /// <summary>
    /// Initializes the coin with a specified value.
    /// </summary>
    public void InitializeCoin(int coinValue)
    {
        value = coinValue;
    }

    /// <summary>
    /// Detects when the player collides with the coin, adds its value to the player's total coins,
    /// updates the coin UI, plays a sound effect, and destroys the coin object.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.AddCoins(value);
            UIController.Instance.UpdateCoinText();
            SoundEffects.Instance.PlaySound(coinSound);
            Destroy(gameObject);  
        }
    }
}
