using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerProjectile : MonoBehaviour
{
    public float damage;  
    public float lifeTime = 3f;   

    private void Awake()
    {
        StartCoroutine(DestroyAfterTime());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Apply dagger damage to the enemy
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Enemy hit by dagger! Damage applied: " + damage);
            }

            Destroy(gameObject);
        }
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
