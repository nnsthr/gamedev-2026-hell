using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [SerializeField] private float lowJumpGravityMultiplier = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float moveInput;
    private bool jumpRequested;
    private bool jumpHeld;
    private bool isGrounded;
    private readonly ContactPoint2D[] groundContacts = new ContactPoint2D[16];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        moveInput = 0f;
        jumpHeld = false;
        if (keyboard != null)
        {
            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) moveInput -= 1f;
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) moveInput += 1f;
            if (keyboard.spaceKey.wasPressedThisFrame) jumpRequested = true;
            jumpHeld = keyboard.spaceKey.isPressed;
        }
        if (spriteRenderer != null && moveInput != 0f)
            spriteRenderer.flipX = moveInput < 0f;
    }

    private void FixedUpdate()
    {
        // Only real upward-facing contacts count: overlap queries also see the
        // underside of a one-way platform while the player passes through it.
        var groundFilter = new ContactFilter2D();
        groundFilter.SetLayerMask(groundLayer);
        groundFilter.useTriggers = false;
        int contactCount = rb.GetContacts(groundFilter, groundContacts);
        isGrounded = false;

        for (int i = 0; i < contactCount; i++)
        {
            var contact = groundContacts[i];
            if (contact.normal.y <= 0.65f) continue;
            if (rb.linearVelocity.y <= 0.1f) isGrounded = true;
        }
        float horizontalSpeed = moveInput * moveSpeed;
        rb.linearVelocity = new Vector2(horizontalSpeed,
            isGrounded ? 0f : rb.linearVelocity.y);

        if (jumpRequested && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
        jumpRequested = false;

        if (!isGrounded && rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallGravityMultiplier - 1f) * Time.fixedDeltaTime);
        }
        else if (!isGrounded && rb.linearVelocity.y > 0f && !jumpHeld)
        {
            rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (lowJumpGravityMultiplier - 1f) * Time.fixedDeltaTime);
        }

    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
