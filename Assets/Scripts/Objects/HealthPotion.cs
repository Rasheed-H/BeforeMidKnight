using UnityEngine;

public class HealthPotion : MonoBehaviour
{

    public int healthRestored = 2;


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {

                if (player.currentHealth < player.maxHealth)
                {
                    player.Heal(healthRestored);  
                    Destroy(gameObject);  
                }
            }
        }
    }
}
