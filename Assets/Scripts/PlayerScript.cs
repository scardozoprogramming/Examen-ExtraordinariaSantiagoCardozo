using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
[RequireComponent(typeof(PlayerInput), typeof(Collider2D))]
public class PlayerScript : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementForce = 35f;
    [SerializeField] private float maxHorizontalSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.12f;
    [SerializeField] private float groundRayInset = 0.05f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PlayerInput playerInput;
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;

    private Vector2 moveInput;
    private bool jumpRequested;
    private bool isGrounded;

    private Transform currentPlatform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        col = GetComponent<Collider2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (playerInput == null) return;

        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        if (playerInput.actions["Jump"].triggered)
            jumpRequested = true;

        UpdateVisuals();
    }

    private void FixedUpdate()
    {
        UpdateGroundCheck();
        ApplyHorizontalMovement();
        HandleJump();
        UpdateAnimatorParameters();
    }

    private void ApplyHorizontalMovement()
    {
        float xInput = moveInput.x;

        if (Mathf.Abs(xInput) > 0.01f)
        {
            rb.AddForce(Vector2.right * xInput * movementForce, ForceMode2D.Force);
        }

        // Limitar velocidad horizontal para que AddForce no acelere indefinidamente.
        float clampedX = Mathf.Clamp(rb.linearVelocity.x, -maxHorizontalSpeed, maxHorizontalSpeed);
        rb.linearVelocity = new Vector2(clampedX, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (!jumpRequested)
            return;

        if (isGrounded)
        {
            // Opcional: anular caída previa para que el salto sea consistente.
            if (rb.linearVelocity.y < 0f)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false; // bloquea doble salto inmediatamente
        }

        jumpRequested = false;
    }

    private void UpdateGroundCheck()
    {
        Bounds bounds = col.bounds;

        Vector2 leftOrigin = new Vector2(bounds.min.x + groundRayInset, bounds.min.y);
        Vector2 rightOrigin = new Vector2(bounds.max.x - groundRayInset, bounds.min.y);

        bool leftHit = Physics2D.Raycast(leftOrigin, Vector2.down, groundCheckDistance, groundLayer);
        bool rightHit = Physics2D.Raycast(rightOrigin, Vector2.down, groundCheckDistance, groundLayer);

        isGrounded = leftHit || rightHit;

        Debug.DrawRay(leftOrigin, Vector2.down * groundCheckDistance, leftHit ? Color.green : Color.red);
        Debug.DrawRay(rightOrigin, Vector2.down * groundCheckDistance, rightHit ? Color.green : Color.red);
    }

    private void UpdateVisuals()
    {
        bool running = Mathf.Abs(moveInput.x) > 0.01f;
        animator.SetBool("isRunning", running);

        if (spriteRenderer != null)
        {
            if (moveInput.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (moveInput.x < -0.01f)
                spriteRenderer.flipX = true;
        }
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("verticalSpeed", rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryAttachToPlatform(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryAttachToPlatform(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (currentPlatform != null && collision.transform == currentPlatform)
        {
            transform.SetParent(null, true);
            currentPlatform = null;
        }
    }

    private void TryAttachToPlatform(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);

            // Si la normal apunta claramente hacia arriba, estamos apoyados sobre esa superficie.
            if (contact.normal.y > 0.5f)
            {
                Rigidbody2D otherRb = collision.rigidbody;

                bool isMovingPlatform =
                    (otherRb != null && otherRb.bodyType == RigidbodyType2D.Kinematic) ||
                    SafeCompareTag(collision.collider, "MovingPlatform");

                if (isMovingPlatform)
                {
                    if (currentPlatform != collision.transform)
                    {
                        transform.SetParent(collision.transform, true);
                        currentPlatform = collision.transform;
                    }
                }

                return;
            }
        }

        if (currentPlatform != null && collision.transform == currentPlatform)
        {
            transform.SetParent(null, true);
            currentPlatform = null;
        }
    }

    // CompareTag lanza excepción si la tag no está definida; envolvemos en try/catch para evitar el error en runtime.
    private bool SafeCompareTag(Collider2D collider, string tag)
    {
        if (collider == null) return false;
        try
        {
            return collider.CompareTag(tag);
        }
        catch (UnityException)
        {
            // La tag no existe en el proyecto; evitar excepción en tiempo de ejecución.
            return false;
        }
    }
}