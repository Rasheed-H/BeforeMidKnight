using UnityEngine;

public class HealthPotion : MonoBehaviour
{

    public int healthRestored = 2;


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();

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
