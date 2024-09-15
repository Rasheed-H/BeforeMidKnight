using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerAttack : MonoBehaviour
{
    public GameObject daggerPrefab;     
    public float damage = 5f;     
    public float speed = 10f;    


    public void PerformDaggerAttack(Vector2 attackDirection)
    {
        GameObject dagger = Instantiate(daggerPrefab, transform.position, Quaternion.identity);

        Rigidbody2D rb = dagger.GetComponent<Rigidbody2D>();

        rb.velocity = attackDirection * speed;

        DaggerProjectile daggerProjectile = dagger.GetComponent<DaggerProjectile>();
        if (daggerProjectile != null)
        {
            daggerProjectile.damage = damage;
        }

        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
        dagger.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}