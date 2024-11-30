using UnityEngine;

/// <summary>
/// Enum representing the two weapon types: Slash and Dagger.
/// </summary>
public enum WeaponType
{
    Slash,
    Dagger
}

public class PlayerAttack : MonoBehaviour
{
    public int slashDamage => (int)GameManager.Instance.GetStat("slashDamage");
    public int daggerDamage => (int)GameManager.Instance.GetStat("daggerDamage");
    public float daggerProjectileSpeed => GameManager.Instance.GetStat("daggerSpeed");

    public float slashCooldown => GameManager.Instance.GetStat("slashCooldown");
    public float daggerCooldown => GameManager.Instance.GetStat("daggerCooldown");
    private float lastSlashTime = -Mathf.Infinity;
    private float lastDaggerTime = -Mathf.Infinity;

    private WeaponType currentWeapon = WeaponType.Slash;

    public AbilityCooldownDisplay slashCooldownUI;
    public AbilityCooldownDisplay daggerCooldownUI;

    public GameObject daggerProjectilePrefab;
    public AudioClip slashSound;
    public AudioClip daggerThrowSound;

    private AudioSource audioSource;
    private Animator animator;

    public Collider2D AttackUpHitbox;
    public Collider2D AttackDownHitbox;
    public Collider2D AttackLeftHitbox;
    public Collider2D AttackRightHitbox;

    private void Awake()
    {
        audioSource = GetComponentInParent<AudioSource>();
        animator = GetComponentInParent<Animator>();
    }

    void Update()
    {
        HandleAttacks();
        HandleWeaponSwitch();
    }

    private void HandleAttacks()
    {
        if (UserInput.Instance.AttackUpInput) PerformAttack(Vector2.up, AttackUpHitbox, "AttackUp");
        else if (UserInput.Instance.AttackDownInput) PerformAttack(Vector2.down, AttackDownHitbox, "AttackDown");
        else if (UserInput.Instance.AttackLeftInput) PerformAttack(Vector2.left, AttackLeftHitbox, "AttackLeft");
        else if (UserInput.Instance.AttackRightInput) PerformAttack(Vector2.right, AttackRightHitbox, "AttackRight");
    }

    private void PerformAttack(Vector2 direction, Collider2D hitbox, string animationTrigger)
    {
        if (currentWeapon == WeaponType.Slash && Time.time > lastSlashTime + slashCooldown)
        {
            lastSlashTime = Time.time;
            slashCooldownUI.StartCooldown(slashCooldown);
            DisableAllHitboxes();
            hitbox.enabled = true;
            animator.SetTrigger(animationTrigger);
            audioSource.PlayOneShot(slashSound);
        }
        else if (currentWeapon == WeaponType.Dagger && Time.time > lastDaggerTime + daggerCooldown)
        {
            lastDaggerTime = Time.time;
            daggerCooldownUI.StartCooldown(daggerCooldown);
            ThrowDagger(direction);
        }
    }

    private void ThrowDagger(Vector2 direction)
    {
        GameObject dagger = Instantiate(daggerProjectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = dagger.GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * daggerProjectileSpeed;
        audioSource.PlayOneShot(daggerThrowSound);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        dagger.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void HandleWeaponSwitch()
    {
        if (UserInput.Instance.SwitchWeaponInput)
        {
            currentWeapon = currentWeapon == WeaponType.Slash ? WeaponType.Dagger : WeaponType.Slash;
            Debug.Log("Switched to " + currentWeapon + " weapon");
        }
    }

    public void DisableAllHitboxes()
    {
        AttackUpHitbox.enabled = false;
        AttackDownHitbox.enabled = false;
        AttackLeftHitbox.enabled = false;
        AttackRightHitbox.enabled = false;
    }
}
