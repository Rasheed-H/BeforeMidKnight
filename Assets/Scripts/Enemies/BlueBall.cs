using UnityEngine;

public class BlueBall : MonoBehaviour
{
    public int damage = 1;
    public float speed = 5f;
    public float lifespan = 5f;

    private void Start()
    {
        Destroy(gameObject, lifespan); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject); 
        }
        else if (collision.CompareTag("Wall") || collision.CompareTag("Slash"))
        {
            Destroy(gameObject);
        }
    }
}
