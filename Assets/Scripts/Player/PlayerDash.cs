using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public int dashDamage => (int)GameManager.Instance.GetStat("dashDamage");
    public float dashSpeed => GameManager.Instance.GetStat("dashSpeed");
    public float dashDistance => GameManager.Instance.GetStat("dashDistance");
    public float dashCooldown => GameManager.Instance.GetStat("dashCooldown");
    private float lastDashTime = -Mathf.Infinity;

    private bool isDashing = false;
    

    public AbilityCooldownDisplay dashCooldownUI;
    
    public AudioClip dashSound;

    private TrailRenderer dashTrail;
    private Collider2D dashHitbox;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Player player;

    private void Awake()
    {
        dashHitbox = GetComponent<Collider2D>();
        dashTrail = GetComponent<TrailRenderer>();
        rb = GetComponentInParent<Rigidbody2D>();
        audioSource = GetComponentInParent<AudioSource>();
        player = GetComponentInParent<Player>();
    }

    void Update()
    {
        HandleDash();
    }

    private void HandleDash()
    {
        if (UserInput.Instance.DashInput && Time.time > lastDashTime + dashCooldown && !isDashing)
        {
            lastDashTime = Time.time;
            dashCooldownUI.StartCooldown(dashCooldown);

            Vector2 dashDirection = UserInput.Instance.MoveInput != Vector2.zero
                ? UserInput.Instance.MoveInput.normalized
                : player.LastFacedDirection;

            StartCoroutine(DashCoroutine(dashDirection));
        }
    }

    private IEnumerator DashCoroutine(Vector2 dashDirection)
    {
        isDashing = true;
        player.isInvincible = true;
        dashHitbox.enabled = true;
        dashTrail.emitting = true;
        SoundEffects.Instance.PlaySound(dashSound);

        float dashDuration = dashDistance / dashSpeed;
        float elapsedTime = 0f;
        Vector2 dashVelocity = dashDirection * dashSpeed;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        while (elapsedTime < dashDuration)
        {
            rb.velocity = dashVelocity;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        dashHitbox.enabled = false;
        dashTrail.emitting = false;
        player.isInvincible = false;
        isDashing = false;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (GameManager.Instance.IsSpecialEffectActive("KillingBlow") && enemy.currentHealth - dashDamage <= 0)
                {
                    ResetCooldown();
                    Debug.Log("Killing Blow Activated: Dash Reset");
                }

                enemy.TakeDamage(dashDamage);
                
            }
        }
    }

    public void ResetCooldown()
    {
        lastDashTime = -Mathf.Infinity; 
        dashCooldownUI.ResetCooldownUI(); 
        Debug.Log("Dash cooldown reset due to KillingBlow effect.");
    }
}
