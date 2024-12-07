using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value;

    public void InitializeCoin(int coinValue)
    {
        value = coinValue;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.AddCoins(value);
            UIController.Instance.UpdateCoinText();
            Destroy(gameObject);  
        }
    }
}
