using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Enum representing the two weapon types: Slash and Dagger.
/// </summary>
public enum WeaponType
{
    Slash,
    Dagger
}


/// <summary>
/// PlayerController class handles player movement, attacks, dashing, taking damage, 
/// and managing invincibility and cooldowns for attacks.
/// </summary>
public class PlayerController : MonoBehaviour
{
     
    // Player Properties
    public int maxHealth = 10;
    public int currentHealth;
    public float moveSpeed = 5f;
    public float invincibilityDurationAfterHit = 3f;

    // Attack Properties
    public int slashDamage = 10;
    public int daggerDamage = 5;
    public int dashDamage = 10;
    public float daggerProjectileSpeed = 10f;
    public float dashSpeed = 20f;        
    public float dashDistance = 5f;

    // Cooldown Variables
    public float slashCooldown = 1f;
    public float daggerCooldown = 0.5f;
    public float dashCooldown = 2.5f;
    private float lastSlashTime = -Mathf.Infinity;
    private float lastDaggerTime = -Mathf.Infinity;
    private float lastDashTime = -Mathf.Infinity;

    // Player States
    private bool isDashing = false;
    private bool isAlive = true;
    private bool isInvincible = false;
    private Vector2 lastFacedDirection = Vector2.down;
    private Vector2 moveInput;

    // Player Components
    public Collider2D AttackRightHitbox;
    public Collider2D AttackLeftHitbox;
    public Collider2D AttackUpHitbox;
    public Collider2D AttackDownHitbox;
    public GameObject daggerProjectilePrefab;
    public Collider2D dashHitbox;        
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerInputActions playerInputActions;
    private AudioSource audioSource;
    public TrailRenderer dashTrail;

    // Sound Effects
    public AudioClip slashSound;         
    public AudioClip daggerThrowSound;   
    public AudioClip dashSound;          

    // Default weapon
    private WeaponType currentWeapon = WeaponType.Slash;


