using UnityEngine;

public class BlueBall : MonoBehaviour
{
    public int damage = 1;
    public float speed;
    public float lifespan = 5f;

    private void Start()
    {
        speed = GameManager.Instance.wizardProjSpeed;
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
        else if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
