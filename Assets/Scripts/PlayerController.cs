using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerInputActions playerInputActions;

    private SlashAttack slashAttack;
    private DaggerAttack daggerAttack;
    private DashAttack dashAttack;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInputActions = new PlayerInputActions();

        slashAttack = GetComponent<SlashAttack>();
        daggerAttack = GetComponent<DaggerAttack>();
        dashAttack = GetComponent<DashAttack>();
    }

    void OnEnable()
    {
        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMove;

        playerInputActions.Player.PrimaryAttack.performed += ctx => slashAttack.PerformSlashAttack(GetMouseDirection());
        playerInputActions.Player.SecondaryAttack.performed += ctx => daggerAttack.PerformDaggerAttack(GetMouseDirection());
        playerInputActions.Player.SpecialAttack.performed += ctx => dashAttack.PerformDashAttack();

        playerInputActions.Player.Enable();
    }

    void OnDisable()
    {
        playerInputActions.Player.Move.performed -= OnMove;
        playerInputActions.Player.Move.canceled -= OnMove;

        playerInputActions.Player.PrimaryAttack.performed -= ctx => slashAttack.PerformSlashAttack(GetMouseDirection());
        playerInputActions.Player.SecondaryAttack.performed -= ctx => daggerAttack.PerformDaggerAttack(GetMouseDirection());
        playerInputActions.Player.SpecialAttack.performed -= ctx => dashAttack.PerformDashAttack();

        playerInputActions.Player.Disable();
    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        UpdateMovement();
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }

    void UpdateMovement()
    {
        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool("isRunning", isMoving);
        if (isMoving)
        {
            animator.SetFloat("MoveX", moveInput.x);
            animator.SetFloat("MoveY", moveInput.y);
        }
    }

    private Vector2 GetMouseDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f; 
        return (mousePos - transform.position).normalized;
    }
}
