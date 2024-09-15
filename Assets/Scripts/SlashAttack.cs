using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAttack : MonoBehaviour
{
    public Collider2D AttackRightHitbox;
    public Collider2D AttackLeftHitbox;
    public Collider2D AttackUpHitbox;
    public Collider2D AttackDownHitbox;
    private Animator animator;
    public float damage = 10f;

    void Awake()
    {
        animator = GetComponent<Animator>(); 
    }

    public void PerformSlashAttack(Vector2 attackDirection)
    {
        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

        if (angle >= 45 && angle < 135)
        {
            animator.SetTrigger("AttackUp");
        }
        else if (angle >= -135 && angle < -45)
        {
            animator.SetTrigger("AttackDown");
        }
        else if (angle >= -45 && angle < 45)
        {
            animator.SetTrigger("AttackRight");
        }
        else
        {
            animator.SetTrigger("AttackLeft");
        }
    }

    public void EnableHitbox(string direction)
    {
        DisableAllHitboxes(); 

        switch (direction)
        {
            case "Right":
                AttackRightHitbox.enabled = true;
                break;
            case "Left":
                AttackLeftHitbox.enabled = true;
                break;
            case "Up":
                AttackUpHitbox.enabled = true;
                break;
            case "Down":
                AttackDownHitbox.enabled = true;
                break;
            default:
                Debug.LogError("Invalid direction for EnableHitbox");
                break;
        }
        Debug.Log(direction + " hitbox enabled");
    }

    public void DisableAllHitboxes()
    {
        AttackRightHitbox.enabled = false;
        AttackLeftHitbox.enabled = false;
        AttackUpHitbox.enabled = false;
        AttackDownHitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Enemy hit by melee! Damage applied: " + damage);
            }
        }
    }

}