    /// <summary>
    /// Called when the script instance is being loaded. Initializes references to components and sets default health values.
    /// </summary>
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInputActions = new PlayerInputActions();
        audioSource = GetComponent<AudioSource>();
        currentHealth = maxHealth;
        DisableAllHitboxes();
    }


    /// <summary>
    /// Enables input listeners when the player is enabled. Handles movement, attacks, and dashing.
    /// </summary>
    void OnEnable()
    {
        if (!isAlive)
            return;
        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMove;
        playerInputActions.Player.AttackUp.performed += OnAttackUp;
        playerInputActions.Player.AttackDown.performed += OnAttackDown;
        playerInputActions.Player.AttackLeft.performed += OnAttackLeft;
        playerInputActions.Player.AttackRight.performed += OnAttackRight;
        playerInputActions.Player.SwitchWeapon.performed += OnSwitchWeapon;
        playerInputActions.Player.Dash.performed += OnDash;
        playerInputActions.Player.Enable();
    }


    /// <summary>
    /// Disables input listeners when the player is disabled.
    /// </summary>
    void OnDisable()
    {
        playerInputActions.Player.Move.performed -= OnMove;
        playerInputActions.Player.Move.canceled -= OnMove;
        playerInputActions.Player.AttackUp.performed -= OnAttackUp;
        playerInputActions.Player.AttackDown.performed -= OnAttackDown;
        playerInputActions.Player.AttackLeft.performed -= OnAttackLeft;
        playerInputActions.Player.AttackRight.performed -= OnAttackRight;
        playerInputActions.Player.SwitchWeapon.performed -= OnSwitchWeapon;
        playerInputActions.Player.Dash.performed -= OnDash;
        playerInputActions.Player.Disable();
    }


    /// <summary>
    /// Called when the player moves. Updates movement input and triggers animation updates.
    /// </summary>
    /// <param name="context">The movement input context</param>
    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        UpdateMovementAnimation();
    }


    /// <summary>
    /// Updates the player's movement based on input each physics frame.
    /// </summary>
    void FixedUpdate()
    {
        if (isDashing)
            return;

        rb.velocity = moveInput * moveSpeed;
    }


    /// <summary>
    /// Updates the player's movement animation and tracks the last faced direction.
    /// </summary>
    void UpdateMovementAnimation()
    {
        if (!isAlive)
            return;

        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool("isRunning", isMoving);

        if (isMoving)
        {
            animator.SetFloat("MoveX", moveInput.x);
            animator.SetFloat("MoveY", moveInput.y);
            lastFacedDirection = moveInput.normalized;
        }
    }


    /// <summary>
    /// Switches between the Slash and Dagger weapons when the corresponding input is received.
    /// </summary>
    /// <param name="context">The input context for switching weapons</param>
    private void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        if (currentWeapon == WeaponType.Slash)
        {
            currentWeapon = WeaponType.Dagger;
            Debug.Log("Switched to Dagger weapon");
        }
        else
        {
            currentWeapon = WeaponType.Slash;
            Debug.Log("Switched to Slash weapon");
        }
    }


    /// <summary>
    /// Handles the logic for the upward attack. Activates the hitbox and triggers animations based on the weapon type.
    /// </summary>
    private void OnAttackUp(InputAction.CallbackContext context)
    {
        if (isDashing)
            return;

        if (currentWeapon == WeaponType.Slash && Time.time > lastSlashTime + slashCooldown)
        {
            lastSlashTime = Time.time;
            DisableAllHitboxes();
            AttackUpHitbox.enabled = true;
            animator.SetTrigger("AttackUp");
            audioSource.PlayOneShot(slashSound);
        }
        if (currentWeapon == WeaponType.Dagger && Time.time > lastDaggerTime + daggerCooldown)
        {
            lastDaggerTime = Time.time;
            ThrowDagger(Vector2.up);
        }
        else
            return;
    }


    /// <summary>
    /// Handles the logic for the upward attack. Activates the hitbox and triggers animations based on the weapon type.
    /// </summary>
    private void OnAttackDown(InputAction.CallbackContext context)
    {
        if (isDashing)
            return;

        if (currentWeapon == WeaponType.Slash && Time.time > lastSlashTime + slashCooldown)
        {
            lastSlashTime = Time.time;
            DisableAllHitboxes();
            AttackDownHitbox.enabled = true;
            animator.SetTrigger("AttackDown");
            audioSource.PlayOneShot(slashSound);
        }
        if (currentWeapon == WeaponType.Dagger && Time.time > lastDaggerTime + daggerCooldown)
        {
            lastDaggerTime = Time.time;
            ThrowDagger(Vector2.down);
        }
        else
            return;
    }


    /// <summary>
    /// Handles the logic for the upward attack. Activates the hitbox and triggers animations based on the weapon type.
    /// </summary>
    private void OnAttackLeft(InputAction.CallbackContext context)
    {
        if (isDashing)
            return;

        if (currentWeapon == WeaponType.Slash && Time.time > lastSlashTime + slashCooldown)
        {
            lastSlashTime = Time.time;
            DisableAllHitboxes();
            AttackLeftHitbox.enabled = true;
            animator.SetTrigger("AttackLeft");
            audioSource.PlayOneShot(slashSound);
        }
        if (currentWeapon == WeaponType.Dagger && Time.time > lastDaggerTime + daggerCooldown)
        {
            lastDaggerTime = Time.time;
            ThrowDagger(Vector2.left);
        }
        else
            return;
    }


    /// <summary>
    /// Handles the logic for the upward attack. Activates the hitbox and triggers animations based on the weapon type.
    /// </summary>
    private void OnAttackRight(InputAction.CallbackContext context)
    {
        if (isDashing)
            return;

        if (currentWeapon == WeaponType.Slash && Time.time > lastSlashTime + slashCooldown)
        {
            lastSlashTime = Time.time;
            DisableAllHitboxes();
            AttackRightHitbox.enabled = true;
            animator.SetTrigger("AttackRight");
            audioSource.PlayOneShot(slashSound);
        }
        if (currentWeapon == WeaponType.Dagger && Time.time > lastDaggerTime + daggerCooldown)
        {
            lastDaggerTime = Time.time;
            ThrowDagger(Vector2.right);
        }
        else
            return;
    }


    /// <summary>
    /// Throws a dagger in the given direction, sets the velocity, and rotates the sprite accordingly.
    /// </summary>
    /// <param name="direction">The direction in which the dagger is thrown</param>
    private void ThrowDagger(Vector2 direction)
    {
        GameObject dagger = Instantiate(daggerProjectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = dagger.GetComponent<Rigidbody2D>();
        DaggerProjectile daggerScript = dagger.GetComponent<DaggerProjectile>();
        audioSource.PlayOneShot(daggerThrowSound);
        rb.velocity = direction.normalized * daggerProjectileSpeed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        dagger.transform.rotation = Quaternion.Euler(0, 0, angle);
        daggerScript.damage = daggerDamage;
    }

    
    /// <summary>
    /// Handles the dash input, checks for cooldown, and initiates the dash.
    /// </summary>
    private void OnDash(InputAction.CallbackContext context)
    {
        if (isDashing || Time.time < lastDashTime + dashCooldown)
            return;

        lastDashTime = Time.time; 

        Vector2 dashDirection = moveInput != Vector2.zero ? moveInput.normalized : lastFacedDirection;
        PerformDash(dashDirection);
    }


    /// <summary>
    /// Starts the dash process, enabling invincibility and activating the dash hitbox.
    /// </summary>
    /// <param name="dashDirection">The direction of the dash</param>
    private void PerformDash(Vector2 dashDirection)
    {
        StartCoroutine(DashCoroutine(dashDirection));
    }


    /// <summary>
    /// Coroutine for managing the dash duration, movement, and effects.
    /// </summary>
    /// <param name="dashDirection">The direction in which the player dashes</param>
    private IEnumerator DashCoroutine(Vector2 dashDirection)
    {
        isDashing = true;
        isInvincible = true;
        dashHitbox.enabled = true;
        dashTrail.emitting = true;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        float dashDuration = dashDistance / dashSpeed;
        float elapsedTime = 0f;
        Vector2 dashVelocity = dashDirection.normalized * dashSpeed;

        audioSource.PlayOneShot(dashSound);

        while (elapsedTime < dashDuration)
        {
            rb.velocity = dashVelocity;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        dashHitbox.enabled = false;
        dashTrail.emitting = false;
        isInvincible = false;
        isDashing = false;

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);

    }


    /// <summary>
    /// Disables all directional attack hitboxes.
    /// </summary>
    public void DisableAllHitboxes()
    {
        AttackRightHitbox.enabled = false;
        AttackLeftHitbox.enabled = false;
        AttackUpHitbox.enabled = false;
        AttackDownHitbox.enabled = false;
    }


    /// <summary>
    /// Handles taking damage, playing hurt animations, and triggering invincibility if not already invincible.
    /// </summary>
    /// <param name="damage">The amount of damage to take</param>
    public void TakeDamage(int damage)
    {
        if (!isAlive || isInvincible)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hurt");
            StartCoroutine(InvincibilityCoroutine());
        }
    }


    /// <summary>
    /// Coroutine for handling the player's invincibility after taking damage.
    /// </summary>
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        StartCoroutine(FlashSprite());
        yield return new WaitForSeconds(invincibilityDurationAfterHit);
        isInvincible = false;
    }


    /// <summary>
    /// Coroutine to make the sprite flash during invincibility.
    /// </summary>
    private IEnumerator FlashSprite()
    {
        while (isInvincible)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }


    /// <summary>
    /// Handles player death, disables movement, and stops all actions.
    /// </summary>
    void Die()
    {
        animator.SetTrigger("Die");
        isAlive = false;
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        this.enabled = false;
    }
}
