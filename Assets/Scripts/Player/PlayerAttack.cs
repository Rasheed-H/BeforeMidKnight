using UnityEngine;

/// <summary>
/// Enum representing the two weapon types: Slash and Dagger.
/// </summary>
public enum WeaponType
{
    Slash,
    Dagger
}

/// <summary>
/// Manages the player's attack system, including melee (slash) and ranged (dagger) attacks,
/// cooldowns, and weapon switching.
/// </summary>
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

    private Animator animator;

    public Collider2D AttackUpHitbox;
    public Collider2D AttackDownHitbox;
    public Collider2D AttackLeftHitbox;
    public Collider2D AttackRightHitbox;

    /// <summary>
    /// Initializes the player attack component by setting up the animator reference.
    /// </summary>
    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    /// <summary>
    /// Continuously checks for attack inputs and weapon switch inputs, handling each appropriately.
    /// </summary>
    void Update()
    {
        HandleAttacks();
        HandleWeaponSwitch();
    }

    /// <summary>
    /// Handles attack inputs for the currently selected weapon, triggering the appropriate attack
    /// based on direction and cooldown state.
    /// </summary>
    private void HandleAttacks()
    {
        if (UserInput.Instance.AttackUpInput) PerformAttack(Vector2.up, AttackUpHitbox, "AttackUp");
        else if (UserInput.Instance.AttackDownInput) PerformAttack(Vector2.down, AttackDownHitbox, "AttackDown");
        else if (UserInput.Instance.AttackLeftInput) PerformAttack(Vector2.left, AttackLeftHitbox, "AttackLeft");
        else if (UserInput.Instance.AttackRightInput) PerformAttack(Vector2.right, AttackRightHitbox, "AttackRight");
    }

    /// <summary>
    /// Executes the selected attack based on the current weapon, direction, and cooldowns.
    /// Activates hitboxes for melee attacks and instantiates projectiles for ranged attacks.
    /// </summary>
    private void PerformAttack(Vector2 direction, Collider2D hitbox, string animationTrigger)
    {
        if (currentWeapon == WeaponType.Slash && Time.time > lastSlashTime + slashCooldown)
        {
            lastSlashTime = Time.time;
            slashCooldownUI.StartCooldown(slashCooldown);
            DisableAllHitboxes();
            hitbox.enabled = true;
            animator.SetTrigger(animationTrigger);
            SoundEffects.Instance.PlaySound(slashSound);
        }
        else if (currentWeapon == WeaponType.Dagger && Time.time > lastDaggerTime + daggerCooldown)
        {
            lastDaggerTime = Time.time;
            daggerCooldownUI.StartCooldown(daggerCooldown);
            ThrowDagger(direction);
        }
    }

    /// <summary>
    /// Instantiates and launches a dagger projectile in the specified direction, applying damage
    /// and visual effects.
    /// </summary>
    private void ThrowDagger(Vector2 direction)
    {
        GameObject dagger = Instantiate(daggerProjectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = dagger.GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * daggerProjectileSpeed;
        DaggerProjectile daggerProjectile = dagger.GetComponent<DaggerProjectile>();
        if (daggerProjectile != null)
        {
            daggerProjectile.damage = daggerDamage;
        }
        SoundEffects.Instance.PlaySound(daggerThrowSound);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        dagger.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// Switches between the Slash and Dagger weapons, updating the UI and logging the weapon change.
    /// </summary>
    private void HandleWeaponSwitch()
    {
        if (UserInput.Instance.SwitchWeaponInput)
        {
            currentWeapon = currentWeapon == WeaponType.Slash ? WeaponType.Dagger : WeaponType.Slash;

            if (currentWeapon == WeaponType.Slash)
            {
                slashCooldownUI.SetCooldownFrameActive(true);
                daggerCooldownUI.SetCooldownFrameActive(false);
            }
            else if (currentWeapon == WeaponType.Dagger)
            {
                slashCooldownUI.SetCooldownFrameActive(false);
                daggerCooldownUI.SetCooldownFrameActive(true);
            }
        }
    }

    /// <summary>
    /// Disables all melee attack hitboxes. Called from animation event.
    /// </summary>
    public void DisableAllHitboxes()
    {
        AttackUpHitbox.enabled = false;
        AttackDownHitbox.enabled = false;
        AttackLeftHitbox.enabled = false;
        AttackRightHitbox.enabled = false;
    }
}
